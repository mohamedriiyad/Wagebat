
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Wagebat.Models
{
    public class Course
    {
        public Course()
        {
            CategoryCourses = new Collection<CategoryCourse>();
            CoursePackages = new Collection<CoursePackage>();
            Instructors = new Collection<ApplicationUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public int UniversityId { get; set; }
        public University University { get; set; }
        public ICollection<CategoryCourse> CategoryCourses { get; set; }
        public ICollection<CoursePackage> CoursePackages { get; set; }
        public ICollection<ApplicationUser> Instructors { get; set; }
    }
}