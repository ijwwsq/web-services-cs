using Microsoft.EntityFrameworkCore;
using UniversityApi.Models;

namespace UniversityApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<Teacher>()
            .HasIndex(t => t.Email)
            .IsUnique();

        modelBuilder.Entity<Course>(course =>
        {
            course.HasKey(c => c.Id);

            course.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            course.Property(c => c.Description)
                .HasMaxLength(1000);

            course.Property(c => c.Credits)
                .IsRequired();

            course.HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(enrollment =>
        {
            enrollment.HasKey(e => e.Id);

            enrollment.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            enrollment.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            enrollment.HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();
        });
    }
}
