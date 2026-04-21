using Car_Manigment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;

var builder = WebApplication.CreateBuilder(args);

// Add request size limits
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB limit
    options.ValueLengthLimit = 100 * 1024; // 100 KB per form field
    options.ValueCountLimit = 1000; // Max 1000 form fields
});

builder.Services.AddControllersWithViews(options =>
{
    // Add automatic ValidateAntiForgeryToken to all POST actions
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminEmail = "admin@carmanagement.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, "Admin123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// Middleware pipeline configuration
// Order is critical: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/

// 1. HTTPS Redirection (must be early)
app.UseHttpsRedirection();

// 2. HSTS (production only)
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// 3. Static Files (before error handling so CSS/JS loads on error pages)
app.UseStaticFiles();

// 4. Routing (must be before authentication)
app.UseRouting();

// 5. Authentication & Authorization (must be after routing, before endpoints)
app.UseAuthentication(); 
app.UseAuthorization();

// 6. Custom Error Pages (after authentication so [AllowAnonymous] works)
// Configure for all environments - custom pages work in both dev and production
app.UseExceptionHandler("/Error/500");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Note: If you need detailed error traces during debugging, temporarily comment out
// the two lines above and uncomment this:
// app.UseDeveloperExceptionPage();

// 7. Security Headers - XSS and CSRF Protection
app.Use(async (context, next) =>
{
    // Prevent clickjacking attacks
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Prevent MIME type sniffing attacks
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Enable XSS protection in legacy browsers
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // Enhanced Content Security Policy to prevent XSS attacks
    // Note: 'unsafe-inline' and 'unsafe-eval' are required for Bootstrap, jQuery validation
    // In production, consider using nonces or hashes for stricter CSP
    context.Response.Headers.Append("Content-Security-Policy", 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://ajax.aspnetcdn.com; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://ajax.aspnetcdn.com; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://cdn.jsdelivr.net data:; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self';");

    // Referrer Policy to limit information leakage
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    // Permissions Policy (formerly Feature Policy)
    context.Response.Headers.Append("Permissions-Policy", 
        "geolocation=(), microphone=(), camera=(), payment=()");

    await next();
});

// 8. Map endpoints
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); 

app.Run();
