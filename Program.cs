using MaillotStore.Components;
using MaillotStore.Components.Account;
using MaillotStore.Data;
using MaillotStore.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
  .AddInteractiveServerComponents();

builder.Services.AddSingleton<OrderStateService>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddHttpContextAccessor(); // Needed for the service to access cookies
builder.Services.AddScoped<ReferralService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
  .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- Enable Roles ---
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false) // Allow login without email confirmation
    .AddRoles<IdentityRole>() // Add role services
    .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddSignInManager()
  .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<MaillotStore.Services.ICartService, MaillotStore.Services.CartService>();
builder.Services.AddScoped<StateContainer>();
builder.Services.AddSingleton<SearchStateService>();
builder.Services.AddSingleton<OrderStateService>();

var app = builder.Build();

// --- NEW BLOCK: Seed Roles and Run Migrations on Startup ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // 1. Seed Roles
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Influencer" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2. Run Database Migrations (for Render)
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        // This tells Render to update its PostgreSQL database
        await dbContext.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
// --- End of new block ---

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