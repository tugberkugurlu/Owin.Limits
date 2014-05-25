namespace Owin.Limits
{
    using System;

    /// <summary>
    /// Extension methods for OWIN Limits middlware.
    /// </summary>
    public static class LimitsMiddlewareExtensions
    {
        /// <summary>
        /// Timeouts the connection if there hasn't been an read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> ConnectionTimeout(this Action<MidFunc> builder, TimeSpan timeout)
        {
            return ConnectionTimeout(builder, () => timeout);
        }

        /// <summary>
        /// Timeouts the connection if there hasn't been an read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getTimeout">A delegate to retrieve the timeout timespan. Allows you
        /// to supply different values at runtime.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> ConnectionTimeout(this Action<MidFunc> builder, Func<TimeSpan> getTimeout)
        {
            return ConnectionTimeout(builder, new ConnectionTimeoutOptions(getTimeout));
        }

        /// <summary>
        /// Timeouts the connection if there hasn't been an read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="options">The connection timeout options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static Action<MidFunc> ConnectionTimeout(this Action<MidFunc> builder, ConnectionTimeoutOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.ConnectionTimeout(options));
            return builder;
        }

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
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static Action<MidFunc> MaxBandwidth(this Action<MidFunc> builder, MaxBandwidthOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.MaxBandwidth(options));
            return builder;
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
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
        /// <param name="builder">The OWIN builder instance.</param>
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
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="options">The max concurrent request options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static Action<MidFunc> MaxConcurrentRequests(this Action<MidFunc> builder, MaxConcurrentRequestOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.MaxConcurrentRequests(options));
            return builder;
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxQueryStringLength(this Action<MidFunc> builder, int maxQueryStringLength)
        {
            return MaxQueryStringLength(builder, () => maxQueryStringLength);
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxQueryStringLength(this Action<MidFunc> builder, Func<int> getMaxQueryStringLength)
        {
            return MaxQueryStringLength(builder, new MaxQueryStringLengthOptions(getMaxQueryStringLength));
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="options">The max querystring length options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static Action<MidFunc> MaxQueryStringLength(this Action<MidFunc> builder, MaxQueryStringLengthOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.MaxQueryStringLength(options));
            return builder;
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxContentLength">Maximum length of the content.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxRequestContentLength(this Action<MidFunc> builder, int maxContentLength)
        {
            return MaxRequestContentLength(builder, () => maxContentLength);
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxRequestContentLength(this Action<MidFunc> builder, Func<int> getMaxContentLength)
        {
            return MaxRequestContentLength(builder, new MaxRequestContentLengthOptions(getMaxContentLength));
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The max request content length options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static Action<MidFunc> MaxRequestContentLength(this Action<MidFunc> builder, MaxRequestContentLengthOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.MaxRequestContentLength(options));
            return builder;
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="maxUrlLength">Maximum length of the URL.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxUrlLength(this Action<MidFunc> builder, int maxUrlLength)
        {
            return MaxUrlLength(builder, () => maxUrlLength);
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxUrlLength(this Action<MidFunc> builder, Func<int> getMaxUrlLength)
        {
            return MaxUrlLength(builder, new MaxUrlLengthOptions(getMaxUrlLength));
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="options">The max url length options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static Action<MidFunc> MaxUrlLength(this Action<MidFunc> builder, MaxUrlLengthOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.MaxUrlLength(options));
            return builder;
        }
    }
}