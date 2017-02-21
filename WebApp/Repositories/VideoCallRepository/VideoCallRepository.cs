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

        public ApiResultModel DoctorCallPatient(long doctorId, string doctorEmail, long patientId)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = doctorEmail,
                message = string.Format("Doctor {0} called patient", doctorEmail)
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


        public ApiResultModel PatientAcceptsCall(long patientId, long doctorId, string patientEmail)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = patientEmail,
                message = string.Format("Patient {0} accepted call", patientEmail)
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

        public ApiResultModel PatientRejectsCall(long patientId, long doctorId, string patientEmail)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = patientEmail,
                message = string.Format("Patient {0} rejected call", patientEmail)
            });
        }

        public ApiResultModel ConsultCreatedLog(long patientId, long doctorId, string consultCreatedBy)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = consultCreatedBy,
                message = string.Format("Consult Created By {0} ", consultCreatedBy)
            });
        }

        public ApiResultModel ConsultCreationFailedLog(long patientId, long doctorId, string consultCreatedBy)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = consultCreatedBy,
                message = string.Format("Consult unable to created By {0} ", consultCreatedBy)
            });
        }

        public ApiResultModel AddCallLog(long patientId, long doctorId, string from,string message)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = from,
                message = message
            });
        }

        public ApiResultModel TokboxCreatedLog(long patientId, long doctorId, string tokboxCreatedBy)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = tokboxCreatedBy,
                message = string.Format("Tokbox info Created By {0} ", tokboxCreatedBy)
            });
        }

        public ApiResultModel TokboxFailLog(long patientId, long doctorId, string tokboxCreatedBy)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = tokboxCreatedBy,
                message = string.Format("Tokbox info unable to generated by {0} ", tokboxCreatedBy)
            });
        }

        public ApiResultModel PaymentFailLog(long patientId, long doctorId, string paymentBy)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = paymentBy,
                message = string.Format("Stripe Payment failed  by {0} ", paymentBy)
            });
        }

        public ApiResultModel PaymentSuccessLog(long patientId, long doctorId, string paymentBy)
        {
            return AddLiveReqLog(new LiveReqLogModel
            {
                doctorID = doctorId,
                patientID = patientId,
                From = paymentBy,
                message = string.Format("Stripe Payment successfull by {0} ", paymentBy)
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