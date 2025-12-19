using System.Reflection;
using AlphaProjectManager.Utility;
using Application;
using Application.Services.TelegramBot;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using TeamProjectConnection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(AuthorizationConfiguration.ConfigureJwtBearerAuthorization);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        policy => policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
        options.AddPolicy("AllowLocalNet3000",
        policy => policy.WithOrigins("http://192.168.1.106:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    AuthorizationConfiguration.ConfigureSwaggerWithJwtBearer(c);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alpha Project Manager", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.TryAddApplicationLayer(builder.Configuration);
builder.Services.AddHostedService<TelegramBotBackgroundService>();
builder.Services.AddInfrastructure();
builder.Services.AddTeamProjectConnection();

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost3000");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await InfrastructureStartup.CheckAndMigrateDatabaseAsync(app.Services);

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
app.Run();