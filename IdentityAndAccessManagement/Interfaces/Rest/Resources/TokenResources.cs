namespace Molinex.Platform.IdentityAndAccessManagement.Interfaces.Rest.Resources;

public record SignInResource(string Email, string Password);

public record AuthenticatedUserResource(string Token, string Email, string Role);
