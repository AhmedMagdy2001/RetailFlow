using MediatR;

namespace OrderService.Application.Queries;

public record GetAllOrdersQuery : IRequest<List<DTOs.OrderDto>>;