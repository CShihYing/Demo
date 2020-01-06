using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ITSWeb.Helpers
{
    /// <summary>
    /// 取得目前應用程式預設組態的 AppSettingsSection 資料
    /// </summary>
    public static class ConfigHelper
    {
        #region 取得config資料
        /// <summary>
        /// 取得config資料，預設使用 HtmlDecode 
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            var config = ConfigurationManager.AppSettings.Get(key);
            if (!string.IsNullOrEmpty(config))
            {
                return HttpUtility.HtmlDecode(config);
            }
            return string.Empty;
        }
        #endregion

        #region 取得config資料
        /// <summary>
        /// 取得config資料（字串）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public static string GetStr(string key)
        {
            var config = ConfigurationManager.AppSettings.Get(key);
            if (!string.IsNullOrEmpty(config))
            {
                return config;
            }
            return string.Empty;
        }
        #endregion

        #region 取得config資料（數值）
        /// <summary>
        /// 取得config資料（數值）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static int GetInteger(string key, int output = 0)
        {
            var config = Get(key);

            var isNumeric = int.TryParse(config, out output);

            return output;
        }

        /// <summary>
        /// 取得config資料（數值）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static float GetFloat(string key, float output = 0)
        {
            var config = Get(key);

            var isNumeric = float.TryParse(config, out output);

            return output;
        }
        #endregion

        #region 取得config資料（布林值）
        /// <summary>
        /// 取得config資料（布林值）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public static bool GetBoolean(string key, bool output = false)
        {
            var config = Get(key);

            if (!string.IsNullOrEmpty(config))
            {
                return Convert.ToBoolean(config);
            }
            else
            {
                return output;
            }
        }
        #endregion

        #region 取得config資料（日期）
        /// <summary>
        /// 取得config資料（日期）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public static DateTime? GetDateTime(string key)
        {
            var config = Get(key);

            DateTime output;

            var isDateTime = DateTime.TryParse(config, out output);

            if (isDateTime) { return output; }

            return null;
        }
        #endregion

        #region 取得config資料（路徑）
        /// <summary>
        /// 取得config資料（路徑）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public static string GetPath(string key)
        {
            var config = Get(key);

            if (!string.IsNullOrEmpty(config))
            {
                return VirtualPathUtility.ToAbsolute(config);
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 取得config資料（加密）
        /// <summary>
        /// 取得config資料（加密）
        /// </summary>
        /// <param name="key">鍵值</param>
        /// <returns></returns>
        public static string GetEncrypt(string key)
        {
            var config = Get(key);
            
            if (!string.IsNullOrEmpty(config))
            {
                return EncryptHelper.decDecrypt(config);
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }
}