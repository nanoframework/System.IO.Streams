//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

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
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        /// <remarks>
        /// This method is called by the public <see cref="Dispose()"/> method and the Finalize method. By default, this method specifies the disposing parameter as <see langword="true"/>. Finalize specifies the disposing parameter as false.
        /// When the disposing parameter is true, this method releases all resources held by any managed objects that this TextReader references. This method invokes the <see cref="Dispose()"/> method of each referenced object.
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
        /// The <see cref="Peek"/> method returns an integer value in order to determine whether the end of the file, or another error has occurred. This allows a user to first check if the returned value is -1 before casting it to a Char type.
        /// </para>
        /// <para>
        /// The current position of the <see cref="TextReader"/> is not changed by this operation.The returned value is -1 if no more characters are available.The default implementation returns -1.
        /// </para>
        /// <para>The TextReader class is an abstract class. Therefore, you do not instantiate it in your code.For an example of using the Peek method, see the StreamReader.Peek method.
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
        /// This method returns after either count characters are read or the end of the file is reached. <see cref="ReadBlock"/> is a blocking version of this method.
        /// </para>
        /// <para>
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code.For an example of using the <see cref="Read(char[], int , int )"/> method, see the StreamReader.Read method.
        /// </para>
        /// </remarks>
        public virtual int Read(char[] buffer, int index, int count)
        {
            return -1;
        }

        /// <summary>
        /// Reads a specified maximum number of characters from the current text reader and writes the data to a buffer, beginning at the specified index.
        /// </summary>
        /// <param name="buffer">When this method returns, this parameter contains the specified character array with the values between <paramref name="index"/> and (<paramref name="index"/> + <paramref name="count"/> -1) replaced by the characters read from the current source.</param>
        /// <param name="index">The position in <paramref name="buffer"/> at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        /// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count"/>, depending on whether all input characters have been read.</returns>
        /// <remarks>
        /// <para>
        /// The position of the underlying text reader is advanced by the number of characters that were read into buffer.
        /// </para>
        /// <para>
        /// The method blocks until either count characters are read, or all characters have been read.This is a blocking version of <see cref="Read(char[], int, int)"/>.
        /// </para>
        /// </remarks>
        public virtual int ReadBlock(char[] buffer, int index, int count)
        {
            int i, n = 0;

            do
            {
                n += (i = Read(buffer, index + n, count - n));
            } while (i > 0 && n < count);

            return n;
        }

        /// <summary>
        /// Reads a line of characters from the text reader and returns the data as a string.
        /// </summary>
        /// <returns>The next line from the reader, or <see langword="null"/> if all characters have been read.</returns>
        /// <remarks>
        /// A line is defined as a sequence of characters followed by a carriage return (0x000d), a line feed (0x000a), a carriage return followed by a line feed, Environment.NewLine, or the end-of-stream marker. The string that is returned does not contain the terminating carriage return or line feed. The return value is null if the end of the input stream has been reached.
        /// If the method throws an OutOfMemoryException exception, the reader's position in the underlying Stream is advanced by the number of characters the method was able to read, but the characters that were already read into the internal ReadLine buffer are discarded. Because the position of the reader in the stream cannot be changed, the characters that were already read are unrecoverable and can be accessed only by reinitializing the TextReader object. If the initial position within the stream is unknown or the stream does not support seeking, the underlying Stream also needs to be reinitialized.
        /// To avoid such a situation and produce robust code you should use the Read method and store the read characters in a preallocated buffer.
        /// The TextReader class is an abstract class. Therefore, you do not instantiate it in your code.For an example of using the ReadLine method, see the StreamReader.ReadLine method.
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
        /// If the method throws an <see cref="OutOfMemoryException"/> exception, the reader's position in the underlying Stream is advanced by the number of characters the method was able to read, but the characters that were already read into the internal ReadToEnd buffer are discarded. Because the position of the reader in the stream cannot be changed, the characters that were already read are unrecoverable and can be accessed only by reinitializing the TextReader. If the initial position within the stream is unknown or the stream does not support seeking, the underlying Stream also needs to be reinitialized.
        /// To avoid such a situation and produce robust code you should use the Read method and store the read characters in a preallocated buffer.
        /// The <see cref="TextReader"/> class is an abstract class. Therefore, you do not instantiate it in your code.For an example of using the ReadToEnd method, see the StreamReader.ReadToEnd method.</remarks>
        public virtual string ReadToEnd()
        {
            return null;
        }
    }
}
