using System.Security.Principal;
using System.Web;
using ITSWeb.Interface;

namespace ITSWeb.Infrastructure
{
    /// <summary>
    ///取得當前使用者資訊
    /// </summary>
    public class CurrentUserSerivce : ICurrentUserSerivce
    {
        /// <summary>
        /// 取得當前使用者資訊
        /// </summary>
        public IPrincipal CurrentUser
        {
            get { return HttpContext.Current.User; }
            set { HttpContext.Current.User = value; }
        }

    }
}