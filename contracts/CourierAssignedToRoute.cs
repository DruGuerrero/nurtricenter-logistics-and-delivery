namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a courier was assigned to a route.
/// </summary>
public record CourierAssignedToRoute : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public CourierAssignedToRoute()
    {
        RouteId = Guid.Empty;
        CourierId = Guid.Empty;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public CourierAssignedToRoute(string? correlationId, string? source)
        : base(correlationId, source)
    {
        RouteId = Guid.Empty;
        CourierId = Guid.Empty;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Route.CourierAssigned";

    public Guid RouteId { get; init; }

    public Guid CourierId { get; init; }
}
