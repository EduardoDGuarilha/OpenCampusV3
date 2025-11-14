using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professors");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Bio)
            .HasMaxLength(2000);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(p => new { p.InstitutionId, p.FullName })
            .IsUnique();

        builder.HasOne(p => p.Institution)
            .WithMany(i => i.Professors)
            .HasForeignKey(p => p.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Professor)
            .HasForeignKey(r => r.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
