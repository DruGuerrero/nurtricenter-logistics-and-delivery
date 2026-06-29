#pragma warning disable CS8618 // Parameterless constructor for serialization

namespace Nurtricenter.Core.Domain.Courier;

using Nurtricenter.Core.Domain.Base;
using Nurtricenter.Core.Domain.Courier.Enums;

public sealed class Courier : Entity<Guid>
{
    public string FullName { get; private set; }
    public CourierStatus Status { get; private set; }

    public Courier(Guid id, string fullName, CourierStatus status = CourierStatus.Available)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));

        FullName = fullName;
        Status = status;
    }

    private Courier() : base(Guid.Empty) { }

    public void SetStatus(CourierStatus status)
    {
        Status = status;
    }
}
