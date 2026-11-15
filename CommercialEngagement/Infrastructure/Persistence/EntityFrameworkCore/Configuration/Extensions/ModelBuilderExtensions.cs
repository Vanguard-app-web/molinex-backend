using Microsoft.EntityFrameworkCore;
using Molinex.Platform.CommercialEngagement.Domain.Model.Aggregates;
using Molinex.Platform.CommercialEngagement.Domain.Model.ValueObjects;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace Molinex.Platform.CommercialEngagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyCommercialEngagementConfiguration(this ModelBuilder builder)
    {
        builder.Entity<CommercialInquiry>(inquiry =>
        {
            inquiry.ToTable("commercial_inquiries");
            inquiry.HasKey(i => i.Id);
            inquiry.Property(i => i.Id)
                .HasConversion(new StronglyTypedIdConverter<CommercialInquiryId, long>(value => new CommercialInquiryId(value)))
                .ValueGeneratedOnAdd();
            inquiry.Property(i => i.FullName).HasMaxLength(CommercialInquiry.MaxTextLength).IsRequired();
            inquiry.Property(i => i.CompanyName).HasMaxLength(CommercialInquiry.MaxTextLength).IsRequired();
            inquiry.Property(i => i.Email)
                .HasConversion(email => email.Address, address => new ContactEmail(address))
                .HasMaxLength(ContactEmail.MaxLength).IsRequired();
            inquiry.Property(i => i.Message).HasMaxLength(CommercialInquiry.MaxTextLength).IsRequired();
            inquiry.Property(i => i.SubmittedAt).IsRequired();
        });
    }
}
