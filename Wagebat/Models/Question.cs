using System;
using System.Collections.Generic;

namespace Wagebat.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SubscriptionId { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public ICollection<QuestionAttachment> QuestionAttachments { get; set; }
    }
}
