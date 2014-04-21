namespace Owin.Limits
{
    using System;
    using Microsoft.Owin;

    /// <summary>
    /// OWIN middleware that limits the reqeust content length. Supports chunked requests.
    /// </summary>
    public static class MaxRequestContentLengthMiddleware
    {
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
        /// <exception cref="System.ArgumentNullException">builder</exception>
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
        /// <exception cref="System.ArgumentNullException">builder or options </exception>
        public static Action<MidFunc> MaxRequestContentLength(this Action<MidFunc> builder, MaxRequestContentLengthOptions options)
        {
            builder.MustNotNull("builder");
            options.MustNotNull("options");

            builder(next => async env =>
            {
                var context = new OwinContext(env);
                IOwinRequest request = context.Request;
                string requestMethod = request.Method.Trim().ToUpper();

                if (requestMethod == "GET" || requestMethod == "HEAD")
                {
                    options.Tracer.AsVerbose("GET or HEAD request without checking forwarded.");
                    await next(env);
                    return;
                }
                int maxContentLength = options.GetMaxContentLength();
                options.Tracer.AsVerbose("Max valid content length is {0}.", maxContentLength);
                if (!IsChunkedRequest(request))
                {
                    options.Tracer.AsVerbose("Not a chunked request. Checking content lengt header.");
                    string contentLengthHeaderValue = request.Headers.Get("Content-Length");
                    if (contentLengthHeaderValue == null)
                    {
                        options.Tracer.AsInfo("No content length header provided. Request rejected.");
                        SetResponseStatusCodeAndReasonPhrase(context, 411, options);
                        return;
                    }
                    int contentLength;
                    if (!int.TryParse(contentLengthHeaderValue, out contentLength))
                    {
                        options.Tracer.AsInfo("Invalid content length header value. Value: {0}", contentLengthHeaderValue);
                        SetResponseStatusCodeAndReasonPhrase(context, 400, options);
                        return;
                    }
                    if (contentLength > maxContentLength)
                    {
                        options.Tracer.AsInfo("Content length of {0} exceeds maximum of {1}. Request rejected.", contentLength, maxContentLength);
                        SetResponseStatusCodeAndReasonPhrase(context, 413, options);
                        return;
                    }
                    options.Tracer.AsVerbose("Content length header check passed.");
                }
                else
                {
                    options.Tracer.AsVerbose("Chunked request. Content length header not checked.");
                }

                request.Body = new ContentLengthLimitingStream(request.Body, maxContentLength);
                options.Tracer.AsVerbose("Request body stream configured with length limiting stream of {0}.", maxContentLength);

                try
                {
                    options.Tracer.AsVerbose("Request forwarded.");
                    await next(env);
                    options.Tracer.AsVerbose("Processing finished.");
                }
                catch (ContentLengthExceededException)
                {
                    options.Tracer.AsInfo("Content length of {0} exceeded. Request canceled and rejected.", maxContentLength);
                    SetResponseStatusCodeAndReasonPhrase(context, 413, options);
                }
            });
            return builder;
        }

        private static bool IsChunkedRequest(IOwinRequest request)
        {
            string header = request.Headers.Get("Transfer-Encoding");
            return header != null && header.Equals("chunked", StringComparison.OrdinalIgnoreCase);
        }

        private static void SetResponseStatusCodeAndReasonPhrase(IOwinContext context, int statusCode, MaxRequestContentLengthOptions options)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ReasonPhrase = options.LimitReachedReasonPhrase(context.Response.StatusCode);
        }
    }
}