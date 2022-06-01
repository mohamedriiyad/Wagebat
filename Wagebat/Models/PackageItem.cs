namespace Wagebat.Models
{
    public class PackageItem
    {
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
    }
}
