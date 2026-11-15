using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;
using Molinex.Platform.QualityAndYieldControl.Application.QueryServices;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Aggregates;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Commands;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Errors;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Queries;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Transform;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Tags("Quality Assessments")]
public class QualityAssessmentsController(
    IMediator mediator,
    IQualityQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Record the quality results of a production record")]
    [ProducesResponseType<QualityAssessmentResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateQualityAssessment([FromBody] CreateQualityAssessmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = RecordQualityAssessmentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<RecordQualityAssessmentCommand, Result<QualityAssessment>>(command, cancellationToken);
        return result.Match(
            assessment => CreatedAtAction(nameof(GetQualityAssessmentById), new { id = assessment.Id.Value },
                QualityAssessmentResourceFromEntityAssembler.ToResourceFromEntity(assessment)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet]
    [EndpointSummary("List quality assessments, optionally by production record")]
    [ProducesResponseType<IEnumerable<QualityAssessmentResource>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetQualityAssessments([FromQuery] long? productionRecordId,
        CancellationToken cancellationToken)
    {
        var assessments = await queryService.Handle(new GetQualityAssessmentsQuery(productionRecordId),
            cancellationToken);
        return Ok(assessments.Select(QualityAssessmentResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:long}")]
    [EndpointSummary("Get a quality assessment by id")]
    [ProducesResponseType<QualityAssessmentResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQualityAssessmentById(long id, CancellationToken cancellationToken)
    {
        var assessment = await queryService.Handle(new GetQualityAssessmentByIdQuery(id), cancellationToken);
        return assessment is null
            ? problemDetailsFactory.ToActionResult(this, QualityErrors.AssessmentNotFound)
            : Ok(QualityAssessmentResourceFromEntityAssembler.ToResourceFromEntity(assessment));
    }
}
