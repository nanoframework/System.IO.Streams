// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class Length
    {
        #region Helper methods

        private bool TestLength(MemoryStream ms, long expectedLength)
        {
            if (ms.Length != expectedLength)
            {
                OutputHelper.WriteLine("Expected length " + expectedLength + " but got, " + ms.Length);
                return false;
            }

            return true;
        }

        private bool TestPosition(MemoryStream ms, long expectedPosition)
        {
            if (ms.Position != expectedPosition)
            {
                OutputHelper.WriteLine("Expected position " + expectedPosition + " but got, " + ms.Position);
                return false;
            }

            return true;
        }

        #endregion Helper methods

        #region Test Cases

        [TestMethod]
        public void ObjectDisposed()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                ms.Close();

                long length = 0;

                Assert.ThrowsException(typeof(ObjectDisposedException),
                    () =>
                    {
                        length = ms.Length;
                    },
                    $"Expected ObjectDisposedException, but got length {length}");
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void LengthTests()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Set initial length to 50, and position to 50");
                    ms.SetLength(50);
                    ms.Position = 50;
                    Assert.IsTrue(TestLength(ms, 50));

                    OutputHelper.WriteLine("Write 'foo bar'");

                    StreamWriter sw = new StreamWriter(ms);
                    sw.Write("foo bar");
                    sw.Flush();
                    Assert.IsTrue(TestLength(ms, 57));

                    OutputHelper.WriteLine("Shorten Length to 30");
                    ms.SetLength(30);
                    Assert.IsTrue(TestLength(ms, 30));

                    OutputHelper.WriteLine("Verify position was adjusted");
                    Assert.IsTrue(TestPosition(ms, 30));

                    OutputHelper.WriteLine("Extend length to 100");
                    ms.SetLength(100);
                    Assert.IsTrue(TestLength(ms, 100));
                }

                OutputHelper.WriteLine("Verify memorystream is 0 bytes after close");

                using (MemoryStream ms = new MemoryStream())
                {
                    Assert.IsTrue(TestLength(ms, 0));
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
