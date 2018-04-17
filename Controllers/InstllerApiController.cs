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
using OneDirect.Helper;
using System.Data;
using OneDirect.ViewModels;
using Newtonsoft.Json;
using OneDirect.Vsee;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [Route("api/[controller]")]
    public class InstallerApiController : Controller
    {
        private readonly IAppointmentInterface lIAppointmentRepository;
        private readonly IUserInterface lIUserRepository;
        private readonly IPatient IPatient;
        private readonly ILogger logger;
        private OneDirectContext context;

        public InstallerApiController(OneDirectContext context, ILogger<PatientApiController> plogger)
        {
            logger = plogger;
            this.context = context;
            IPatient = new PatientRepository(context);
            lIUserRepository = new UserRepository(context);
            lIAppointmentRepository = new AppointmentRepository(context);
        }

        [HttpGet]
        [Route("installerlogin")]
        public JsonResult installerlogin(string installerid, string password)
        {
            try
            {
                User _result = null;
                if (!string.IsNullOrEmpty(installerid))
                {
                    _result = lIUserRepository.userLogin(installerid, password, 4);
                }
                if (_result != null)
                {
                    return Json(new { Status = (int)HttpStatusCode.OK, Installer = _result, SessionId = _result.LoginSessionId, result = "success", TimeZone = DateTime.UtcNow.ToString("s") });
                }
                else
                {
                    return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Status = (int)HttpStatusCode.InternalServerError, SessionId = "", result = "failed", TimeZone = DateTime.UtcNow.ToString("s") });
            }

        }
    }

}
