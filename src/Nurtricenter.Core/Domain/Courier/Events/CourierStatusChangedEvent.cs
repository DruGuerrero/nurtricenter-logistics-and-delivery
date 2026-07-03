namespace Nurtricenter.Core.Domain.Courier.Events;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Courier.Enums;

public sealed record CourierStatusChangedEvent(Guid CourierId, CourierStatus OldStatus, CourierStatus NewStatus) : DomainEvent;
