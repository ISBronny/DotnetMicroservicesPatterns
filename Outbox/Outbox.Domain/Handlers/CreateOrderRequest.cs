using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Outbox.Domain.Interfaces;
using Outbox.Domain.Models;

namespace Outbox.Domain.Handlers;

public static class CreateOrder
{
	public class CreateOrderRequest : IRequest<CreateOrderResponse>
    {
    	public Guid CustomerId { get; set; }
    	public Guid MangerId { get; set; }
        public double Price { get; set; }
        
    }
	public class CreateOrderResponse
	{
		public Order? Order { get; set; }
		public bool IsValid { get; set; }
		public IDictionary<string, string[]> ValidationResult { get; set; }
	}
	
	public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
	{

		public CreateOrderRequestValidator(IStringLocalizer<CreateOrderRequestValidator> localizer)
		{
			RuleFor(c => c.Price)
				.GreaterThanOrEqualTo(0.0)
				.WithMessage(localizer["PricePositiveValue"]);
    
			RuleFor(c => c.CustomerId)
				.NotEqual(Guid.Empty);
    			
			RuleFor(c => c.MangerId)
				.NotEqual(Guid.Empty);
		}
    		
	}

    [UsedImplicitly]
    public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
    	private ILogger<CreateOrderRequestHandler> _logger;
    	private IOrderRepository _orderRepository;
        private CreateOrderRequestValidator _validator;
    
    	public CreateOrderRequestHandler(ILogger<CreateOrderRequestHandler> logger, IOrderRepository orderRepository, CreateOrderRequestValidator validator)
    	{
    		_logger = logger;
    		_orderRepository = orderRepository;
            _validator = validator;
        }
    
    	public async Task<CreateOrderResponse> Handle(CreateOrderRequest message, CancellationToken cancellationToken)
        {
	        var validationResult = await _validator.ValidateAsync(message, cancellationToken);
	        if (!validationResult.IsValid)
    		{
    			using (_logger.BeginScope(validationResult.Errors))
    			{
    				_logger.LogError("CreateOrderCommand validation error");
    			}
    			return new CreateOrderResponse()
                {
	                ValidationResult = validationResult.ToDictionary(),
	                IsValid = false
                };
    		}
    
    		var order = new Order()
    		{
    			Id = Guid.NewGuid(),
    			CustomerId = message.CustomerId,
    			MangerId = message.MangerId,
    			State = OrderState.Created,
    			Price = message.Price,
    			CreatedAt = DateTime.UtcNow,
    		};


            _orderRepository.Add(order);
    		
    		await _orderRepository.SaveChangesAsync();

            return new CreateOrderResponse()
            {
	            Order = order,
	            IsValid = validationResult.IsValid,
	            ValidationResult = validationResult.ToDictionary()
            };
        }
    }

}
