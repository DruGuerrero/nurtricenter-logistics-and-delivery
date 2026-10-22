namespace Nurtricenter.Application.DomainEvents;

using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Route.Events;

/// <summary>
/// Translates the outbox message that carries <see cref="DeliveryAddedToRouteEvent"/> into the
/// <see cref="DeliveryAddedToRoute"/> external contract and publishes it to RabbitMQ.
/// </summary>
public sealed class PublishIntegrationEventWhenDeliveryAddedToRoute : INotificationHandler<OutboxMessage<DomainEvent>>
{
    public const string Destination = NurtricenterExchanges.DeliveryAddedToRoute;
    public const string Source = "nurtricenter";

    private readonly ILogger<PublishIntegrationEventWhenDeliveryAddedToRoute> _logger;
    private readonly IExternalPublisher _publisher;

    public PublishIntegrationEventWhenDeliveryAddedToRoute(
        ILogger<PublishIntegrationEventWhenDeliveryAddedToRoute> logger,
        IExternalPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(OutboxMessage<DomainEvent> notification, CancellationToken cancellationToken = default)
    {
        if (notification.Content is not DeliveryAddedToRouteEvent deliveryAdded)
        {
            return;
        }

        var message = new DeliveryAddedToRoute(notification.CorrelationId, Source)
        {
            RouteId = deliveryAdded.RouteId,
            DeliveryId = deliveryAdded.DeliveryId,
            PackageId = deliveryAdded.PackageId,
            PatientId = deliveryAdded.PatientId
        };

        _logger.LogInformation(
            "Publishing {EventType} for delivery {DeliveryId} on route {RouteId}",
            message.EventType, message.DeliveryId, message.RouteId);

        await _publisher.PublishAsync(message, Destination, declareDestination: true);
    }
}
