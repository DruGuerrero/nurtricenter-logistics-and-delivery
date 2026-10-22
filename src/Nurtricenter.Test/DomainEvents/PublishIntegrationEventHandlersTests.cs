namespace Nurtricenter.Test.DomainEvents;

using FluentAssertions;
using Joseco.Communication.External.Contracts.Services;
using Joseco.DDD.Core.Abstractions;
using Joseco.Outbox.Contracts.Model;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nurtricenter.Application.DomainEvents;
using Nurtricenter.Contracts.Integration;
using Nurtricenter.Core.Domain.Courier.Enums;
using Nurtricenter.Core.Domain.Courier.Events;
using Nurtricenter.Core.Domain.Delivery.Events;
using Nurtricenter.Core.Domain.Route.Events;

public class PublishIntegrationEventHandlersTests
{
    private readonly Mock<IExternalPublisher> _publisher = new();

    private static OutboxMessage<DomainEvent> WithCorrelation(DomainEvent domainEvent) =>
        new(domainEvent) { CorrelationId = "corr-123" };

    [Fact]
    public async Task CourierCreated_PublishesMappedContractToItsExchange()
    {
        var courierId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenCourierCreated(
            NullLogger<PublishIntegrationEventWhenCourierCreated>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new CourierCreatedEvent(courierId, "Jane Doe", CourierStatus.Available)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<CourierCreated>(c =>
                c.CourierId == courierId
                && c.FullName == "Jane Doe"
                && c.Status == "Available"
                && c.EventType == "Courier.Created"
                && c.CorrelationId == "corr-123"
                && c.Source == "nurtricenter"),
            NurtricenterExchanges.CourierCreated,
            true), Times.Once);
    }

    [Fact]
    public async Task CourierStatusChanged_PublishesMappedContractToItsExchange()
    {
        var courierId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenCourierStatusChanged(
            NullLogger<PublishIntegrationEventWhenCourierStatusChanged>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new CourierStatusChangedEvent(courierId, CourierStatus.Available, CourierStatus.OnRoute)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<CourierStatusChanged>(c =>
                c.CourierId == courierId
                && c.OldStatus == "Available"
                && c.NewStatus == "OnRoute"
                && c.EventType == "Courier.StatusChanged"),
            NurtricenterExchanges.CourierStatusChanged,
            true), Times.Once);
    }

    [Fact]
    public async Task RouteCreated_PublishesMappedContractToItsExchange()
    {
        var routeId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var scheduledDate = new DateOnly(2026, 10, 21);
        var handler = new PublishIntegrationEventWhenRouteCreated(
            NullLogger<PublishIntegrationEventWhenRouteCreated>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new RouteCreatedEvent(routeId, courierId, scheduledDate)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<RouteCreated>(c =>
                c.RouteId == routeId
                && c.CourierId == courierId
                && c.ScheduledDate == scheduledDate
                && c.EventType == "Route.Created"),
            NurtricenterExchanges.RouteCreated,
            true), Times.Once);
    }

    [Fact]
    public async Task CourierAssignedToRoute_PublishesMappedContractToItsExchange()
    {
        var routeId = Guid.NewGuid();
        var courierId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenCourierAssignedToRoute(
            NullLogger<PublishIntegrationEventWhenCourierAssignedToRoute>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new CourierAssignedToRouteEvent(routeId, courierId)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<CourierAssignedToRoute>(c =>
                c.RouteId == routeId
                && c.CourierId == courierId
                && c.EventType == "Route.CourierAssigned"),
            NurtricenterExchanges.CourierAssignedToRoute,
            true), Times.Once);
    }

    [Fact]
    public async Task DeliveryAddedToRoute_PublishesMappedContractToItsExchange()
    {
        var routeId = Guid.NewGuid();
        var deliveryId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenDeliveryAddedToRoute(
            NullLogger<PublishIntegrationEventWhenDeliveryAddedToRoute>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new DeliveryAddedToRouteEvent(routeId, deliveryId, "package-1", "patient-1")), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<DeliveryAddedToRoute>(c =>
                c.RouteId == routeId
                && c.DeliveryId == deliveryId
                && c.PackageId == "package-1"
                && c.PatientId == "patient-1"
                && c.EventType == "Route.DeliveryAdded"),
            NurtricenterExchanges.DeliveryAddedToRoute,
            true), Times.Once);
    }

    [Fact]
    public async Task RouteStarted_PublishesMappedContractToItsExchange()
    {
        var routeId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenRouteStarted(
            NullLogger<PublishIntegrationEventWhenRouteStarted>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new RouteStartedEvent(routeId)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<RouteStarted>(c => c.RouteId == routeId && c.EventType == "Route.Started"),
            NurtricenterExchanges.RouteStarted,
            true), Times.Once);
    }

    [Fact]
    public async Task RouteCompleted_PublishesMappedContractToItsExchange()
    {
        var routeId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenRouteCompleted(
            NullLogger<PublishIntegrationEventWhenRouteCompleted>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new RouteCompletedEvent(routeId)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<RouteCompleted>(c => c.RouteId == routeId && c.EventType == "Route.Completed"),
            NurtricenterExchanges.RouteCompleted,
            true), Times.Once);
    }

    [Fact]
    public async Task RouteCancelled_PublishesMappedContractToItsExchange()
    {
        var routeId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenRouteCancelled(
            NullLogger<PublishIntegrationEventWhenRouteCancelled>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new RouteCancelledEvent(routeId)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<RouteCancelled>(c => c.RouteId == routeId && c.EventType == "Route.Cancelled"),
            NurtricenterExchanges.RouteCancelled,
            true), Times.Once);
    }

    [Fact]
    public async Task DeliveryCompleted_PublishesMappedContractToItsExchange()
    {
        var deliveryId = Guid.NewGuid();
        var routeId = Guid.NewGuid();
        var deliveredAt = new DateTime(2026, 10, 21, 10, 30, 0, DateTimeKind.Utc);
        var handler = new PublishIntegrationEventWhenDeliveryCompleted(
            NullLogger<PublishIntegrationEventWhenDeliveryCompleted>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new DeliveryCompletedEvent(deliveryId, routeId, deliveredAt)), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<DeliveryCompleted>(c =>
                c.DeliveryId == deliveryId
                && c.RouteId == routeId
                && c.DeliveredAt == deliveredAt
                && c.EventType == "Delivery.Completed"),
            NurtricenterExchanges.DeliveryCompleted,
            true), Times.Once);
    }

    [Fact]
    public async Task DeliveryFailed_PublishesMappedContractToItsExchange()
    {
        var deliveryId = Guid.NewGuid();
        var routeId = Guid.NewGuid();
        var handler = new PublishIntegrationEventWhenDeliveryFailed(
            NullLogger<PublishIntegrationEventWhenDeliveryFailed>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new DeliveryFailedEvent(deliveryId, routeId, "recipient absent")), CancellationToken.None);

        _publisher.Verify(p => p.PublishAsync(
            It.Is<DeliveryFailed>(c =>
                c.DeliveryId == deliveryId
                && c.RouteId == routeId
                && c.Reason == "recipient absent"
                && c.EventType == "Delivery.Failed"),
            NurtricenterExchanges.DeliveryFailed,
            true), Times.Once);
    }

    [Fact]
    public async Task Handler_IgnoresUnrelatedDomainEvents()
    {
        var handler = new PublishIntegrationEventWhenDeliveryFailed(
            NullLogger<PublishIntegrationEventWhenDeliveryFailed>.Instance, _publisher.Object);

        await handler.Handle(WithCorrelation(new CourierCreatedEvent(Guid.NewGuid(), "Jane Doe", CourierStatus.Available)), CancellationToken.None);

        _publisher.Verify(
            p => p.PublishAsync(It.IsAny<DeliveryFailed>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
    }
}
