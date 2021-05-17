using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fiskaly.Client.Models
{
    public class JsonRpcResponse<T>
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("error")]
        public JsonRpcError Error { get; set; }

        [JsonProperty("id")]
        public string RequestId { get; set; }
    }

    public class CreateContextResult
    {
        [JsonProperty("context")]
        public string Context;
    }

    public class RequestResult
    {
        [JsonProperty("response")]
        public Response Response { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }
    }

    public class Response
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("headers")]
        public Dictionary<string, string[]> Headers { get; set; }

        [JsonProperty("body")]
        public byte[] Body { get; set; }
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

    public class JsonRpcError
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }

    public class ErrorData
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public class VersionData
    {
        [JsonProperty("smaers")]
        public VersionRow SmaersVersion { get; set; }

        [JsonProperty("client")]
        public VersionRow ClientVersion { get; set; }
    }

    public class VersionRow
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("source_hash")]
        public string SourceHash { get; set; }

        [JsonProperty("commit_hash")]
        public string CommitHash { get; set; }
    }

    public class HealthStatus
    {
        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        [JsonProperty("smaers")]
        public string Smaers { get; set; }

        [JsonProperty("backend")]
        public Dictionary<string, string> backend { get; set; }
    }
}
