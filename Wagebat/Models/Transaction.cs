using System;
using System.Collections.Generic;

namespace Wagebat.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public string AcceptedBy { get; set; }
        public ApplicationUser Acceptor { get; set; }
        public string Answer { get; set; }
        public DateTime AnswerDate { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<TransactionAttachment> TransactionAttachments { get; set; }
    }
}
