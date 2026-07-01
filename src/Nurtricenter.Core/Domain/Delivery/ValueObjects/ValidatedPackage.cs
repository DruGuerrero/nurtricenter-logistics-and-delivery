#pragma warning disable CS8618 // Parameterless constructor for serialization

namespace Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed record ValidatedPackage
{
    public string PackageId { get; }
    public string PatientId { get; }
    public string LabelData { get; }

    public ValidatedPackage(string packageId, string patientId, string labelData)
    {
        if (string.IsNullOrWhiteSpace(packageId))
            throw new ArgumentException("Package ID cannot be empty.", nameof(packageId));
        if (string.IsNullOrWhiteSpace(patientId))
            throw new ArgumentException("Patient ID cannot be empty.", nameof(patientId));
        if (string.IsNullOrWhiteSpace(labelData))
            throw new ArgumentException("Label data cannot be empty.", nameof(labelData));

        PackageId = packageId;
        PatientId = patientId;
        LabelData = labelData;
    }

    private ValidatedPackage() { }
}
