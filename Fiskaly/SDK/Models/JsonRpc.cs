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
        public string Id { get; set; }

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

    public class JsonRpcResponse<T>
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("error")]
        public JsonRpcResponseError Error { get; set; }

        [JsonProperty("id")]
        public string RequestId { get; set; }
    }

    public class RequestResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("header")]
        public Dictionary<string, string[]> Header { get; set; }

        [JsonProperty("body")]
        public byte[] Body { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
    }

    public class FiskalyApiError
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class JsonRpcResponseError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public ResponseErrorData Data { get; set; }
    }

    public class ResponseErrorData
    {
        [JsonProperty("line")]
        public string Line { get; set; }

        [JsonProperty("data")]
        public ResponseErrorDataData Data { get; set; }
    }

    public class ResponseErrorDataData
    {
        [JsonProperty("response")]
        public RequestResult Result { get; set; }

        [JsonProperty("request-id")]
        public string RequestId { get; set; }
    }
}
