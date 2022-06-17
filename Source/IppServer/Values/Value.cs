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

namespace IppServer.Values;

public enum Value
{
    // Value Tags (out-of-band)
    UNSUPPORTED = 0x10,
    UNKNOWN = 0x12,
    NO_VALUE = 0x13,

    // Value Tags (integer)
    INTEGER = 0x21,
    BOOLEAN = 0x22,
    ENUM = 0x23,

    // Value Tags (octet-string)
    OCTET_STRING = 0x30, // With unspecified format
    DATE_TIME = 0x31,
    RESOLUTION = 0x32,
    RANGE_OF_INTEGER = 0x33,
    COLLECTION = 0x34,
    TEXT_WITH_LANG = 0x35,
    NAME_WITH_LANG = 0x36,

    // Value Tags (character-string)
    TEXT_WITHOUT_LANG = 0x41,
    NAME_WITHOUT_LANG = 0x42,
    KEYWORD = 0x44,
    URI = 0x45,
    URI_SCHEME = 0x46,
    CHARSET = 0x47,
    NATURAL_LANG = 0x48,
    MIME_MEDIA_TYPE = 0x49,
}