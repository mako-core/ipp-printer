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
using System.IO;
using System.Linq;
using IppServer.Tests.Extensions;
using NUnit.Framework;

namespace IppServer.Tests;

public static class PathHelper
{
    public static DirectoryInfo GetSourceDirectory()
    {
        var directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directoryInfo != null && !directoryInfo.GetFiles("*.sln").Any())
            directoryInfo = directoryInfo.Parent;

        Assert.IsNotNull(directoryInfo);
        return directoryInfo!;
    }

    public static FileInfo GetTestFile(string leafName) => TestPath.CreateFileInfo(leafName);

    public static DirectoryInfo GetTestDirectory(string leafName) => TestPath.CreateSubdirectory(leafName);

    public static DirectoryInfo TestPath
    {
        get
        {
            var sourceDirectory = GetSourceDirectory();

            if (sourceDirectory.ContainsDirectory("UnitTests"))
                return sourceDirectory.EnsureSubDirectory("UnitTests").EnsureSubDirectory("TestFiles");

            return sourceDirectory.EnsureSubDirectory("CSharp.UnitTests").EnsureSubDirectory("TestFiles");
        }
    }
}