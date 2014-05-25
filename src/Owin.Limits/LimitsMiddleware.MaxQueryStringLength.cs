namespace Owin.Limits
{
    using System;
    using Microsoft.Owin;

    public static partial class LimitsMiddleware
    {
        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="options">The max concurrent request options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxQueryStringLength(MaxQueryStringLengthOptions options)
        {
            options.MustNotNull("options");

            return
                next =>
                async env =>
                {
                    var context = new OwinContext(env);
                    QueryString queryString = context.Request.QueryString;
                    if (queryString.HasValue)
                    {
                        int maxQueryStringLength = options.GetMaxQueryStringLength();
                        string unescapedQueryString = Uri.UnescapeDataString(queryString.Value);
                        options.Tracer.AsVerbose("Querystring of request with an unescaped length of {0}", unescapedQueryString.Length);
                        if (unescapedQueryString.Length > maxQueryStringLength)
                        {
                            options.Tracer.AsInfo("Querystring (Length {0}) too long (allowed {1}). Request rejected.",
                                unescapedQueryString.Length,
                                maxQueryStringLength);
                            context.Response.StatusCode = 414;
                            context.Response.ReasonPhrase = options.LimitReachedReasonPhrase(context.Response.StatusCode);
                            return;
                        }
                        options.Tracer.AsVerbose("Querystring length check passed.");
                    }
                    else
                    {
                        options.Tracer.AsVerbose("No querystring.");
                    }
                    await next(env);
                };
        }
    }
}