// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO
{
    /// <summary>
    /// Specifies the position in a stream to use for seeking.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="SeekOrigin"/> is used by the Seek methods of <see cref="Stream"/>, <see cref="MemoryStream"/>, and other streams.
    /// </para>
    /// <para>
    /// These constants match Win32's FILE_BEGIN, FILE_CURRENT, and FILE_END values.
    /// </para>
    /// </remarks>
    [Serializable]
    public enum SeekOrigin
    {
        /// <summary>
        /// Specifies the beginning of a stream.
        /// </summary>
        /// <remarks>
        /// Seeking to the beginning of a stream sets the position to zero.
        /// </remarks>
        Begin = 0,

        /// <summary>
        /// Specifies the current position within a stream.
        /// </summary>
        /// <remarks>
        /// Seeking relative to the current position allows you to move forward or backward from the current position by specifying a positive or negative offset.
        /// </remarks>
        Current = 1,

        /// <summary>
        /// Specifies the end of a stream.
        /// </summary>
        /// <remarks>
        /// Seeking relative to the end of a stream allows you to position at or before the end by specifying a zero or negative offset.
        /// </remarks>
        End = 2,
    }
}
