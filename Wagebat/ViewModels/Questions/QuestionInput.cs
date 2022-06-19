using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Wagebat.Models;

namespace Wagebat.ViewModels.Questions
{
    public class QuestionInput
    {
        public int Id { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
}
