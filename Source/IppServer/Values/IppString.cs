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
using System.Text;

namespace IppServer.Values;

public class IppString : IppValue<string>
{
    public IppString(string value) : base(value)
    {
    }

    public static implicit operator IppString(string s) => new(s);

    public override void Encode(List<byte> buffer)
    {
        var utf8Bytes = Encoding.UTF8.GetBytes(Value);

        var length = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(length, (short)utf8Bytes.Length);

        buffer.AddRange(length);
        buffer.AddRange(utf8Bytes);
    }

    public static IppString Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var length = BinaryPrimitives.ReadInt16BigEndian(buffer.Slice(offset, 2));

        // Additional value fields will have a string of length zero.
        var value = length < 1 ? string.Empty : Encoding.UTF8.GetString(buffer.Slice(offset + 2, length));

        offset += length + 2;

        return new IppString(value);
    }
}