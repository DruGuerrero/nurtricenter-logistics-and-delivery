namespace Nurtricenter.Core.Domain.Route.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record CourierAssignedToRouteEvent(Guid RouteId, Guid CourierId) : DomainEvent;
