namespace Nurtricenter.Application.Commands.Deliveries.Add;

using Joseco.DDD.Core.Abstractions;
using Joseco.DDD.Core.Results;
using MediatR;
using Nurtricenter.Core.Interfaces.Services.ClinicService;
using Nurtricenter.Core.Domain.Delivery.ValueObjects;
using Nurtricenter.Core.Domain.Route.Repositories;

public sealed class AddDeliveriesHandler : IRequestHandler<AddDeliveriesCommand, AddDeliveriesResponse>
{
    private readonly IClinicService _clinicService;
    private readonly IRouteRepository _routeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddDeliveriesHandler(
        IClinicService clinicService,
        IRouteRepository routeRepository,
        IUnitOfWork unitOfWork)
    {
        _clinicService = clinicService;
        _routeRepository = routeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddDeliveriesResponse> Handle(
        AddDeliveriesCommand request,
        CancellationToken cancellationToken)
    {
        var patientIds = request.Items
            .Select(i => i.PatientId)
            .Distinct()
            .ToList();

        var patients = await _clinicService.GetPatientsContactInfoAsync(patientIds, cancellationToken);

        if (patients.Count != patientIds.Count)
        {
            var foundIds = patients.Select(p => p.PatientId).ToHashSet();
            var missingIds = patientIds.Where(id => !foundIds.Contains(id)).ToList();

            throw new DomainException(
                Error.NotFound(
                    "ClinicService.PatientsNotFound",
                    "The following patients were not found: {patientIds}.",
                    string.Join(", ", missingIds)));
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var route = await _routeRepository.GetLatestRouteForTodayAsync(today, cancellationToken);

        if (route is null)
        {
            throw new DomainException(
                Error.NotFound(
                    "Route.NoRouteForToday",
                    "No route found for today ({date}). Create a route first.",
                    today.ToString("yyyy-MM-dd")));
        }

        foreach (var item in request.Items)
        {
            var coordinate = new Coordinate(item.Latitude, item.Longitude);
            var address = new DeliveryAddress(item.Address, coordinate);
            var package = new ValidatedPackage(item.PackageId, item.PatientId, item.AdditionalDetails);

            route.AddDelivery(package, address);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        var responseDeliveries = route.Deliveries
            .TakeLast(request.Items.Count)
            .Select(d => new DeliveryItemResponse(
                d.Package.PatientId,
                d.Id,
                d.Status.ToString().ToLowerInvariant()))
            .ToList();

        return new AddDeliveriesResponse(route.Id, responseDeliveries);
    }
}
