using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web;
using WebApp.Models;

namespace WebApp.Helper
{
    public static class OauthHelper
    {
        public static string AccessToken => GetOAuthToken();


        private static string GetOAuthToken()
        {
            const string clientKey = "abc";
            const string clientSecret = "ddddddddd";
            string username = SessionHandler.UserName;//"dr@gmail.com";
            var password = SessionHandler.Password;//"Admin@12345";
            var url = Models.ApplicationGlobalVariables.Instance.ApiBaseUrl;


            var session = HttpContext.Current.Session;
            AccessTokenModel token = null;
            if (session["Token"] != null)
            {
                token = session["Token"] as AccessTokenModel;
                if (DateTime.Compare(DateTime.Now, token.ExpiresOn.Value) > 0)
                    token = null;
            }
            if (token != null) return token.AccessToken;

            using (var client = new HttpClient())
            {
                // Get the bearer token.

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Add("Authorization", authorization);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("client_id", clientKey),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("grant_type", "password"),
                });
                //////////////////////////New

                

                var result = client.PostAsync("token", content).Result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    token = JsonConvert.DeserializeObject<AccessTokenModel>(result);
                    session["Token"] = token;
                }
                return token.AccessToken;
                /////////////////
            }
            
        }
        
    }
}
