namespace Owin.Limits {
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class ContentLengthExceededException : Exception {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ContentLengthExceededException() {
        }
        public ContentLengthExceededException(string message) : base(message) {
        }
        public ContentLengthExceededException(string message, Exception inner) : base(message, inner) {
        }
        protected ContentLengthExceededException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {
        }
    }
}