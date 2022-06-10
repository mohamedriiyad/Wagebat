using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wagebat.ViewModels.Users
{
    public class InstructorCourseInput
    {
        [Required]
        public List<int> CoursesIds { get; set; }
    }
}
