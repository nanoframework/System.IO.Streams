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
