namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a route was scheduled for a courier.
/// </summary>
public record RouteCreated : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public RouteCreated()
    {
        RouteId = Guid.Empty;
        CourierId = Guid.Empty;
        ScheduledDate = DateOnly.MinValue;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public RouteCreated(string? correlationId, string? source)
        : base(correlationId, source)
    {
        RouteId = Guid.Empty;
        CourierId = Guid.Empty;
        ScheduledDate = DateOnly.MinValue;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Route.Created";

    public Guid RouteId { get; init; }

    /// <summary>Courier the route was assigned to.</summary>
    public Guid CourierId { get; init; }

    /// <summary>Day the route is scheduled for. Serialized as an ISO date (yyyy-MM-dd).</summary>
    public DateOnly ScheduledDate { get; init; }
}
