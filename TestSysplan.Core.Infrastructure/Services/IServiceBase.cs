using System;
using System.Collections.Generic;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    public interface IServiceBase<Entity> where Entity : ModelBase
    {
        bool Exists(Guid uuid);

        bool Exists(Entity e);

        Entity Get(long id);

        Entity Get(Guid uuid);

        List<Entity> PagingIndex(int index, int count);

        List<Entity> Paging(int page = 0, int limit = -1);

        List<Entity> List();

        Entity Insert(Entity entity);

        Entity Update(Entity entity);

        int Delete(IEnumerable<Guid> uuid);

        int Delete(params Guid[] uuid);

        bool Delete(Guid uuid);

        bool Delete(IEnumerable<Entity> entity);

        bool Delete(params Entity[] entity);
    }
}
