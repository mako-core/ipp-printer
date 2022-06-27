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

using IppServer.Processing;
using IppServer.Values;

namespace IppServer.Models;

public class IppPrinter : IIppPrinter
{
    private readonly List<IIppJob> m_jobs = new();

    public IppPrinter(string name, ushort port, string uri)
    {
        Name = name;
        Port = port;
        Uri = uri;
    }

    public string Name { get; }

    public ushort Port { get; }

    public string Uri { get; }

    public PrinterState State { get; private set; } = PrinterState.PRINTER_STOPPED;

    public DateTime Started { get; } = DateTime.UtcNow;

    public IReadOnlyList<IIppJob> Jobs => m_jobs;

    public List<string> SupportedDocumentFormats { get; } = new() {"application/pdf"};

    public string DefaultDocumentFormat { get; } = "application/pdf";

    public void Start()
    {
        State = PrinterState.PRINTER_IDLE;
        Console.WriteLine($"Printer '{Name}' started.");
    }

    public void Stop()
    {
        State = PrinterState.PRINTER_STOPPED;
        Console.WriteLine($"Printer '{Name}' started.");
    }

    public void AddJob(IIppJob job)
    {
        m_jobs.Add(job);
        Console.WriteLine($"Printer '{Name}' started.");
    }

    public List<IppAttribute> Attributes => new()
    {
        new(Value.NAME_WITHOUT_LANG, "printer-name") { Values = new List<IIppValue>{new IppString(Name)} },
        new(Value.TEXT_WITHOUT_LANG, "printer-info") { Values = new List<IIppValue>{(IppString)"This is a virtual IPP printer."} },
        new(Value.TEXT_WITHOUT_LANG, "printer-make-and-model") {Values = new List<IIppValue>{(IppString)"Manufacturer - Model"} },
        new(Value.TEXT_WITHOUT_LANG, "printer-device-id") {Values = new List<IIppValue>{(IppString)"MyDeviceID"} },
        new(Value.URI, "printer-uuid") {Values = new List<IIppValue>{(IppString)"urn:printer:uuid:123456" } },
        new(Value.URI, "printer-uri-supported") {Values = new List<IIppValue>{ (IppString)Uri } },
        new(Value.URI, "printer-uri") {Values = new List<IIppValue>{ (IppString)Uri} },
        new(Value.TEXT_WITHOUT_LANG, "printer-location") {Values = new List<IIppValue>{ (IppString)string.Empty } },
        new(Value.KEYWORD, "media-supported") {Values = new List<IIppValue>{ (IppString) "na_letter_8.5x11in", (IppString) "na_legal_8.5x14in", (IppString) "iso_a4_210x297mm" } },
        new(Value.KEYWORD, "media-type-supported") {Values = new List<IIppValue>{ (IppString) "auto" } },
        new(Value.KEYWORD, "printer-kind") {Values = new List<IIppValue>{ (IppString)"document" } },
        new(Value.KEYWORD, "uri-security-supported") {Values = new List<IIppValue>{(IppString)"tls" } },
        new(Value.KEYWORD, "uri-authentication-supported") {Values = new List<IIppValue>{(IppString)"none"} },
        new(Value.ENUM, "printer-state") {Values = new List<IIppValue>{(IppEnum)(int)State} },
        new(Value.TEXT_WITHOUT_LANG, "printer-state-message") {Values = new List<IIppValue>{(IppString)"Idle."} },
        new(Value.KEYWORD, "printer-state-reasons") {Values = new List<IIppValue>{(IppString)"none"} },
        new(Value.KEYWORD, "ipp-versions-supported") {Values = new List<IIppValue>{ (IppString)"1.0", (IppString)"1.1", (IppString)"2.0"} },
        new(Value.ENUM, "operations-supported") {Values = new List<IIppValue>
            {
                (IppEnum)(int)Operation.PRINT_JOB, 
                (IppEnum)(int)Operation.PRINT_URI, 
                (IppEnum)(int)Operation.VALIDATE_JOB, 
                (IppEnum)(int)Operation.CREATE_JOB, 
                (IppEnum)(int)Operation.SEND_DOCUMENT,
                (IppEnum)(int)Operation.CANCEL_JOB, 
                (IppEnum)(int)Operation.GET_JOB_ATTRIBUTES, 
                (IppEnum)(int)Operation.GET_JOBS, 
                (IppEnum)(int)Operation.GET_PRINTER_ATTRIBUTES, 
                (IppEnum)(int)Operation.HOLD_JOB, 
                (IppEnum)(int)Operation.RELEASE_JOB, 
                (IppEnum)(int)Operation.RESTART_JOB, 
                (IppEnum)(int)Operation.PAUSE_PRINTER, 
                (IppEnum)(int)Operation.RESUME_PRINTER, 
                (IppEnum)(int)Operation.PURGE_JOBS
            }
        },
        new(Value.CHARSET, "charset-configured") {Values = new List<IIppValue>{(IppString)"utf-8"} },
        new(Value.CHARSET, "charset-supported") {Values = new List<IIppValue>{(IppString)"utf-8"} },
        new(Value.NATURAL_LANG, "natural-language-configured") {Values = new List<IIppValue>{(IppString)"en-us"} },
        new(Value.NATURAL_LANG, "generated-natural-language-supported") {Values = new List<IIppValue>{(IppString)"en-us"} },
        new(Value.MIME_MEDIA_TYPE, "document-format-default") {Values = new List<IIppValue>{(IppString)DefaultDocumentFormat } },
        new(Value.MIME_MEDIA_TYPE, "document-format-supported") {Values = SupportedDocumentFormats.Select(d => (IppString)d).Cast<IIppValue>().ToList() },
        new(Value.MIME_MEDIA_TYPE, "document-format-preferred") {Values = new List<IIppValue>{(IppString)DefaultDocumentFormat } },
        new(Value.BOOLEAN, "printer-is-accepting-jobs") {Values = new List<IIppValue>{(IppBool)true} },
        new(Value.INTEGER, "queued-job-count") {Values = new List<IIppValue>{(IppInt) Jobs.Count} },
        new(Value.KEYWORD, "pdl-override-supported") {Values = new List<IIppValue>{(IppString)"not-attempted" } },
        new(Value.INTEGER, "printer-up-time") {Values = new List<IIppValue>{(IppInt) (DateTime.UtcNow - Started).TotalSeconds } },
        new(Value.DATE_TIME, "printer-current-time") {Values = new List<IIppValue>{new IppDateTime(DateTime.UtcNow)} },
        new(Value.KEYWORD, "compression-supported") {Values = new List<IIppValue>{(IppString)"none" } }
    };

    public void ClearJobs()
    {
        m_jobs.Clear();
    }
}