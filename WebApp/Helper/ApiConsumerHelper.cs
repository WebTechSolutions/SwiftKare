using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using WebApp.Models;
using System.Net.Http;

namespace WebApp.Helper
{
    public static class ApiConsumerHelper
    {
        public static string GetResponseString(string requestUriString)
        {
            var baseUri = ApplicationGlobalVariables.Instance.ApiBaseUrl;
            var accessToken = OauthHelper.AccessToken;
            var request = WebRequest.Create(baseUri+requestUriString);
            request.Method = "Get";
            request.ContentType = "application/json";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpWebResponse response;
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            var actual = "";
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            actual = response != null ? UnPackResponse(response) : "Status:OK";
            return actual;
        }
        
        public static string PostData(string endPointAddress, string strContent)
        {
            var accessToken = OauthHelper.AccessToken;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApplicationGlobalVariables.Instance.ApiBaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    try
                    {
                        dynamic requestResult = client.PostAsync(endPointAddress, new StringContent(strContent, Encoding.Default, "application/json")).Result.Content.ReadAsStringAsync().Result;
                        return requestResult;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string PutData(string endPointAddress, string strContent)
        {
            var accessToken = OauthHelper.AccessToken;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApplicationGlobalVariables.Instance.ApiBaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    try
                    {
                        dynamic requestResult = client.PutAsync(endPointAddress, new StringContent(strContent, Encoding.Default, "application/json")).Result.Content.ReadAsStringAsync().Result;
                        return requestResult;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string DeleteData(string endPointAddress)
        {
            var accessToken = OauthHelper.AccessToken;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApplicationGlobalVariables.Instance.ApiBaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    try
                    {
                        dynamic requestResult = client.DeleteAsync(endPointAddress).Result.Content.ReadAsStringAsync().Result;
                        return requestResult;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #region Private Memebers
        private static string UnPackResponse(WebResponse response)
        {
            var dataStream = response.GetResponseStream();
            if (dataStream == null) return "";
            var reader = new StreamReader(dataStream);
            return reader.ReadToEnd();
        }
        #endregion
    }
}
