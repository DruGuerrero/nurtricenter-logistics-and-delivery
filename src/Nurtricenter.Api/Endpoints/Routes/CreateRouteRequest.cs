namespace Nurtricenter.Api.Endpoints.Routes;

public sealed record CreateRouteRequest(Guid CourierId, DateOnly ScheduledDate);
