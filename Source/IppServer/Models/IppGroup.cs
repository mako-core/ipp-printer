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
using IppServer.Values;

namespace IppServer.Models;

public class IppGroup
{
    public IppGroup(AttributesTag tag)
    {
        Tag = tag;
    }

    public IppGroup(AttributesTag tag, IList<IppAttribute> attributes)
    {
        Tag = tag;
        Attributes = attributes;
    }

    public static IppGroup CreateOperationAttributesGroup()
    {
        var group = new IppGroup(AttributesTag.OPERATION_ATTRIBUTES_TAG, new List<IppAttribute>
        {
            new(Value.CHARSET, "attributes-charset") { Values = new List<IIppValue> {(IppString)"utf-8" }  },
            new(Value.NATURAL_LANG, "attributes-natural-language") { Values = new List<IIppValue> {(IppString)"en-us" }  }
        });

        return group;
    }

    public AttributesTag Tag { get; }

    public IList<IppAttribute> Attributes { get; } = new List<IppAttribute>();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"\tGroup tag: {Tag}");
    
        foreach (var attribute in Attributes)
        {
            stringBuilder.AppendLine(attribute.ToString());
        }
    
        return stringBuilder.ToString();
    }
}