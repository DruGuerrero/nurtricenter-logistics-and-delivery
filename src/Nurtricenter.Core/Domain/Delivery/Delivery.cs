#pragma warning disable CS8618 // Private parameterless ctor for serialization

namespace Nurtricenter.Core.Domain.Delivery;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Delivery.Enums;
using Nurtricenter.Core.Domain.Delivery.Events;
using Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed class Delivery : Entity
{
    public Guid RouteId { get; internal set; }
    public ValidatedPackage Package { get; private set; }
    public DeliveryAddress Address { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DeliveryConfirmation? Confirmation { get; private set; }

    public bool IsTerminal => Status == DeliveryStatus.Delivered || Status == DeliveryStatus.Failed;

    internal Delivery(Guid id, ValidatedPackage package, DeliveryAddress address)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(address);

        Package = package;
        Address = address;
        Status = DeliveryStatus.Pending;
    }

    private Delivery() : base() { }

    internal void StartDelivery()
    {
        if (Status != DeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot start a delivery that is {Status}.");

        Status = DeliveryStatus.InProgress;
    }

    internal void RegisterSuccessfulDelivery(DeliveryConfirmation confirmation)
    {
        ArgumentNullException.ThrowIfNull(confirmation);

        if (Status != DeliveryStatus.InProgress && Status != DeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot register a successful delivery when the delivery is {Status}.");

        Confirmation = confirmation;
        Status = DeliveryStatus.Delivered;

        AddDomainEvent(new DeliveryCompletedEvent(Id, RouteId, confirmation.DeliveredAt));
    }

    internal void RegisterFailedDelivery(string reason)
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
