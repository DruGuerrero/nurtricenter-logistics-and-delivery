using Nurtricenter.Core.Interfaces.Services.ClinicService.Dto;

namespace Nurtricenter.Core.Interfaces.Services.ClinicService;

public interface IClinicService
{
    Task<IReadOnlyList<PatientContactInfo>> GetPatientsContactInfoAsync(
        IReadOnlyList<string> patientIds,
        CancellationToken cancellationToken);
}
