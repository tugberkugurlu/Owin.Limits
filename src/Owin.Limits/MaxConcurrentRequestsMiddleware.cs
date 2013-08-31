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
            int concurrentRequests = Interlocked.Increment(ref _concurrentRequests);

            Console.WriteLine("{0}, max {1}", concurrentRequests, maxConcurrentRequests);

            if (concurrentRequests > maxConcurrentRequests)
            {
                var conext = new OwinContext(environment);
                conext.Response.StatusCode = 503;
                conext.Response.ReasonPhrase = "Service Unavailable";
                Interlocked.Decrement(ref _concurrentRequests);
                return;
            }
            await _next(environment);
            Interlocked.Decrement(ref _concurrentRequests);
        }
    }
}