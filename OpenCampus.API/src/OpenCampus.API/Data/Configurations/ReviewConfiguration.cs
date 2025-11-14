using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        builder.Property(r => r.AuthorId)
            .IsRequired();

        var targetConverter = new ValueConverter<ReviewTargetType, string>(
            value => value.ToString().ToUpperInvariant(),
            value => Enum.Parse<ReviewTargetType>(value, true));

        builder.Property(r => r.TargetType)
            .HasConversion(targetConverter)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(r => r.ScoreClarity)
            .IsRequired();

        builder.Property(r => r.ScoreRelevance)
            .IsRequired();

        builder.Property(r => r.ScoreSupport)
            .IsRequired();

        builder.Property(r => r.ScoreInfrastructure)
            .IsRequired();

        builder.HasCheckConstraint("CK_Reviews_ScoreClarity", $"\"{nameof(Review.ScoreClarity)}\" BETWEEN 1 AND 5");
        builder.HasCheckConstraint("CK_Reviews_ScoreRelevance", $"\"{nameof(Review.ScoreRelevance)}\" BETWEEN 1 AND 5");
        builder.HasCheckConstraint("CK_Reviews_ScoreSupport", $"\"{nameof(Review.ScoreSupport)}\" BETWEEN 1 AND 5");
        builder.HasCheckConstraint("CK_Reviews_ScoreInfrastructure", $"\"{nameof(Review.ScoreInfrastructure)}\" BETWEEN 1 AND 5");

        builder.Property(r => r.Text)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(r => r.Approved)
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(r => new { r.TargetType, r.Approved });
        builder.HasIndex(r => r.AuthorId);

        builder.HasOne(r => r.Author)
            .WithMany(u => u.ReviewsAuthored)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Institution)
            .WithMany(i => i.Reviews)
            .HasForeignKey(r => r.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Professor)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Subject)
            .WithMany(s => s.Reviews)
            .HasForeignKey(r => r.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Comments)
            .WithOne(c => c.Review)
            .HasForeignKey(c => c.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
