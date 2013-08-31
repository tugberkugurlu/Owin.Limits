namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    internal class MaxBandwidthMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly Func<int> _getMaxBytesPerSecond;

        public MaxBandwidthMiddleware(Func<IDictionary<string, object>, Task> next, Func<int> getGetMaxBytesPerSecond)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            _next = next;
            _getMaxBytesPerSecond = getGetMaxBytesPerSecond;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            var context = new OwinContext(environment);
            var requestBodyStream = context.Request.Body ?? Stream.Null;
            var responseBodyStream = context.Response.Body;
            var maxBytesPerSecond = _getMaxBytesPerSecond();
            if (maxBytesPerSecond < 0)
            {
                maxBytesPerSecond = 0;
            }
            context.Request.Body = new ThrottledStream(requestBodyStream, maxBytesPerSecond);
            context.Response.Body = new ThrottledStream(responseBodyStream, maxBytesPerSecond);

            //TODO consider SendFile interception

            await _next(environment);

            context.Request.Body = requestBodyStream;
            context.Response.Body = responseBodyStream;
        }
    }
}