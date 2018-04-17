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
    public class AppointmentViewController : Controller
    {
        private readonly IAppointmentInterface lIAppointmentRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public AppointmentViewController(OneDirectContext context, ILogger<AppointmentViewController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIAppointmentRepository = new AppointmentRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index(string patId = "")
        {
            try
            {
                List<AppointmentView> pView = null;
                if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                {
                    int res = lIAppointmentRepository.updateAppointmentsStatusforSupport(HttpContext.Session.GetString("UserId"));
                }
                else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                {
                    int res = lIAppointmentRepository.updateAppointmentsStatusforTherapist(HttpContext.Session.GetString("UserId"));
                }
                if (!string.IsNullOrEmpty(patId))
                {
                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                    {
                        ViewBag.Patients = lIAppointmentRepository.getAppointmentPatientListBySupport(HttpContext.Session.GetString("UserId"));
                        pView = lIAppointmentRepository.getSupportAppointmentListByPatientId(patId, AppointmentTypeConstants.Support.ToString(), HttpContext.Session.GetString("UserId"));
                    }
                    else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                    {
                        ViewBag.Patients = lIAppointmentRepository.getAppointmentPatientListByTherapist(HttpContext.Session.GetString("UserId"));
                        pView = lIAppointmentRepository.getTherapistAppointmentListByPatientId(patId, AppointmentTypeConstants.Therapist.ToString(), HttpContext.Session.GetString("UserId"));
                    }
                    else
                    {
                        ViewBag.Patients = lIAppointmentRepository.getAppointmentPatientList();
                        pView = lIAppointmentRepository.getAppointmentListByPatientId(patId);
                    }


                    User lpat = lIUserRepository.getUser(patId);
                    if (lpat != null)
                    {
                        ViewBag.SelectedPatient = lpat.Name;
                    }
                    else
                    {
                        ViewBag.SelectedPatient = patId;
                    }
                }
                else
                {

                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                    {
                        pView = lIAppointmentRepository.getAppointmentListBySupport(HttpContext.Session.GetString("UserId"));
                        ViewBag.Patients = lIAppointmentRepository.getAppointmentPatientListBySupport(HttpContext.Session.GetString("UserId"));
                    }
                    else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                    {
                        pView = lIAppointmentRepository.getAppointmentListByTherapsitId(HttpContext.Session.GetString("UserId"));
                        ViewBag.Patients = lIAppointmentRepository.getAppointmentPatientListByTherapist(HttpContext.Session.GetString("UserId"));
                    }
                    else
                    {
                        ViewBag.Patients = lIAppointmentRepository.getAppointmentPatientList();
                        pView = lIAppointmentRepository.getAppointmentList();
                    }
                    ViewBag.SelectedPatient = "All";
                }
                User luser = lIUserRepository.getUser(HttpContext.Session.GetString("UserId"));
                if (luser != null)
                {
                    ViewBag.TherapistName = luser.Name;
                }

                return View(pView);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }

        public IActionResult Assign(string appointmentId, string appointmentDate = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(appointmentId))
                {
                    AppointmentView pView = lIAppointmentRepository.GetAppointmentViewbyID(appointmentId);
                    if (pView != null)
                    {
                        var slotlist = new List<CheckModel>
                            {
                                 new CheckModel{Id = 0, Name = "12 AM - 1 AM", Checked = false},
                                 new CheckModel{Id = 1, Name = "1 AM - 2 AM", Checked = false},
                                 new CheckModel{Id = 2, Name = "2 AM - 3 AM", Checked = false},
                                 new CheckModel{Id = 3, Name = "3 AM - 4 AM", Checked = false},
                                 new CheckModel{Id = 4, Name = "4 AM - 5 AM", Checked = false},
                                 new CheckModel{Id = 5, Name = "5 AM - 6 AM", Checked = false},
                                 new CheckModel{Id = 6, Name = "6 AM - 7 AM", Checked = false},
                                 new CheckModel{Id = 7, Name = "7 AM - 8 AM", Checked = false},
                                 new CheckModel{Id = 8, Name = "8 AM - 9 AM", Checked = false},
                                 new CheckModel{Id = 9, Name = "9 AM - 10 AM", Checked = false},
                                 new CheckModel{Id = 10, Name = "10 AM - 11 AM", Checked = false},
                                 new CheckModel{Id = 11, Name = "11 AM - 12 PM", Checked = false},
                                 new CheckModel{Id = 12, Name = "12 PM - 1 PM", Checked = false},
                                 new CheckModel{Id = 13, Name = "1 PM - 2 PM", Checked = false},
                                 new CheckModel{Id = 14, Name = "2 PM - 3 PM", Checked = false},
                                 new CheckModel{Id = 15, Name = "3 PM - 4 PM", Checked = false},
                                 new CheckModel{Id = 16, Name = "4 PM - 5 PM", Checked = false},
                                 new CheckModel{Id = 17, Name = "5 PM - 6 PM", Checked = false},
                                 new CheckModel{Id = 18, Name = "6 PM - 7 PM", Checked = false},
                                 new CheckModel{Id = 19, Name = "7 PM - 8 PM", Checked = false},
                                 new CheckModel{Id = 20, Name = "8 PM - 9 PM", Checked = false},
                                 new CheckModel{Id = 21, Name = "9 PM - 10 PM", Checked = false},
                                 new CheckModel{Id = 22, Name = "10 PM - 11 PM", Checked = false},
                                 new CheckModel{Id = 23, Name = "11 PM - 12 AM", Checked = false}

                        };
                        pView.AppointmentSlots = slotlist;

                        if (!string.IsNullOrEmpty(pView.AvailableSlots))
                        {
                            List<Slots> AssignedSlots = JsonConvert.DeserializeObject<List<Slots>>(pView.AvailableSlots);

                            // Slots res = ConvertSlotToPatientTimeZone(AssignedSlots[0], pView.Timezone);
                            ViewBag.AssignedSlots = AssignedSlots;
                            if (AssignedSlots.Count > 0 && !string.IsNullOrEmpty(appointmentDate) && AssignedSlots.Where(m => m.SlotDate == appointmentDate).Count() > 0)
                            {
                                ViewBag.action = "edit";
                                pView.AppointmentDate = appointmentDate;
                                Slots editSlot = AssignedSlots.Where(m => m.SlotDate == appointmentDate).FirstOrDefault();
                                if (editSlot != null && editSlot.SlotTimes != null)
                                {

                                    pView.AppointmentSlots = pView.AppointmentSlots.Concat(editSlot.SlotTimes)
                                    .ToLookup(p => p.Id)
                                    .Select(g => g.Aggregate((p1, p2) => new CheckModel
                                    {
                                        Name = p1.Name,
                                        Id = p1.Id,
                                        Checked = p2.Checked
                                    })).ToList();

                                    //List<CheckModel> editSlotTimes = editSlot.SlotTimes;
                                    //for (int i = 0; i < editSlotTimes.Count; i++)
                                    //{
                                    //    for (Json = 0;)
                                    //}
                                }
                            }

                        }

                        return View(pView);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Assign(AppointmentView appointment)
        {
            try
            {
                Appointments lappointment = lIAppointmentRepository.GetAppointmentbyID(appointment.AppointmentId);
                if (lappointment != null && appointment.AppointmentSlots.Where(m => m.Checked == true).Count() > 0)
                {
                    List<Slots> slotDetails = new List<Slots>();
                    if (!string.IsNullOrEmpty(lappointment.AvailableSlots))
                    {
                        slotDetails = JsonConvert.DeserializeObject<List<Slots>>(lappointment.AvailableSlots);
                        if (slotDetails.Count > 0 && slotDetails.Where(m => m.SlotDate == appointment.AppointmentDate).Count() > 0)
                        {
                            Slots sremove = slotDetails.Where(m => m.SlotDate == appointment.AppointmentDate).FirstOrDefault();
                            slotDetails.Remove(sremove);

                            //TempData["msg"] = "<script>Helpers.ShowMessage('Slot already allocated to the selected date', 1);</script>";

                            //return RedirectToAction("Assign", new { appointmentId = lappointment.AppointmentId });
                        }
                    }

                    Slots s = new Slots();
                    s.AppointmentId = appointment.AppointmentId;
                    s.SlotDate = appointment.AppointmentDate;


                    List<string> slots = new List<string>();
                    if (lappointment.AppointmentType == AppointmentTypeConstants.Therapist.ToString())
                    {
                        slots = lIAppointmentRepository.GetAssignedSlotbyTherapistId(lappointment.TherapistUserId, appointment.AppointmentDate);
                    }
                    else if (lappointment.AppointmentType == AppointmentTypeConstants.Support.ToString())
                    {
                        slots = lIAppointmentRepository.GetAssignedSlotbySupportId(lappointment.SupportUserId, appointment.AppointmentDate);
                    }
                    if (slots.Count > 0)
                        s.SlotTimes = appointment.AppointmentSlots.Where(m => m.Checked == true).ToList().Where(p => !slots.Any(p2 => p2 == p.Id.ToString())).ToList();
                    else
                        s.SlotTimes = appointment.AppointmentSlots.Where(m => m.Checked == true).ToList();

                    slotDetails.Add(s);

                    string sslot = JsonConvert.SerializeObject(slotDetails);
                    lappointment.AvailableSlots = sslot;
                    lappointment.UpdatedDate = DateTime.UtcNow;
                    lappointment.Status = AppointmentConstants.SlotsReceived;
                    if (lappointment.AppointmentType == AppointmentTypeConstants.Support.ToString())
                    {
                        lappointment.SupportUserId = HttpContext.Session.GetString("UserId");
                    }
                    lIAppointmentRepository.UpdateAppointment(lappointment);
                }
                return RedirectToAction("Assign", new { appointmentId = appointment.AppointmentId });
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return RedirectToAction("Index");
            }
        }

        public Slots ConvertSlotToPatientTimeZone(Slots s, string timezone)
        {
            Slots result = new Slots();
            List<CheckModel> rSlotTime = new List<CheckModel>();
            var patientoffset = Utilities.convert(Convert.ToDouble(timezone) / 60);
            var localoffset = Utilities.convert(5.5);
            DateTime pdateTime = Convert.ToDateTime(s.SlotDate);
            if (s.SlotTimes != null && s.SlotTimes.Count > 0)
            {
                foreach (CheckModel c in s.SlotTimes)
                {
                    DateTime d = pdateTime.AddHours(c.Id);
                    string localUtcTime = Utilities.GetUTCDateTime(d, localoffset);
                    string patientDateTime = Utilities.GetLocalDateTime(Convert.ToDateTime(localUtcTime), patientoffset);
                    if (!string.IsNullOrEmpty(patientDateTime))
                    {
                        DateTime p = Convert.ToDateTime(patientDateTime);

                        CheckModel mcheck = new CheckModel();
                        mcheck.Id = p.Hour;
                        mcheck.Name = Utilities.GetTimeSlot(mcheck.Id.ToString());
                        rSlotTime.Add(mcheck);
                    }

                }
                result.SlotTimes = rSlotTime;
                result.SlotDate = s.SlotDate;
                result.AppointmentId = s.AppointmentId;

            }
            return result;
        }

        public IActionResult GenerateURI(string appointmentId)
        {
            try
            {
                Appointments lappointment = lIAppointmentRepository.GetAppointmentbyID(appointmentId);
                if (lappointment != null && lappointment.ConfirmedSlot.HasValue)
                {
                    User pPatient = lIUserRepository.getUser(lappointment.PatientUserId);
                    User pTherapistorSupport = lIUserRepository.getUser(lappointment.TherapistUserId);
                    if (pPatient != null && !string.IsNullOrEmpty(pPatient.Vseeid) && pTherapistorSupport != null && !string.IsNullOrEmpty(pTherapistorSupport.Vseeid))
                    {
                        VSeeHelper vsee = new VSeeHelper();
                        dynamic resURI = vsee.GetURI(pTherapistorSupport.Vseeid, pTherapistorSupport.Password, pPatient.Vseeid);
                        if (resURI != null)
                        {
                            lappointment.Urikey = resURI;
                            int res = lIAppointmentRepository.UpdateAppointment(lappointment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Generate URI Error: " + ex);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string appointmentId, string patId = "")
        {
            try
            {
                Appointments lappointment = lIAppointmentRepository.GetAppointmentbyID(appointmentId);
                if (lappointment != null)
                {
                    string res = lIAppointmentRepository.DeleteAppointment(appointmentId);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Generate URI Error: " + ex);
            }
            if (!string.IsNullOrEmpty(patId))
            {
                return RedirectToAction("Index", new { patId = patId });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public JsonResult updateExpiryStatus(string appointmentDate)
        {
            try
            {

                if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                {
                    int res = lIAppointmentRepository.updateAppointmentsStatusforSupport(HttpContext.Session.GetString("UserId"));
                }
                else
                {
                    int res = lIAppointmentRepository.updateAppointmentsStatusforTherapist(HttpContext.Session.GetString("UserId"));
                }
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

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

                    return Json(new { slots = slots });

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

    }
}




