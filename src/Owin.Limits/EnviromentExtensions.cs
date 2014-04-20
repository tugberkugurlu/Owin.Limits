namespace Owin.Limits
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal static class EnviromentExtensions
    {
        internal static AppFunc ToAppFunc(this Func<IDictionary<string, object>, Task> appFunc)
        {
            appFunc.MustNotNull("appFunc");
            return env => appFunc(env);
        }
    }
}