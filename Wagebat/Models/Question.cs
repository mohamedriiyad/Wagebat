using System;

namespace Wagebat.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SubscriptionId { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}
