namespace Nurtricenter.Application.Commands.Deliveries.Add;

using MediatR;

public sealed record AddDeliveriesCommand(IReadOnlyList<AddDeliveryItem> Items) : IRequest<AddDeliveriesResponse>;
