namespace Nurtricenter.Infrastructure.Services;

using System.Net.Http.Json;
using Joseco.DDD.Core.Results;
using Nurtricenter.Core.Interfaces.Services.ClinicService;
using Nurtricenter.Core.Interfaces.Services.ClinicService.Dto;

public sealed class ClinicService : IClinicService
{
    private readonly HttpClient _httpClient;

    public ClinicService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<PatientContactInfo>> GetPatientsContactInfoAsync(
        IReadOnlyList<string> patientIds,
        CancellationToken cancellationToken)
    {
        if (patientIds is null || patientIds.Count == 0)
            return Array.Empty<PatientContactInfo>();

        var response = await _httpClient.PostAsJsonAsync(
            "/api/v1/patients/contact-information",
            patientIds,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new DomainException(
                Error.Problem(
                    "ClinicService.RequestFailed",
                    "Clinic service returned {statusCode}: {body}",
                    ((int)response.StatusCode).ToString(),
                    body));
        }

        var patients = await response.Content
            .ReadFromJsonAsync<List<PatientContactInfo>>(cancellationToken);

        if (patients is null)
        {
            throw new DomainException(
                Error.Problem(
                    "ClinicService.EmptyResponse",
                    "Clinic service returned an empty or invalid response."));
        }

        return patients;
    }
}
