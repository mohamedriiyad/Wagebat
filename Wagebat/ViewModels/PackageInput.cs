using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wagebat.ViewModels
{
    public class PackageInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float PriceBefore { get; set; }
        public float PriceAfter { get; set; }
        public int QuestionsCount { get; set; }
        public List<int> WithItemsIds { get; set; }
        public List<int> WithoutItemsIds { get; set; }
        public List<int> CoursesIds { get; set; }
    }
}
