using MaillotStore.Components;
using MaillotStore.Components.Account;
using MaillotStore.Data;
using MaillotStore.Services.Implementations;
using MaillotStore.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// --- REGISTER YOUR APP SERVICES ---
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderStateService, OrderStateService>();
builder.Services.AddScoped<ReferralService>();
builder.Services.AddScoped<StateContainer>();

// FIX: Register the concrete service first
builder.Services.AddScoped<SearchStateService>();

builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ILeagueService, LeagueService>();

// FIX: Point the Interface to the SAME instance as the concrete service
builder.Services.AddScoped<ISearchStateService>(sp => sp.GetRequiredService<SearchStateService>());

builder.Services.AddScoped<MaillotStore.Services.Implementations.CloudinaryService>();
// ----------------------------------

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ==================================================================================
// FIX: DATABASE REGISTRATION
// ==================================================================================

// 1. Register the Factory (Singleton) - This manages the DB connections safely for Blazor
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Register the Scoped Context (for Identity/Login) using the Factory
// This allows the "UserManager" to get a context request-by-request, while Blazor uses the Factory.
builder.Services.AddScoped<ApplicationDbContext>(p =>
    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

// ==================================================================================

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// =========================================================================
//  ADD THIS BLOCK HERE: It automatically creates tables on startup
// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        // This command applies any pending migrations and creates the database if it doesn't exist
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        // Log errors if the migration fails
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
// =========================================================================

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();