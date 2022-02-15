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
    public class Write
    {
        #region Local Helper methods

        private bool TestWrite(MemoryStream ms, int length)
        {
            return TestWrite(ms, length, length, 0);
        }

        private bool TestWrite(MemoryStream ms, int length, long ExpectedLength)
        {
            return TestWrite(ms, length, length, 0, ExpectedLength);
        }

        private bool TestWrite(MemoryStream ms, int BufferLength, int BytesToWrite, int Offset)
        {
            return TestWrite(ms, BufferLength, BytesToWrite, Offset, ms.Position + BytesToWrite);
        }

        private bool TestWrite(MemoryStream ms, int BufferLength, int BytesToWrite, int Offset, long ExpectedLength)
        {
            bool result = true;
            long startLength = ms.Position;
            long nextbyte = startLength % 256;

            byte[] byteBuffer = new byte[BufferLength];

            for (int i = Offset; i < (Offset + BytesToWrite); i++)
            {
                byteBuffer[i] = (byte)nextbyte;

                // Reset if wraps past 255
                if (++nextbyte > 255)
                {
                    nextbyte = 0;
                }
            }

            ms.Write(byteBuffer, Offset, BytesToWrite);
            ms.Flush();

            if (ExpectedLength < ms.Length)
            {
                result = false;
                OutputHelper.WriteLine($"Expected final length of {ExpectedLength} bytes, but got {ms.Length} bytes");
            }

            return result;
        }

        #endregion Local Helper methods

        #region Test Cases

        [TestMethod]
        public void InvalidCases()
        {
            MemoryStream fs = new MemoryStream();
            byte[] writebuff = new byte[1024];

            try
            {
                Assert.Throws(typeof(ArgumentNullException),
                    () =>
                      {
                          OutputHelper.WriteLine("Write to null buffer");
                          fs.Write(null, 0, writebuff.Length);

                      },
                "Expected ArgumentNullException"
                    );

                Assert.Throws(typeof(ArgumentOutOfRangeException),
                    () =>
                    {
                        OutputHelper.WriteLine("Write to negative offset");
                        fs.Write(writebuff, -1, writebuff.Length);
                    }
                    , "Expected ArgumentOutOfRangeException"
                    );


                Assert.Throws(typeof(ArgumentException),
    () =>
    {
        OutputHelper.WriteLine("Write to out of range offset");
        fs.Write(writebuff, writebuff.Length + 1, writebuff.Length);
    }
    ,
    "Expected ArgumentException"
    );

                Assert.Throws(typeof(ArgumentOutOfRangeException),
       () =>
       {
           OutputHelper.WriteLine("Write negative count");
           fs.Write(writebuff, 0, -1);


       }

       ,

       "Expected ArgumentOutOfRangeException");

                Assert.Throws(typeof(ArgumentException),
            () =>
            {
                OutputHelper.WriteLine("Write count larger then buffer");
                fs.Write(writebuff, 0, writebuff.Length + 1);
            }

            ,

            "Expected ArgumentException");

                Assert.Throws(typeof(ObjectDisposedException),
                () =>
                {
                    OutputHelper.WriteLine("Write closed stream");
                    fs.Close();
                    fs.Write(writebuff, 0, writebuff.Length);
                }
               ,

                "Expected ObjectDisposedException");

                Assert.Throws(typeof(ObjectDisposedException),
    () =>

    {
        OutputHelper.WriteLine("Write disposed stream");
        fs = new MemoryStream();
        fs.Dispose();
        fs.Write(writebuff, 0, writebuff.Length);
    }
    ,
    "Expected ObjectDisposedException");

            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine("Unexpected exception: " + ex.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
        }

        [TestMethod]
        public void VanillaWrite_Dynamic_Ctor()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Write 256 bytes of data");

                    Assert.True(TestWrite(ms, 256));

                    OutputHelper.WriteLine("Write middle of buffer");
                    Assert.True(TestWrite(ms, 256, 100, 100));

                    // 1000 - 256 - 100 = 644
                    OutputHelper.WriteLine("Write start of buffer");
                    Assert.True(TestWrite(ms, 1000, 644, 0));

                    OutputHelper.WriteLine("Write end of buffer");
                    Assert.True(TestWrite(ms, 1000, 900, 100));

                    OutputHelper.WriteLine("Rewind and verify all bytes written");

                    ms.Seek(0, SeekOrigin.Begin);
                    Assert.True(MemoryStreamHelper.VerifyRead(ms));

                    OutputHelper.WriteLine("Verify Read validation with UTF8 string");
                    ms.SetLength(0);

                    string test = "MFFramework Test";

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
                OutputHelper.WriteLine("Unexpected exception: " + ex.Message);
            }
        }

        [TestMethod]
        public void VanillaWrite_Static_Ctor()
        {
            try
            {
                byte[] buffer = new byte[1024];

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    OutputHelper.WriteLine("Write 256 bytes of data");
                    Assert.True(TestWrite(ms, 256, 1024));

                    OutputHelper.WriteLine("Write middle of buffer");
                    Assert.True(TestWrite(ms, 256, 100, 100, 1024));

                    // 1000 - 256 - 100 = 644
                    OutputHelper.WriteLine("Write start of buffer");
                    Assert.True(TestWrite(ms, 1000, 644, 0, 1024));

                    OutputHelper.WriteLine("Write past end of buffer");

                    Assert.Throws(typeof(NotSupportedException),
                        () =>
                        {
                            TestWrite(ms, 50, 1024);
                        },
                        "Expected NotSupportedException");

                    OutputHelper.WriteLine("Verify failed Write did not move position");
                    Assert.Equal(ms.Position, 1000, $"Expected position to be 1000, but it is {ms.Position}");

                    OutputHelper.WriteLine("Write final 24 bytes of static buffer");
                    Assert.True(TestWrite(ms, 24));

                    OutputHelper.WriteLine("Rewind and verify all bytes written");

                    ms.Seek(0, SeekOrigin.Begin);
                    Assert.True(MemoryStreamHelper.VerifyRead(ms));

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
                OutputHelper.WriteLine("Unexpected exception: " + ex.Message);
            }
        }

        [TestMethod]
        public void ShiftBuffer()
        {
            try
            {
                int bufSize;

                for (int i = 1; i < 10; i++)
                {
                    bufSize = i;

                    MemoryStream ms = new MemoryStream();

                    for (int j = 0; j < bufSize; ++j)
                    {
                        ms.WriteByte((byte)j);
                    }

                    // Move everything forward by 1 byte
                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] buf = ms.ToArray();
                    ms.Write(buf, 1, bufSize - 1);

                    ms.Seek(0, SeekOrigin.Begin);

                    //we'll read till one before the last since these should be shifted by 1
                    for (int j = 0; j < ms.Length - 1; ++j)
                    {
                        int bit = ms.ReadByte();

                        Assert.Equal(bit, j + 1, $"Err_8324t! Check #458551, Returned: {bit}, Expected: {j + 1}");
                    }

                    //last bit should be the same
                    Assert.Equal(ms.ReadByte(), i - 1, "Err_32947gs! Last bit is not correct Check VSWhdibey #458551");
                }

                //Buffer sizes of 9 (10 here since we shift by 1) and above doesn't have the above 'optimization' problem
                for (int i = 10; i < 64; i++)
                {
                    bufSize = i;

                    MemoryStream ms = new MemoryStream();

                    for (int j = 0; j < bufSize; ++j)
                    {
                        ms.WriteByte((byte)j);
                    }

                    // Move everything forward by 1 byte
                    ms.Seek(0, SeekOrigin.Begin);

                    byte[] buf = ms.ToArray();
                    ms.Write(buf, 1, bufSize - 1);

                    ms.Seek(0, SeekOrigin.Begin);

                    for (int j = 0; j < ms.Length; ++j)
                    {
                        int bit = ms.ReadByte();

                        if (j != ms.Length - 1)
                        {
                            Assert.Equal(bit, j + 1, $"Err_235radg_{i}! Check VSWhdibey #458551, Returned: {bit}, Expected: {j + 1}");
                        }
                        else
                        {
                            Assert.Equal(bit, j, $"Err_235radg_{i}! Check VSWhdibey #458551, Returned: {bit}, Expected:{j + 1}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine("Unexpected exception: " + ex.Message);
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
                        TestWrite(ms, i);
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
