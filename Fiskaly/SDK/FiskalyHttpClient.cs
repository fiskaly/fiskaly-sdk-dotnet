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

            InitialContextSet = true;
            JsonRpcResponse<string> response = JsonConvert
                .DeserializeObject<JsonRpcResponse<string>>(createContextResponse);

            Context = response.Result;
        }

        private byte[] CreateRequestPayload(string method, string path, byte[] body, Dictionary<string, string> headers, Dictionary<string, string> query)
        {
            byte[] payload = PayloadFactory.BuildRequestPayload(DateTime.Now.ToString(), new RequestParams
            {
                Method = method == null ? "GET" : method,
                Path = path == null ? "/" : path,
                Body = body == null ? new byte[0] : body,
                Headers = headers == null ? new Dictionary<string, string>() : headers,
                Query = query == null ? new Dictionary<string, string>() : query,
                Context = Context
            });

            return payload;
        }

        private FiskalyHttpError CreateFiskalyHttpError<T>(JsonRpcResponse<T> response)
        {
            ResponseErrorData errorData = JsonConvert
                .DeserializeObject<ResponseErrorData>(response.Error.Data.ToString());

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
            string responseString = Client.Invoke(payload);

            JsonRpcResponse<RequestResult> response =
                JsonConvert.DeserializeObject<JsonRpcResponse<RequestResult>>(responseString);

            ThrowOnError(response);

            Context = response.Result.Context;

            return new FiskalyHttpResponse
            {
                Status = response.Result.Status,
                Headers = response.Result.Headers,
                Body = Transformer.DecodeBase64BytesToUtf8Bytes(response.Result.Body)
            };
        }

        public ClientConfiguration ConfigureClient(ClientConfiguration configuration)
        {
            byte[] payload = PayloadFactory
                .BuildClientConfigurationPayload(DateTime.Now.ToString(), configuration);

            string responseString = Client.Invoke(payload);

            JsonRpcResponse<ConfigParams> response =
                JsonConvert.DeserializeObject<JsonRpcResponse<ConfigParams>>(responseString);

            ThrowOnError(response);

            ConfigParams config = response.Result;

            return new ClientConfiguration
            {
                ClientTimeout = config.ClientTimeout,
                SmaersTimeout = config.SmaersTimeout,
                DebugFile = config.DebugFile,
                DebugLevel = (DebugLevel)config.DebugLevel
            };
        }
    }
}
