using Microsoft.EntityFrameworkCore;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace Molinex.Platform.ProductionManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    private const string CaseInsensitiveCollation = "utf8mb4_0900_ai_ci";

    public static void ApplyProductionManagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<RawMaterialReception>(reception =>
        {
            reception.ToTable("raw_material_receptions");
            reception.HasKey(r => r.Id);
            reception.Property(r => r.Id)
                .HasConversion(new StronglyTypedIdConverter<RawMaterialReceptionId, long>(value => new RawMaterialReceptionId(value)))
                .ValueGeneratedOnAdd();
            reception.Property(r => r.ReceivedAt).IsRequired();
            reception.Property(r => r.Supplier)
                .HasConversion(supplier => supplier.Name, name => new Supplier(name))
                .HasMaxLength(Supplier.MaxLength).IsRequired();
            reception.Property(r => r.Origin)
                .HasConversion(origin => origin.Description, description => new MaterialOrigin(description))
                .HasMaxLength(MaterialOrigin.MaxLength).IsRequired();
            reception.OwnsOne(r => r.Quantity, quantity =>
            {
                quantity.Property(w => w.Value).HasColumnName("quantity_value").HasPrecision(18, 6).IsRequired();
                quantity.Property(w => w.Unit).HasColumnName("quantity_unit")
                    .HasConversion(unit => unit.ToCode(), code => MeasurementUnitExtensions.FromCode(code))
                    .HasMaxLength(20).IsRequired();
            });
            reception.Navigation(r => r.Quantity).IsRequired();
        });

        builder.Entity<ProductionBatch>(batch =>
        {
            batch.ToTable("production_batches");
            batch.HasKey(b => b.Id);
            batch.Property(b => b.Id)
                .HasConversion(new StronglyTypedIdConverter<ProductionBatchId, long>(value => new ProductionBatchId(value)))
                .ValueGeneratedOnAdd();
            batch.Property(b => b.Code)
                .HasConversion(code => code.Value, value => new BatchCode(value))
                .HasMaxLength(BatchCode.MaxLength).UseCollation(CaseInsensitiveCollation).IsRequired();
            batch.HasIndex(b => b.Code).IsUnique();
            batch.Property(b => b.ReceptionId)
                .HasConversion(new StronglyTypedIdConverter<RawMaterialReceptionId, long>(value => new RawMaterialReceptionId(value)))
                .IsRequired();
            batch.Property(b => b.RegisteredAt).IsRequired();
            batch.HasOne<RawMaterialReception>().WithMany()
                .HasForeignKey(b => b.ReceptionId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProductionRecord>(record =>
        {
            record.ToTable("production_records");
            record.HasKey(r => r.Id);
            record.Property(r => r.Id)
                .HasConversion(new StronglyTypedIdConverter<ProductionRecordId, long>(value => new ProductionRecordId(value)))
                .ValueGeneratedOnAdd();
            record.Property(r => r.BatchId)
                .HasConversion(new StronglyTypedIdConverter<ProductionBatchId, long>(value => new ProductionBatchId(value)))
                .IsRequired();
            record.HasOne<ProductionBatch>().WithMany()
                .HasForeignKey(r => r.BatchId).OnDelete(DeleteBehavior.Restrict);
            record.OwnsOne(r => r.Details, details =>
            {
                details.Property(d => d.ProcessName).HasColumnName("process_name")
                    .HasConversion(name => name.Value, value => new ProductionProcessName(value))
                    .HasMaxLength(ProductionProcessName.MaxLength).IsRequired();
                details.Property(d => d.Status).HasColumnName("status")
                    .HasConversion(status => status.ToCode(), code => ProductionStatusExtensions.FromCode(code))
                    .HasMaxLength(20).IsRequired();
                details.OwnsOne(d => d.ProcessedWeight, weight =>
                {
                    weight.Property(w => w.Value).HasColumnName("processed_weight_value").HasPrecision(18, 6).IsRequired();
                    weight.Property(w => w.Unit).HasColumnName("processed_weight_unit")
                        .HasConversion(unit => unit.ToCode(), code => MeasurementUnitExtensions.FromCode(code))
                        .HasMaxLength(20).IsRequired();
                });
                details.Navigation(d => d.ProcessedWeight).IsRequired();
                details.OwnsOne(d => d.Period, period =>
                {
                    period.Property(p => p.StartedAt).HasColumnName("started_at").IsRequired();
                    period.Property(p => p.FinishedAt).HasColumnName("finished_at");
                    period.Ignore(p => p.IsFinished);
                    period.HasIndex(p => p.StartedAt);
                });
                details.Navigation(d => d.Period).IsRequired();
            });
            record.Navigation(r => r.Details).IsRequired();
        });
    }
}
