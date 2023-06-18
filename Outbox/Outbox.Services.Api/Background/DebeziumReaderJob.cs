using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Quartz;

namespace Outbox.Services.Api.Background;

public class DebeziumReaderJob : IJob
{
	private IOptions<KafkaOptions> _options;
	private ILogger<DebeziumReaderJob> _logger;

	public DebeziumReaderJob(IOptions<KafkaOptions> options, ILogger<DebeziumReaderJob> logger)
	{
		_options = options;
		_logger = logger;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var config = new ConsumerConfig
		{
			BootstrapServers = _options.Value.BootstrapServers,
			GroupId = "foo",
			AutoOffsetReset = AutoOffsetReset.Earliest
		};

		using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
		
		consumer.Subscribe("postgres.outbox.orders");

		while (true)
		{
			try
			{
				var consumeResult = consumer.Consume();
				
				using (CreateLoggingScope(consumeResult)) 
				{
					_logger.LogInformation("Consumed message: {@Message}", consumeResult.Message.Value);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error");
				throw;
			}
		}

		consumer.Close();
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
}

public class KafkaOptions
{
	public const string Key = "Kafka";

	public string BootstrapServers { get; set; } = default!;
}