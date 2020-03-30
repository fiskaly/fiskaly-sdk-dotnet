using Newtonsoft.Json;

namespace Fiskaly.Client.Models
{
    public class JsonRpcParams { }

    public class JsonRpcRequest
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("id")]
        public string RequestId { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public JsonRpcParams Params { get; set; }
    }

    public class CreateContextParams : JsonRpcParams
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("api_secret")]
        public string ApiSecret { get; set; }

        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }
    }

    public class RequestParams : JsonRpcParams
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("body")]
        public byte[] Body { get; set; }

        [JsonProperty("query")]
        public object Query { get; set; }

        [JsonProperty("headers")]
        public object Headers { get; set; }

        [JsonProperty("context")]
        public object Context { get; set; }
    }

    public class ConfigParams : JsonRpcParams
    {
        [JsonProperty("debug_level")]
        public int DebugLevel { get; set; }

        [JsonProperty("debug_file")]
        public string DebugFile { get; set; }

        [JsonProperty("client_timeout")]
        public int ClientTimeout { get; set; }

        [JsonProperty("smaers_timeout")]
        public int SmaersTimeout { get; set; }
    }
}
