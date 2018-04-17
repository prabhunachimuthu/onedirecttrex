using Microsoft.EntityFrameworkCore;
using OneDirect.Helper;
using OneDirect.Models;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository
{
    public class AppointmentRepository : IAppointmentInterface
    {
        private OneDirectContext context;

        public AppointmentRepository(OneDirectContext context)
        {
            this.context = context;
        }



        public string DeleteAppointment(string AppointmentId)
        {
            try
            {
                var Appointment = (from p in context.Appointments where p.AppointmentId == AppointmentId select p).ToList();
                context.Appointments.RemoveRange(Appointment);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public int InsertAppointment(Appointments pAppointment)
        {
            context.Appointments.Add(pAppointment);
            return context.SaveChanges();
        }

        public int UpdateAppointment(Appointments pAppointment)
        {
            var _Appointment = (from p in context.Appointments
                                where p.AppointmentId == pAppointment.AppointmentId
                                select p).FirstOrDefault();
            if (_Appointment != null)
            {
                context.Entry(_Appointment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return context.SaveChanges();
            }
            else
                return 0;
        }

        public List<AppointmentView> getAppointmentListByTherapsitId(string ltherapistId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientUserId equals _patient.PatientLoginId
                    join _provider in context.User on _patient.ProviderId equals _provider.UserId
                    where p.AppointmentType == AppointmentTypeConstants.Therapist.ToString() && p.TherapistUserId == ltherapistId
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid,
                        Provider = _provider,
                        TotalSession = _patient.Session.Count,
                        PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                    }).OrderByDescending(x => x.ConfirmedSlot).ThenBy(x => x.SlotId).ToList();


        }
        public List<AppointmentView> getSupportAppointmentListByPatientId(string lpatientId, string appintmentType, string SupportId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientUserId equals _patient.PatientLoginId
                    join _provider in context.User on _patient.ProviderId equals _provider.UserId
                    where p.PatientUserId == lpatientId && ((p.SupportUserId == SupportId && p.Status == 0) || p.AppointmentType == AppointmentTypeConstants.Support.ToString())
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid,
                        Provider = _provider,
                        TotalSession = _patient.Session.Count,
                        PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                    }).OrderByDescending(x => x.ConfirmedSlot).ThenBy(x => x.SlotId).ToList();


        }
        public List<AppointmentView> getTherapistAppointmentListByPatientId(string lpatientId, string appintmentType, string TherapistId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientUserId equals _patient.PatientLoginId
                    join _provider in context.User on _patient.ProviderId equals _provider.UserId
                    where p.TherapistUserId == TherapistId && p.PatientUserId == lpatientId && p.AppointmentType == AppointmentTypeConstants.Therapist.ToString()
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid,
                        Provider = _provider,
                        TotalSession = _patient.Session.Count,
                        PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                    }).OrderByDescending(x => x.ConfirmedSlot).ThenBy(x => x.SlotId).ToList();


        }
        public List<AppointmentView> getAppointmentListBySupport(string lsupportId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientUserId equals _patient.PatientLoginId
                    join _provider in context.User on _patient.ProviderId equals _provider.UserId
                    where (p.AppointmentType == AppointmentTypeConstants.Support.ToString() && p.Status == 0) || p.SupportUserId == lsupportId
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid,
                        Provider = _provider,
                        TotalSession = _patient.Session.Count,
                        PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                    }).OrderByDescending(x => x.ConfirmedSlot).ThenBy(x => x.SlotId).ToList();


        }
        public List<AppointmentView> getAppointmentListByPatientId(string lpatientId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientUserId equals _patient.PatientLoginId
                    join _provider in context.User on _patient.ProviderId equals _provider.UserId
                    where p.PatientUserId == lpatientId
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid,
                        Provider = _provider,
                        TotalSession = _patient.Session.Count,
                        PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                    }).OrderByDescending(x => x.ConfirmedSlot).ThenBy(x => x.SlotId).ToList();


        }

        public List<AppointmentView> getAppointmentList()
        {
            return (from p in context.Appointments
                    join _patient in context.Patient.Include(pro => pro.Protocol).ThenInclude(s => s.Session).Include(rx => rx.PatientRx) on p.PatientUserId equals _patient.PatientLoginId
                    join _provider in context.User on _patient.ProviderId equals _provider.UserId
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid,
                        Provider = _provider,
                        TotalSession = _patient.Session.Count,
                        PatientRx = _patient.PatientRx.OrderBy(x => x.RxStartDate).FirstOrDefault()
                    }).OrderByDescending(x => x.ConfirmedSlot).ThenBy(x => x.SlotId).ToList();


        }

        public int updateAppointmentsStatusforPatient(string patientloginId)
        {
            //string lldate = Utilities.GetLocalDateTime(Convert.ToDateTime("2018-02-15 00:00:00").AddHours(11), Utilities.convert(Convert.ToDouble("-300") / 60));
            //DateTime ldate = Convert.ToDateTime(lldate);
            List<Appointments> result = (from p in context.Appointments
                                         where p.PatientUserId == patientloginId
                                         && Convert.ToDateTime(p.ConfirmedSlot).AddHours(Convert.ToInt32(p.SlotId) + 1) < Convert.ToDateTime(Utilities.GetLocalDateTime(DateTime.UtcNow, Utilities.convert(Convert.ToDouble(p.Timezone) / 60))) && p.Status == AppointmentConstants.SlotAccepted
                                         select p).ToList();
            //List<Appointments> result = (from p in context.Appointments
            //                             where p.TherapistUserId == TherapistId && p.AppointmentType == AppointmentTypeConstants.Therapist.ToString()
            //                             && Convert.ToDateTime(p.ConfirmedSlot).AddHours(Convert.ToInt32(p.SlotId) + 1) < DateTime.Now && p.Status == AppointmentConstants.SlotAccepted
            //                             select p).ToList();

            if (result != null && result.Count > 0)
            {
                result.ForEach(x => x.Status = AppointmentConstants.Expired);

                return context.SaveChanges();

            }
            return 0;

        }

        public int updateAppointmentsStatusforTherapist(string TherapistId)
        {
            //string lldate = Utilities.GetLocalDateTime(Convert.ToDateTime("2018-02-15 00:00:00").AddHours(11), Utilities.convert(Convert.ToDouble("-300") / 60));
            //DateTime ldate = Convert.ToDateTime(lldate);
            List<Appointments> result = (from p in context.Appointments
                                         where p.TherapistUserId == TherapistId && p.AppointmentType == AppointmentTypeConstants.Therapist.ToString()
                                         && Convert.ToDateTime(p.ConfirmedSlot).AddHours(Convert.ToInt32(p.SlotId) + 1) < Convert.ToDateTime(Utilities.GetLocalDateTime(DateTime.UtcNow, Utilities.convert(Convert.ToDouble(p.Timezone) / 60))) && p.Status == AppointmentConstants.SlotAccepted
                                         select p).ToList();
            //List<Appointments> result = (from p in context.Appointments
            //                             where p.TherapistUserId == TherapistId && p.AppointmentType == AppointmentTypeConstants.Therapist.ToString()
            //                             && Convert.ToDateTime(p.ConfirmedSlot).AddHours(Convert.ToInt32(p.SlotId) + 1) < DateTime.Now && p.Status == AppointmentConstants.SlotAccepted
            //                             select p).ToList();

            if (result != null && result.Count > 0)
            {
                result.ForEach(x => x.Status = AppointmentConstants.Expired);

                return context.SaveChanges();

            }
            return 0;

        }
        public int updateAppointmentsStatusforSupport(string SupportId)
        {

            List<Appointments> result = (from p in context.Appointments
                                         where p.SupportUserId == SupportId && p.AppointmentType == AppointmentTypeConstants.Support.ToString()
                                         && Convert.ToDateTime(p.ConfirmedSlot).AddHours(Convert.ToInt32(p.SlotId) + 1) < Convert.ToDateTime(Utilities.GetLocalDateTime(DateTime.UtcNow, Utilities.convert(Convert.ToDouble(p.Timezone) / 60))) && p.Status == AppointmentConstants.SlotAccepted
                                         select p).ToList();

            if (result != null && result.Count > 0)
            {
                result.ForEach(x => x.Status = AppointmentConstants.Expired);

                return context.SaveChanges();

            }
            return 0;

        }

        public List<Patient> getAppointmentPatientList()
        {
            return (from p in context.Appointments
                    join _patient in context.Patient on p.PatientUserId equals _patient.PatientLoginId
                    select _patient).Distinct().OrderBy(x => x.PatientName).ToList();
        }
        public List<Patient> getAppointmentPatientListByTherapist(string ltherapistId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient on p.PatientUserId equals _patient.PatientLoginId
                    where p.AppointmentType == AppointmentTypeConstants.Therapist.ToString() && p.TherapistUserId == ltherapistId
                    select _patient).Distinct().OrderBy(x => x.PatientName).ToList();
        }

        public List<Patient> getAppointmentPatientListBySupport(string lsupportId)
        {
            return (from p in context.Appointments
                    join _patient in context.Patient on p.PatientUserId equals _patient.PatientLoginId
                    where (p.AppointmentType == AppointmentTypeConstants.Support.ToString() && p.Status == 0) || p.SupportUserId == lsupportId
                    select _patient).Distinct().OrderBy(x => x.PatientName).ToList();
        }

        public Appointments GetAppointmentbyPatientandTherapistIDandDate(Appointments pAppointment)
        {
            Appointments _Appointment = (from p in context.Appointments
                                         where p.PatientUserId == pAppointment.PatientUserId && p.TherapistUserId == pAppointment.TherapistUserId && p.ConfirmedSlot == pAppointment.ConfirmedSlot
                                         select p).FirstOrDefault();
            return _Appointment;

        }

        public Appointments GetTherapistOpenAppointmentForPatient(string patientId, string therapistId)
        {
            Appointments _Appointment = (from p in context.Appointments
                                         where p.PatientUserId == patientId && p.TherapistUserId == therapistId && p.Status <= AppointmentConstants.SlotAccepted
                                         && (p.ConfirmedSlot == null || (p.ConfirmedSlot != null && Convert.ToDateTime(p.ConfirmedSlot) >= DateTime.UtcNow))
                                         select p).FirstOrDefault();
            return _Appointment;
        }
        public Appointments GetSupportOpenAppointmentForPatient(string patientId)
        {
            Appointments _Appointment = (from p in context.Appointments
                                         where p.PatientUserId == patientId && p.AppointmentType == AppointmentTypeConstants.Support.ToString() && p.Status <= AppointmentConstants.SlotAccepted
                                         && (p.ConfirmedSlot == null || (p.ConfirmedSlot != null && Convert.ToDateTime(p.ConfirmedSlot) >= DateTime.UtcNow))
                                         select p).FirstOrDefault();
            return _Appointment;
        }
        public List<Appointments> GetAppointmentbyPatientId(string patientId)
        {
            List<Appointments> _Appointments = (from p in context.Appointments
                                                where p.PatientUserId == patientId && p.Status < AppointmentConstants.Completed
                                                select p).ToList();
            return _Appointments;

        }

        public List<Appointments> GetAllAppointmentbyPatientId(string patientId)
        {
            List<Appointments> _Appointments = (from p in context.Appointments
                                                where p.PatientUserId == patientId
                                                select p).ToList();
            return _Appointments;

        }

        public Appointments GetAppointmentbyID(string Id)
        {
            Appointments _Appointment = (from p in context.Appointments
                                         where p.AppointmentId == Id
                                         select p).FirstOrDefault();
            return _Appointment;

        }

        public Appointments GetAppointmentbyTherapistIdandSlot(string TherapistId, string slotdate, string slotId)
        {
            Appointments _Appointment = (from p in context.Appointments
                                         where p.TherapistUserId == TherapistId && Convert.ToDateTime(p.ConfirmedSlot) == Convert.ToDateTime(slotdate)
                                         && p.SlotId == slotId
                                         select p).FirstOrDefault();

            return _Appointment;

        }
        public List<string> GetAssignedSlotbyTherapistId(string TherapistId, string slotdate)
        {
            List<string> slots = (from p in context.Appointments
                                  where p.TherapistUserId == TherapistId && Convert.ToDateTime(p.ConfirmedSlot) == Convert.ToDateTime(slotdate)
                                  select p.SlotId).ToList();

            return slots;

        }

        public List<string> GetAssignedSlotbySupportId(string SupportId, string slotdate)
        {
            List<string> slots = (from p in context.Appointments
                                  where p.SupportUserId == SupportId && Convert.ToDateTime(p.ConfirmedSlot) == Convert.ToDateTime(slotdate)
                                  select p.SlotId).ToList();

            return slots;

        }

        public Appointments GetAppointmentbySupportIdandSlot(string SupportId, string slotdate, string slotId)
        {
            Appointments _Appointment = (from p in context.Appointments
                                         where p.SupportUserId == SupportId && Convert.ToDateTime(p.ConfirmedSlot) == Convert.ToDateTime(slotdate)
                                         && p.SlotId == slotId
                                         select p).FirstOrDefault();

            return _Appointment;

        }

        public AppointmentView GetAppointmentViewbyID(string Id)
        {

            return (from p in context.Appointments
                        //join _patient in context.Patient on p.PatientUserId equals _patient.PhoneNumber
                        //join _patientuser in context.User on p.PatientUserId equals _patientuser.UserId
                        //join _therapist in context.User on _patient.Therapistid equals _therapist.UserId
                    where p.AppointmentId == Id
                    select new AppointmentView
                    {
                        AppointmentId = p.AppointmentId,
                        PatientUserId = p.PatientUserId,
                        AppointmentType = p.AppointmentType,
                        TherapistUserId = p.TherapistUserId,
                        SupportUserId = p.SupportUserId,
                        AvailableSlots = p.AvailableSlots,
                        ConfirmedSlot = p.ConfirmedSlot,
                        SlotId = p.SlotId,
                        SlotTime = p.SlotTime,
                        Urikey = p.Urikey,
                        Status = p.Status,
                        Duration = p.Duration,
                        TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                        SupportName = !string.IsNullOrEmpty(p.SupportUserId) ? (from m in context.User where m.UserId == p.SupportUserId select m).FirstOrDefault().Name : "",
                        PatientComment = p.PatientComment,
                        TherapistSupportComment = p.TherapistSupportComment,
                        CreateDate = p.CreateDate,
                        UpdatedDate = p.UpdatedDate,
                        Timezone = p.Timezone,
                        PatientName = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name,
                        PatientVseeId = (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Vseeid

                    }).FirstOrDefault();



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
