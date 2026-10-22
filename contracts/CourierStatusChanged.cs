namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a courier's status changed.
/// </summary>
public record CourierStatusChanged : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public CourierStatusChanged()
    {
        CourierId = Guid.Empty;
        OldStatus = string.Empty;
        NewStatus = string.Empty;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public CourierStatusChanged(string? correlationId, string? source)
        : base(correlationId, source)
    {
        CourierId = Guid.Empty;
        OldStatus = string.Empty;
        NewStatus = string.Empty;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Courier.StatusChanged";

    public Guid CourierId { get; init; }

    /// <summary>Previous status as a string, so consumers need no knowledge of this service's enum.</summary>
    public string OldStatus { get; init; }

    /// <summary>New status as a string, so consumers need no knowledge of this service's enum.</summary>
    public string NewStatus { get; init; }
}
