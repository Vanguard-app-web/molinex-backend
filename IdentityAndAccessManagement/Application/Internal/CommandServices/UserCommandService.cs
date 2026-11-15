using Cortex.Mediator.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Application.CommandServices;
using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.OutboundServices;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Errors;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.ValueObjects;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Repositories;

namespace Molinex.Platform.IdentityAndAccessManagement.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IHashingService hashingService,
    ITokenService tokenService)
    : IUserCommandService,
        ICommandHandler<SignInCommand, Result<AuthenticatedUser>>,
        ICommandHandler<RegisterAdministratorCommand, Result<User>>
{
    public const int MinimumPasswordLength = 12;

    public async Task<Result<AuthenticatedUser>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrEmpty(command.Password))
            return Result<AuthenticatedUser>.Failure(IamErrors.InvalidCredentials);

        EmailAddress email;
        try
        {
            email = new EmailAddress(command.Email);
        }
        catch (DomainRuleException)
        {
            return Result<AuthenticatedUser>.Failure(IamErrors.InvalidCredentials);
        }

        var user = await userRepository.FindByEmailAsync(email, cancellationToken);
        if (user is not { Active: true } || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            return Result<AuthenticatedUser>.Failure(IamErrors.InvalidCredentials);

        return Result<AuthenticatedUser>.Success(new AuthenticatedUser(user, tokenService.GenerateToken(user)));
    }

    public async Task<Result<User>> Handle(RegisterAdministratorCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.AnyAsync(cancellationToken))
            return Result<User>.Failure(IamErrors.AdministratorAlreadyRegistered);
        if (string.IsNullOrEmpty(command.Password) || command.Password.Length < MinimumPasswordLength)
            return Result<User>.Failure(IamErrors.InvalidUserData);

        User user;
        try
        {
            user = User.RegisterAdministrator(new EmailAddress(command.Email),
                hashingService.HashPassword(command.Password));
        }
        catch (DomainRuleException exception)
        {
            return Result<User>.Failure(exception.Error);
        }

        await userRepository.AddAsync(user, cancellationToken);
        return Result<User>.Success(user);
    }
}
