using Fiskaly.Client.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Fiskaly.Client.Tests
{
    [TestClass]
    public class ClientTests
    {
        private AbstractClient GetClientInstance()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsClient();
            }

            return new LinuxClient();
        }

        [TestMethod()]
        public void TestCStringConversion()
        {
            AbstractClient client = GetClientInstance();

            string input = "/äöü+#*'_-?ß!§$%&/()=<>|😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒";
            byte[] encodedInput = Encoding.UTF8.GetBytes(input);

            JsonRpcRequest request = new JsonRpcRequest
            {
                RequestId = DateTime.Now.ToString(),
                JsonRpc = "2.0",
                Method = "echo",
                Params = new RequestParams
                {
                    Request = new Request
                    {
                        Body = encodedInput,
                        Headers = new Dictionary<string, string>(),
                        Method = "PUT",
                        Path = "/tx",
                        Query = new Dictionary<string, object>()
                    },
                    Context = "",
                }
            };

            byte[] encodedPayload = Transformer.EncodeJsonRpcRequest(request);
            string result = client.Invoke(encodedPayload);

            Debug.WriteLine(result);

            JsonRpcResponse<RequestParams> deserializedResponse =
                JsonConvert.DeserializeObject<JsonRpcResponse<RequestParams>>(result);

            Debug.WriteLine(deserializedResponse);

            string decodedInput = Transformer.DecodeBase64Body(deserializedResponse.Result.Request.Body);

            Assert.AreEqual(input, decodedInput);
        }

        [TestMethod()]
        public void FaultyStringShouldCauseError()
        {
            AbstractClient client = GetClientInstance();

            string faultyString = "faulty test";
            byte[] encoded = Encoding.UTF8.GetBytes(faultyString);

            encoded[1] = 0xFE;

            string invocation = client.Invoke(encoded);

            Assert.IsTrue(invocation.Contains("encoding error"));
        }
    }
}