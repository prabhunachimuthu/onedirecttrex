using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using System.Data;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;
using System.Net.Http.Headers;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class EquipmentController : Controller
    {
        private readonly IDeviceCalibrationInterface lIDeviceCalibrationRepository;
        private readonly IAssignmentInterface lIAssignmentInterface;
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public EquipmentController(OneDirectContext context, ILogger<EquipmentController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIAssignmentInterface = new AssignmentRepository(context);
            lIDeviceCalibrationRepository = new DeviceCalibrationRepository(context);
            lIUserRepository = new UserRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            try
            {
                PatientConfigurationResult lresult = new PatientConfigurationResult();
                List<PatientConfigurationDetails> llist = lIDeviceCalibrationRepository.getAllPatientDeviceCalibration();
                List<DeviceConfigurationDetails> llist1 = lIDeviceCalibrationRepository.getDeviceCalibration();

                lresult.patientconfiguration = llist;
                lresult.devicecalibration = llist1;
                //if (llist == null || (llist != null && llist.Count == 0))
                //{

                //}
                //logger.LogDebug("Pain Post Start");
                //List<EquipmentAssignment> passignmentlist = lIAssignmentInterface.getEquipmentAssignment();
                //if (passignmentlist != null && passignmentlist.Count > 0)
                //    foreach (EquipmentAssignment equ in passignmentlist)
                //    {
                //        if (equ.Limb == "1")
                //            equ.Limb = "Ankle";
                //        else if (equ.Limb == "2")
                //            equ.Limb = "Knee";
                //        else if (equ.Limb == "3")
                //            equ.Limb = "Elbow";
                //        else if (equ.Limb == "4")
                //            equ.Limb = "Shoulder";

                //    }
                //ViewBag.assignmentlist = passignmentlist;
                return View(lresult);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }

        public IActionResult Delete(string id)
        {
            lIDeviceCalibrationRepository.deleteDeviceCalibrationCascade(id);
            return RedirectToAction("Index", "Equipment");
        }
        public IActionResult AddEdit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    DeviceConfigurationDetails ldevice = lIDeviceCalibrationRepository.getDeviceCalibrationbySetupId(id);
                    return View(ldevice);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
            return null;
        }
        [HttpPost]
        public IActionResult AddEdit(DeviceConfigurationDetails pdevice)
        {
            try
            {
                if (pdevice != null && !string.IsNullOrEmpty(pdevice.devicecalibration.SetupId))
                {
                    DeviceCalibration ldevice = lIDeviceCalibrationRepository.getDeviceCalibration(pdevice.devicecalibration.SetupId);
                    if (ldevice != null)
                    {
                        ldevice.Description = pdevice.devicecalibration.Description;
                        lIDeviceCalibrationRepository.UpdateDeviceCalibration(ldevice);
                        return RedirectToAction("Index", "Equipment");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
            return null;
        }
        public IActionResult Add()
        {
            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;

            List<User> _userPatientlist = lIUserRepository.getUserListByType(1);

            var ObjList = _userPatientlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Patient = new SelectList(ObjList, "Value", "Text");
            ViewBag.Patient = ObjList;

            List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapists = new SelectList(ObjList, "Value", "Text");
            ViewBag.Therapists = ObjTherapistList;

            return View("Add");
        }
        private XElement GetAddress(string address)
        {
            //  var address = "HAL 2nd stage,Indiranagar,Bangalore";
            string requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = new HttpResponseMessage();
            httpClient.BaseAddress = new Uri(requestUri);
            string urlParameters = "";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            response = httpClient.GetAsync(urlParameters).Result;
            XElement resultEle = null;
            if (response.StatusCode.ToString().ToLower() == "ok")
            {
                var ss = response.Content.ReadAsStringAsync().Result.ToString();
                var xdoc = XDocument.Parse(ss);
                foreach (XElement element in xdoc.Descendants("location"))
                {
                    resultEle = element;
                    if (resultEle != null)
                        break;
                }
            }
            return resultEle;
        }
        [HttpPost]
        public IActionResult Add(EquipmentView pEquipmentAssignment)
        {

            XElement ele = GetAddress(pEquipmentAssignment.Address);
            if (ele != null)
            {
                pEquipmentAssignment.Latitude = ele.Element("lat").FirstNode.ToString();
                pEquipmentAssignment.Longitude = ele.Element("lng").FirstNode.ToString();
            }
            EquipmentAssignment pequipment = UserExtension.EquipmentViewToEquipment(pEquipmentAssignment);
            string str = lIAssignmentInterface.InsertEquipmentAssignment(pequipment);
            return RedirectToAction("Index");

        }
        public IActionResult Profile(string id)
        {
            EquipmentAssignment pUser = lIAssignmentInterface.getEquipmentAssignment(id);

            //XElement ele = GetAddress(pUser.Address);
            //pUser.Latitude = ele.FirstNode.ToString();
            //pUser.Longitude = ele.LastNode.ToString();

            List<SelectListItem> myList = new List<SelectListItem>()
             {
                 new SelectListItem{ Value="1",Text="Ankle Unit"},
                 new SelectListItem{ Value="2",Text="Knee Unit"},
                 new SelectListItem{ Value="3",Text="Elbow Unit"},
                 new SelectListItem{ Value="4",Text="Shoulder Unit"},
             };

            ViewBag.equipment = myList;

            List<User> _userPatientlist = lIUserRepository.getUserListByType(1);

            var ObjList = _userPatientlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Patient = new SelectList(ObjList, "Value", "Text");
            ViewBag.Patient = ObjList;

            List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });
            ViewBag.Therapists = new SelectList(ObjList, "Value", "Text");
            ViewBag.Therapists = ObjTherapistList;
            EquipmentView pequipment = UserExtension.EquipmentToEquipmentAssignmentExtension(pUser);
            return View(pequipment);
        }
        [HttpPost]
        public IActionResult Profile(EquipmentView pEquipmentAssignment)
        {
            XElement ele = GetAddress(pEquipmentAssignment.Address);
            if (ele != null)
            {
                pEquipmentAssignment.Latitude = ele.Element("lat").FirstNode.ToString();
                pEquipmentAssignment.Longitude = ele.Element("lng").FirstNode.ToString();
            }
            EquipmentAssignment pequipment = UserExtension.EquipmentViewToEquipment(pEquipmentAssignment);
            string _result = lIAssignmentInterface.UpdateEquipmentAssignment(pequipment);
            return RedirectToAction("Index");
        }
    }
}
