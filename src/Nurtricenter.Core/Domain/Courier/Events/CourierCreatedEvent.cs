namespace Nurtricenter.Core.Domain.Courier.Events;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Courier.Enums;

public sealed record CourierCreatedEvent(Guid CourierId, string FullName, CourierStatus Status) : DomainEvent;
