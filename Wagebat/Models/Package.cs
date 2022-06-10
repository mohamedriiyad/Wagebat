using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wagebat.Models
{
    public class Package
    {
        public Package()
        {
            PackageItems = new Collection<PackageItem>();
            CoursePackages = new Collection<CoursePackage>();
            Subscriptions = new Collection<Subscription>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float PriceBefore { get; set; }
        public float PriceAfter { get; set; }
        public int QuestionsCount { get; set; }
        public ICollection<CoursePackage> CoursePackages { get; set; }
        public ICollection<PackageItem> PackageItems { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
