using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Outbox.Application.AutoMapper;
using Outbox.Application.Persistent;
using Outbox.Domain.Handlers;
using Outbox.Infra.Data;
using Outbox.Services.Api.Background;
using Outbox.Services.Api.Middleware;
using Prometheus;
using Quartz;
using Services.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomLogging(builder.Configuration);
builder.Services.AddCustomTelemetry(builder.Configuration);

builder.Services.AddQuartz(q =>
{
	q.UseInMemoryStore();
});
builder.Services.AddQuartzServer(options =>
{
	options.WaitForJobsToComplete = true;
});

builder.Services.AddOptions<KafkaOptions>();

builder.Services.AddDbContext<OutboxDbContext>(
	x => x.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.RegisterApplicationServices();
builder.Services.RegisterRepositoriesServices();
builder.Services.AddLocalization(options => options.ResourcesPath = ".");


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateOrder.CreateOrderRequestHandler>());
builder.Services.AddAutoMapper(typeof(OrdersProfile));

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