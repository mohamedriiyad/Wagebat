using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Wagebat.Models
{
    public class Question
    {
        public Question()
        {
            QuestionAttachments = new Collection<QuestionAttachment>();
        }
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
        public ICollection<QuestionAttachment> QuestionAttachments { get; set; }
    }
}
