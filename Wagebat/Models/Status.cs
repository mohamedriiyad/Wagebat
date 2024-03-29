﻿using System.Collections.Generic;

namespace Wagebat.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
