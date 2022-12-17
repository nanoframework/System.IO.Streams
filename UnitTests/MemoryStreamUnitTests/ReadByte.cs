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
    public class ReadByte
    {
        #region Test Cases

        [TestMethod]
        public void InvalidCases()
        {
            try
            {
                MemoryStream ms2 = new MemoryStream();
                MemoryStreamHelper.Write(ms2, 100);

                ms2.Seek(0, SeekOrigin.Begin);
                ms2.Close();

                OutputHelper.WriteLine("Read from closed stream");

                int readBytes = 0;
                Assert.ThrowsException(typeof(ObjectDisposedException),
                    () =>
                    {
                        readBytes = ms2.ReadByte();
                    },
                    $"Expected ObjectDisposedException, but read {readBytes} bytes");
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void VanillaCases()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    MemoryStreamHelper.Write(ms, 256);
                    ms.Position = 0;

                    OutputHelper.WriteLine("ReadBytes and verify");

                    for (int i = 0; i < 256; i++)
                    {
                        int b = ms.ReadByte();

                        Assert.AreEqual(b, i, $"Expected {i} but got {b}");
                    }

                    OutputHelper.WriteLine("Bytes past EOS should return -1");

                    int rb = ms.ReadByte();

                    Assert.AreEqual(rb, -1, $"Expected -1 but got {rb}");
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
