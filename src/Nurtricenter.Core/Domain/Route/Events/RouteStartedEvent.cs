namespace Nurtricenter.Core.Domain.Route.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record RouteStartedEvent(Guid RouteId) : DomainEvent;
