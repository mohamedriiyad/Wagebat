
using System.Collections.Generic;

namespace Wagebat.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public int UniversityId { get; set; }
        public University University { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Package> Packages { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}