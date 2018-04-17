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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace OneDirect.Controllers
{
    [LoginAuthorizeAttribute]
    public class PatientController : Controller
    {
        private readonly IUserInterface lIUserRepository;
        private readonly ILogger logger;
        private OneDirectContext context;

        public PatientController(OneDirectContext context, ILogger<PatientController> plogger)
        {
            logger = plogger;
            this.context = context;
            lIUserRepository = new UserRepository(context);
        }


        // GET: /<controller>/
        public IActionResult Index(string provider = "", string ptype = "", string providername = "")
        {
            var response = new Dictionary<string, object>();
            try
            {
                if (!String.IsNullOrEmpty(ptype) && ptype == "3")
                {
                    ViewBag.ProviderType = ptype;
                    ViewBag.ProviderName = providername;
                }
                string _uType = HttpContext.Session.GetString("UserType");
                if (!String.IsNullOrEmpty(provider))
                {
                    HttpContext.Session.SetString("UserId", provider);
                    _uType = ptype;
                }
                List<NewPatient> pUsera = null;
                List<User> pUser = new List<Models.User>();
                logger.LogDebug("Pain Post Start");

                if (_uType != "2" && _uType != "3")
                    pUsera = lIUserRepository.getPatientListByType(1);
                else if (_uType == "3")
                {
                    pUsera = lIUserRepository.getPatientListByProviderId(HttpContext.Session.GetString("UserId"));
                    // pUsera = pUser.GroupBy(u => u.UserId, (key, group) => group.First()).ToList();
                    // pUsera= UserExtension.UserToUserViewModelList(pUser);
                }
                else
                {
                    pUsera = lIUserRepository.getUserListByTherapistId(HttpContext.Session.GetString("UserId"));
                    // pUser = pUser.GroupBy(u => u.UserId, (key, group) => group.First()).ToList();
                    // pUsera = UserExtension.UserToUserViewModelList(pUser);
                }

                //if (pUser != null && pUser.Count > 0)
                //{
                //    foreach (User user in pUser)
                //    {
                //        if (user.Type != 3)
                //        {
                //            User _provider = lIUserRepository.getUser(user.ProviderId);
                //            if (_provider != null)
                //            {
                //                user.ProviderId = _provider.Name;
                //            }
                //        }
                //    }
                //}

                //  List<UserViewModel>  _user = UserExtension.UserToUserViewModelList(pUser);

                return View(pUsera);
            }
            catch (Exception ex)
            {
                logger.LogDebug("User Post Error: " + ex);
                return null;
            }
        }
        public IActionResult Add()
        {
            //List<User> _userTherapistlist = lIUserRepository.getUserListByType(2);

            //var ObjTherapistList = _userTherapistlist.Select(r => new SelectListItem
            //{
            //    Value = r.UserId.ToString(),
            //    Text = r.Name
            //});

            //ViewBag.ddTherapist = new SelectList(ObjTherapistList, "Value", "Text");
            //ViewBag.ddTherapist = ObjTherapistList;

            List<User> _userProviderlist = lIUserRepository.getUserListByType(3);

            var ObjProviderList = _userProviderlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });

            ViewBag.ddProvider = new SelectList(ObjProviderList, "Value", "Text");
            ViewBag.ddProvider = ObjProviderList;

            return View();
        }
        [HttpPost]
        public IActionResult Add(UserViewModel pUser)
        {
            pUser.Type = 1;
            User _user = UserExtension.UserViewModelToUser(pUser);
            string _User = lIUserRepository.InsertUser(_user);
            return RedirectToAction("Index");
        }
        public IActionResult Profile(string id)
        {
            User pUser = lIUserRepository.getUser(id);

            List<User> _userProviderlist = lIUserRepository.getUserListByType(3);

            var ObjProviderList = _userProviderlist.Select(r => new SelectListItem
            {
                Value = r.UserId.ToString(),
                Text = r.Name
            });

            ViewBag.ddProvider = new SelectList(ObjProviderList, "Value", "Text");
            ViewBag.ddProvider = ObjProviderList;
            UserViewModel _user = UserExtension.UserToUserViewModel(pUser);
            return View(_user);
        }
        [HttpPost]
        public IActionResult Profile(UserViewModel pUser)
        {
            pUser.Type = 1;
            User _user = UserExtension.UserViewModelToUser(pUser);
            string _result = lIUserRepository.UpdateUser(_user);
            return RedirectToAction("Index");
        }
    }
}