using Application.Users;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebApi.Seed
{
    /// <summary>
    /// Orquestra os seeds: garante o papel "Admin", o usuário administrador padrão e,
    /// opcionalmente, os dados de demonstração no tenant desse administrador. Idempotente.
    /// </summary>
    public class DatabaseSeeder
    {
        public const string AdminRole = "Admin";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly DbContextOptions<AppDbContext> _dbOptions;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly SeedOptions _options;

        public DatabaseSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            DbContextOptions<AppDbContext> dbOptions,
            IOptions<SeedOptions> options,
            IHostEnvironment environment,
            ILogger<DatabaseSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbOptions = dbOptions;
            _environment = environment;
            _logger = logger;
            _options = options.Value;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
                return;

            if (string.IsNullOrWhiteSpace(_options.AdminEmail) || string.IsNullOrWhiteSpace(_options.AdminPassword))
                throw new InvalidOperationException(
                    "Seed habilitado, mas Seed:AdminEmail/Seed:AdminPassword não foram configurados (Seed__AdminPassword).");

            var admin = await GarantirAdminAsync(_options.AdminEmail, _options.AdminPassword);

            if (!_options.DemoData)
                return;

            if (_environment.IsProduction() && !_options.AllowDemoDataInProduction)
            {
                _logger.LogWarning(
                    "Seed: dados de demonstração ignorados no ambiente de produção. " +
                    "Defina Seed__AllowDemoDataInProduction=true (SEED_DEMO_DATA_IN_PRODUCTION) para criá-los.");
                return;
            }

            // O AppDbContext aplica o TenantId pelo provedor; fora de uma requisição usamos o tenant do admin.
            await using var context = new AppDbContext(_dbOptions, new FixedTenantProvider(admin.TenantId));
            var criado = await DemoDataSeeder.SeedAsync(context, cancellationToken);

            _logger.LogInformation(criado
                ? "Seed: dados de demonstração criados para o tenant do administrador."
                : "Seed: dados de demonstração já existem; nada a fazer.");
        }

        private async Task<ApplicationUser> GarantirAdminAsync(string email, string password)
        {
            if (!await _roleManager.RoleExistsAsync(AdminRole))
                Garantir(await _roleManager.CreateAsync(new IdentityRole<Guid>(AdminRole)), "criar o papel Admin");

            var admin = await _userManager.FindByEmailAsync(email);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    TenantId = Guid.NewGuid()
                };
                Garantir(await _userManager.CreateAsync(admin, password), "criar o usuário administrador");
                _logger.LogInformation("Seed: usuário administrador {Email} criado.", email);
            }
            else if (admin.TenantId == Guid.Empty)
            {
                admin.TenantId = Guid.NewGuid();
                Garantir(await _userManager.UpdateAsync(admin), "atribuir tenant ao administrador");
            }

            if (!await _userManager.IsInRoleAsync(admin, AdminRole))
                Garantir(await _userManager.AddToRoleAsync(admin, AdminRole), "associar o administrador ao papel Admin");

            return admin;
        }

        private static void Garantir(IdentityResult result, string acao)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Seed: não foi possível {acao}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        }
    }
}
