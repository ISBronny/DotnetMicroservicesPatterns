using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using Serilog;

namespace Services.Hosting;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCustomLogging(this IServiceCollection services, IConfiguration configuration)
	{
		var loggerConfiguration = new LoggerConfiguration()
			.Enrich.FromLogContext()
			.WriteTo.Seq(configuration.GetConnectionString("Seq")!, period: TimeSpan.FromMilliseconds(100))
			.WriteTo.Console();
		
		return services.AddSerilog(loggerConfiguration.CreateLogger());
	}
	
	public static IServiceCollection AddCustomTelemetry(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOpenTelemetry()
			.WithTracing(c =>
			{
				c.AddZipkinExporter();
			}).WithMetrics(c =>
			{
				
			});

		return services;
	}
	
}
