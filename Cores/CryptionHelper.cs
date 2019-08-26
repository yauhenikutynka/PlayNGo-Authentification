using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Playngo.Modules.Authentication
{
    /// <summary>
    /// Encrypt & Decrypt Helper
    /// </summary>
    public class CryptionHelper
    {

        /// <summary>
        /// Authorization Key
        /// </summary>
        private const String AuthorizationKey = "$ccvza$fsr2#@";





        #region "public string EncryptString(string Value)"

        /// <summary>
        /// 3des Encrypt(ECB Encrypt,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="encryptValue">Encrypt String</param>
        /// <returns>Base64 String</returns>
        public static string EncryptString(string encryptValue)
        {
            return EncryptString(encryptValue, AuthorizationKey);
        }

        /// <summary>
        /// 3des Encrypt(ECB Encrypt,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="encryptValue">Encrypt String</param>
        /// <param name="key">key</param>
        /// <returnsBase64 String</returns>
        public static string EncryptString(string encryptValue, string key)
        {
            string enstring = "Encrypt Error!";
            ICryptoTransform ct; 
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateEncryptor();

            byt = Encoding.UTF8.GetBytes(encryptValue);

         

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                enstring = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                enstring = ex.ToString();
            }
            finally
            {
                cs.Close();
                cs.Dispose();
                ms.Close();
                ms.Dispose();
                des3.Clear();
                ct.Dispose();
            }
            enstring = Convert.ToBase64String(ms.ToArray());
            return enstring;
        }
        #endregion

        #region "public string DecryptString(string Value)"

        /// <summary>
        /// 3des Decrypt(ECB Encrypt,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="decryptString">Decrypt String</param>
        /// <returns>Normal String</returns>
        public static string DecryptString(string decryptString)
        {
            return DecryptString(decryptString, AuthorizationKey);
        }


        /// <summary>
        /// 3des Decrypt(ECB Encrypt,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="decryptString">Decrypt String</param>
        /// <param name="key">key</param>
        /// <returns>Normal String</returns>
        public static string DecryptString(string decryptString, string key)
        {
            string destring = "Decrypt String Error!";
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateDecryptor();

            byt = Convert.FromBase64String(decryptString);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                destring = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                destring = ex.ToString();
            }
            finally
            {
                ms.Close();
                cs.Close();
                ms.Dispose();
                cs.Dispose();
                ct.Dispose();
                des3.Clear();
            }
            return destring;
        }
        #endregion


      
        /// <summary>
        ///  split + string
        /// </summary>
        public static string splitStringLen(string s, int len, char b)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            if (s.Length >= len)
                return s.Substring(0, len);
            return s.PadRight(len, b);
        }
  



    }
}
