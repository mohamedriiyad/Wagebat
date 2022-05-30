using System;

namespace Wagebat.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TransactionId { get; set; }
        public byte Liked { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
    }
}
