using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationService;
using Persistence;
using ServiceDefaults.Extensions;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Configuration["USE_ASPIRE"] == "true")
    builder.AddServiceDefaults();

builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();
