namespace Nurtricenter.Application.DomainEvents;

using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Route.Events;

/// <summary>
/// Translates the outbox message that carries <see cref="RouteCancelledEvent"/> into the
/// <see cref="RouteCancelled"/> external contract and publishes it to RabbitMQ.
/// </summary>
public sealed class PublishIntegrationEventWhenRouteCancelled : INotificationHandler<OutboxMessage<DomainEvent>>
{
    public const string Destination = NurtricenterExchanges.RouteCancelled;
    public const string Source = "nurtricenter";

    private readonly ILogger<PublishIntegrationEventWhenRouteCancelled> _logger;
    private readonly IExternalPublisher _publisher;

    public PublishIntegrationEventWhenRouteCancelled(
        ILogger<PublishIntegrationEventWhenRouteCancelled> logger,
        IExternalPublisher publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(OutboxMessage<DomainEvent> notification, CancellationToken cancellationToken = default)
    {
        if (notification.Content is not RouteCancelledEvent routeCancelled)
        {
            return;
        }

        var message = new RouteCancelled(notification.CorrelationId, Source)
        {
            RouteId = routeCancelled.RouteId
        };

        _logger.LogInformation(
            "Publishing {EventType} for route {RouteId}",
            message.EventType, message.RouteId);

        await _publisher.PublishAsync(message, Destination, declareDestination: true);
    }
}
