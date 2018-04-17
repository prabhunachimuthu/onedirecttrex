using OneDirect.Extensions;
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
    public class TransactionLogRepository : ITransactionLogInterface
    {
        private OneDirectContext context;

        public TransactionLogRepository(OneDirectContext context)
        {
            this.context = context;
        }



        public string DeleteTransactionLog(string plogId)
        {
            try
            {
                var log = (from p in context.TransactionLog where p.TransactionId == plogId select p).ToList();
                context.TransactionLog.RemoveRange(log);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return "fail";
            }
            return "success";
        }

        public int InsertTransactionLog(TransactionLog plog)
        {
            context.TransactionLog.Add(plog);
            return context.SaveChanges();
        }

        public int UpdateTransactionLog(TransactionLog plog)
        {
            var _log = (from p in context.TransactionLog
                        where p.TransactionId == plog.TransactionId
                        select p).FirstOrDefault();
            if (_log != null)
            {
                context.Entry(_log).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return context.SaveChanges();
            }
            else
                return 0;
        }





        public TransactionLog GetTransactionLogbyID(string Id)
        {
            TransactionLog _Log = (from p in context.TransactionLog
                                   where p.TransactionId == Id
                                   select p).FirstOrDefault();
            return _Log;

        }

        public List<TransactionLogView> GetTransactionbyuserId(string userId)
        {
            List<TransactionLog> _log = (from p in context.TransactionLog
                                         where p.TherapistUserId == userId
                                         select p).ToList();

            List<TransactionLogView> logView = _log.Select(p => new TransactionLogView()
            {
                TransactionId = p.TransactionId,
                PatientUserId = p.PatientUserId,
                TransactionType = p.TransactionType,
                TherapistUserId = p.TherapistUserId,
                LinkToActivity = p.LinkToActivity,
                Duration = p.Duration,
                Comment = p.Comment,
                CreateDate = p.CreateDate,
                UpdatedDate = p.UpdatedDate,
                TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                UserName = !string.IsNullOrEmpty(p.PatientUserId) ? (from m in context.Patient where m.PatientId == Convert.ToInt32(p.PatientUserId) select m).FirstOrDefault().PatientName : ""
            }).ToList();
            return logView;

        }

        public List<TransactionLogView> GetTransactionLogs()
        {
            List<TransactionLog> _log = (from p in context.TransactionLog
                                         select p).ToList();

            List<TransactionLogView> logView = _log.Select(p => new TransactionLogView()
            {
                TransactionId = p.TransactionId,
                PatientUserId = p.PatientUserId,
                TransactionType = p.TransactionType,
                TherapistUserId = p.TherapistUserId,
                LinkToActivity = p.LinkToActivity,
                Duration = p.Duration,
                Comment = p.Comment,
                CreateDate = p.CreateDate,
                UpdatedDate = p.UpdatedDate,
                TherapistName = !string.IsNullOrEmpty(p.TherapistUserId) ? (from m in context.User where m.UserId == p.TherapistUserId select m).FirstOrDefault().Name : "",
                UserName = !string.IsNullOrEmpty(p.PatientUserId) ? (from m in context.User where m.UserId == p.PatientUserId select m).FirstOrDefault().Name : ""
            }).ToList();
            return logView;

        }

        public List<TransactionLog> GetTransactionbyTherapist(string therapistId)
        {
            List<TransactionLog> _log = (from p in context.TransactionLog
                                         where p.TherapistUserId == therapistId
                                         select p).ToList();
            return _log;

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
