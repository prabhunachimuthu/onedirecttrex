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
    public class TransactionLogController : Controller
    {
        private readonly ITransactionLogInterface lITransactionLogRepository;
        private readonly IPatient lIPatientRepository;
        private readonly IAppointmentInterface lIAppointmentRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public TransactionLogController(OneDirectContext context, ILogger<AppointmentController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIAppointmentRepository = new AppointmentRepository(context);
            lITransactionLogRepository = new TransactionLogRepository(context);
            lIPatientRepository = new PatientRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            try
            {
                List<TransactionLogView> pView = null;
                if (HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Therapist.ToString())
                    pView = lITransactionLogRepository.GetTransactionbyuserId(HttpContext.Session.GetString("UserId"));
                else
                    pView = lITransactionLogRepository.GetTransactionLogs();

                return View(pView);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return View();
            }
        }

        public IActionResult Add(string transactionId = "")
        {
            try
            {
                TransactionLogView llog = new TransactionLogView();
                List<Patient> patientList = null;
                if (HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Therapist.ToString())
                {
                    patientList = lIPatientRepository.GetPatientByTherapistId(HttpContext.Session.GetString("UserId").ToString());
                }
                else
                {
                    patientList = lIPatientRepository.GetAllPatients();
                }
                if(patientList !=null)
                {
                    var ObjListPatient = patientList.Select(r => new SelectListItem
                    {
                        Value = r.PatientId.ToString(),
                        Text = r.PatientName
                    });
                    ViewBag.Patients = new SelectList(ObjListPatient, "Value", "Text");
                }
                //List<User> _userlist = lIUserRepository.getUserListByType(ConstantsVar.Therapist);


                //var ObjListTherapist = _userTherapistlist.Select(r => new SelectListItem
                //{
                //    Value = r.UserId.ToString(),
                //    Text = r.Name
                //});
                //ViewBag.Therapist = new SelectList(ObjListTherapist, "Value", "Text");
                List<SelectListItem> types = new List<SelectListItem>();
                types.Add(new SelectListItem() { Value = "0", Text = "Video call" });
                types.Add(new SelectListItem() { Value = "1", Text = "Progress Review" });
                types.Add(new SelectListItem() { Value = "2", Text = "Exercise Edit" });
                SelectList transactionTypes = new SelectList(types, "Value", "Text");
                ViewBag.Types = transactionTypes;


                //List<SelectListItem> activities = new List<SelectListItem>();
                //activities.Add(new SelectListItem() { Value = "0", Text = "Appointment record" });
                //activities.Add(new SelectListItem() { Value = "1", Text = "Patient table" });
                //activities.Add(new SelectListItem() { Value = "2", Text = "Exercise table ID" });
                //SelectList linkactivities = new SelectList(activities, "Value", "Text");
                //ViewBag.Activities = linkactivities;

                return View(llog);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Add(TransactionLogView llog)
        {
            try
            {
                if (llog != null)
                {
                    llog.TherapistUserId = HttpContext.Session.GetString("UserId");
                    llog.CreateDate = DateTime.UtcNow;
                    llog.UpdatedDate = DateTime.UtcNow;
                    llog.TransactionId = Guid.NewGuid().ToString();
                    TransactionLog plog = TransactionLogExtension.TransactionLogViewToTransactionLog(llog);
                    if (plog != null)
                    {
                        int result = lITransactionLogRepository.InsertTransactionLog(plog);
                        if (result > 0)
                            return RedirectToAction("Index");
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return RedirectToAction("Index");
            }
        }


        public IActionResult Delete(string transactionId)
        {
            try
            {
                TransactionLog lLog = lITransactionLogRepository.GetTransactionLogbyID(transactionId);
                if (lLog != null)
                {
                    string res = lITransactionLogRepository.DeleteTransactionLog(lLog.TransactionId);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Generate URI Error: " + ex);
            }
            return RedirectToAction("Index");
        }


    }
}




