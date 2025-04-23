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
    public class Writer
    {
        [TestMethod]
        public void LeaveOpen()
        {
            var TEST_TEXT1 = "This IS a TEST!";
            var TEST_TEXT2 = "This IS a TEST!";
            var TEST_TEXT = TEST_TEXT1 + TEST_TEXT2;

            using (var stream = new MemoryStream())
            {
                //stream.SetLength(0);
                using (var streamWriter = new StreamWriter(stream, true))
                {
                    streamWriter.Write(TEST_TEXT1);
                }
                Assert.AreEqual(TEST_TEXT1.Length, (int)stream.Length);
                Assert.AreEqual(TEST_TEXT1.Length, (int)stream.Position);

                stream.Position = 0;

                using (StreamReader streamReader = new StreamReader(stream, true))
                {
                    var res1 = streamReader.ReadLine();
                    Assert.AreEqual(TEST_TEXT1, res1);
                }

                stream.Position = stream.Length;

                using (var streamWriter = new StreamWriter(stream, true))
                {
                    streamWriter.WriteLine(TEST_TEXT2);
                }



                stream.Position = 0;

                using (StreamReader streamReader = new StreamReader(stream, false))
                {
                    var res1 = streamReader.ReadLine();
                    Assert.AreEqual(TEST_TEXT, res1);
                }

                try
                {
                    stream.Position = 0;
                    throw new Exception("Should not run");
                }
                catch
                {
                }
            }
        }
    }
}
