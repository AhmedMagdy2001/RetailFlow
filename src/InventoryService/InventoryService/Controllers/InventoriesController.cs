using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Application.DTOs;
using InventoryService.Application.Queries;
using FluentValidation;

namespace InventoryService.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<AddInventoryDto> _validator;
    private readonly ILogger<InventoriesController> _logger;

    public InventoriesController(IMediator mediator, IValidator<AddInventoryDto> validator, ILogger<InventoriesController> logger)
    {
        _mediator = mediator;
        _validator = validator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<InventoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllInventoriesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{productId}")]
    [ProducesResponseType(typeof(InventoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(InventoryResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int productId, CancellationToken cancellationToken)
    {
        var query = new GetInventoryQuery(productId);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "InventoryService" });
    }
}