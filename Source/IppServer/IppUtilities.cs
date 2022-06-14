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
using IppServer.Values;

namespace IppServer;

public static class IppUtilities
{
    public static ReadOnlySpan<byte> EncodeResponse(IppResponse response)
    {
        var buffer = new List<byte>
        {
            (byte) response.MajorVersion,
            (byte) response.MinorVersion
        };

        var statusCode = new byte[2];
        BinaryPrimitives.TryWriteInt16BigEndian(statusCode, (short)response.StatusCode);
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

        buffer.Add((byte)AttributesTag.END_OF_ATTRIBUTES_TAG);

        return new ReadOnlySpan<byte>(buffer.ToArray());
    }

    public static IppRequest DecodeRequest(ReadOnlySpan<byte> buffer)
    {
        var offset = 0;

        var request = new IppRequest
        {
            MajorVersion = buffer[offset++],
            MinorVersion = buffer[offset++],
            Operation = (Operation)BinaryPrimitives.ReadInt16BigEndian(buffer.Slice(offset, 2))
        };

        offset += 2;

        request.Id = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset, 4));
        offset += 4;
        
        // Consume begin attribute group tag.
        var tag = (int)buffer[offset++];

        while (tag != (int)AttributesTag.END_OF_ATTRIBUTES_TAG && offset < buffer.Length)
        {
            var group = new IppGroup((AttributesTag) tag);

            // This is either a value, an additional value or the a delimiter.
            tag = buffer[offset++];

            IppAttribute? currentAttribute = null;

            // Ensure it's not a delimiter and it's a value tag instead.
            while (tag > 0x0F)
            {
                var name = IppString.Decode(buffer, ref offset);

                // If there's no name, it means it's an additional value, so add it to the last attribute.
                // Otherwise, create a new attribute add add values to it.
                if (!string.IsNullOrEmpty(name))
                {
                    currentAttribute = new IppAttribute((Tag) tag, name);

                    group.Attributes.Add(currentAttribute);
                }

                var attribute = DecodeValue(buffer, tag, ref offset);

                if (attribute != null)
                    currentAttribute?.Values.Add(attribute);

                tag = buffer[offset++];
            }

            request.Groups.Add(group);
        }

        return request;
    }

    internal static IIppValue? DecodeValue(ReadOnlySpan<byte> buffer, int tag, ref int offset)
    {
        switch ((Tag) tag)
        {
            case Tag.INTEGER:
                return IppInt.Decode(buffer, ref offset);
            case Tag.BOOLEAN:
                return IppBool.Decode(buffer, ref offset);
            case Tag.ENUM:
                return IppEnum.Decode(buffer, ref offset);
            case Tag.DATE_TIME:
                return IppDateTime.Decode(buffer, ref offset);
            case Tag.TEXT_WITH_LANG:
            case Tag.NAME_WITH_LANG:
                return IppLangString.Decode(buffer, ref offset);
            case Tag.RANGE_OF_INTEGER:
                return IppIntegerRange.Decode(buffer, ref offset);
            case Tag.COLLECTION:
                return IppCollection.Decode(buffer, ref offset);
            case Tag.RESOLUTION:
                return IppResolution.Decode(buffer, ref offset);
            case Tag.CHARSET:
            case Tag.NATURAL_LANG:
            case Tag.KEYWORD:
            case Tag.URI:
            case Tag.URI_SCHEME:
            case Tag.OCTET_STRING:
            case Tag.MIME_MEDIA_TYPE:
            case Tag.TEXT_WITHOUT_LANG:
            case Tag.NAME_WITHOUT_LANG:
            case Tag.UNSUPPORTED:
                return IppString.Decode(buffer, ref offset);
            case Tag.NO_VALUE:
                // No value appears to have two null bytes. Skip past them.
                offset += 2;
                return null;
            case Tag.UNKNOWN:
                // No value appears to have two null bytes. Skip past them.
                offset += 2;
                return null;
            default:
                throw new InvalidOperationException($"Unsupported tag type '{tag}' found.");
        }
    }
}

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

            var value = IppUtilities.DecodeValue(buffer, tag, ref offset);

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

public class IppIntegerRange : IIppValue
{
    public void Encode(List<byte> buffer)
    {
        var length = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(length, 8);
        buffer.AddRange(length);

        var lowerBytes = new byte[4];
        var upperBytes = new byte[4];

        BinaryPrimitives.WriteInt32BigEndian(lowerBytes, Lower);
        BinaryPrimitives.WriteInt32BigEndian(lowerBytes, Upper);

        buffer.AddRange(lowerBytes);
        buffer.AddRange(upperBytes);
    }

    public static IppIntegerRange Decode(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var lower = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset + 2, 4));
        var upper = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset + 6, 4));

        offset += 10;

        return new IppIntegerRange(lower, upper);
    }

    public IppIntegerRange(int lower, int upper)
    {
        Lower = lower;
        Upper = upper;
    }

    public int Lower { get; }
    public int Upper { get; }
}