namespace Nurtricenter.Core.Domain.Delivery.Events;

using Joseco.DDD.Core.Abstractions;

public sealed record DeliveryCreatedEvent(Guid DeliveryId, Guid RouteId, string PackageId, string PatientId) : DomainEvent;
