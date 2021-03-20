using System;
using System.Collections.Generic;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    public interface IServiceBase<Entity> where Entity : ModelBase
    {
        #region [C]reate
        Entity Insert(Entity entity);
        #endregion

        #region [R]ead
        bool Exists(Guid uuid);

        bool Exists(Entity e);

        Entity Get(long id);

        Entity Get(Guid uuid);

        List<Entity> PagingIndex(int index, int count);

        List<Entity> Paging(int page = 0, int limit = -1);

        List<Entity> List();
        #endregion

        #region [U]pdate
        Entity Update(Entity entity);
        #endregion

        #region [D]elete
        int Delete(IEnumerable<Guid> uuid);

        int Delete(params Guid[] uuid);

        bool Delete(Guid uuid);

        bool Delete(IEnumerable<Entity> entity);

        bool Delete(params Entity[] entity);
        #endregion
    }
}
