using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneDirect.Helper;
using OneDirect.Repository.Interface;
using OneDirect.Models;
using Microsoft.Extensions.Logging;
using OneDirect.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using OneDirect.ViewModels;
using OneDirect.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class SupportController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly IProtocolInterface lIProtocolInterface;
        private readonly IPatientRxInterface lIPatientRxRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public SupportController(OneDirectContext context, ILogger<SupportController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
            lIProtocolInterface = new ProtocolRepository(context);
            lIPatientRxRepository = new PatientRxRepository(context);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var response = new Dictionary<string, object>();
            List<User> pUser = new List<User>();
            try
            {
                logger.LogDebug("Supporter Start");

                pUser = lIUserRepository.getUserListByType(ConstantsVar.Support);

                ViewBag.userlist = pUser;
                return View();
            }
            catch (Exception ex)
            {
                logger.LogDebug("Support Error: " + ex);
                return null;
            }

        }
        public IActionResult Dashboard()
        {
            List<DashboardView> lDashboardView = null;
            string _uType = HttpContext.Session.GetString("UserType");
            if (HttpContext.Session.GetString("UserId") != null && HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Support.ToString())
            {
                try
                {
                    lDashboardView = lIPatientRxRepository.getDashboardForSupport();
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

        public IActionResult Add()
        {
            //List<User> _userlist = lIUserRepository.getUserListByType(3);
            //_userlist = _userlist.Where(r => r.Name != "").ToList();
            //if (_userlist != null && _userlist.Count > 0)
            //{
            //    var ObjList = _userlist.Select(r => new SelectListItem
            //    {

            //        Value = r.UserId.ToString(),
            //        Text = r.Name

            //    });
            //    ViewBag.Providers = new SelectList(ObjList, "Value", "Text");
            //    // ViewBag.Providers = ObjList;
            //}
            return View();
        }
        [HttpPost]
        public IActionResult Add(UserViewModel pUser)
        {
            pUser.Type = ConstantsVar.Support;
            User _user = UserExtension.UserViewModelToUser(pUser);
            string _User = lIUserRepository.InsertUser(_user);
            return RedirectToAction("Index");
        }
        public IActionResult Profile(string id)
        {
            User pUser = lIUserRepository.getUser(id);

            //List<User> _userlist = lIUserRepository.getUserListByType(3);
            //if (_userlist != null && _userlist.Count > 0)
            //{
            //    var ObjList = _userlist.Select(r => new SelectListItem
            //    {
            //        Value = r.UserId.ToString().Trim(),
            //        Text = r.Name
            //    });
            //    ViewBag.Provider = new SelectList(ObjList, "Value", "Text");
            //}
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
            pUser.Type = ConstantsVar.Support;
            User _user = UserExtension.UserViewModelToUser(pUser);
            string _result = lIUserRepository.UpdateUser(_user);
            if (HttpContext.Session.GetString("UserType") != null && HttpContext.Session.GetString("UserType").ToString() == ConstantsVar.Support.ToString())
            {
                return RedirectToAction("Profile", new { id = pUser.UserId });
            }
            return RedirectToAction("Index");
        }


    }
}
