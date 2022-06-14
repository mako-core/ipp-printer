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

namespace IppServer;

public static class StatusCodes
{
    private static readonly Dictionary<StatusCode, string> StatusCodeMap = new()
    {
        {StatusCode.SUCCESSFUL_OK, "successful-ok"},
        {StatusCode.SUCCESSFUL_OK_CONFLICTING_ATTRIBUTES, "successful-ok-ignored-or-substituted-attributes"},
        {StatusCode.SUCCESSFUL_OK_IGNORED_OR_SUBSTITUTED_ATTRIBUTES, "successful-ok-conflicting-attributes"},
        {StatusCode.CLIENT_ERROR_BAD_REQUEST, "client-error-bad-request"},
        {StatusCode.SERVER_ERROR_INTERNAL_ERROR, "server-error-internal-error"},
        {StatusCode.SERVER_ERROR_OPERATION_NOT_SUPPORTED, "server-error-operation-not-supported"},
        {StatusCode.SERVER_ERROR_SERVICE_UNAVAILABLE, "server-error-service-unavailable"},
        {StatusCode.SERVER_ERROR_VERSION_NOT_SUPPORTED, "server-error-version-not-supported"},
        {StatusCode.SERVER_ERROR_DEVICE_ERROR, "server-error-device-error"},
        {StatusCode.SERVER_ERROR_TEMPORARY_ERROR, "server-error-temporary-error"},
        {StatusCode.SERVER_ERROR_NOT_ACCEPTING_JOBS, "server-error-not-accepting-jobs"},
        {StatusCode.SERVER_ERROR_BUSY, "server-error-busy"},
        {StatusCode.SERVER_ERROR_JOB_CANCELED, "server-error-job-canceled"},
        {StatusCode.SERVER_ERROR_MULTIPLE_DOCUMENT_JOBS_NOT_SUPPORTED, "server-error-multiple-document-jobs-not-supported"}
    };

    public static string GetMessage(StatusCode code)
    {
        return StatusCodeMap[code];
    }
}