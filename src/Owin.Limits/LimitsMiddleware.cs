namespace Owin.Limits
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using MidFunc = System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>;
    using BuildFunc = System.Action<System.Func<System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>, System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>>;

    public static partial class LimitsMiddleware
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

        /// <summary>
        /// Limits the number of concurrent requests that can be handled used by the subsequent stages in the owin pipeline.
        /// </summary>
        /// <param name="options">The max concurrent request options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxConcurrentRequests(MaxConcurrentRequestOptions options)
        {
            options.MustNotNull("options");

            int concurrentRequestCounter = 0;
            return
                next =>
                async env =>
                {
                    int maxConcurrentRequests = options.GetMaxConcurrentRequests();
                    if (maxConcurrentRequests <= 0)
                    {
                        maxConcurrentRequests = int.MaxValue;
                    }
                    try
                    {
                        int concurrentRequests = Interlocked.Increment(ref concurrentRequestCounter);
                        options.Tracer.AsVerbose("Concurrent counter incremented.");
                        options.Tracer.AsVerbose("Checking concurrent request #{0}.", concurrentRequests);
                        if (concurrentRequests > maxConcurrentRequests)
                        {
                            options.Tracer.AsInfo("Limit of {0} exceeded with #{1}. Request rejected.", maxConcurrentRequests, concurrentRequests);
                            IOwinResponse response = new OwinContext(env).Response;
                            response.StatusCode = 503;
                            response.ReasonPhrase = options.LimitReachedReasonPhrase(response.StatusCode);
                            return;
                        }
                        options.Tracer.AsVerbose("Request forwarded.");
                        await next(env);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref concurrentRequestCounter);
                        options.Tracer.AsVerbose("Concurrent counter decremented.");
                    }
                };
        }

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

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="options">The max request content lenght options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxRequestContentLength(MaxRequestContentLengthOptions options)
        {
            options.MustNotNull("options");

            return
                next =>
                async env =>
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
                };
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

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="options">The max request content lenght options.</param>
        /// <returns>The middleware delegate.</returns>
        public static MidFunc MaxUrlLength(MaxUrlLengthOptions options)
        {
            options.MustNotNull("options");

            return
                next =>
                env =>
                {
                    var context = new OwinContext(env);
                    int maxUrlLength = options.GetMaxUrlLength();
                    string unescapedUri = Uri.UnescapeDataString(context.Request.Uri.AbsoluteUri);

                    options.Tracer.AsVerbose("Checking request url length.");
                    if (unescapedUri.Length > maxUrlLength)
                    {
                        options.Tracer.AsInfo(
                            "Url \"{0}\"(Length: {2}) exceeds allowed length of {1}. Request rejected.",
                            unescapedUri,
                            maxUrlLength,
                            unescapedUri.Length);
                        context.Response.StatusCode = 414;
                        context.Response.ReasonPhrase = options.LimitReachedReasonPhrase(context.Response.StatusCode);
                        return Task.FromResult(0);
                    }
                    options.Tracer.AsVerbose("Check passed. Request forwarded.");
                    return next(env);
                };
        }

        /// <summary>
        /// Timeouts the connection if there hasn't been an read activity on the request body stream or any
        /// write activity on the response body stream.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static BuildFunc ConnectionTimeout(this BuildFunc builder, TimeSpan timeout)
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
        public static BuildFunc ConnectionTimeout(this BuildFunc builder, Func<TimeSpan> getTimeout)
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
        public static BuildFunc ConnectionTimeout(this BuildFunc builder, ConnectionTimeoutOptions options)
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
        public static BuildFunc MaxBandwidth(this BuildFunc builder, int maxBytesPerSecond)
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
        public static BuildFunc MaxBandwidth(this BuildFunc builder, Func<int> getMaxBytesPerSecond)
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
        public static BuildFunc MaxBandwidth(this BuildFunc builder, MaxBandwidthOptions options)
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
        public static BuildFunc MaxConcurrentRequests(this BuildFunc builder, int maxConcurrentRequests)
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
        public static BuildFunc MaxConcurrentRequests(this BuildFunc builder, Func<int> getMaxConcurrentRequests)
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
        public static BuildFunc MaxConcurrentRequests(this BuildFunc builder, MaxConcurrentRequestOptions options)
        {
            builder.MustNotNull("builder");

            builder(MaxConcurrentRequests(options));
            return builder;
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="maxQueryStringLength">Maximum length of the query string.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static BuildFunc MaxQueryStringLength(this BuildFunc builder, int maxQueryStringLength)
        {
            return MaxQueryStringLength(builder, () => maxQueryStringLength);
        }

        /// <summary>
        /// Limits the length of the query string.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getMaxQueryStringLength">A delegate to get the maximum query string length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static BuildFunc MaxQueryStringLength(this BuildFunc builder, Func<int> getMaxQueryStringLength)
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
        public static BuildFunc MaxQueryStringLength(this BuildFunc builder, MaxQueryStringLengthOptions options)
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
        public static BuildFunc MaxRequestContentLength(this BuildFunc builder, int maxContentLength)
        {
            return MaxRequestContentLength(builder, () => maxContentLength);
        }

        /// <summary>
        /// Limits the length of the request content.
        /// </summary>
        /// <param name="builder">The <see cref="IAppBuilder"/> instance.</param>
        /// <param name="getMaxContentLength">A delegate to get the maximum content length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static BuildFunc MaxRequestContentLength(this BuildFunc builder, Func<int> getMaxContentLength)
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
        public static BuildFunc MaxRequestContentLength(this BuildFunc builder, MaxRequestContentLengthOptions options)
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
        public static BuildFunc MaxUrlLength(this BuildFunc builder, int maxUrlLength)
        {
            return MaxUrlLength(builder, () => maxUrlLength);
        }

        /// <summary>
        /// Limits the length of the URL.
        /// </summary>
        /// <param name="builder">The OWIN builder instance.</param>
        /// <param name="getMaxUrlLength">A delegate to get the maximum URL length.</param>
        /// <returns>The OWIN builder instance.</returns>
        public static BuildFunc MaxUrlLength(this BuildFunc builder, Func<int> getMaxUrlLength)
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
        public static BuildFunc MaxUrlLength(this BuildFunc builder, MaxUrlLengthOptions options)
        {
            builder.MustNotNull("builder");

            builder(LimitsMiddleware.MaxUrlLength(options));
            return builder;
        }
    }
}