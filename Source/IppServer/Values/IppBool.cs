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

public class IppBool : IppValue<bool>
{
    public IppBool(bool value) : base(value)
    {
    }

    public static implicit operator IppBool(bool b) => new(b);

    public override void Encode(List<byte> buffer)
    {
        var bytes = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(bytes, 1);
        buffer.AddRange(bytes);

        buffer.Add((byte)(Value ? 0x01 : 0x00));
    }

    public static IppBool Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        // Skip the length, we know how big the bool will be.
        var i = buffer[offset + 2];
        offset += 3;

        return new IppBool(i > 0);
    }
}