using OneDirect.Models;
using OneDirect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Repository.Interface
{
    interface IMessageInterface : IDisposable
    {
        Messages getMessage(string senderId, string receiverId);
        void InsertMessage(Messages pMessage);
        void UpdateMessage(Messages pMessage);
        List<Messages> getMessagebyMessageId(string messageId);
        List<Messages> getBySenderIdAndReceiverId(string senderId, string receiverId);
        List<PatientMessageView> getPatientMessages(string id);
        List<PatientMessageView> getPatientMessagesforAdmin();
    }
}
