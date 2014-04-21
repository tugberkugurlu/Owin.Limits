namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Owin.Limits.Annotations;

    public abstract class MiddlewareBase
    {
        private readonly AppFunc _next;
        private readonly Tracer _tracer;

        protected MiddlewareBase(AppFunc next, Tracer tracer = null)
        {
            next.MustNotNull("next");

            _next = next;
            _tracer = tracer ?? DefaultDelegateHelper.DefaultTracer;
        }

        [UsedImplicitly]
        public async Task Invoke(IDictionary<string, object> environment)
        {
            environment.MustNotNull("environment");
            Stopwatch stopwatch = Stopwatch.StartNew();
            _tracer.AsVerbose("{0} processing start.", GetType().Name);
            await InvokeInternal(_next, environment);
            _tracer.AsVerbose("{0} processing end. Time taken {0}ms.", GetType().Name, stopwatch.ElapsedMilliseconds);
        }

        protected abstract Task InvokeInternal(AppFunc next, IDictionary<string, object> environment);
    }
}