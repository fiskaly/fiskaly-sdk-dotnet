using Fiskaly.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SdkTest
{
    [TestClass]
    public class EncodingConversionTest
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
            AbstractClient windowsClient = new WindowsClient();

            string input = "/äöü+#*'_-?ß!§$%&/()=<>|😀 😁 😂 🤣 😃 😄 😅 😆 😉 😊 😋 😎 😍 😘 🥰 😗 😙 😚 ☺️ 🙂 🤗 🤩 🤔 🤨 😐 😑 😶 🙄 😏 😣 😥 😮 🤐 😯 😪 😫 😴 😌 😛 😜 😝 🤤 😒 😓 😔 😕 🙃 🤑 😲 ☹️ 🙁 😖 😞 😟 😤 😢 😭 😦 😧 😨 😩 🤯 😬 😰 😱 🥵 🥶 😳 🤪 😵 😡 😠 🤬 😷 🤒";
            byte[] encodedInput = Encoding.UTF8.GetBytes(input);

            string result = windowsClient.Invoke(encodedInput);

            Assert.Equals(result, input);

            Console.WriteLine("Test runs");
        }
    }
}
