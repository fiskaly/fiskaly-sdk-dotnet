# fiskaly SDK for .NET

The fiskaly SDK includes an HTTP client that is needed<sup>[1](#fn1)</sup> for accessing the [kassensichv.io](https://kassensichv.io) API that implements a cloud-based, virtual **CTSS** (Certified Technical Security System) / **TSE** (Technische Sicherheitseinrichtung) as defined by the German **KassenSichV** ([Kassen­sich­er­ungsver­ord­nung](https://www.bundesfinanzministerium.de/Content/DE/Downloads/Gesetze/2017-10-06-KassenSichV.pdf)).

## Supported Versions

* .NET Framework 2.0+ (legacy version - `SDK.legacy.csproj`)
* .NET Standard 2.0+ (and therefore also .NET Framework 4.6.1+)
* .NET Framework 4.0

## Features

- [X] Automatic authentication handling (fetch/refresh JWT and re-authenticate upon 401 errors).
- [X] Automatic retries on failures (server errors or network timeouts/issues).
- [ ] Automatic JSON parsing and serialization of request and response bodies.
- [X] Future: [<a name="fn1">1</a>] compliance regarding [BSI CC-PP-0105-2019](https://www.bsi.bund.de/SharedDocs/Downloads/DE/BSI/Zertifizierung/Reporte/ReportePP/pp0105b_pdf.pdf?__blob=publicationFile&v=7) which mandates a locally executed SMA component for creating signed log messages. 
- [ ] Future: Automatic offline-handling (collection and documentation according to [Anwendungserlass zu § 146a AO](https://www.bundesfinanzministerium.de/Content/DE/Downloads/BMF_Schreiben/Weitere_Steuerthemen/Abgabenordnung/AO-Anwendungserlass/2019-06-17-einfuehrung-paragraf-146a-AO-anwendungserlass-zu-paragraf-146a-AO.pdf?__blob=publicationFile&v=1))

## Integration

### NuGet
![Nuget](https://img.shields.io/nuget/dt/fiskaly-dotnet-sdk)

The .NET SDK is available for download on [NuGet](https://www.nuget.org/packages/fiskaly-dotnet-sdk/1.0.0.1-alpha).

#### Package Manager

`PM> Install-Package fiskaly-dotnet-sdk -Version 1.0.0.1-alpha`

#### .NET (dotnet) CLI

`$ dotnet add package fiskaly-dotnet-sdk --version 1.0.0.1-alpha`

### Client

Additionaly to the SDK, you'll also need the fiskaly client. Follow these steps to integrate it into your project:

1. Go to [https://developer.fiskaly.com/downloads](https://developer.fiskaly.com/downloads)
2. Download the appropriate client build for your platform
3. Move the client into your project output directory or somewhere within the OS search path

### Manually building the project

When using the `dotnet` CLI, use the following command to build the project:

`[Fiskaly/SDK]$ dotnet build SDK.standard.csproj`

If you are using and older version than the earliest supported version, there is also a legacy build configuration that supports .NET Framework versions as early as .NET Framework 2.0.

`[Fiskaly/SDK]$ dotnet build SDK.legacy.csproj`

## Usage

```c#
using System;
using System.Text;
using Fiskaly;
using Fiskaly.Client;
using Fiskaly.Client.Models;

namespace Demo
{
    class Demo
    {
        static String ApiKey = Environment.GetEnvironmentVariable("API_KEY"); // create your own API key and secret at https://dashboard.fiskaly.com
        static String ApiSecret = Environment.GetEnvironmentVariable("API_SECRET");
        static FiskalyHttpClient client = new FiskalyHttpClient(ApiKey, ApiSecret, "https://kassensichv.io/api/v0");

        static void Main(string[] args)
        {
            FiskalyHttpResponse result = client.Request("GET", "tss", null, null, null);
            Console.WriteLine(result.Status);
            Console.WriteLine(result.Reason);
            Console.WriteLine(result.Headers);
            Console.WriteLine(Encoding.UTF8.GetString(result.Body));
        }
    }
}
```

## Related

* [fiskaly.com](https://fiskaly.com)
* [dashboard.fiskaly.com](https://dashboard.fiskaly.com)
* [kassensichv.io](https://kassensichv.io)
* [kassensichv.net](https://kassensichv.net)
