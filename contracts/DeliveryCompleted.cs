namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a delivery was completed.
/// </summary>
public record DeliveryCompleted : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public DeliveryCompleted()
    {
        DeliveryId = Guid.Empty;
        RouteId = Guid.Empty;
        DeliveredAt = DateTime.MinValue;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public DeliveryCompleted(string? correlationId, string? source)
        : base(correlationId, source)
    {
        DeliveryId = Guid.Empty;
        RouteId = Guid.Empty;
        DeliveredAt = DateTime.MinValue;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Delivery.Completed";

    public Guid DeliveryId { get; init; }

    public Guid RouteId { get; init; }

    /// <summary>Moment the delivery was confirmed, exactly as <c>DeliveryCompletedEvent.DeliveredAt</c> carried it.</summary>
    public DateTime DeliveredAt { get; init; }
}
