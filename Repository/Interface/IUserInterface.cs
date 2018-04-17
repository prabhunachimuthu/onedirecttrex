using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IUserInterface : IDisposable
    {
        User getUserbySessionId(string sessionId);
        User userLogin(string lUserID, string password, int type);
        User getUser(string lUserID, string password, int type);
        User getUser(string lUserID, string password);

        User getUser(string lUserID);

        string getUserdatabyPatientAndtherapist(string lpatientId, string lTherapistId);
        List<User> getUserData(string lUserID);
        List<User> getUserListByType(int lUserType);
        List<NewPatient> getUserListByTherapistId(string lTherapist);
        List<User> getTherapistListByProviderId(string lProvider);

        List<NewPatient> getPatientListByProviderId(string lProvider);

        List<User> getUserListByTypeValue(int lUserType);
        string InsertUser(User pUser);
        string UpdateUser(User pUser);
        User LoginUser(string username, string password);
        EquipmentAssignment getEquUser(string lUserID);

        List<UserViewModel> getUserListByTypeValueqq(int lUserType);
        List<NewPatient> getPatientListByType(int lUserType);
    }
}
