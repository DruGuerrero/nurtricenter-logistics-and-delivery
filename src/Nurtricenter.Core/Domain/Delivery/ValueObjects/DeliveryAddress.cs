#pragma warning disable CS8618 // Parameterless constructor for serialization

namespace Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed record DeliveryAddress
{
    public string Description { get; }
    public Coordinate PlanarCoordinate { get; }

    public DeliveryAddress(string description, Coordinate planarCoordinate)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        Description = description;
        PlanarCoordinate = planarCoordinate ?? throw new ArgumentNullException(nameof(planarCoordinate));
    }

    private DeliveryAddress() { }
}
