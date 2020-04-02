namespace Fiskaly.Client.Models
{
    public static class PayloadFactory
    {
        private const string JSON_RPC_VERSION = "2.0";

        public static byte[] BuildCreateContextPayload(string id, string apiKey, string apiSecret, string baseUrl)
        {
            JsonRpcRequest request = new JsonRpcRequest
            {
                RequestId = id,
                JsonRpc = JSON_RPC_VERSION,
                Method = "create-context",
                Params = new CreateContextParams
                {
                    ApiKey = apiKey,
                    ApiSecret = apiSecret,
                    BaseUrl = baseUrl,
                    SdkVersion = Constants.SDK_VERSION
                }
            };

            return Transformer.EncodeJsonRpcRequest(request);
        }

        public static byte[] BuildRequestPayload(string id, RequestParams paramsValue)
        {
            JsonRpcRequest request = new JsonRpcRequest
            {
                RequestId = id,
                JsonRpc = JSON_RPC_VERSION,
                Method = "request",
                Params = paramsValue
            };

            return Transformer.EncodeJsonRpcRequest(request);
        }

        public static byte[] BuildClientConfigurationPayload(string id, ConfigParams configParams)
        {
            JsonRpcRequest request = new JsonRpcRequest
            {
                RequestId = id,
                JsonRpc = JSON_RPC_VERSION,
                Method = "config",
                Params = configParams
            };

            return Transformer.EncodeJsonRpcRequest(request);
        }

        public static byte[] BuildGetVersionPayload(string id)
        {
            JsonRpcRequest request = new JsonRpcRequest
            {
                RequestId = id,
                JsonRpc = JSON_RPC_VERSION,
                Method = "version",
                Params = new JsonRpcParams { },
            };

            return Transformer.EncodeJsonRpcRequest(request);
        }
    }
}
