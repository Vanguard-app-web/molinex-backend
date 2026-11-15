using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;
using Molinex.Platform.QualityAndYieldControl.Application.QueryServices;
using Molinex.Platform.QualityAndYieldControl.Domain.Model.Queries;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Resources;
using Molinex.Platform.QualityAndYieldControl.Interfaces.Rest.Transform;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.QualityAndYieldControl.Interfaces.Rest;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Tags("Yield and Composition Indicators")]
public class YieldIndicatorsController(IQualityQueryService queryService, ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get yield and waste indicators by production record")]
    [EndpointDescription("Production records are read through the anti-corruption layer towards Production Management.")]
    [ProducesResponseType<YieldIndicatorsResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetYieldIndicators([FromQuery] long? productionRecordId,
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetYieldIndicatorsQuery(productionRecordId, from, to),
            cancellationToken);
        return result.Match(
            report => Ok(YieldIndicatorsResourceFromReportAssembler.ToResourceFromReport(report)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }
}
