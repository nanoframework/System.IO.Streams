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
    public class MemoryStreamHelper
    {
        public static bool WriteReadEmpty(Stream ms)
        {
            bool result = true;
            if (ms.Position != 0)
            {
                result = false;
                OutputHelper.WriteLine("Expected postion 0, but got " + ms.Position);
            }

            if (ms.Length != 0)
            {
                result = false;
                OutputHelper.WriteLine("Expected length 0, but got " + ms.Length);
            }

            return WriteReadVerify(ms) & result;
        }

        public static bool WriteReadVerify(Stream ms)
        {
            bool result = Write(ms, 300);

            // Flush writes
            ms.Flush();

            OutputHelper.WriteLine("Seek to start and Read");
            ms.Seek(0, SeekOrigin.Begin);
            result &= VerifyRead(ms);

            return result;
        }

        public static bool Write(Stream ms, int length)
        {
            bool result = true;
            long startLength = ms.Length;

            // we can only write 0-255, so mod the 
            // length to figure out next data value
            long data = startLength % 256;

            OutputHelper.WriteLine("Seek to end");
            ms.Seek(0, SeekOrigin.End);

            OutputHelper.WriteLine("Write to file");
            for (long i = startLength; i < startLength + length; i++)
            {
                ms.WriteByte((byte)data++);

                // if we hit max byte, reset
                if (data > 255)
                {
                    data = 0;
                }
            }

            return result;
        }

        public static bool VerifyRead(Stream ms)
        {
            bool result = true;
            OutputHelper.WriteLine("Verify " + ms.Length + " bytes of data in file");

            // we can only read 0-255, so mod the 
            // position to figure out next data value
            int nextbyte = (int)ms.Position % 256;

            for (int i = 0; i < ms.Length; i++)
            {
                int readByte = ms.ReadByte();
                if (readByte != nextbyte)
                {
                    result = false;
                    OutputHelper.WriteLine("Byte in position " + i + " has wrong value: " + readByte);
                }

                // Reset if wraps past 255
                if (++nextbyte > 255)
                {
                    nextbyte = 0;
                }
            }

            return result;
        }

        public static byte[] GetRandomBytes(int length)
        {
            byte[] byteArr = new byte[length];

            Random s_random = new Random();
            
            s_random.NextBytes(byteArr);

            return byteArr;
        }
    }
}
