using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(c => new { c.InstitutionId, c.Name })
            .IsUnique();

        builder.HasOne(c => c.Institution)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Subjects)
            .WithOne(s => s.Course)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Reviews)
            .WithOne(r => r.Course)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
