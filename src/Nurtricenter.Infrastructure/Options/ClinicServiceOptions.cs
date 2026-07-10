namespace Nurtricenter.Infrastructure.Options;

public sealed class ClinicServiceOptions
{
    public const string SectionName = "ClinicService";

    public required string BaseUrl { get; init; }
}
