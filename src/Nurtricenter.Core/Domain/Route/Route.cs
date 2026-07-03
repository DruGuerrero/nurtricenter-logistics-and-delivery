namespace Nurtricenter.Core.Domain.Route;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Route.Enums;
using Nurtricenter.Core.Domain.Route.Events;
using DeliveryEntity = Nurtricenter.Core.Domain.Delivery.Delivery;

public sealed class Route : AggregateRoot
{
    private readonly List<DeliveryEntity> _deliveries = new();

    public Guid CourierId { get; private set; }
    public DateOnly ScheduledDate { get; private set; }
    public RouteStatus Status { get; private set; }
    public IReadOnlyList<DeliveryEntity> Deliveries => _deliveries.AsReadOnly();

    public Route(Guid id, Guid courierId, DateOnly scheduledDate)
        : base(id)
    {
        CourierId = courierId;
        ScheduledDate = scheduledDate;
        Status = RouteStatus.Pending;

        AddDomainEvent(new RouteCreatedEvent(id, courierId, scheduledDate));
    }

    private Route() : base() { }

    public void AssignCourier(Guid courierId)
    {
        if (Status == RouteStatus.Completed || Status == RouteStatus.Cancelled)
            throw new InvalidOperationException("Cannot assign a courier to a completed or cancelled route.");

        CourierId = courierId;

        AddDomainEvent(new CourierAssignedToRouteEvent(Id, courierId));
    }

    public void StartRoute()
    {
        if (Status != RouteStatus.Pending)
            throw new InvalidOperationException("Only pending routes can be started.");

        Status = RouteStatus.InProgress;

        AddDomainEvent(new RouteStartedEvent(Id));
    }

    public void CompleteRoute()
    {
        if (Status != RouteStatus.InProgress)
            throw new InvalidOperationException("Only in-progress routes can be completed.");

        Status = RouteStatus.Completed;

        AddDomainEvent(new RouteCompletedEvent(Id));
    }

    public void CancelRoute()
    {
        if (Status == RouteStatus.Completed)
            throw new InvalidOperationException("A completed route cannot be cancelled.");

        Status = RouteStatus.Cancelled;

        AddDomainEvent(new RouteCancelledEvent(Id));
    }

    public void AddDelivery(DeliveryEntity delivery)
    {
        ArgumentNullException.ThrowIfNull(delivery);

        if (Status == RouteStatus.Completed || Status == RouteStatus.Cancelled)
            throw new InvalidOperationException("Cannot add deliveries to a completed or cancelled route.");

        _deliveries.Add(delivery);

        AddDomainEvent(new DeliveryAddedToRouteEvent(Id, delivery.Id));
    }
}
