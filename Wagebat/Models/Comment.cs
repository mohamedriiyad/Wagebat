using System;

namespace Wagebat.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TransactionId { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}
