// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class Close
    {
        [TestMethod]
        public void VerifyClose()
        {

            try
            {
                MemoryStream ms = new MemoryStream();

                ms.WriteByte(0);

                OutputHelper.WriteLine("Close stream");
                ms.Close();

                Assert.ThrowsException(typeof(ObjectDisposedException),
                    () =>
                    {
                        OutputHelper.WriteLine("Verify actually closed by writing to it");
                        ms.WriteByte(0);
                    },
                    "Expected ObjectDisposedException");
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }
    }
}
