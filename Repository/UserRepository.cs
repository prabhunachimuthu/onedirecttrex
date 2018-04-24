using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.Response;
using OneDirect.ViewModels;
using OneDirect.Vsee;
using OneDirect.VSee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class UserRepository : IUserInterface
    {
        private OneDirectContext context;

        public UserRepository(OneDirectContext context)
        {
            this.context = context;
        }

        public User getUser(string lUserID, string password)
        {
            return (from p in context.User
                    where p.UserId == lUserID && p.Password == password
                    select p).FirstOrDefault();
        }

        public User userLogin(string lUserID, string password, int type)
        {
            User luser = (from p in context.User
                          where p.UserId == lUserID && p.Password == password && p.Type == type
                          select p).FirstOrDefault();
            if (luser != null)
            {
                luser.LoginSessionId = Guid.NewGuid().ToString();
                context.Entry(luser).State = EntityState.Modified;
                int result = context.SaveChanges();
                if (result > 0)
                {
                    return luser;
                }
            }
            return null;
        }
        public User getUserbySessionId(string sessionId)
        {
            return (from p in context.User
                    where p.LoginSessionId == sessionId
                    select p).FirstOrDefault();
        }
        public User getUser(string lUserID, string password, int type)
        {
            return (from p in context.User
                    where p.UserId == lUserID && p.Password == password && p.Type == type
                    select p).FirstOrDefault();
        }

        public User getUser(string lUserID)
        {
            return (from p in context.User
                    where p.UserId == lUserID
                    select p).FirstOrDefault();
        }
        public EquipmentAssignment getEquUser(string lUserID)
        {
            return (from p in context.EquipmentAssignment
                    where p.PatientId == Convert.ToInt32(lUserID) && p.InstallerId == "th2"
                    select p).FirstOrDefault();
        }
        public List<User> getUserData(string lUserID)
        {
            return (from p in context.User
                    where p.UserId == lUserID
                    select p).ToList();
        }

        public List<User> getUserListByType(int lUserType)
        {
            return (from p in context.User
                    where p.Type == lUserType
                    select p).ToList();
        }
        public List<User> getUserListByTypeValue(int lUserType)
        {
            //return (from p in context.User
            //        join _provider in context.User on p.ProviderId equals _provider.UserId
            //        where p.Type == lUserType
            //        select new User
            //        {
            //            UserId = p.UserId,
            //            Type = p.Type,
            //            Name = p.Name,
            //            Email = p.Email,
            //            Phone = p.Phone,
            //            ProviderId = p.Type != 3 ? _provider.Name : "",
            //            Address = p.Address,
            //            Password = p.Password
            //        }).ToList();
            return null;

        }

        public List<UserViewModel> getUserListByTypeValueqq(int lUserType)
        {
            //return (from p in context.User
            //        join _provider in context.User on p.ProviderId equals _provider.UserId
            //        join _equip in context.EquipmentAssignment on p.UserId equals _equip.InstallerId
            //        join _therapist in context.User on _equip.InstallerId equals _therapist.UserId

            //        where p.Type == lUserType
            //        select new UserViewModel
            //        {
            //            UserId = p.UserId,
            //            Type = p.Type,
            //            Name = p.Name,
            //            Email = p.Email,
            //            Phone = p.Phone,
            //            ProviderId = p.Type != 3 ? _provider.Name : "",
            //            Address = p.Address,
            //            Password = p.Password,
            //            Therapist = _therapist.Name
            //        }).ToList();
            return null;

        }

        public List<NewPatient> getPatientListByType(int lUserType)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    // where p.Type == lUserType
                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Name,
                        TherapistId = p.Therapistid,
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,
                        EquipmentType = p.EquipmentType
                    }).ToList();


        }
        public List<NewPatient> getUserListByTherapistId(string lTherapist)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    where p.Therapistid == lTherapist
                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Name,
                        TherapistId = p.Therapistid,
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,
                        EquipmentType = p.EquipmentType
                    }).ToList();

        }

        public List<User> getTherapistListByProviderId(string lProvider)
        {
            //return (from _user in context.User
            //        join _provider in context.User on _user.ProviderId equals _provider.UserId
            //        where _user.ProviderId == lProvider && _user.Type == 2
            //        select new User
            //        {
            //            UserId = _user.UserId,
            //            Type = _user.Type,
            //            Name = _user.Name,
            //            Email = _user.Email,
            //            Phone = _user.Phone,
            //            ProviderId = _user.Type != 3 ? _provider.Name : "",
            //            Address = _user.Address,
            //            Password = _user.Password
            //        }).Distinct().ToList();
            return null;
        }
        public List<NewPatient> getPatientListByProviderId(string lProvider)
        {
            return (from p in context.Patient
                    join _provider in context.User on p.ProviderId equals _provider.UserId
                    where p.ProviderId == lProvider
                    select new NewPatient
                    {
                        PatientLoginId = p.PatientLoginId,
                        PatientId = p.PatientId,
                        PatientName = p.PatientName,
                        PhoneNumber = p.PhoneNumber,
                        ProviderId = _provider.Type != 3 ? _provider.Name : "",
                        AddressLine = p.AddressLine,
                        Dob = p.Dob,
                        City = p.City,
                        State = p.State,
                        SurgeryDate = p.SurgeryDate,
                        Ssn = p.Ssn,
                        Side = p.Side,
                        EquipmentType = p.EquipmentType
                    }).ToList();
        }

        public string InsertUser(User pUser)
        {
            string result = string.Empty;
            User _user = null;

            //if (pUser.Type == 1 || pUser.Type == 2)
            //    _user = (from p in context.User
            //             where p.UserId == pUser.UserId && p.ProviderId == pUser.ProviderId
            //             select p).FirstOrDefault();

            //if (pUser.Type == 3)
            _user = (from p in context.User
                     where p.UserId == pUser.UserId
                     select p).FirstOrDefault();
            if (_user == null)
            {
                if (!string.IsNullOrEmpty(pUser.Password) && (pUser.Type == ConstantsVar.Patient || pUser.Type == ConstantsVar.Therapist || pUser.Type == ConstantsVar.Support))
                {
                    AddUser luser = new AddUser();
                    luser.secretkey = ConfigVars.NewInstance.secretkey;
                    luser.username = pUser.UserId;
                    luser.fn = pUser.Name;
                    luser.ln = pUser.Name;
                    luser.password = pUser.Password;

                    VSeeHelper lhelper = new VSeeHelper();
                    var resUser = lhelper.AddUser(luser);
                    if (resUser != null && resUser["status"] == "success")
                    {
                        pUser.Vseeid = "onedirect+" + pUser.UserId.ToLower();
                        context.User.Add(pUser);
                        context.SaveChanges();
                        result = "success";
                    }
                }
                else
                {
                    context.User.Add(pUser);
                    context.SaveChanges();
                    result = "success";
                }
                //Prabhu

            }
            else
            {
                result = "Username already exists";
            }

            return result;
        }
        public string getUserdatabyPatientAndtherapist(string lpatientId, string lTherapistId)
        {
            string result = string.Empty;
            JsonUserData _user = new JsonUserData();
            _user.Users = new List<User>();
            try
            {
                var _patient = (from p in context.User
                                where p.UserId == lpatientId
                                select p).FirstOrDefault();
                var _therapist = (from p in context.User
                                  where p.UserId == lTherapistId
                                  select p).FirstOrDefault();
                //var _provider = (from p in context.User
                //                 where p.UserId == _patient.ProviderId
                //                 select p).FirstOrDefault();
                if (_patient != null)
                    _user.Users.Add(_patient);
                if (_therapist != null)
                    _user.Users.Add(_therapist);
                //if (_provider != null)
                //    _user.Users.Add(_provider);
                _user.result = "success";
            }
            catch (Exception ex)
            {
                _user.result = "failed";
            }
            return JsonConvert.SerializeObject(_user);
        }

        public string UpdateUser(User pUser)
        {
            string result = string.Empty;
            var _user = (from p in context.User
                         where p.UserId == pUser.UserId
                         select p).FirstOrDefault();
            if (_user != null)
            {
                dynamic resUser = null;
                VSeeHelper lhelper = new VSeeHelper();

                AddUser luser = new AddUser();
                luser.secretkey = ConfigVars.NewInstance.secretkey;
                luser.username = pUser.UserId;
                luser.fn = pUser.Name;
                luser.ln = pUser.Name;
                luser.password = pUser.Password;

                if (!string.IsNullOrEmpty(pUser.Password) && !string.IsNullOrEmpty(pUser.Vseeid) && (pUser.Type == ConstantsVar.Patient || pUser.Type == ConstantsVar.Therapist || pUser.Type == ConstantsVar.Support))
                {
                    resUser = lhelper.UpdateUser(luser);
                }
                else if (!string.IsNullOrEmpty(pUser.Password) && string.IsNullOrEmpty(pUser.Vseeid) && (pUser.Type == ConstantsVar.Patient || pUser.Type == ConstantsVar.Therapist || pUser.Type == ConstantsVar.Support))
                {
                    resUser = lhelper.AddUser(luser);
                }

                if (resUser != null && resUser["status"] == "success")
                {
                    _user.Vseeid = "onedirect+" + pUser.UserId.ToLower();
                }

                _user.Name = pUser.Name;
                _user.Email = pUser.Email;
                _user.Address = pUser.Address;
                _user.Phone = pUser.Phone;
                _user.Password = pUser.Password;
                _user.Type = pUser.Type;
                _user.Npi = pUser.Npi;

                context.Entry(_user).State = EntityState.Modified;
                context.SaveChanges();
                result = "success";





                //Prabhu
                //AddUser luser = new AddUser();
                //luser.secretkey = ConfigVars.NewInstance.secretkey;
                //luser.username = pUser.Name;
                //luser.fn = pUser.Name;
                //luser.ln = pUser.Name;
                //luser.password = pUser.Password;
                //VSeeHelper lhelper = new VSeeHelper();
                //var resUser = lhelper.AddUser(luser);
                //if (pUser.Type == 1)
                //{
                //    var _eqAssign = (from p in context.EquipmentAssignment
                //                   where p.PatientId == pUser.UserId 
                //                   select p).FirstOrDefault();
                //    //EquipmentAssignment _eqAssign = new EquipmentAssignment();
                //    _eqAssign.PatientId = pUser.UserId;
                //    _eqAssign.TherapistId = pUser.Therapist;
                //    _eqAssign.DateTime = DateTime.Now;
                //    _eqAssign.EquipmentId = null;
                //    _eqAssign.EquipmentType = null;
                //  //  context.EquipmentAssignment.Add(_eqAssign);
                //    context.EquipmentAssignment.Add(_eqAssign).State = EntityState.Modified;
                //    context.SaveChanges();

                // }
            }

            return result;
        }

        public User LoginUser(string username, string password)
        {
            return (from p in context.User
                    where p.UserId == username && p.Password == password
                    select p).FirstOrDefault();
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
