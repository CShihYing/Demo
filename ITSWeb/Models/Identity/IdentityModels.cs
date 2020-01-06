using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ITSWeb.Models.Identity
{
    // 您可以在 ApplicationUser 類別新增更多屬性，為使用者新增設定檔資料，請造訪 http://go.microsoft.com/fwlink/?LinkID=317594 以深入了解。
    public sealed class ApplicationUser : IdentityUser<Guid, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser() : base()
        {
            this.Id = Guid.NewGuid();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, Guid> manager)
        {
            // 注意 authenticationType 必須符合 CookieAuthenticationOptions.AuthenticationType 中定義的項目
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在這裡新增自訂使用者宣告
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }
        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            // 第一次執行時會來這做Code First資料庫整合及建立初始的帳戶或角色
            Database.SetInitializer<ApplicationDbContext>(
                new MigrateDatabaseToLatestVersion<ApplicationDbContext, ApplicationDbMigrationsCongiguration>());
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class ApplicationRole : IdentityRole<Guid, ApplicationUserRole>
    {
        public ApplicationRole() : base()
        {
            this.Id = Guid.NewGuid();
        }

        public ApplicationRole(string name, string description, string chtName) : this()
        {
            this.Name = name;
            this.Description = description;
            this.RoleChtName = chtName;
        }

        //  這邊可以自行修改需要的角色屬性
        /// <summary>
        /// 說明
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// 角色中文名稱
        /// </summary>
        public string RoleChtName { get; set; }
    }
    public class ApplicationUserRole : IdentityUserRole<Guid> { }
    public class ApplicationUserClaim : IdentityUserClaim<Guid> { }
    public class ApplicationUserLogin : IdentityUserLogin<Guid> { }

}