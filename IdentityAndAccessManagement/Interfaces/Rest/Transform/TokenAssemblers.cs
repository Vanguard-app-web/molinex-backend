using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;
using Molinex.Platform.IdentityAndAccessManagement.Interfaces.Rest.Resources;

namespace Molinex.Platform.IdentityAndAccessManagement.Interfaces.Rest.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource) =>
        new(resource.Email, resource.Password);
}

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(AuthenticatedUser authenticatedUser) =>
        new(authenticatedUser.Token, authenticatedUser.User.Email.Address, authenticatedUser.User.Role);
}
