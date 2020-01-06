namespace DataAccess.SampleDataBase
{
    public class SampleAspNetUsers
    {
        public System.Guid Id { get; set; }
        public string NickName { get; set; }
        public string JobTitle { get; set; }
        public string StaffNo { get; set; }
        public System.DateTime LastModifyTime { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
    }
}
