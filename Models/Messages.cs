using System;
using System.Collections.Generic;

namespace OneDirect.Models
{
    public partial class Messages
    {
        public int Id { get; set; }
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public int? ReadStatus { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public string Attachment { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public virtual User Receiver { get; set; }
        public virtual User Sender { get; set; }
    }
}
