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
using WebApi.Service;
using WebApi.Service.Auditoria;
using WebApi.Service.Auth;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHttpContextAccessor();

// Registra o serviço de auditoria e o banco onde os logs ficam
builder.Services.AddAuditLog<CurrentUserResolver>(options =>
    options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("Infrastructure")));
// Associa o interceptor ao DbContext da sua aplicação
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseSqlServer(connectionString).UseAuditLog(sp));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ItemCompraAtualizarDtoValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? builder.Configuration["JWT_KEY"]
    ?? Environment.GetEnvironmentVariable("Jwt__Key")
    ?? Environment.GetEnvironmentVariable("JWT_KEY");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BoxPK_CM";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BoxPK_CMClients";

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Configuração 'Jwt:Key' não encontrada ou vazia.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
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
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IFornecedorService, FornecedorService>();
builder.Services.AddScoped<IJogoService, JogoService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IColecaoService, ColecaoService>();
builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddScoped<IMovimentacaoEstoque, MovimentacaoEstoqueService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IItemCompraService, ItemCompraService>();



builder.Services.AddControllers();

var app = builder.Build();

if (app.Configuration.GetValue<bool>("APPLY_MIGRATIONS"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var auditDbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    await auditDbContext.Database.MigrateAsync();
}

app.UseCors("PermitirReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.Run();

