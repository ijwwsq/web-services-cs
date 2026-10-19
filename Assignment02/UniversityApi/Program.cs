using System.Text;
using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using UniversityApi.Common;
using UniversityApi.Data;
using UniversityApi.Mapping;
using UniversityApi.Middleware;
using UniversityApi.Models;
using UniversityApi.Repositories.Implementations;
using UniversityApi.Repositories.Interfaces;
using UniversityApi.Services.Implementations;
using UniversityApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Ключ подписи не хранится в репозитории: он берётся из user-secrets или переменной окружения Jwt__Key.
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException(
        "Jwt:Key is not configured. Set it with dotnet user-secrets or the Jwt__Key environment variable.");
}

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.User.RequireUniqueEmail = true;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // 401 и 403 возвращаются в том же формате ReturnResult, что и остальные ответы API.
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                await WriteError(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    ErrorCodes.Unauthorized,
                    "Authentication is required");
            },
            OnForbidden = async context =>
            {
                await WriteError(
                    context.HttpContext,
                    StatusCodes.Status403Forbidden,
                    ErrorCodes.Forbidden,
                    "You do not have permission to perform this action");
            }
        };
    });

builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("UniversityClients", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var messages = context.ModelState
            .Where(entry => entry.Value != null && entry.Value.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => entry.Key + ": " + error.ErrorMessage));

        var body = ReturnResult<object>.Fail(
            StatusCodes.Status400BadRequest,
            ErrorCodes.ValidationError,
            string.Join("; ", messages),
            context.HttpContext.TraceIdentifier);

        return new BadRequestObjectResult(body);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "University Course Management API", Version = "v1" });
    options.SwaggerDoc("v2", new OpenApiInfo { Title = "University Course Management API", Version = "v2" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Вставьте JWT, полученный в POST /api/auth/login"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context, logger);
    await IdentitySeeder.SeedAsync(
        context,
        services.GetRequiredService<UserManager<ApplicationUser>>(),
        services.GetRequiredService<RoleManager<IdentityRole>>(),
        app.Configuration,
        logger);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "University API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "University API v2");
    });
}

app.UseHttpsRedirection();
app.UseCors("UniversityClients");

// Сначала выясняем, кто пользователь, и только потом проверяем его права.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task WriteError(HttpContext context, int statusCode, string errorCode, string message)
{
    if (context.Response.HasStarted)
    {
        return;
    }

    var body = ReturnResult<object>.Fail(statusCode, errorCode, message, context.TraceIdentifier);

    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";

    var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    await context.Response.WriteAsync(JsonSerializer.Serialize(body, options));
}
