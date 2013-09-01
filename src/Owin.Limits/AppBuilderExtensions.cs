// ReSharper disable CheckNamespace
namespace Owin
// ReSharper restore CheckNamespace
{
    using System;
    using Owin.Limits;

    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxBytesPerSecond">The maximum number of bytes per second to be transferred. Use 0 or a negative
        /// number to specify infinite bandwidth.</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static IAppBuilder MaxBandwidth(this IAppBuilder builder, Func<int> getMaxBytesPerSecond)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            return builder.Use<MaxBandwidthMiddleware>(getMaxBytesPerSecond);
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxConcurrentRequests">The maximum number of concurrent requests. Use 0 or a negative
        /// number to specify unlimited number of concurrent requests.</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static IAppBuilder MaxConcurrentRequests(this IAppBuilder builder, Func<int> getMaxConcurrentRequests)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            return builder.Use<MaxConcurrentRequestsMiddleware>(getMaxConcurrentRequests);
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="maxConcurrentRequests">The maximum number of concurrent requests. Use 0 or a negative
        /// number to specify unlimited number of concurrent requests.</param>
        /// <returns></returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder builder, TimeSpan maxConcurrentRequests)
        {
            return ConnectionTimeout(builder, () => maxConcurrentRequests);
        }

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxConcurrentRequests">A delegate to retrieve the maximum number of concurrent requests. Allows you
        /// to supply different values at runtime. Use 0 or a negative number to specify unlimited number of concurrent requests.</param>
        /// <returns></returns>
        public static IAppBuilder ConnectionTimeout(this IAppBuilder builder, Func<TimeSpan> getMaxConcurrentRequests)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            return builder.Use<ConnectionTimeoutMiddleware>(getMaxConcurrentRequests);
        }
    }
}