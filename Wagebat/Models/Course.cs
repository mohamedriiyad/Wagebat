
namespace Wagebat.Models.Course
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LevelId { get; set; }
        public int UniversityId { get; set; }
    }
}