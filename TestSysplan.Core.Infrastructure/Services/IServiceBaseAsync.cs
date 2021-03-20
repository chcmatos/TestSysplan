using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    public interface IServiceBaseAsync<Entity> where Entity : ModelBase
    {
        #region [C]reate
        Task<Entity> InsertAsync(Entity entity, CancellationToken cancellationToken = default);
        #endregion

        #region [R]ead
        Task<bool> ExistsAsync(Guid uuid, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Entity e, CancellationToken cancellationToken = default);

        Task<Entity> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<Entity> GetAsync(Guid uuid, CancellationToken cancellationToken = default);

        Task<List<Entity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken = default);

        Task<List<Entity>> PagingAsync(int page = 0, int limit = -1, CancellationToken cancellationToken = default);

        Task<List<Entity>> ListAsync(CancellationToken cancellationToken = default);
        #endregion

        #region [U]pdate
        Task<Entity> UpdateAsync(Entity entity, CancellationToken cancellationToken = default);
        #endregion

        #region [D]elete
        Task<int> DeleteAsync(IEnumerable<Guid> uuid, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(Guid[] uuid, CancellationToken cancellationToken);

        Task<int> DeleteAsync(params Guid[] uuid);

        Task<bool> DeleteAsync(Guid uuid, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(IEnumerable<Entity> entity, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Entity[] entity, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(params Entity[] entity);
        #endregion
    }
}
