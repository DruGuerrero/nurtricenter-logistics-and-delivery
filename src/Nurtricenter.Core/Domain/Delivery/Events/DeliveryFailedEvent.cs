namespace Nurtricenter.Core.Domain.Delivery.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record DeliveryFailedEvent(Guid DeliveryId, Guid RouteId, string Reason) : DomainEvent;
