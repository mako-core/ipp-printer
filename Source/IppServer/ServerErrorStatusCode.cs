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

public enum StatusCode
{
    SUCCESSFUL_OK = 0x0000,
    SUCCESSFUL_OK_IGNORED_OR_SUBSTITUTED_ATTRIBUTES = 0x0001,
    SUCCESSFUL_OK_CONFLICTING_ATTRIBUTES = 0x0002,
    CLIENT_ERROR_BAD_REQUEST = 0x0400,
    SERVER_ERROR_INTERNAL_ERROR = 0x0500,
    SERVER_ERROR_OPERATION_NOT_SUPPORTED = 0x0501,
    SERVER_ERROR_SERVICE_UNAVAILABLE = 0x0502,
    SERVER_ERROR_VERSION_NOT_SUPPORTED = 0x0503,
    SERVER_ERROR_DEVICE_ERROR = 0x0504,
    SERVER_ERROR_TEMPORARY_ERROR = 0x0505,
    SERVER_ERROR_NOT_ACCEPTING_JOBS = 0x0506,
    SERVER_ERROR_BUSY = 0x0507,
    SERVER_ERROR_JOB_CANCELED = 0x0508,
    SERVER_ERROR_MULTIPLE_DOCUMENT_JOBS_NOT_SUPPORTED = 0x0509
}