namespace Nurtricenter.Api.Endpoints.Deliveries;

public sealed record AddDeliveryRequestItem(
    string PatientId,
    PackageDetailsRequest PackageDetails);

public sealed record PackageDetailsRequest(
    string PackageId,
    DestinationRequest Destination,
    string AdditionalDetails);

public sealed record DestinationRequest(
    string Address,
    CoordinateRequest AddressCoordinates);

public sealed record CoordinateRequest(double Lat, double Long);
