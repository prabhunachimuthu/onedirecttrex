using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using System.Net;
using System.Data;
using OneDirect.Helper;
using OneDirect.ViewModels;
using Newtonsoft.Json;
using OneDirect.Vsee;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class PatientApiController : Controller
    {
        private readonly IAppointmentInterface lIAppointmentRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientApiController(OneDirectContext context, ILogger<PatientApiController> plogger)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lIUserRepository = new UserRepository(context);
            lIAppointmentRepository = new AppointmentRepository(context);
        }

        [HttpGet]
        [Route("claimpatient")]
        public JsonResult claimpatient(string patientloginid, string surgerydate)
        {
            string _result = IPatient.ClaimPatient(patientloginid, surgerydate);
            if (_result == "success")
            {
                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            else if (_result == "fail")
            {
                return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "No such patient record", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            else
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }
        [HttpGet]
        [Route("createpin")]
        public JsonResult createpin(string patientloginid, string surgerydate, string PIN)
        {
            string _result = IPatient.CreatePIN(patientloginid, surgerydate, PIN);
            if (_result == "success")
            {
                User pUser = lIUserRepository.getUser(patientloginid);
                if (pUser != null)
                {
                    pUser.Password = PIN;
                    lIUserRepository.UpdateUser(pUser);
                }

                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            else if (_result == "fail")
            {
                return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "No such patient record", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            else
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }
        [HttpGet]
        [Route("patientlogin")]
        public JsonResult patientlogin(string patientloginid, string PIN)
        {
            //PatientView _result = IPatient.PatientLoginsReturnPatientView(patientphone, PIN);
            PatientLoginView _result = null;
            if (!string.IsNullOrEmpty(patientloginid))
            {
                _result = IPatient.PatientLoginsReturnPatientLoginViewUsingPatientLoginId(patientloginid.ToLower(), PIN);
            }
            //else
            //{
            //    _result = IPatient.PatientLoginsReturnPatientLoginView(patientphone, PIN);
            //}

            if (_result != null)
            {
                _result.PatientFirstName = _result.PatientFirstName.Split(new char[0]).Length > 0 ? _result.PatientFirstName.Split(new char[0])[0] : _result.PatientFirstName;
                return Json(new { Status = (int)HttpStatusCode.OK, SessionId = _result.LoginSessionId, Patient = _result, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
            }
            else
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }

        [HttpPost]
        [Route("requestappointment")]
        public JsonResult requestappointment([FromBody]Appointments pappointment)
        {
            try
            {
                if (pappointment != null && pappointment.AppointmentType == AppointmentTypeConstants.Therapist.ToString())
                {
                    Patient lpatient = IPatient.GetPaitentbyTherapistIDandPatientLoginId(pappointment.PatientUserId, pappointment.TherapistUserId);
                    Appointments lappointment = lIAppointmentRepository.GetTherapistOpenAppointmentForPatient(pappointment.PatientUserId, pappointment.TherapistUserId);
                    if (lpatient != null)
                    {
                        if (lappointment == null)
                        {
                            pappointment.Status = 0;
                            pappointment.CreateDate = DateTime.UtcNow;
                            pappointment.UpdatedDate = DateTime.UtcNow;
                            pappointment.AppointmentId = Guid.NewGuid().ToString();
                            int _result = lIAppointmentRepository.InsertAppointment(pappointment);
                            if (_result > 0)
                            {
                                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = pappointment, TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is already in open", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "therapist is not assigned to this patient", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else if (pappointment != null && pappointment.AppointmentType == AppointmentTypeConstants.Support.ToString())
                {
                    Appointments lappointment = lIAppointmentRepository.GetSupportOpenAppointmentForPatient(pappointment.PatientUserId);

                    if (lappointment == null)
                    {
                        pappointment.Status = AppointmentConstants.Requested;
                        pappointment.SupportUserId = "";
                        pappointment.TherapistUserId = "";
                        pappointment.CreateDate = DateTime.UtcNow;
                        pappointment.UpdatedDate = DateTime.UtcNow;
                        pappointment.AppointmentId = Guid.NewGuid().ToString();
                        int _result = lIAppointmentRepository.InsertAppointment(pappointment);
                        if (_result > 0)
                        {
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = pappointment, TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is already in open", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        [HttpGet]
        [Route("getappointments")]
        public JsonResult getappointments(string appointmentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(appointmentId))
                {
                    Appointments _result = lIAppointmentRepository.GetAppointmentbyID(appointmentId);
                    if (_result != null && !string.IsNullOrEmpty(_result.AvailableSlots))
                    {
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else if (_result != null && string.IsNullOrEmpty(_result.AvailableSlots))
                    {
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success, slots is not assigned", Appointment = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        [HttpGet]
        [Route("getopenappointments")]
        public JsonResult getopenappointments(string patietnloginid, string PIN)
        {
            try
            {
                if (!string.IsNullOrEmpty(patietnloginid))
                {
                    Patient lpatient = IPatient.GetPatientByPatientLoginId(patietnloginid, PIN);
                    if (lpatient != null)
                    {
                        int result = lIAppointmentRepository.updateAppointmentsStatusforPatient(lpatient.PatientLoginId);
                        List<Appointments> _result = lIAppointmentRepository.GetAppointmentbyPatientId(lpatient.PatientLoginId);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        [HttpGet]
        [Route("getassignedslots")]
        public JsonResult getassignedslots(string appointmentId, string appointmentDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(appointmentId) && !string.IsNullOrEmpty(appointmentDate))
                {
                    Appointments lappointment = lIAppointmentRepository.GetAppointmentbyID(appointmentId);
                    List<string> slots = new List<string>();
                    if (lappointment != null)
                    {
                        if (lappointment.AppointmentType == AppointmentTypeConstants.Therapist.ToString())
                        {
                            slots = lIAppointmentRepository.GetAssignedSlotbyTherapistId(lappointment.TherapistUserId, appointmentDate);
                        }
                        else if (lappointment.AppointmentType == AppointmentTypeConstants.Support.ToString())
                        {
                            slots = lIAppointmentRepository.GetAssignedSlotbySupportId(lappointment.SupportUserId, appointmentDate);
                        }
                    }

                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", slots = slots, TimeZone = DateTime.UtcNow.ToString("s") });

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        [HttpGet]
        [Route("deleteappointment")]
        public JsonResult deleteappointment(string appointmentId)
        {
            try
            {
                if (!string.IsNullOrEmpty(appointmentId))
                {
                    Appointments _result = lIAppointmentRepository.GetAppointmentbyID(appointmentId);
                    if (_result != null && !string.IsNullOrEmpty(_result.AvailableSlots))
                    {
                        string res = lIAppointmentRepository.DeleteAppointment(appointmentId);
                        if (!string.IsNullOrEmpty(res) && res == "success")
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                        else
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "appointment is not deleted", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }
        [HttpPost]
        [Route("confirmappointment")]
        public JsonResult confirmappointment([FromBody]ConfirmAppointment confirm)
        {
            try
            {
                if (confirm != null)
                {
                    Appointments lappointment = lIAppointmentRepository.GetAppointmentbyID(confirm.AppointmentId);
                    if (lappointment != null && !string.IsNullOrEmpty(lappointment.AvailableSlots))
                    {
                        List<Slots> AssignedSlots = JsonConvert.DeserializeObject<List<Slots>>(lappointment.AvailableSlots);
                        string appointmentDate = Convert.ToDateTime(confirm.SlotDate).ToString("dd-MMM-yyyy");
                        if (AssignedSlots.Count > 0 && !string.IsNullOrEmpty(appointmentDate) && AssignedSlots.Where(m => m.SlotDate == appointmentDate).Count() > 0)
                        {
                            Slots lslot = AssignedSlots.Where(m => m.SlotDate == appointmentDate).FirstOrDefault();
                            if (lslot != null && lslot.SlotTimes != null && lslot.SlotTimes.Count > 0 && lslot.SlotTimes.Where(m => m.Id == Convert.ToInt32(confirm.SlotId)).Count() > 0)
                            {
                                Appointments pAppointment = null;
                                if (lappointment.AppointmentType == AppointmentTypeConstants.Therapist.ToString())
                                {
                                    pAppointment = lIAppointmentRepository.GetAppointmentbyTherapistIdandSlot(lappointment.TherapistUserId, appointmentDate, confirm.SlotId);
                                }
                                else if (lappointment.AppointmentType == AppointmentTypeConstants.Support.ToString())
                                {
                                    pAppointment = lIAppointmentRepository.GetAppointmentbySupportIdandSlot(lappointment.SupportUserId, appointmentDate, confirm.SlotId);
                                }
                                if (pAppointment == null)
                                {
                                    User pPatient = lIUserRepository.getUser(lappointment.PatientUserId);
                                    User pTherapistorSupport = lIUserRepository.getUser(lappointment.AppointmentType == AppointmentTypeConstants.Therapist.ToString() ? lappointment.TherapistUserId : lappointment.SupportUserId);
                                    if (pPatient != null && !string.IsNullOrEmpty(pPatient.Vseeid) && pTherapistorSupport != null && !string.IsNullOrEmpty(pTherapistorSupport.Vseeid))
                                    {
                                        VSeeHelper vsee = new VSeeHelper();
                                        dynamic resURI = vsee.GetURI(pTherapistorSupport.Vseeid, pTherapistorSupport.Password, pPatient.Vseeid);
                                        if (resURI != null)
                                        {
                                            lappointment.ConfirmedSlot = Convert.ToDateTime(appointmentDate);
                                            lappointment.SlotId = confirm.SlotId;
                                            lappointment.SlotTime = Utilities.GetTimeSlot(confirm.SlotId);
                                            lappointment.UpdatedDate = DateTime.UtcNow;
                                            lappointment.Status = AppointmentConstants.SlotAccepted;
                                            lappointment.Urikey = resURI;
                                            int res = lIAppointmentRepository.UpdateAppointment(lappointment);
                                            if (res > 0)
                                            {
                                                return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = lappointment, TimeZone = DateTime.UtcNow.ToString("s") });
                                            }
                                            else
                                            {
                                                return Json(new { Status = (int)HttpStatusCode.OK, result = "success, appointment slot is not registerd", TimeZone = DateTime.UtcNow.ToString("s") });
                                            }
                                        }
                                        else
                                        {
                                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success, appointment slot is not registerd", TimeZone = DateTime.UtcNow.ToString("s") });
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success, appointment slot is not registerd", TimeZone = DateTime.UtcNow.ToString("s") });
                                    }

                                }
                                else
                                {
                                    return Json(new { Status = (int)HttpStatusCode.OK, result = "success, slots is already allocated", TimeZone = DateTime.UtcNow.ToString("s") });
                                }
                            }
                            else
                            {
                                return Json(new { Status = (int)HttpStatusCode.OK, result = "success, appointment slot is not registerd", TimeZone = DateTime.UtcNow.ToString("s") });
                            }
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success, appointment date is not registerd", TimeZone = DateTime.UtcNow.ToString("s") });
                        }

                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success, appointment is not initiated", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                    //if (_result != null && !string.IsNullOrEmpty(_result.AvailableSlots))
                    //{
                    //    return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    //}
                    //else if (_result != null && string.IsNullOrEmpty(_result.AvailableSlots))
                    //{
                    //    return Json(new { Status = (int)HttpStatusCode.OK, result = "success, slots is not assigned", Appointment = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    //}
                    //else
                    //{
                    //    return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                    //}
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

        [HttpPost]
        [Route("updateduration")]
        public JsonResult updateduration([FromBody]Appointments pappointment)
        {
            try
            {
                if (pappointment != null && !string.IsNullOrEmpty(pappointment.AppointmentId))
                {
                    Appointments lappointment = lIAppointmentRepository.GetAppointmentbyID(pappointment.AppointmentId);
                    if (lappointment != null && !string.IsNullOrEmpty(lappointment.AvailableSlots))
                    {
                        lappointment.Duration = pappointment.Duration;
                        lappointment.Status = AppointmentConstants.Completed;
                        lappointment.UpdatedDate = DateTime.UtcNow;
                        int _result = lIAppointmentRepository.UpdateAppointment(lappointment);

                        if (_result > 0)
                        {
                            return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = lappointment, TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                        else
                        {
                            return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "duration is not updated", TimeZone = DateTime.UtcNow.ToString("s") });
                        }
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not updated", TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "appointment is not created", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }


        [HttpGet]
        [Route("downloadappointments")]
        public JsonResult downloadappointments(string sessionid)
        {
            try
            {
                if (!string.IsNullOrEmpty(sessionid))
                {
                    Patient lpatient = IPatient.GetPatientBySessionID(sessionid);
                    if (lpatient != null)
                    {
                        List<Appointments> _result = lIAppointmentRepository.GetAllAppointmentbyPatientId(lpatient.PatientLoginId);
                        return Json(new { Status = (int)HttpStatusCode.OK, result = "success", Appointment = _result, TimeZone = DateTime.UtcNow.ToString("s") });
                    }
                    else
                    {
                        return Json(new { Status = (int)HttpStatusCode.BadRequest, result = "patient is not registered", TimeZone = DateTime.UtcNow.ToString("s") });
                    }

                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.Created, result = "request string is not proper", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, result = "Internal server error", TimeZone = DateTime.UtcNow.ToString("s") });
            }
        }

    }

}
