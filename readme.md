# fiskaly SDK for .NET

The fiskaly SDK includes an HTTP client that is needed<sup>[1](#fn1)</sup> for accessing the [kassensichv.io](https://kassensichv.io) API that implements a cloud-based, virtual **CTSS** (Certified Technical Security System) / **TSE** (Technische Sicherheitseinrichtung) as defined by the German **KassenSichV** ([Kassen­sich­er­ungsver­ord­nung](https://www.bundesfinanzministerium.de/Content/DE/Downloads/Gesetze/2017-10-06-KassenSichV.pdf)).

## Supported Versions

* .NET Framework 2.0+ (legacy version - `SDK.legacy.csproj`)
* .NET Standard 2.1+ (and therefore also .NET Framework 4.6.1+)
* .NET Framework 4.0

## Features

- [X] Automatic authentication handling (fetch/refresh JWT and re-authenticate upon 401 errors).
- [X] Automatic retries on failures (server errors or network timeouts/issues).
- [ ] Automatic JSON parsing and serialization of request and response bodies.
- [X] Future: [<a name="fn1">1</a>] compliance regarding [BSI CC-PP-0105-2019](https://www.bsi.bund.de/SharedDocs/Downloads/DE/BSI/Zertifizierung/Reporte/ReportePP/pp0105b_pdf.pdf?__blob=publicationFile&v=7) which mandates a locally executed SMA component for creating signed log messages. 
- [ ] Future: Automatic offline-handling (collection and documentation according to [Anwendungserlass zu § 146a AO](https://www.bundesfinanzministerium.de/Content/DE/Downloads/BMF_Schreiben/Weitere_Steuerthemen/Abgabenordnung/AO-Anwendungserlass/2019-06-17-einfuehrung-paragraf-146a-AO-anwendungserlass-zu-paragraf-146a-AO.pdf?__blob=publicationFile&v=1))

## How to build

1. Go to https://developer.fiskaly.com/clients/downloads
2. Download both the `386` and the `amd64` Windows builds of the fiskaly Client
3. Unzip both ZIP files in the directory `FiskalyClient`

### When using dotnet CLI

When using the `dotnet` CLI, use the following command to build the project:

```
[Fiskaly/SDK]$ dotnet build SDK.csproj
```

If you are using and older version than the earliest supported version, there is also a legacy build configuration that supports .NET Framework versions as early as .NET Framework 2.0.

```
[Fiskaly/SDK]$ dotnet build SDK.legacy.csproj
```

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

- [fiskaly.com](https://fiskaly.com)
- [dashboard.fiskaly.com](https://dashboard.fiskaly.com)
- [kassensichv.io](https://kassensichv.io)
- [kassensichv.net](https://kassensichv.net)

