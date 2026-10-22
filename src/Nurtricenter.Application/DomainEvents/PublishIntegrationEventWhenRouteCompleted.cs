namespace Nurtricenter.Application.DomainEvents;

using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Route.Events;

/// <summary>
/// Translates the outbox message that carries <see cref="RouteCompletedEvent"/> into the
/// <see cref="RouteCompleted"/> external contract and publishes it to RabbitMQ.
/// </summary>
public sealed class PublishIntegrationEventWhenRouteCompleted : INotificationHandler<OutboxMessage<DomainEvent>>
{
    public const string Destination = NurtricenterExchanges.RouteCompleted;
    public const string Source = "nurtricenter";

    private readonly ILogger<PublishIntegrationEventWhenRouteCompleted> _logger;
    private readonly IExternalPublisher _publisher;

    public PublishIntegrationEventWhenRouteCompleted(
        ILogger<PublishIntegrationEventWhenRouteCompleted> logger,
        IExternalPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(OutboxMessage<DomainEvent> notification, CancellationToken cancellationToken = default)
    {
        if (notification.Content is not RouteCompletedEvent routeCompleted)
        {
            return;
        }

        var message = new RouteCompleted(notification.CorrelationId, Source)
        {
            RouteId = routeCompleted.RouteId
        };

        _logger.LogInformation(
            "Publishing {EventType} for route {RouteId}",
            message.EventType, message.RouteId);

        await _publisher.PublishAsync(message, Destination, declareDestination: true);
    }
}
