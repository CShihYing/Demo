using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ITSWeb.Helpers
{
    public static class EncryptHelper
    {
        private static byte[] key = { 0xCF, 0xF3, 0x33, 0xBA, 0x11, 0xBE, 0x7C, 0xA9 };
        private static byte[] iv = { 0x2E, 0xA2, 0xDC, 0xCB, 0x82, 0x87, 0x0A, 0x2F };

        private static byte[] salt = new byte[] { 0x0A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0xF1 };

        #region 加密字串
        /// <summary>
        /// 加密字串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string decEncrypt(this string source)
        {
            var des = new DESCryptoServiceProvider();
            des.Key = new Rfc2898DeriveBytes(key, salt, 8).GetBytes(8);
            des.IV = new Rfc2898DeriveBytes(iv, salt, 8).GetBytes(8);

            var dataByteArray = Encoding.UTF8.GetBytes(source);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        #endregion

        #region 解密字串
        /// <summary>
        /// 解密字串
        /// </summary>
        /// <param name="encrypt"></param>
        /// <returns></returns>
        public static string decDecrypt(this string encrypt)
        {
            var des = new DESCryptoServiceProvider();
            des.Key = new Rfc2898DeriveBytes(key, salt, 8).GetBytes(8);
            des.IV = new Rfc2898DeriveBytes(iv, salt, 8).GetBytes(8);

            var dataByteArray = Convert.FromBase64String(encrypt);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        #endregion

        #region 密碼加密
        /// <summary>
        /// 密碼加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Password(this string source)
        {
            var obj = SHA1.Create();
            var crypto = obj.ComputeHash(Encoding.UTF8.GetBytes(source));

            return Convert.ToBase64String(crypto);
        }
        #endregion

        #region 加𥂭
        /// <summary>
        /// 加𥂭
        /// </summary>
        /// <returns></returns>
        public static string Salt()
        {
            var rng = new RNGCryptoServiceProvider();

            byte[] buf = new byte[15];

            // 將產生的密碼亂數填入byte[]陣列
            rng.GetBytes(buf);

            // 將陣列轉成字串
            string salt = Convert.ToBase64String(buf);

            return salt;
        }
        #endregion

        #region 字串遮罩
        /// <summary>
        /// 字串遮罩
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string Mask(this string source, string mask = "*")
        {
            return Mask(source, mask, true);
        }
        #endregion

        #region private
        private static string Mask(string source, string mask, bool disable)
        {
            var output = string.Empty;

            source = source.Trim();

            var sourceArry = source.ToCharArray();
            var array = new int[] { 0, source.Trim().Length - 1 };

            if (Regex.IsMatch(source.Trim(), @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$"))
            {
                // Email
                output += Mask(source.Split('@')[0], mask) + "@";

                for (int i = 0; i < source.Split('@')[1].Split('.').Length; i++)
                {
                    string oStrL = source.Split('@')[1].Split('.')[i].ToString();

                    if (i == 0)
                        output += Mask(oStrL, mask, false);
                    else
                    {
                        output += "." + Mask(oStrL, mask, false);
                    }
                }

                return output;
            }
            else if (Regex.IsMatch(source.Trim(), "^(09([0-9]){8})$"))
            {
                // 手機
                array = new int[] { 0, 1, 2, 7, 8, 9 };
            }
            else if (Regex.IsMatch(source.Trim(), "^[a-zA-Z][0-9]{9}$"))
            {
                // 身分證號
                array = new int[] { 0, 1, 2, 3, 5 };
            }

            for (var i = 0; i < sourceArry.Length; i++)
            {
                if (disable)
                {
                    output += array.Contains(i) ? sourceArry[i].ToString() : mask;
                }
                else
                {
                    output += array.Contains(i) ? mask : sourceArry[i].ToString();
                }
            }

            return output;

        }
        #endregion
    }
}