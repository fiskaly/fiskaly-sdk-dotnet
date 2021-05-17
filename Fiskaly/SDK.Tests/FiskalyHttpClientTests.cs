using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fiskaly.Errors;
using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Fiskaly.Client;

namespace Fiskaly.Tests
{
    [TestClass()]
    public class FiskalyHttpClientTests
    {
        public readonly string API_KEY = Environment.GetEnvironmentVariable("FISKALY_API_KEY");
        public readonly string API_SECRET = Environment.GetEnvironmentVariable("FISKALY_API_SECRET");

        public static readonly string SIGN_API_V1_PATH = "https://kassensichv.io/api/v1";
        public static readonly string SIGN_API_V0_PATH = "https://kassensichv.io/api/v0";

        public string TSS_ID = Guid.NewGuid().ToString();
        public string CLIENT_ID = Guid.NewGuid().ToString();

        private FiskalyHttpClient GetClient(string baseUrl)
        {
            return new FiskalyHttpClient(API_KEY, API_SECRET, baseUrl);
        }

        [TestMethod()]
        public void CanAccessApiV0()
        {
            var client = GetClient(SIGN_API_V0_PATH);

            try
            {
                var response = client.Request("GET", "/", null, null, null);

                Assert.IsNotNull(response);
                Assert.AreEqual(200, response.Status);
            } catch (FiskalyHttpError)
            {
                Assert.Fail("Should be able to access v1.");
            }
        }

        [TestMethod()]
        public void CanAccessApiV1()
        {
            var client = GetClient(SIGN_API_V1_PATH);

            try
            {
                var response = client.Request("GET", "/", null, null, null);

                Assert.IsNotNull(response);
                Assert.AreEqual(200, response.Status);
            } catch (FiskalyHttpError)
            {
                Assert.Fail("Should be able to access v1.");
            }
        }
        public void CreateClientV1(FiskalyHttpClient client)
        {
            var clientSerial = "fiskaly-dotnet-sdk-test-client";
            var createTssPayload = "{ \"serial_number\": \"" + clientSerial + "\" }";
            var bodyBytes = Encoding.UTF8.GetBytes(createTssPayload);

            var response = client.Request("PUT", "/tss/" + TSS_ID + "/client/" + CLIENT_ID, bodyBytes, null, null);
            var decodedBody = Encoding.UTF8.GetString(response.Body);
            var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedBody);

            Assert.AreEqual(200, response.Status);
            Assert.AreEqual(clientSerial, body.GetValueOrDefault("serial_number", "NO VALUE"));
            Assert.AreEqual(CLIENT_ID, body.GetValueOrDefault("_id", CLIENT_ID));
        }

        public void CreateTssV1(FiskalyHttpClient client)
        {
            var descriptionContent = "TSS created by .NET SDK Test CreateClientV1 at " + DateTime.Now.ToString();
            var createClientPayload = "{ \"description\": \"" + descriptionContent + "\", \"state\": \"UNINITIALIZED\" }";

            var bodyBytes = Encoding.UTF8.GetBytes(createClientPayload);

            var response = client.Request("PUT", "/tss/" + TSS_ID, bodyBytes, null, null);

            var decodedBody = Encoding.UTF8.GetString(response.Body);
            var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedBody);

            Assert.AreEqual(200, response.Status);
            Assert.AreEqual(descriptionContent, body.GetValueOrDefault("description", "NO VALUE"));
            Assert.AreEqual("UNINITIALIZED", body.GetValueOrDefault("state", "NO VALUE"));
            Assert.AreEqual(TSS_ID, body.GetValueOrDefault("_id", TSS_ID));

            var initializeTssPayload = "{ \"state\": \"INITIALIZED\" }";
            bodyBytes = Encoding.UTF8.GetBytes(initializeTssPayload);

            var initializeResponse = client.Request("PUT", "/tss/" + TSS_ID, bodyBytes, null, null);
            Assert.AreEqual(200, initializeResponse.Status);
        }

        [TestInitialize()]
        public void InitializeTss()
        {
            var client = GetClient(SIGN_API_V1_PATH);

            CreateTssV1(client);
            CreateClientV1(client);
        }

        [TestMethod()]
        public void CanStartTransaction()
        {
            var client = GetClient(SIGN_API_V1_PATH);

            var txId = Guid.NewGuid().ToString();
            var payload = "{ \"state\": \"ACTIVE\", \"client_id\": \"" + CLIENT_ID + "\" }";

            var bodyBytes = Encoding.UTF8.GetBytes(payload);

            var response = client.Request("PUT", "/tss/" + TSS_ID + "/tx/" + txId, bodyBytes, null, null);

            Assert.AreEqual(200, response.Status);
        }

        [TestMethod()]
        public void CanConfigureClient()
        {
            const int clientTimeout = 12340;
            const int smaersTimeout = 1212;
            const DebugLevel debugLevel = DebugLevel.EVERYTHING;
            const string debugFile = "C:\\temp";

            var client = GetClient(SIGN_API_V1_PATH);

            var response = client.ConfigureClient(new Client.ClientConfiguration
            {
                ClientTimeout = clientTimeout,
                SmaersTimeout = smaersTimeout,
                DebugLevel = debugLevel
            });

            Assert.AreEqual(clientTimeout, response.ClientTimeout);
            Assert.AreEqual(smaersTimeout, response.SmaersTimeout);
            Assert.AreEqual(debugLevel, response.DebugLevel);

            response = client.ConfigureClient(new Client.ClientConfiguration
            {
                DebugFile = "C:\\temp"
            });

            Assert.AreEqual(clientTimeout, response.ClientTimeout);
            Assert.AreEqual(smaersTimeout, response.SmaersTimeout);
            Assert.AreEqual(debugLevel, response.DebugLevel);
            Assert.AreEqual(debugFile, response.DebugFile);
        }

        [TestMethod()]
        public void CanGetVersion()
        {
            var client = GetClient(SIGN_API_V1_PATH);
            var version = client.Version();

            Assert.IsNotNull(version);
        }

        [TestMethod()]
        public void CanGetHealthStatus()
        {
            var client = GetClient(SIGN_API_V1_PATH);
            var health = client.HealthCheck();

            Assert.IsNotNull(health);
            Assert.IsNotNull(health.Backend);
            Assert.IsNotNull(health.Proxy);
            Assert.IsNotNull(health.Proxy.Status);
            Assert.IsNotNull(health.Smaers);
            Assert.IsNotNull(health.Smaers.Status);
        }

        [TestMethod()]
        public void QueryParametersArray()
        {
            var client = GetClient(SIGN_API_V1_PATH);
            var query = new Dictionary<string, object>();

            query.Add("states", new string[] {"INITIALIZED", "DISABLED"});

            var response = client.Request("GET", "/tss", null, null, query);

            Assert.IsNotNull(response);
            Assert.AreEqual(200, response.Status);
        }

        [TestMethod()]
        public void QueryParametersString()
        {
            var client = GetClient(SIGN_API_V1_PATH);
            var query = new Dictionary<string, object>();

            query.Add("id", "10");

            var response = client.Request("GET", "/tss", null, null, query);

            Assert.IsNotNull(response);
            Assert.AreEqual(200, response.Status);
        }
    }
}