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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class MessageViewController : Controller
    {
        private readonly IMessageInterface lIMessageRepository;
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public MessageViewController(OneDirectContext context, ILogger<PainViewController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIMessageRepository = new MessageRepository(context);
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = new PainRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            List<PatientMessageView> patientList = null;
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")) && HttpContext.Session.GetString("UserType") != "0")
                {
                    patientList = lIMessageRepository.getPatientMessages(HttpContext.Session.GetString("UserId"));
                }
                else
                {
                    patientList = lIMessageRepository.getPatientMessagesforAdmin();
                }
                patientList = patientList.Where(x => x.ReceiveMessage > 0 || x.SentMessage > 0).ToList();
            }
            catch (Exception ex)
            {
                logger.LogDebug("Patent Rx Error: " + ex);
            }
            return View(patientList);
        }
        public IActionResult Messages(string patientId = "")
        {
            List<Messages> MessageList = null;
            try
            {
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                {

                    if (!string.IsNullOrEmpty(patientId))
                    {
                        ViewBag.PatientId = patientId;
                        MessageList = lIMessageRepository.getBySenderIdAndReceiverId(patientId, HttpContext.Session.GetString("UserId"));
                        MessageList = MessageList.Where(x => x.ReadStatus == 0).ToList();
                        if (MessageList != null && MessageList.Count > 0)
                        {
                            foreach (Messages message in MessageList)
                            {
                                message.ReadStatus = 1;
                                message.DateModified = DateTime.UtcNow;
                                lIMessageRepository.UpdateMessage(message);
                            }
                        }
                        MessageList = lIMessageRepository.getBySenderIdAndReceiverId(patientId, HttpContext.Session.GetString("UserId"));
                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogDebug("Patent Rx Error: " + ex);
            }
            return View(MessageList);
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
