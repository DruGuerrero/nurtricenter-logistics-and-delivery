#pragma warning disable CS8618 // Private parameterless ctor for serialization

namespace Nurtricenter.Core.Domain.Courier;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Courier.Enums;

public sealed class Courier : Entity
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

    private Courier() : base() { }

    public void SetStatus(CourierStatus status)
    {
        Status = status;
    }
}
