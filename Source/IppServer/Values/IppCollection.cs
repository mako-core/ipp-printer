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

using IppServer.Processing;

namespace IppServer.Values;

public class IppCollection : IIppValue
{
    public Dictionary<string, IIppValue> Members { get; }

    public void Encode(List<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public static IppCollection Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        // Skip the value length, it's always zero.
        offset += 2;

        var members = new Dictionary<string, IIppValue>();

        // If we've reached the end, we're done.
        while (buffer[offset] != 0x37)
        {
            // Skip the value tag and name length, they are always 0x4A and zero respectively.
            offset += 3;

            // Get the value
            string name = IppString.Decode(buffer, ref offset);

            var tag = buffer[offset++];

            // Skip the empty name length.
            offset += 2;

            var value = IppDecoder.DecodeValue(buffer, tag, ref offset);

            if (value != null)  
                members.Add(name, value);
        }

        // Skip the end tag, end name length and end value length, they are always zero.
        offset += 5;

        return new IppCollection(members);
    }

    public IppCollection(Dictionary<string, IIppValue> members)
    {
        Members = members ?? throw new ArgumentNullException(nameof(members));
    }
}