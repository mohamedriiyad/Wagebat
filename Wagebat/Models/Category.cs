using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wagebat.Models
{
    public class Category
    {
        public Category()
        {
            CategoryCourses = new Collection<CategoryCourse>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CategoryCourse> CategoryCourses { get; set; }
    }
}
