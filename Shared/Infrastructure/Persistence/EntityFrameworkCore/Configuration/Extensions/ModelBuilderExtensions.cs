using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Outbox;

namespace Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
        new(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> NullableUtcConverter =
        new(value => value, value => value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value);

    public static void ApplySharedConfiguration(this ModelBuilder builder)
    {
        builder.Entity<OutboxMessage>(outbox =>
        {
            outbox.ToTable("integration_outbox");
            outbox.HasKey(message => message.Id);
            outbox.Property(message => message.Id).ValueGeneratedNever();
            outbox.Property(message => message.Type).HasMaxLength(200).IsRequired();
            outbox.Property(message => message.Payload).IsRequired();
            outbox.Property(message => message.OccurredAt).IsRequired();
            outbox.HasIndex(message => message.ProcessedAt);
        });
    }

    /// <summary>
    ///     Applies snake_case names to columns, keys, foreign keys and indexes, and reads every date as UTC.
    ///     Table names are configured explicitly by each bounded context.
    /// </summary>
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var table = entity.GetTableName();
            var sharesOwnerTable = entity.IsOwned() && entity.FindOwnership()?.PrincipalEntityType.GetTableName() == table;
            foreach (var property in entity.GetProperties())
            {
                // An owned type stored in its owner's table shares the owner's key column.
                if (!(sharesOwnerTable && property.IsPrimaryKey()))
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                if (property.ClrType == typeof(DateTime)) property.SetValueConverter(UtcConverter);
                if (property.ClrType == typeof(DateTime?)) property.SetValueConverter(NullableUtcConverter);
            }

            if (string.IsNullOrEmpty(table)) continue;
            if (!sharesOwnerTable)
                foreach (var key in entity.GetKeys().Where(key => key.IsPrimaryKey()))
                    key.SetName($"pk_{table}");

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                if (foreignKey.IsOwnership && sharesOwnerTable) continue;
                foreignKey.SetConstraintName(Identifier($"fk_{table}_{ColumnList(foreignKey.Properties)}"));
            }

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(Identifier($"{(index.IsUnique ? "ux" : "ix")}_{table}_{ColumnList(index.Properties)}"));
        }
    }

    /// <summary>MySQL identifiers are limited to 64 characters.</summary>
    private static string Identifier(string name) => name.Length <= 64 ? name : name[..64];

    private static string ColumnList(IEnumerable<IMutableProperty> properties) =>
        string.Join("_", properties.Select(property => property.GetColumnName()));
}
