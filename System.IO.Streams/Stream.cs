// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO
{
    /// <summary>
    /// Provides a generic view of a sequence of bytes. This is an abstract class.
    /// </summary>
    [Serializable]
    public abstract class Stream : MarshalByRefObject, IDisposable
    {
        // this value has been trimmed down from the original value that is used in full framework:81920.
        private const int _CopyToBufferSize = 2048;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value><see langword="true"/> if the stream supports reading; otherwise, <see langword="false"/>.</value>
        public abstract bool CanRead { get; }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value><see langword="true"/> if the stream supports seeking; otherwise, <see langword="false"/>.</value>
        public abstract bool CanSeek { get; }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <value><see langword="true"/> if the current stream can time out; otherwise, <see langword="false"/>.</value>
        public virtual bool CanTimeout => false;

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value><see langword="true"/> if the stream supports writing; otherwise, <see langword="false"/>.</value>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value>
        /// A <see cref="long"/> value representing the length of the stream in bytes.
        /// </value>
        public abstract long Length { get; }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value>
        /// The current position within the stream.
        /// </value>
        public abstract long Position { get; set; }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before timing out.
        /// </summary>
        /// <value>
        /// A value, in milliseconds, that determines how long the stream will attempt to read before timing out.
        /// </value>
        /// <exception cref="InvalidOperationException">Always thrown. Timeouts are not supported on this stream.</exception>
        public virtual int ReadTimeout
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before timing out.
        /// </summary>
        /// <value>
        /// A value, in milliseconds, that determines how long the stream will attempt to write before timing out.
        /// </value>
        /// <exception cref="InvalidOperationException">Always thrown. Timeouts are not supported on this stream.</exception>
        public virtual int WriteTimeout
        {
            get
            {
                throw new InvalidOperationException();
            }

            set
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// Instead of calling this method, ensure that the stream is properly disposed.
        /// </summary>
        /// <remarks>
        /// Stream used to require that all cleanup logic went into <see cref="Close"/>,
        /// which was thought up before <see cref="IDisposable"/> existed. However, the
        /// <see cref="IDisposable"/> pattern should be followed so that users can write
        /// sensible subclasses without inspecting all their base classes and without
        /// worrying about version brittleness.
        /// </remarks>
        public virtual void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reads the bytes from the current stream and writes them to another stream.
        /// </summary>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// The current stream does not support reading.
        /// -or-
        /// <paramref name="destination"/> does not support writing.
        /// </exception>
        /// <exception cref="ObjectDisposedException">Either the current stream or <paramref name="destination"/> was closed before the <see cref="CopyTo"/> method was called.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <remarks>
        /// Copying begins at the current position in the current stream, and does not reset the position of the destination stream after the copy operation is complete.
        /// </remarks>
        public void CopyTo(Stream destination)
        {
            ArgumentNullException.ThrowIfNull(destination);

            if (!CanRead && !CanWrite || !destination.CanRead && !destination.CanWrite)
            {
                throw new ObjectDisposedException();
            }

            if (!CanRead || !destination.CanWrite)
            {
                throw new NotSupportedException();
            }

            byte[] buffer = new byte[_CopyToBufferSize];
            int read;

            while ((read = Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                Close();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Finalizes the current instance of the <see cref="Stream"/> class.
        /// </summary>
        ~Stream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Stream"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public abstract void Flush();

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public abstract long Seek(
            long offset,
            SeekOrigin origin);

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public abstract void SetLength(long value);

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current source.</param>
        /// <returns>
        /// The total number of bytes read into the <paramref name="buffer"/>. This can be less than the number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        public abstract int Read(Span<byte> buffer);

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> -1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the <paramref name="buffer"/>. This can be less than the number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.
        /// </returns>
        public abstract int Read(
            byte[] buffer,
            int offset,
            int count);

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an <see cref="int"/>, or -1 if at the end of the stream.</returns>
        public virtual int ReadByte()
        {
            Span<byte> oneByteSpan = stackalloc byte[1];

            int r = Read(oneByteSpan);

            return r == 0 ? -1 : oneByteSpan[0];
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public abstract void Write(
            byte[] buffer,
            int offset,
            int count);

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">A region of memory. This method copies the contents of this region to the current stream.</param>
        /// <remarks>
        /// Use the <see cref="CanWrite"/> property to determine whether the current instance supports writing.
        /// If the write operation is successful, the position within the stream advances by the number of bytes written.
        /// If an exception occurs, the position within the stream remains unchanged.
        /// </remarks>
        public abstract void Write(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public virtual void WriteByte(byte value)
        {
            var oneByteArray = new byte[1];
            oneByteArray[0] = value;
            Write(oneByteArray, 0, 1);
        }
    }
}
