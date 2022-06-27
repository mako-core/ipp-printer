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

using System.Text;
using IppServer.Processing;

namespace IppServer.Models;

public class IppResponse
{
    public IppResponse(StatusCode statusCode, int requestId)
    {
        StatusCode = statusCode;
        RequestId = requestId;
    }

    public static Task<IppResponse> CreateErrorResponse(StatusCode statusCode, int requestId)
    {
        var response = new IppResponse(statusCode, requestId);
        response.Groups.Add(IppGroup.CreateOperationAttributesGroup());

        return Task.FromResult(response);
    }

    public static Task<IppResponse> CreateSuccessResponse(int requestId)
    {
        var response = new IppResponse(StatusCode.SUCCESSFUL_OK, requestId);
        response.Groups.Add(IppGroup.CreateOperationAttributesGroup());

        return Task.FromResult(response);
    }

    public int MajorVersion => 1;
    public int MinorVersion => 1;
    public StatusCode StatusCode { get; }
    public int RequestId { get; }
    public List<IppGroup> Groups { get; } = new();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Version: {MajorVersion}.{MinorVersion}");
        stringBuilder.AppendLine($"StatusCode: {StatusCode}");
        stringBuilder.AppendLine($"Request ID: {RequestId}");

        foreach (var ippGroup in Groups)
        {
            stringBuilder.Append(ippGroup);
        }

        return stringBuilder.ToString();
    }
}