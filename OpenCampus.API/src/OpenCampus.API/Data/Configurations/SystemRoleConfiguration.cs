using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class SystemRoleConfiguration : IEntityTypeConfiguration<SystemRole>
{
    public void Configure(EntityTypeBuilder<SystemRole> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id)
            .ValueGeneratedOnAdd();

        builder.Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(role => role.NormalizedName)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(role => role.DisplayName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(role => role.Description)
            .HasMaxLength(512);

        builder.Property(role => role.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.HasIndex(role => role.NormalizedName)
            .IsUnique();
    }
}
