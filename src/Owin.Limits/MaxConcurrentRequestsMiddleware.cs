namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    internal class MaxConcurrentRequestsMiddleware
    {
        private readonly Func<int> _getMaxConcurrentRequests;
        private readonly Func<IDictionary<string, object>, Task> _next;
        private int _concurrentRequests;

        public MaxConcurrentRequestsMiddleware(Func<IDictionary<string, object>, Task> next, Func<int> getMaxConcurrentRequests)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            _next = next;
            _getMaxConcurrentRequests = getMaxConcurrentRequests;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            int maxConcurrentRequests = _getMaxConcurrentRequests();
            if (maxConcurrentRequests <= 0)
            {
                maxConcurrentRequests = int.MaxValue;
            }
            try
            {
                int concurrentRequests = Interlocked.Increment(ref _concurrentRequests);
                if (concurrentRequests > maxConcurrentRequests)
                {
                    var conext = new OwinContext(environment);
                    conext.Response.StatusCode = 503;
                    conext.Response.ReasonPhrase = "Service Unavailable";
                    return;
                }
                await _next(environment);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentRequests);
            }
        }
    }
}