// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.TestFramework;

namespace System.IO.MemoryStreamUnitTests
{
    [TestClass]
    public class StreamReaderTests
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

            using (StreamWriter streamWriter = new StreamWriter(stream, true))
            {
                streamWriter.Write(_testText2);
            }

            Assert.AreEqual(_testText.Length, (int)stream.Length);
            Assert.AreEqual(_testText.Length, (int)stream.Position);

            using (StreamWriter streamWriter = new StreamWriter(stream, true))
            {
                streamWriter.WriteLine();
            }

            stream.Position = 0;

            using (StreamReader streamReader = new StreamReader(stream, true))
            {
                var res1 = streamReader.ReadLine();

                Assert.AreEqual(_testText, res1);
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

        [TestMethod]
        public void TestReadArray()
        {
            string testText = "Hello, World! This is a test of Read with arrays!!";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(testText);

            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);

            // Test reading into a buffer
            char[] buffer = new char[20];
            int charsRead = reader.Read(buffer, 0, 20);

            Assert.AreEqual(20, charsRead);
            Assert.AreEqual("Hello, World! This i", new string(buffer));

            // Read more
            charsRead = reader.Read(buffer, 0, 20);
            Assert.AreEqual(20, charsRead);
            Assert.AreEqual("s a test of Read wit", new string(buffer));

            // Read remaining (10 characters: "h arrays!!")
            char[] smallBuffer = new char[10];
            charsRead = reader.Read(smallBuffer, 0, 10);
            Assert.AreEqual(10, charsRead);
            Assert.AreEqual("h arrays!!", new string(smallBuffer));

            // Verify end of stream
            charsRead = reader.Read(buffer, 0, 20);
            Assert.AreEqual(0, charsRead);
        }

        [TestMethod]
        public void TestReadArray_EmptyStream()
        {
            using MemoryStream stream = new MemoryStream();
            using StreamReader reader = new StreamReader(stream);

            char[] buffer = new char[10];
            int charsRead = reader.Read(buffer, 0, 10);

            Assert.AreEqual(0, charsRead);
        }

        [TestMethod]
        public void TestReadArray_SmallChunks()
        {
            string testText = "ABC";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(testText);

            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);

            // Read one character at a time
            char[] buffer = new char[1];
            
            int charsRead = reader.Read(buffer, 0, 1);
            Assert.AreEqual(1, charsRead);
            Assert.AreEqual('A', buffer[0]);

            charsRead = reader.Read(buffer, 0, 1);
            Assert.AreEqual(1, charsRead);
            Assert.AreEqual('B', buffer[0]);

            charsRead = reader.Read(buffer, 0, 1);
            Assert.AreEqual(1, charsRead);
            Assert.AreEqual('C', buffer[0]);

            charsRead = reader.Read(buffer, 0, 1);
            Assert.AreEqual(0, charsRead);
        }

        [TestMethod]
        public void TestReadArray_LargerThanStream()
        {
            string testText = "Short";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(testText);

            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);

            // Buffer larger than content
            char[] buffer = new char[100];
            int charsRead = reader.Read(buffer, 0, 100);

            Assert.AreEqual(5, charsRead);
            char[] result = new char[charsRead];
            Array.Copy(buffer, 0, result, 0, charsRead);
            Assert.AreEqual("Short", new string(result));
        }

        [TestMethod]
        public void TestReadArray_Disposed()
        {
            using MemoryStream stream = new MemoryStream();
            StreamReader reader = new StreamReader(stream);
            reader.Dispose();

            char[] buffer = new char[10];
            Assert.ThrowsException(typeof(ObjectDisposedException), () => reader.Read(buffer, 0, 10));
        }

        [TestMethod]
        public void TestReadToEnd()
        {
            string testText = "This is a complete test of ReadToEnd method. It should read all characters from the current position to the end of the stream.";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(testText);

            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);

            string result = reader.ReadToEnd();

            Assert.AreEqual(testText, result);
        }

        [TestMethod]
        public void TestReadToEnd_EmptyStream()
        {
            using MemoryStream stream = new MemoryStream();
            using StreamReader reader = new StreamReader(stream);

            string result = reader.ReadToEnd();

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TestReadToEnd_AfterPartialRead()
        {
            string testText = "First part. Second part. Third part.";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(testText);

            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);

            // Read first 10 characters ("First part")
            char[] buffer = new char[10];
            reader.Read(buffer, 0, 10);

            // ReadToEnd should read the rest (". Second part. Third part.")
            string result = reader.ReadToEnd();

            Assert.AreEqual(". Second part. Third part.", result);
        }

        [TestMethod]
        public void TestReadToEnd_MultipleLines()
        {
            string testText = "Line 1\r\nLine 2\r\nLine 3";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(testText);

            using MemoryStream stream = new MemoryStream(bytes);
            using StreamReader reader = new StreamReader(stream);

            string result = reader.ReadToEnd();

            Assert.AreEqual(testText, result);
        }
    }
}
