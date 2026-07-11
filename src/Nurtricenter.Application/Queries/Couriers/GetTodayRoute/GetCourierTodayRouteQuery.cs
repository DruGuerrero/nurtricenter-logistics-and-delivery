namespace Nurtricenter.Application.Queries.Couriers.GetTodayRoute;

using MediatR;

public sealed record GetCourierTodayRouteQuery(Guid CourierId) : IRequest<CourierTodayRouteResponse>;
