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
public class RiceCompositionController(IQualityQueryService queryService, ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the whole and broken grain composition of a production batch")]
    [ProducesResponseType<RiceCompositionResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRiceComposition([FromQuery] long batchId, CancellationToken cancellationToken)
    {
        var result = await queryService.Handle(new GetRiceCompositionQuery(batchId), cancellationToken);
        return result.Match(
            report => Ok(RiceCompositionResourceFromReportAssembler.ToResourceFromReport(report)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }
}
