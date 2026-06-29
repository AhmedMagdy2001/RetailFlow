using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Domain.Services;
using Shared.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    private readonly IOrderRepository _repository;
    private readonly IOrderDomainService _domainService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IOrderDomainService domainService,
        IMessagePublisher messagePublisher,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _repository = repository;
        _domainService = domainService;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = _mapper.Map<Order>(request.Dto);

            // Validate using domain service
            var validationResult = await _domainService.ValidateOrderAsync(order, cancellationToken);
            if (!validationResult.Success)
                return new OrderResponseDto(false, validationResult.Message, null, validationResult.ErrorCode);

            // Save to repository
            await _repository.AddAsync(order, cancellationToken);

            // Publish event for async processing
            await _messagePublisher.PublishAsync("order-queue", order, cancellationToken);

            _logger.LogInformation("Order created: {OrderId}", order.Id);

            var orderDto = _mapper.Map<OrderDto>(order);
            return new OrderResponseDto(true, "Order created successfully", orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return new OrderResponseDto(false, $"Error creating order: {ex.Message}", null, "ORDER_CREATION_ERROR");
        }
    }
}