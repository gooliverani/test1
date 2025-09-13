using Microsoft.AspNetCore.SignalR;

namespace AccessControl.Api.RealTime;

public class AccessEventsHub : Hub
{
    private readonly RealTimeEventPublisher _publisher;

    public AccessEventsHub(RealTimeEventPublisher publisher)
    {
        _publisher = publisher;
    }

    // SignalR hub for /hubs/access-events
}