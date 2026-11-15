using Molinex.Platform.ProductionManagement.Domain.Model.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Errors;
using Molinex.Platform.ProductionManagement.Domain.Model.Events;
using Molinex.Platform.ProductionManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;

public class ProductionRecord : AuditableAggregateRoot
{
    private ProductionRecord()
    {
        Details = null!;
    }

    public ProductionRecord(RecordProductionInformationCommand command)
    {
        Guard.Against(command.BatchId <= 0, ProductionErrors.BatchNotFound);
        BatchId = new ProductionBatchId(command.BatchId);
        Details = ProductionDetails.From(command.ProcessName, command.ProcessedWeight, command.Unit,
            command.StartedAt, command.FinishedAt, command.Status);
        AddDomainEvent(() => new ProductionInformationRecordedEvent(Id, BatchId));
    }

    public ProductionRecordId Id { get; private set; }
    public ProductionBatchId BatchId { get; private set; }
    public ProductionDetails Details { get; private set; }

    /// <summary>Updates the process information. The record keeps its identity, batch and start date.</summary>
    public void UpdateInformation(UpdateProductionInformationCommand command)
    {
        Guard.Against(command.BatchId != BatchId.Value, ProductionErrors.BatchChangeNotAllowed);
        var details = ProductionDetails.From(command.ProcessName, command.ProcessedWeight, command.Unit,
            command.StartedAt, command.FinishedAt, command.Status);
        Guard.Against(details.Period.StartedAt != Details.Period.StartedAt,
            ProductionErrors.StartDateChangeNotAllowed);
        Details = details;
        AddDomainEvent(() => new ProductionInformationUpdatedEvent(Id, Details.Status.ToCode()));
    }
}
