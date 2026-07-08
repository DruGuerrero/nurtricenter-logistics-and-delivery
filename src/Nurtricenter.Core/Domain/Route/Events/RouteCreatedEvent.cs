namespace Nurtricenter.Core.Domain.Route.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record RouteCreatedEvent(Guid RouteId, Guid CourierId, DateOnly ScheduledDate) : DomainEvent;
