using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using ITSWeb.Models.Identity;

namespace ITSWeb
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                if (message != null)
                {
                    var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                    var settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");

                    if (settings != null)
                    {
                        WebMail.SmtpPort = settings.Smtp.Network.Port;
                        WebMail.SmtpServer = settings.Smtp.Network.Host;
                        WebMail.UserName = settings.Smtp.Network.UserName;
                        WebMail.Password = settings.Smtp.Network.Password;
                        WebMail.EnableSsl = true;
                        WebMail.From = settings.Smtp.From;
                    }
                    WebMail.Send(message.Destination, message.Subject, message.Body);

                }
            }
            catch (Exception ex)
            {
                //Logger.Error(Log.GetExceptionDetails("IdentityConfig.SendAsync()", ex));
            }
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // 將您的 SMS 服務外掛到這裡以傳送簡訊。
            return Task.FromResult(0);
        }
    }

    // 設定此應用程式中使用的應用程式使用者管理員。UserManager 在 ASP.NET Identity 中定義且由應用程式中使用。
    public class ApplicationUserManager : UserManager<ApplicationUser, Guid>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, Guid> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, Guid, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<ApplicationDbContext>()));
            // 設定使用者名稱的驗證邏輯
            manager.UserValidator = new UserValidator<ApplicationUser, Guid>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // 設定密碼的驗證邏輯
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // 設定使用者鎖定詳細資料
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // 註冊雙因素驗證提供者。此應用程式使用手機和電子郵件接收驗證碼以驗證使用者
            // 您可以撰寫專屬提供者，並將它外掛到這裡。
            manager.RegisterTwoFactorProvider("電話代碼", new PhoneNumberTokenProvider<ApplicationUser, Guid>
            {
                MessageFormat = "您的安全碼為 {0}"
            });
            manager.RegisterTwoFactorProvider("電子郵件代碼", new EmailTokenProvider<ApplicationUser, Guid>
            {
                Subject = "安全碼",
                BodyFormat = "您的安全碼為 {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // 設定在此應用程式中使用的應用程式登入管理員。
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, Guid>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole, Guid>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, Guid> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole, Guid, ApplicationUserRole>(context.Get<ApplicationDbContext>()));
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    /// <summary>
    /// 資料庫自動整合及建立初始帳戶及角色
    /// </summary>
    internal sealed class ApplicationDbMigrationsCongiguration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public ApplicationDbMigrationsCongiguration()
        {
            AutomaticMigrationsEnabled = true;

            // 開啟下列項目在資料庫整合時可能會導致資料遺失，必須注意。(預設false)
            // 不開的話當資料庫整合時程式偵測到可能的資料遺失情況就會產生Exception。
            // 所以建議是，不要一直改帳戶ApplicationUser及角色ApplicationRole的Model。
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            string name = string.Empty;
            string pwd = "1qaz@WSX";
            string email = string.Empty;
            string roleName = string.Empty;//角色名稱(英文)
            string roleDescription = string.Empty;//角色說明
            string roleChtName = string.Empty;//角色名稱(中文)
            IList<string> rolesForUser = null;
            ApplicationUser user = null;
            ApplicationRole role = null;
            if (HttpContext.Current == null) return;
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            #region Create Role Admin if it does not exist
            roleName = "Admin";
            roleChtName = "系統管理者";
            role = roleManager.FindByName(roleName);
            roleDescription = "系統管理者";

            if (role == null)
            {
                role = new ApplicationRole(roleName, roleDescription, roleChtName);
                roleManager.Create(role);
            }
            #endregion

            #region Create Role User if it does not exist
            roleName = "User";
            roleChtName = "一般使用者";
            role = roleManager.FindByName(roleName);
            roleDescription = "一般使用者";
            if (role == null)
            {
                role = new ApplicationRole(roleName, roleDescription, roleChtName);
                roleManager.Create(role);
            }
            #endregion

            #region Add user Admin

            name = "admin";
            email = "admin@Kingwaytek.com";
            user = userManager.FindByName(name);

            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = email };
                var result = userManager.Create(user, pwd);
                if (result.Succeeded)
                {
                    userManager.SetLockoutEnabled(user.Id, false);
                }
            }

            rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains("Admin"))
            {
                role = roleManager.FindByName("Admin");

                if (role != null)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }

            }
            #endregion

            #region Add user User 

            name = "user";
            email = "user@Kingwaytek.com";

            user = userManager.FindByName(name);

            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = email };
                var result = userManager.Create(user, pwd);
                if (result.Succeeded)
                {
                    userManager.SetLockoutEnabled(user.Id, false);
                }
            }

            rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains("User"))
            {
                role = roleManager.FindByName("User");

                if (role != null)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
            }
            #endregion

        }

    }
}
