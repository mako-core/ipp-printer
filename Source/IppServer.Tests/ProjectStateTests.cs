// -----------------------------------------------------------------------
//  MIT License
//  
//  Copyright (c) 2022 Global Graphics Software Ltd.
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IppServer.Tests.Extensions;
using NUnit.Framework;

namespace IppServer.Tests;

[TestFixture]
public class ProjectStateTests
{
    protected virtual IEnumerable<string> AdditionalFilesToSkip { get; } = new List<string>();
    private string ExpectedVersion => "1.0.0.0";

    [Test]
    public void CheckProjectAssemblyInfo()
    {
        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", "resources.designer.cs" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        Assert.Multiple(() =>
        {
            foreach (var file in PathHelper.GetSourceDirectory().EnumerateFiles("AssemblyInfo.cs", SearchOption.AllDirectories))
            {
                if (filesToSkip.Any(o => file.FullName.Contains(o, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                if (IsUnderVsPackagesFolder(file.Directory!))
                    continue;

                var fileText = file.ReadAllText();

                Assert.That(Regex.IsMatch(fileText, "\\[assembly: AssemblyCopyright\\(\"Copyright © 20\\d\\d Global Graphics Software Ltd\\.\"\\)\\]"), Is.True, $"{file.FullName} : Missing/bad Copyright");
                Assert.That(fileText.Contains("[assembly: AssemblyCompany(\"Global Graphics Software\")]"), Is.True, $"{file.FullName} : Missing/bad AssemblyCompany");
                Assert.That(fileText.Contains($"[assembly: AssemblyVersion(\"{ExpectedVersion}\")]"), Is.True, $"{file.FullName} : Missing/bad AssemblyVersion");
                Assert.That(fileText.Contains($"[assembly: AssemblyFileVersion(\"{ExpectedVersion}\")]"), Is.True, $"{file.FullName} : Missing/bad AssemblyFileVersion");
            }
        });
    }

    [Test]
    public void CheckSdkProjectInfo()
    {
        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", "resources.designer.cs" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        Assert.Multiple(() =>
        {
            foreach (var file in PathHelper.GetSourceDirectory().EnumerateFiles("*.csproj", SearchOption.AllDirectories))
            {
                if (filesToSkip.Any(o => file.FullName.Contains(o, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                if (IsUnderVsPackagesFolder(file.Directory!))
                    continue;

                var fileText = file.ReadAllText();
                if (!fileText.Contains("<Project Sdk=\"Microsoft.NET.Sdk"))
                    continue;

                // Check if we're still using a manually created assembly info file.
                if (fileText.Contains("<GenerateAssemblyInfo>false</GenerateAssemblyInfo>"))
                    continue;

                Assert.That(Regex.IsMatch(fileText, @"<Copyright>Copyright © 20\d\d Global Graphics Software Ltd.<"), Is.True, $"{file.FullName} : Missing/bad Copyright");
                Assert.That(fileText.Contains("<Company>Global Graphics Software<"), Is.True, $"{file.FullName} : Missing/bad Company");
                Assert.That(fileText.Contains($"<Version>{ExpectedVersion}<"), Is.True, $"{file.FullName} : Missing/bad Version");
            }
        });
    }

    [Test]
    public void CheckForCodeCopyright()
    {
        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", "resources.ja-jp.designer.cs", "resources.designer.cs", "assemblyinfo.cs", "resource.h", "packages" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        var sourceFiles = new[] { "*.cs", "*.cpp", "*.h", "*.c" }.SelectMany(ext => PathHelper.GetSourceDirectory().EnumerateFiles(ext, SearchOption.AllDirectories));

        var builder = new StringBuilder();

        foreach (var file in sourceFiles)
        {
            if (filesToSkip.Any(o => file.FullName.Contains(o, StringComparison.InvariantCultureIgnoreCase)))
                continue;

            if (IsUnderVsPackagesFolder(file.Directory!))
                continue;

            var fileText = file.ReadAllText();
            if (fileText.Contains("This code was generated by a tool."))
                continue;

            if (!Regex.IsMatch(fileText, @"Copyright \(c\) 20\d\d Global Graphics Software Ltd\."))
                builder.AppendLine($"{file.FullName} No code copyright detected");
        }

        if (builder.Length > 0)
            Assert.Fail(builder.ToString());
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected static readonly IEnumerable<FileInfo> AllProjects = PathHelper.GetSourceDirectory().EnumerateFiles("*.csproj", SearchOption.AllDirectories);

    [Test, Sequential]
    public void CheckCSharpProjectSettings([ValueSource(nameof(AllProjects))] FileInfo project)
    {
        var text = project.ReadAllText();

        if (!text.Contains("<TreatWarningsAsErrors>true"))
            Assert.Fail($"{project.FullName}: 'TreatWarningsAsErrors' is not enabled.");
    }

    [Test]
    public void CheckForCommentedOutCode()
    {
        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", "resources.designer.cs", "assemblyinfo.cs" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        var sourceFiles = new[] { "*.cs", "*.cpp", "*.h", "*.c" }.SelectMany
            (ext => PathHelper.GetSourceDirectory().EnumerateFiles(ext, SearchOption.AllDirectories));
        var builder = new StringBuilder();

        foreach (var file in sourceFiles)
        {
            if (filesToSkip.Any(o => file.FullName.ToLower().Contains(o)))
                continue;
            if (IsUnderVsPackagesFolder(file.Directory!))
                continue;

            var fileText = file.ReadAllText();
            if (Regex.IsMatch(fileText, @"^\s*(\/\/|\/\*).*(\(\)|;)\s*$", RegexOptions.Multiline))
                builder.AppendLine($"{file.FullName}: contains code that has been commented out.");
        }

        Assert.That(builder.Length == 0, Is.True, builder.ToString());
    }

    [Test]
    public void CheckForUndesiredExpressions()
    {
        var undesiredWords = new[]
        {
                "shiny", "pony", @"[/|\*]*\s*todo:*", "hack", "fixme", "fix me", "stupid", "idiot", "not sure",
                "magic", "meh", "wobbly", "refactor", "foobar", "seems* to", "evil", "i don'*t know", "yuck", "somehow"
            }.Select(o => $"\\b{o}\\b");
        var undesiredExpressions = undesiredWords.Union(
                                                        new[]
                                                        {
                                                                @"\r\n *\r\n *\{",
                                                                @"\r\n *\r\n *\}",
                                                                @"\t"
                                                        }).ToList();

        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", "resources.designer.cs", "assemblyinfo.cs", "projectstatetests.cs", "uitestbase.cs", "projectstatetestbase.cs" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        var sourceFiles = new[] { "*.cs", "*.cpp", "*.h", "*.c", "*.xaml", "*.xaml.cs", "resources.resx" }.SelectMany
            (ext => PathHelper.GetSourceDirectory().EnumerateFiles(ext, SearchOption.AllDirectories));
        var builder = new StringBuilder();

        foreach (var file in sourceFiles)
        {
            if (filesToSkip.Any(o => file.FullName.ToLower().Contains(o)))
                continue;
            if (IsUnderVsPackagesFolder(file.Directory!))
                continue;

            var fileText = file.ReadAllText();
            foreach (var match in undesiredExpressions.Select(o => new Regex(o, RegexOptions.IgnoreCase).Match(fileText)).Where(o => o.Success))
            {
                var lineIndex = fileText.Substring(0, match.Index).IndicesOf('\r').Count();
                builder.AppendLine($"{file.FullName}:{lineIndex + 1} contains an undesired expression matching the regex '{match}'.");
            }
        }

        Assert.That(builder.Length == 0, Is.True, builder.ToString());
    }

    [Test]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void CheckForBritishEnglishSpelling()
    {
        string[] britishEnglishWords =
        {
                "normalis", "linearis", "optimis", "rasteris", "initialis", "posteris", "minimis", "recognis", "synchronis", "serialis", "centralis",
                "colour", "behaviour", "neighbour", "analogu", "authoris", "metre", "finalis"
            };

        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", "resources.designer.cs", "assemblyinfo.cs", "projectstatetests.cs", "harlequindirect_api.h" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        var sourceFiles = new[] { "*.cs", "*.cpp", "*.h", "*.c", "*.xaml", "*.xaml.cs", "resources.resx" }.SelectMany
            (ext => PathHelper.GetSourceDirectory().EnumerateFiles(ext, SearchOption.AllDirectories));
        var builder = new StringBuilder();

        foreach (var file in sourceFiles)
        {
            if (filesToSkip.Any(o => file.FullName.ToLower().Contains(o)))
                continue;
            if (IsUnderVsPackagesFolder(file.Directory!))
                continue;
            if (file.Name == "ProjectStateTestBase.cs")
                continue;

            var fileText = file.ReadAllText();
            foreach (var word in britishEnglishWords)
            {
                if (Regex.IsMatch(fileText, word, RegexOptions.IgnoreCase))
                    builder.AppendLine($"{file.FullName}: contains an undesired word matching the regex '{word}'.");
            }
        }

        Assert.That(builder.Length == 0, Is.True, builder.ToString());
    }

    [Test]
    public void CheckForDoubleNewLines()
    {
        var filesToSkip = new[] { "\\.vs\\", "\\obj\\", "resharper", "\\bin\\", ".designer.cs", "assemblyinfo.cs" };
        filesToSkip = filesToSkip.Union(AdditionalFilesToSkip).ToArray();

        var sourceFiles = new[] { "*.cs", "*.cpp", "*.h", "*.c", "*.xaml", "resources.resx" }.SelectMany
            (ext => PathHelper.GetSourceDirectory().EnumerateFiles(ext, SearchOption.AllDirectories));

        var builder = new StringBuilder();
        foreach (var fileInfo in sourceFiles)
        {
            if (filesToSkip.Any(o => fileInfo.FullName.Contains(o, StringComparison.OrdinalIgnoreCase)))
                continue;
            if (IsUnderVsPackagesFolder(fileInfo.Directory!))
                continue;

            var allLines = fileInfo.ReadAllLines();
            var lineIndices = Enumerable.Range(0, allLines.Count - 1).Where(i => string.IsNullOrWhiteSpace(allLines[i]) && string.IsNullOrWhiteSpace(allLines[i + 1]));
            foreach (var lineIndex in lineIndices)
                builder.AppendLine($"{fileInfo.FullName}:{lineIndex + 1} contains two consecutive new lines.");
        }

        if (builder.Length > 0)
            Assert.Fail(builder.ToString());
    }

    private static bool IsUnderVsPackagesFolder(DirectoryInfo folder)
    {
        if (!folder.FullName.Contains("packages", StringComparison.OrdinalIgnoreCase))
            return false;

        var packagesFolderFound = false;
        var packagesFolder = folder;
        while (packagesFolder?.Parent != null)
        {
            if (packagesFolder.Name.Equals("packages", StringComparison.OrdinalIgnoreCase) && packagesFolder.Parent.GetFiles("*.sln").Any())
            {
                packagesFolderFound = true;
                break;
            }

            packagesFolder = packagesFolder.Parent;
        }

        return packagesFolderFound && folder.FullName.StartsWith(packagesFolder!.FullName);
    }

    [Test]
    public void CheckUiStringsAreLocalizable()
    {
        var sourceFiles = PathHelper.GetSourceDirectory().EnumerateFiles("*.xaml", SearchOption.AllDirectories);

        var regex = new Regex("(\\s+Text|\\s+Header|\\s+Content|\\.Hint|ToolTip)\\s*=\"[a-zA-Z].+\"");
        var builder = new StringBuilder();
        foreach (var file in sourceFiles.Where(o => !GetAllowHardCodedUiString(o)))
        {
            var fileText = file.ReadAllLines();

            foreach (var lineIndex in Enumerable.Range(0, fileText.Count).Where(i => regex.IsMatch(fileText[i])))
                builder.AppendLine($"{file.FullName}:{lineIndex + 1} contains a hard-coded UI string.");
        }

        if (builder.Length > 0) 
            Assert.Fail(builder.ToString());
    }

    /// <summary>
    /// Implementers should override to allow XAML files containing hard-coded UI strings.
    /// </summary>
    protected virtual bool GetAllowHardCodedUiString(FileInfo xamlFile) => false;
}