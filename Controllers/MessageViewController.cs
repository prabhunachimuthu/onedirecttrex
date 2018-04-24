using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using Microsoft.AspNetCore.Http;
using OneDirect.ViewModels;
using OneDirect.Helper;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class MessageViewController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly IMessageInterface lIMessageRepository;
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public MessageViewController(OneDirectContext context, ILogger<PainViewController> plogger)
        {
            logger = plogger;
            lIUserRepository = new UserRepository(context);
            this.context = context;
            IPatient = new PatientRepository(context);
            lIMessageRepository = new MessageRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = new PainRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
        }
        // GET: /<controller>/
        public IActionResult Index(string patientid = "")
        {
            //List<PatientMessageView> patientList = null;
            //try
            //{
            //    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") != "0")
            //    {
            //        patientList = lIMessageRepository.getPatientMessages(HttpContext.Session.GetString("UserId"));
            //    }
            //    else
            //    {
            //        patientList = lIMessageRepository.getPatientMessagesforAdmin();
            //    }
            //    patientList = patientList.Where(x => x.ReceiveMessage > 0 || x.SentMessage > 0).ToList();
            //}
            //catch (Exception ex)
            //{
            //    logger.LogDebug("Patent Rx Error: " + ex);
            //}
            //return View(patientList);
            List<Patient> lpatientList = new List<Patient>();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") != "0")
            {
                if (HttpContext.Session.GetString("UserType") == ConstantsVar.Therapist.ToString())
                {
                    lpatientList = IPatient.GetPatientByTherapistId(HttpContext.Session.GetString("UserId")).OrderBy(x => x.PatientName).ToList();
                }
                else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Provider.ToString())
                {
                    lpatientList = IPatient.GetPatientByProviderId(HttpContext.Session.GetString("UserId")).OrderBy(x => x.PatientName).ToList();
                }
                else if (HttpContext.Session.GetString("UserType") == ConstantsVar.Support.ToString())
                {
                    lpatientList = IPatient.GetAllPatients().OrderBy(x => x.PatientName).ToList();
                }
                if (lpatientList != null && lpatientList.Count > 0)
                {
                    Patient lpatient = (!string.IsNullOrEmpty(patientid) ? lpatientList.FirstOrDefault(x => x.PatientLoginId == patientid) : lpatientList.FirstOrDefault());
                    ViewBag.Patient = lpatient.PatientName;
                    ViewBag.PatientId = lpatient.PatientLoginId;
                    List<MessageView> lmessages = lIMessageRepository.getMessagesbyTimeZone(lpatient.PatientLoginId, HttpContext.Session.GetString("timezoneid"));

                    ViewBag.Messages = lmessages;
                }
                ViewBag.PatientList = lpatientList;
            }
            return View();
        }

        public JsonResult sendmessage(string patientId = "", string message = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(patientId) && !string.IsNullOrEmpty(message))
                {
                    User luser = lIUserRepository.getUser(HttpContext.Session.GetString("UserId"));
                    if (luser != null)
                    {
                        Messages lmessage = new Messages();
                        lmessage.PatientId = patientId;
                        lmessage.BodyText = message;
                        lmessage.UserId = luser.UserId;
                        lmessage.UserType = luser.Type;
                        lmessage.UserName = luser.Name;
                        lmessage.SentReceivedFlag = 1;
                        lmessage.ReadStatus = 0;
                        lmessage.Datetime = Convert.ToDateTime(Utilities.ConverTimetoServerTimeZone(DateTime.Now, HttpContext.Session.GetString("timezoneid")));
                        int res = lIMessageRepository.InsertMessage(lmessage);
                        if (res > 0)
                        {
                            lmessage.Datetime = Convert.ToDateTime(Utilities.ConverTimetoBrowserTimeZone(lmessage.Datetime, HttpContext.Session.GetString("timezoneid")));
                            return Json(new { result = "success", message = lmessage });
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
        public IActionResult Messages(string patientId = "")
        {
            //List<Messages> MessageList = null;
            //try
            //{
            //    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            //    {

            //        if (!string.IsNullOrEmpty(patientId))
            //        {
            //            ViewBag.PatientId = patientId;
            //            MessageList = lIMessageRepository.getBySenderIdAndReceiverId(patientId, HttpContext.Session.GetString("UserId"));
            //            MessageList = MessageList.Where(x => x.ReadStatus == 0).ToList();
            //            if (MessageList != null && MessageList.Count > 0)
            //            {
            //                foreach (Messages message in MessageList)
            //                {
            //                    message.ReadStatus = 1;
            //                    message.DateModified = DateTime.UtcNow;
            //                    lIMessageRepository.UpdateMessage(message);
            //                }
            //            }
            //            MessageList = lIMessageRepository.getBySenderIdAndReceiverId(patientId, HttpContext.Session.GetString("UserId"));
            //        }

            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.LogDebug("Patent Rx Error: " + ex);
            //}
            //return View(MessageList);
            return View();
        }
        //public JsonResult ViewMessages(string id)
        //{
        //    List<Messages> lMessageList = new List<Messages>();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(id))
        //        {
        //            lMessageList = lIMessageRepository.getMessagebyMessageId(id);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogDebug("Patent Rx Error: " + ex);
        //    }
        //    return Json()
        //}
    }
}
