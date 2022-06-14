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

using System.IO;

namespace IppServer.Tests.Extensions;

public static class DirectoryInfoExtensions
{
    public static FileInfo CreateFileInfo(this DirectoryInfo directoryInfo, string leafName, bool autoCreateDirectory = true)
    {
        if (autoCreateDirectory)
            directoryInfo.Create();
        return new FileInfo(Path.Combine(directoryInfo.FullName, leafName));
    }

    public static bool ContainsDirectory(this DirectoryInfo directoryInfo, string leafName) => Directory.Exists(Path.Combine(directoryInfo.FullName, leafName));

    /// <summary>
    /// Create a reference to a child directory, without actually creating it on disk.
    /// </summary>
    public static DirectoryInfo SubDirectory(this DirectoryInfo directoryInfo, string leafName) => new DirectoryInfo(Path.Combine(directoryInfo.FullName, leafName));

    /// <summary>
    /// Create a reference to a child directory, creating it on disk if necessary.
    /// </summary>
    public static DirectoryInfo EnsureSubDirectory(this DirectoryInfo directoryInfo, string leafName)
    {
        var subFolder = SubDirectory(directoryInfo, leafName);
        subFolder.Create();
        return subFolder;
    }
}