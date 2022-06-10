using Wagebat.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wagebat.ViewModels.Users
{
    public class UserSelfEditVM : UserEditVM
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

    }
}
