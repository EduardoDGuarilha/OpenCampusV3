using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.StudentEmail)
            .HasMaxLength(256);

        builder.Property(u => u.Cpf)
            .HasMaxLength(14);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        var roleConverter = new ValueConverter<UserRole, string>(
            role => role.ToString().ToUpperInvariant(),
            value => Enum.Parse<UserRole>(value, true));

        builder.Property(u => u.Role)
            .HasConversion(roleConverter)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasMany(u => u.ReviewsAuthored)
            .WithOne(r => r.Author)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.CommentsAuthored)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ChangeRequestsCreated)
            .WithOne(cr => cr.CreatedBy)
            .HasForeignKey(cr => cr.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ChangeRequestsResolved)
            .WithOne(cr => cr.ResolvedBy)
            .HasForeignKey(cr => cr.ResolvedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
