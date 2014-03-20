namespace Owin.Limits {
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;


    /*
     * Template from http://forums.asp.net/t/1966232.aspx?Web+API+OWIN+Host+Limit+Request+Size
     * 
     */

    internal class ContentLengthLimitingStream : Stream {
        private readonly Stream _innerStream;
        private long _totalBytesReadCount = 0;
        private readonly long _maxRequestSizeInBytes = 0;

        public ContentLengthLimitingStream(Stream innerStream, long maxReceivedMessageSize)
        {
            _innerStream = innerStream;
            _maxRequestSizeInBytes = maxReceivedMessageSize;
        }

        protected Stream InnerStream
        {
            get { return _innerStream; }
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

        public override int ReadTimeout
        {
            get { return _innerStream.ReadTimeout; }
            set { _innerStream.ReadTimeout = value; }
        }

        public override bool CanTimeout
        {
            get { return _innerStream.CanTimeout; }
        }

        public override int WriteTimeout
        {
            get { return _innerStream.WriteTimeout; }
            set { _innerStream.WriteTimeout = value; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream.Dispose();
            }
            base.Dispose(disposing);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int currentNumberOfBytesRead = _innerStream.Read(buffer, offset, count);

            ValidateRequestSize(currentNumberOfBytesRead);

            return currentNumberOfBytesRead;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int currentNumberOfBytesRead = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

            ValidateRequestSize(currentNumberOfBytesRead);

            return currentNumberOfBytesRead;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            int currentNumberOfBytesRead = _innerStream.EndRead(asyncResult);

            ValidateRequestSize(currentNumberOfBytesRead);

            return currentNumberOfBytesRead;
        }

        public override int ReadByte()
        {
            int currentNumberOfBytesRead = _innerStream.ReadByte();

            ValidateRequestSize(currentNumberOfBytesRead);

            return currentNumberOfBytesRead;
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return _innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _innerStream.FlushAsync(cancellationToken);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            _innerStream.EndWrite(asyncResult);
        }

        public override void WriteByte(byte value)
        {
            _innerStream.WriteByte(value);
        }

        private void ValidateRequestSize(int currentNumberOfBytesRead)
        {
            _totalBytesReadCount += currentNumberOfBytesRead;

            if (_totalBytesReadCount > _maxRequestSizeInBytes)
            {
                throw new ContentLengthExceededException(string.Format("Request size exceeds the allowed maximum size of {0} bytes", _maxRequestSizeInBytes));
            }
        } 
    }
}