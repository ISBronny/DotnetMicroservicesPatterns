using FlueFlame.AspNetCore;
using FlueFlame.Http.Host;
using FlueFlame.Serialization.Newtonsoft;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Outbox.AcceptanceTests;

public abstract class TestBase : IDisposable
{
	protected IFlueFlameHttpHost HttpHost { get; }
	protected IServiceProvider ServiceProvider { get; }
	protected TestServer TestServer { get; }

	protected TestBase()
	{
		var webApp = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					services.RemoveAll<IHostedService>();
				});
			});

		TestServer = webApp.Server;
		ServiceProvider = webApp.Services;

		var builder = FlueFlameAspNetBuilder.CreateDefaultBuilder(webApp)
			.ConfigureHttpClient(c =>
			{

			});

		HttpHost = builder.BuildHttpHost(b =>
		{
			//Use System.Text.Json serializer
			b.UseNewtonsoftJsonSerializer();
			
			//Configure HttpClient only for FlueFlameHttpHost
			b.ConfigureHttpClient(client =>
			{
				
			});
		});
		
	}

	public void Dispose()
	{
		TestServer.Dispose();
	}
}