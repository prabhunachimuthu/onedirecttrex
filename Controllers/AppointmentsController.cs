using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OneDirect.Helper;
using OneDirect.Vsee;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class AppointmentsController : Controller
    {
        private readonly INewPatient INewPatient;
        private readonly IPatient IPatient;
        private readonly IAppointmentScheduleInterface lIAppointmentScheduleRepository;
        private readonly IAppointmentInterface lIAppointmentRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public AppointmentsController(OneDirectContext context, ILogger<AppointmentsController> plogger)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            INewPatient = new NewPatientRepository(context);
            lIUserRepository = new UserRepository(context);
            lIAppointmentRepository = new AppointmentRepository(context);
            lIAppointmentScheduleRepository = new AppointmentScheduleRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index(string id = "", string patId = "")
        {
            string timezoneid = TimeZoneInfo.Local.SupportsDaylightSavingTime ? TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;
            //string datetime = Utilities.ConverTimetoBrowserTimeZone(Convert.ToDateTime("2018-04-11 03:00:00"), "330");

            Console.Write("TREX Server TimeZone offset :" + timezoneid);

            Console.Write("TREX Browser TimeZone offset :" + HttpContext.Session.GetString("timezoneid"));
            //var servertimezone = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalMinutes;
            //Console.Write("Server TimeZone Offset :" + servertimezone);
            if (!string.IsNullOrEmpty(patId))
            {
                ViewBag.PatId = patId;
            }
            ViewBag.availability = null;
            ViewBag.appointment = null;
            ViewBag.history = null;
            if (!string.IsNullOrEmpty(id) && id == "Availability")
            {
                List<availabilityView> AvailabilityList = new List<availabilityView>();
                AvailabilityList = lIAppointmentScheduleRepository.GetAvailability(HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));
                ViewBag.availability = AvailabilityList;
                ViewBag.Page = "Availability";
            }
            else if (!string.IsNullOrEmpty(id) && id == "History")
            {
                ViewBag.Page = "History";

                if (!string.IsNullOrEmpty(patId))
                {

                    ViewBag.Patients = lIAppointmentScheduleRepository.getAppointmentPatientListByUserId(HttpContext.Session.GetString("UserId"));

                    ViewBag.History = lIAppointmentScheduleRepository.getAppointmentListByPatientId(Convert.ToInt32(patId), HttpContext.Session.GetString("timezoneid"));


                    Patient lpat = IPatient.GetPatientByPatientID(Convert.ToInt32(patId));
                    if (lpat != null)
                    {
                        ViewBag.SelectedPatient = lpat.PatientName;
                    }
                    else
                    {
                        ViewBag.SelectedPatient = patId;
                    }

                }
                else
                {

                    ViewBag.Patients = lIAppointmentScheduleRepository.getAppointmentPatientListByUserId(HttpContext.Session.GetString("UserId"));

                    ViewBag.History = lIAppointmentScheduleRepository.getAppointmentListByUserId(HttpContext.Session.GetString("UserId"), HttpContext.Session.GetString("timezoneid"));



                    ViewBag.SelectedPatient = "All";
                }
            }
            else
            {
                List<appointmentView> AvailabilityList = new List<appointmentView>();
                AvailabilityList = lIAppointmentScheduleRepository.GetAvailableSlotsForAppointmentCalendar(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), DateTime.Now.Date, DateTime.Now.Date.AddDays(60), HttpContext.Session.GetString("timezoneid"));
                ViewBag.appointment = AvailabilityList;

                //Console.Write("Avaialibility Prabhu :" + JsonConvert.SerializeObject(AvailabilityList));
                ViewBag.Page = "Calendar";
            }

            return View();
        }
        public IActionResult deleteappointment(string appointmentid)
        {
            try
            {
                if (!string.IsNullOrEmpty(appointmentid))
                {
                    lIAppointmentScheduleRepository.DeleteAppointmentSchedule(appointmentid);
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index", "Appointments", new { id = "History" });
        }
        [HttpPost]
        public JsonResult savetimezone(string timezoneoffset = "", string timezoneid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(timezoneoffset))
                {
                    HttpContext.Session.SetString("timezone", timezoneoffset);

                }
                if (!string.IsNullOrEmpty(timezoneid))
                {
                    HttpContext.Session.SetString("timezoneid", timezoneid);
                }
                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }

        [HttpGet]
        [ActionName("generateuri")]
        public JsonResult generateuri(string id = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    AppointmentSchedule lappointment = lIAppointmentScheduleRepository.GetAppointment(id);
                    if (lappointment != null && lappointment.PatientId.HasValue)
                    {
                        Patient lpatient = IPatient.GetPatientByPatientID(lappointment.PatientId.Value);
                        if (lpatient != null)
                        {
                            User pPatient = lIUserRepository.getUser(lpatient.PatientLoginId);
                            User pTherapistorSupport = lIUserRepository.getUser(lappointment.UserId);
                            if (pPatient != null && !string.IsNullOrEmpty(pPatient.Vseeid) && pTherapistorSupport != null && !string.IsNullOrEmpty(pTherapistorSupport.Vseeid))
                            {
                                VSeeHelper vsee = new VSeeHelper();
                                dynamic resURI = vsee.GetURI(pTherapistorSupport.Vseeid, pTherapistorSupport.Password, pPatient.Vseeid);
                                if (resURI != null)
                                {
                                    lappointment.VseeUrl = resURI;
                                    int _result = lIAppointmentScheduleRepository.UpdateAppointment(lappointment);
                                    if (_result > 0)
                                        return Json(new { result = "success", url = resURI });
                                    else
                                        return Json("");
                                }
                                else
                                {
                                    return Json("");
                                }
                            }
                            else
                            {
                                return Json("");
                            }

                        }
                        else
                        {
                            return Json("");
                        }

                    }
                    else
                    {
                        return Json("");
                    }
                }
                else
                {
                    return Json("");
                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }


        [HttpPost]
        public JsonResult refresh(string day = "", string hour = "", string timezoneoffset = "")
        {
            try
            {
                List<appointmentView> appointmentlist = lIAppointmentScheduleRepository.GetAvailableSlotsForAppointmentCalendar(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), DateTime.Now.Date, DateTime.Now.Date.AddDays(60), HttpContext.Session.GetString("timezoneid"));
                if (appointmentlist != null && appointmentlist.Count > 0)
                    return Json(new { result = "success", appointmentlist = appointmentlist });
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult insert(string day = "", string hour = "", string minute = "", string timezoneoffset = "")
        {
            try
            {
                //if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(minute) && !string.IsNullOrEmpty(timezoneoffset))

                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(timezoneoffset))
                {
                    string result = lIAppointmentScheduleRepository.GetAvailability(HttpContext.Session.GetString("UserId"), day, hour, timezoneoffset);
                    if (string.IsNullOrEmpty(result))
                    {
                        string res = lIAppointmentScheduleRepository.InsertAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                        return Json(new { result = res });
                    }
                    else
                    {
                        List<AppointmentSchedule> lappointments = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                        if (lappointments == null || (lappointments != null && lappointments.Count == 0))
                        {
                            string res = lIAppointmentScheduleRepository.RemoveAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                            return Json(new { result = res });
                        }
                        else
                        {
                            return Json(new { result = "failure", appointments = lappointments });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }

        [HttpPost]
        public JsonResult insertschedule(string date = "", string timezoneoffset = "", string color = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(timezoneoffset) && !string.IsNullOrEmpty(color))
                {
                    if (color == "green")
                    {
                        DateTime datevalue = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                        AppointmentSchedule lappointment = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), datevalue);
                        if (lappointment == null)
                        {
                            AppointmentSchedule lbookAppointment = new AppointmentSchedule();
                            lbookAppointment.AppointmentId = Guid.NewGuid().ToString();
                            lbookAppointment.UserType = Utilities.getUserType(HttpContext.Session.GetString("UserType"));
                            lbookAppointment.UserId = HttpContext.Session.GetString("UserId");
                            lbookAppointment.Datetime = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                            //lbookAppointment.PatientId = "";
                            lbookAppointment.SlotStatus = "Blocked";
                            lbookAppointment.CallStatus = "Caller No Show";
                            lbookAppointment.CreateDate = DateTime.UtcNow;
                            lbookAppointment.UpdateDate = DateTime.UtcNow;
                            lbookAppointment.RecordedFile = "";

                            int _result = lIAppointmentScheduleRepository.InsertAppointment(lbookAppointment);

                            return Json(new { result = _result > 0 ? "success" : "" });
                        }
                        else
                        {
                            int _result = lIAppointmentScheduleRepository.DeleteAppointmentSchedule(lappointment.AppointmentId);
                            return Json(new { result = _result > 0 ? "success" : "" });
                        }
                    }
                    else if (color == "white")
                    {
                        AppointmentSchedule lbookAppointment = new AppointmentSchedule();
                        lbookAppointment.AppointmentId = Guid.NewGuid().ToString();
                        lbookAppointment.UserType = Utilities.getUserType(HttpContext.Session.GetString("UserType"));
                        lbookAppointment.UserId = HttpContext.Session.GetString("UserId");
                        lbookAppointment.Datetime = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                        //lbookAppointment.PatientId = "";
                        lbookAppointment.SlotStatus = "Extra";
                        lbookAppointment.CallStatus = "Extra";
                        lbookAppointment.CreateDate = DateTime.UtcNow;
                        lbookAppointment.UpdateDate = DateTime.UtcNow;
                        lbookAppointment.RecordedFile = "";

                        int _result = lIAppointmentScheduleRepository.InsertAppointment(lbookAppointment);

                        return Json(new { result = _result > 0 ? "success" : "" });
                    }
                    else if (color == "grey")
                    {
                        DateTime datevalue = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(Convert.ToDateTime(date), timezoneoffset));
                        AppointmentSchedule lappointment = lIAppointmentScheduleRepository.CheckAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), datevalue);
                        if (lappointment != null)
                        {
                            int _result = lIAppointmentScheduleRepository.DeleteAppointmentSchedule(lappointment.AppointmentId);
                            return Json(new { result = _result > 0 ? "success" : "" });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }




        [HttpPost]
        public JsonResult delete(string day = "", string hour = "", string minute = "", string timezoneoffset = "")
        {
            try
            {
                //if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(minute) && !string.IsNullOrEmpty(timezoneoffset))
                if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(hour) && !string.IsNullOrEmpty(timezoneoffset))
                {
                    string res = lIAppointmentScheduleRepository.UpdateAppointmentSchedule(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                    if (!string.IsNullOrEmpty(res))
                    {
                        string result = lIAppointmentScheduleRepository.RemoveAvailability(HttpContext.Session.GetString("UserId"), Utilities.getUserType(HttpContext.Session.GetString("UserType")), timezoneoffset, day, hour);
                        return Json(new { result = result });
                    }
                    else
                    {
                        return Json(new { result = "" });
                    }

                }
            }
            catch (Exception ex)
            {
                return Json("");
            }
            return Json("");
        }

    }
}




