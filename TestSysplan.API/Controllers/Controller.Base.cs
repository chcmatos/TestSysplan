using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace TestSysplan.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class ControllerBase<Model, Service> : ControllerBase
    {
        private class EmptyLogger : ILogger<ControllerBase<Model, Service>>, IDisposable
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }

            void IDisposable.Dispose() { }
        }

        protected readonly Service service;
        protected readonly ILogger logger;
        
        protected ControllerBase(Service service, ILogger<ControllerBase<Model, Service>> logger)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.logger = logger;
        }

        protected ControllerBase(Service service) : this(service, new EmptyLogger()) { }
    }
}
