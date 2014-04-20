namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class MaxRequestContentLengthMiddleware : MiddlewareBase
    {
        private readonly MaxRequestContentLengthOptions _options;

        public MaxRequestContentLengthMiddleware(Func<IDictionary<string, object>, Task> next, MaxRequestContentLengthOptions options)
            : base(next.ToAppFunc(), options.Tracer)
        {
            _options = options;
        }

        protected override async Task InvokeInternal(AppFunc next, IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            IOwinRequest request = context.Request;
            string requestMethod = request.Method.Trim().ToUpper();

            if (requestMethod == "GET" || requestMethod == "HEAD")
            {
                _options.Tracer.AsVerbose("GET or HEAD request without checking forwarded.");
                await next(environment);
                return;
            }
            int maxContentLength = _options.GetMaxContentLength();
            _options.Tracer.AsVerbose("Max valid content length is {0}.", maxContentLength);
            if (!IsChunkedRequest(request))
            {
                _options.Tracer.AsVerbose("Not a chunked request. Checking content lengt header.");
                string contentLengthHeaderValue = request.Headers.Get("Content-Length");
                if (contentLengthHeaderValue == null)
                {
                    _options.Tracer.AsInfo("No content length header provided. Request rejected.");
                    SetResponseStatusCodeAndReasonPhrase(context, 411);
                    return;
                }
                int contentLength;
                if (!int.TryParse(contentLengthHeaderValue, out contentLength))
                {
                    _options.Tracer.AsInfo("Invalid content length header value. Value: {0}", contentLengthHeaderValue);
                    SetResponseStatusCodeAndReasonPhrase(context, 400);
                    return;
                }
                if (contentLength > maxContentLength)
                {
                    _options.Tracer.AsInfo("Content length of {0} exceeds maximum of {1}. Request rejected.", contentLength, maxContentLength);
                    SetResponseStatusCodeAndReasonPhrase(context, 413);
                    return;
                }
                _options.Tracer.AsVerbose("Content length header check passed.");
            }
            else
            {
                _options.Tracer.AsVerbose("Chunked request. Content length header not checked.");
            }

            request.Body = new ContentLengthLimitingStream(request.Body, maxContentLength);
            _options.Tracer.AsVerbose("Request body stream configured with length limiting stream of {0}.", maxContentLength);

            try
            {
                _options.Tracer.AsVerbose("Request forwarded.");
                await next(environment);
                _options.Tracer.AsVerbose("Processing finished.");
            }
            catch (ContentLengthExceededException)
            {
                _options.Tracer.AsInfo("Content length of {0} exceeded. Request canceled and rejected.", maxContentLength);
                SetResponseStatusCodeAndReasonPhrase(context, 413);
            }
        }

        private static bool IsChunkedRequest(IOwinRequest request)
        {
            string header = request.Headers.Get("Transfer-Encoding");
            return header != null && header.Equals("chunked", StringComparison.OrdinalIgnoreCase);
        }

        private void SetResponseStatusCodeAndReasonPhrase(IOwinContext context, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ReasonPhrase = _options.LimitReachedReasonPhrase(context.Response.StatusCode);
        }
    }
}