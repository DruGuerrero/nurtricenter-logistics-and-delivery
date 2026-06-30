namespace Nurtricenter.Core.Domain.Delivery.Repositories;

using Joseco.DDD.Core.Abstractions;
using Nurtricenter.Core.Domain.Delivery;

public interface IDeliveryRepository : IRepository<Delivery>
{
    Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetByRouteIdAsync(Guid routeId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Delivery delivery, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
