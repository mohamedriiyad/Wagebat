using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wagebat.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<Subscription> Confirmations { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}
