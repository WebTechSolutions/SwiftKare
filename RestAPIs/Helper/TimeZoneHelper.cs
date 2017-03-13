using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPIs.Helper
{
    public class TimeZoneHelper
    {
        private SwiftKareDBEntities db = new SwiftKareDBEntities();
        public DateTime convertTimeZone(DateTime appDateTime, string timezoneid)
        {

            TimeZoneInfo zoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneid);
            DateTime convrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(appDateTime, zoneInfo);
            return convrtedAppDateTime;
        }
        public DateTime? convertTimeZone(DateTime appDateTime,long id)
        {
            var pattz = db.Patients.Where(p => p.patientID == id).Select(p => p.timezone).FirstOrDefault();
            var doctz = db.Doctors.Where(p => p.doctorID == id).Select(p => p.timezone).FirstOrDefault();
            if (pattz != null)
            {
                TimeZoneInfo pzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(pattz.ToString());
                DateTime pconvrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(appDateTime, pzoneInfo);
                return pconvrtedAppDateTime;
            }
            else if (doctz != null)
            {
                TimeZoneInfo dzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(doctz.ToString());
                DateTime dconvrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(appDateTime, dzoneInfo);
                return dconvrtedAppDateTime;
            }
            return null;
       }
    }
}