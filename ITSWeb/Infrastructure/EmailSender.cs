using ITSWeb.Interface;
using ITSWeb.Models.Domain;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ITSWeb.Infrastructure
{
    /// <summary>
    /// 寄件通知
    /// </summary>
    public sealed class EmailSender : EmailSenderBase
    {
        /// <summary>
        /// config file
        /// </summary>
        private Configuration _config;

        /// <summary>
        /// 郵件設定
        /// </summary>
        private MailConfigModel _configModel;

        /// <summary>
        /// MailSettingsSectionGroup
        /// </summary>
        private MailSettingsSectionGroup _mailSettings;

        /// <summary>
        /// 初始化，取得郵件設定
        /// </summary>
        /// <param name="log">Log</param>
        public EmailSender(ILog log) : base(log)
        {
            if (log == null)
            {
                throw new AggregateException("ILog is null");
            }

            this.Inital();
        }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Gmail寄發通知信
        /// </summary>
        /// <param name="model">SendModel</param>
        /// <returns></returns>
        public override Task<bool> SendAsync(SendModel model)
        {
            bool result = false;
            MailMessage emailMessage = null;
            SmtpClient smtpClient = null;

            try
            {
                if (model == null || model.Validate() == false)
                {
                    return Task.FromResult(false);
                }

                if (this._mailSettings == null)
                {
                    return Task.FromResult(false);
                }

                this._configModel.HostServer = this._mailSettings.Smtp.Network.Host;
                this._configModel.UserName = this._mailSettings.Smtp.Network.UserName;
                this._configModel.Password = this._mailSettings.Smtp.Network.Password;
                this._configModel.From = this._mailSettings.Smtp.From;

                try
                {
                    this._configModel.Port = this._mailSettings.Smtp.Network.Port;
                }
                catch (Exception ex)
                {
                    this.Log.Error("BusinessLogic.Utilities.Mail.SendAsync()", ex);
                    return Task.FromResult(false);
                }

                emailMessage = new MailMessage
                {
                    IsBodyHtml = model.BodyIsHtml,
                    From = new MailAddress(this._configModel.From),
                    Subject = model.Subject,
                    Body = model.Message,
                    Priority = model.MailPriority,
                    BodyEncoding = model.Encoding
                };

                // 加入收件人
                foreach (var item in model.RecipientAddress)
                {
                    if (item != null)
                    {
                        emailMessage.To.Add(item);
                    }
                }

                // 加入副本收件人
                if (model.RecipientsOfCc != null && model.RecipientsOfCc.Count > 0)
                {
                    model.RecipientsOfCc.ForEach(delegate (string item)
                    {
                        emailMessage.CC.Add(item);
                    });
                }

                // 加入密件收件人
                if (model.RecipientsOfBcc != null && model.RecipientsOfBcc.Count > 0)
                {
                    model.RecipientsOfBcc.ForEach(delegate (string item)
                    {
                        emailMessage.Bcc.Add(item);
                    });
                }

                smtpClient = new SmtpClient(_configModel.HostServer, _configModel.Port)
                {
                    Credentials = new System.Net.NetworkCredential(_configModel.UserName, _configModel.Password),
                    EnableSsl = this._mailSettings.Smtp.Network.EnableSsl
                };

                result = true;

                try
                {
                    smtpClient.Send(emailMessage);
                }
                catch (Exception generalException)
                {
                    this.Log.Error("BusinessLogic.Utilities.Mail.SendMessage()", generalException);
                }

            }
            catch (Exception ex)
            {
                result = false;
                this.ErrorMessage = ex.Message;
                base.Log.Error("BusinessLogic.Utilities.Mail.SendMessage()", ex);
            }

            return Task.FromResult(result);
        }

        /// <summary>
        /// 取得郵件設定
        /// </summary>
        private void Inital()
        {
            _configModel = new MailConfigModel();
            var dllPath = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            this._config = ConfigurationManager.OpenExeConfiguration(dllPath);

            if (this._config == null)
            {
                this._config = WebConfigurationManager.OpenWebConfiguration("~/web.config");
            }

            if (this._config == null)
            {
                throw new AggregateException("Configuration file is null");
            }

            this._mailSettings = (MailSettingsSectionGroup)this._config.GetSectionGroup("system.net/mailSettings");

            if (this._mailSettings != null)
            {
                _configModel.HostServer = this._mailSettings.Smtp.Network.Host;
                _configModel.UserName = this._mailSettings.Smtp.Network.UserName;
                _configModel.Password = this._mailSettings.Smtp.Network.Password;
                _configModel.From = this._mailSettings.Smtp.From;

                try
                {
                    _configModel.Port = this._mailSettings.Smtp.Network.Port;
                }
                catch (Exception)
                {
                    _configModel.Port = 0;
                }
            }
        }

        /// <summary>
        /// MailConfig
        /// </summary>
        private class MailConfigModel
        {
            /// <summary>
            /// Who is sending message
            /// </summary>
            public string From { get; set; }

            /// <summary>
            /// SMTP Server for SmtpClient
            /// </summary>
            public string HostServer { get; set; }
            /// <summary>
            /// Password for credentials
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// Specified port e.g. 587 for Comcast
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// User name for credentials
            /// </summary>
            public string UserName { get; set; }
        }
    }
}