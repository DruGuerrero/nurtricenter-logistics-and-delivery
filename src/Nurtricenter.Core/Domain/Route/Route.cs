namespace Nurtricenter.Core.Domain.Route;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Delivery.ValueObjects;
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
        if (Status == RouteStatus.Completed || Status == RouteStatus.Cancelled || Status == RouteStatus.InProgress)
            throw new InvalidOperationException("Cannot assign a courier to a completed, cancelled or in progress route.");

        CourierId = courierId;

        AddDomainEvent(new CourierAssignedToRouteEvent(Id, courierId));
    }

    public void AddDelivery(ValidatedPackage package, DeliveryAddress address)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(address);

        if (Status == RouteStatus.Completed || Status == RouteStatus.Cancelled)
            throw new InvalidOperationException("Cannot add deliveries to a completed or cancelled route.");

        var delivery = new DeliveryEntity(Guid.NewGuid(), package, address);
        delivery.RouteId = Id;

        _deliveries.Add(delivery);

        AddDomainEvent(new DeliveryAddedToRouteEvent(Id, delivery.Id, package.PackageId, package.PatientId));
    }

    public void StartRoute()
    {
        if (Status != RouteStatus.Pending)
            throw new InvalidOperationException("Only pending routes can be started.");

        Status = RouteStatus.InProgress;

        foreach (var delivery in _deliveries)
        {
            delivery.StartDelivery();
        }

        AddDomainEvent(new RouteStartedEvent(Id));
    }

    public void CompleteRoute()
    {
        if (Status != RouteStatus.InProgress)
            throw new InvalidOperationException("Only in-progress routes can be completed.");

        if (_deliveries.Any(d => !d.IsTerminal))
            throw new InvalidOperationException(
                "Cannot complete a route with pending or in-progress deliveries.");

        Status = RouteStatus.Completed;

        AddDomainEvent(new RouteCompletedEvent(Id));
    }

    public void CancelRoute()
    {
        if (Status == RouteStatus.Completed || Status == RouteStatus.Cancelled)
            throw new InvalidOperationException("A completed/cancelled route cannot be cancelled.");

        foreach (var delivery in _deliveries.Where(d => !d.IsTerminal))
        {
            delivery.RegisterFailedDelivery("Route was cancelled.");
        }

        Status = RouteStatus.Cancelled;

        AddDomainEvent(new RouteCancelledEvent(Id));
    }

    public void CompleteDelivery(Guid deliveryId, DeliveryConfirmation confirmation)
    {
        ArgumentNullException.ThrowIfNull(confirmation);

        var delivery = FindDelivery(deliveryId);
        delivery.RegisterSuccessfulDelivery(confirmation);
    }

    public void FailDelivery(Guid deliveryId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be empty.", nameof(reason));

        var delivery = FindDelivery(deliveryId);
        delivery.RegisterFailedDelivery(reason);
    }

    private DeliveryEntity FindDelivery(Guid deliveryId)
    {
        var delivery = _deliveries.FirstOrDefault(d => d.Id == deliveryId);

        if (delivery is null)
            throw new ArgumentException(
                $"Delivery '{deliveryId}' not found in this route.", nameof(deliveryId));

        return delivery;
    }
}
