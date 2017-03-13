using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    class ApiConsumerModels
    {
    }
    public class ErrorList
    {
        [JsonProperty("Errors")]
        public List<Error> Errors { get; set; } = new List<Error>();
    }
    public class Error
    {
        [JsonProperty("ErrorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class AccessTokenModel
    {
        int _expiresIn;
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn
        {
            get { return _expiresIn; }
            set
            {
                _expiresIn = value;
                ExpiresOn = DateTime.Now.AddSeconds(value);
            }
        }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonIgnore]
        public DateTime? ExpiresOn { get; set; }

        public bool IsValid()
        {
            if (ExpiresOn.HasValue && DateTime.Now.CompareTo(ExpiresOn) < 0)
                return true;
            return false;
        }
    }

}
