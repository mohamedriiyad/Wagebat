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
            builder.Entity<Course>()
              .HasMany(left => left.Categories)
              .WithMany(right => right.Courses)
              .UsingEntity<CategoryCourse>(
                cc => cc.HasOne(prop => prop.Category).WithMany().HasForeignKey(prop => prop.CategoryId),
                cc => cc.HasOne(prop => prop.Course).WithMany().HasForeignKey(prop => prop.CourseId),
                cc =>
                    {
                        cc.HasKey(prop => new { prop.CourseId, prop.CategoryId });
                    }
                );

            builder.Entity<Course>()
                .HasMany(left => left.Packages)
                .WithMany(right => right.Courses)
                .UsingEntity<CoursePackage>(
                    cp => cp.HasOne(prop => prop.Package).WithMany().HasForeignKey(prop => prop.PackageId),
                    cp => cp.HasOne(prop => prop.Course).WithMany().HasForeignKey(prop => prop.CourseId),
                    cp =>
                    {
                        cp.HasKey(prop => new { prop.CourseId, prop.PackageId });
                    }
                );

            builder.Entity<Package>()
                .HasMany(left => left.Items)
                .WithMany(right => right.Packages)
                .UsingEntity<PackageItem>(
                    pi => pi.HasOne(prop => prop.Item).WithMany().HasForeignKey(prop => prop.ItemId),
                    pi => pi.HasOne(prop => prop.Package).WithMany().HasForeignKey(prop => prop.PackageId),
                    pi =>
                    {
                        pi.HasKey(prop => new { prop.PackageId, prop.ItemId });
                    }
                );

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
                .HasMany(left => left.Users)
                .WithMany(right => right.Courses)
                .UsingEntity<InstructorCourse>(
                    ic => ic.HasOne(prop => prop.Instructor).WithMany().HasForeignKey(prop => prop.InstuctorId),
                    ic => ic.HasOne(prop => prop.Course).WithMany().HasForeignKey(prop => prop.CourseId),
                    ic =>
                    {
                        ic.HasKey(prop => new { prop.InstuctorId, prop.CourseId });
                    }
                );
        }
    }
}
