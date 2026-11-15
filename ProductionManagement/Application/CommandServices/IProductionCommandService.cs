using Molinex.Platform.ProductionManagement.Domain.Model.Aggregates;
using Molinex.Platform.ProductionManagement.Domain.Model.Commands;

namespace Molinex.Platform.ProductionManagement.Application.CommandServices;

/// <summary>
///     Application contract for Production Management commands. Commands are dispatched through the
///     mediator so the logging, exception and transaction behaviors run around every handler.
/// </summary>
public interface IProductionCommandService
{
    Task<Result<RawMaterialReception>> Handle(RecordRawMaterialReceptionCommand command,
        CancellationToken cancellationToken);

    Task<Result<ProductionBatch>> Handle(RegisterProductionBatchCommand command, CancellationToken cancellationToken);

    Task<Result<ProductionRecord>> Handle(RecordProductionInformationCommand command,
        CancellationToken cancellationToken);

    Task<Result<ProductionRecord>> Handle(UpdateProductionInformationCommand command,
        CancellationToken cancellationToken);
}
