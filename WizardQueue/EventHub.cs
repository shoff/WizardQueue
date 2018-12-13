using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Wizard.Shared.Models;

namespace WizardQueue
{
    public class EventHub : Hub
    {
        private readonly ILogger<EventHub> logger;
        private static readonly HashSet<string> ConnectedIds = new HashSet<string>();

        public EventHub(ILogger<EventHub> logger)
        {
            this.logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            ConnectedIds.Add(Context.ConnectionId);
            this.logger.LogInformation($"New connection {Context.ConnectionId} joined EventHub.");
            await Clients.All.SendAsync("SendAction", "joined", ConnectedIds.Count);
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            ConnectedIds.Remove(Context.ConnectionId);
            this.logger.LogInformation($"{Context.ConnectionId} left EventHub.");
            await Clients.All.SendAsync("SendAction", "left", ConnectedIds.Count);
        }

        public void EventMessageArrived(EventModel eventMessage)
        {
            this.logger.LogInformation($"Sending message to {ConnectedIds.Count}");
            Clients.All.SendAsync("EventMessageArrived", eventMessage);
        }
    }
}