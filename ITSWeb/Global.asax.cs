using Elmah;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebGrease.Css.Extensions;

namespace ITSWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// ApplicationStart
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            HtmlHelper.ClientValidationEnabled = true;
            HtmlHelper.UnobtrusiveJavaScriptEnabled = true;
            MvcHandler.DisableMvcResponseHeader = true;
        }


        /// <summary>
        /// Application_PreSendRequestHeaders
        /// </summary>
        /// <param name="source">Object</param>
        /// <param name="e">EventArgs</param>
        protected void Application_PreSendRequestHeaders(Object source, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Headers.Remove("Server");
            Response.Headers.Remove("X-Frame-Options");
        }

        #region   ELMAH_Error_Mail 

        /// <summary>
        /// 過濾錯誤訊息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorLog_Filtering(object sender, Elmah.ExceptionFilterEventArgs e)
        {
            this.FilterError404(e);

            var exception = e.Exception.GetBaseException();

            if (exception is HttpRequestValidationException)
            {
                e.Dismiss();
            }

        }

        /// <summary>
        /// 過濾錯誤訊息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            var exception = e.Exception.GetBaseException();

            FilterError404(e);

            if (exception is System.IO.FileNotFoundException || exception is HttpRequestValidationException || exception is HttpException)
            {
                e.Dismiss();
            }
        }

        /// <summary>
        /// 增加寄件副本人員
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ErrorMail_Mailing(object sender, Elmah.ErrorMailEventArgs e)
        {
            var mailAddress = System.Configuration.ConfigurationManager.AppSettings["RecipientAddress"];
            var eMails = mailAddress.Split(new[] {';', ','}, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrEmpty(mailAddress) && eMails.Length > 0)
            {
                eMails.ForEach(delegate (string mail)
                {
                    e.Mail.CC.Add(mail);
                });
            }
        }

        /// <summary>
        /// 過濾錯誤訊息
        /// </summary>
        /// <param name="e"></param>
        private void FilterError404(ExceptionFilterEventArgs e)
        {
            if (e.Exception.GetBaseException() is HttpException)
            {
                HttpException ex = (HttpException)e.Exception.GetBaseException();

                if (ex.GetHttpCode() == 404)
                {
                    e.Dismiss();
                }
            }
        }

        #endregion
    }
}
