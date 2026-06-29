using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries;

public record GetOrderQuery(int OrderId) : IRequest<OrderResponseDto>;