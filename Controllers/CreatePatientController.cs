using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.ViewModels;
using OneDirect.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class CreatePatientController : Controller
    {
        // GET: /<controller>/
        private readonly IUserInterface lIUserRepository;
        private readonly IPatientConfigurationInterface lIPatientConfigurationRepository;
        private readonly INewPatient INewPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public CreatePatientController(OneDirectContext context, ILogger<CreatePatientController> plogger)
        {
            logger = plogger;
            this.context = context;
            INewPatient = new NewPatientRepository(context);
            lIUserRepository = new UserRepository(context);
            lIPatientConfigurationRepository = new PatientConfigurationRepository(context);
        }

        public IActionResult CreatePatient(string patid = "", string operaton = "")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            NewPatient _newPatient = new NewPatient();
            if (TempData["Patient"] != null && !string.IsNullOrEmpty(TempData["Patient"].ToString()))
            {
                _newPatient = JsonConvert.DeserializeObject<NewPatient>(TempData["Patient"].ToString());
            }
            getDetails();
            ViewBag.Action = operaton;
            List<Sides> Sides = Utilities.GetSide();
            list = new List<SelectListItem>();
            foreach (Sides ex in Sides)
            {
                list.Add(new SelectListItem { Text = ex.Side.ToString(), Value = ex.Side.ToString() });
            }
            ViewBag.sides = new SelectList(list, "Value", "Text");

            if (!String.IsNullOrEmpty(operaton) && operaton == "edit")
            {
                _newPatient = INewPatient.GetPatientByPatId(Convert.ToInt32(patid));
                ViewBag.PatientName = _newPatient.PatientName;
                _newPatient.Action = operaton;
            }

            if (HttpContext.Session.GetString("UserType") == ConstantsVar.Admin.ToString())
            {
                List<User> _userProviderlist = lIUserRepository.getUserListByType(ConstantsVar.Provider);

                var ObjList = _userProviderlist.Select(r => new SelectListItem
                {
                    Value = r.UserId.ToString(),
                    Text = r.Name
                });
                ViewBag.Provider = new SelectList(ObjList, "Value", "Text");


            }
            List<User> _userTherapistlist = lIUserRepository.getUserListByType(ConstantsVar.Therapist);

            var ObjListTherapist = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapist = new SelectList(ObjListTherapist, "Value", "Text");

            return View(_newPatient);
        }
        [HttpPost]
        public IActionResult CreatePatient(NewPatient _NewPatient)
        {
            if (_NewPatient.Action == "edit")
            {
                string _result = INewPatient.UpdatePatient(_NewPatient);
                if (_result == "success")
                {
                    //Prabhu
                    User pUser = lIUserRepository.getUser(_NewPatient.PatientLoginId);
                    if (pUser != null)
                    {
                        User lpatient = new Models.User();
                        lpatient.UserId = _NewPatient.PatientLoginId;
                        lpatient.Name = _NewPatient.PatientName;
                        lpatient.Password = _NewPatient.Pin.HasValue ? _NewPatient.Pin.Value.ToString() : "";
                        lpatient.Type = ConstantsVar.Patient;
                        lpatient.Email = "";
                        lpatient.Address = _NewPatient.AddressLine + " " + _NewPatient.City + " " + _NewPatient.State + " " + _NewPatient.Zip;
                        lpatient.Phone = _NewPatient.PhoneNumber;
                        lIUserRepository.UpdateUser(lpatient);
                    }
                    else
                    {
                        User lpatient = new Models.User();
                        lpatient.UserId = _NewPatient.PatientLoginId;
                        lpatient.Name = _NewPatient.PatientName;
                        lpatient.Password = _NewPatient.Pin.HasValue ? _NewPatient.Pin.Value.ToString() : "";
                        lpatient.Type = ConstantsVar.Patient;
                        lpatient.Email = "";
                        lpatient.Address = _NewPatient.AddressLine + " " + _NewPatient.City + " " + _NewPatient.State + " " + _NewPatient.Zip;
                        lpatient.Phone = _NewPatient.PhoneNumber;
                        lIUserRepository.InsertUser(lpatient);
                    }
                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                    {
                        return RedirectToAction("Dashboard", "Provider", new { id = _NewPatient.ProviderId });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Patient");
                    }
                }
            }
            else
            {
                string patientId = _NewPatient.Ssn + _NewPatient.PhoneNumber.Substring(_NewPatient.PhoneNumber.Length - 4);
                NewPatient lexistpatient = INewPatient.GetPatientByPatitentLoginId(patientId);
                if (lexistpatient == null)
                {
                    NewPatientWithProtocol Patient = new NewPatientWithProtocol();
                    Patient.NewPatient = new NewPatient();
                    Patient.NewPatient = _NewPatient;
                    HttpContext.Session.SetString("NewPatient", JsonConvert.SerializeObject(Patient));
                    return RedirectToAction("PatientRX");
                }
                else
                {
                    TempData["Patient"] = JsonConvert.SerializeObject(_NewPatient);
                    TempData["msg"] = "<script>Helpers.ShowMessage('Patient SSN and Phonenumber is already registered, please use different one', 1);</script>";
                    return RedirectToAction("CreatePatient");
                }

               
            }
            return RedirectToAction("PatientRX");
        }

        public IActionResult PatientRX(string patid = "", string operaton = "")
        {
            NewPatientWithProtocol newPatientWithProtocol = new NewPatientWithProtocol();
            List<EquipmentExcercise> EquipmentExcerciselist = Utilities.GetEquipmentExcercise();
            List<ExcerciseProtocol> ExcerciseProtocollist = Utilities.GetExcerciseProtocol();
            newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();
            List<PatientRx> PatientRx = null;

            ViewBag.Action = operaton;
            if (!String.IsNullOrEmpty(operaton) && operaton == "edit")
            {
                PatientRx = INewPatient.GetNewPatientRxByPatId(patid);
                if (PatientRx != null && PatientRx.Count > 0)
                {
                    foreach (PatientRx patRx in PatientRx)
                    {
                        ViewBag.EquipmentType = patRx.EquipmentType;
                        ViewBag.PatientName = patRx.Patient.PatientName;
                        ViewBag.SurgeryDate = patRx.Patient.SurgeryDate;
                        EquipmentExcercise EquipmentExcercise = EquipmentExcerciselist.Where(p => p.Limb.ToLower() == patRx.EquipmentType.ToLower() && p.ExcerciseEnum == patRx.DeviceConfiguration).FirstOrDefault();
                        ExcerciseProtocol ExcerciseProtocol = ExcerciseProtocollist.Where(p => p.Limb == patRx.EquipmentType && p.ExcerciseEnum == patRx.DeviceConfiguration).Distinct().FirstOrDefault();
                        NewPatientRx _NewPatientRx = new NewPatientRx();
                        _NewPatientRx.Action = "edit";
                        if (patRx.EquipmentType.ToLower() == "shoulder")
                        {
                            if (patRx.DeviceConfiguration == "Forward Flexion")
                            {
                                _NewPatientRx.HeadingFlexion = "Degree of Flexion";
                                _NewPatientRx.CurrentFlex = Constants.Sh_Flex_Current;
                                _NewPatientRx.GoalFlex = Constants.Sh_Flex_Goal;
                            }
                            if (patRx.DeviceConfiguration == "External Rotation")
                            {
                                _NewPatientRx.HeadingFlexion = "Degree of External Rotation";
                                _NewPatientRx.CurrentFlex = Constants.Sh_ExRot_Current;
                                _NewPatientRx.GoalFlex = Constants.Sh_ExRot_Goal;
                            }
                            //if (patRx.DeviceConfiguration == 2)
                            //{
                            //    _NewPatientRx.HeadingFlexion = "Degree of Abduction";
                            //    _NewPatientRx.CurrentFlex = Constants.Sh_Abd_Current;
                            //    _NewPatientRx.GoalFlex = Constants.Sh_Abd_Goal;
                            //}

                        }
                        else if (patRx.EquipmentType.ToLower() == "ankle")
                        {
                            _NewPatientRx.HeadingFlexion = "Degree of Flexion";
                            _NewPatientRx.HeadingExtension = "Degree of Extension";
                            _NewPatientRx.CurrentFlex = Constants.Ankle_Flex_Current;
                            _NewPatientRx.GoalFlex = Constants.Ankle_Flex_Goal;
                            _NewPatientRx.CurrentExten = Constants.Ankle_Ext_Current;
                            _NewPatientRx.GoalExten = Constants.Ankle_Ext_Goal;
                        }
                        else
                        {
                            _NewPatientRx.HeadingFlexion = "Degree of Flexion";
                            _NewPatientRx.HeadingExtension = "Degree of Extension";
                            _NewPatientRx.CurrentFlex = Constants.Knee_Flex_Current;
                            _NewPatientRx.GoalFlex = Constants.Knee_Flex_Goal;
                            _NewPatientRx.CurrentExten = Constants.Knee_Ext_Current;
                            _NewPatientRx.GoalExten = Constants.Knee_Ext_Goal;
                        }
                        _NewPatientRx.TherapyType = EquipmentExcercise.ExcerciseName;
                        _NewPatientRx.DeviceConfiguration = EquipmentExcercise.ExcerciseEnum;
                        _NewPatientRx.ProtocolEnum = ExcerciseProtocol.ProtocolEnum;
                        _NewPatientRx.ProtocolName = ExcerciseProtocol.ProtocolName;
                        _NewPatientRx.EquipmentType = patRx.EquipmentType;

                        _NewPatientRx.RxId = patRx.RxId;
                        _NewPatientRx.RxDaysPerweek = patRx.RxDaysPerweek;
                        _NewPatientRx.RxSessionsPerWeek = patRx.RxSessionsPerWeek;
                        _NewPatientRx.RxStartDate = patRx.RxStartDate;
                        _NewPatientRx.RxEndDate = patRx.RxEndDate;
                        _NewPatientRx.ProviderId = patRx.ProviderId;
                        _NewPatientRx.PatientId = patRx.PatientId;

                        _NewPatientRx.CurrentExtension = patRx.CurrentExtension;
                        _NewPatientRx.CurrentFlexion = patRx.CurrentFlexion;
                        _NewPatientRx.GoalExtension = patRx.GoalExtension;
                        _NewPatientRx.GoalFlexion = patRx.GoalFlexion;
                        _NewPatientRx.PainThreshold = patRx.PainThreshold;
                        _NewPatientRx.RateOfChange = patRx.RateOfChange;

                        newPatientWithProtocol.PainThreshold = patRx.PainThreshold;
                        newPatientWithProtocol.RateOfChange = patRx.RateOfChange;
                        newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                    }
                }
            }
            else
            {
                NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();
                if (HttpContext.Session.GetString("NewPatient") != null)
                {
                    _NewPatient = JsonConvert.DeserializeObject<NewPatientWithProtocol>(HttpContext.Session.GetString("NewPatient").ToString());
                }
                else
                {
                    return RedirectToAction("CreatePatient");
                }

                EquipmentExcerciselist = EquipmentExcerciselist.Where(p => p.Limb.ToLower() == _NewPatient.NewPatient.EquipmentType.ToLower()).ToList();

                newPatientWithProtocol.NewPatientRXList = new List<NewPatientRx>();

                ExcerciseProtocollist = ExcerciseProtocollist.Where(p => p.Limb == _NewPatient.NewPatient.EquipmentType).Distinct().ToList();

                NewPatientRx _NewPatientRx = null;

                if (_NewPatient.NewPatient.EquipmentType.ToLower() == "shoulder")
                {
                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[0].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[0].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = "Degree of Flexion";
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[0].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[0].ProtocolName;
                    _NewPatientRx.CurrentFlex = Constants.Sh_Flex_Current;
                    _NewPatientRx.GoalFlex = Constants.Sh_Flex_Goal;
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);

                    //_NewPatientRx = new NewPatientRx();
                    //_NewPatientRx.Action = "add";
                    //_NewPatientRx.TherapyType = EquipmentExcerciselist[1].ExcerciseName;
                    //_NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[1].ExcerciseEnum;
                    //_NewPatientRx.HeadingFlexion = "Degree of Abduction";
                    //_NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    //_NewPatientRx.ProtocolEnum = ExcerciseProtocollist[1].ProtocolEnum;
                    //_NewPatientRx.ProtocolName = ExcerciseProtocollist[1].ProtocolName;
                    //_NewPatientRx.CurrentFlex = Constants.Sh_Abd_Current;
                    //_NewPatientRx.GoalFlex = Constants.Sh_Abd_Goal;
                    //newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);

                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[1].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[1].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = "Degree of External Rotation";
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[1].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[1].ProtocolName;
                    _NewPatientRx.CurrentFlex = Constants.Sh_ExRot_Current;
                    _NewPatientRx.GoalFlex = Constants.Sh_ExRot_Goal;
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                }
                else
                {
                    _NewPatientRx = new NewPatientRx();
                    _NewPatientRx.Action = "add";
                    _NewPatientRx.TherapyType = EquipmentExcerciselist[0].ExcerciseName;
                    _NewPatientRx.DeviceConfiguration = EquipmentExcerciselist[0].ExcerciseEnum;
                    _NewPatientRx.HeadingFlexion = "Degree of Flexion";
                    _NewPatientRx.HeadingExtension = "Degree of Extension";
                    _NewPatientRx.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                    _NewPatientRx.ProtocolEnum = ExcerciseProtocollist[0].ProtocolEnum;
                    _NewPatientRx.ProtocolName = ExcerciseProtocollist[0].ProtocolName;
                    if (_NewPatient.NewPatient.EquipmentType.ToLower() == "ankle")
                    {
                        _NewPatientRx.CurrentFlex = Constants.Ankle_Flex_Current;
                        _NewPatientRx.GoalFlex = Constants.Ankle_Flex_Goal;
                        _NewPatientRx.CurrentExten = Constants.Ankle_Ext_Current;
                        _NewPatientRx.GoalExten = Constants.Ankle_Ext_Goal;
                    }
                    else
                    {
                        _NewPatientRx.CurrentFlex = Constants.Knee_Flex_Current;
                        _NewPatientRx.GoalFlex = Constants.Knee_Flex_Goal;
                        _NewPatientRx.CurrentExten = Constants.Knee_Ext_Current;
                        _NewPatientRx.GoalExten = Constants.Knee_Ext_Goal;
                    }
                    newPatientWithProtocol.NewPatientRXList.Add(_NewPatientRx);
                }

                ViewBag.EquipmentType = _NewPatient.NewPatient.EquipmentType;
                ViewBag.SurgeryDate = _NewPatient.NewPatient.SurgeryDate;
            }
            return View(newPatientWithProtocol);
        }
        [HttpPost]
        public IActionResult PatientRX(NewPatientWithProtocol _NewPatientRx)
        {
            if (_NewPatientRx.NewPatientRXList[0].Action == "edit")
            {
                string _result = INewPatient.UpdatePatientRx(_NewPatientRx.NewPatientRXList, _NewPatientRx.PainThreshold, _NewPatientRx.RateOfChange, HttpContext.Session.GetString("UserType"));
                if (_result == "success")
                {
                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                        return RedirectToAction("Dashboard", "Provider", new { id = _NewPatientRx.ProviderId });
                    else
                        return RedirectToAction("Index", "Patient");
                }
            }
            else
            {
                NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();
                if (HttpContext.Session.GetString("NewPatient") != null)
                {
                    _NewPatient = JsonConvert.DeserializeObject<NewPatientWithProtocol>(HttpContext.Session.GetString("NewPatient").ToString());
                }
                _NewPatientRx.NewPatient = new NewPatient();
                _NewPatientRx.NewPatient = _NewPatient.NewPatient;

                _NewPatient.NewPatientRX = new NewPatientRx();
                _NewPatientRx.NewPatientRX = _NewPatient.NewPatientRX;

                if (HttpContext.Session.GetString("UserId") != null)
                {
                    //prabhu
                    if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                        _NewPatientRx.ProviderId = HttpContext.Session.GetString("UserId").ToString();
                    else
                        _NewPatientRx.ProviderId = _NewPatient.NewPatient.ProviderId;

                    string _result = INewPatient.CreateNewPatientByProvider(_NewPatientRx);
                    string[] _resultstr = _result.Split('/');
                    if (_resultstr[1] == "success")
                    {
                        //Prabhu
                        User pUser = lIUserRepository.getUser(_NewPatient.NewPatient.PatientLoginId);
                        if (pUser == null)
                        {
                            User lpatient = new Models.User();
                            lpatient.UserId = _resultstr[0];
                            lpatient.Name = _NewPatient.NewPatient.PatientName;
                            lpatient.Password = "";
                            lpatient.Type = ConstantsVar.Patient;
                            lpatient.Email = "";
                            lpatient.Address = _NewPatient.NewPatient.AddressLine + " " + _NewPatient.NewPatient.City + " " + _NewPatient.NewPatient.State + " " + _NewPatient.NewPatient.Zip;
                            lpatient.Phone = _NewPatient.NewPatient.PhoneNumber;
                            lIUserRepository.InsertUser(lpatient);
                        }

                        HttpContext.Session.SetString("NewPatient", "");
                        return RedirectToAction("ProtocolList", new { patId = _resultstr[0], eType = _NewPatient.NewPatient.EquipmentType });

                    }
                }
            }
            // HttpContext.Session.SetString("NewPatient", JsonConvert.SerializeObject(_NewPatient));
            return RedirectToAction("CreatePatient");
        }
        public IActionResult ProtocolList(string patId, string eType, string Username = "")
        {
            ViewBag.PatientName = Username;
            List<NewProtocol> _result = INewPatient.GetProtocolListBypatId(patId);
            ViewBag.VisibleAddButton = false;
            ViewBag.VisibleAddButtonExt = false;
            if (!string.IsNullOrEmpty(eType))
            {
                if (eType.ToLower() == "shoulder")
                {
                    PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(patId), eType, "Forward Flexion");
                    if (lconfig != null)
                    {
                        ViewBag.VisibleAddButton = true;
                        if (_result != null)
                        {
                            List<NewProtocol> _resultFlex = _result.Where(x => x.ExcerciseEnum == "Forward Flexion").ToList();
                            if (_resultFlex == null || (_resultFlex != null && _resultFlex.Count == 0))
                            {
                                ViewBag.RxId = lconfig.RxId;
                            }
                        }
                    }
                    PatientConfiguration lconfigext = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(patId), eType, "External Rotation");
                    if (lconfigext != null)
                    {
                        ViewBag.VisibleAddButtonExt = true;
                        if (_result != null)
                        {
                            List<NewProtocol> _resultExt = _result.Where(x => x.ExcerciseEnum == "External Rotation").ToList();
                            if (_resultExt == null || (_resultExt != null && _resultExt.Count == 0))
                            {
                                ViewBag.RxIdExt = lconfig.RxId;
                            }
                        }

                    }
                }
                else
                {
                    PatientConfiguration lconfig = lIPatientConfigurationRepository.getPatientConfigurationbyPatientId(Convert.ToInt32(patId), eType);

                    if (lconfig != null)
                    {
                        ViewBag.VisibleAddButton = true;

                        if (_result == null || (_result != null && _result.Count == 0))
                        {
                            ViewBag.RxId = lconfig.RxId;
                        }
                        else
                        {
                            ViewBag.RxId = _result[0].RxId;
                        }
                    }

                }
            }


            ViewBag.ProtocolList = _result;
            ViewBag.etype = eType;

            if (String.IsNullOrEmpty(Username))
                ViewBag.PatientName = Username;
            if (_result != null && _result.Count > 0 && String.IsNullOrEmpty(Username))
            {
                // ViewBag.RxId = _result[0].RxId;
                ViewBag.PatientName = _result[0].PatientName;
            }



            return View(_result);
        }
        public IActionResult Protocol(string protocolid = "", string protocolName = "", string ExeName = "", string RXID = "")
        {
            NewProtocol _protocol = new NewProtocol();
            PatientRx PatientRx = null;
            ViewBag.Configuration = ExeName;
            // ViewBag.Excercise = ExeName;
            ViewBag.ProtocolName = ExeName;
            List<EquipmentExcercise> EquipmentExcercise = Utilities.GetEquipmentExcercise();
            List<ExcerciseProtocol> ExcerciseProtocol = Utilities.GetExcerciseProtocol();
            if (String.IsNullOrEmpty(protocolid) && !String.IsNullOrEmpty(RXID))
            {
                PatientRx = INewPatient.GetNewPatientRxByRxId(RXID);
                if (PatientRx != null)
                {
                    ViewBag.PatientName = PatientRx.Patient.PatientName;
                    _protocol.EquipmentType = PatientRx.EquipmentType;
                    ViewBag.EType = PatientRx.EquipmentType;
                    EquipmentExcercise _EquipmentExcercise = EquipmentExcercise.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).FirstOrDefault();
                    _protocol.ExcerciseName = _EquipmentExcercise.ExcerciseName;
                    _protocol.ExcerciseEnum = _EquipmentExcercise.ExcerciseEnum;
                    ViewBag.Configuration = _EquipmentExcercise.ExcerciseName;

                    ExcerciseProtocol _ExcerciseProtocol = ExcerciseProtocol.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).FirstOrDefault();
                    ViewBag.ProtocolName = _ExcerciseProtocol.ProtocolName;
                    _protocol.ProtocolName = _ExcerciseProtocol.ProtocolName;
                    _protocol.ProtocolEnum = _ExcerciseProtocol.ProtocolEnum;
                    _protocol.PatientId = PatientRx.PatientId;
                    _protocol.StartDate = PatientRx.RxStartDate;
                    _protocol.EndDate = PatientRx.RxEndDate;

                    _protocol.RateOfChange = PatientRx.RateOfChange;

                    _protocol.SurgeryDate = PatientRx.RxStartDate;
                    _protocol.RxEndDate = PatientRx.RxEndDate;

                    _protocol.RestAt = 0;
                    _protocol.RepsAt = 0;
                    _protocol.Speed = 0;

                    List<ExcerciseProtocol> prolist = ExcerciseProtocol.Where(p => p.Limb.ToLower() == PatientRx.EquipmentType.ToLower() && p.ExcerciseEnum == PatientRx.DeviceConfiguration).ToList();
                    List<SelectListItem> list = new List<SelectListItem>();
                    foreach (ExcerciseProtocol ex in prolist)
                    {
                        list.Add(new SelectListItem { Text = ex.ProtocolName.ToString(), Value = ex.ProtocolEnum.ToString() });
                    }
                    ViewBag.Protocol = new SelectList(list, "Value", "Text");
                }
            }

            NewPatientWithProtocol _NewPatient = new NewPatientWithProtocol();


            if (!String.IsNullOrEmpty(protocolid))
            {
                _protocol = INewPatient.GetProtocolByproId(protocolid);
                ViewBag.Configuration = getExcercise(_protocol.EquipmentType, _protocol.ExcerciseEnum);
                ViewBag.Action = "edit";
                ViewBag.EType = _protocol.EquipmentType;
                ViewBag.PatientName = _protocol.PatientName;

            }
            if (_protocol.EquipmentType.ToLower() == "shoulder")
            {
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);
                    //_protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + (Convert.ToInt32(PatientRx.RateOfChange) * ((PatientRx.RxEndDate.Value - PatientRx.RxStartDate.Value).Days / 7));
                    //_protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + (((Convert.ToInt32(PatientRx.CurrentFlexion) * (Convert.ToInt32(PatientRx.RateOfChange)) / 100)));//PatientRx.GoalFlexion;

                }
                if (_protocol.ExcerciseEnum == "Forward Flexion")
                {
                    _protocol.CurrentFlex = Constants.Sh_Flex_Current;
                    _protocol.GoalFlex = Constants.Sh_Flex_Goal;
                }
                //if (_protocol.ExcerciseEnum == 2)
                //{
                //    _protocol.CurrentFlex = Constants.Sh_Abd_Current;
                //    _protocol.GoalFlex = Constants.Sh_Abd_Goal;
                //}
                if (_protocol.ExcerciseEnum == "External Rotation")
                {
                    _protocol.CurrentFlex = Constants.Sh_ExRot_Current;
                    _protocol.GoalFlex = Constants.Sh_ExRot_Goal;
                }
            }
            else if (_protocol.EquipmentType.ToLower() == "ankle")
            {
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);
                    //_protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + (Convert.ToInt32(PatientRx.RateOfChange) * ((PatientRx.RxEndDate.Value - PatientRx.RxStartDate.Value).Days / 7));
                    //_protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + (((Convert.ToInt32(PatientRx.CurrentFlexion) * (Convert.ToInt32(PatientRx.RateOfChange)) / 100)));//PatientRx.GoalFlexion;

                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;
                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);
                    //_protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) + (Convert.ToInt32(PatientRx.RateOfChange) * ((PatientRx.RxEndDate.Value - PatientRx.RxStartDate.Value).Days / 7));
                    //_protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - (((Convert.ToInt32(PatientRx.CurrentExtension) * (Convert.ToInt32(PatientRx.RateOfChange)) / 100)));//PatientRx.GoalExtension;
                }
                _protocol.CurrentFlex = Constants.Ankle_Flex_Current;
                _protocol.GoalFlex = Constants.Ankle_Flex_Goal;
                _protocol.CurrentExten = Constants.Ankle_Ext_Current;
                _protocol.GoalExten = Constants.Ankle_Ext_Goal;
            }
            else
            {
                if (PatientRx != null)
                {
                    _protocol.FlexUpLimit = PatientRx.CurrentFlexion;
                    _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + Convert.ToInt32(PatientRx.RateOfChange);
                    //_protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + (Convert.ToInt32(PatientRx.RateOfChange) * ((PatientRx.RxEndDate.Value - PatientRx.RxStartDate.Value).Days / 7));
                    // _protocol.StretchUpLimit = Convert.ToInt32(PatientRx.CurrentFlexion) + (((Convert.ToInt32(PatientRx.CurrentFlexion) * (Convert.ToInt32(PatientRx.RateOfChange)) / 100)));//PatientRx.GoalFlexion;

                    _protocol.FlexDownLimit = PatientRx.CurrentExtension;
                    _protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - Convert.ToInt32(PatientRx.RateOfChange);
                    //_protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) + (Convert.ToInt32(PatientRx.RateOfChange) * ((PatientRx.RxEndDate.Value - PatientRx.RxStartDate.Value).Days / 7));
                    //_protocol.StretchDownLimit = Convert.ToInt32(PatientRx.CurrentExtension) - (((Convert.ToInt32(PatientRx.CurrentExtension) * (Convert.ToInt32(PatientRx.RateOfChange)) / 100)));//PatientRx.GoalExtension;
                }
                _protocol.CurrentFlex = Constants.Knee_Flex_Current;
                _protocol.GoalFlex = Constants.Knee_Flex_Goal;
                _protocol.CurrentExten = Constants.Knee_Ext_Current;
                _protocol.GoalExten = Constants.Knee_Ext_Goal;
            }
           
            return View(_protocol);
        }
        private string getProtocolType(string limb, int? proenum, string exenum)
        {
            List<ExcerciseProtocol> ExcerciseProtocollist = Utilities.GetExcerciseProtocol();
            ExcerciseProtocol _EquipmentExcercise = ExcerciseProtocollist.Where(p => p.Limb.ToLower() == limb.ToLower() && p.ProtocolEnum == Convert.ToInt32(proenum) && p.ExcerciseEnum == exenum).FirstOrDefault();
            return _EquipmentExcercise.ProtocolName;
        }
        private string getExcercise(string limb, string exenum)
        {
            List<EquipmentExcercise> EquipmentExcercise = Utilities.GetEquipmentExcercise();
            EquipmentExcercise _EquipmentExcercise = EquipmentExcercise.Where(p => p.Limb.ToLower() == limb.ToLower() && p.ExcerciseEnum == exenum).FirstOrDefault();
            return _EquipmentExcercise.ExcerciseName;
        }

        [HttpPost]
        public IActionResult Protocol(NewProtocol NewProtocol)
        {
            //PatientConfiguration lpatconfig = lIPatientConfigurationRepository.getPatientConfigurationbyRxId(NewProtocol.RxId);
            //if (lpatconfig != null && lpatconfig.Setup != null)
            //{
            //    int limitValueFlex = 0;
            //    int limitValueExt = 0;
            //    if (NewProtocol.EquipmentType.ToLower() == "shoulder")
            //    {
            //        if (NewProtocol.ExcerciseEnum == "Forward Flexion")
            //        {
            //            limitValueFlex = lpatconfig.Setup.Actuator2ExtendedAngle;
            //        }
            //        if (NewProtocol.ExcerciseEnum == "External Rotation")
            //        {
            //            limitValueFlex = lpatconfig.Setup.Actuator3ExtendedAngle.HasValue ? lpatconfig.Setup.Actuator3ExtendedAngle.Value : 0;
            //        }
            //        if (NewProtocol.StretchUpLimit > limitValueFlex)
            //        {
            //            NewProtocol.StretchUpLimit = limitValueFlex;
            //        }
            //    }
            //    else if (NewProtocol.EquipmentType.ToLower() == "ankle")
            //    {
            //        limitValueFlex = lpatconfig.Setup.Actuator1ExtendedAngle;
            //        limitValueExt = lpatconfig.Setup.Actuator1RetractedAngle;
            //        if (NewProtocol.ProtocolEnum.ToString() == "1")
            //        {
            //            if (NewProtocol.StretchUpLimit > limitValueFlex)
            //            {
            //                NewProtocol.StretchUpLimit = limitValueFlex;
            //            }
            //        }
            //        else if (NewProtocol.ProtocolEnum.ToString() == "2")
            //        {
            //            if (NewProtocol.StretchDownLimit > limitValueExt)
            //            {
            //                NewProtocol.StretchDownLimit = limitValueExt;
            //            }
            //        }
            //        else
            //        {
            //            if (NewProtocol.StretchUpLimit > limitValueFlex)
            //            {
            //                NewProtocol.StretchUpLimit = limitValueFlex;
            //            }
            //            if (NewProtocol.StretchDownLimit > limitValueExt)
            //            {
            //                NewProtocol.StretchDownLimit = limitValueExt;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        limitValueFlex = lpatconfig.Setup.Actuator2ExtendedAngle;
            //        limitValueExt = lpatconfig.Setup.Actuator2RetractedAngle;

            //        if (NewProtocol.ProtocolEnum.ToString() == "1")
            //        {
            //            if (NewProtocol.StretchUpLimit > limitValueFlex)
            //            {
            //                NewProtocol.StretchUpLimit = limitValueFlex;
            //            }
            //        }
            //        else if (NewProtocol.ProtocolEnum.ToString() == "2")
            //        {
            //            if (NewProtocol.StretchDownLimit > limitValueExt)
            //            {
            //                NewProtocol.StretchDownLimit = limitValueExt;
            //            }
            //        }
            //        else
            //        {
            //            if (NewProtocol.StretchUpLimit > limitValueFlex)
            //            {
            //                NewProtocol.StretchUpLimit = limitValueFlex;
            //            }
            //            if (NewProtocol.StretchDownLimit > limitValueExt)
            //            {
            //                NewProtocol.StretchDownLimit = limitValueExt;
            //            }
            //        }
            //    }
            //}
            string _result = INewPatient.CreateProtocol(NewProtocol);
            if (_result == "success")
                return RedirectToAction("ProtocolList", new { patId = NewProtocol.PatientId, eType = NewProtocol.EquipmentType });

            return RedirectToAction("CreatePatient");
        }

        public IActionResult Delete(string proId, string patId, string patName, string eType)
        {
            try
            {
                string result = INewPatient.DeleteProtocolRecordsWithCasecade(proId);
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("ProtocolList", "CreatePatient", new { patId = patId, Username = patName, eType = eType });
        }
        public void getDetails()
        {
            //List<SelectListItem> myList = new List<SelectListItem>()
            //             {
            //                new SelectListItem{ Value="1",Text="Ankle"},
            //                new SelectListItem{ Value="2",Text="Knee"},
            //                new SelectListItem{ Value="4",Text="Shoulder"}
            //             };
            List<SelectListItem> myList = new List<SelectListItem>()
                         {
                            new SelectListItem{ Value="Ankle",Text="Ankle"},
                            new SelectListItem{ Value="Knee",Text="Knee"},
                            new SelectListItem{ Value="Shoulder",Text="Shoulder"}
                         };
            ViewBag.equipment = myList;

        }
    }
}
