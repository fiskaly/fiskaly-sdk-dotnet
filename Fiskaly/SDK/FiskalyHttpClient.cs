using System;
using System.Collections.Generic;
using Fiskaly.Client.Models;
using Newtonsoft.Json;
using Fiskaly.Client;
using FiskalyClient.Errors;
using System.Diagnostics;

namespace Fiskaly
{
    public class FiskalyHttpClient
    {
        private string Context { get; set; }

        public FiskalyHttpClient(string apiKey, string apiSecret, string baseUrl)
        {
            byte[] payload = PayloadFactory.BuildCreateContextPayload(DateTime.Now.ToString(), apiKey, apiSecret, baseUrl);
            string createContextResponse = ClientLibrary.Invoke(payload);

            SetContext(createContextResponse);
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

            string response = ClientLibrary.Invoke(payload);

            JsonRpcResponse<RequestResult> deserializedResponse = JsonConvert.DeserializeObject<JsonRpcResponse<RequestResult>>(response);

            if (deserializedResponse.Error != null)
            {
                Debug.WriteLine("Error payload: " + response);

                // HTTP error
                if (deserializedResponse.Error.Code > 0)
                {
                    string jsonError = deserializedResponse.Error.Data.Data.Body;
                    FiskalyApiError errorBody = JsonConvert.DeserializeObject<FiskalyApiError>(jsonError);
                    throw new FiskalyHttpError(deserializedResponse.Error.Code, errorBody.Error, errorBody.Message, errorBody.Code, deserializedResponse.RequestId);
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
