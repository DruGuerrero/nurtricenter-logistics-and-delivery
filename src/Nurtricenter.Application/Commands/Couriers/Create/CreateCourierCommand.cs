namespace Nurtricenter.Application.Commands.Couriers.Create;

using MediatR;

public sealed record CreateCourierCommand(string FullName) : IRequest<CourierResponse>;
