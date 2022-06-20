namespace Wagebat.Models
{
    public class TransactionAttachment
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string Path { get; set; }
        public bool IsImage { get; set; }
    }
}
