// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class CanRead
    {
        [TestMethod]
        public void CanRead_Default_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify CanRead is true for default Ctor");

                using (MemoryStream fs = new MemoryStream())
                {
                    Assert.IsTrue(fs.CanRead, "Expected CanRead == true, but got CanRead == false");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void CanRead_Byte_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify CanRead is true for Byte[] Ctor");

                byte[] buffer = new byte[1024];

                using (MemoryStream fs = new MemoryStream(buffer))
                {
                    Assert.IsTrue(fs.CanRead, "Expected CanRead == true, but got CanRead == false");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }
    }
}
