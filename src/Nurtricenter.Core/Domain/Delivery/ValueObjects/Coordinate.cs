namespace Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed record Coordinate
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    private Coordinate() { }

    public override string ToString() => $"({Latitude}, {Longitude})";
}
