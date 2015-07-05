/*   MSEA-WZSearcher-HackTool - A handy tool for MapleStory packet editing
    Copyright (C) 2012~2015 eaxvac/lastBattle https://github.com/eaxvac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using System.Text;

namespace MSEAHackUtility
{
    public static class ByteUtils
    {
        public static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        } 

        public static byte[] HexToBytes(string pValue)
        {
            // FIRST. Use StringBuilder.
            StringBuilder builder = new StringBuilder();
            // SECOND... USE STRINGBUILDER!... and LINQ.
            foreach (char c in pValue.Where(IsHexDigit).Select(Char.ToUpper))
            {
                builder.Append(c);
            }

            // THIRD. If you have an odd number of characters, something is very wrong.
            string hexString = builder.ToString();
            if (hexString.Length % 2 == 1)
            {
                //throw new InvalidOperationException("There is an odd number of hexadecimal digits in this string.");
                // I will just add a zero to the end, who cares (0 padding)
          //      Log.WriteLine(LogLevel.Debug, "Hexstring had an odd number of hexadecimal digits.");
                hexString += '0';
            }

            byte[] bytes = new byte[hexString.Length / 2];
            Random rand = new Random();
            // FOURTH. Use the for-loop like a pro :D
            for (int i = 0, j = 0; i < bytes.Length; i++, j += 2)
            {
                string byteString = String.Concat(hexString[j], hexString[j + 1]);
                if (byteString == "**")
                {
                    bytes[i] = (byte)rand.Next(0, byte.MaxValue);
                }
                else
                {
                    bytes[i] = HexToByte(byteString);
                }
            }
            return bytes;
        }

        /// <summary>
        /// Creates a hex-string from byte array.
        /// </summary>
        /// <param name="bytes">Input bytes.</param>
        /// <returns>String that represents the byte-array.</returns>
        public static string BytesToHex(byte[] bytes, string header = "")
        {
            StringBuilder builder = new StringBuilder(header);
            foreach (byte c in bytes)
            {
                builder.AppendFormat("{0:X2} ", c);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Checks if a character is a hexadecimal digit.
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>true if <paramref name="c"/>is a hexadecimal digit; otherwise, false.</returns>
        public static bool IsHexDigit(char c)
        {
            if (('0' <= c && c <= '9') || 
                ('A' <= c && c <= 'F') || 
                ('a' <= c && c <= 'f') ||
                c == '*') //we allow wildcards
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Convert a 2-digit hexadecimal string to a byte.
        /// </summary>
        /// <param name="hex">The hexadecimal string.</param>
        /// <returns>The byte representation of the string.</returns>
        private static byte HexToByte(string hex)
        {
            if (hex == null) throw new ArgumentNullException("hex");
            if (hex.Length == 0 || 2 < hex.Length)
            {
                throw new ArgumentOutOfRangeException("hex", "The hexadecimal string must be 1 or 2 characters in length.");
            }
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }
    }
}
