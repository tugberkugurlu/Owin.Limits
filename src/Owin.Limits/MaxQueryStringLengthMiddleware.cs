namespace Owin.Limits
{
    using System;
    using Microsoft.Owin;

    /// <summary>
    /// OWIN middleware to limit the size of a query string.
    /// </summary>
    public static class MaxQueryStringLengthMiddleware
    {
        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">An OWIN builder instance.</param>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxQueryStringLength(this Action<MidFunc> builder, int maxQueryStringLength)
        {
            return MaxQueryStringLength(builder, () => maxQueryStringLength);
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">An OWIN builder instance.</param>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static Action<MidFunc> MaxQueryStringLength(this Action<MidFunc> builder, Func<int> getMaxQueryStringLength)
        {
            return MaxQueryStringLength(builder, new MaxQueryStringLengthOptions(getMaxQueryStringLength));
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">An OWIN builder instance.</param>
        /// <param name="options">The max querystring length options.</param>
        /// <returns>The OWIN builder instance.</returns>
        /// <exception cref="System.ArgumentNullException">builder or options</exception>
        public static Action<MidFunc> MaxQueryStringLength(this Action<MidFunc> builder, MaxQueryStringLengthOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");

            builder(next => async env =>
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
            });
            return builder;
        }
    }
}