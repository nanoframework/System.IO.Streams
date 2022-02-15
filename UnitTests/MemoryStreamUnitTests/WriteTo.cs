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
    public class WriteTo
    {

        #region Test Cases

        [TestMethod]
        public void InvalidArgs()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Initialize stream");
                    MemoryStreamHelper.Write(ms, 1000);

                    Assert.Throws(typeof(ArgumentNullException), () =>
                    {
                        OutputHelper.WriteLine("null stream");
                        ms.WriteTo(null);

                    },
                    "Expected ArgumentNullException"
                    );

                    Assert.Throws(typeof(ObjectDisposedException),
                        () =>
                    {
                        OutputHelper.WriteLine("Target Stream closed");
                        MemoryStream mst = new MemoryStream();
                        mst.Close();
                        ms.WriteTo(mst);
                    },
                    "Expected ObjectDisposedException"
                    );

                    OutputHelper.WriteLine("Current Stream closed");
                    ms.Close();

                    Assert.Throws(typeof(ObjectDisposedException),
                        () =>
                        {
                            using (MemoryStream mst = new MemoryStream())
                            {
                                ms.WriteTo(mst);

                                OutputHelper.WriteLine("Expected ObjectDisposedException");
                            }
                        },
                     "Expected ObjectDisposedException");

                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void WriteTo_MemoryStream()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    OutputHelper.WriteLine("Initialize stream with 1234 bytes");

                    MemoryStreamHelper.Write(ms, 1234);

                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        OutputHelper.WriteLine("WriteTo MemoryStream");
                        ms.WriteTo(ms2);

                        OutputHelper.WriteLine("Verify 2nd MemoryStream");

                        Assert.Equal(ms2.Length, 1234, $"Expected 1234 bytes, but got: {ms2.Length}");

                        ms2.Position = 0;

                        Assert.True(MemoryStreamHelper.VerifyRead(ms2));
                    }
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
