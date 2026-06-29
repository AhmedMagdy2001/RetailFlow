using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Queries;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(IOrderRepository repository, IMapper mapper, ILogger<GetAllOrdersQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<OrderDto>>(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all orders");
            throw;
        }
    }
}