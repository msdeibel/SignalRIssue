using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;

namespace SignalRClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client started!");

            Thread.Sleep(3000);

            var p = new Program();
            p.OpenConnection(args[0]);

            p.TriggerValueCreation();

            p.SubscribeToVarsSignalR(new int[] { 1, 3, 5 });

            var t = new System.Timers.Timer(20000);
            t.Elapsed += (sender, e) => p.UnsubscribeFromVarsSignalR(new int[] { 1, 3 }); 
            t.AutoReset = false;
            t.Start();

            var t2 = new System.Timers.Timer(30000);
            t2.Elapsed += (sender, e) => p.SubscribeToVarsSignalR(new int[] { 2, 4 });
            t2.AutoReset = false;
            t2.Start();

            Console.ReadKey();
        }

        HubConnection hubConnection;
        private void OpenConnection(string uri)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(uri)
                .ConfigureLogging(logging =>
                {
                    // Log to the Console
                    logging.AddConsole();

                    // This will set ALL logging to Debug level
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .Build();

            hubConnection.On<string>("NewValue", WriteMessageToConsole);

            try
            {
                hubConnection.StartAsync().Wait();
                Console.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void WriteMessageToConsole(string message)
        {
            Console.WriteLine(message);
        }

        private void TriggerValueCreation()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.GetAsync(new Uri($"http://localhost:5000/api/RegisterValues/LiveVariableValues")).Wait();
        }

        private void SubscribeToVarsSignalR(int[] variableIds)
        {
            hubConnection.SendCoreAsync("SubscribeToVariables", new[] { variableIds });
            Console.WriteLine("Subscribed to variables.");
        }

        private void UnsubscribeFromVarsSignalR(int[] variableIds)
        {
            hubConnection.SendCoreAsync("UnsubscribeFromVariables", new[] { variableIds });
            Console.WriteLine("Unsubscribed from variables.");
        }
    }
}
