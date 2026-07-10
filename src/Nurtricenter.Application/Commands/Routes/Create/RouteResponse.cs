namespace Nurtricenter.Application.Commands.Routes.Create;

using Nurtricenter.Core.Domain.Route.Enums;

public sealed record RouteResponse(Guid Id, Guid CourierId, DateOnly ScheduledDate, RouteStatus Status);
