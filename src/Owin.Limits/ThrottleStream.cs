namespace Owin.Limits
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /* From http://www.codeproject.com/Articles/18243/Bandwidth-throttling
     * Release under CPOL licence http://www.codeproject.com/info/cpol10.aspx
     */

    internal class ThrottledStream : Stream
    {
        private const long Infinite = 0;
        private readonly Stream _baseStream;
        private long _byteCount;
        private readonly long _maximumBytesPerSecond;
        private long _start;
        
        public ThrottledStream(Stream baseStream, long maximumBytesPerSecond = Infinite)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            if (maximumBytesPerSecond < 0)
            {
                throw new ArgumentOutOfRangeException("maximumBytesPerSecond",
                    maximumBytesPerSecond,
                    "The maximum number of bytes per second can't be negatie.");
            }

            _baseStream = baseStream;
            _maximumBytesPerSecond = maximumBytesPerSecond;
            _start = CurrentMilliseconds;
            _byteCount = 0;
        }

        private long CurrentMilliseconds
        {
            get { return Environment.TickCount; }
        }

        public override bool CanRead
        {
            get { return _baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _baseStream.CanWrite; }
        }

        public override long Length
        {
            get { return _baseStream.Length; }
        }

        public override long Position
        {
            get { return _baseStream.Position; }
            set { _baseStream.Position = value; }
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override void Close()
        {
            _baseStream.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Throttle(count).Wait();

            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Throttle(count);
            await _baseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Throttle(count);
            return await _baseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Throttle(count).Wait();
            _baseStream.Write(buffer, offset, count);
        }

        public override string ToString()
        {
            return _baseStream.ToString();
        }

        private async Task Throttle(int bufferSizeInBytes)
        {
            // Make sure the buffer isn't empty.
            if (_maximumBytesPerSecond <= 0 || bufferSizeInBytes <= 0)
            {
                return;
            }

            _byteCount += bufferSizeInBytes;
            long elapsedMilliseconds = CurrentMilliseconds - _start;

            if (elapsedMilliseconds >= 0)
            {
                // Calculate the current bps.
                long bps = elapsedMilliseconds == 0 ? long.MaxValue : _byteCount*1000L/elapsedMilliseconds;

                // If the bps are more then the maximum bps, try to throttle.
                if (bps > _maximumBytesPerSecond)
                {
                    // Calculate the time to sleep.
                    long wakeElapsed = _byteCount*1000L/_maximumBytesPerSecond;
                    var toSleep = (int) (wakeElapsed - elapsedMilliseconds);

                    if (toSleep > 1)
                    {
                        try
                        {
                            // The time to sleep is more then a millisecond, so sleep.
                            await Task.Delay(toSleep);
                        }
                        catch (ThreadAbortException)
                        {
                            // Eatup ThreadAbortException.
                        }

                        // A sleep has been done, reset.
                        Reset();
                    }
                }
            }
        }

        private void Reset()
        {
            long difference = CurrentMilliseconds - _start;

            // Only reset counters when a known history is available of more then 1 second.
            if (difference > 1000)
            {
                _byteCount = 0;
                _start = CurrentMilliseconds;
            }
        }
    }
}   