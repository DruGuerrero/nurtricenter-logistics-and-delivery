#pragma warning disable CS8618 // Parameterless constructor for serialization

namespace Nurtricenter.Core.Domain.Delivery;

using Nurtricenter.Core.Domain.Base;
using Nurtricenter.Core.Domain.Delivery.Enums;
using Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed class Delivery : AggregateRoot<Guid>
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
    }

    private Delivery() : base(Guid.Empty) { }

    public void RegisterSuccessfulDelivery(DeliveryConfirmation confirmation)
    {
        ArgumentNullException.ThrowIfNull(confirmation);

        if (Status != DeliveryStatus.InProgress && Status != DeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot register a successful delivery when the delivery is {Status}.");

        Confirmation = confirmation;
        Status = DeliveryStatus.Delivered;
    }

    public void RegisterFailedDelivery(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason cannot be empty.", nameof(reason));

        if (Status != DeliveryStatus.InProgress && Status != DeliveryStatus.Pending)
            throw new InvalidOperationException(
                $"Cannot register a failed delivery when the delivery is {Status}.");

        Status = DeliveryStatus.Failed;
    }
}
