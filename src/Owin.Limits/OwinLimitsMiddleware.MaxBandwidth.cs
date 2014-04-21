namespace Owin.Limits
{
    using System;
    using System.IO;
    using Microsoft.Owin;

    public static partial class OwinLimitsMiddleware
    {
        /// <summary>
        /// Limits the bandwith used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="options">The max bandwidth options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxBandwidth(MaxBandwidthOptions options)
        {
            options.MustNotNull("options");

            return 
                next =>
                env =>
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
                };
        }
    }
}