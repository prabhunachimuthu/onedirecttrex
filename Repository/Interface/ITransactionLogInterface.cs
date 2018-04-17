using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface ITransactionLogInterface : IDisposable
    {

        int InsertTransactionLog(TransactionLog plog);
        int UpdateTransactionLog(TransactionLog plog);
        string DeleteTransactionLog(string pLogId);
        TransactionLog GetTransactionLogbyID(string Id);
        List<TransactionLogView> GetTransactionbyuserId(string userId);
        List<TransactionLog> GetTransactionbyTherapist(string therapistId);
        List<TransactionLogView> GetTransactionLogs();
    }
}
