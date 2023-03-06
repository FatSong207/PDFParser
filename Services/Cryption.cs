using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace PdfParser.Cryption
{
    public class LicenceCryption
    {
        private string EncryptKey = "";
        private string EncryptIV = "";

        public LicenceCryption()
        {

            var jsonFileCryption = File.ReadAllText("./cryption.dll");

            Cryption Cryption = JsonConvert.DeserializeObject<Cryption>(jsonFileCryption);

            EncryptKey = Cryption.EncryptKey;
            EncryptIV = Cryption.EncryptIV;
        }

        public bool CheckLicense(string LicenseId, string LicenseKey)
        {
            try
            {
                return Decryption(LicenseKey) == LicenseId;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string DoMask(string PlainText)
        {
            if (string.IsNullOrWhiteSpace(PlainText))
            {
                return "";
            }
            else
            {            
                char[] ch = PlainText.ToCharArray();
                for (int i=1; i< ch.Length-1; i++)
                {
                    ch[i] = '*';
                }                
                return new string(ch); ;
            }
        }

        public string Decryption(string CipherText)
        {
            if (string.IsNullOrWhiteSpace(CipherText))
            {
                return CipherText;
            }
            else
            {
                ////D4G21QodH09z8JPwCgbCmg==
                //if(CipherText.Length < 24)
                //{
                //    for (int i = CipherText.Length; i <= 24; i++)
                //    {
                //        CipherText += "*";
                //    }
                //}

                //https://ithelp.ithome.com.tw/articles/10187947
                using (Aes aesAlg = Aes.Create())
                {
                    //加密金鑰(32 Byte)
                    aesAlg.Key = Encoding.Unicode.GetBytes(EncryptKey);
                    //初始向量(Initial Vector, iv) 類似雜湊演算法中的加密鹽(16 Byte)
                    aesAlg.IV = Encoding.Unicode.GetBytes(EncryptIV);
                    //加密器
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    //執行加密
                    byte[] decrypted = decryptor.TransformFinalBlock(Convert.FromBase64String(CipherText), 0, Convert.FromBase64String(CipherText).Length);
                    return Encoding.Unicode.GetString(decrypted);
                }
            }
        }

        internal static object DoMask(object userName)
        {
            throw new NotImplementedException();
        }

        public string Eecryption(string PlainText)
        {
            //https://ithelp.ithome.com.tw/articles/10187947
            using (Aes aesAlg = Aes.Create())
            {
                //加密金鑰(32 Byte)
                aesAlg.Key = Encoding.Unicode.GetBytes(EncryptKey);
                //初始向量(Initial Vector, iv) 類似雜湊演算法中的加密鹽(16 Byte)
                aesAlg.IV = Encoding.Unicode.GetBytes(EncryptIV);
                //加密器
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                //執行加密
                byte[] encrypted = encryptor.TransformFinalBlock(Encoding.Unicode.GetBytes(PlainText), 0, Encoding.Unicode.GetBytes(PlainText).Length);

                return Convert.ToBase64String(encrypted);
            }
        }
    }

    public class Cryption
    {
        public string EncryptKey { get; set; } = "";
        public string EncryptIV { get; set; } = "";
    }
}


