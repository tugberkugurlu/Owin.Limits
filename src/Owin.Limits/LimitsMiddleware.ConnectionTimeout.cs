namespace Owin.Limits
{
    using System;
    using System.IO;
    using Microsoft.Owin;

    /// <summary>
    /// OWIN Limits middleware functions.
    /// </summary>
    public static partial class LimitsMiddleware
    {
        /// <summary>
        /// Timeouts the connection if there hasn't been an read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="options">The connection timeout options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc ConnectionTimeout(ConnectionTimeoutOptions options)
        {
            options.MustNotNull("options");

            return 
                next =>
                env =>
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
                };
        }
    }
}