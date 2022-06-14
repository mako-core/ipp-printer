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

using IppServer.Values;

namespace IppServer;

class IppPrinterService : IIppPrinterService
{
    private readonly IppPrinter m_printer;

    public IppPrinterService()
    {
        m_printer = new IppPrinter("MyPrinter", 1234, "ipp:/something/else");
    }

    public Task<IppResponse> Process(IppRequest request)
    {
        if (request.Id == 0)
            return CreateErrorResponse(StatusCode.CLIENT_ERROR_BAD_REQUEST, 0);

        if (request.MajorVersion > 2)
            return CreateErrorResponse(StatusCode.SERVER_ERROR_VERSION_NOT_SUPPORTED, request.Id);

        switch (request.Operation)
        {
            case Operation.PRINT_JOB:
            case Operation.PRINT_URI:
            case Operation.VALIDATE_JOB:
            case Operation.CREATE_JOB:
            case Operation.SEND_DOCUMENT:
            case Operation.SEND_URI:
            case Operation.CANCEL_JOB:
            case Operation.GET_JOB_ATTRIBUTES:
            case Operation.GET_JOBS:
            case Operation.GET_PRINTER_ATTRIBUTES:
                return CreatePrinterAttributesResponse(request);
            case Operation.HOLD_JOB:
            case Operation.RELEASE_JOB:
            case Operation.RESTART_JOB:
            case Operation.PAUSE_PRINTER:
            case Operation.RESUME_PRINTER:
            case Operation.PURGE_JOBS:
            default:
                return CreateErrorResponse(StatusCode.SERVER_ERROR_OPERATION_NOT_SUPPORTED, request.Id);
        }
    }

    private Task<IppResponse> CreatePrinterAttributesResponse(IppRequest request)
    {
        var requestedAttributes = GetRequestedAttributes(request).ToList();
        var printerAttributes = m_printer.Attributes;

        if (!requestedAttributes.Any() || requestedAttributes.Any(a => a.Equals("all", StringComparison.InvariantCultureIgnoreCase)))
            return CreateAllPrinterAttributesResponse(request, printerAttributes);
        
        var unsupportedPrinterAttributeNames = requestedAttributes.Where(a => !printerAttributes.Select(attr => attr.Name).Contains(a));
        var supportedPrinterAttributes = printerAttributes.Where(a => requestedAttributes.Contains(a.Name)).ToList();

        var unsupportedGroup = new IppGroup(AttributesTag.UNSUPPORTED_ATTRIBUTES_TAG);
        foreach (var unsupportedPrinterAttributeName in unsupportedPrinterAttributeNames)
        {
            unsupportedGroup.Attributes.Add(new IppAttribute(Tag.UNSUPPORTED, unsupportedPrinterAttributeName)
            {
                Values = new List<IIppValue> {(IppString) "unsupported"}
            });
        }

        var supportedGroup = new IppGroup(AttributesTag.PRINTER_ATTRIBUTES_TAG, supportedPrinterAttributes);
        var operationAttributesGroup = CreateOperationAttributesGroup();

        var response = new IppResponse((short) StatusCode.SUCCESSFUL_OK, request.Id);
        response.Groups.Add(operationAttributesGroup);
        response.Groups.Add(supportedGroup);
        response.Groups.Add(unsupportedGroup);

        return Task.FromResult(response);
    }

    private Task<IppResponse> CreateAllPrinterAttributesResponse(IppRequest request, List<IppAttribute> printerAttributes)
    {
        var supportedGroup = new IppGroup(AttributesTag.PRINTER_ATTRIBUTES_TAG, printerAttributes);
        var operationAttributesGroup = CreateOperationAttributesGroup();

        var response = new IppResponse((short)StatusCode.SUCCESSFUL_OK, request.Id);

        response.Groups.Add(operationAttributesGroup);
        response.Groups.Add(supportedGroup);

        return Task.FromResult(response);
    }

    private IEnumerable<string> GetRequestedAttributes(IppRequest request)
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

    private Task<IppResponse> CreateErrorResponse(StatusCode statusCode, int requestId)
    {
        var response = new IppResponse(statusCode, requestId);
        response.Groups.Add(CreateOperationAttributesGroup());

        return Task.FromResult(response);
    }

    private IppGroup CreateOperationAttributesGroup()
    {
        var group = new IppGroup(AttributesTag.OPERATION_ATTRIBUTES_TAG, new List<IppAttribute>
        {
            new(Tag.CHARSET, "attributes-charset") { Values = new List<IIppValue> {(IppString)"utf-8" }  },
            new(Tag.NATURAL_LANG, "attributes-natural-language") { Values = new List<IIppValue> {(IppString)"en-us" }  }
        });

        return group;
    }
}