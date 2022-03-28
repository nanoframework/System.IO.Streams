//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Text;

namespace System.IO
{

    /// <summary>
    /// Represents a writer that can write a sequential series of characters. This class is abstract.
    /// </summary>
    [Serializable]
    public abstract class TextWriter : MarshalByRefObject, IDisposable
    {
        private const string _InitialNewLine = "\r\n";

        /// <summary>
        /// Stores the newline characters used for this <see cref="TextWriter"/>.
        /// </summary>
        protected char[] CoreNewLine = new char[] { '\r', '\n' };

        /// <summary>
        /// When overridden in a derived class, returns the character encoding in which the output is written.
        /// </summary>
        public abstract Encoding Encoding { get; }

        /// <summary>
        /// Gets or sets the line terminator string used by the current <see cref="TextWriter"/>.
        /// </summary>
        public virtual string NewLine
        {
            get
            {
                return new string(CoreNewLine);
            }

            set
            {
                if (value == null)
                {
                    value = _InitialNewLine;
                }

                CoreNewLine = value.ToCharArray();
            }
        }

        /// <summary>
        /// Closes the current writer and releases any system resources associated with the writer.
        /// </summary>
        public virtual void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="TextWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Releases all resources used by the <see cref="TextWriter"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This default method does nothing, but derived classes can override the method to provide the appropriate functionality.
        /// </para>
        /// <para>Flushing the stream will not flush its underlying encoder unless you explicitly call <see cref="Flush"/> or <see cref="Close"/>. Setting AutoFlush to <see langword="true"/> means that data will be flushed from the buffer to the stream, but the encoder state will not be flushed. This allows the encoder to keep its state (partial characters) so that it can encode the next block of characters correctly. This scenario affects UTF8 and UTF7 where certain characters can only be encoded after the encoder receives the adjacent character or characters.
        /// </para>
        /// </remarks>
        public virtual void Flush()
        {
        }

        /// <summary>
        /// Writes a character to the text stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public virtual void Write(char value)
        {
        }

        /// <summary>
        /// Writes a character array to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public virtual void Write(char[] buffer)
        {
            if (buffer != null)
            {
                Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="buffer"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentException">The <paramref name="buffer"/> length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        public virtual void Write(char[] buffer, int index, int count)
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

            for (int i = 0; i < count; i++)
            {
                Write(buffer[index + i]);
            }
        }

        /// <summary>
        /// Writes the text representation of a <see cref="bool"/> value to the text stream.
        /// </summary>
        /// <param name="value">The <see cref="bool"/> value to write.</param>
        public virtual void Write(bool value)
        {
            Write(value);
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public virtual void Write(int value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        [System.CLSCompliant(false)]
        public virtual void Write(uint value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public virtual void Write(long value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        [System.CLSCompliant(false)]
        public virtual void Write(ulong value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public virtual void Write(float value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public virtual void Write(double value)
        {
            Write(value.ToString());
        }

        /// <summary>
        /// Writes a string to the text stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public virtual void Write(string value)
        {
            if (value != null)
            {
                Write(value.ToCharArray());
            }
        }

        /// <summary>
        /// Writes the text representation of an object to the text stream by calling the <see cref="object.ToString"/> method on that object.
        /// </summary>
        /// <param name="value">The object to write.</param>
        public virtual void Write(object value)
        {
            if (value != null)
            {
                Write(value.ToString());
            }
        }

        /// <summary>
        /// Writes a line terminator to the text stream.
        /// </summary>
        public virtual void WriteLine()
        {
            Write(CoreNewLine);
        }

        /// <summary>
        /// Writes a character to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public virtual void WriteLine(char value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes a character array to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public virtual void WriteLine(char[] buffer)
        {
            Write(buffer);
            WriteLine();
        }

        /// <summary>
        /// Writes a subarray of characters to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="buffer"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentException">The <paramref name="buffer"/> length minus <paramref name="index"/> is less than <paramref name="count"/>.</exception>
        public virtual void WriteLine(char[] buffer, int index, int count)
        {
            Write(buffer, index, count);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of a <see cref="bool"/> value to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The <see cref="bool"/> value to write.</param>
        public virtual void WriteLine(bool value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of a 4-byte signed integer to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public virtual void WriteLine(int value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of a 4-byte unsigned integer to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        [System.CLSCompliant(false)]
        public virtual void WriteLine(uint value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of an 8-byte signed integer to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public virtual void WriteLine(long value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of an 8-byte unsigned integer to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        [System.CLSCompliant(false)]
        public virtual void WriteLine(ulong value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of a 4-byte floating-point value to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public virtual void WriteLine(float value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of an 8-byte floating-point value to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public virtual void WriteLine(double value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes a string to the text stream, followed by a line terminator.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public virtual void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }

        /// <summary>
        /// Writes the text representation of an object to the text stream by calling the <see cref="object.ToString"/> method on that object, followed by a line terminator.
        /// </summary>
        /// <param name="value">The object to write.</param>
        public virtual void WriteLine(object value)
        {
            if (value == null)
            {
                WriteLine();
            }
            else
            {
                WriteLine(value.ToString());
            }
        }
    }
}
