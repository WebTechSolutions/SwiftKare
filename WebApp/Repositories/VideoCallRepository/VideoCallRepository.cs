using System;
using System.Linq;
using DataAccess;
using WebApp.Interfaces;
using Newtonsoft.Json;
using WebApp.Helper;
using Identity.Membership.Models;
using WebApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using DataAccess.CustomModels;

namespace WebApp.Repositories.VideoCallRepository
{
    public class VideoCallRepository
    {

        /// <summary>
        /// Patient calls doctor
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="patientEmail"></param>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public ApiResultModel PatientCallDoctor(long patientId, string patientEmail, long doctorId)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = patientEmail,
                message = string.Format("Patient {0} called doctor", patientEmail)
            });
        }


        /// <summary>
        /// Doctor accepts call
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="doctorId"></param>
        /// <param name="doctorEmail"></param>
        /// <returns></returns>
        public ApiResultModel DoctorAcceptsCall(long patientId, long doctorId, string doctorEmail)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = doctorEmail,
                message = string.Format("Doctor {0} accepted call", doctorEmail)
            });
        }

        /// <summary>
        /// Doctor rejects call
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="doctorId"></param>
        /// <param name="doctorEmail"></param>
        /// <returns></returns>
        public ApiResultModel DoctorRejectsCall(long patientId, long doctorId, string doctorEmail)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = doctorEmail,
                message = string.Format("Doctor {0} rejected call", doctorEmail)
            });
        }

        /// <summary>
        /// Adds live request logs into database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ApiResultModel AddLiveReqLog(LiveReqLogModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addLiveReqLog", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public ApiResultModel CreateConsult(CreateConsultModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/createConsult", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public ApiResultModel CreateConsultWithoutAppointment(CreateConsultModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/createConsultWithoutAppointment", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        /// <summary>
        /// Call when consultation starts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResultModel AddConsultStartTime(AddConsultTimeModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addConsultStartTime", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        /// <summary>
        /// Call when consulation ends
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResultModel AddConsultEndTime(AddConsultTimeModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addConsultEndTime", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        /// <summary>
        /// Add Doctor Notes
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResultModel AddDoctorNotes(DoctorNotes model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addDoctorNotes", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }



        /// <summary>
        /// Add consult ROS
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResultModel AddConsultROS(ConsultROSModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addconsultROS", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel RemoveConsultROS(ConsultROSModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/removeconsultROS", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        /// <summary>
        /// Add chat messages
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResultModel AddChatMessages(ChatMessageModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addChatMessages", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        /// <summary>
        /// Add VC log
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResultModel AddVCLog(VCLogModel model)
        {
            var strContent = JsonConvert.SerializeObject(model);
            var request = ApiConsumerHelper.PostData("api/addVCLog", strContent);

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }


        public ApiResultModel AddDoctorNotesSubjective(long consultId, string subjective)
        {
            var request = ApiConsumerHelper.PostData("api/addDoctorNotesSubjective?consultId=" + consultId + "&subjective=" + subjective, "");

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel AddDoctorNotesObjective(long consultId, string objective)
        {
            var request = ApiConsumerHelper.PostData("api/AddDoctorNotesObjective?consultId=" + consultId + "&objective=" + objective, "");

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel AddDoctorNotesAssessment(long consultId, string assessment)
        {
            var request = ApiConsumerHelper.PostData("api/addDoctorNotesAssessment?consultId=" + consultId + "&assessment=" + assessment, "");

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public ApiResultModel AddDoctorNotesPlans(long consultId, string plans)
        {
            var request = ApiConsumerHelper.PostData("api/addDoctorNotesPlans?consultId=" + consultId + "&plans=" + plans, "");

            var result = JsonConvert.DeserializeObject<ApiResultModel>(request);
            return result;
        }

        public List<ROSItem> GetROSItems()
        {
            try
            {
                var request = ApiConsumerHelper.GetResponseString("api/getROSItems");
                var result = JsonConvert.DeserializeObject<List<ROSItem>>(request);
                return result;
            }
            catch (Exception ex)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                httpResponseMessage.Content = new StringContent(ex.Message);
                throw new HttpResponseException(httpResponseMessage);
            }
        }


    }
}