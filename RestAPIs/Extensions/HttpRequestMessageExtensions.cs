using RestAPIs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace RestAPIs.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        ///     Returns a dictionary of QueryStrings that's easier to work with
        ///     than GetQueryNameValuePairs KevValuePairs collection.
        ///     If you need to pull a few single values use GetQueryString instead.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetQueryStrings(this HttpRequestMessage request)
        {
            return request.GetQueryNameValuePairs()
                .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns an individual querystring value
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetQueryString(this HttpRequestMessage request, string key)
        {
            // IEnumerable<KeyValuePair<string,string>> - right!
            var queryStrings = request.GetQueryNameValuePairs();
            if (queryStrings == null)
                return null;

            var match =
                queryStrings.FirstOrDefault(kv => string.Compare(kv.Key, key, StringComparison.OrdinalIgnoreCase) == 0);
            if (string.IsNullOrEmpty(match.Value))
                return null;

            return match.Value;
        }

        /// <summary>
        ///     Returns an individual HTTP Header value
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetHeader(this HttpRequestMessage request, string key)
        {
            IEnumerable<string> keys;
            if (!request.Headers.TryGetValues(key, out keys))
                return null;

            return keys.First();
        }

        public static bool IsValidClient(this HttpRequestMessage request)
        {
            var headerValue = request.GetHeader("Authorization");
            if (headerValue == null)
                return false;

            if (headerValue.Contains(":"))
            {
                var arr = headerValue.Split(':');
                if (arr.Length > 1)
                {
                    var clientId = arr[0];
                    var clientSecret = arr[1];
                    var objModel = new OauthUserModel();
                    if (!(clientId == $"Basic {objModel.OauthClient}" && clientSecret == objModel.OauthClientSecret))
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
    }



        /// <summary>
        ///     Retrieves an individual cookie from the cookies collection
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static string GetCookie(this HttpRequestMessage request, string cookieName)
        {
            var cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
                return cookie[cookieName].Value;

            return null;
        }
        public static string ToStringBase64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string ToStringBase64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}