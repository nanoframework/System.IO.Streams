// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class CanSeek
    {
        [TestMethod]
        public void CanSeek_Default_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify CanSeek is true for default Ctor");

                using (MemoryStream fs = new MemoryStream())
                {
                    Assert.IsTrue(fs.CanSeek, "Expected CanSeek == true, but got CanSeek == false");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void CanSeek_Byte_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify CanSeek is true for Byte[] Ctor");

                byte[] buffer = new byte[1024];

                using (MemoryStream fs = new MemoryStream(buffer))
                {
                    Assert.IsTrue(fs.CanSeek, "Expected CanSeek == true, but got CanSeek == false");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }
    }
}
