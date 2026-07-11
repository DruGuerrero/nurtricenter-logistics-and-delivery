namespace Nurtricenter.Api.Endpoints.Routes;

using Microsoft.AspNetCore.Mvc;

public sealed record StartRouteRequest([FromRoute] Guid RouteId);
