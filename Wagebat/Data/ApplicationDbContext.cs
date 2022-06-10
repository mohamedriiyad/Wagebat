using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Wagebat.Models;

namespace Wagebat.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryCourse> CategoryCourses { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<University> Universities { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<CoursePackage> CoursePackages { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PackageItem> PackageItems { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<QuestionAttachment> QuestionAttachments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<InstructorCourse> InstructorCourses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TransactionAttachment> TransactionAttachments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CategoryCourse>()
                .HasKey(prop => new { prop.CourseId, prop.CategoryId });
            builder.Entity<CategoryCourse>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CategoryCourses);
            builder.Entity<CategoryCourse>()
                .HasOne(cs => cs.Category)
                .WithMany(c => c.CategoryCourses);


            builder.Entity<CoursePackage>()
                .HasKey(prop => new { prop.CourseId, prop.PackageId });
            builder.Entity<CoursePackage>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.CoursePackages);
            builder.Entity<CoursePackage>()
                .HasOne(cs => cs.Package)
                .WithMany(c => c.CoursePackages);


            builder.Entity<PackageItem>()
                .HasKey(pi => new { pi.PackageId, pi.ItemId, pi.IsWith });

            builder.Entity<PackageItem>()
                .HasOne(left => left.Item)
                .WithMany(right => right.PackageItems)
                .HasForeignKey(pi => pi.ItemId);
            builder.Entity<PackageItem>()
                .HasOne(left => left.Package)
                .WithMany(right => right.PackageItems)
                .HasForeignKey(pi => pi.PackageId);

            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId);

            builder.Entity<Subscription>()
                .HasOne(s => s.Confirmer)
                .WithMany(u => u.Confirmations)
                .HasForeignKey(s => s.ConfirmerId);


            builder.Entity<Transaction>()
                .HasOne(t => t.Acceptor)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.AcceptedBy);

            builder.Entity<Transaction>()
                .HasOne(t => t.Status)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Course>()
                .HasMany(left => left.Instructors)
                .WithMany(right => right.Courses)
                .UsingEntity<InstructorCourse>(
                    ic => ic.HasOne(prop => prop.Instructor).WithMany().HasForeignKey(prop => prop.InstuctorId),
                    ic => ic.HasOne(prop => prop.Course).WithMany().HasForeignKey(prop => prop.CourseId),
                    ic =>
                    {
                        ic.HasKey(prop => new { prop.InstuctorId, prop.CourseId });
                    }
                );

            builder.Entity<Question>()
                .HasOne(q => q.Subscription)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SubscriptionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
