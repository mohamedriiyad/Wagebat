namespace Wagebat.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float PriceBefore { get; set; }
        public float PriceAfter { get; set; }
        public int QuestionsCount { get; set; }
    }
}
