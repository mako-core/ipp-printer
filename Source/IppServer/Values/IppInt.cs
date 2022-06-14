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

public class IppInt : IppValue<int>
{
    private IppInt(int value) : base(value)
    {
    }

    public static implicit operator IppInt(int i) => new(i);

    public static IppInt Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        // Skip the length, we know how big the int will be.
        var value = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset + 2, 4));
        offset += 6;

        return new IppInt(value);
    }

    public override void Encode(List<byte> buffer)
    {
        var length = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(length, 4);
        buffer.AddRange(length);

        var value = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(value, Value);
        buffer.AddRange(value);
    }
}