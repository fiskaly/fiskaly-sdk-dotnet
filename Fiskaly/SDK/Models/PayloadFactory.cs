namespace Fiskaly.Client.Models
{
    public static class PayloadFactory
    {
        private const string JSON_RPC_VERSION = "2.0";

        private static JsonRpcRequest BuildRpcRequest(
            string id,
            string apiKey,
            string apiSecret,
            string baseUrl,
            string email,
            string password,
            string organizationId,
            string environment
            )
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
                    SdkVersion = Constants.SDK_VERSION,
                    Email = email,
                    Password = password,
                    OrganizationId = organizationId,
                    Environment = environment
                }
            };

            return request;
        }

        public static byte[] BuildCreateContextPayload(string id, string apiKey, string apiSecret, string baseUrl)
        {
            var request = BuildRpcRequest(id, apiKey, apiSecret, baseUrl, null, null, null, null);

            return Transformer.EncodeJsonRpcRequest(request);
        }

        public static byte[] BuildCreateContextPayload(
            string id,
            string apiKey,
            string apiSecret,
            string baseUrl,
            string email,
            string password,
            string organizationId,
            string environment
            )
        {
            var request = BuildRpcRequest(id, apiKey, apiSecret, baseUrl, email, password, organizationId, environment);

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

        public static byte[] BuildHealthCheckPayload(string id, HealthStatusRequestParams healthStatusRequestParams)
        {
            JsonRpcRequest request = new JsonRpcRequest
            {
                RequestId = id,
                JsonRpc = JSON_RPC_VERSION,
                Method = "self-test",
                Params = healthStatusRequestParams
            };

            return Transformer.EncodeJsonRpcRequest(request);
        }
    }
}
