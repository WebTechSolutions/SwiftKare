using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
namespace RestAPIs.Models
{
    public class RequestHandler
    {
        public string UnPackResponse(WebResponse response)
        {
            var dataStream = response.GetResponseStream();
            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);
                return reader.ReadToEnd();
            }
            return "";
        }

        public WebRequest GetRequest(string method, string contentType, string endPoint, string content)
        {
            var request = GetRequest(method, contentType, endPoint);
            var dataArray = Encoding.UTF8.GetBytes(content);
            request.ContentLength = dataArray.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(dataArray, 0, dataArray.Length);
            requestStream.Flush();
            requestStream.Close();
            return request;
        }

        public WebRequest GetRequest(string method, string contentType, string endPoint)
        {
            var request = WebRequest.Create(endPoint);
            request.Method = method;
            request.ContentType = contentType;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            return request;
        }
    }

    public class ApiHelper
    {
        public static object RestApiCaller(ServiceAttributes attributes)
        {
            var objRequest = new RequestHandler();
            var request = objRequest.GetRequest(attributes.MethodType, attributes.ContentType, attributes.ApiUrl);
            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }
            return JsonConvert.DeserializeObject(objRequest.UnPackResponse(response));
        }
    }

    public class ServiceAttributes
    {
        public string MethodType { get; set; }
        public string ContentType { get; set; }
        public string ApiUrl { get; set; }
    }
}