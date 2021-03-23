using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace TestSysplan.API.Test
{
    internal sealed class XUnitLogger<T> : ILogger<T>, IDisposable
    {
        private ITestOutputHelper output;

        ~XUnitLogger()
        {
            output = null;
        }

        public XUnitLogger(ITestOutputHelper output)
        {
            this.output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            output?.WriteLine(state.ToString());
        }

        void IDisposable.Dispose() 
        {
            output = null;
        }

    }
}
