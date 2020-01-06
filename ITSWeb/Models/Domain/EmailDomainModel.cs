using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace ITSWeb.Models.Domain
{
    /// <summary>
    /// 提供log使用
    /// </summary>
    public class SendModel
    {
        /// <summary>
        /// 發送對象
        /// </summary>
        public List<string> RecipientAddress { get; set; }

        /// <summary>
        /// 副本對象
        /// </summary>
        public List<string> RecipientsOfCc { get; set; }

        /// <summary>
        /// 密件副本對象
        /// </summary>
        public List<string> RecipientsOfBcc { get; set; }

        /// <summary>
        /// 主旨
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 郵件內容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 是否啟用html
        /// </summary>
        public bool BodyIsHtml { get; set; }

        /// <summary>
        /// 優先權(預設Normal)
        /// </summary>
        public MailPriority MailPriority { get; set; } = MailPriority.Normal;

        /// <summary>
        /// 編碼
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 驗證寄件地址對象、寄件副本對象、密件副本對象
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (this.RecipientAddress == null || !this.RecipientAddress.Any())
            {
                if (this.RecipientsOfBcc == null || !this.RecipientsOfBcc.Any())
                {
                    if (this.RecipientsOfCc == null || !this.RecipientsOfCc.Any())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}