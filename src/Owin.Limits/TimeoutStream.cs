namespace Owin.Limits
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Timer = System.Timers.Timer;

    internal class TimeoutStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly TimeSpan _timeout;
        private readonly Timer _timer;
        private readonly Action<TraceEventType, string> _tracer;

        public TimeoutStream(Stream innerStream, TimeSpan timeout, Action<TraceEventType, string> tracer)
        {
            _innerStream = innerStream;
            _timeout = timeout;
            _tracer = tracer;
            _timer = new Timer(_timeout.TotalMilliseconds)
            {
                AutoReset = false
            };
            _timer.Elapsed += (sender, args) =>
            {
                tracer.AsInfo("Timeout of {0} reached.".FormattedWith(_timeout));
                Close();
            };
            _timer.Start();
        }

        public override bool CanRead
        {
            get { return _innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _innerStream.CanWrite; }
        }

        public override long Length
        {
            get { return _innerStream.Length; }
        }

        public override long Position
        {
            get { return _innerStream.Position; }
            set { _innerStream.Position = value; }
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            _timer.Dispose();
            _innerStream.Close();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _innerStream.FlushAsync(cancellationToken);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int read = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
            Reset();
            return read;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
            Reset();
        }

        private void Reset()
        {
            _timer.Stop();
            _tracer.AsVerbose("Timeout timer reseted.");
            _timer.Start();
        }
    }
}