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
    public class Reader
    {
        [TestMethod]
        public void LeaveOpen()
        {
            var TEST_TEXT = "This IS a TEST!";
            using (var stream = new MemoryStream())
            {
                //stream.SetLength(0);
                using (var streamWriter = new StreamWriter(stream, true))
                {
                    streamWriter.WriteLine(TEST_TEXT);
                }


                stream.Position = 0;

                using (StreamReader streamReader = new StreamReader(stream, true))
                {
                    var res1 = streamReader.ReadLine();
                    Assert.AreEqual(TEST_TEXT, res1);
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
