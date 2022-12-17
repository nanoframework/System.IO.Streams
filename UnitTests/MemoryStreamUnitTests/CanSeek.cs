//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;

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
