using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using OneDirect.Extensions;
using OneDirect.ViewModels;
using Microsoft.AspNetCore.Http;
using OneDirect.Helper;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class ProviderController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public ProviderController(OneDirectContext context, ILogger<ProviderController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIPatientRxRepository = new PatientRxRepository(context);
            lIUserRepository = new UserRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var response = new Dictionary<string, object>();
            try
            {
                logger.LogDebug("Pain Post Start");
                List<User> pUser = lIUserRepository.getUserListByType(ConstantsVar.Provider);
                ViewBag.userlist = pUser;
                return View(pUser);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }
        public IActionResult Add()
        {
            return View("Add");
        }
        [HttpPost]
        public IActionResult Add(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Provider;
            User _user = UserExtension.UserViewModelToUser(pUser);
            string _User = lIUserRepository.InsertUser(_user);
            return RedirectToAction("Index");
        }
        public IActionResult Profile(string id)
        {
            User pUser = lIUserRepository.getUser(id);
            if (pUser != null)
            {
                UserViewModel _user = UserExtension.UserToUserViewModel(pUser);
                ViewBag.Name = _user.Name;
                return View(_user);
            }
            else
            {
                return View(null);
            }
        }
        [HttpPost]
        public IActionResult Profile(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Provider;
            User _user = UserExtension.UserViewModelToUser(pUser);
            string _result = lIUserRepository.UpdateUser(_user);
            return RedirectToAction("Index");
        }

        public IActionResult Dashboard()
        {
            List<DashboardView> lDashboardView = null;
            string _uType = HttpContext.Session.GetString("UserType");
            if (HttpContext.Session.GetString("UserId") != null && HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Provider.ToString())
            {
                try
                {
                    lDashboardView = lIPatientRxRepository.getDashboard(HttpContext.Session.GetString("UserId"));
                }
                catch (Exception ex)
                {
                    throw;
                }
                return View(lDashboardView);
            }
            else
            {
                //pPatients = lIUserRepository.getUserListByTherapistId(HttpContext.Session.GetString("UserId"));
                //pUser = pUser.GroupBy(u => u.UserId, (key, group) => group.First()).ToList();
                //pUsera = UserExtension.UserToUserViewModelList(pUser);
            }
            return View(null);
        }

        public IActionResult Patient(int id, string Username, string equipmentType, string actuator = "")
        {
            ViewBag.User = Username;
            ViewBag.actuator = actuator;
            //User pUser = lIUserRepository.getUser(id);
            string _uType = HttpContext.Session.GetString("UserType");
            if (_uType == "3" || _uType == "2" || _uType == "1")
            {
                //if (equipmentType.ToLower() == "shoulder")
                {
                    PatientRx patientRx = lIPatientRxRepository.getByRxIDId(id, actuator);
                    ViewBag.PatientRx = patientRx;
                }
                //Usage
                PatientRx lPatientRx = lIPatientRxRepository.getPatientRx(id, equipmentType, actuator);
                UsageViewModel lusage = new UsageViewModel();
                lusage.MaxSessionSuggested = 0;
                lusage.PercentageCompleted = 0;
                lusage.PercentagePending = 100;
                ViewBag.Usage = lusage;
                if (lPatientRx != null)
                {
                    double requiredSession = (((Convert.ToDateTime(lPatientRx.RxEndDate) - Convert.ToDateTime(lPatientRx.RxStartDate)).TotalDays / 7) * (int)lPatientRx.RxDaysPerweek * (int)lPatientRx.RxSessionsPerWeek);
                    int totalSession = lPatientRx.Session != null ? lPatientRx.Session.ToList().Count : 0;
                    lusage.MaxSessionSuggested = (int)requiredSession;
                    lusage.PercentageCompleted = (int)((totalSession / requiredSession) * 100);
                    lusage.PercentagePending = 100 - lusage.PercentageCompleted;
                    ViewBag.Usage = lusage;
                }

                //Pain
                PatientRx lPatientRxPain = lIPatientRxRepository.getPatientRxPain(id, equipmentType, actuator);
                PainViewModel lpain = new PainViewModel();
                lpain.TotalPain = 0;
                lpain.LowPain = 0;
                lpain.MediumPain = 0;
                lpain.HighPain = 100;
                ViewBag.Pain = lpain;
                if (lPatientRxPain != null)
                {
                    List<Session> lSessionList = lPatientRx.Session != null ? lPatientRxPain.Session.ToList() : null;
                    if (lSessionList != null && lSessionList.Count > 0)
                    {
                        lpain.TotalPain = lSessionList.Select(x => x.Pain.Count).Sum();
                        lpain.LowPain = lpain.TotalPain > 0 ? (int)(((double)(lSessionList.Select(x => x.Pain.Where(y => y.PainLevel <= 2).Count()).Sum()) / lpain.TotalPain) * 100) : 0;
                        lpain.MediumPain = lpain.TotalPain > 0 ? (int)(((double)(lSessionList.Select(x => x.Pain.Where(y => y.PainLevel > 2 && y.PainLevel <= 5).Count()).Sum()) / lpain.TotalPain) * 100) : 0;
                        lpain.HighPain = 100 - lpain.MediumPain - lpain.LowPain;
                        ViewBag.Pain = lpain;
                    }
                }


                //ROM
                //if (equipmentType == "Shoulder")
                //{
                //    DashboardView ROM = lIPatientRxRepository.getPatientRxROMShoulder(id, equipmentType, actuator);
                //    if (ROM != null)
                //    {
                //        ViewBag.ROM = ROM;
                //    }
                //}
                //else if (equipmentType == "Ankle")
                //{
                //    DashboardView ROM = lIPatientRxRepository.getPatientRxROMAnkle(id, equipmentType, actuator);
                //    if (ROM != null)
                //    {
                //        ViewBag.ROM = ROM;
                //    }
                //}
                //else
                //{
                //    DashboardView ROM = lIPatientRxRepository.getPatientRxROMKnee(id, equipmentType, actuator);
                //    if (ROM != null)
                //    {
                //        ViewBag.ROM = ROM;
                //    }
                //}

                ROMChartViewModel ROM = lIPatientRxRepository.getPatientRxROMChart(id, equipmentType, actuator);
                if (ROM != null)
                {
                    //var maxValue = ROM.Max(x => x.Flexion);
                    //var incValu = Math.Abs((decimal)maxValue / ROM.Count);
                    //ViewBag.ROMIncrementValue = incValu;
                    ViewBag.ROM = ROM;
                }

                ViewBag.EquipmentType = equipmentType;
                if (equipmentType == "Shoulder")
                {
                    //Equipment ROM
                    ViewBag.EquipmentType = equipmentType;
                    List<ShoulderViewModel> ROMList = lIPatientRxRepository.getPatientRxEquipmentROMForShoulder(id, equipmentType, actuator);
                    if (ROMList != null && ROMList.Count > 0)
                    {
                        ViewBag.FlexionColor = (ROMList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        ViewBag.EquipmentROM = ROMList;
                    }
                    //Compliance
                    List<ShoulderViewModel> ComplianceList = lIPatientRxRepository.getPatientRxComplianceForShoulder(id, equipmentType, actuator);
                    if (ComplianceList != null && ComplianceList.Count > 0)
                    {
                        ViewBag.ComFlexionColor = (ComplianceList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        ViewBag.Compliance = ComplianceList;
                    }
                }
                else
                {
                    //Equipment ROM
                    ViewBag.EquipmentType = equipmentType;
                    List<FlexionViewModel> FlexionList = lIPatientRxRepository.getPatientRxEquipmentROMByFlexion(id, equipmentType, actuator);
                    if (FlexionList != null && FlexionList.Count > 0)
                    {
                        // ViewBag.FlexionColor = (FlexionList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        ViewBag.EquipmentFlexion = FlexionList;
                    }
                    List<ExtensionViewModel> ExtensionList = lIPatientRxRepository.getPatientRxEquipmentROMByExtension(id, equipmentType, actuator);
                    if (ExtensionList != null && ExtensionList.Count > 0)
                    {
                        //   ViewBag.ExtensionColor = (ExtensionList.Where(x => x.Extension > 0).Count() > 0) ? "Blue" : "White";
                        ViewBag.EquipmentExtension = ExtensionList;
                    }
                    //List<ROMViewModel> ROMList = lIPatientRxRepository.getPatientRxEquipmentROMByProtocol(id, equipmentType, actuator, 3);
                    //if (ROMList != null && ROMList.Count > 0)
                    //{
                    //   // ViewBag.FlexionColor = (ROMList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                    //  //  ViewBag.ExtensionColor = (ROMList.Where(x => x.Extension > 0).Count() > 0) ? "Blue" : "White";
                    //    ViewBag.EquipmentROM = ROMList;
                    //}

                    //Compliance

                    List<FlexionViewModel> CFlexionList = lIPatientRxRepository.getPatientRxComplianceByFlexion(id, equipmentType, actuator);
                    if (CFlexionList != null && CFlexionList.Count > 0)
                    {
                        //ViewBag.ComFlexionColor = (ComplianceList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        //ViewBag.ComExtensionColor = (ComplianceList.Where(x => x.Extension > 0).Count() > 0) ? "Red" : "White";
                        ViewBag.FlexionCompliance = CFlexionList;
                    }
                    List<ExtensionViewModel> CExtensionList = lIPatientRxRepository.getPatientRxComplianceByExtension(id, equipmentType, actuator);
                    if (CExtensionList != null && CExtensionList.Count > 0)
                    {
                        //ViewBag.ComFlexionColor = (ComplianceList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                        //ViewBag.ComExtensionColor = (ComplianceList.Where(x => x.Extension > 0).Count() > 0) ? "Red" : "White";
                        ViewBag.ExtensionCompliance = CExtensionList;
                    }
                    //List<ROMViewModel> ComplianceList = lIPatientRxRepository.getPatientRxComplianceByProtocol(id, equipmentType, actuator, 3);
                    //if (ComplianceList != null && ComplianceList.Count > 0)
                    //{
                    //    //ViewBag.ComFlexionColor = (ComplianceList.Where(x => x.Flexion > 0).Count() > 0) ? "Orange" : "White";
                    //    //ViewBag.ComExtensionColor = (ComplianceList.Where(x => x.Extension > 0).Count() > 0) ? "Red" : "White";
                    //    ViewBag.Compliance = ComplianceList;
                    //}

                }


                //Treatment Calendar
                List<TreatmentCalendarViewModel> TreatmentCalendarList = lIPatientRxRepository.getTreatmentCalendar(id, equipmentType, actuator);
                if (TreatmentCalendarList != null && TreatmentCalendarList.Count > 0)
                {
                    ViewBag.TreatmentCalendar = TreatmentCalendarList;
                }

                //Current Sessions
                List<Session> SessionList = lIPatientRxRepository.getCurrentSessions(id, equipmentType, actuator);
                if (SessionList != null && SessionList.Count > 0)
                {
                    ViewBag.SessionList = SessionList;
                }

                //if (actuator == 2)
                //{
                //    ViewBag.Flexion = "Abduction";
                //    ViewBag.Extension = "Extension";
                //}
                //else
                if (actuator == "Forward Flexion" || actuator == "External Rotation")
                {
                    ViewBag.Extension = "External Rotation";
                    ViewBag.Flexion = "Flexion";
                }
                else
                {
                    ViewBag.Flexion = "Flexion";
                    ViewBag.Extension = "Extension";
                }
            }

            return View();

        }

        public IActionResult Delete(int patid)
        {
            try
            {
                string result = lIPatientRxRepository.DeletePatientRecordsWithCasecade(patid);
            }
            catch (Exception ex)
            {
                throw;
            }
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "0")
                return RedirectToAction("Index", "Patient");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "1")
                return RedirectToAction("Dashboard", "Support");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "2")
                return RedirectToAction("Dashboard", "Therapist");
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") == "3")
                return RedirectToAction("Dashboard", "Provider");
            else
                return RedirectToAction("Dashboard", "Provider");
        }
    }
}
