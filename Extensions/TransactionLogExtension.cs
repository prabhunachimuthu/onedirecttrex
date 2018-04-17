using OneDirect.Models;
using OneDirect.Repository;
using OneDirect.Repository.Interface;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Extensions
{
    public class TransactionLogExtension
    {
        
        public static TransactionLogView TransactionLogToTransactionLogView(TransactionLog llog)
        {
            if (llog == null)
                return null;
            TransactionLogView plog = new TransactionLogView()
            {
                TransactionId = llog.TransactionId,
                PatientUserId = llog.PatientUserId,
                TransactionType = llog.TransactionType,
                TherapistUserId = llog.TherapistUserId,
                LinkToActivity = llog.LinkToActivity,
                Duration = llog.Duration,
                Comment = llog.Comment,
                CreateDate = llog.CreateDate,
                UpdatedDate = llog.UpdatedDate
            };
            return plog;
        }

        public static TransactionLog TransactionLogViewToTransactionLog(TransactionLogView llog)
        {
            if (llog == null)
                return null;
            TransactionLog plog = new TransactionLog()
            {
                TransactionId = llog.TransactionId,
                PatientUserId = llog.PatientUserId,
                TransactionType = llog.TransactionType,
                TherapistUserId = llog.TherapistUserId,
                LinkToActivity = llog.LinkToActivity,
                Duration = llog.Duration,
                Comment = llog.Comment,
                CreateDate = llog.CreateDate,
                UpdatedDate = llog.UpdatedDate
            };
            return plog;
        }
    }
}
