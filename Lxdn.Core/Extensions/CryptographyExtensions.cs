using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace Lxdn.Core.Extensions
{
    public static class CryptographyExtensions
    {
        public static string Anonymize(this string s, string realm)
        {
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day); // salt is the same within one day, so the generated bidder ids are unique within the same period

            using (var pdb = new Rfc2898DeriveBytes(realm, BitConverter.GetBytes(today.Ticks))) // alternatively, may use some other existing providers, e.g. like there
            using (var provider = Rijndael.Create())
            using (var encryptor = provider.CreateEncryptor(pdb.GetBytes(32), pdb.GetBytes(16)))
            using (var memory = new MemoryStream())
            {
                using (var encrypted = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                {
                    var data = Encoding.UTF8.GetBytes(s);
                    encrypted.Write(data, 0, data.Length);
                }

                return memory.ToArray().Aggregate(new StringBuilder(),
                    (builder, b) => builder.Append(b.ToString("X2"))).ToString();//.Substring(0, 8); // e.g. "CC9AC164"
            }
        }

        /// <summary>
        ///     Encryptes a string using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToEncrypt">String that must be encrypted.</param>
        /// <param name="key">Encryptionkey.</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        public static string Encrypt( this string stringToEncrypt, string publicKey )
        {
            if( string.IsNullOrEmpty( stringToEncrypt ) )
            {
                throw new ArgumentException( "An empty string value cannot be encrypted." );
            }

            if( string.IsNullOrEmpty(publicKey) )
            {
                throw new ArgumentException( "Cannot encrypt using an empty key. Please supply an encryption key." );
            }

            var parameters = new CspParameters { KeyContainerName = publicKey };

            using (var rsa = new RSACryptoServiceProvider(parameters) { PersistKeyInCsp = true })
            {
                var bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(stringToEncrypt), true);
                return BitConverter.ToString(bytes);
            }
        }

        /// <summary>
        ///     Decryptes a string using the supplied key. Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToDecrypt">String that must be decrypted.</param>
        /// <param name="key">Decryptionkey.</param>
        /// <returns>The decrypted string or null if decryption failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        public static string Decrypt( this string stringToDecrypt, string publicKey)
        {
            string result = null;

            if( string.IsNullOrEmpty( stringToDecrypt ) )
            {
                throw new ArgumentException( "An empty string value cannot be encrypted." );
            }

            if( string.IsNullOrEmpty(publicKey) )
            {
                throw new ArgumentException( "Cannot decrypt using an empty key. Please supply a decryption key." );
            }

            CspParameters cspp = new CspParameters();
            cspp.KeyContainerName = publicKey;

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            string[] decryptArray = stringToDecrypt.Split(new[] { "-" }, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll(decryptArray, (s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber))));

            byte[] bytes = rsa.Decrypt(decryptByteArray, true);

            result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        public static string CreateMd5Hash(this string input)
        {
            using (var md5 = MD5.Create())
            {
                var buffer = input.IfExists(Encoding.Default.GetBytes) ?? new byte[] {}; // .Default (=ANSI) is NOT good, should be UTF8
                var bytes = md5.ComputeHash(buffer);

                return bytes.Aggregate(new StringBuilder(), (hash, @byte) => 
                    hash.Append(@byte.ToString("X2"))).ToString();
            }
        }

        public static byte[] SymmetricEncrypt(this byte[] data, byte[] key, byte[] salt = null) // refactored from https://stackoverflow.com/questions/2150703/symmetric-encrypt-decrypt-in-net
        {
            using (var aes = new AesCryptoServiceProvider())
            using (var transform = aes.CreateEncryptor(key, salt))
            using (var memory = new MemoryStream())
            {
                using (var stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
                {
                    stream.Write(data, 0, data.Length);
                }

                return memory.ToArray();
            }
        }

        public static byte[] SymmetricDecrypt(this byte[] data, byte[] key, byte[] salt = null)
        {
            using (var aes = new AesCryptoServiceProvider())
            using (var transform = aes.CreateDecryptor(key, salt))
            using (var memory = new MemoryStream())
            {
                using (var stream = new CryptoStream(memory, transform, CryptoStreamMode.Write))
                {
                    stream.Write(data, 0, data.Length);
                }

                return memory.ToArray();
            }
        }

        public static byte[] SymmetricEncrypt(this string data, byte[] key, byte[] salt = null)
        {
            return Encoding.UTF8.GetBytes(data).SymmetricEncrypt(key, salt);
        }

        public static byte[] SymmetricEncrypt(this string data, string key, string salt = null)
        {
            return data.SymmetricEncrypt(Encoding.ASCII.GetBytes(key), salt.IfExists(Encoding.ASCII.GetBytes));
        }

        public static string SymmetricDecrypt(this byte[] data, string key, string salt = null)
        {
            var decrypted = data.SymmetricDecrypt(Encoding.ASCII.GetBytes(key), salt.IfExists(Encoding.ASCII.GetBytes));
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}

// key lengths: 128, 192, 256 bit (16, 24, 32 bytes)
// salt length: 128 bit