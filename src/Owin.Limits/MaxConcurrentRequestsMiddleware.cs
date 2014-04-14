namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class MaxConcurrentRequestsMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly MaxConcurrentRequestOptions _options;
        private int _concurrentRequests;

        public MaxConcurrentRequestsMiddleware(Func<IDictionary<string, object>, Task> next, MaxConcurrentRequestOptions options)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            _next = next;
            _options = options;
        }

        [UsedImplicitly]
        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            _options.Tracer.AsVerbose("Start processing.");

            int maxConcurrentRequests = GetMaxConcurrentRequestLimit();
            try
            {
                int concurrentRequests = Interlocked.Increment(ref _concurrentRequests);
                _options.Tracer.AsVerbose("Concurrent counter incremented.");
                _options.Tracer.AsVerbose("Checking concurrent request #{0}.".FormattedWith(concurrentRequests));
                if (concurrentRequests > maxConcurrentRequests)
                {
                    _options.Tracer.AsInfo("Limit of {0} exceeded with #{1}. Request rejected.".FormattedWith(maxConcurrentRequests, concurrentRequests));
                    IOwinResponse response = new OwinContext(environment).Response;
                    response.StatusCode = 503;
                    response.ReasonPhrase = _options.LimitReachedReasonPhrase(response.StatusCode);
                    return;
                }
                _options.Tracer.AsVerbose("Request forwarded.");
                await _next(environment);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentRequests);
                _options.Tracer.AsVerbose("Concurrent counter decremented.");
            }
            _options.Tracer.AsVerbose("Processing finished.");
        }

        private int GetMaxConcurrentRequestLimit()
        {
            int limit = _options.GetMaxConcurrentRequests();
            if (limit <= 0)
            {
                limit = int.MaxValue;
            }
            return limit;
        }
    }
}