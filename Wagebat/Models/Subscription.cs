using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wagebat.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        
        [Required]
        public int PackageId { get; set; }
        public Package Package { get; set; }
        
        [Required]
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime Date { get; set; }
        public string ConfirmerId { get; set; }
        public ApplicationUser Confirmer { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
