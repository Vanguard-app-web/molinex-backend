using Microsoft.EntityFrameworkCore;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.Aggregates;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Model.ValueObjects;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAssetAndMaintenanceConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Machine>(machine =>
        {
            machine.ToTable("machines");
            machine.HasKey(m => m.Id);
            machine.Property(m => m.Id)
                .HasConversion(new StronglyTypedIdConverter<MachineId, long>(value => new MachineId(value)))
                .ValueGeneratedOnAdd();
            machine.Property(m => m.Code)
                .HasConversion(code => code.Value, value => new MachineCode(value))
                .HasMaxLength(MachineCode.MaxLength).IsRequired();
            machine.HasIndex(m => m.Code).IsUnique();
            machine.Property(m => m.Name)
                .HasConversion(name => name.Value, value => new MachineName(value))
                .HasMaxLength(MachineName.MaxLength).IsRequired();
            machine.Property(m => m.Model)
                .HasConversion(model => model.Value, value => new MachineModel(value))
                .HasMaxLength(MachineModel.MaxLength).IsRequired();
            machine.Property(m => m.Status)
                .HasConversion(status => status.ToCode(), code => MachineStatusExtensions.FromCode(code))
                .HasMaxLength(30).IsRequired();
            machine.Property(m => m.StatusChangedAt).IsRequired();
        });

        builder.Entity<MaintenanceRecord>(record =>
        {
            record.ToTable("maintenance_records");
            record.HasKey(r => r.Id);
            record.Property(r => r.Id)
                .HasConversion(new StronglyTypedIdConverter<MaintenanceRecordId, long>(value => new MaintenanceRecordId(value)))
                .ValueGeneratedOnAdd();
            record.Property(r => r.MachineId)
                .HasConversion(new StronglyTypedIdConverter<MachineId, long>(value => new MachineId(value)))
                .IsRequired();
            record.HasOne<Machine>().WithMany()
                .HasForeignKey(r => r.MachineId).OnDelete(DeleteBehavior.Restrict);
            record.Property(r => r.Type)
                .HasConversion(type => type.ToCode(), code => MaintenanceTypeExtensions.FromCode(code))
                .HasMaxLength(20).IsRequired();
            record.Property(r => r.PerformedAt).IsRequired();
            record.HasIndex(r => r.PerformedAt);
            record.Property(r => r.Description)
                .HasConversion(description => description.Value, value => new MaintenanceDescription(value))
                .HasMaxLength(MaintenanceDescription.MaxLength).IsRequired();
            record.Property(r => r.AnomalyReference)
                .HasColumnName("anomaly_id")
                .HasConversion(reference => reference!.AnomalyId, value => new AnomalyReference(value))
                .HasMaxLength(AnomalyReference.MaxLength);
            record.OwnsOne(r => r.Responsible, technician =>
            {
                technician.Property(t => t.DisplayName).HasColumnName("technician_name")
                    .HasMaxLength(TechnicianReference.MaxLength).IsRequired();
                technician.Property(t => t.PrincipalId).HasColumnName("technician_id")
                    .HasMaxLength(TechnicianReference.MaxLength);
            });
            record.Navigation(r => r.Responsible).IsRequired();
            record.OwnsOne(r => r.CorrectiveDetails, details =>
            {
                details.Property(d => d.Failure).HasColumnName("failure")
                    .HasMaxLength(CorrectiveMaintenanceDetails.FailureMaxLength).IsRequired();
                details.Property(d => d.Cause).HasColumnName("cause")
                    .HasMaxLength(CorrectiveMaintenanceDetails.CauseMaxLength).IsRequired();
                details.Property(d => d.ActionTaken).HasColumnName("action_taken")
                    .HasMaxLength(CorrectiveMaintenanceDetails.ActionTakenMaxLength).IsRequired();
                details.Property(d => d.DowntimeMinutes).HasColumnName("downtime_minutes").IsRequired();
            });
            record.Navigation(r => r.CorrectiveDetails).IsRequired(false);
        });
    }
}
