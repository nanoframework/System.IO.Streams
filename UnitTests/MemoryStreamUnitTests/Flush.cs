// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class Flush
    {
        [TestMethod]
        public void VerifyFlush()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] data = MemoryStreamHelper.GetRandomBytes(5000);

                    ms.Write(data, 0, data.Length);
                    ms.Flush();

                    Assert.AreEqual(ms.Length, 5000, $"Expected 5000 bytes, but got {ms.Length}");
                }
            }
            catch (Exception ex)
            {
                 OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }
    }
}
