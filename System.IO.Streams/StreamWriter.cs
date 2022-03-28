//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Text;

namespace System.IO
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for writing characters to a stream in a particular encoding.
    /// </summary>
    public class StreamWriter : TextWriter
    {
        private const string c_NewLine = "\r\n";
        private const int c_BufferSize = 0xFFF;

        private bool _disposed;
        private byte[] _buffer;
        private int _curBufPos;

        /// <summary>
        /// Gets the underlying stream that interfaces with a backing store.
        /// </summary>
        /// <value>The stream this <see cref="StreamWriter"/> is writing to.</value>
        public virtual Stream BaseStream { get; private set; }

        /// <summary>
        /// Gets the <see cref="Encoding"/> in which the output is written.
        /// </summary>
        /// <value>The Encoding specified in the constructor for the current instance, or <see cref="UTF8Encoding"/> if an encoding was not specified.</value>
        public override Encoding Encoding => Encoding.UTF8;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWriter"/> class for the specified stream by using UTF-8 encoding and the default buffer size.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not writable.</exception>
        public StreamWriter(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            if (!stream.CanWrite)
            {
                throw new ArgumentException();
            }

            BaseStream = stream;
            _buffer = new byte[c_BufferSize];
            _curBufPos = 0;
            _disposed = false;
        }

        /// <summary>
        /// Closes the current <see cref="StreamWriter"/> object and the underlying stream.
        /// </summary>
        /// <remarks>
        /// This method overrides <see cref="Stream.Close"/>.
        /// This implementation of <see cref="Close"/> calls the <see cref="Dispose"/> method passing a true value.
        /// You must call <see cref="Close"/> to ensure that all data is correctly written out to the underlying stream.Following a call to <see cref="Close"/>, any operations on the <see cref="StreamWriter"/> might raise exceptions. If there is insufficient space on the disk, calling <see cref="Close"/> will raise an exception.
        /// Flushing the stream will not flush its underlying encoder unless you explicitly call <see cref="Flush"/> or <see cref="Close"/>.
        /// </remarks>
        public override void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Causes any buffered data to be written to the underlying stream, releases the unmanaged resources used by the StreamWriter, and optionally the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>
        /// When the disposing parameter is <see langword="true"/>, this method releases all resources held by any managed objects that this StreamWriter references. This method invokes the <see cref="Dispose"/> method of each referenced object.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (BaseStream != null)
            {
                if (disposing)
                {
                    try
                    {
                        if (BaseStream.CanWrite)
                        {
                            Flush();
                        }
                    }
                    catch { }

                    try
                    {
                        BaseStream.Close();
                    }
                    catch { }
                }

                BaseStream = null;
                _buffer = null;
                _curBufPos = 0;
            }

            _disposed = true;
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The current writer is closed.</exception>
        /// <exception cref="IOException">An I/O error has occurred.</exception>
        /// <remarks>
        /// This method overrides TextWriter.Flush.
        /// Flushing the stream will not flush its underlying encoder unless you explicitly call Flush or <see cref="Close"/>.
        /// </remarks>
        public override void Flush()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            if (_curBufPos > 0)
            {
                try
                {
                    BaseStream.Write(_buffer, 0, _curBufPos);
                }
                catch (Exception e)
                {
                    throw new IOException("StreamWriter Flush. ", e);
                }

                _curBufPos = 0;
            }
        }

        /// <summary>
        /// Writes a character to the stream.
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// This method overrides <see cref="TextWriter.Write(char)"/>.
        /// The specified character is written to the underlying stream unless the end of the stream is reached prematurely.
        /// </remarks>
        public override void Write(char value)
        {
            byte[] buffer = Encoding.GetBytes(value.ToString());

            WriteBytes(buffer, 0, buffer.Length);
        }

        /// <inheritdoc/>
        public override void WriteLine()
        {
            byte[] tempBuf = Encoding.GetBytes(c_NewLine);

            WriteBytes(tempBuf, 0, tempBuf.Length);

            return;
        }

        /// <summary>
        /// Writes a string to the stream, followed by a line terminator.
        /// </summary>
        /// <remarks>
        /// This overload is equivalent to the <see cref="TextWriter.Write(string)"/> overload.
        /// The line terminator is defined by the CoreNewLine field.
        /// This method does not search the specified string for individual newline characters(hexadecimal 0x000a) and replace them with NewLine.
        /// </remarks>
        public override void WriteLine(string value)
        {
            byte[] tempBuf = Encoding.GetBytes(value + c_NewLine);
            WriteBytes(tempBuf, 0, tempBuf.Length);
            return;
        }

        internal void WriteBytes(
            byte[] buffer,
            int index,
            int count)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            // if this write will overrun the buffer flush the current buffer to stream and
            // write remaining bytes directly to stream.
            if (_curBufPos + count >= c_BufferSize)
            {
                // Flush the current buffer to the stream and write new bytes
                // directly to stream.
                try
                {
                    BaseStream.Write(_buffer, 0, _curBufPos);
                    _curBufPos = 0;

                    BaseStream.Write(buffer, index, count);

                    return;
                }
                catch (Exception e)
                {
                    throw new IOException("StreamWriter WriteBytes.", e);
                }
            }

            // else add bytes to the internal buffer
            Array.Copy(buffer, index, _buffer, _curBufPos, count);

            _curBufPos += count;

            return;
        }
    }
}


