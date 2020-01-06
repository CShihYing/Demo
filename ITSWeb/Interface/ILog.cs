using System;

namespace ITSWeb.Interface
{
    /// <summary>
    /// Log
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// 訊息，記錄不影響系統執行的訊息，通常會記錄登入登出或是資料的建立刪除、傳輸等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        void Info(string message);

        /// <summary>
        /// 警告，用於需要提示的訊息，例如庫存不足、貨物超賣、餘額即將不足等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        void Warn(string message);

        /// <summary>
        /// 用於開發，於開發時將一些需要特別關注的訊息以Debug傳出。
        /// </summary>
        /// <param name="message">訊息內容</param>
        void Debug(string message);

        /// <summary>
        /// 錯誤，記錄系統實行所發生的錯誤，例如資料庫錯誤、遠端連線錯誤、發生例外等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        void Error(string message);

        /// <summary>
        /// 錯誤，記錄系統實行所發生的錯誤，例如資料庫錯誤、遠端連線錯誤、發生例外等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <param name="exception">發生例外</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// 錯誤，記錄系統實行所發生的錯誤，例如資料庫錯誤、遠端連線錯誤、發生例外等。
        /// </summary>
        /// <param name="exception">發生例外</param>
        void Error(Exception exception);

        /// <summary>
        /// 致命，用來記錄會讓系統無法執行的錯誤，例如資料庫無法連線、重要資料損毀等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        void Fatal(string message);

        /// <summary>
        /// 致命，用來記錄會讓系統無法執行的錯誤，例如資料庫無法連線、重要資料損毀等。
        /// </summary>
        /// <param name="exception">發生例外</param>
        void Fatal(Exception exception);

        /// <summary>
        /// 寄出訊息
        /// </summary>
        /// <param name="snamespace">命名空間</param>
        /// <param name="exception">發生例外</param>
        void SendException(string snamespace, Exception exception);
    }
}