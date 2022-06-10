using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wagebat.ViewModels
{
    public class CourseInput
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int LevelId { get; set; }
        public int UniversityId { get; set; }
        public List<int> CategoriesIds { get; set; }
    }
}
