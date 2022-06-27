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

using IppServer.Models;
using IppServer.Processing;
using IppServer.Values;

namespace IppServer.Operations;

internal class PrinterAttributesOperation : IOperationHandler
{
    public async Task<IppResponse> Process(IIppPrinter printer, IppRequest request)
    {
        var requestedAttributes = GetRequestedAttributes(request).ToList();
        var printerAttributes = printer.Attributes;

        if (!requestedAttributes.Any() ||
            requestedAttributes.Any(a => a.Equals("all", StringComparison.InvariantCultureIgnoreCase)))
            return await CreateAllPrinterAttributesResponse(request, printerAttributes);

        var unsupportedPrinterAttributeNames =
            requestedAttributes.Where(a => !printerAttributes.Select(attr => attr.Name).Contains(a));
        var supportedPrinterAttributes = printerAttributes.Where(a => requestedAttributes.Contains(a.Name)).ToList();

        var unsupportedGroup = new IppGroup(AttributesTag.UNSUPPORTED_ATTRIBUTES_TAG);
        foreach (var unsupportedPrinterAttributeName in unsupportedPrinterAttributeNames)
        {
            unsupportedGroup.Attributes.Add(new IppAttribute(Value.UNSUPPORTED, unsupportedPrinterAttributeName)
            {
                Values = new List<IIppValue> { (IppString)"unsupported" }
            });
        }

        var supportedGroup = new IppGroup(AttributesTag.PRINTER_ATTRIBUTES_TAG, supportedPrinterAttributes);

        var response = await IppResponse.CreateSuccessResponse(request.Id);
        response.Groups.Add(supportedGroup);
        response.Groups.Add(unsupportedGroup);

        return response;
    }

    private static async Task<IppResponse> CreateAllPrinterAttributesResponse(IppRequest request, List<IppAttribute> printerAttributes)
    {
        var supportedGroup = new IppGroup(AttributesTag.PRINTER_ATTRIBUTES_TAG, printerAttributes);

        var response = await IppResponse.CreateSuccessResponse(request.Id);
        response.Groups.Add(supportedGroup);

        return response;
    }

    private static IEnumerable<string> GetRequestedAttributes(IppRequest request)
    {
        var attributeGroup = request.Groups.FirstOrDefault(g => g.Tag == AttributesTag.OPERATION_ATTRIBUTES_TAG);
        if (attributeGroup == null)
            return new List<string>();

        var requestedAttributes = attributeGroup.Attributes
            .FirstOrDefault(a => a.Name == "requested-attributes");

        if (requestedAttributes == null)
            return new List<string>();

        var requestedAttributesNames = requestedAttributes.Values.OfType<IppString>().Select(s => (string)s);

        return requestedAttributesNames.ToList();
    }
}