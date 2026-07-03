#pragma warning disable CS8618 // Private parameterless ctor for serialization

namespace Nurtricenter.Core.Domain.Delivery;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Delivery.Enums;
using Nurtricenter.Core.Domain.Delivery.Events;
using Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed class Delivery : AggregateRoot
{
    public Guid RouteId { get; private set; }
    public ValidatedPackage Package { get; private set; }
    public DeliveryAddress Address { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DeliveryConfirmation? Confirmation { get; private set; }

    public Delivery(
        Guid id,
        Guid routeId,
        ValidatedPackage package,
        DeliveryAddress address)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(address);

        RouteId = routeId;
        Package = package;
        Address = address;
        Status = DeliveryStatus.Pending;

        AddDomainEvent(new DeliveryCreatedEvent(id, routeId, package.PackageId, package.PatientId));
    }

    private Delivery() : base() { }

    public void RegisterSuccessfulDelivery(DeliveryConfirmation confirmation)
    {
        ArgumentNullException.ThrowIfNull(confirmation);

        if (Status != DeliveryStatus.InProgress && Status != DeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot register a successful delivery when the delivery is {Status}.");

        Confirmation = confirmation;
        Status = DeliveryStatus.Delivered;

        AddDomainEvent(new DeliveryCompletedEvent(Id, RouteId, confirmation.DeliveredAt));
    }

    public void RegisterFailedDelivery(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be empty.", nameof(reason));

        if (Status != DeliveryStatus.InProgress && Status != DeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot register a failed delivery when the delivery is {Status}.");

        Status = DeliveryStatus.Failed;

        AddDomainEvent(new DeliveryFailedEvent(Id, RouteId, reason));
    }
}
