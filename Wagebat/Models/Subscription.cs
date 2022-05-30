using System;

namespace Wagebat.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public int StatusId { get; set; }
        public DateTime Date { get; set; }
        public string ConfirmedBy { get; set; }
    }
}
