// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO
{
    /// <summary>
    /// Represents a reader that can read a sequential series of characters.
    /// </summary>
    /// <remarks>
    /// <see cref="TextReader"/> is the abstract base class of StreamReader and StringReader, which read characters from streams and strings, respectively. Use these derived classes to open a text file for reading a specified range of characters, or to create a reader based on an existing stream.
    /// </remarks>
    [Serializable()]
    public abstract class TextReader : MarshalByRefObject, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextReader"/> class.
        /// </summary>
        /// <remarks>
        /// Use this constructor for derived classes.
        /// </remarks>
        protected TextReader() { }

        /// <summary>
        /// Closes the <see cref="TextReader"/> and releases any system resources associated with the <see cref="TextReader"/>.
        /// </summary>
        /// <remarks>
        /// This implementation of <see cref="Close"/> calls the <see cref="TextReader.Dispose(bool)"/> method and passes it a true value.
        /// </remarks>
        public virtual void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="TextReader"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TextReader"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>
        /// This method is called by the public <see cref="Dispose()"/> method and the Finalize method. By default, this method specifies the disposing parameter as <see langword="true"/>. Finalize specifies the disposing parameter as <see langword="false"/>.
        /// When the disposing parameter is <see langword="true"/>, this method releases all resources held by any managed objects that this <see cref="TextReader"/> references. This method invokes the <see cref="Dispose()"/> method of each referenced object.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Reads the next character without changing the state of the reader or the character source. Returns the next available character without actually reading it from the reader.
        /// </summary>
        /// <returns>An integer representing the next character to be read, or -1 if no more characters are available or the reader does not support seeking.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="Peek"/> method returns an integer value in order to determine whether the end of the file, or another error has occurred. This allows a user to first check if the returned value is -1 before casting it to a <see cref="char"/> type.
        /// </para>
        /// <para>
        /// The current position of the <see cref="TextReader"/> is not changed by this operation. The returned value is -1 if no more characters are available. The default implementation returns -1.
        /// </para>
        /// <para>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code. For an example of using the <see cref="Peek"/> method, see the <see cref="StreamReader.Peek"/> method.
        /// </para>
        /// </remarks>
        public virtual int Peek()
        {
            return -1;
        }

        /// <summary>
        /// Reads the next character from the text reader and advances the character position by one character.
        /// </summary>
        /// <returns>The next character from the text reader, or -1 if no more characters are available. The default implementation returns -1.</returns>
        /// <remarks>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code. For an example of using the Read method, see the StreamReader.Read method.
        /// </remarks>
        public virtual int Read()
        {
            return -1;
        }

        /// <summary>
        /// Reads a specified maximum number of characters from the current reader and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The position in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read. If the end of the reader is reached before the specified number of characters is read into the buffer, the method returns.</param>
        /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether the data is available within the reader. This method returns 0 (zero) if it is called when no more characters are left to read.</returns>
        /// <remarks>
        /// <para>
        /// This method returns after either <paramref name="count"/> characters are read or the end of the file is reached. <see cref="ReadBlock(char[], int, int)"/> is a blocking version of this method.
        /// </para>
        /// <para>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code. For an example of using the <see cref="Read(char[], int, int)"/> method, see the <see cref="StreamReader.Read(char[], int, int)"/> method.
        /// </para>
        /// </remarks>
        public virtual int Read(char[] buffer, int index, int count)
        {
            return -1;
        }

        /// <summary>
        /// Reads a specified maximum number of characters from the current text reader into a character span.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the character span with values replaced by the characters read from the current source.</param>
        /// <returns>The number of characters that have been read, or 0 if at the end of the reader and no data was read. The number will be less than or equal to the <paramref name="buffer"/> length, depending on whether the data is available within the reader.</returns>
        /// <remarks>
        /// <para>
        /// This method reads at most <paramref name="buffer"/>.Length characters from the current text reader and stores them in <paramref name="buffer"/>.
        /// </para>
        /// <para>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code. For an example of using the <see cref="Read(Span{char})"/> method, see the StreamReader.Read method.
        /// </para>
        /// </remarks>
        public virtual int Read(Span<char> buffer)
        {
            char[] array = new char[buffer.Length];
            int numRead = Read(array, 0, buffer.Length);

            if (numRead > 0)
            {
                new Span<char>(array, 0, numRead).CopyTo(buffer);
            }

            return numRead;
        }

        /// <summary>
        /// Reads a specified maximum number of characters from the current text reader and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, this parameter contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> - 1) replaced by the characters read from the current source.</param>
        /// <param name="index">The position in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether all input characters have been read.</returns>
        /// <remarks>
        /// <para>
        /// The position of the underlying text reader is advanced by the number of characters that were read into <paramref name="buffer"/>.
        /// </para>
        /// <para>
        /// The method blocks until either <paramref name="count"/> characters are read, or all characters have been read. This is a blocking version of <see cref="Read(char[], int, int)"/>.
        /// </para>
        /// </remarks>
        public virtual int ReadBlock(char[] buffer, int index, int count)
        {
            int i, n = 0;

            do
            {
                i = Read(buffer, index + n, count - n);
                n += i;
            } while (i > 0 && n < count);

            return n;
        }

        /// <summary>
        /// Reads a line of characters from the text reader and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the reader, or <see langword="null"/> if all characters have been read.</returns>
        /// <remarks>
        /// <para>
        /// A line is defined as a sequence of characters followed by a carriage return (0x000d), a line feed (0x000a), a carriage return followed by a line feed, or the end-of-stream marker. The string that is returned does not contain the terminating carriage return or line feed. The return value is <see langword="null"/> if the end of the input stream has been reached.
        /// </para>
        /// <para>
        /// If the method throws an <see cref="OutOfMemoryException"/>, the reader's position in the underlying <see cref="Stream"/> is advanced by the number of characters the method was able to read, but the characters that were already read into the internal ReadLine buffer are discarded. Because the position of the reader in the stream cannot be changed, the characters that were already read are unrecoverable and can be accessed only by reinitializing the <see cref="TextReader"/> object. If the initial position within the stream is unknown or the stream does not support seeking, the underlying <see cref="Stream"/> also needs to be reinitialized.
        /// </para>
        /// <para>
        /// To avoid such a situation and produce robust code you should use the <see cref="Read(char[], int, int)"/> method and store the read characters in a preallocated buffer.
        /// </para>
        /// <para>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code. For an example of using the <see cref="ReadLine"/> method, see the <see cref="StreamReader.ReadLine"/> method.
        /// </para>
        /// </remarks>
        public virtual string ReadLine()
        {
            return null;
        }

        /// <summary>
        /// Reads all characters from the current position to the end of the text reader and returns them as one string.
        /// </summary>
        /// <returns>A string that contains all characters from the current position to the end of the text reader.</returns>
        /// <remarks>
        /// <para>
        /// If the method throws an <see cref="OutOfMemoryException"/>, the reader's position in the underlying <see cref="Stream"/> is advanced by the number of characters the method was able to read, but the characters that were already read into the internal ReadToEnd buffer are discarded. Because the position of the reader in the stream cannot be changed, the characters that were already read are unrecoverable and can be accessed only by reinitializing the <see cref="TextReader"/>. If the initial position within the stream is unknown or the stream does not support seeking, the underlying <see cref="Stream"/> also needs to be reinitialized.
        /// </para>
        /// <para>
        /// To avoid such a situation and produce robust code you should use the <see cref="Read(char[], int, int)"/> method and store the read characters in a preallocated buffer.
        /// </para>
        /// <para>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code. For an example of using the <see cref="ReadToEnd"/> method, see the <see cref="StreamReader.ReadToEnd"/> method.
        /// </para>
        /// </remarks>
        public virtual string ReadToEnd()
        {
            return null;
        }
    }
}
