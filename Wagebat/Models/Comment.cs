using System;

namespace Wagebat.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
    }
}
