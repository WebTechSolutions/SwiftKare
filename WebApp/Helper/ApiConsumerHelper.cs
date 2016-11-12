using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using WebApp.Models;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.Helper
{
    public static class ApiConsumerHelper
    {
        public static string GetResponseString(string requestUriString)
        {
            var baseUri = ApplicationGlobalVariables.Instance.ApiBaseUrl;
            var accessToken = OauthHelper.AccessToken;
            var request = WebRequest.Create(baseUri + requestUriString);
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
        public static string GetResponseString(string requestUriString, bool isBearerRequired)
        {
            var baseUri = ApplicationGlobalVariables.Instance.ApiBaseUrl;
            var accessToken = isBearerRequired ? OauthHelper.AccessToken : "";
            var request = WebRequest.Create(baseUri + requestUriString);
            request.Method = "Get";
            request.ContentType = "application/json";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpWebResponse response;
            var authorizationheader = "";
            if (isBearerRequired)
                authorizationheader = $"Bearer {accessToken}";
            else
                authorizationheader = $"Basic {ApplicationGlobalVariables.Instance.ClientId}:{ApplicationGlobalVariables.Instance.Secret}";

            request.Headers.Add("Authorization", authorizationheader);

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
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }

        public static string PostData(string endPointAddress, string strContent, bool isBearerRequired)
        {
            var accessToken = isBearerRequired ? OauthHelper.AccessToken : "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ApplicationGlobalVariables.Instance.ApiBaseUrl);
                    client.DefaultRequestHeaders.Clear();
                    var authorizationheader = "";

                    if (isBearerRequired)
                        authorizationheader = $"Bearer {accessToken}";
                    else
                        authorizationheader = $"Basic {ApplicationGlobalVariables.Instance.ClientId}:{ApplicationGlobalVariables.Instance.Secret}";

                    client.DefaultRequestHeaders.Add("Authorization", authorizationheader);
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
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
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
