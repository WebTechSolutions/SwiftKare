using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.CustomModels
{
    public class NewsVM
    {
        public long newsID { get; set; }

        public byte[] newsThumbnail { get; set; }
        public byte[] newsImage { get; set; }

        public string newsTitle { get; set; }
        public string newsDetail { get; set; }

        public DateTime? createdDate { get; set; }

        //Custom Properties
        public bool hasThumbnail
        {
            get
            {
                return newsThumbnail != null && newsThumbnail.Count() > 0;
            }
        }
        public bool hasImage
        {
            get
            {
                return newsImage != null && newsImage.Count() > 0;
            }
        }
        public string newsThumbnailBase64
        {
            get
            {
                try
                {
                    return Convert.ToBase64String(newsThumbnail);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string newsImageBase64
        {
            get
            {
                try
                {
                    return Convert.ToBase64String(newsImage);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

    }
}
