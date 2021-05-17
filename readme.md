# fiskaly SDK for .NET

The fiskaly SDK includes an HTTP client that is needed<sup>[1](#fn1)</sup> for accessing the [kassensichv.io](https://kassensichv.io) API that implements a cloud-based, virtual **CTSS** (Certified Technical Security System) / **TSE** (Technische Sicherheitseinrichtung) as defined by the German **KassenSichV** ([Kassen­sich­er­ungsver­ord­nung](https://www.bundesfinanzministerium.de/Content/DE/Downloads/Gesetze/2017-10-06-KassenSichV.pdf)).

## Supported Versions

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

The .NET SDK is available for download on [NuGet](https://www.nuget.org/packages/fiskaly-dotnet-sdk).

#### Package Manager

`PM> Install-Package fiskaly-dotnet-sdk -Version 1.2.200`

#### .NET (dotnet) CLI

`$ dotnet add package fiskaly-dotnet-sdk --version 1.2.200`

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

### Demo

```c#
using System;
using System.Text;
using Fiskaly;
using Fiskaly.Client;
using Fiskaly.Client.Models;

namespace Demo
{
    static class Demo
    {
        // create your own API key and secret at https://dashboard.fiskaly.com
        public static String FiskalyApiKey = Environment.GetEnvironmentVariable("API_KEY");
        public static String FiskalyApiSecret = Environment.GetEnvironmentVariable("API_SECRET");

        public static string TSS_ID = Guid.NewGuid().ToString();
        public static string CLIENT_ID = Guid.NewGuid().ToString();


        public static FiskalyHttpClient client =
            new FiskalyHttpClient(FiskalyApiKey, FiskalyApiSecret, "https://kassensichv.io/api/v1");

        public static void Main(string[] args)
        {
            InitializeTss();

            var startTxResponse = StartTransaction();
        }

        public static void CreateClient()
        {
            var clientSerial = "fiskaly-dotnet-sdk-test-client";
            var createTssPayload = "{ \"serial_number\": \"" + clientSerial + "\" }";
            var bodyBytes = Encoding.UTF8.GetBytes(createTssPayload);

            var response = client
                .Request("PUT", "/tss/" + TSS_ID + "/client/" + CLIENT_ID, bodyBytes, null, null);
            var decodedBody = Encoding.UTF8.GetString(response.Body);
            var body = JsonConvert
                .DeserializeObject<Dictionary<string, object>>(decodedBody);
        }

        public static void CreateTss()
        {
            var descriptionContent = "TSS created by .NET SDK CreateClient at "
                + DateTime.Now.ToString();
            var createClientPayload = "{ \"description\": \""
                + descriptionContent + "\", \"state\": \"UNINITIALIZED\" }";

            var bodyBytes = Encoding.UTF8.GetBytes(createClientPayload);

            var response = client
                .Request("PUT", "/tss/" + TSS_ID, bodyBytes, null, null);

            var decodedBody = Encoding.UTF8.GetString(response.Body);
            var body = JsonConvert
                .DeserializeObject<Dictionary<string, object>>(decodedBody);

            var initializeTssPayload = "{ \"state\": \"INITIALIZED\" }";
            bodyBytes = Encoding.UTF8.GetBytes(initializeTssPayload);

            var initializeResponse = client
                .Request("PUT", "/tss/" + TSS_ID, bodyBytes, null, null);
        }

        public static void InitializeTss()
        {
            CreateTss();
            CreateClient();
        }

        public static FiskalyHttpResponse StartTransaction()
        {
            var txId = Guid.NewGuid().ToString();
            var payload = "{ \"state\": \"ACTIVE\", \"client_id\": \"" + CLIENT_ID + "\" }";

            var bodyBytes = Encoding.UTF8.GetBytes(payload);

            var response = client
                .Request("PUT", "/tss/" + TSS_ID + "/tx/" + txId, bodyBytes, null, null);

            return response;
        }
    }
}
```

### Client Configuration

The SDK is built on the [fiskaly Client](https://developer.fiskaly.com/en/docs/client-documentation) which can be [configured](https://developer.fiskaly.com/en/docs/client-documentation#configuration) through the SDK.

A reason why you would do this, is to enable the [debug mode](https://developer.fiskaly.com/en/docs/client-documentation#debug-mode).

#### Enabling the debug mode

The following code snippet demonstrates how to enable the debug mode in the client.

```c#

public static void Main(string[] args)
{
    var client = new FiskalyHttpClient(
        FiskalyApiKey,
        FiskalyApiSecret,
        "https://kassensichv.io/api/v1"
    );

    var configuration = new ClientConfiguration
    {
        DebugLevel = DebugLevel.EVERYTHING
    };

    client.ConfigureClient(configuration);
}
```

## Related

* [fiskaly.com](https://fiskaly.com)
* [dashboard.fiskaly.com](https://dashboard.fiskaly.com)
* [kassensichv.io](https://kassensichv.io)
* [kassensichv.net](https://kassensichv.net)
