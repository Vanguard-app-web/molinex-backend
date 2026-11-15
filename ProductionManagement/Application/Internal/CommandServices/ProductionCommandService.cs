using Cortex.Mediator.Commands;
using Molinex.Platform.ProductionManagement.Application.CommandServices;
using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.Commands;
using Molinex.Platform.ProductionManagement.Domain.Model.Errors;
using Molinex.Platform.ProductionManagement.Domain.Repositories;
using Molinex.Platform.ProductionManagement.Interfaces.Events;
using Molinex.Platform.Shared.Application.Internal.OutboundServices;

namespace Molinex.Platform.ProductionManagement.Application.Internal.CommandServices;

public class ProductionCommandService(
    IRawMaterialReceptionRepository receptionRepository,
    IProductionBatchRepository batchRepository,
    IProductionRecordRepository recordRepository,
    IIntegrationEventOutbox outbox,
    TimeProvider timeProvider)
    : IProductionCommandService,
        ICommandHandler<RecordRawMaterialReceptionCommand, Result<RawMaterialReception>>,
        ICommandHandler<RegisterProductionBatchCommand, Result<ProductionBatch>>,
        ICommandHandler<RecordProductionInformationCommand, Result<ProductionRecord>>,
        ICommandHandler<UpdateProductionInformationCommand, Result<ProductionRecord>>
{
    public async Task<Result<RawMaterialReception>> Handle(RecordRawMaterialReceptionCommand command,
        CancellationToken cancellationToken)
    {
        RawMaterialReception reception;
        try
        {
            reception = new RawMaterialReception(command, timeProvider.GetUtcNow().UtcDateTime);
        }
        catch (DomainRuleException exception)
        {
            return Result<RawMaterialReception>.Failure(exception.Error);
        }

        await receptionRepository.AddAsync(reception, cancellationToken);
        return Result<RawMaterialReception>.Success(reception);
    }

    public async Task<Result<ProductionBatch>> Handle(RegisterProductionBatchCommand command,
        CancellationToken cancellationToken)
    {
        ProductionBatch batch;
        try
        {
            batch = new ProductionBatch(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<ProductionBatch>.Failure(exception.Error);
        }

        if (!await receptionRepository.ExistsByIdAsync(batch.ReceptionId.Value, cancellationToken))
            return Result<ProductionBatch>.Failure(ProductionErrors.ReceptionNotFound);
        if (await batchRepository.ExistsByCodeAsync(batch.Code, cancellationToken))
            return Result<ProductionBatch>.Failure(ProductionErrors.DuplicateBatchCode);

        // The batch and its integration event are written by the same unit of work.
        await batchRepository.AddAsync(batch, cancellationToken);
        await outbox.EnqueueAsync(
            new ProductionBatchRegisteredIntegrationEvent(batch.Code.Value, batch.ReceptionId.Value, batch.RegisteredAt),
            cancellationToken);
        return Result<ProductionBatch>.Success(batch);
    }

    public async Task<Result<ProductionRecord>> Handle(RecordProductionInformationCommand command,
        CancellationToken cancellationToken)
    {
        ProductionRecord record;
        try
        {
            record = new ProductionRecord(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<ProductionRecord>.Failure(exception.Error);
        }

        if (!await batchRepository.ExistsByIdAsync(record.BatchId.Value, cancellationToken))
            return Result<ProductionRecord>.Failure(ProductionErrors.BatchNotFound);

        await recordRepository.AddAsync(record, cancellationToken);
        return Result<ProductionRecord>.Success(record);
    }

    public async Task<Result<ProductionRecord>> Handle(UpdateProductionInformationCommand command,
        CancellationToken cancellationToken)
    {
        var record = await recordRepository.FindByIdAsync(command.RecordId, cancellationToken);
        if (record is null) return Result<ProductionRecord>.Failure(ProductionErrors.RecordNotFound);
        try
        {
            record.UpdateInformation(command);
        }
        catch (DomainRuleException exception)
        {
            return Result<ProductionRecord>.Failure(exception.Error);
        }

        return Result<ProductionRecord>.Success(record);
    }
}
