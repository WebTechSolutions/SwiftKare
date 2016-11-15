using Identity.Membership.Models;
//jam
namespace WebApp.Models
{
    public class LoginRegisterViewModel
    {
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
        public bool IsPatient { get; set; }
    }
}
