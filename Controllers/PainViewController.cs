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
    public class PainViewController : Controller
    {
        private readonly ISessionInterface lISessionRepository;
        private readonly IPainInterface lIPainRepository;
        private readonly IAssignmentInterface lIEquipmentAssignmentRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public PainViewController(OneDirectContext context, ILogger<PainViewController> plogger)
        {
            logger = plogger;
            this.context = context;
            lISessionRepository = new SessionRepository(context);
            lIPainRepository = new PainRepository(context);
            lIEquipmentAssignmentRepository = new AssignmentRepository(context);
        }
        // GET: /<controller>/
        public IActionResult Index(string sessionId, string date, string time, string protocol = "")
        {
            ViewBag.date = date;
            ViewBag.time = time;
            ViewBag.sessionId = sessionId;
            if (!String.IsNullOrEmpty(date))
                HttpContext.Session.SetString("sessionDate", Convert.ToDateTime(date).ToString("MMM-dd-yyy hh:mm:ss"));
            if (!String.IsNullOrEmpty(time))
                HttpContext.Session.SetString("sessionTime", time);
            UserSession _lsession = lISessionRepository.getSessionbySessionId(sessionId);
            List<SessionPain> _lpain = lIPainRepository.getPainBySessionId(sessionId);
            ViewBag.SessionList = _lsession;
            ViewBag.PainList = _lpain;
            return View();
        }
        public IActionResult Delete(string painId, string sessionId = "", string date = "", string time = "")
        {
            try
            {
                string result = lIPainRepository.DeletePain(painId);
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index", "PainView", new { sessionId = sessionId, date = date, time = time });
        }
    }
}
