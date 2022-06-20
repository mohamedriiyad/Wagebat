namespace Wagebat.Models
{
    public class QuestionAttachment
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public bool IsImage { get; set; }
    }
}
