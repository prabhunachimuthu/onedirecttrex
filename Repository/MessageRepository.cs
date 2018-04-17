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
    public class MessageRepository : IMessageInterface
    {
        private OneDirectContext context;

        public MessageRepository(OneDirectContext context)
        {
            this.context = context;
        }
        public Messages getMessage(string senderId, string receiverId)
        {
            return (from p in context.Messages
                    where p.SenderId == senderId && p.ReceiverId == receiverId
                    select p).FirstOrDefault();
        }
        public List<Messages> getMessagebyMessageId(string messageId)
        {
            return (from p in context.Messages
                    join sender in context.User on p.SenderId equals sender.UserId
                    join receiver in context.User on p.SenderId equals receiver.UserId
                    where p.MessageId == messageId
                    select new Messages
                    {
                        Id = p.Id,
                        MessageId = p.MessageId,
                        SenderId = p.SenderId,
                        ReceiverId = p.ReceiverId,
                        ReadStatus = p.ReadStatus,
                        Subject = p.Subject,
                        BodyText = p.BodyText,
                        Attachment = p.Attachment,
                        DateCreated = p.DateCreated,
                        DateModified = p.DateModified,
                        Receiver = receiver,
                        Sender = sender,
                    }).ToList();
            //return (from p in context.Messages.Include(x => x.Sender).Include(x => x.Receiver)
            //        where p.MessageId == messageId
            //        select p).ToList();

        }
        public List<PatientMessageView> getPatientMessages(string id)
        {
            List<PatientMessageView> messages=(from p in context.Patient
                    .Where(p => p.ProviderId == id)
                    select new PatientMessageView
                    {
                        Patient = (from m in context.User where m.UserId == p.PatientLoginId select m).FirstOrDefault(),
                        ReceiveMessage = (from m in context.Messages where m.SenderId == p.PatientId.ToString() && m.ReceiverId == id select m).Count(),
                        SentMessage = (from m in context.Messages where m.SenderId == id && m.ReceiverId == p.PatientId.ToString() select m).Count(),
                        TotalUnreadMessage = (from m in context.Messages where m.SenderId == p.PatientId.ToString() && m.ReceiverId == id && m.ReadStatus != 2 select m).Count(),
                        LastMessageDate = (from m in context.Messages where m.SenderId == id || m.ReceiverId == id select m).OrderByDescending(m => m.DateCreated).FirstOrDefault().DateCreated
                    }).ToList();

            return messages;
            //return null;
        }
        public List<PatientMessageView> getPatientMessagesforAdmin()
        {
            return (from p in context.User
                    .Where(p => p.Type == 5)
                    select new PatientMessageView
                    {
                        Patient = p,
                        ReceiveMessage = (from m in context.Messages where m.SenderId == p.UserId && m.ReceiverId == ConfigVars.NewInstance.AdminUserName select m).Count(),
                        SentMessage = (from m in context.Messages where m.SenderId == ConfigVars.NewInstance.AdminUserName && m.ReceiverId == p.UserId select m).Count(),
                        TotalUnreadMessage = (from m in context.Messages where m.SenderId == p.UserId && m.ReceiverId == ConfigVars.NewInstance.AdminUserName && m.ReadStatus != 2 select m).Count(),
                        LastMessageDate = (from m in context.Messages where m.SenderId == ConfigVars.NewInstance.AdminUserName || m.ReceiverId == ConfigVars.NewInstance.AdminUserName select m).OrderByDescending(m => m.DateCreated).FirstOrDefault().DateCreated
                    }).ToList();
        }
        public List<Messages> getBySenderIdAndReceiverId(string senderId, string receiverId)
        {
            return (from p in context.Messages.Include(x => x.Sender).Include(x => x.Receiver)
                    where (p.SenderId == senderId && p.ReceiverId == receiverId) || (p.SenderId == receiverId && p.ReceiverId == senderId)
                    select p).OrderBy(p => p.DateCreated).ToList();
        }

        public void InsertMessage(Messages pMessage)
        {
            context.Messages.Add(pMessage);
            context.SaveChanges();
        }

        public void UpdateMessage(Messages pMessage)
        {
            var _message = (from p in context.Messages
                            where p.Id == pMessage.Id
                            select p).FirstOrDefault();
            if (_message != null)
            {
                context.Entry(_message).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
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
