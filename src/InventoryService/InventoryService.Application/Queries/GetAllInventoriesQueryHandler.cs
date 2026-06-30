using AutoMapper;
using MediatR;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Queries;

public class GetAllInventoriesQueryHandler : IRequestHandler<GetAllInventoriesQuery, List<InventoryDto>>
{
    private readonly IInventoryRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllInventoriesQueryHandler> _logger;

    public GetAllInventoriesQueryHandler(IInventoryRepository repository, IMapper mapper, ILogger<GetAllInventoriesQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<InventoryDto>> Handle(GetAllInventoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var inventories = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<InventoryDto>>(inventories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving inventories");
            throw;
        }
    }
}