using System.Collections.Generic;

namespace Wagebat.Models
{
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
