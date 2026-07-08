namespace Nurtricenter.Core.Domain.Route.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record RouteCompletedEvent(Guid RouteId) : DomainEvent;
