# Runnable SDK sample

This project is a runnable sample for the fiskaly .NET SDK targeting .NET Framework 4.0.

## Build

The project can either be built using Visual Studio or using the `dotnet` CLI.

```bash
> dotnet build
```

This will produce an exe file which can be run to test compatibility with your target system.

Don't forget to download the [fiskaly Client](https://developer.fiskaly.com/downloads) and place it within the execution directory.

## Runtime configuration

The demo expects two environment variables to be set:

- FISKALY_API_KEY
- FISKALY_API_SECRET

Set these to values you've created in the [dashboard](https://dashboard.fiskaly.com/).