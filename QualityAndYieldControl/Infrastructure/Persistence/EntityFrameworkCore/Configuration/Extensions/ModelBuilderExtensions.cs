using Microsoft.EntityFrameworkCore;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.ValueObjects;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace Molinex.Platform.QualityAndYieldControl.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    /// <remarks>
    ///     <c>production_record_id</c> is a reference by identity to Production Management. No foreign key is
    ///     declared across bounded contexts; its existence is verified through the anti-corruption layer.
    /// </remarks>
    public static void ApplyQualityAndYieldControlConfiguration(this ModelBuilder builder)
    {
        builder.Entity<QualityAssessment>(assessment =>
        {
            assessment.ToTable("quality_assessments");
            assessment.HasKey(a => a.Id);
            assessment.Property(a => a.Id)
                .HasConversion(new StronglyTypedIdConverter<QualityAssessmentId, long>(value => new QualityAssessmentId(value)))
                .ValueGeneratedOnAdd();
            assessment.Property(a => a.ProductionRecordId)
                .HasConversion(new StronglyTypedIdConverter<ProductionRecordId, long>(value => new ProductionRecordId(value)))
                .IsRequired();
            assessment.HasIndex(a => a.ProductionRecordId);
            assessment.Property(a => a.AssessedAt).IsRequired();
            assessment.OwnsMany(a => a.Measurements, measurement =>
            {
                measurement.ToTable("quality_measurements");
                measurement.WithOwner().HasForeignKey("QualityAssessmentId");
                measurement.Property<long>("Id").ValueGeneratedOnAdd();
                measurement.HasKey("Id");
                measurement.Property(m => m.Indicator)
                    .HasConversion(indicator => indicator.ToCode(), code => QualityIndicatorExtensions.FromCode(code))
                    .HasMaxLength(40).IsRequired();
                measurement.Property(m => m.Value)
                    .HasConversion(percentage => percentage.Value, value => new Percentage(value))
                    .HasPrecision(7, 4).IsRequired();
                measurement.OwnsOne(m => m.ExpectedRange, range =>
                {
                    range.Property(r => r.Minimum).HasColumnName("expected_minimum")
                        .HasConversion(percentage => percentage.Value, value => new Percentage(value))
                        .HasPrecision(7, 4).IsRequired();
                    range.Property(r => r.Maximum).HasColumnName("expected_maximum")
                        .HasConversion(percentage => percentage.Value, value => new Percentage(value))
                        .HasPrecision(7, 4).IsRequired();
                });
                measurement.Navigation(m => m.ExpectedRange).IsRequired();
                measurement.HasIndex("QualityAssessmentId", nameof(QualityMeasurement.Indicator)).IsUnique();
            });
            assessment.Navigation(a => a.Measurements).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<WasteRecord>(waste =>
        {
            waste.ToTable("waste_records");
            waste.HasKey(w => w.Id);
            waste.Property(w => w.Id)
                .HasConversion(new StronglyTypedIdConverter<WasteRecordId, long>(value => new WasteRecordId(value)))
                .ValueGeneratedOnAdd();
            waste.Property(w => w.ProductionRecordId)
                .HasConversion(new StronglyTypedIdConverter<ProductionRecordId, long>(value => new ProductionRecordId(value)))
                .IsRequired();
            waste.HasIndex(w => w.ProductionRecordId);
            waste.Property(w => w.RecordedAt).IsRequired();
            waste.Ignore(w => w.Percentage);
            waste.OwnsOne(w => w.Quantity, quantity =>
            {
                quantity.Property(q => q.Value).HasColumnName("quantity_value").HasPrecision(18, 6).IsRequired();
                quantity.Property(q => q.Unit).HasColumnName("quantity_unit")
                    .HasConversion(unit => unit.ToCode(), code => MeasurementUnitExtensions.FromCode(code))
                    .HasMaxLength(20).IsRequired();
            });
            waste.Navigation(w => w.Quantity).IsRequired();
            waste.OwnsOne(w => w.BaseWeight, baseWeight =>
            {
                baseWeight.Property(b => b.Value).HasColumnName("base_weight_value").HasPrecision(18, 6).IsRequired();
                baseWeight.Property(b => b.Unit).HasColumnName("base_weight_unit")
                    .HasConversion(unit => unit.ToCode(), code => MeasurementUnitExtensions.FromCode(code))
                    .HasMaxLength(20).IsRequired();
            });
            waste.Navigation(w => w.BaseWeight).IsRequired(false);
        });
    }
}
