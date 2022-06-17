using IppServer.Models;
using IppServer.Values;

namespace IppServer.Extensions;

public static class PrinterAttributesResponseExtensions
{
    public static async Task<IppResponse> CreateSupportedAttributesResponse(this IppPrinter printer, IppRequest request)
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