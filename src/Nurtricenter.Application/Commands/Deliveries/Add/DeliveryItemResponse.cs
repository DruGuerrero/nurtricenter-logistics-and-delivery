namespace Nurtricenter.Application.Commands.Deliveries.Add;

public sealed record DeliveryItemResponse(string PatientId, Guid DeliveryId, string Status);
