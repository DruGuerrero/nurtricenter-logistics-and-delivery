namespace Nurtricenter.Application.Commands.Routes.Create;

using MediatR;

public sealed record CreateRouteCommand(Guid CourierId, DateOnly ScheduledDate) : IRequest<RouteResponse>;
