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
    public class AbstractClientTests
    {
        private AbstractClient GetClientInstance()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsClient();
            }

            return new LinuxClient();
        }

        [TestMethod]
        public void TestCStringConversion()
        {
            AbstractClient client = GetClientInstance();

            string input = "/äöü+#*'_-?ß!§$%&/()=<>|😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒";
            byte[] encodedInput = Encoding.UTF8.GetBytes(input);

            JsonRpcRequest request = new JsonRpcRequest
            {
                Id = DateTime.Now.ToString(),
                JsonRpc = "2.0",
                Method = "echo",
                Params = new RequestParams
                {
                    Body = encodedInput,
                    Context = "",
                    Headers = new Dictionary<string, string>(),
                    Method = "PUT",
                    Path = "/tx",
                    Query = new Dictionary<string, string>()
                }
            };

            byte[] encodedPayload = Transformer.EncodeJsonRpcRequest(request);
            string payload = Encoding.UTF8.GetString(encodedPayload);

            Debug.WriteLine(Encoding.UTF8.GetString(encodedPayload));

            string result = client.Invoke(encodedPayload);
            Debug.WriteLine(result);

            JsonRpcResponse<RequestResult> deserializedResponse =
                JsonConvert.DeserializeObject<JsonRpcResponse<RequestResult>>(result);

            string decodedInput = Transformer.DecodeBase64Body(deserializedResponse.Result.Body);

            Assert.AreEqual(input, decodedInput);
        }
    }
}