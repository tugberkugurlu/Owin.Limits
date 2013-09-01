namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    internal class ConnectionTimeoutMiddleware
    {
        private readonly Func<TimeSpan> _getConnectionTimeout;
        private readonly Func<IDictionary<string, object>, Task> _next;

        public ConnectionTimeoutMiddleware(Func<IDictionary<string, object>, Task> next, Func<TimeSpan> getConnectionTimeout)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            _next = next;
            _getConnectionTimeout = getConnectionTimeout;
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
            TimeSpan connectionTimeout = _getConnectionTimeout();
            context.Request.Body = new TimeoutStream(requestBodyStream, connectionTimeout);
            context.Response.Body = new TimeoutStream(responseBodyStream, connectionTimeout);

            await _next(environment);

            context.Request.Body = requestBodyStream;
            context.Response.Body = responseBodyStream;
        }
    }
}