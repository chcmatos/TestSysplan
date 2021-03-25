using System.Collections.Generic;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Messenger.Services
{
    public interface IPublishMessageService
    {
        void Publish<TModel>(TModel model, string rountingKey = null) where TModel : ModelBase;

        void Publish<TModel>(IEnumerable<TModel> model, string rountingKey = null) where TModel : ModelBase;
    }
}
