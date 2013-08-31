namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class ConnectionTimeoutMiddleware
    {
        private readonly Func<TimeSpan> _getConnectionTimeout;
        private readonly Func<IDictionary<string, object>, Task> _next;

        public ConnectionTimeoutMiddleware(Func<IDictionary<string, object>, Task> next, Func<TimeSpan> getConnectionTimeout)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            _next = next;
            _getConnectionTimeout = getConnectionTimeout;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            await _next(environment);
        }
    }
}