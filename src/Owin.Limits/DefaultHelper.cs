namespace Owin.Limits {
    using System;
    using System.Diagnostics;

    internal static class DefaultHelper {
        public static Action<TraceEventType, string> Tracer {
            get { return (type, msg) => { }; }
        }
        public static Func<int, string> ReasonPhrase {
            get { return code => ""; }
        }
    }
}