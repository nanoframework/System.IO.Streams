// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
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

                    Assert.ThrowsException(typeof(ArgumentNullException), () =>
                    {
                        OutputHelper.WriteLine("null stream");
                        ms.WriteTo(null);

                    },
                    "Expected ArgumentNullException"
                    );

                    Assert.ThrowsException(typeof(ObjectDisposedException),
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

                    Assert.ThrowsException(typeof(ObjectDisposedException),
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

                        Assert.AreEqual(ms2.Length, 1234, $"Expected 1234 bytes, but got: {ms2.Length}");

                        ms2.Position = 0;

                        Assert.IsTrue(MemoryStreamHelper.VerifyRead(ms2));
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
