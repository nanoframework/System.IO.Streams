﻿//
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
    public class CanWrite
    {

        [TestMethod]
        public void CanWrite_Default_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify CanWrite is true for default Ctor");
         
                using (MemoryStream fs = new MemoryStream())
                {
                    Assert.IsTrue(fs.CanWrite, "Expected CanWrite == true, but got CanWrite == false");
                }
            }
            catch (Exception ex)
            {
                 OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        public void CanWrite_Byte_Ctor()
        {
            try
            {
                OutputHelper.WriteLine("Verify CanWrite is true for Byte[] Ctor");
         
                byte[] buffer = new byte[1024];
                
                using (MemoryStream fs = new MemoryStream(buffer))
                {
                    Assert.IsTrue(fs.CanWrite, "Expected CanWrite == true, but got CanWrite == false");
                }
            }
            catch (Exception ex)
            {
                 OutputHelper.WriteLine($"Unexpected exception {ex}");
            }
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CanWrite_bool_isWritable_Ctor(bool isWritable)
        {
            // Arrange
            byte[] buffer = new byte[1024];

            // Act
            using MemoryStream fs = new MemoryStream(buffer, isWritable);

            // Assert
            Assert.AreEqual(isWritable, fs.CanWrite, $"Expected CanWrite == {isWritable}, but got CanWrite == {!isWritable}");
            if(!isWritable)
            {
                Assert.ThrowsException(typeof(NotSupportedException), () => fs.WriteByte(0), "Expected exception when attempt to write to a read only stream.");
            }
        }
    }
}
