namespace Nurtricenter.Application.Commands.Couriers.Create;

using Nurtricenter.Core.Domain.Courier.Enums;

public sealed record CourierResponse(Guid Id, string FullName, CourierStatus Status);
