# ipp-printer

An implementation of a simple IPP printer and encoder/decoder, including a sample Printer Support App for configuration.

Although this implementation has been written from scratch for C# paradigms, it was inspired by https://github.com/watson/ipp-printer.

# Goals

This project aims to provide a framework for creating virtual IPP printers, using C# and ASP.NET. Specifically, we aim to:

- [x] Create an IPP request decoder.
- [x] Create an IPP response encoder.
- [ ] Get IPP 1.1 compliance.
- [ ] Get IPP 2.0 compliance.
- [ ] Get IPP 2.1 compliance.
- [ ] Enable the IPP printer to be installed as a Windows printer.
- [ ] Create clean interfaces for encoding/decoding, accessible through a NuGet package.
- [ ] Create clean interfaces for running an IPP server, accessible through a NuGet package.

# Compliance 

Compliance will be determined by using the IPP working group's ipptool and their associated tests. https://istopwg.github.io/ippsample/ipptool.html

# Tasks

- [ ] Improve documentation and readme.
- [x] Support GET_PRINTER_ATTRIBUTES 
- [x] Support PURGE_JOBS
- [x] Support PAUSE_PRINTER
- [x] Support RESUME_PRINTER
- [ ] Support PRINT_JOB (simple write to disk)
- [ ] Support PRINT_URI
- [ ] Support VALIDATE_JOB
- [ ] Support CREATE_JOB
- [ ] Support SEND_DOCUMENT
- [ ] Support SEND_URI
- [ ] Support CANCEL_JOB
- [ ] Support GET_JOB_ATTRIBUTES
- [ ] Support GET_JOBS
- [ ] Support HOLD_JOB
- [ ] Support RELEASE_JOB
- [ ] Support RESTART_JOB

# Contributions

Please feel free to pick up some of these tasks and contribute via pull requests.
