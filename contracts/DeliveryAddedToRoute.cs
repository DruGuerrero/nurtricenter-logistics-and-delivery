namespace Nurtricenter.Contracts.Integration;

using Joseco.Communication.External.Contracts.Message;

/// <summary>
/// External contract announcing that a delivery was added to a route.
/// </summary>
public record DeliveryAddedToRoute : IntegrationMessage
{
    /// <summary>
    /// Creates the contract with every member at a safe default. Required by the
    /// Newtonsoft deserializer used by the RabbitMQ library.
    /// </summary>
    public DeliveryAddedToRoute()
    {
        RouteId = Guid.Empty;
        DeliveryId = Guid.Empty;
        PackageId = string.Empty;
        PatientId = string.Empty;
    }

    /// <summary>
    /// Creates the contract and forwards the trace metadata to the shared envelope.
    /// </summary>
    public DeliveryAddedToRoute(string? correlationId, string? source)
        : base(correlationId, source)
    {
        RouteId = Guid.Empty;
        DeliveryId = Guid.Empty;
        PackageId = string.Empty;
        PatientId = string.Empty;
    }

    /// <summary>
    /// Stable discriminator carried on the wire. Consumers must switch on this value, not on the
    /// CLR type.
    /// </summary>
    public string EventType => "Route.DeliveryAdded";

    public Guid RouteId { get; init; }

    public Guid DeliveryId { get; init; }

    /// <summary>Package identifier, exactly as <c>DeliveryAddedToRouteEvent.PackageId</c> carried it.</summary>
    public string PackageId { get; init; }

    /// <summary>Patient identifier, exactly as <c>DeliveryAddedToRouteEvent.PatientId</c> carried it.</summary>
    public string PatientId { get; init; }
}
