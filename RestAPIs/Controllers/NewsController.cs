using DataAccess;
using DataAccess.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestAPIs.Controllers
{
    [Authorize]
    public class NewsController : ApiController
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        HttpResponseMessage response;

        [Route("api/getNewsList")]
        public HttpResponseMessage GetNewsList()
        {
            try
            {
                List<News> objNewsList = new List<News>();
                var newslist = (from l in db.News
                                  where l.active == true 
                                  orderby l.newsID descending
                                  select new 
                                  {
                                      l.newsID,
                                      l.newsThumbnail,
                                      l.newsTitle,
                                      l.newsDetail,
                                      l.cd
                                  }).ToList();
                int i = 1;
                foreach (var item in newslist)
                {
                    var count = item.newsDetail.Length;
                    
                    string detail = "";
                    if (i <= newslist.Count)
                    {
                        if (count > 250)
                        {

                            detail = item.newsDetail.Substring(0, 250) + "...";
                        }
                        else
                        {
                            detail = item.newsDetail.Substring(0, count);
                        }
                        News objNews = new News(); 
                        objNews.newsID = item.newsID;
                        objNews.newsThumbnail = item.newsThumbnail;
                        objNews.newsTitle = item.newsTitle;
                        objNews.newsDetail = detail;
                        objNews.cd = item.cd;
                        objNewsList.Add(objNews);
                        i++;
                    }
                }   
                
                response = Request.CreateResponse(HttpStatusCode.OK, objNewsList);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetNewsList in NewsController");
            }
        }

        [Route("api/getNewsDetail")]
        public HttpResponseMessage GetNewsDetail(long newsID)
        {
            try
            {
                
                var newsDetail = (from l in db.News
                                where l.active == true
                                orderby l.newsID descending
                                select new
                                {
                                    l.newsID,
                                    l.newsImage,
                                    l.newsTitle,
                                    l.newsDetail,
                                    l.cd
                                }).ToList();
                

                response = Request.CreateResponse(HttpStatusCode.OK, newsDetail);
                return response;
            }
            catch (Exception ex)
            {
                return ThrowError(ex, "GetNewsDetail in NewsController");
            }
        }

        private HttpResponseMessage ThrowError(Exception ex, string Action)
        {
            response = Request.CreateResponse(HttpStatusCode.BadRequest, new ApiResultModel { ID = 0, message = "Following Error occurred at method: " + Action + "\n" + ex.Message });
            return response;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
