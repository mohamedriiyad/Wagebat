namespace Wagebat.Models
{
    public class InstructorCourse
    {
        public string InstuctorId { get; set; }
        public ApplicationUser Instructor { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
