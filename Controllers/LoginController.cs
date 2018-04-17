using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Repository.Interface;
using Microsoft.Extensions.Logging;
using OneDirect.Models;
using OneDirect.Repository;
using Microsoft.AspNetCore.Http;
using OneDirect.Helper;
using OneDirect.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;
        public LoginController(OneDirectContext context, ILogger<LoginController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);

        }
        // GET: /<controller>/
        public IActionResult Index(string id = "")
        {
            //var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

            // string offset1=Utilities.convert(5.5);
            //string offset2 = Utilities.convert(-5);

            //string result = Utilities.GetUTCDateTime(DateTime.Now, offset1);
            //string result1 = Utilities.GetLocalDateTime(Convert.ToDateTime(result), offset2);
            if (!string.IsNullOrEmpty(id))
            {
                //ViewBag.SessionMessage = "Session Expired, Login again";
                TempData["msg"] = "<script>Helpers.ShowMessage('Session expired, please login again', 1);</script>";
            }
            return View();
        }

        public IActionResult Signout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Index(LoginViewModel pUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!String.IsNullOrEmpty(pUser.UserId) && !String.IsNullOrEmpty(pUser.Password))
                    {
                        if (pUser.UserId.ToString().Trim() == ConfigVars.NewInstance.AdminUserName && pUser.Password.ToString().Trim() == ConfigVars.NewInstance.AdminPassword)
                        {
                            HttpContext.Session.SetString("UserName", pUser.UserId);
                            HttpContext.Session.SetString("UserId", pUser.UserId);
                            HttpContext.Session.SetString("UserType", "0");
                            return RedirectToAction("Index", "Patient");
                        }
                        else
                        {
                            User _users = new Models.User();
                            _users = lIUserRepository.LoginUser(pUser.UserId.ToString().Trim(), pUser.Password.ToString().Trim());
                            if (_users != null)
                            {
                                HttpContext.Session.SetString("UserId", pUser.UserId);
                                HttpContext.Session.SetString("UserName", _users.Name);
                                HttpContext.Session.SetString("UserType", _users.Type.ToString());

                                if (_users.Type.ToString() == "1")
                                    return RedirectToAction("Dashboard", "Support");
                                if (_users.Type.ToString() == "2")
                                    return RedirectToAction("Dashboard", "Therapist");
                                if (_users.Type.ToString() == "3")
                                    return RedirectToAction("Dashboard", "Provider");
                                //return RedirectToAction("Index", "Therapist");
                            }
                            else
                            {
                                TempData["msg"] = "<script>alert('Invalid Username or Password');</script>";
                            }

                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

    }
}
