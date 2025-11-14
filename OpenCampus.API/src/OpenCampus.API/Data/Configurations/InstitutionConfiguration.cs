using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("Institutions");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Acronym)
            .HasMaxLength(32);

        builder.Property(i => i.WebsiteUrl)
            .HasMaxLength(256);

        builder.Property(i => i.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(i => i.Name)
            .IsUnique();

        builder.HasMany(i => i.Courses)
            .WithOne(c => c.Institution)
            .HasForeignKey(c => c.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Professors)
            .WithOne(p => p.Institution)
            .HasForeignKey(p => p.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Reviews)
            .WithOne(r => r.Institution)
            .HasForeignKey(r => r.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.ChangeRequests)
            .WithOne(cr => cr.Institution)
            .HasForeignKey(cr => cr.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
