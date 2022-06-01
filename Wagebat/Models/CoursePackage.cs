namespace Wagebat.Models
{
    public class CoursePackage
    {
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
