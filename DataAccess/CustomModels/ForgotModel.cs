namespace DataAccess.CustomModels
{
    public class ForgotModel
    {
        public string Email { get; set; }
        public string SecretQuestion { get; set; }
        public string SecretAnswer { get; set; }
    }
}
