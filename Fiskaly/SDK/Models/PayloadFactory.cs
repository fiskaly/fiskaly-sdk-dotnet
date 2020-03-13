using System.Diagnostics;
using System.Text;

namespace Fiskaly.Client.Models
{
    class PayloadFactory
    {
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

            return Transformer.EncodeJsonRpcRequest(request);
        }

        public static byte[] BuildRequestPayload(string id, RequestParams paramsValue)
        {
            Debug.WriteLine(Encoding.UTF8.GetString(paramsValue.Body));

            paramsValue.Body = Transformer.EncodeHttpBody(paramsValue.Body);

            JsonRpcRequest request = new JsonRpcRequest
            {
                Id = id,
                JsonRpc = "2.0",
                Method = "request",
                Params = paramsValue
            };

            return Transformer.EncodeJsonRpcRequest(request);
        }
    }
}
