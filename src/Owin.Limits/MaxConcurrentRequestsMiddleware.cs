namespace Owin.Limits
{
    using System;
    using System.Threading;
    using Microsoft.Owin;

    /// <summary>
    /// OWIN middleware that limits the number of concurrent requests.
    /// </summary>
    public static class MaxConcurrentRequestsMiddleware
    {
        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">An OWIN builder instance.</param>
        /// <param name="maxConcurrentRequests">The maximum number of concurrent requests. Use 0 or a negative
        /// number to specify unlimited number of concurrent requests.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxConcurrentRequests(this Action<MidFunc> builder, int maxConcurrentRequests)
        {
            return MaxConcurrentRequests(builder, () => maxConcurrentRequests);
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">An OWIN builder instance.</param>
        /// <param name="getMaxConcurrentRequests">A delegate to retrieve the maximum number of concurrent requests. Allows you
        /// to supply different values at runtime. Use 0 or a negative number to specify unlimited number of concurrent requests.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxConcurrentRequests(this Action<MidFunc> builder, Func<int> getMaxConcurrentRequests)
        {
            return MaxConcurrentRequests(builder, new MaxConcurrentRequestOptions(getMaxConcurrentRequests));
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">An OWIN builder instance.</param>
        /// <param name="options">The max concurrent request options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder or options</exception>
        public static Action<MidFunc> MaxConcurrentRequests(this Action<MidFunc> builder, MaxConcurrentRequestOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");
            
            int concurrentRequestCounter = 0;

            builder(next => async env =>
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
            });
            return builder;
        }
    }
}