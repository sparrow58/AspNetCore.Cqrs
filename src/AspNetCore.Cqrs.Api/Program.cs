using Autofac;
using AspNetCore.Cqrs.Api.Infrastructure.Filters;
using AspNetCore.Cqrs.Application.AutofacModules;
using AspNetCore.Cqrs.Infrastructure.AutofacModules;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.RegisterDefaults();

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<HttpGlobalExceptionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "AspNetCore.Cqrs API",
                        Version = "v1",
                        Description = "HTTP API for accessing AspNetCore.Cqrs data"
                    });
    options.DescribeAllParametersInCamelCase();
});
builder.Services.AddCors();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"))
    .AddSqlServer(builder.Configuration["Database:SqlConnectionString"]!);

//Add HSTS
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule(builder.Configuration));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsProduction())
{
    // Required to forward headers from load balancers and reverse proxies
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();

    //Add security response headers
    app.UseHsts();
    app.Use((context, next) =>
    {
        context.Response.Headers["X-Xss-Protection"] = "1; mode=block";
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
        return next.Invoke();
    });
}

app.UseCors(options =>
{
    options.AllowAnyMethod()
           .AllowAnyHeader()
           .AllowAnyOrigin()
           .WithExposedHeaders("Content-Disposition");
});

app.UseAuthorization();

app.MapHealthChecks("healthz");
app.MapHealthChecks("liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.MapControllers();

app.Run();
