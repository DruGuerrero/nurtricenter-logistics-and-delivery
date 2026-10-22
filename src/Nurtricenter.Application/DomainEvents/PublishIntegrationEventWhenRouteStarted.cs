namespace Nurtricenter.Application.DomainEvents;

using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Route.Events;

/// <summary>
/// Translates the outbox message that carries <see cref="RouteStartedEvent"/> into the
/// <see cref="RouteStarted"/> external contract and publishes it to RabbitMQ.
/// </summary>
public sealed class PublishIntegrationEventWhenRouteStarted : INotificationHandler<OutboxMessage<DomainEvent>>
{
    public const string Destination = NurtricenterExchanges.RouteStarted;
    public const string Source = "nurtricenter";

    private readonly ILogger<PublishIntegrationEventWhenRouteStarted> _logger;
    private readonly IExternalPublisher _publisher;

    public PublishIntegrationEventWhenRouteStarted(
        ILogger<PublishIntegrationEventWhenRouteStarted> logger,
        IExternalPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(OutboxMessage<DomainEvent> notification, CancellationToken cancellationToken = default)
    {
        if (notification.Content is not RouteStartedEvent routeStarted)
        {
            return;
        }

        var message = new RouteStarted(notification.CorrelationId, Source)
        {
            RouteId = routeStarted.RouteId
        };

        _logger.LogInformation(
            "Publishing {EventType} for route {RouteId}",
            message.EventType, message.RouteId);

        await _publisher.PublishAsync(message, Destination, declareDestination: true);
    }
}
