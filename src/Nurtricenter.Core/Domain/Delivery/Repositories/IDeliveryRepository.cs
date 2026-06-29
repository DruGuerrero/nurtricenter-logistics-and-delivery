namespace Nurtricenter.Core.Domain.Delivery.Repositories;

using Nurtricenter.Core.Domain.Delivery;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task AddAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
