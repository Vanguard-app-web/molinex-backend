using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using Molinex.Platform.CommercialEngagement.Application.QueryServices;
using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.Commands;
using Molinex.Platform.CommercialEngagement.Domain.Model.Errors;
using Molinex.Platform.CommercialEngagement.Domain.Model.Queries;
using Molinex.Platform.CommercialEngagement.Interfaces.Rest.Resources;
using Molinex.Platform.CommercialEngagement.Interfaces.Rest.Transform;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Attributes;
using Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails;

namespace Molinex.Platform.CommercialEngagement.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Commercial Inquiries")]
public class CommercialInquiriesController(
    IMediator mediator,
    ICommercialQueryService queryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Submit a commercial inquiry")]
    [EndpointDescription("Public. Used by the landing page contact form.")]
    [ProducesResponseType<CommercialInquiryResource>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCommercialInquiry([FromBody] CreateCommercialInquiryResource resource,
        CancellationToken cancellationToken)
    {
        var command = SubmitCommercialInquiryCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await mediator
            .SendCommandAsync<SubmitCommercialInquiryCommand, Result<CommercialInquiry>>(command, cancellationToken);
        return result.Match(
            inquiry => CreatedAtAction(nameof(GetCommercialInquiryById), new { id = inquiry.Id.Value },
                CommercialInquiryResourceFromEntityAssembler.ToResourceFromEntity(inquiry)),
            error => problemDetailsFactory.ToActionResult(this, error));
    }

    [HttpGet("{id:long}")]
    [Authorize]
    [EndpointSummary("Get a commercial inquiry by id")]
    [ProducesResponseType<CommercialInquiryResource>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCommercialInquiryById(long id, CancellationToken cancellationToken)
    {
        var inquiry = await queryService.Handle(new GetCommercialInquiryByIdQuery(id), cancellationToken);
        return inquiry is null
            ? problemDetailsFactory.ToActionResult(this, CommercialErrors.InquiryNotFound)
            : Ok(CommercialInquiryResourceFromEntityAssembler.ToResourceFromEntity(inquiry));
    }
}
