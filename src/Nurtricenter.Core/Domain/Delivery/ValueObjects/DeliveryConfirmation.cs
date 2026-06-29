#pragma warning disable CS8618 // Parameterless constructor for serialization

namespace Nurtricenter.Core.Domain.Delivery.ValueObjects;

public sealed record DeliveryConfirmation
{
    public DateTime DeliveredAt { get; }
    public string EvidencePhotoUrl { get; }
    public string DigitalSignature { get; }

    public DeliveryConfirmation(DateTime deliveredAt, string evidencePhotoUrl, string digitalSignature)
    {
        if (string.IsNullOrWhiteSpace(evidencePhotoUrl))
            throw new ArgumentException("Evidence photo URL cannot be empty.", nameof(evidencePhotoUrl));
        if (string.IsNullOrWhiteSpace(digitalSignature))
            throw new ArgumentException("Digital signature cannot be empty.", nameof(digitalSignature));

        DeliveredAt = deliveredAt;
        EvidencePhotoUrl = evidencePhotoUrl;
        DigitalSignature = digitalSignature;
    }

    private DeliveryConfirmation() { }
}
