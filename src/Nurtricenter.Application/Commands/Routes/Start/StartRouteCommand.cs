namespace Nurtricenter.Application.Commands.Routes.Start;

using MediatR;

public sealed record StartRouteCommand(Guid RouteId) : IRequest<StartRouteResponse>;
