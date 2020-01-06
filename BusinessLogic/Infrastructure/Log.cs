using System;
using System.Linq;
using System.Text;
using BusinessLogic.Interface;
using DataAccess.Domain;
using NLog;

namespace BusinessLogic.Infrastructure
{
    /// <summary>
    /// 記錄錯誤訊息
    /// </summary>
    public class Log : ILog
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///  訊息，記錄不影響系統執行的訊息，通常會記錄登入登出或是資料的建立刪除、傳輸等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        public void Info(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        ///  警告，用於需要提示的訊息，例如庫存不足、貨物超賣、餘額即將不足等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        public void Warn(string message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        ///  用於開發，於開發時將一些需要特別關注的訊息以Debug傳出。
        /// </summary>
        /// <param name="message">訊息內容</param>
        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        /// <summary>
        /// 錯誤，記錄系統實行所發生的錯誤，例如資料庫錯誤、遠端連線錯誤、發生例外等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        public void Error(string message)
        {
            Logger.Error(message);
        }

        /// <summary>
        ///  錯誤，記錄系統實行所發生的錯誤，例如資料庫錯誤、遠端連線錯誤、發生例外等。
        /// </summary>
        /// <param name="snamespace">命名空間</param>
        /// <param name="exception">發生例外</param>
        public void Error(string snamespace, Exception exception)
        {
            Logger.Error(this.CreateContent(snamespace, exception, false));
        }

        /// <summary>
        /// 錯誤，記錄系統實行所發生的錯誤，例如資料庫錯誤、遠端連線錯誤、發生例外等。
        /// </summary>
        /// <param name="exception">發生例外</param>
        public void Error(Exception exception)
        {
            Logger.Error(this.CreateContent(string.Empty, exception, false));
        }

        /// <summary>
        /// 致命，用來記錄會讓系統無法執行的錯誤，例如資料庫無法連線、重要資料損毀等。
        /// </summary>
        /// <param name="message">訊息內容</param>
        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        /// <summary>
        /// 致命，用來記錄會讓系統無法執行的錯誤，例如資料庫無法連線、重要資料損毀等。
        /// </summary>
        /// <param name="exception">發生例外</param>
        public void Fatal(Exception exception)
        {
            Logger.Fatal(exception);
        }

        /// <summary>
        /// 寄發錯誤訊息
        /// </summary>
        /// <param name="snamespace">命名空間</param>
        /// <param name="exception">發生例外</param>
        public void SendException(string snamespace, Exception exception)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["RecipientAddress"] == null || string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["RecipientAddress"]))
            {
                Logger.Info("RecipientAddress is null");
                return;
            }

            var recipient = System.Configuration.ConfigurationManager.AppSettings["RecipientAddress"];

            var recipientAddress = recipient.Split(',').ToList();

            if (recipientAddress != null && recipientAddress.Count > 0)
            {
                EmailSenderBase mail = new EmailSender(new Log());
                mail.SendAsync(new SendModel()
                {
                    BodyIsHtml = true,
                    Message = CreateContent(snamespace, exception, true),
                    RecipientAddress = recipientAddress,
                    Subject = "Exception"
                });
            }
        }

        /// <summary>
        /// CreateContent
        /// </summary>
        /// <param name="snamespace"></param>
        /// <param name="ex"></param>
        /// <param name="isEmailFormat">是否使用email格式</param>
        /// <returns></returns>
        private string CreateContent(string snamespace, Exception ex, bool isEmailFormat)
        {
            Exception logException = ex;
            string htmlLine = string.Empty;

            if (ex?.InnerException != null)
            {
                logException = ex.InnerException;
            }

            if (isEmailFormat)
            {
                htmlLine = "<br>";
            }

            StringBuilder message = new StringBuilder();
            message.AppendLine();
            message.AppendLine("namespace:" + snamespace + $"{htmlLine}");
            message.AppendLine("例外類型:" + logException.GetType().Name + $"{htmlLine}");
            message.AppendLine("例外訊息:" + logException.Message + $"{htmlLine}");
            message.AppendLine("例外來源:" + logException.Source + $"{htmlLine}");
            message.AppendLine("Stack Trace:" + logException.StackTrace + $"{htmlLine}");
            message.AppendLine("TargetSite:" + logException.TargetSite + $"{htmlLine}");

            return message.ToString();
        }
    }
}
