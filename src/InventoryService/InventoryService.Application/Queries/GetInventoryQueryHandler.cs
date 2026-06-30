using AutoMapper;
using MediatR;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Queries;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, InventoryResponseDto>
{
    private readonly IInventoryRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInventoryQueryHandler> _logger;

    public GetInventoryQueryHandler(IInventoryRepository repository, IMapper mapper, ILogger<GetInventoryQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InventoryResponseDto> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var inventory = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
            if (inventory == null)
                return new InventoryResponseDto(false, "Product not found", null, "PRODUCT_NOT_FOUND");

            var dto = _mapper.Map<InventoryDto>(inventory);
            return new InventoryResponseDto(true, "Success", dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventory");
            return new InventoryResponseDto(false, $"Error: {ex.Message}", null, "RETRIEVAL_ERROR");
        }
    }
}