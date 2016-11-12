using System.Web;

namespace WebApp.Helper
{
    public static class SessionHandler
    {
        private static string username;  
        public static string UserName    
        {
            get
            {
                return HttpContext.Current.Session["username"].ToString();
            }
            set
            {
                HttpContext.Current.Session["username"] = value;
                username = value;

            }
        }


        private static string password;
        public static string Password
        {
            get
            {
                return HttpContext.Current.Session["password"].ToString();
            }
            set
            {
                HttpContext.Current.Session["password"] = value;
                password = value;

            }
        }

        private static string userId;
        public static string UserId
        {
            get
            {
                return HttpContext.Current.Session["userId"].ToString();
            }
            set
            {
                HttpContext.Current.Session["userId"] = value;
                userId = value;

            }
        }


    }
}
