namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that every delivery on a route reached a terminal state and the
/// route was completed.
/// </summary>
public record RouteCompleted : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public RouteCompleted()
    {
        RouteId = Guid.Empty;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public RouteCompleted(string? correlationId, string? source)
        : base(correlationId, source)
    {
        RouteId = Guid.Empty;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Route.Completed";

    public Guid RouteId { get; init; }
}
