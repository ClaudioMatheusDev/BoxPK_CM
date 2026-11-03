using Application.Common;
using Application.Dtos;
using Application.Users;
using AuditLogCM.EFCore.Extensions;
using AuditLogCM.EFCore.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using WebApi.Middleware;
using WebApi.Service;
using WebApi.Service.Auditoria;
using WebApi.Service.Auth;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Chave JWT: configuração "Jwt:Key" (variável Jwt__Key) ou JWT_KEY. O TokenService lê apenas "Jwt:Key".
var jwtKey = builder.Configuration["Jwt:Key"] ?? builder.Configuration["JWT_KEY"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Configuração Jwt:Key não encontrada ou vazia.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("A chave Jwt:Key deve ter pelo menos 32 caracteres.");
builder.Configuration["Jwt:Key"] = jwtKey;

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BoxPK_CM";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BoxPK_CMClients";

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();

// Registra o serviço de auditoria e o banco onde os logs ficam
builder.Services.AddAuditLog<CurrentUserResolver>(options =>
    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("Infrastructure")));
// Associa o interceptor ao DbContext da sua aplicação
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseSqlServer(connectionString).UseAuditLog(sp));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ItemCompraAtualizarDtoValidator>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173", "http://127.0.0.1:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Limite de requisições nos endpoints de autenticação (por IP)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IColecaoService, ColecaoService>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IMovimentacaoEstoqueService, MovimentacaoEstoqueService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IItemCompraService, ItemCompraService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

if (app.Configuration.GetValue<bool>("APPLY_MIGRATIONS"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var auditDbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    await auditDbContext.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Redirecionamento HTTPS apenas quando habilitado (em containers a API serve HTTP atrás de um proxy).
if (app.Configuration.GetValue<bool>("Https:Redirect"))
{
    app.UseHttpsRedirection();
}

app.UseCors("PermitirReact");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
