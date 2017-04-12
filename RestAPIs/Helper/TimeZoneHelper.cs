using DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public DateTime? convertTimeZone(DateTime appDateTime,long? patid,long? docid,int utcflag)
        {
            DateTime? utcdate;
            if (patid!=0)
            {
                var pattz = db.Patients.Where(p => p.patientID == patid).Select(p => p.timezone).FirstOrDefault();
                if (pattz != null)
                {
                    TimeZoneInfo pzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(pattz.ToString());
                    if(utcflag==0)
                    {
                        utcdate = TimeZoneInfo.ConvertTimeToUtc(appDateTime);
                        DateTime pconvrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcdate.Value, pzoneInfo);
                        return pconvrtedAppDateTime;
                    }
                    if (utcflag == 1)
                    {
                        DateTime pconvrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(appDateTime, pzoneInfo);
                        return pconvrtedAppDateTime;
                    }

                }
            }
            
            
            if (docid != 0)
            {
                var doctz = db.Doctors.Where(p => p.doctorID == docid).Select(p => p.timezone).FirstOrDefault();
                if (doctz != null)
                {
                    TimeZoneInfo dzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(doctz.ToString());
                    if (utcflag == 0)
                    {
                        utcdate = TimeZoneInfo.ConvertTimeToUtc(appDateTime);
                        DateTime dconvrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcdate.Value, dzoneInfo);
                        return dconvrtedAppDateTime;
                    }
                    if (utcflag == 1)
                    {
                        DateTime dconvrtedAppDateTime = TimeZoneInfo.ConvertTimeFromUtc(appDateTime, dzoneInfo);
                        return dconvrtedAppDateTime;
                    }
                }
            }
                
            return null;
       }
        public TimeSpan? toUTCTimeSpan(TimeSpan? appTime, long id) //convert user local to UTC timespan
        {
            DateTimeOffset sourceTime, targetTime;
            DateTime mydateTime = DateTime.ParseExact(appTime.Value.ToString(),
                                             "HH:mm:ss", CultureInfo.InvariantCulture);
           
            var pattzo = db.Patients.Where(p => p.patientID == id).Select(p => p.timezoneoffset).FirstOrDefault();
            var doctzo = db.Doctors.Where(p => p.doctorID == id).Select(p => p.timezoneoffset).FirstOrDefault();
            if (pattzo != null)
            {
                var hrs = Convert.ToInt16(pattzo) / 60;
                var mins = Convert.ToInt16(pattzo) % 60;
                sourceTime = new DateTimeOffset(mydateTime, new TimeSpan(-(hrs), mins, 0));
                targetTime = sourceTime.ToOffset(TimeSpan.Zero);
                return targetTime.TimeOfDay;
            }
            else if (doctzo != null)
            {
                var hrs = Convert.ToInt16(doctzo.ToString()) / 60;
                var mins = Convert.ToInt16(doctzo.ToString()) % 60;
                sourceTime = new DateTimeOffset(mydateTime, new TimeSpan(-(hrs), mins, 0));
                targetTime = sourceTime.ToOffset(TimeSpan.Zero);
                return targetTime.TimeOfDay;
            }
            return null;
        }
        public TimeSpan? toUserTimeSpan(TimeSpan? appTime, long id) //convert UTC to user local timespan
        {
            DateTimeOffset sourceTime, targetTime;
            DateTime mydateTime = DateTime.ParseExact(appTime.Value.ToString(),
                                             "HH:mm:ss", CultureInfo.InvariantCulture);
            sourceTime = new DateTimeOffset(mydateTime, new TimeSpan(0,0,0));
            //sourceTime = sourceTime.ToOffset(TimeSpan.Zero);
            //targetTime = sourceTime.ToOffset(new TimeSpan(-8, 0, 0));// Convert to 8 hours behind UTC
            //targetTime = sourceTime.ToOffset(new TimeSpan(3, 0, 0));// Convert to 3 hours ahead of UTC
            var pattzo = db.Patients.Where(p => p.patientID == id).Select(p => p.timezoneoffset).FirstOrDefault();
            var doctzo = db.Doctors.Where(p => p.doctorID == id).Select(p => p.timezoneoffset).FirstOrDefault();
            if (pattzo != null)
            {
                var hrs = Convert.ToInt16(pattzo) / 60;
                var mins = Convert.ToInt16(pattzo) % 60;
                targetTime = sourceTime.ToOffset(new TimeSpan(-(hrs), mins, 0));
                return targetTime.TimeOfDay;
            }
            else if (doctzo != null)
            {
                var hrs = Convert.ToInt16(doctzo.ToString()) / 60;
                var mins = Convert.ToInt16(doctzo.ToString()) % 60;
                targetTime = sourceTime.ToOffset(new TimeSpan(-(hrs), mins, 0));
                return targetTime.TimeOfDay;
            }
            return null;
        }
    }
}