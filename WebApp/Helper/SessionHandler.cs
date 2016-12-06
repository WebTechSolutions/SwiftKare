using System.Web;
using WebApp.Models;
namespace WebApp.Helper
{
    public static class SessionHandler
    {
        public static bool IsExpired
        {
            get
            {

                return HttpContext.Current.Session["username"] == null || string.IsNullOrEmpty(HttpContext.Current.Session["username"].ToString());
            }
            set
            {
                HttpContext.Current.Session["username"] = value;
            }
        }
        public static string UserName
        {
            get
            {

                return HttpContext.Current.Session["username"] != null ? HttpContext.Current.Session["username"].ToString() : "";
            }
            set
            {
                HttpContext.Current.Session["username"] = value;
            }
        }
        public static string Password
        {
            get
            {
                return HttpContext.Current.Session["password"]!=null? HttpContext.Current.Session["password"].ToString():"";
            }
            set
            {
                HttpContext.Current.Session["password"] = value;
            }
        }
        public static string UserId
        {
            get
            {
                return HttpContext.Current.Session["userId"]!=null?HttpContext.Current.Session["userId"].ToString():"";
            }
            set
            {
                HttpContext.Current.Session["userId"] = value;
            }
        }

        public static UserInfoModel UserInfo
        {
            get
            {
                return HttpContext.Current.Session["UserInfo"] != null ? (UserInfoModel)HttpContext.Current.Session["UserInfo"] : new UserInfoModel();
            }
            set
            {
                HttpContext.Current.Session["UserInfo"] = value;
            }
        }
    }

}
