//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Collections;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// Implements a <see cref="TextReader"/> that reads characters from a byte stream in a particular encoding.
    /// </summary>
    public class StreamReader : TextReader
    {
        private const int c_MaxReadLineLen = 0xFFFF;
        private const int c_BufferSize = 512;

        // Initialized in constructor by CurrentEncoding scheme.
        // Encoding can be changed by resetting this variable.
        readonly Decoder _decoder;

        // temporary buffer used for decoder in Read() function.
        // Made it class member to save creation of buffer on each call to Read()
        // Initialized in StreamReader(String path)
        readonly char[] _singleCharBuff;

        private bool _disposed;
        private readonly bool _closable;

        // internal stream read buffer
        private byte[] _buffer;
        private int _curBufPos;
        private int _curBufLen;

        /// <summary>
        /// Returns the underlying stream.
        /// </summary>
        /// <value>The underlying stream.</value>
        /// <remarks>
        /// You use this property to access the underlying stream. The StreamReader class buffers input from the underlying stream when you call one of the Read methods. If you manipulate the position of the underlying stream after reading data into the buffer, the position of the underlying stream might not match the position of the internal buffer. To reset the internal buffer, call the DiscardBufferedData method; however, this method slows performance and should be called only when absolutely necessary. The StreamReader constructors that have the detectEncodingFromByteOrderMarks parameter can change the encoding the first time you read from the StreamReader object.
        /// </remarks>
        public virtual Stream BaseStream { get; private set; }

        /// <summary>
        /// Gets the current character encoding that the current <see cref="StreamReader"/> object is using.
        /// </summary>
        /// <value>The current character encoding used by the current reader. The value can be different after the first call to any <see cref="Read()"/> method of <see cref="StreamReader"/>, since encoding autodetection is not done until the first call to a <see cref="Read()"/> method.</value>
        public virtual Encoding CurrentEncoding => System.Text.Encoding.UTF8;

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end of the stream.
        /// </summary>
        /// <value><see langword="true"/> if the current stream position is at the end of the stream; otherwise <see langword="false"/>.</value>
        public bool EndOfStream
        {
            get
            {
                return _curBufLen == _curBufPos;
            }
        }
        internal bool LeaveOpen
        {
            get { return !_closable; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamReader"/> class for the specified stream.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="leaveOpen">Leave stream open after dispose writer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
        public StreamReader(Stream stream, bool leaveOpen = false)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException();
            }

            _singleCharBuff = new char[1];
            _buffer = new byte[c_BufferSize];
            _curBufPos = 0;
            _curBufLen = 0;
            BaseStream = stream;
            _decoder = CurrentEncoding.GetDecoder();
            _disposed = false;
            _closable = !leaveOpen;
        }



        /// <summary>
        /// Closes the <see cref="StreamReader"/> object and the underlying stream, and releases any system resources associated with the reader.
        /// </summary>
        /// <remarks>
        /// This method overrides the <see cref="TextReader.Close"/> method.
        /// This implementation of <see cref="Close"/> calls the <see cref="Dispose"/> method, passing a <see langword="true"/> value.
        /// Following a call to <see cref="Close"/>, any operations on the reader might raise exceptions.
        /// </remarks>
        public override void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Closes the underlying stream, releases the unmanaged resources used by the <see cref="StreamReader"/>, and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>
        /// This method is called by the public <see cref="Dispose"/> method and the Finalize method. Dispose invokes the protected <see cref="Dispose"/> method with the disposing parameter set to <see langword="true"/>. Finalize invokes <see cref="Dispose"/> with disposing set to <see langword="false"/>.
        /// When the disposing parameter is <see langword="true"/>, this method releases all resources held by any managed objects that the StreamReader object references.This method invokes the <see cref="Dispose"/> method of each referenced object.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            if (BaseStream != null)
            {
                if (disposing)
                {
                    if (!LeaveOpen)
                    {
                        BaseStream.Close();
                    }
                }

                BaseStream = null;
                _buffer = null;
                _curBufPos = 0;
                _curBufLen = 0;
            }

            _disposed = true;
        }

        /// <summary>
        /// Returns the next available character but does not consume it.
        /// </summary>
        /// <returns>An integer representing the next character to be read, or -1 if there are no characters to be read or if the stream does not support seeking.</returns>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <remarks>
        /// The <see cref="Peek"/> method returns an integer value in order to determine whether the end of the file, or another error has occurred. This allows a user to first check if the returned value is -1 before casting it to a <see cref="char"/> type.
        /// This method overrides <see cref="TextReader.Peek"/>.
        /// The current position of the <see cref="StreamReader"/> object is not changed by <see cref="Peek"/>.
        /// </remarks>
        public override int Peek()
        {
            int tempPos = _curBufPos;
            int nextChar;

            // If buffer needs refresh take into account max UTF8 bytes if the next character is UTF8 encoded
            // Note: In some occasions, _curBufPos may go beyond _curBufLen-1 (for example, when trying to peek after reading the last character of the buffer), so we need to refresh the buffer in these cases too
            if ((_curBufPos >= (_curBufLen - 1)) ||
               ((_buffer[_curBufPos + 1] & 0x80) != 0
                && (_curBufPos + 3 >= _curBufLen)))
            {
                // move any bytes read for this character to front of new buffer
                int totRead;
                for (totRead = 0; totRead < _curBufLen - _curBufPos; ++totRead)
                {
                    _buffer[totRead] = _buffer[_curBufPos + totRead];
                }

                // get the new buffer
                try
                {
                    // retry read until response timeout expires
                    while (BaseStream.Length > 0 && totRead < _buffer.Length)
                    {
                        int len = (int)(_buffer.Length - totRead);

                        if (len > BaseStream.Length)
                        {
                            len = (int)BaseStream.Length;
                        }

                        len = BaseStream.Read(_buffer, totRead, len);

                        if (len <= 0)
                        {
                            break;
                        }

                        totRead += len;
                    }
                }
                catch (Exception e)
                {
                    throw new IOException("Stream.Read", e);
                }

                tempPos = 0;
                _curBufPos = 0;
                _curBufLen = totRead;
            }

            // get the next character and reset _curBufPos
            nextChar = Read();

            _curBufPos = tempPos;

            return nextChar;
        }

        /// <summary>
        /// Reads the next character from the input stream and advances the character position by one character.
        /// </summary>
        /// <returns>The next character from the input stream represented as an <see cref="int"/> object, or -1 if no more characters are available.</returns>
        /// <remarks>
        /// This method overrides <see cref="TextReader.Read()"/>.
        /// This method returns an integer so that it can return -1 if the end of the stream has been reached. If you manipulate the position of the underlying stream after reading data into the buffer, the position of the underlying stream might not match the position of the internal buffer.To reset the internal buffer, call the DiscardBufferedData method; however, this method slows performance and should be called only when absolutely necessary.
        /// </remarks>
        public override int Read()
        {
            int byteUsed;

            while (true)
            {
                _decoder.Convert(
                    _buffer,
                    _curBufPos,
                    _curBufLen - _curBufPos,
                    _singleCharBuff,
                    0,
                    1,
                    false,
                    out byteUsed,
                    out System.Int32 charUsed,
                    out _);

                _curBufPos += byteUsed;

                if (charUsed == 1)
                {
                    // done here
                    break;
                }
                else
                {
                    // get more data to feed the decider and try again.
                    // try to fill the m_buffer.
                    // FillBufferAndReset purges processed data in front of buffer. Thus we can use up to full _buffer.Length
                    int readCount = _buffer.Length;

                    // Put it to the maximum of available data and readCount
                    readCount = readCount > (int)BaseStream.Length ? (int)BaseStream.Length : readCount;

                    if (readCount == 0)
                    {
                        readCount = 1;
                    }

                    // If there is no data, then return -1
                    if (FillBufferAndReset(readCount) == 0)
                    {
                        return -1;
                    }
                }
            }

            return _singleCharBuff[0];
        }

        /// <summary>
        /// Reads a specified maximum of characters from the current stream into a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The index of <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <returns>
        /// The number of characters that have been read, or 0 if at the end of the stream and no data was read. The number will be less than or equal to the <paramref name="count"/> parameter, depending on whether the data is available within the stream.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentException">The buffer length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        /// <exception cref="ObjectDisposedException">An I/O error occurs, such as the stream is closed.</exception>
        /// <remarks>
        /// This method overrides TextReader.Read.
        /// This method returns an integer so that it can return 0 if the end of the stream has been reached.
        /// When using the Read method, it is more efficient to use a buffer that is the same size as the internal buffer of the stream, where the internal buffer is set to your desired block size, and to always read less than the block size.If the size of the internal buffer was unspecified when the stream was constructed, its default size is 4 kilobytes(4096 bytes). If you manipulate the position of the underlying stream after reading data into the buffer, the position of the underlying stream might not match the position of the internal buffer.To reset the internal buffer, call the DiscardBufferedData method; however, this method slows performance and should be called only when absolutely necessary.
        /// This method returns after either the number of characters specified by the count parameter are read, or the end of the file is reached. <see cref="TextReader.ReadBlock(char[], int, int)"/> is a blocking version of <see cref="Read(char[], int, int)"/>.
        /// </remarks>
        public override int Read(
            char[] buffer,
            int index,
            int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (buffer.Length - index < count)
            {
                throw new ArgumentException();
            }

            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            int byteUsed, charUsed = 0;

            if (_curBufLen == 0)
            {
                _ = FillBufferAndReset(count);
            }

            int offset = 0;

            while (true)
            {
                _decoder.Convert(
                    _buffer,
                    _curBufPos,
                    _curBufLen - _curBufPos,
                    buffer,
                    offset,
                    count,
                    false,
                    out byteUsed,
                    out charUsed,
                    out _);

                count -= charUsed;
                _curBufPos += byteUsed;
                offset += charUsed;

                if (count == 0 || (FillBufferAndReset(count) == 0))
                {
                    break;
                }
            }

            return charUsed;
        }

        /// <summary>
        /// Reads a line of characters from the current stream and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the input stream, or <see langword="null"/> if the end of the input stream is reached.</returns>
        /// <exception cref="Exception"></exception>
        public override string ReadLine()
        {
            int bufLen = c_BufferSize;
            char[] readLineBuff = new char[bufLen];
            int growSize = c_BufferSize;
            int curPos = 0;
            int newChar;

            // Look for \r\n
            while ((newChar = Read()) != -1)
            {
                // Grow the line buffer if needed
                if (curPos == bufLen)
                {
                    if (bufLen + growSize > c_MaxReadLineLen)
                    {
                        throw new Exception();
                    }

                    char[] tempBuf = new char[bufLen + growSize];

                    Array.Copy(readLineBuff, 0, tempBuf, 0, bufLen);

                    readLineBuff = tempBuf;
                    bufLen += growSize;
                }

                // store the new character
                readLineBuff[curPos] = (char)newChar;

                if (readLineBuff[curPos] == '\n')
                {
                    return new string(readLineBuff, 0, curPos);
                }

                // check for \r and \r\n
                if (readLineBuff[curPos] == '\r')
                {
                    // If the next character is \n eat it
                    if (Peek() == '\n')
                    {
                        _ = Read();
                    }

                    return new string(readLineBuff, 0, curPos);
                }

                // move to the next byte
                ++curPos;
            }

            // reached end of stream. Send line up
            if (curPos == 0)
            {
                return null;
            }

            return new string(readLineBuff, 0, curPos);
        }

        /// <summary>
        /// Reads all characters from the current position to the end of the stream.
        /// </summary>
        /// <returns>The rest of the stream as a string, from the current position to the end. If the current position is at the end of the stream, returns an empty string ("").</returns>
        /// <remarks>
        /// This method overrides TextReader.ReadToEnd.
        /// ReadToEnd works best when you need to read all the input from the current position to the end of the stream.If more control is needed over how many characters are read from the stream, use the Read(Char[], Int32, Int32) method overload, which generally results in better performance.
        /// ReadToEnd assumes that the stream knows when it has reached an end.For interactive protocols in which the server sends data only when you ask for it and does not close the connection, ReadToEnd might block indefinitely because it does not reach an end, and should be avoided.
        /// Note that when using the Read method, it is more efficient to use a buffer that is the same size as the internal buffer of the stream.If the size of the buffer was unspecified when the stream was constructed, its default size is 4 kilobytes (4096 bytes).
        /// If the current method throws an OutOfMemoryException, the reader's position in the underlying Stream object is advanced by the number of characters the method was able to read, but the characters already read into the internal ReadLine buffer are discarded. If you manipulate the position of the underlying stream after reading data into the buffer, the position of the underlying stream might not match the position of the internal buffer. To reset the internal buffer, call the DiscardBufferedData method; however, this method slows performance and should be called only when absolutely necessary.
        /// </remarks>
        public override string ReadToEnd()
        {
            char[] result = null;

            if (BaseStream.CanSeek)
            {
                result = ReadSeekableStream();
            }
            else
            {
                result = ReadNonSeekableStream();
            }

            return new string(result);
        }

        private char[] ReadSeekableStream()
        {
            char[] chars = new char[(int)BaseStream.Length];

            _ = Read(chars, 0, chars.Length);

            return chars;
        }

        private char[] ReadNonSeekableStream()
        {
            ArrayList buffers = new();

            int read;
            int totalRead = 0;

            char[] lastBuffer = null;
            bool done = false;

            do
            {
                char[] chars = new char[c_BufferSize];

                read = Read(chars, 0, chars.Length);

                totalRead += read;

                if (read < c_BufferSize) // we are done
                {
                    if (read > 0) // copy last scraps
                    {
                        char[] newChars = new char[read];

                        Array.Copy(chars, newChars, read);

                        lastBuffer = newChars;
                    }

                    done = true;
                }
                else
                {
                    lastBuffer = chars;
                }

                buffers.Add(lastBuffer);
            }
            while (!done);

            if (buffers.Count > 1)
            {
                char[] text = new char[totalRead];

                int len = 0;
                for (int i = 0; i < buffers.Count; ++i)
                {
                    char[] buffer = (char[])buffers[i];

                    buffer.CopyTo(text, len);

                    len += buffer.Length;
                }

                return text;
            }
            else
            {
                return (char[])buffers[0];
            }
        }

        private int FillBufferAndReset(int count)
        {
            if (_curBufPos != 0) Reset();

            int totalRead = 0;

            try
            {
                while (count > 0 && _curBufLen < _buffer.Length)
                {
                    int spaceLeft = _buffer.Length - _curBufLen;

                    if (count > spaceLeft) count = spaceLeft;

                    int read = BaseStream.Read(_buffer, _curBufLen, count);

                    if (read == 0) break;

                    totalRead += read;
                    _curBufLen += read;

                    count -= read;
                }
            }
            catch (Exception e)
            {
                throw new IOException("Stream.Read", e);
            }

            return totalRead;
        }

        private void Reset()
        {
            int bytesAvailable = _curBufLen - _curBufPos;

            // here we trust that the copy in place doe not overwrites data
            Array.Copy(_buffer, _curBufPos, _buffer, 0, bytesAvailable);

            _curBufPos = 0;
            _curBufLen = bytesAvailable;
        }
    }
}
