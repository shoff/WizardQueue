using System;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog.Web;
using Wizard.Shared;
using Wizard.Shared.Messages;


namespace Wizard.Subscriber
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string hubUrl = "https://localhost:5001/eventhub";
            var builder = new HubConnectionBuilder().WithUrl(hubUrl);
            //Set connection
            var connection = builder.Build();
            await connection.StartAsync();


            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                bus.SubscribeAsync<EventMessage>("eventMessages", message => Task.Factory.StartNew(async () =>
                {
                    Console.WriteLine($"{message.EventName} {message.EventDescription}");
                    await connection.InvokeAsync<EventMessage>("EventMessageArrived", message);

                }).ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        Console.WriteLine("Finished processing all messages");
                    }
                    else
                    {
                        // Don't catch this, it is caught further up the hierarchy and results in being sent to the default error queue
                        // on the broker
                        throw new EasyNetQException(
                            "Message processing exception - look in the default error queue (broker)");
                    }
                }));

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }
    }
}