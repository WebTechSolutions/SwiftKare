using DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace SwiftKare.Models.Utilities
{
    public class Utility
    {
        SwiftKareDBEntities db = new SwiftKareDBEntities();
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }

        public byte[] GetImageFromDataBase(int Id,string type)
        {
            byte[] cover=null;
            if(type=="thumbnail")
            {
                var img = from temp in db.News where temp.newsID == Id select temp.newsThumbnail;
                cover = img.First();
            }
            else if (type=="detailimage")
            {
                var img = from temp in db.News where temp.newsID == Id select temp.newsImage;
                cover = img.First();
            }
            return cover;
        }
    }
}