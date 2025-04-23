//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class StreamWriterTests
    {
        [TestMethod]
        public void TestReaderLeaveOpen()
        {
            string _testText1 = "This IS a TEST!";
            string _testText2 = "This IS a TEST!";
            string _testText = _testText1 + _testText2;

            using MemoryStream stream = new MemoryStream();

            using (StreamWriter streamWriter = new StreamWriter(stream, true))
            {
                streamWriter.Write(_testText1);
            }

            Assert.AreEqual(_testText1.Length, (int)stream.Length);
            Assert.AreEqual(_testText1.Length, (int)stream.Position);

            stream.Position = 0;

            using (StreamReader streamReader = new StreamReader(stream, true))
            {
                var res1 = streamReader.ReadLine();
                Assert.AreEqual(_testText1, res1);
            }

            stream.Position = stream.Length;

            using (StreamWriter streamWriter = new StreamWriter(stream, true))
            {
                streamWriter.WriteLine(_testText2);
            }

            stream.Position = 0;

            using (StreamReader streamReader = new StreamReader(stream, false))
            {
                var res1 = streamReader.ReadLine();
                Assert.AreEqual(_testText, res1);
            }

            // cannot set position on a disposed stream, test this by trying to set the position
            Assert.ThrowsException(typeof(ObjectDisposedException), () => stream.Position = 0);
        }
    }
}
