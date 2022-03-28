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
    public class WriteByte
    {

        #region Helper methods

        private bool TestWrite(MemoryStream ms, int BytesToWrite)
        {
            return TestWrite(ms, BytesToWrite, ms.Position + BytesToWrite);
        }

        private bool TestWrite(MemoryStream ms, int BytesToWrite, long ExpectedLength)
        {
            bool result = true;
            long startLength = ms.Position;
            long nextbyte = startLength % 256;

            for (int i = 0; i < BytesToWrite; i++)
            {
                ms.WriteByte((byte)nextbyte);

                // Reset if wraps past 255
                if (++nextbyte > 255)
                    nextbyte = 0;
            }

            ms.Flush();
            if (ExpectedLength < ms.Length)
            {
                result = false;
                OutputHelper.WriteLine($"Expected final length of {ExpectedLength} bytes, but got {ms.Length} bytes");
            }

            return result;
        }

        #endregion Helper methods

        #region Test Cases

        [TestMethod]
        public void ExtendBuffer()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Set Position past end of stream");
                    // Internal buffer is initialized to 256, if this changes, this test is no longer valid.  
                    // Exposing capcity would have made this test easier/dynamic.

                    ms.Position = 300;
                    ms.WriteByte(123);

                    Assert.Equal(ms.Length, 301, $"Expected length 301, got length {ms.Length}");

                    ms.Position = 300;
                    int read = ms.ReadByte();

                    Assert.Equal(read, 123, $"Expected value 123, but got value {read}");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void InvalidRange()
        {

            try
            {
                byte[] buffer = new byte[100];
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    OutputHelper.WriteLine("Set Position past end of static stream");

                    ms.Position = buffer.Length + 1;

                    Assert.Throws(typeof(NotSupportedException),
                        () =>
                        {
                            ms.WriteByte(1);


                        },
                        "Expected NotSupportedException");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void VanillaWrite()
        {
            try
            {
                OutputHelper.WriteLine("Static Buffer");

                byte[] buffer = new byte[100];

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    OutputHelper.WriteLine("Write 50 bytes of data");

                    Assert.True(TestWrite(ms, 50, 100));

                    OutputHelper.WriteLine("Write final 50 bytes of data");

                    Assert.True(TestWrite(ms, 50, 100));

                    OutputHelper.WriteLine("Any more bytes written should throw");

                    Assert.Throws(typeof(NotSupportedException),
                        () =>
                        {
                            ms.WriteByte(50);
                        },
                        "Expected NotSupportedException");

                    OutputHelper.WriteLine("Rewind and verify all bytes written");

                    ms.Seek(0, SeekOrigin.Begin);

                    Assert.True(MemoryStreamHelper.VerifyRead(ms));
                }

                OutputHelper.WriteLine("Dynamic Buffer");

                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Write 100 bytes of data");
                    Assert.True(TestWrite(ms, 100));

                    OutputHelper.WriteLine("Extend internal buffer, write 160");
                    Assert.True(TestWrite(ms, 160));

                    OutputHelper.WriteLine("Double extend internal buffer, write 644");
                    Assert.True(TestWrite(ms, 644));

                    OutputHelper.WriteLine("write another 1100");
                    Assert.True(TestWrite(ms, 1100));

                    OutputHelper.WriteLine("Rewind and verify all bytes written");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assert.True(MemoryStreamHelper.VerifyRead(ms));
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void BoundaryCheck()
        {
            try
            {
                for (int i = 250; i < 260; i++)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        MemoryStreamHelper.Write(ms, i);
                        ms.Position = 0;

                        Assert.True(MemoryStreamHelper.VerifyRead(ms));

                        OutputHelper.WriteLine("Position: " + ms.Position);
                        OutputHelper.WriteLine("Length: " + ms.Length);

                        Assert.True(
                            (i != ms.Position | i != ms.Length),
                            $"Expected Position and Length to be {i}");
                    }
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine("Unexpected exception: " + ex.Message);
            }
        }

        #endregion Test Cases
    }
}
