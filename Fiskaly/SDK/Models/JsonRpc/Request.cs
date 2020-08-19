using Newtonsoft.Json;
using System.Collections.Generic;

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

        [JsonProperty("sdk_version")]
        public string SdkVersion { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
        
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("organization_id")]
        public string OrganizationId { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }
    }

    public class RequestParams : JsonRpcParams
    {
        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
    }

    public class Request
    {
        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("body")]
        public byte[] Body { get; set; }

        [JsonProperty("query")]
        public Dictionary<string, object> Query { get; set; }

        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }
    }

    public class ConfigParams : JsonRpcParams
    {
        [JsonProperty("config")]
        public Configuration Configuration { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
    }

    public class Configuration
    {
        [JsonProperty("debug_level")]
        public int DebugLevel { get; set; }

        [JsonProperty("debug_file")]
        public string DebugFile { get; set; }

        [JsonProperty("client_timeout")]
        public int ClientTimeout { get; set; }

        [JsonProperty("smaers_timeout")]
        public int SmaersTimeout { get; set; }

        [JsonProperty("http_proxy")]
        public string HttpProxy { get; set; }
    }

    public class HealthStatusRequestParams : JsonRpcParams
    {
        [JsonProperty("context")]
        public string Context { get; set; }
    }
}
