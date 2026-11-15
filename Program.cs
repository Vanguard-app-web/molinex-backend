using Cortex.Mediator;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.CommandServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.CommandServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.Internal.QueryServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Application.QueryServices;
using Molinex.Platform.AssetAndMaintenanceManagement.Domain.Repositories;
using Molinex.Platform.AssetAndMaintenanceManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Molinex.Platform.CommercialEngagement.Application.CommandServices;
using Molinex.Platform.CommercialEngagement.Application.Internal.CommandServices;
using Molinex.Platform.CommercialEngagement.Application.Internal.QueryServices;
using Molinex.Platform.CommercialEngagement.Application.QueryServices;
using Molinex.Platform.CommercialEngagement.Domain.Repositories;
using Molinex.Platform.CommercialEngagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Molinex.Platform.IdentityAndAccessManagement.Application.CommandServices;
using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.CommandServices;
using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.OutboundServices;
using Molinex.Platform.IdentityAndAccessManagement.Application.Internal.QueryServices;
using Molinex.Platform.IdentityAndAccessManagement.Application.QueryServices;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Aggregates;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Model.Commands;
using Molinex.Platform.IdentityAndAccessManagement.Domain.Repositories;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Hashing.BCrypt.Services;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Pipeline.Middleware.Extensions;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Tokens.Jwt.Configuration;
using Molinex.Platform.IdentityAndAccessManagement.Infrastructure.Tokens.Jwt.Services;
using Molinex.Platform.ProductionManagement.Application.Acl;
using Molinex.Platform.ProductionManagement.Application.CommandServices;
using Molinex.Platform.ProductionManagement.Application.Internal.CommandServices;
using Molinex.Platform.ProductionManagement.Application.Internal.QueryServices;
using Molinex.Platform.ProductionManagement.Application.QueryServices;
using Molinex.Platform.ProductionManagement.Domain.Repositories;
using Molinex.Platform.ProductionManagement.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Molinex.Platform.ProductionManagement.Interfaces.Acl;
using Molinex.Platform.QualityAndYieldControl.Application.CommandServices;
using Molinex.Platform.QualityAndYieldControl.Application.Internal.CommandServices;
using Molinex.Platform.QualityAndYieldControl.Application.Internal.OutboundServices.Acl;
using Molinex.Platform.QualityAndYieldControl.Application.Internal.QueryServices;
using Molinex.Platform.QualityAndYieldControl.Application.OutboundServices.Acl;
using Molinex.Platform.QualityAndYieldControl.Application.QueryServices;
using Molinex.Platform.QualityAndYieldControl.Domain.Repositories;
using Molinex.Platform.QualityAndYieldControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Molinex.Platform.Shared.Application.Internal.OutboundServices;
using Molinex.Platform.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Interfaces.AspNetCore.OpenApi;
using Molinex.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Outbox;
using Molinex.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Molinex.Platform.Shared.Infrastructure.Pipeline.Middleware.Components;
using Scalar.AspNetCore;
using ProblemDetailsFactory = Molinex.Platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// ASP.NET Core: kebab-case routes, Problem Details for invalid input and unexpected failures.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
            context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>()
                .ToActionResult(context.HttpContext, SharedErrors.InvalidInput);
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddLocalization();
builder.Services.AddSingleton<ProblemDetailsFactory>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:5173"];
    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
}));

// OpenAPI document with the JWT Bearer scheme; Scalar is the interactive client.
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// Persistence: MySQL through EF Core. The connection string comes from the environment or user secrets.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Configure ConnectionStrings__DefaultConnection with a MySQL connection string.");
    options.UseMySQL(connectionString)
        .AddInterceptors(serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
    if (builder.Environment.IsDevelopment()) options.EnableDetailedErrors();
});
builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IIntegrationEventOutbox, IntegrationEventOutbox>();

// Production Management
builder.Services.AddScoped<IRawMaterialReceptionRepository, RawMaterialReceptionRepository>();
builder.Services.AddScoped<IProductionBatchRepository, ProductionBatchRepository>();
builder.Services.AddScoped<IProductionRecordRepository, ProductionRecordRepository>();
builder.Services.AddScoped<IProductionCommandService, ProductionCommandService>();
builder.Services.AddScoped<IProductionQueryService, ProductionQueryService>();
builder.Services.AddScoped<IProductionContextFacade, ProductionContextFacade>();

// Quality and Yield Control (reads Production Management only through its ACL)
builder.Services.AddScoped<IQualityAssessmentRepository, QualityAssessmentRepository>();
builder.Services.AddScoped<IWasteRecordRepository, WasteRecordRepository>();
builder.Services.AddScoped<IExternalProductionService, ExternalProductionService>();
builder.Services.AddScoped<IQualityCommandService, QualityCommandService>();
builder.Services.AddScoped<IQualityQueryService, QualityQueryService>();

// Asset and Maintenance Management
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IMaintenanceRecordRepository, MaintenanceRecordRepository>();
builder.Services.AddScoped<IMaintenanceCommandService, MaintenanceCommandService>();
builder.Services.AddScoped<IMaintenanceQueryService, MaintenanceQueryService>();

// Identity and Access Management
builder.Services.AddOptions<TokenSettings>()
    .Bind(builder.Configuration.GetSection(TokenSettings.SectionName))
    .PostConfigure(settings =>
    {
        // Development convenience only: an ephemeral secret, so tokens stop being valid when the API restarts.
        if (string.IsNullOrWhiteSpace(settings.Secret) && builder.Environment.IsDevelopment())
            settings.Secret = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    })
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.Secret)
                          && System.Text.Encoding.UTF8.GetByteCount(settings.Secret) >= TokenSettings.MinimumSecretBytes,
        "Configure TokenSettings__Secret with at least 32 bytes.")
    .ValidateOnStart();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Commercial Engagement
builder.Services.AddScoped<ICommercialInquiryRepository, CommercialInquiryRepository>();
builder.Services.AddScoped<ICommercialCommandService, CommercialCommandService>();
builder.Services.AddScoped<ICommercialQueryService, CommercialQueryService>();

// Mediator: every command runs through logging, exception handling and the transaction (unit of work).
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(LoggingCommandBehavior<,>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ExceptionHandlingCommandBehavior<,>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionCommandBehavior<,>));
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations and, when configured, register the first administrator account.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    services.GetRequiredService<AppDbContext>().Database.Migrate();

    var email = app.Configuration["Bootstrap:Email"];
    var password = app.Configuration["Bootstrap:Password"];
    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrEmpty(password))
    {
        var result = await services.GetRequiredService<IMediator>()
            .SendCommandAsync<RegisterAdministratorCommand, Result<User>>(
                new RegisterAdministratorCommand(email, password));
        app.Logger.LogInformation("Bootstrap administrator: {Outcome}",
            result.IsSuccess ? "registered" : result.Error.Code);
    }
}

app.UseExceptionHandler();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures("en", "es")
    .AddSupportedUICultures("en", "es"));
app.MapOpenApi();
app.MapScalarApiReference();
app.UseCors();
app.UseRequestAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).ExcludeFromDescription();

app.Run();

/// <summary>Entry point type, exposed so the mediator can scan this assembly.</summary>
public partial class Program;
