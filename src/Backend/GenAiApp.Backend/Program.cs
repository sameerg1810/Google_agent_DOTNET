using Microsoft.EntityFrameworkCore;
using GenAiApp.Backend.Data; // <--- MAKE SURE THIS USING IS HERE
using GenAiApp.Backend.Extensions;
using GenAiApp.Backend.Services;

var builder = WebApplication.CreateBuilder(args);
// 1. Define the CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Your Angular URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
// 1. Register Services
builder.Services.AddSingleton<GeminiService>();

// 2. Register Database (THIS IS WHAT WAS MISSING OR BROKEN)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register Auth
builder.Services.AddGoogleIdentityAuth(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowAngularApp");
// 4. Middleware
app.UseAuthentication();
app.UseAuthorization();

// 5. Map Endpoints
app.MapGlobalEndpoints();

app.Run();