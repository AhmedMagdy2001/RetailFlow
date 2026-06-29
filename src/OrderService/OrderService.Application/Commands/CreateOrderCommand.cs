using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands;

public record CreateOrderCommand(CreateOrderDto Dto) : IRequest<OrderResponseDto>;