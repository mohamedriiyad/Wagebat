using System.Collections.Generic;

namespace Wagebat.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PackageItem> PackageItems { get; set; }
    }
}
