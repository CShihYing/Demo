using System.Security.Principal;

namespace ITSWeb.Interface
{
    /// <summary>
    /// 取得當前使用者資訊
    /// </summary>
    public interface ICurrentUserSerivce
    {
        IPrincipal CurrentUser { get; set; }
    }
}