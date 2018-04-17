using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    public interface INewPatient
    {
        NewPatient GetPatientByPatitentLoginId(string patLoginId);
        PatientRx GetPatientRxByPatIdandDeviceConfig(int patId, string deviceConfig);
        string CreateNewPatientByProvider(NewPatientWithProtocol NewPatient);
        List<NewProtocol> GetProtocolListBypatId(string patId);
        NewProtocol GetProtocolByproId(string proId);
        PatientRx GetNewPatientRxByRxId(string Rxid);
        List<PatientRx> GetNewPatientRxByPatId(string Patid);
        NewPatient GetPatientByPatId(int PatId);
        string CreateProtocol(NewProtocol protocol);
        string UpdatePatient(NewPatient NewPatient);
        //string UpdatePatientRx(List<NewPatientRx> NewPatientRxs);
        string UpdatePatientRx(List<NewPatientRx> NewPatientRxs, int PainThreshold = 0, int RateOfChange = 0, string usertype = "");
        string DeleteProtocolRecordsWithCasecade(string proId);
        int ChangeRxCurrent(string RxID, int CurrentFlexion, int CurrentExtension, string Code);
        int ChangeRxCurrentFlexion(string RxID, int CurrentFlexion, string Code);
        int ChangeRxCurrentExtension(string RxID, int CurrentExtension, string Code);
    }
}
