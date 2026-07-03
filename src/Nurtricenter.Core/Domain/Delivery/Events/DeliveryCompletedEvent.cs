namespace Nurtricenter.Core.Domain.Delivery.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record DeliveryCompletedEvent(Guid DeliveryId, Guid RouteId, DateTime DeliveredAt) : DomainEvent;
