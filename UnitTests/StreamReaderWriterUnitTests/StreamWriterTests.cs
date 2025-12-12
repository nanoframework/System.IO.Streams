// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

        [TestMethod]
        public void TestWriteDouble()
        {
            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.Write(3.14154);
                writer.Write(" ");
                writer.Write(-123.456);
                writer.Write(" ");
                writer.Write(0.0);
            }

            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            Assert.IsTrue(result.Contains("3.14154"));
            Assert.IsTrue(result.Contains("-123.456"));
            Assert.IsTrue(result.Contains("0"));
        }

        [TestMethod]
        public void TestWriteDoubleWithWriteLine()
        {
            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.WriteLine(123.456);
                writer.WriteLine(-789.011);
            }

            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);
            string line1 = reader.ReadLine();
            string line2 = reader.ReadLine();

            Assert.IsTrue(line1.Contains("123.456"));
            Assert.IsTrue(line2.Contains("-789.011"));
        }

        [TestMethod]
        public void TestWriteReadOnlySpan()
        {
            string testText = "Hello from Span!";
            char[] charArray = testText.ToCharArray();
            ReadOnlySpan<char> span = new ReadOnlySpan<char>(charArray);

            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.Write(span);
            }

            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            Assert.AreEqual(testText, result);
        }

        [TestMethod]
        public void TestWriteReadOnlySpan_EmptySpan()
        {
            ReadOnlySpan<char> span = ReadOnlySpan<char>.Empty;

            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.Write(span);
            }

            Assert.AreEqual(0, (int)stream.Length);
        }

        [TestMethod]
        public void TestWriteReadOnlySpan_MultipleWrites()
        {
            ReadOnlySpan<char> span1 = new ReadOnlySpan<char>("First".ToCharArray());
            ReadOnlySpan<char> span2 = new ReadOnlySpan<char>(" ".ToCharArray());
            ReadOnlySpan<char> span3 = new ReadOnlySpan<char>("Second".ToCharArray());

            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.Write(span1);
                writer.Write(span2);
                writer.Write(span3);
            }

            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            Assert.AreEqual("First Second", result);
        }

        [TestMethod]
        public void TestWriteReadOnlySpan_LargeSpan()
        {
            // Create a large span that exceeds internal buffer size
            char[] chars = new char[5000];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)('A' + (i % 26));
            }
            ReadOnlySpan<char> span = new ReadOnlySpan<char>(chars);

            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.Write(span);
            }

            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            Assert.AreEqual(5000, result.Length);
            Assert.AreEqual('A', result[0]);
            Assert.AreEqual((char)('A' + (4999 % 26)), result[4999]);
        }

        [TestMethod]
        public void TestWriteMixedTypes()
        {
            using MemoryStream stream = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(stream, true))
            {
                writer.Write("Integer: ");
                writer.Write(42);
                writer.Write(", Double: ");
                writer.Write(3.14);
                writer.Write(", Float: ");
                writer.Write(2.71f);
                writer.Write(", String: ");
                ReadOnlySpan<char> span = new ReadOnlySpan<char>("Hello".ToCharArray());
                writer.Write(span);
            }

            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            Assert.IsTrue(result.Contains("42"));
            Assert.IsTrue(result.Contains("3.14"));
            Assert.IsTrue(result.Contains("2.71"));
            Assert.IsTrue(result.Contains("Hello"));
        }
    }
}
