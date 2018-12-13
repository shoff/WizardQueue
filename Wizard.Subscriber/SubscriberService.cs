using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wizard.Shared;
using Wizard.Shared.Messages;

namespace Wizard.Subscriber
{
    public class SubscriberService : BackgroundService
    {
        private IBus bus;
        public SubscriberService(ILogger<SubscriberService> logger)
            : base(logger)
        {
            this.bus = RabbitHutch.CreateBus("host=localhost");
            this.bus.SubscribeAsync<EventMessage>("eventMessages", HandleMessageAsync);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Subscriber service is stopping.");
            this.logger.LogInformation("Subscriber service is stopping.");
            this.bus.Dispose();
            return Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            do
            {
                this.logger.LogInformation("BackgroundService.ExecuteAsync.");
                Process();
                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Task.Delay(1000, cancellationToken); // 5 seconds delay
            }
            while (!cancellationToken.IsCancellationRequested);

            return Task.CompletedTask;
        }

        protected override Task Process()
        {
            return Task.CompletedTask;
            
        }

        private async Task HandleMessageAsync(EventMessage message)
        {
            string hubUrl = "https://localhost:5001/eventhub";
            var builder = new HubConnectionBuilder().WithUrl(hubUrl);
            Console.WriteLine(JsonConvert.SerializeObject(message));
            //Set connection
            var connection = builder.Build();
            await connection.StartAsync();
            await connection.InvokeAsync<EventMessage>("EventMessageArrived", message);
        }
    }
}