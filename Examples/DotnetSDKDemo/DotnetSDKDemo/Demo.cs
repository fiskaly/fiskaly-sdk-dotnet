using Fiskaly;
using Fiskaly.Client.Models;
using Fiskaly.Errors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSDKDemo
{
    public class Demo
    {
        public static string API_KEY = Environment.GetEnvironmentVariable("FISKALY_API_KEY");
        public static string API_SECRET = Environment.GetEnvironmentVariable("FISKALY_API_SECRET");
        public static FiskalyHttpClient client;

        public static string TSS_ID = Guid.NewGuid().ToString();
        public static string CLIENT_ID = Guid.NewGuid().ToString();
        public static string TX_ID = Guid.NewGuid().ToString();

        public static void Main(string[] args)
        {
            Console.WriteLine("Using API-Key: \"" + API_KEY + "\"");
            Console.WriteLine("Using API-Key: \"" + API_SECRET + "\"");

            client = new FiskalyHttpClient(API_KEY, API_SECRET, "https://kassensichv.io/api/v1");
            client.ConfigureClient(new Fiskaly.Client.ClientConfiguration 
                { DebugLevel = Fiskaly.Client.DebugLevel.EVERYTHING, DebugFile = "./fiskaly-demo.log" }
            );

            Console.WriteLine("\n" + client.Version() + "\n");

            try
            {
                Console.WriteLine("Creating TSS with ID: " + TSS_ID);
                CreateTss();
            } catch (FiskalyHttpError error)
            {
                Console.Error.WriteLine("Error occurred while trying to create a new TSS: " + error);
                Console.ReadLine();
                return;
            }

            try
            {
                Console.WriteLine("Creating Client with ID: " + CLIENT_ID);
                CreateClient();
            } catch (FiskalyHttpError error)
            {
                Console.Error.WriteLine("Error occurred while trying to create a new client: " + error);
                Console.ReadLine();
                return;
            }

            try
            {
                Console.WriteLine("Starting transaction with ID: " + TX_ID);
                StartTransaction();

            } catch (FiskalyHttpError error)
            {
                Console.Error.WriteLine("Error occurred while trying to start: " + error);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\n\nfiskaly SDK demo ran successfully.");
            Console.ReadLine();
        }

        public static void CreateClient()
        {
            var clientSerial = "fiskaly-dotnet-sdk-test-client";
            var createTssPayload = "{ \"serial_number\": \"" + clientSerial + "\" }";

            var bodyBytes = Encoding.UTF8.GetBytes(createTssPayload);

            var response = client
                .Request("PUT", "/tss/" + TSS_ID + "/client/" + CLIENT_ID, bodyBytes, null, null);

            var decodedBody = Encoding.UTF8.GetString(response.Body);
            JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedBody);
        }

        public static void CreateTss()
        {
            var descriptionContent = "TSS created by .NET SDK CreateClient at "
                + DateTime.Now.ToString();
            var createClientPayload = "{ \"description\": \""
                + descriptionContent + "\", \"state\": \"UNINITIALIZED\" }";

            var bodyBytes = Encoding.UTF8.GetBytes(createClientPayload);

            var response = client
                .Request("PUT", "/tss/" + TSS_ID, bodyBytes, null, null);

            var decodedBody = Encoding.UTF8.GetString(response.Body);

            var initializeTssPayload = "{ \"state\": \"INITIALIZED\" }";
            bodyBytes = Encoding.UTF8.GetBytes(initializeTssPayload);

            client.Request("PUT", "/tss/" + TSS_ID, bodyBytes, null, null);
        }

        public static FiskalyHttpResponse StartTransaction()
        {
            var payload = "{ \"state\": \"ACTIVE\", \"client_id\": \"" + CLIENT_ID + "\" }";
            var bodyBytes = Encoding.UTF8.GetBytes(payload);

            var response = client.Request("PUT", "/tss/" + TSS_ID + "/tx/" + TX_ID, bodyBytes, null, null);

            return response;
        }
    }
}
