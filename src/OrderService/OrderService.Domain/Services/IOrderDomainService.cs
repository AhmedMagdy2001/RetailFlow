using Shared.Domain.Core;
using OrderService.Domain.Entities;

namespace OrderService.Domain.Services;

public interface IOrderDomainService
{
    Task<Result> ValidateOrderAsync(Order order, CancellationToken cancellationToken = default);
    decimal CalculateOrderTotal(Order order);
}