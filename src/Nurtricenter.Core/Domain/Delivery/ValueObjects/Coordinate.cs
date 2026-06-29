namespace Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed record Coordinate
{
    public double X { get; }
    public double Y { get; }

    public Coordinate(double x, double y)
    {
        X = x;
        Y = y;
    }

    private Coordinate() { }

    public override string ToString() => $"({X}, {Y})";
}
