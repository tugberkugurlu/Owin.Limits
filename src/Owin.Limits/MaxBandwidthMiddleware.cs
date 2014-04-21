namespace Owin.Limits
{
    using System;
    using System.IO;
    using Microsoft.Owin;

    /// <summary>
    /// OWIN middleware to limit the bandwidth of a request.
    /// </summary>
    public static class MaxBandwidthMiddleware
    {
        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="maxBytesPerSecond">The maximum number of bytes per second to be transferred. Use 0 or a negative
        /// number to specify infinite bandwidth.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxBandwidth(this Action<MidFunc> builder, int maxBytesPerSecond)
        {
            return MaxBandwidth(builder, () => maxBytesPerSecond);
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getMaxBytesPerSecond">A delegate to retrieve the maximum number of bytes per second to be transferred.
        /// Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.</param>
        /// <returns>The builder instance.</returns>
        public static Action<MidFunc> MaxBandwidth(this Action<MidFunc> builder, Func<int> getMaxBytesPerSecond)
        {
            return MaxBandwidth(builder, new MaxBandwidthOptions(getMaxBytesPerSecond));
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="options">The max bandwith options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder or options</exception>
        public static Action<MidFunc> MaxBandwidth(this Action<MidFunc> builder, MaxBandwidthOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");

            builder(next => env =>
            {
                var context = new OwinContext(env);
                Stream requestBodyStream = context.Request.Body ?? Stream.Null;
                Stream responseBodyStream = context.Response.Body;
                int maxBytesPerSecond = options.GetMaxBytesPerSecond();
                if (maxBytesPerSecond < 0)
                {
                    maxBytesPerSecond = 0;
                }
                options.Tracer.AsVerbose("Configure streams to be limited.");
                context.Request.Body = new ThrottledStream(requestBodyStream, maxBytesPerSecond);
                context.Response.Body = new ThrottledStream(responseBodyStream, maxBytesPerSecond);

                //TODO consider SendFile interception
                options.Tracer.AsVerbose("With configured limit forwarded.");
                return next(env);
            });
            return builder;
        }
    }
}