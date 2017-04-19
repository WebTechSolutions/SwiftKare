namespace RestAPIs.Models
{
    public class LoginApiModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class PatientLoginApiModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string iOSToken { get; set; }
        public string andriodToken { get; set; }
        public string offset { get; set; }

    }
    public class DoctorLoginApiModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string iOSToken { get; set; }
        public string andriodToken { get; set; }

    }
}
