namespace Nurtricenter.Application.Commands.Deliveries.Add;

public sealed record AddDeliveriesResponse(Guid RouteId, IReadOnlyList<DeliveryItemResponse> Deliveries);
