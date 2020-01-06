using ITSWeb.Models.Domain;
using System.Threading.Tasks;

namespace ITSWeb.Interface
{
    /// <summary>
    /// 郵件寄送
    /// </summary>
    public abstract class EmailSenderBase
    {
        /// <summary>
        /// 記錄錯誤訊息
        /// </summary>
        protected ILog Log;

        /// <summary>
        /// EmailSender
        /// </summary>
        /// <param name="log">ILog</param>
        protected EmailSenderBase(ILog log)
        {
            this.Log = log;
        }

        /// <summary>
        /// 寄發通知信
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract Task<bool> SendAsync(SendModel model);

    }
}