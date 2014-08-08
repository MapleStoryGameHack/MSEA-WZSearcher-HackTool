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
