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
    public class Seek
    {

        #region Helper methods

        private bool TestSeek(MemoryStream ms, long offset, SeekOrigin origin, long expectedPosition)
        {
            bool result = true;
            long seek = ms.Seek(offset, origin);

            if (seek != ms.Position && seek != expectedPosition)
            {
                result = false;
                OutputHelper.WriteLine("Unexpected seek results!");
                OutputHelper.WriteLine("Expected position: " + expectedPosition);
                OutputHelper.WriteLine("Seek result: " + seek);
                OutputHelper.WriteLine("fs.Position: " + ms.Position);
            }

            return result;
        }

        private bool TestExtend(MemoryStream ms, long offset, SeekOrigin origin, long expectedPosition, long expectedLength)
        {
            bool result = TestSeek(ms, offset, origin, expectedLength);

            ms.WriteByte(1);
            if (ms.Length != expectedLength)
            {
                result = false;
                OutputHelper.WriteLine("Expected seek past end to change length to " + expectedLength + ", but its " + ms.Length);
            }

            return result;
        }

        #endregion Helper methods

        #region Test Cases

        [TestMethod]
        public void InvalidCases()
        {
            MemoryStream fs = new MemoryStream();
            MemoryStreamHelper.Write(fs, 1000);
            long seek;

            try
            {
                Assert.ThrowsException(typeof(IOException),
                    () =>
                    {
                        OutputHelper.WriteLine("Seek -1 from Begin");
                        seek = fs.Seek(-1, SeekOrigin.Begin);


                    },
                    $"Expected IOException, but got position");

                Assert.ThrowsException(typeof(IOException),
                    () =>
                    {
                        OutputHelper.WriteLine("Seek -1001 from Current - at end from write");
                        seek = fs.Seek(-1001, SeekOrigin.Current);

                    },
                    "Expected IOException, but got position ");

                Assert.ThrowsException(typeof(IOException),
                    () =>

                    {
                        OutputHelper.WriteLine("Seek -1001 from End");
                        seek = fs.Seek(-1001, SeekOrigin.End);
                    },
                    "Expected IOException, but got position ");

                Assert.ThrowsException(typeof(ArgumentException),
                    () =>
                    {
                        OutputHelper.WriteLine("Seek invalid -1 origin");
                        seek = fs.Seek(1, (SeekOrigin)(-1));
                    },
                    "Expected ArgumentException, but got position");

                Assert.ThrowsException(typeof(ArgumentException),
                    () =>
                    {
                        OutputHelper.WriteLine("Seek invalid 10 origin");
                        seek = fs.Seek(1, (SeekOrigin)10);
                    },
                    "Expected ArgumentException, but got position");

                Assert.ThrowsException(typeof(ObjectDisposedException),
                    () =>
                    {
                        OutputHelper.WriteLine("Seek with closed stream");
                        fs.Close();
                        seek = fs.Seek(0, SeekOrigin.Begin);
                    },
                    "Expected ObjectDisposedException, but got position");

                Assert.ThrowsException(typeof(ObjectDisposedException),
                    () =>
                    {
                        OutputHelper.WriteLine("Seek with disposed stream");
                        fs.Dispose();
                        seek = fs.Seek(0, SeekOrigin.End);
                    },
                    "Expected ObjectDisposedException, but got position");
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
        public void ValidCases()
        {
            try
            {
                using (MemoryStream fs = new MemoryStream())
                {
                    MemoryStreamHelper.Write(fs, 1000);

                    OutputHelper.WriteLine("Seek to beginning");
                    Assert.IsTrue(TestSeek(fs, 0, SeekOrigin.Begin, 0));

                    OutputHelper.WriteLine("Seek forward offset from begging");
                    Assert.IsTrue(TestSeek(fs, 10, SeekOrigin.Begin, 0));

                    OutputHelper.WriteLine("Seek backwards offset from current");
                    Assert.IsTrue(TestSeek(fs, -5, SeekOrigin.Current, 5));

                    OutputHelper.WriteLine("Seek forwards offset from current");
                    Assert.IsTrue(TestSeek(fs, 20, SeekOrigin.Current, 25));

                    OutputHelper.WriteLine("Seek to end");
                    Assert.IsTrue(TestSeek(fs, 0, SeekOrigin.End, 1000));

                    OutputHelper.WriteLine("Seek backwards offset from end");
                    Assert.IsTrue(TestSeek(fs, -35, SeekOrigin.End, 965));

                    OutputHelper.WriteLine("Seek past end relative to End");
                    Assert.IsTrue(TestExtend(fs, 1, SeekOrigin.End, 1001, 1002));

                    OutputHelper.WriteLine("Seek past end relative to Begin");
                    Assert.IsTrue(TestExtend(fs, 1002, SeekOrigin.Begin, 1002, 1003));

                    OutputHelper.WriteLine("Seek past end relative to Current");
                    Assert.IsTrue(TestSeek(fs, 995, SeekOrigin.Begin, 995));

                    Assert.IsTrue(TestExtend(fs, 10, SeekOrigin.Current, 1005, 1006));

                    // 1000 --123456
                    // verify 011001
                    OutputHelper.WriteLine("Verify proper bytes written at end (zero'd bytes from seek beyond end)");

                    byte[] buff = new byte[6];
                    byte[] verify = new byte[] { 0, 1, 1, 0, 0, 1 };
                    fs.Seek(-6, SeekOrigin.End);
                    fs.Read(buff, 0, buff.Length);

                    for (int i = 0; i < buff.Length; i++)
                    {
                        Assert.AreEqual(buff[i], verify[i], $"Position {i}:{buff[i]} != {verify[i]}");
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
