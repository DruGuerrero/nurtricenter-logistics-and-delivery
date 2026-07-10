namespace Nurtricenter.Application.Commands.Deliveries.Add;

public sealed record AddDeliveryItem(
    string PatientId,
    string PackageId,
    string Address,
    double Latitude,
    double Longitude,
    string AdditionalDetails);
