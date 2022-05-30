using System;

namespace Wagebat.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string AcceptedBy { get; set; }
        public string Answer { get; set; }
        public DateTime AnswerDate { get; set; }
        public int StatusId { get; set; }
    }
}
