using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Data.Configurations;

public class ChangeRequestConfiguration : IEntityTypeConfiguration<ChangeRequest>
{
    public void Configure(EntityTypeBuilder<ChangeRequest> builder)
    {
        builder.ToTable("ChangeRequests");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Id)
            .ValueGeneratedOnAdd();

        var targetConverter = new ValueConverter<ChangeRequestTargetType, string>(
            value => value.ToString().ToUpperInvariant(),
            value => Enum.Parse<ChangeRequestTargetType>(value, true));

        var statusConverter = new ValueConverter<ChangeRequestStatus, string>(
            value => value.ToString().ToUpperInvariant(),
            value => Enum.Parse<ChangeRequestStatus>(value, true));

        builder.Property(cr => cr.TargetType)
            .HasConversion(targetConverter)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(cr => cr.Status)
            .HasConversion(statusConverter)
            .HasMaxLength(32)
            .HasDefaultValue("PENDING")
            .IsRequired();

        builder.Property(cr => cr.SuggestedData)
            .IsRequired()
            .HasMaxLength(8000);

        builder.Property(cr => cr.ResolutionNotes)
            .HasMaxLength(2000);

        builder.Property(cr => cr.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(cr => new { cr.TargetType, cr.Status });
        builder.HasIndex(cr => cr.CreatedById);

        builder.HasOne(cr => cr.CreatedBy)
            .WithMany(u => u.ChangeRequestsCreated)
            .HasForeignKey(cr => cr.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.ResolvedBy)
            .WithMany(u => u.ChangeRequestsResolved)
            .HasForeignKey(cr => cr.ResolvedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Institution)
            .WithMany(i => i.ChangeRequests)
            .HasForeignKey(cr => cr.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Course)
            .WithMany(c => c.ChangeRequests)
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Professor)
            .WithMany(p => p.ChangeRequests)
            .HasForeignKey(cr => cr.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Subject)
            .WithMany(s => s.ChangeRequests)
            .HasForeignKey(cr => cr.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
