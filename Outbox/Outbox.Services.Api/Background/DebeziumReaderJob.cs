using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Outbox.Domain.Events;
using Quartz;

namespace Outbox.Services.Api.Background;

public class DebeziumReaderJob : BackgroundService
{
	private IOptions<KafkaOptions> _options;
	private ILogger<DebeziumReaderJob> _logger;

	public DebeziumReaderJob(IOptions<KafkaOptions> options, ILogger<DebeziumReaderJob> logger)
	{
		_options = options;
		_logger = logger;
	}
	

	private IDisposable? CreateLoggingScope<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult)
	{
		return _logger.BeginScope(new Dictionary<string, object>()
		{
			["Offset"] = consumeResult.Offset,
			["Topic"] = consumeResult.Topic,
			["Partition"] = consumeResult.Partition,
		});
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Factory.StartNew(() =>
			ConsumeTopic(stoppingToken),
		stoppingToken,
		TaskCreationOptions.LongRunning,
		TaskScheduler.Current);

	private Task ConsumeTopic(CancellationToken stoppingToken)
	{
		var config = new ConsumerConfig
		{
			BootstrapServers = _options.Value.BootstrapServers,
			GroupId = "Outbox",
			AutoOffsetReset = AutoOffsetReset.Earliest,
			PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range
		};

		using var consumer = new ConsumerBuilder<string, OrderCreatedEvent>(config).SetValueDeserializer(new KafkaDeserializer<OrderCreatedEvent>()).Build();
		
		consumer.Subscribe("postgres.public.orders");

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				var consumeResult = consumer.Consume(stoppingToken);
				
				using (CreateLoggingScope(consumeResult)) 
				{
					_logger.LogInformation("Consumed message: {@Message}", consumeResult.Message.Value);
				}

				consumer.Commit();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error");
				consumer.Unsubscribe();
				throw;
			}
		}
		
		consumer.Unsubscribe();

		return Task.CompletedTask;
	}
}

internal sealed class KafkaDeserializer<T> : IDeserializer<T>
{
	public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
	{
		var dataJsonString = Encoding.UTF8.GetString(data);

		var jObject = JObject.Parse(dataJsonString);

		return jObject["payload"]["after"].ToObject<T>();
	}
}


public class KafkaOptions
{
	public const string Key = "Kafka";

	public string BootstrapServers { get; set; } = default!;
}