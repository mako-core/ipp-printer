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

namespace IppServer.Values;

internal class IppResolution : IIppValue
{
    public int CrossFeedResolution { get; }
    public int FeedResolution { get; }
    public byte Units { get; }

    private IppResolution(int crossFeedResolution, int feedResolution, byte units)
    {
        CrossFeedResolution = crossFeedResolution;
        FeedResolution = feedResolution;
        Units = units;
    }

    public void Encode(List<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public static IIppValue Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        // Skip the length, we know how big the resolution will be.
        offset += 2;

        var crossFeedResolution = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset, 4));
        offset += 4;

        var feedResolution = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset, 4));
        offset += 4;

        var units = buffer[offset];
        offset++;

        return new IppResolution(crossFeedResolution, feedResolution, units);
    }
}