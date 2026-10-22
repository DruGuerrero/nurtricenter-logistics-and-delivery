namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a courier was registered in the service.
/// </summary>
public record CourierCreated : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public CourierCreated()
    {
        CourierId = Guid.Empty;
        FullName = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public CourierCreated(string? correlationId, string? source)
        : base(correlationId, source)
    {
        CourierId = Guid.Empty;
        FullName = string.Empty;
        Status = string.Empty;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Courier.Created";

    public Guid CourierId { get; init; }

    /// <summary>Courier's full name, exactly as <c>CourierCreatedEvent.FullName</c> carried it.</summary>
    public string FullName { get; init; }

    /// <summary>Courier status as a string, so consumers need no knowledge of this service's enum.</summary>
    public string Status { get; init; }
}
