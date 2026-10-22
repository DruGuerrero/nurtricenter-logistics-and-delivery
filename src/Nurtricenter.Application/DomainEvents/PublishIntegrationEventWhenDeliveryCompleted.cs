namespace Nurtricenter.Application.DomainEvents;

using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Delivery.Events;

/// <summary>
/// Translates the outbox message that carries <see cref="DeliveryCompletedEvent"/> into the
/// <see cref="DeliveryCompleted"/> external contract and publishes it to RabbitMQ.
/// </summary>
public sealed class PublishIntegrationEventWhenDeliveryCompleted : INotificationHandler<OutboxMessage<DomainEvent>>
{
    public const string Destination = NurtricenterExchanges.DeliveryCompleted;
    public const string Source = "nurtricenter";

    private readonly ILogger<PublishIntegrationEventWhenDeliveryCompleted> _logger;
    private readonly IExternalPublisher _publisher;

    public PublishIntegrationEventWhenDeliveryCompleted(
        ILogger<PublishIntegrationEventWhenDeliveryCompleted> logger,
        IExternalPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(OutboxMessage<DomainEvent> notification, CancellationToken cancellationToken = default)
    {
        if (notification.Content is not DeliveryCompletedEvent deliveryCompleted)
        {
            return;
        }

        var message = new DeliveryCompleted(notification.CorrelationId, Source)
        {
            DeliveryId = deliveryCompleted.DeliveryId,
            RouteId = deliveryCompleted.RouteId,
            DeliveredAt = deliveryCompleted.DeliveredAt
        };

        _logger.LogInformation(
            "Publishing {EventType} for delivery {DeliveryId} on route {RouteId}",
            message.EventType, message.DeliveryId, message.RouteId);

        await _publisher.PublishAsync(message, Destination, declareDestination: true);
    }
}
