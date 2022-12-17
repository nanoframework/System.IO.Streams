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
    public class Position
    {
        #region Helper methods

        private bool GetSetPosition(MemoryStream ms, int TestLength)
        {
            bool success = true;

            OutputHelper.WriteLine("Move forwards");
            for (int i = 0; i < TestLength; i++)
            {
                ms.Position = i;

                if (ms.Position != i)
                {
                    success = false;
                    OutputHelper.WriteLine("Expected position " + i + " but got position " + ms.Position);
                }
            }

            OutputHelper.WriteLine("Move backwards");

            for (int i = TestLength - 1; i >= 0; i--)
            {
                ms.Position = i;

                if (ms.Position != i)
                {
                    success = false;
                    OutputHelper.WriteLine("Expected position " + i + " but got position " + ms.Position);
                }
            }

            return success;
        }

        #endregion Helper methods

        #region Test Cases

        [TestMethod]
        public void ObjectDisposed()
        {
            MemoryStream ms = new MemoryStream();
            ms.Close();

            try
            {
                try
                {
                    long position = ms.Position;

                    OutputHelper.WriteLine("Expected ObjectDisposedException, but got position " + position);
                }
                catch (ObjectDisposedException) { /*Pass Case */ }

                try
                {
                    ms.Position = 0;

                    OutputHelper.WriteLine("Expected ObjectDisposedException, but set position");
                }
                catch (ObjectDisposedException) { /*Pass Case */ }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine("Unexpected exception: " + ex.Message);
            }
        }

        [TestMethod]
        public void InvalidRange()
        {
            try
            {
                byte[] buffer = new byte[100];

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    OutputHelper.WriteLine("Try -1 postion");

                    Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                        () =>
                        {
                            ms.Position = -1;
                        },
                        "Expected ArgumentOutOfRangeException");

                    OutputHelper.WriteLine("Try Long.MinValue postion");

                    Assert.ThrowsException(typeof(ArgumentOutOfRangeException),
                        () =>
                        {
                            ms.Position = long.MinValue;
                        },
                        "Expected ArgumentOutOfRangeException");
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void GetSetStaticBuffer()
        {
            try
            {
                byte[] buffer = new byte[1000];

                OutputHelper.WriteLine("Get/Set Position with static buffer");

                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    Assert.IsTrue(GetSetPosition(ms, buffer.Length));
                }
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void GetSetDynamicBuffer()
        {
            try
            {
                OutputHelper.WriteLine("Get/Set Position with dynamic buffer");

                using (MemoryStream ms = new MemoryStream())
                {
                    Assert.IsTrue(GetSetPosition(ms, 1000));
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
