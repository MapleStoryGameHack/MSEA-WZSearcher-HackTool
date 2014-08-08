/*   MSEA-WZSearcher-HackTool - A handy tool for MapleStory packet editing
    Copyright (C) 2012~2014 eaxvac/lastBattle https://github.com/eaxvac

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
using System.Runtime.InteropServices;

namespace MSEAHackUtility
{
    /// <summary>
    /// Class to handle Maplestory RSA password encryption
    /// </summary>
    public class WzRSAEncrypt
    {
        [DllImport("WzCrypto.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int WzRSAEncryptString(byte[] ke, byte[] ran, byte[] dat, byte[] buff);

        /// <summary>
        /// Encrypts a Maplestory password to be sent to the server
        /// </summary>
        /// <param name="key">Unique key received from the server on Maplestory start</param>
        /// <param name="length">Length of the password</param>
        /// <param name="pass">Password to encrypt</param>
        /// <returns>Encrypted password</returns>
        public static byte[] Encrypt(byte[] key, int length, string password)
        {
            byte[] Data = System.Text.Encoding.ASCII.GetBytes(password);
            Random Rand = new Random();
            byte[] RandomInt = new byte[16];
            RandomInt[0] = (byte)Rand.Next();
            byte[] Buffer = new byte[length];

            WzRSAEncryptString(key, RandomInt, Data, Buffer);
            return Buffer;
        }
    }
}
