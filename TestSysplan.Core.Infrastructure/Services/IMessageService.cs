using System.Collections.Generic;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    public interface IMessageService
    {
        void Publish<TModel>(TModel model, string rountingKey = null) where TModel : ModelBase;

        void Publish<TModel>(IEnumerable<TModel> model, string rountingKey = null) where TModel : ModelBase;
    }
}
