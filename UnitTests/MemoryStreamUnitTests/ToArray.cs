// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class ToArray
    {
        #region Helper methods
        private bool VerifyArray(byte[] data, int expected)
        {
            bool result = true;

            OutputHelper.WriteLine("Verify Length");

            if (data.Length != expected)
            {
                result = false;
                OutputHelper.WriteLine("Expected " + expected + " bytes, but got " + data.Length);
            }

            OutputHelper.WriteLine("Verify pattern in array");

            int nextbyte = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != nextbyte)
                {
                    result = false;
                    OutputHelper.WriteLine("Byte in position " + i + " has wrong value: " + data[i]);
                }

                // Reset if wraps past 255
                if (++nextbyte > 255)
                {
                    nextbyte = 0;
                }
            }


            return result;
        }
        #endregion Helper methods

        #region Test Cases
        [TestMethod]
        public void Ctor_ToArray()
        {
            try
            {
                OutputHelper.WriteLine("Dynamic Stream");

                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] stream = ms.ToArray();

                    Assert.AreEqual(stream.Length, 0, $"Expected length 0, but got length {stream.Length}");
                }

                OutputHelper.WriteLine("Static Stream");

                using (MemoryStream ms = new MemoryStream(new byte[512]))
                {
                    byte[] stream = ms.ToArray();

                    Assert.AreEqual(stream.Length, 512, $"Expected length 512, but got length {stream.Length}");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void VerifyValues()
        {
            try
            {
                OutputHelper.WriteLine("Verify Data");

                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Write 1000 bytes in specific pattern");
                    MemoryStreamHelper.Write(ms, 1000);
                    byte[] stream = ms.ToArray();

                    Assert.IsTrue(VerifyArray(stream, 1000));
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void ChangeLengths()
        {
            try
            {
                OutputHelper.WriteLine("Verify array is still valid after truncation (copy array)");
                using (MemoryStream ms = new MemoryStream())
                {
                    MemoryStreamHelper.Write(ms, 1000);
                    ms.SetLength(200);
                    ms.Flush();
                    byte[] stream = ms.ToArray();
                    Assert.IsTrue(VerifyArray(stream, 200));
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
