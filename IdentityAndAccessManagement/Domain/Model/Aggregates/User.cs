using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Errors;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Events;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;

namespace Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;

public class User : AuditableAggregateRoot
{
    public const string AdministratorRole = "Administrator";

    private User()
    {
        Email = null!;
        PasswordHash = string.Empty;
        Role = string.Empty;
    }

    private User(EmailAddress email, string passwordHash, string role)
    {
        Guard.Against(string.IsNullOrWhiteSpace(passwordHash), IamErrors.InvalidUserData);
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Active = true;
        AddDomainEvent(() => new UserRegisteredEvent(Id, Role));
    }

    public UserId Id { get; private set; }
    public EmailAddress Email { get; private set; }

    /// <summary>BCrypt hash. The password itself is never stored.</summary>
    public string PasswordHash { get; private set; }

    public string Role { get; private set; }
    public bool Active { get; private set; }

    public static User RegisterAdministrator(EmailAddress email, string passwordHash) =>
        new(email, passwordHash, AdministratorRole);
}
