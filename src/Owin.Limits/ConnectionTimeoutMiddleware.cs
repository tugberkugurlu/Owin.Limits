namespace Owin.Limits
{
    using System;
    using System.IO;
    using Microsoft.Owin;

    /// <summary>
    /// Middleware to limit the duration of a connection if there hasn't been any read or write activity.
    /// </summary>
    public static class ConnectionTimeoutMiddleware
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
        /// <exception cref="System.ArgumentNullException">builder or options </exception>
        public static Action<MidFunc> ConnectionTimeout(this Action<MidFunc> builder, ConnectionTimeoutOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");

            builder(next => env =>
            {
                var context = new OwinContext(env);
                Stream requestBodyStream = context.Request.Body ?? Stream.Null;
                Stream responseBodyStream = context.Response.Body;

                options.Tracer.AsVerbose("Configure timeouts.");
                TimeSpan connectionTimeout = options.GetTimeout();
                context.Request.Body = new TimeoutStream(requestBodyStream, connectionTimeout, options.Tracer);
                context.Response.Body = new TimeoutStream(responseBodyStream, connectionTimeout, options.Tracer);

                options.Tracer.AsVerbose("Request with configured timeout forwarded.");
                return next(env);
            });
            return builder;
        }
    }
}