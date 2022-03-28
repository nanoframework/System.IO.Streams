//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;
using System.Text;

namespace Sytem.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class Read
    {
        #region Local Helper methods

        private bool TestRead(MemoryStream ms, int length)
        {
            return TestRead(ms, length, length, length);
        }

        private bool TestRead(MemoryStream ms, int BufferLength, int BytesToRead, int BytesExpected)
        {
            bool result = true;
            int nextbyte = (int)ms.Position % 256;

            byte[] byteBuffer = new byte[BufferLength];

            int bytesRead = ms.Read(byteBuffer, 0, BytesToRead);

            if (bytesRead != BytesExpected)
            {
                result = false;
                OutputHelper.WriteLine($"Expected {BytesToRead} bytes, but got {bytesRead} bytes");
            }

            for (int i = 0; i < bytesRead; i++)
            {
                if (byteBuffer[i] != nextbyte)
                {
                    result = false;
                    OutputHelper.WriteLine($"Byte in position {i} has wrong value: {byteBuffer[i]}");
                }

                // Reset if wraps past 255
                if (++nextbyte > 255)
                {
                    nextbyte = 0;
                }
            }

            return result;
        }

        #endregion Local Helper methods

        #region Test Cases

        [TestMethod]
        public void InvalidCases()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("null Buffer");

                    int read = 0;

                    Assert.Throws(typeof(NotSupportedException),
                        () =>
                        {
                            read = ms.Read(null, 0, 0);
                        },
                        $"Expected ArgumentNullException, but read {read} bytes");

                    OutputHelper.WriteLine("negative offset");

                    Assert.Throws(typeof(ArgumentOutOfRangeException),
                        () =>
                        {
                            read = ms.Read(new byte[] { 1 }, -1, 0);
                        },
                        $"Expected ArgumentOutOfRangeException, but read {read} bytes");

                    OutputHelper.WriteLine("negative count");

                    Assert.Throws(typeof(ArgumentOutOfRangeException),
                        () =>
                        {
                            read = ms.Read(new byte[] { 1 }, 0, -1);
                        },
                        $"Expected ArgumentOutOfRangeException, but read {read} bytes");

                    OutputHelper.WriteLine("offset exceeds buffer size");

                    Assert.Throws(typeof(ArgumentException),
                        () =>
                        {
                            read = ms.Read(new byte[] { 1 }, 2, 0);
                        },
                        $"Expected ArgumentException, but read {read} bytes");

                    OutputHelper.WriteLine("count exceeds buffer size");

                    Assert.Throws(typeof(ArgumentException),
                        () =>
                        {
                            read = ms.Read(new byte[] { 1 }, 0, 2);
                        },
                        $"Expected ArgumentException, but read {read} bytes");
                }

                MemoryStream ms2 = new MemoryStream();
                MemoryStreamHelper.Write(ms2, 100);
                ms2.Seek(0, SeekOrigin.Begin);
                ms2.Close();

                OutputHelper.WriteLine("Read from closed stream");

                int readBytes = 0;
                Assert.Throws(typeof(ObjectDisposedException),
                    () =>
                    {
                        readBytes = ms2.Read(new byte[] { 50 }, 0, 50);
                    },
                    "Expected ObjectDisposedException, but read " + readBytes + " bytes");
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void VanillaRead()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Write to stream then reset to beginning
                    MemoryStreamHelper.Write(ms, 1000);
                    ms.Seek(0, SeekOrigin.Begin);

                    OutputHelper.WriteLine("Read 256 bytes of data");
                    Assert.True(TestRead(ms, 256));

                    OutputHelper.WriteLine("Request less bytes then buffer");
                    Assert.True(TestRead(ms, 256, 100, 100));

                    // 1000 - 256 - 100 = 644
                    OutputHelper.WriteLine("Request more bytes then file");
                    Assert.True(TestRead(ms, 1000, 1000, 644));

                    OutputHelper.WriteLine("Request bytes after EOF");
                    Assert.True(TestRead(ms, 100, 100, 0));

                    OutputHelper.WriteLine("Rewind and read entire file in one buffer larger then file");
                    ms.Seek(0, SeekOrigin.Begin);
                    Assert.True(TestRead(ms, 1001, 1001, 1000));

                    OutputHelper.WriteLine("Rewind and read from middle");
                    ms.Position = 500;
                    Assert.True(TestRead(ms, 256));

                    OutputHelper.WriteLine("Read position after EOS");
                    ms.Position = ms.Length + 10;
                    Assert.True(TestRead(ms, 100, 100, 0));

                    OutputHelper.WriteLine("Verify Read validation with UTF8 string");

                    ms.SetLength(0);
                    string test = "nanoFramework Test";

                    ms.Write(Encoding.UTF8.GetBytes(test), 0, test.Length);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] readbuff = new byte[20];
                    ms.Read(readbuff, 0, readbuff.Length);

                    string testResult = new string(Encoding.UTF8.GetChars(readbuff));
                    Assert.Equal(test, testResult, $"Exepected: {test}, but got: {testResult}");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        #endregion Test Cases
    }
}
