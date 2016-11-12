using System.Collections.Generic;

namespace DataAccess.CustomModels
{
    public class UserModel
    {
        public long Id { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Errors { get; set; }
    }
}
