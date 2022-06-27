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

internal class PrintJobOperation : IOperationHandler
{
    private static int m_jobId = 1;

    public async Task<IppResponse> Process(IIppPrinter printer, IppRequest request)
    {
        var operationsAttributes = request.Groups.Single(g => g.Tag == AttributesTag.OPERATION_ATTRIBUTES_TAG).Attributes;

        var jobName = $"Job {m_jobId++}";

        var jobNameAttribute = operationsAttributes.FirstOrDefault(a => a.Name == "job-name");
        if (jobNameAttribute != null)
        {
            if (jobNameAttribute.Values.FirstOrDefault() is IppString jobNameValue)
                jobName = jobNameValue;
        }
        
        var documentFormatAttribute = operationsAttributes.FirstOrDefault(a => a.Name == "document-format");
        if (documentFormatAttribute != null)
        {
            if (documentFormatAttribute.Values.FirstOrDefault() is IppString documentFormat)
            {
                // Check if the document format supplied is supported.
                if (printer.SupportedDocumentFormats.All(f => f != documentFormat))
                    return await IppResponse.CreateErrorResponse(StatusCode.CLIENT_ERROR_BAD_REQUEST, request.Id);
            }
        }

        var job = new IppJob(jobName);
        printer.AddJob(job);

        var response = await IppResponse.CreateSuccessResponse(request.Id);

        var jobAttributes = new IppGroup(AttributesTag.JOB_ATTRIBUTES_TAG, new List<IppAttribute>
        {
            new(Value.URI, "job-uri"){ Values = {(IppString)$"{printer.Uri}/{m_jobId}"}},
            new(Value.INTEGER, "job-id"){ Values = {(IppInt)m_jobId}},

            // TODO: Lookup and provide correct return values (IppEnum).
            new(Value.ENUM, "job-state"){ Values = {(IppString)"pending"}},
            new(Value.KEYWORD, "job-state-reasons"){ Values = {(IppString)"pending"}}
        });

        response.Groups.Add(jobAttributes);

        return response;
    }
}