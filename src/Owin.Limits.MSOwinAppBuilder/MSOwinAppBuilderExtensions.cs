// ReSharper disable once CheckNamespace
namespace Owin //TODO what should this namespace be?
{
    using System;
    using Owin.Limits;

    public static class MSOwinAppBuilderExtensions
    {
        public static Action<MidFunc> Use(this IAppBuilder builder)
        {
            return middleware => builder.Use(middleware);
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxBytesPerSecond">The maximum number of bytes per second to be transferred. Use 0 or a negative
        /// number to specify infinite bandwidth.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxBandwidth(this IAppBuilder builder, int maxBytesPerSecond)
        {
            return MaxBandwidth(builder, () => maxBytesPerSecond);
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxBytesPerSecond">A delegate to retrieve the maximum number of bytes per second to be transferred.
        /// Allows you to supply different values at runtime. Use 0 or a negative number to specify infinite bandwidth.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxBandwidth(this IAppBuilder builder, Func<int> getMaxBytesPerSecond)
        {
            return MaxBandwidth(builder, new MaxBandwidthOptions(getMaxBytesPerSecond));
        }

        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The max bandwith options.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxBandwidth(this IAppBuilder builder, MaxBandwidthOptions options)
        {
            builder.Use().MaxBandwidth(options);
            return builder;
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxConcurrentRequests">The maximum number of concurrent requests. Use 0 or a negative
        /// number to specify unlimited number of concurrent requests.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder builder, int maxConcurrentRequests)
        {
            return MaxConcurrentRequests(builder, () => maxConcurrentRequests);
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxConcurrentRequests">A delegate to retrieve the maximum number of concurrent requests. Allows you
        /// to supply different values at runtime. Use 0 or a negative number to specify unlimited number of concurrent requests.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder builder, Func<int> getMaxConcurrentRequests)
        {
            return MaxConcurrentRequests(builder, new MaxConcurrentRequestOptions(getMaxConcurrentRequests));
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The max concurrent request options.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder builder, MaxConcurrentRequestOptions options)
        {
            builder.Use().MaxConcurrentRequests(options);
            return builder;
        }

        /// <summary>
        /// Timeouts the connection if there hasn't been an read read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder builder, TimeSpan timeout)
        {
            return ConnectionTimeout(builder, () => timeout);
        }

        /// <summary>
        /// Timeouts the connection if there hasn't been an read read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getTimeout">A delegate to retrieve the timeout timespan. Allows you
        /// to supply different values at runtime.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder builder, Func<TimeSpan> getTimeout)
        {
            return ConnectionTimeout(builder, new ConnectionTimeoutOptions(getTimeout));
        }

        /// <summary>
        /// Timeouts the connection if there hasn't been an read read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The connection timeout options.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder builder, ConnectionTimeoutOptions options)
        {
            builder.Use().ConnectionTimeout(options);
            return builder;
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxQueryStringLength(this IAppBuilder builder, int maxQueryStringLength)
        {
            return MaxQueryStringLength(builder, () => maxQueryStringLength);
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static IAppBuilder MaxQueryStringLength(this IAppBuilder builder, Func<int> getMaxQueryStringLength)
        {
            return MaxQueryStringLength(builder, new MaxQueryStringLengthOptions(getMaxQueryStringLength));
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The max querystring length options.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxQueryStringLength(this IAppBuilder builder, MaxQueryStringLengthOptions options)
        {
            builder.Use().MaxQueryStringLength(options);
            return builder;
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxContentLength">Maximum length of the content.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxRequestContentLength(this IAppBuilder builder, int maxContentLength)
        {
            return MaxRequestContentLength(builder, () => maxContentLength);
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static IAppBuilder MaxRequestContentLength(this IAppBuilder builder, Func<int> getMaxContentLength)
        {
            return MaxRequestContentLength(builder, new MaxRequestContentLengthOptions(getMaxContentLength));
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The max request content length options.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxRequestContentLength(this IAppBuilder builder, MaxRequestContentLengthOptions options)
        {
            builder.Use().MaxRequestContentLength(options);
            return builder;
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxUrlLength">Maximum length of the URL.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        public static IAppBuilder MaxUrlLength(this IAppBuilder builder, int maxUrlLength)
        {
            return MaxUrlLength(builder, () => maxUrlLength);
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder</exception>
        public static IAppBuilder MaxUrlLength(this IAppBuilder builder, Func<int> getMaxUrlLength)
        {
            return MaxUrlLength(builder, new MaxUrlLengthOptions(getMaxUrlLength));
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="options">The max url length options.</param>
        /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder or options </exception>
        public static IAppBuilder MaxUrlLength(this IAppBuilder builder, MaxUrlLengthOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");

            return builder.Use(typeof(MaxUrlLengthMiddleware), options);
        }
    }
}