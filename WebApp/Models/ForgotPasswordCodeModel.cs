using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApp.Models
{
    public static class ForgotPasswordCodeModel
    {
        private static string _token;
        public static string Token
        {
            get
            {
                if (HttpContext.Current.Session["ForgotPassword.Token"]!=null)
                {
                    return HttpContext.Current.Session["ForgotPassword.Token"].ToString();
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["ForgotPassword.Token"] = value;
                _token = value;
            }
        }
    }

    public class SecretQuestionModel
    { 
        public string Email { get; set; }
        public string SecretQuestion { get; set; }
        [Required]
        public string SecretAnswer { get; set; }

        public string SecretAnswerHidden { get; set; }
    }

}
