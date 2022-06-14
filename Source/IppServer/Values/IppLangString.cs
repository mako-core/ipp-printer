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

public class IppLangString : IIppValue
{
    public string Language { get; }
    public string Value { get; }

    public IppLangString(string language, string value)
    {
        Language = language ?? throw new ArgumentNullException(nameof(language));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static IppLangString Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        offset += 2;
        var language = IppString.Decode(buffer, ref offset);
        var value = IppString.Decode(buffer, ref offset);

        return new IppLangString(language, value);
    }

    public void Encode(List<byte> buffer)
    {
        var languageEncoded = new List<byte>();
        var valueEncoded = new List<byte>();

        new IppString(Language).Encode(languageEncoded);
        new IppString(Value).Encode(valueEncoded);

        var length = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(length, (short)(languageEncoded.Count + valueEncoded.Count));
        buffer.AddRange(length);
        
        buffer.AddRange(languageEncoded);
        buffer.AddRange(valueEncoded);
    }

    public override string ToString()
    {
        return $"\"{Language}\" - \"{Value}\"";
    }
}