namespace Molinex.Platform.ProductionManagement.Domain.Model.Queries;

public record GetAllRawMaterialReceptionsQuery;

public record GetRawMaterialReceptionByIdQuery(long Id);

public record GetProductionBatchesQuery(long? ReceptionId);

public record GetProductionBatchByIdQuery(long Id);

public record GetProductionRecordByIdQuery(long Id);

/// <summary>Production records, optionally narrowed by batch and by the period in which they started.</summary>
public record GetProductionRecordsQuery(long? BatchId, DateTime? From, DateTime? To);

/// <summary>Production history, newest first, optionally narrowed by start period.</summary>
public record GetProductionHistoryQuery(DateTime? From, DateTime? To);
