//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;

namespace Sytem.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class MemoryStream_Ctor
    {
        #region Test Cases

        [TestMethod]
        public void InvalidArguments()
        {
            Assert.Throws(
               typeof(ArgumentNullException), () =>
               {
                   OutputHelper.WriteLine("null buffer");
                   using (MemoryStream fs = new MemoryStream(null)) { }
                   OutputHelper.WriteLine("Expected ArgumentNullException");
               },
               "Unexpected exception");
        }

        [TestMethod]
        public void Valid_Default_Ctor()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Assert.True(ValidateMemoryStream(ms, 0), "Failed to ValidateMemoryStream");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception: {ex}");

            }
        }

        [TestMethod]
        public void Variable_Buffer_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify buffer constructors length 0-100");

                for (int i = 0; i < 100; i++)
                {
                    byte[] buffer = new byte[i];

                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        Assert.True(ValidateMemoryStream(ms, i), "Failed to ValidateMemoryStream");

                        OutputHelper.WriteLine("Try to extend beyond buffer length");

                        Assert.Throws(typeof(NotSupportedException),
                            () =>
                            {
                                ms.SetLength(i + 1);
                                OutputHelper.WriteLine("Expected NotSupportedException");
                            },
                            "Failed to throw NotSupportedException");

                        OutputHelper.WriteLine("Truncate to 0");

                        for (int j = buffer.Length; j >= 0; j--)
                        {
                            ms.SetLength(j);
                        }
                    }
                }

                OutputHelper.WriteLine("Verify 10k buffer constructor");

                byte[] largeBuffer = new byte[10000];

                using (MemoryStream ms = new MemoryStream(largeBuffer))
                {
                    Assert.True(ValidateMemoryStream(ms, largeBuffer.Length), "Failed to ValidateMemoryStream");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception: {ex}");
            }
        }

        #endregion Test Cases

        #region Helper methods

        private bool ValidateMemoryStream(
            MemoryStream ms,
            int ExpectedLength)
        {
            bool success = true;

            OutputHelper.WriteLine("Check Length");
            if (ms.Length != ExpectedLength)
            {
                success = false;
                OutputHelper.WriteLine($"Expected Length 0, but got Length {ms.Length}");
            }

            OutputHelper.WriteLine("Check CanSeek");
            if (!ms.CanSeek)
            {
                success = false;
                OutputHelper.WriteLine("Expected CanSeek to be true, but was false");
            }

            OutputHelper.WriteLine("Check CanRead");
            if (!ms.CanRead)
            {
                success = false;
                OutputHelper.WriteLine("Expected CanRead to be true, but was false");
            }

            OutputHelper.WriteLine("Check CanWrite");
            if (!ms.CanWrite)
            {
                success = false;
                OutputHelper.WriteLine("Expected CanWrite to be true, but was false");
            }

            if (ms.Position != 0)
            {
                success = false;
                OutputHelper.WriteLine("Expected Position to be 0, but was " + ms.Position);
            }

            return success;
        }

        #endregion Helper methods

    }
}
