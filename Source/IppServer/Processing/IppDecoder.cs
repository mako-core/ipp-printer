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

public class IppDecoder
{
    public static IppRequest Decode(ReadOnlySpan<byte> requestBuffer)
    {
        var offset = 0;

        var majorVersion = requestBuffer[offset++];
        var minorVersion = requestBuffer[offset++];

        var operation = (Operation) BinaryPrimitives.ReadInt16BigEndian(requestBuffer.Slice(offset, 2));
        offset += 2;

        var requestId = BinaryPrimitives.ReadInt32BigEndian(requestBuffer.Slice(offset, 4));
        offset += 4;

        var request = new IppRequest(majorVersion, minorVersion, operation, requestId);

        // Consume begin attribute group tag.
        var tag = (int)requestBuffer[offset++];

        while (tag != (int)AttributesTag.END_OF_ATTRIBUTES_TAG && offset < requestBuffer.Length)
        {
            var group = new IppGroup((AttributesTag) tag);

            // This is either a value, an additional value or the a delimiter.
            tag = requestBuffer[offset++];

            IppAttribute? currentAttribute = null;

            // Ensure it's not a delimiter and it's a value tag instead.
            while (tag > 0x0F)
            {
                var name = IppString.Decode(requestBuffer, ref offset);

                // If there's no name, it means it's an additional value, so add it to the last attribute.
                // Otherwise, create a new attribute add add values to it.
                if (!string.IsNullOrEmpty(name))
                {
                    currentAttribute = new IppAttribute((Value) tag, name);

                    group.Attributes.Add(currentAttribute);
                }

                var attribute = DecodeValue(requestBuffer, tag, ref offset);

                if (attribute != null)
                    currentAttribute?.Values.Add(attribute);

                tag = requestBuffer[offset++];
            }

            request.Groups.Add(group);
        }

        return request;
    }

    internal static IIppValue? DecodeValue(ReadOnlySpan<byte> buffer, int tag, ref int offset)
    {
        switch ((Value) tag)
        {
            case Value.INTEGER:
                return IppInt.Decode(buffer, ref offset);
            case Value.BOOLEAN:
                return IppBool.Decode(buffer, ref offset);
            case Value.ENUM:
                return IppEnum.Decode(buffer, ref offset);
            case Value.DATE_TIME:
                return IppDateTime.Decode(buffer, ref offset);
            case Value.TEXT_WITH_LANG:
            case Value.NAME_WITH_LANG:
                return IppLangString.Decode(buffer, ref offset);
            case Value.RANGE_OF_INTEGER:
                return IppIntegerRange.Decode(buffer, ref offset);
            case Value.COLLECTION:
                return IppCollection.Decode(buffer, ref offset);
            case Value.RESOLUTION:
                return IppResolution.Decode(buffer, ref offset);
            case Value.CHARSET:
            case Value.NATURAL_LANG:
            case Value.KEYWORD:
            case Value.URI:
            case Value.URI_SCHEME:
            case Value.OCTET_STRING:
            case Value.MIME_MEDIA_TYPE:
            case Value.TEXT_WITHOUT_LANG:
            case Value.NAME_WITHOUT_LANG:
            case Value.UNSUPPORTED:
                return IppString.Decode(buffer, ref offset);
            case Value.NO_VALUE:
                // No value appears to have two null bytes. Skip past them.
                offset += 2;
                return null;
            case Value.UNKNOWN:
                // No value appears to have two null bytes. Skip past them.
                offset += 2;
                return null;
            default:
                throw new InvalidOperationException($"Unsupported tag type '{tag}' found.");
        }
    }
}