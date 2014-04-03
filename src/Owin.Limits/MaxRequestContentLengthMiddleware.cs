namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Owin;
    using Owin.Limits.Annotations;

    [UsedImplicitly]
    internal class MaxRequestContentLengthMiddleware
    {
        private readonly Func<int> _getMaxContentLength;
        private readonly Func<IDictionary<string, object>, Task> _next;

        public MaxRequestContentLengthMiddleware(Func<IDictionary<string, object>, Task> next, Func<int> getMaxContentLength)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            if (getMaxContentLength == null)
            {
                throw new ArgumentNullException("getMaxContentLength");
            }

            _next = next;
            _getMaxContentLength = getMaxContentLength;
        }


        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            var context = new OwinContext(environment);
            IOwinRequest request = context.Request;
            string requestMethod = request.Method.Trim().ToUpper();

            if (requestMethod == "GET" || requestMethod == "HEAD")
            {
                await _next(environment);
                return;
            }
            int maxContentLength = _getMaxContentLength();
            if (!IsChunkedRequest(request))
            {
                string contentLengthHeaderValue = request.Headers.Get("Content-Length");
                if (contentLengthHeaderValue == null)
                {
                    context.Response.StatusCode = 411;
                    context.Response.ReasonPhrase = "The Content-Length header is missing.";
                    return;
                }
                int contentLength;
                if (!int.TryParse(contentLengthHeaderValue, out contentLength))
                {
                    context.Response.StatusCode = 400;
                    context.Response.ReasonPhrase = "The Content-Length header value is not a valid number.";
                    return;
                }
                if (contentLength > maxContentLength)
                {
                    context.Response.StatusCode = 413;
                    context.Response.ReasonPhrase = string.Format("The content is too large. It is only a value of {0} allowed.", maxContentLength);
                    return;
                }
            }

            request.Body = new ContentLengthLimitingStream(request.Body, maxContentLength);

            try
            {
                await _next(environment);
            }
            catch (ContentLengthExceededException)
            {
                context.Response.StatusCode = 413;
                context.Response.ReasonPhrase = string.Format("The content is too large. It is only a value of {0} allowed.", maxContentLength);
            }
        }

        private static bool IsChunkedRequest(IOwinRequest request)
        {
            string header = request.Headers.Get("Transfer-Encoding");
            return header != null && header.Equals("chunked", StringComparison.OrdinalIgnoreCase);
        }
    }
}