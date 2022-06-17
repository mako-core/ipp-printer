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

using IppServer.Extensions;
using IppServer.Models;

namespace IppServer.Processing;

class IppPrinterService : IIppPrinterService
{
    private readonly IppPrinter m_printer;

    public IppPrinterService()
    {
        m_printer = new IppPrinter("MyPrinter", 1234, "https://127.0.0.1:7234/ipp/print");
    }

    public Task<IppResponse> Process(IppRequest request)
    {
        if (request.Id == 0)
            return IppResponse.CreateErrorResponse(StatusCode.CLIENT_ERROR_BAD_REQUEST, 0);

        if (request.MajorVersion > 2)
            return IppResponse.CreateErrorResponse(StatusCode.SERVER_ERROR_VERSION_NOT_SUPPORTED, request.Id);

        switch (request.Operation)
        {
            case Operation.GET_PRINTER_ATTRIBUTES:
            {
                return m_printer.CreateSupportedAttributesResponse(request);
            }
            case Operation.PURGE_JOBS:
            {
                m_printer.ClearJobs();
                return IppResponse.CreateSuccessResponse(request.Id);
            }
            case Operation.PAUSE_PRINTER:
            {
                m_printer.Stop();
                return IppResponse.CreateSuccessResponse(request.Id);
            }
            case Operation.RESUME_PRINTER:
            {
                m_printer.Start();
                return IppResponse.CreateSuccessResponse(request.Id);
            }
            case Operation.PRINT_JOB:
            case Operation.PRINT_URI:
            case Operation.VALIDATE_JOB:
            case Operation.CREATE_JOB:
            case Operation.SEND_DOCUMENT:
            case Operation.SEND_URI:
            case Operation.CANCEL_JOB:
            case Operation.GET_JOB_ATTRIBUTES:
            case Operation.GET_JOBS:
            case Operation.HOLD_JOB:
            case Operation.RELEASE_JOB:
            case Operation.RESTART_JOB:
            default:
                return IppResponse.CreateErrorResponse(StatusCode.SERVER_ERROR_OPERATION_NOT_SUPPORTED, request.Id);
        }
    }
}