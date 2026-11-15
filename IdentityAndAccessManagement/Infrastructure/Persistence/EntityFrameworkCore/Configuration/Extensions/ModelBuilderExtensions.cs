using Microsoft.EntityFrameworkCore;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIdentityAndAccessConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>(user =>
        {
            user.ToTable("users");
            user.HasKey(u => u.Id);
            user.Property(u => u.Id)
                .HasConversion(new StronglyTypedIdConverter<UserId, long>(value => new UserId(value)))
                .ValueGeneratedOnAdd();
            user.Property(u => u.Email)
                .HasConversion(email => email.Address, address => new EmailAddress(address))
                .HasMaxLength(EmailAddress.MaxLength).IsRequired();
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.PasswordHash).HasMaxLength(100).IsRequired();
            user.Property(u => u.Role).HasMaxLength(50).IsRequired();
            user.Property(u => u.Active).IsRequired();
        });
    }
}
