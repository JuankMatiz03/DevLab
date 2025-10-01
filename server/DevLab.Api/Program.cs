using DevLab.Api.Application.Common.Behaviors;
using DevLab.Api.Data;
using DevLab.Api.Services;
using FluentValidation;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// MVC + swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// MediatR + Validators + Pipeline
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// CORS y HealthChecks
builder.Services.AddCors(o => o.AddPolicy("dev",
    p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.AddHealthChecks().AddSqlServer(
    builder.Configuration.GetConnectionString("LabDev")!);

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    if (HttpMethods.Head.Equals(ctx.Request.Method, StringComparison.OrdinalIgnoreCase) &&
        ctx.Request.Path.StartsWithSegments("/swagger"))
    {
        ctx.Request.Method = HttpMethods.Get;
    }
    await next();
});

// --- swagger SIEMPRE habilitado  ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevLab API v1");
});

app.UseSerilogRequestLogging();
app.UseCors("dev");

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
