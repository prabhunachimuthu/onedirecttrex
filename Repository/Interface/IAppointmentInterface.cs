using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IAppointmentInterface : IDisposable
    {

        int InsertAppointment(Appointments pAppointment);
        int UpdateAppointment(Appointments pAppointment);
        string DeleteAppointment(string AppointmentId);
        Appointments GetAppointmentbyPatientandTherapistIDandDate(Appointments pAppointment);
        List<AppointmentView> getAppointmentListByTherapsitId(string ltherapistId);
        Appointments GetAppointmentbyID(string Id);
        AppointmentView GetAppointmentViewbyID(string Id);
        Appointments GetAppointmentbyTherapistIdandSlot(string TherapistId, string slotdate, string slotId);
        Appointments GetAppointmentbySupportIdandSlot(string SupportId, string slotdate, string slotId);
        List<AppointmentView> getAppointmentListBySupport(string ltherapistId);
        List<Appointments> GetAppointmentbyPatientId(string patientId);
        List<string> GetAssignedSlotbyTherapistId(string TherapistId, string slotdate);
        List<string> GetAssignedSlotbySupportId(string SupportId, string slotdate);
        List<Patient> getAppointmentPatientListByTherapist(string ltherapistId);
        List<Patient> getAppointmentPatientListBySupport(string lsupportId);
        List<AppointmentView> getSupportAppointmentListByPatientId(string lpatientId, string appintmentType, string SupportId);
        List<AppointmentView> getTherapistAppointmentListByPatientId(string lpatientId, string appintmentType, string TherapistId);
        int updateAppointmentsStatusforTherapist(string TherapistId);
        int updateAppointmentsStatusforSupport(string SupportId);
        Appointments GetTherapistOpenAppointmentForPatient(string patientId, string therapistId);
        Appointments GetSupportOpenAppointmentForPatient(string patientId);
        int updateAppointmentsStatusforPatient(string patientLoginId);
        List<AppointmentView> getAppointmentListByPatientId(string lpatientId);
        List<Patient> getAppointmentPatientList();
        List<AppointmentView> getAppointmentList();
        List<Appointments> GetAllAppointmentbyPatientId(string patientId);
    }
}
