namespace Nurtricenter.Application.Queries.Couriers.GetTodayRoute;

using Joseco.DDD.Core.Results;
using MediatR;
using Nurtricenter.Core.Domain.Courier.Repositories;
using Nurtricenter.Core.Domain.Route.Repositories;
using Nurtricenter.Core.Interfaces.Services.ClinicService;

public sealed class GetCourierTodayRouteHandler : IRequestHandler<GetCourierTodayRouteQuery, CourierTodayRouteResponse>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IClinicService _clinicService;

    public GetCourierTodayRouteHandler(
        ICourierRepository courierRepository,
        IRouteRepository routeRepository,
        IClinicService clinicService)
    {
        _courierRepository = courierRepository;
        _routeRepository = routeRepository;
        _clinicService = clinicService;
    }

    public async Task<CourierTodayRouteResponse> Handle(
        GetCourierTodayRouteQuery request,
        CancellationToken cancellationToken)
    {
        var courier = await _courierRepository.GetByIdAsync(request.CourierId, cancellationToken);

        if (courier is null)
        {
            throw new DomainException(
                Error.NotFound(
                    "Courier.NotFound",
                    "Courier '{courierId}' was not found.",
                    request.CourierId.ToString()));
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var route = await _routeRepository.GetByCourierAndDateAsync(request.CourierId, today, cancellationToken);

        if (route is null)
        {
            throw new DomainException(
                Error.NotFound(
                    "Route.NoRouteForToday",
                    "No route found for courier '{courierId}' on {date}.",
                    request.CourierId.ToString(),
                    today.ToString("yyyy-MM-dd")));
        }

        var deliveries = route.Deliveries;
        var patientIds = deliveries.Select(d => d.Package.PatientId).Distinct().ToList();

        var patients = await _clinicService.GetPatientsContactInfoAsync(patientIds, cancellationToken);
        var patientNameMap = patients.ToDictionary(p => p.PatientId, p => p.FullName);

        var deliveryDtos = deliveries
            .Select((d, index) => new TodayDeliveryDto(
                d.Id,
                index + 1, //hardcoded for now
                patientNameMap.TryGetValue(d.Package.PatientId, out var name) ? name : d.Package.PatientId,
                d.Address.Description))
            .ToList();

        return new CourierTodayRouteResponse(request.CourierId, route.Id, deliveryDtos);
    }
}
