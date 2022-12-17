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
