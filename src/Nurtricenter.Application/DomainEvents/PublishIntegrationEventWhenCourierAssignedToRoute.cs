namespace Nurtricenter.Application.DomainEvents;

using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Route.Events;

/// <summary>
/// Translates the outbox message that carries <see cref="CourierAssignedToRouteEvent"/> into the
/// <see cref="CourierAssignedToRoute"/> external contract and publishes it to RabbitMQ.
/// </summary>
public sealed class PublishIntegrationEventWhenCourierAssignedToRoute : INotificationHandler<OutboxMessage<DomainEvent>>
{
    public const string Destination = NurtricenterExchanges.CourierAssignedToRoute;
    public const string Source = "nurtricenter";

    private readonly ILogger<PublishIntegrationEventWhenCourierAssignedToRoute> _logger;
    private readonly IExternalPublisher _publisher;

    public PublishIntegrationEventWhenCourierAssignedToRoute(
        ILogger<PublishIntegrationEventWhenCourierAssignedToRoute> logger,
        IExternalPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(OutboxMessage<DomainEvent> notification, CancellationToken cancellationToken = default)
    {
        if (notification.Content is not CourierAssignedToRouteEvent courierAssigned)
        {
            return;
        }

        var message = new CourierAssignedToRoute(notification.CorrelationId, Source)
        {
            RouteId = courierAssigned.RouteId,
            CourierId = courierAssigned.CourierId
        };

        _logger.LogInformation(
            "Publishing {EventType} for route {RouteId} and courier {CourierId}",
            message.EventType, message.RouteId, message.CourierId);

        await _publisher.PublishAsync(message, Destination, declareDestination: true);
    }
}
