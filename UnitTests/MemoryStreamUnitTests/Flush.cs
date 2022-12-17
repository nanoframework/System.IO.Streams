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
