namespace Owin.Limits
{
    using System;
    using System.Threading;
    using Microsoft.Owin;

    public static partial class LimitsMiddleware
    {
        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="options">The max concurrent request options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxConcurrentRequests(MaxConcurrentRequestOptions options)
        {
            options.MustNotNull("options");

            int concurrentRequestCounter = 0;
            return 
                next =>
                async env =>
                {
                    int maxConcurrentRequests = options.GetMaxConcurrentRequests();
                    if (maxConcurrentRequests <= 0)
                    {
                        maxConcurrentRequests = int.MaxValue;
                    }
                    try
                    {
                        int concurrentRequests = Interlocked.Increment(ref concurrentRequestCounter);
                        options.Tracer.AsVerbose("Concurrent counter incremented.");
                        options.Tracer.AsVerbose("Checking concurrent request #{0}.", concurrentRequests);
                        if (concurrentRequests > maxConcurrentRequests)
                        {
                            options.Tracer.AsInfo("Limit of {0} exceeded with #{1}. Request rejected.", maxConcurrentRequests, concurrentRequests);
                            IOwinResponse response = new OwinContext(env).Response;
                            response.StatusCode = 503;
                            response.ReasonPhrase = options.LimitReachedReasonPhrase(response.StatusCode);
                            return;
                        }
                        options.Tracer.AsVerbose("Request forwarded.");
                        await next(env);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref concurrentRequestCounter);
                        options.Tracer.AsVerbose("Concurrent counter decremented.");
                    }
                };
        }
    }
}