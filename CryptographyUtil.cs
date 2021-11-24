using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Gafware.Modules.DMS.Cryptography
{
	/// <summary>
	/// Summary description for CryptographyUtil.
	/// </summary>

	public class CryptographyUtil
	{
		private volatile byte[] _key = null;		
		private volatile byte[] _iv = null;

        private const string Key = "OUHUMANRESOURCESNORMANOKLAHOMA14";
        private const string IV = "OUDOCUMENTMANAGE";

        static string ByteToHexBitFiddle(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }
        
        public static string MD5(string plainText)
        {
            //The encoder class used to convert strPlainText to an array of bytes
            UTF8Encoding encoder = new UTF8Encoding();

            //Create an instance of the MD5CryptoServiceProvider class
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            //Call ComputeHash, passing in the plain-text string as an array of bytes
            //The return value is the encrypted value, as an array of bytes
            byte[] hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(plainText));

            return ByteToHexBitFiddle(hashedDataBytes);
        }

		public static string Encrypt(string plainText)
		{
            CryptographyUtil Crypt = new CryptographyUtil(Key, IV);
			return Crypt.EncryptToBase64String( plainText );
		}

		public static string Decrypt(string cipherText)
		{
            CryptographyUtil Crypt = new CryptographyUtil(Key, IV);
			return Crypt.DecryptFromBase64String( cipherText );
		}

        public static byte[] Encrypt(byte[] data)
        {
            CryptographyUtil Crypt = new CryptographyUtil(Key, IV);
            return Crypt.EncryptData(data);
        }

        public static string Decrypt(byte[] data)
        {
            CryptographyUtil Crypt = new CryptographyUtil(Key, IV);
            byte[] cryptdata = Crypt.DecryptData(data);
            return System.Text.UTF8Encoding.UTF8.GetString(cryptdata, 0, cryptdata.Length);
        }

        public static byte[] EncryptBytes(string plainText)
        {
            CryptographyUtil Crypt = new CryptographyUtil(Key, IV);
            return Crypt.EncryptData(System.Text.UTF8Encoding.UTF8.GetBytes(plainText));
        }

        public static byte[] DecryptBytes(string cipherText)
        {
            CryptographyUtil Crypt = new CryptographyUtil(Key, IV);
            return Crypt.DecryptData(System.Text.UTF8Encoding.UTF8.GetBytes(cipherText));
        }

		public CryptographyUtil(string key, string iv)
		{
			try
			{
                _key = System.Text.UTF8Encoding.UTF8.GetBytes(key);
                _iv = System.Text.UTF8Encoding.UTF8.GetBytes(iv);
			}
			catch(Exception ex)
			{
				throw new Exception( "Failed to create CryptographyUtil", ex.InnerException );
			}		
		}		

		public string EncryptToBase64String(string plainText)
		{
            if (!String.IsNullOrEmpty(plainText))
            {
                System.Security.Cryptography.AesManaged aes = new AesManaged();
                //Get an encryptor.
                ICryptoTransform encryptor = null;
                encryptor = aes.CreateEncryptor(_key, _iv);
                //Encrypt the data.
                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                UTF8Encoding _textConverter = new UTF8Encoding();
                //Convert the data to a byte array.
                byte[] plainTextBytes = _textConverter.GetBytes(plainText);
                //Write all data to the crypto stream and flush it.
                csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                csEncrypt.FlushFinalBlock();
                //Get encrypted array of bytes.
                byte[] encrypted = msEncrypt.ToArray();
                return System.Convert.ToBase64String(encrypted);
            }
            return String.Empty;
		}

        public byte[] EncryptData(byte[] plainTextBytes)
        {
            System.Security.Cryptography.AesManaged aes = new AesManaged();
            //Get an encryptor.
            ICryptoTransform encryptor = null;
            encryptor = aes.CreateEncryptor(_key, _iv);
            //Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            //Write all data to the crypto stream and flush it.
            csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
            csEncrypt.FlushFinalBlock();
            //Get encrypted array of bytes.
            byte[] encrypted = msEncrypt.ToArray();
            return encrypted;
        }

		public string DecryptFromBase64String(string cipherText)
		{            
			//Now decrypt the previously encrypted data obtained in the method above.
            if (!String.IsNullOrEmpty(cipherText))
            {
                System.Security.Cryptography.AesManaged aes = new AesManaged();
                //Get an decryptor.
                ICryptoTransform decryptor = null;
                decryptor = aes.CreateDecryptor(_key, _iv);
                byte[] cipherTextBytes = System.Convert.FromBase64String(cipherText);
                MemoryStream msDecrypt = new MemoryStream(cipherTextBytes);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                //Read the data out of the crypto stream.
                csDecrypt.Read(plainTextBytes, 0, plainTextBytes.Length);
                UTF8Encoding _textConverter = new UTF8Encoding();
                //Convert the byte array back into a string.
                return _textConverter.GetString(plainTextBytes, 0, plainTextBytes.Length).TrimEnd(new char[] { '\0' });
            }
            return String.Empty;
		}

        public byte[] DecryptData(byte[] cipherTextBytes)
        {
            //Now decrypt the previously encrypted data obtained in the method above.
            System.Security.Cryptography.AesManaged aes = new AesManaged();
            //Get an decryptor.
            ICryptoTransform decryptor = null;
            decryptor = aes.CreateDecryptor(_key, _iv);
            MemoryStream msDecrypt = new MemoryStream(cipherTextBytes);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            //Read the data out of the crypto stream.
            csDecrypt.Read(plainTextBytes, 0, plainTextBytes.Length);
            UTF8Encoding _textConverter = new UTF8Encoding();
            //Convert the byte array back into a string.
            return plainTextBytes;
        }
    }	
}
