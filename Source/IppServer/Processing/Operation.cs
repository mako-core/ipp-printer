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

namespace IppServer.Processing;

public enum Operation
{
    // Operation Ids
    PRINT_JOB = 0x02,
    PRINT_URI = 0x03,
    VALIDATE_JOB = 0x04,
    CREATE_JOB = 0x05,
    SEND_DOCUMENT = 0x06,
    SEND_URI = 0x07,
    CANCEL_JOB = 0x08,
    GET_JOB_ATTRIBUTES = 0x09,
    GET_JOBS = 0x0a,
    GET_PRINTER_ATTRIBUTES = 0x0b,
    HOLD_JOB = 0x0c,
    RELEASE_JOB = 0x0d,
    RESTART_JOB = 0x0e,
    PAUSE_PRINTER = 0x10,
    RESUME_PRINTER = 0x11,
    PURGE_JOBS = 0x12,
}