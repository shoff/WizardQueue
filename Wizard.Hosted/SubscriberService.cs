using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wizard.Shared;
using Wizard.Shared.Messages;

namespace Wizard.Hosted
{
    public class SubscriberService : BackgroundService
    {
        private IBus bus;
        private readonly HubConnection connection;

        public SubscriberService(ILogger<SubscriberService> logger)
            : base(logger)
        {
            string hubUrl = "https://localhost:5001/eventhub";
            var builder = new HubConnectionBuilder().WithUrl(hubUrl);
            //Set connection
            this.connection = builder.Build();
            this.connection.Closed += SignalRConnectionClosed;
        }

        private Task SignalRConnectionClosed(Exception arg)
        {
            this.logger.LogError(arg, arg.Message);
            // gracefully retry with polly
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Subscriber service is stopping.");
            this.logger.LogInformation("Subscriber service is stopping.");
            this.bus?.Dispose();
            var hubConnection = this.connection;
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
            }
        }

        protected override Task Process()
        {
            return Task.CompletedTask;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await connection.StartAsync(cancellationToken);
            this.bus = RabbitHutch.CreateBus("host=localhost");
            this.bus.SubscribeAsync<EventMessage>("eventMessages", HandleMessageAsync);
            Console.WriteLine("Listening for messages. Hit <return> to quit.");
            await base.StartAsync(cancellationToken);
        }

        private async Task HandleMessageAsync(EventMessage message)
        {

            Console.WriteLine(JsonConvert.SerializeObject(message));

            await connection.InvokeAsync<EventMessage>("EventMessageArrived", message);
        }
    }
}