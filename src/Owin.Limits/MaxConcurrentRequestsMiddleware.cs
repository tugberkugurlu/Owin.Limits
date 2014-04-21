namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    public class MaxConcurrentRequestsMiddleware : MiddlewareBase
    {
        private readonly MaxConcurrentRequestOptions _options;
        private int _concurrentRequests;

        public MaxConcurrentRequestsMiddleware(Func<IDictionary<string, object>, Task> next, MaxConcurrentRequestOptions options)
            : base(next.ToAppFunc(), options.Tracer)
        {
            _options = options;
        }

        protected override async Task InvokeInternal(AppFunc next, IDictionary<string, object> environment)
        {
            int maxConcurrentRequests = GetMaxConcurrentRequestLimit();
            try
            {
                int concurrentRequests = Interlocked.Increment(ref _concurrentRequests);
                _options.Tracer.AsVerbose("Concurrent counter incremented.");
                _options.Tracer.AsVerbose("Checking concurrent request #{0}.", concurrentRequests);
                if (concurrentRequests > maxConcurrentRequests)
                {
                    _options.Tracer.AsInfo("Limit of {0} exceeded with #{1}. Request rejected.", maxConcurrentRequests, concurrentRequests);
                    IOwinResponse response = new OwinContext(environment).Response;
                    response.StatusCode = 503;
                    response.ReasonPhrase = _options.LimitReachedReasonPhrase(response.StatusCode);
                    return;
                }
                _options.Tracer.AsVerbose("Request forwarded.");
                await next(environment);
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentRequests);
                _options.Tracer.AsVerbose("Concurrent counter decremented.");
            }
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