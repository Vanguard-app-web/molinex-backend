using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.Commands;
using Molinex.Platform.CommercialEngagement.Interfaces.Rest.Resources;

namespace Molinex.Platform.CommercialEngagement.Interfaces.Rest.Transform;

public static class SubmitCommercialInquiryCommandFromResourceAssembler
{
    public static SubmitCommercialInquiryCommand ToCommandFromResource(CreateCommercialInquiryResource resource) =>
        new(resource.FullName, resource.CompanyName, resource.Email, resource.Message);
}

public static class CommercialInquiryResourceFromEntityAssembler
{
    public static CommercialInquiryResource ToResourceFromEntity(CommercialInquiry entity) =>
        new(entity.Id.ToString(), entity.FullName, entity.CompanyName, entity.Email.Address, entity.Message,
            entity.SubmittedAt);
}
