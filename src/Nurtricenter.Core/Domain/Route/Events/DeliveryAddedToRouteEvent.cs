namespace Nurtricenter.Core.Domain.Route.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record DeliveryAddedToRouteEvent(
    Guid RouteId,
    Guid DeliveryId,
    string PackageId,
    string PatientId) : DomainEvent;
