using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;

namespace Fiskaly.Client.Models
{
    class PayloadFactory
    {
        private static byte[] EncodeHttpBody(byte[] bodyBytes)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(bodyBytes));
        }

        private static byte[] EncodeJsonRpcRequest(JsonRpcRequest request)
        {
            string payload = JsonConvert.SerializeObject(request);
            return Encoding.UTF8.GetBytes(payload);
        }

        public static byte[] BuildCreateContextPayload(string id, string apiKey, string apiSecret, string baseUrl)
        {
            JsonRpcRequest request = new JsonRpcRequest
            {
                Id = id,
                JsonRpc = "2.0",
                Method = "create-context",
                Params = new CreateContextParams
                {
                    ApiKey = apiKey,
                    ApiSecret = apiSecret,
                    BaseUrl = baseUrl
                }
            };

            return EncodeJsonRpcRequest(request);
        }

        public static byte[] BuildRequestPayload(string id, RequestParams paramsValue)
        {
            Debug.WriteLine(Encoding.UTF8.GetString(paramsValue.Body));

            paramsValue.Body = EncodeHttpBody(paramsValue.Body);

            JsonRpcRequest request = new JsonRpcRequest
            {
                Id = id,
                JsonRpc = "2.0",
                Method = "request",
                Params = paramsValue
            };

            return EncodeJsonRpcRequest(request);
        }
    }
}
