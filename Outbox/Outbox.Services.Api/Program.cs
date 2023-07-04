using Microsoft.EntityFrameworkCore;
using Outbox.Application.AutoMapper;
using Outbox.Application.Persistent;
using Outbox.Domain.Handlers;
using Outbox.Infra.Data;
using Outbox.Services.Api.Background;
using Outbox.Services.Api.Middleware;
using Prometheus;
using Serilog;
using Services.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Host.UseSerilog();

builder.Services.AddCustomLogging(builder.Configuration);
builder.Services.AddCustomTelemetry(builder.Configuration);

builder.Services.AddOptions<KafkaOptions>().Bind(builder.Configuration.GetSection(KafkaOptions.Key));

builder.Services.AddDbContext<OutboxDbContext>(
	x => x.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.RegisterApplicationServices();
builder.Services.RegisterRepositoriesServices();
builder.Services.AddLocalization(options => options.ResourcesPath = ".");


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateOrder.CreateOrderRequestHandler>());
builder.Services.AddAutoMapper(typeof(OrdersProfile));
	
builder.Services.AddHostedService<DebeziumReaderJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapMetrics();

app.Run();