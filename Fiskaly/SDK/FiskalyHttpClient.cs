using Fiskaly.Client;
using Fiskaly.Client.Models;
using Fiskaly.Errors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fiskaly
{
    public class FiskalyHttpClient
    {
        private string Context { get; set; }

        private bool InitialContextSet { get; set; }

        private AbstractClient Client { get; set; }

        private string ApiKey { get; }
        private string ApiSecret { get; }
        private string BaseUrl { get; }

        public FiskalyHttpClient(string apiKey, string apiSecret, string baseUrl)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException("apiKey may not be null");
            }

            if (apiSecret == null)
            {
                throw new ArgumentNullException("apiSecret may not be null");
            }

            if (baseUrl == null)
            {
                throw new ArgumentNullException("baseUrl may not be null");
            }

            ApiKey = apiKey;
            ApiSecret = apiSecret;
            BaseUrl = baseUrl;

            InitializeClient();
        }

        private void InitializeClient() {
        #if NET40
            this.Client = new WindowsClient();

        // Non-Windows platforms are only supported through .NET Standard 2.1 at the moment
        #elif NETSTANDARD2_0

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Client = new WindowsClient();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Client = new LinuxClient();
            }
            else
            {
                Client = new LinuxClient();
            }
        // Use Windows as default for safety
        #else
            Client = new WindowsClient();
        #endif
        }

        private void InitializeContext()
        {
            byte[] payload = PayloadFactory
                .BuildCreateContextPayload(DateTime.Now.ToString(), ApiKey, ApiSecret, BaseUrl);

            string createContextResponse = Client.Invoke(payload);
            System.Diagnostics.Debug.WriteLine("CreateContextResponse: " + createContextResponse);

            InitialContextSet = true;
            JsonRpcResponse<CreateContextResult> response = JsonConvert
                .DeserializeObject<JsonRpcResponse<CreateContextResult>>(createContextResponse);

            ThrowOnError(response);

            Context = response.Result.Context;
            System.Diagnostics.Debug.WriteLine("Set context to: " + Context);
        }

        private byte[] CreateRequestPayload(string method, string path, byte[] body, Dictionary<string, string> headers, Dictionary<string, string> query)
        {
            byte[] payload = PayloadFactory.BuildRequestPayload(DateTime.Now.ToString(),
                new RequestParams
                {
                    Request = new Request
                    {
                        Method = method == null ? "GET" : method,
                        Path = path == null ? "/" : path,
                        Body = body == null ? new byte[0] : body,
                        Headers = headers == null ? new Dictionary<string, string>() : headers,
                        Query = query == null ? new Dictionary<string, string>() : query
                    },
                    Context = Context
                }
            );

            return payload;
        }

        private FiskalyHttpError CreateFiskalyHttpError<T>(JsonRpcResponse<T> response)
        {
            ErrorData errorData = JsonConvert
                .DeserializeObject<ErrorData>(response.Error.Data.ToString());

            FiskalyApiError errorBody = JsonConvert
                .DeserializeObject<FiskalyApiError>(
                    Transformer.DecodeBase64Body(errorData.Response.Body));

            string[] requestIdHeaders;

            errorData
                .Response
                .Headers
                .TryGetValue("X-Request-Id", out requestIdHeaders);

            string requestId = requestIdHeaders[0];

            return new FiskalyHttpError(
                response.Error.Code,
                errorBody.Error,
                errorBody.Message,
                errorData.Response.Status,
                requestId
            );
        }

        private FiskalyClientError CreateFiskalyClientError<T>(JsonRpcResponse<T> response)
        {
            return new FiskalyClientError(
                response.Error.Code,
                response.Error.Message,
                response.Error.Data.ToString()
            );
        }
        
        private void ThrowOnError<T>(JsonRpcResponse<T> response)
        {
            if (response.Error != null)
            {
                if (response.Error.Code == ClientError.HTTP_ERROR)
                {
                    throw CreateFiskalyHttpError(response);
                }
                else if (response.Error.Code == ClientError.HTTP_TIMEOUT_ERROR)
                {
                    throw new FiskalyHttpTimeoutError(response.Error.Message);
                }
                else
                {
                    throw CreateFiskalyClientError(response);
                }
            }
        }

        public FiskalyHttpResponse Request(string method, string path, byte[] body, Dictionary<string, string> headers, Dictionary<string, string> query)
        {
            if (!InitialContextSet)
            {
                InitializeContext();
            }

            byte[] payload = CreateRequestPayload(method, path, body, headers, query);
            string invocationResponse = Client.Invoke(payload);
            System.Diagnostics.Debug.WriteLine(invocationResponse);

            JsonRpcResponse<RequestResult> rpcResponse =
                JsonConvert.DeserializeObject<JsonRpcResponse<RequestResult>>(invocationResponse);

            ThrowOnError(rpcResponse);

            Context = rpcResponse.Result.Context;

            return new FiskalyHttpResponse
            {
                Status = rpcResponse.Result.Response.Status,
                Headers = rpcResponse.Result.Response.Headers,
                Body = Transformer.DecodeBase64BytesToUtf8Bytes(rpcResponse.Result.Response.Body)
            };
        }

        public ClientConfiguration ConfigureClient(ClientConfiguration configuration)
        {
            if (!InitialContextSet)
            {
                InitializeContext();
            }

            byte[] payload = PayloadFactory
                .BuildClientConfigurationPayload(DateTime.Now.ToString(), new ConfigParams
                {
                    Configuration = new Configuration
                    {
                        ClientTimeout = configuration.ClientTimeout,
                        SmaersTimeout = configuration.SmaersTimeout,
                        DebugFile = configuration.DebugFile,
                        DebugLevel = (int)configuration.DebugLevel
                    },
                    Context = Context
                });

            string invocationResponse = Client.Invoke(payload);
            System.Diagnostics.Debug.WriteLine("ConfigureClient[invocationResponse]: " + invocationResponse);

            JsonRpcResponse<ConfigParams> rpcResponse =
                JsonConvert.DeserializeObject<JsonRpcResponse<ConfigParams>>(invocationResponse);

            ThrowOnError(rpcResponse);

            Configuration config = rpcResponse.Result.Configuration;
            Context = rpcResponse.Result.Context;

            return new ClientConfiguration
            {
                ClientTimeout = config.ClientTimeout,
                SmaersTimeout = config.SmaersTimeout,
                DebugFile = config.DebugFile,
                DebugLevel = (DebugLevel)config.DebugLevel
            };
        }

        public Models.Version Version()
        {
            byte[] payload = PayloadFactory
                .BuildGetVersionPayload(DateTime.Now.ToString());

            string invocationResponse = Client.Invoke(payload);
            System.Diagnostics.Debug.WriteLine("Version[invocationResponse]: " + invocationResponse);

            JsonRpcResponse<VersionData> rpcResponse =
                JsonConvert.DeserializeObject<JsonRpcResponse<VersionData>>(invocationResponse);

            ThrowOnError(rpcResponse);

            VersionData versions = rpcResponse.Result;

            Models.Version version = new Models.Version
            {
                ClientVersion = new Models.ComponentVersion
                {
                    Version = versions.ClientVersion.Version,
                    CommitHash = versions.ClientVersion.CommitHash,
                    SourceHash = versions.ClientVersion.SourceHash
                },
                SmaersVersion = new Models.ComponentVersion
                {
                    Version = versions.SmaersVersion.Version,
                    CommitHash = versions.SmaersVersion.CommitHash,
                    SourceHash = versions.SmaersVersion.SourceHash
                },
                SdkVersion = Constants.SDK_VERSION
            };

            return version;
        }
    }
}
