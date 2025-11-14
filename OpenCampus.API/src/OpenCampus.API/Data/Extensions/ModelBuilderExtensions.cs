using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Data.Extensions;

public static class ModelBuilderExtensions
{
    private const string SqliteCurrentTimestamp = "CURRENT_TIMESTAMP";

    public static void ApplyAuditableEntityDefaults(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(IsAuditableEntity))
        {
            var createdAtProperty = entityType.FindProperty(nameof(AuditableEntity.CreatedAt));
            if (createdAtProperty is not null && createdAtProperty.GetDefaultValueSql() is null && createdAtProperty.GetDefaultValue() is null)
            {
                createdAtProperty.SetDefaultValueSql(SqliteCurrentTimestamp);
            }

            var updatedAtProperty = entityType.FindProperty(nameof(AuditableEntity.UpdatedAt));
            if (updatedAtProperty is not null)
            {
                updatedAtProperty.IsNullable = true;
            }
        }
    }

    public static void ApplyRestrictDeleteBehavior(this ModelBuilder modelBuilder)
    {
        foreach (var foreignKey in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(entityType => entityType.GetForeignKeys()))
        {
            if (foreignKey.IsOwnership)
            {
                continue;
            }

            if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }

    private static bool IsAuditableEntity(IMutableEntityType entityType)
        => entityType.ClrType is not null && typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType);
}
