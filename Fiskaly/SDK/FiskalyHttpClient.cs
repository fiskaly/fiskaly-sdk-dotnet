using Fiskaly.Client;
using Fiskaly.Client.Models;
using FiskalyClient.Errors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fiskaly
{
    public class FiskalyHttpClient
    {
        private string Context { get; set; }
        private AbstractClient Client { get; set; }

        public FiskalyHttpClient(string apiKey, string apiSecret, string baseUrl)
        {
            this.InitializeClient();

            byte[] payload = PayloadFactory.BuildCreateContextPayload(DateTime.Now.ToString(), apiKey, apiSecret, baseUrl);
            string createContextResponse = this.Client.Invoke(payload);

            SetContext(createContextResponse);
        }

        private void InitializeClient() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.Client = new WindowsClient();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                this.Client = new LinuxClient();
            }
            else
            {
                this.Client = new LinuxClient();
            }
        }

        private string SetContext(string invocationResponse)
        {
            JsonRpcResponse<string> response = JsonConvert.DeserializeObject<JsonRpcResponse<string>>(invocationResponse);

            this.Context = response.Result;

            return this.Context;
        }

        public FiskalyHttpResponse Request(string method, string path, byte[] body, Dictionary<string, string> headers, Dictionary<string, string> query)
        {
            byte[] payload = PayloadFactory.BuildRequestPayload(DateTime.Now.ToString(), new RequestParams
            {
                Method = method,
                Path = path,
                Body = body,
                Headers = headers,
                Query = query,
                Context = this.Context
            });

            string response = this.Client.Invoke(payload);

            JsonRpcResponse<RequestResult> deserializedResponse =
                JsonConvert.DeserializeObject<JsonRpcResponse<RequestResult>>(response);

            if (deserializedResponse.Error != null)
            {
                // HTTP error
                if (deserializedResponse.Error.Code > 0)
                {
                    string jsonError = deserializedResponse.Error.Data.Data.Body;
                    FiskalyApiError errorBody = JsonConvert.DeserializeObject<FiskalyApiError>(jsonError);
                    throw new FiskalyHttpError(
                        deserializedResponse.Error.Code,
                        errorBody.Error,
                        errorBody.Message,
                        errorBody.Code,
                        deserializedResponse.RequestId
                    );
                }
                else // JSON-RPC error
                {
                    throw new FiskalyClientError(deserializedResponse.Error.Code, deserializedResponse.Error.Message);
                }
            }

            this.Context = deserializedResponse.Result.Context;

            // "200 OK"
            string[] statusLine = deserializedResponse.Result.Status.Split(' ');

            int status = Int32.Parse(statusLine[0]);
            string reason = statusLine[1];

            return new FiskalyHttpResponse
            {
                Status = status,
                Reason = reason,
                Headers = deserializedResponse.Result.Header,
                Body = Convert.FromBase64String(deserializedResponse.Result.Body)
            };
        }
    }
}
