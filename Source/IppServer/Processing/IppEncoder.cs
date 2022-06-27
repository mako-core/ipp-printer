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

using System.Buffers.Binary;
using IppServer.Models;
using IppServer.Values;

namespace IppServer.Processing;

public static class IppEncoder
{
    public static ReadOnlySpan<byte> Encode(IppResponse response)
    {
        var buffer = new List<byte>
        {
            (byte) response.MajorVersion,
            (byte) response.MinorVersion
        };

        var statusCode = new byte[2];
        BinaryPrimitives.TryWriteInt16BigEndian(statusCode, (short) response.StatusCode);
        buffer.AddRange(statusCode);

        var requestId = new byte[4];
        BinaryPrimitives.TryWriteInt32BigEndian(requestId, response.RequestId);
        buffer.AddRange(requestId);

        foreach (var group in response.Groups)
        {
            buffer.Add((byte) group.Tag);

            foreach (var attribute in group.Attributes)
            {
                var firstAttribute = true;
                foreach (var value in attribute.Values)
                {
                    buffer.Add((byte) attribute.Tag);

                    new IppString(firstAttribute ? attribute.Name : string.Empty).Encode(buffer);
                    firstAttribute = false;

                    value.Encode(buffer);
                }
            }
        }

        buffer.Add((byte) AttributesTag.END_OF_ATTRIBUTES_TAG);

        return new ReadOnlySpan<byte>(buffer.ToArray());
    }
}