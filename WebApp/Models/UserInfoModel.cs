namespace WebApp.Models
{
    public class UserInfoModel
    {
        public long Id { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Email { get; set; } = "";
        public string title { get; set; } = "";
        public string timeZoneOffset { get; set; } = "";

        public string userId { get; set; } = "";
        public string role { get; set; } = "";
        public string iOSToken { get; set; } = "";
        public string AndroidToken { get; set; } = "";

    }
    

}
