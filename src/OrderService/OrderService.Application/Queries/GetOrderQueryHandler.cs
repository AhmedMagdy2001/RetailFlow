using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Queries;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderResponseDto>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(IOrderRepository repository, IMapper mapper, ILogger<GetOrderQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OrderResponseDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _repository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
                return new OrderResponseDto(false, "Order not found", null, "ORDER_NOT_FOUND");

            var orderDto = _mapper.Map<OrderDto>(order);
            return new OrderResponseDto(true, "Order retrieved successfully", orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order");
            return new OrderResponseDto(false, $"Error retrieving order: {ex.Message}", null, "ORDER_RETRIEVAL_ERROR");
        }
    }
}