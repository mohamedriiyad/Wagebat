using System.Collections.Generic;

namespace Wagebat.Models
{
    public class Level
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}
