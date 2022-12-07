using dotnet_playground;
using dotnet_playground.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


const string ServiceName = "Playground";

try
{
    var builder = WebApplication.CreateBuilder(args);

    // MassTransit
    var uriBuilder = new UriBuilder(builder.Configuration.GetConnectionString("rabbit"));
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<RequestConsumer>();
        x.AddConsumer<ChainConsumer>();
        
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(uriBuilder.Uri, h =>
            {
                h.Username(uriBuilder.UserName);
                h.Password(uriBuilder.Password);
            });

            cfg.ConfigureEndpoints(context);
            // cfg.ConfigurePublish(callback => callback.UseExecute(BusConfigurator.SetContextHeaders));

            // cfg.UseFilter(new OperationContextFilter<ConsumeContext>());
            // cfg.UseConsumeFilter(typeof(LoggingFilter<>), context);
            cfg.UseInMemoryOutbox();
        });
    });

    
    // MVC
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers(c =>
        {
            // c.Filters.Add<ExceptionHandlingFilter>();
        })
        .AddNewtonsoftJson(settings =>
        {
            // settings.SerializerSettings.InitSettings();
        });
    
    builder.Services.AddHealthChecks();
    
    // Diagnostics
    builder.Services.AddOpenTelemetryTracing(builder => builder
        // .AddAspNetCoreInstrumentation(options => { options.RecordException = true; })
        .AddMassTransitInstrumentation()
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddTelemetrySdk()
                .AddService(serviceName: ServiceName, serviceVersion: "1.0.0")
                // .AddDigmaAttributes(opt =>
                // {
                //     opt.NamespaceRoot = "Quali";
                //     opt.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); ;
                // })
        )
        // .AddOtlpExporter(c =>
        // {
        //     c.Endpoint = new Uri(digmaSettings.CollectorUrl);
        // })
        .AddSource("*")
    );
    
        
    // Domain
    builder.Services.AddHostedService<Worker>();

    
    // Application
    builder.Logging.ClearProviders();
    // builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/api/health-check");
    
    app.Run();
}
catch (Exception)
{
    throw;
}
