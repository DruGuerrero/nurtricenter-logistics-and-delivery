namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a delivery failed.
/// </summary>
public record DeliveryFailed : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public DeliveryFailed()
    {
        DeliveryId = Guid.Empty;
        RouteId = Guid.Empty;
        Reason = string.Empty;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public DeliveryFailed(string? correlationId, string? source)
        : base(correlationId, source)
    {
        DeliveryId = Guid.Empty;
        RouteId = Guid.Empty;
        Reason = string.Empty;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Delivery.Failed";

    public Guid DeliveryId { get; init; }

    public Guid RouteId { get; init; }

    /// <summary>Failure reason, exactly as <c>DeliveryFailedEvent.Reason</c> carried it.</summary>
    public string Reason { get; init; }
}
