using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MediatR;
using FluentValidation;
using OrderService.Application.Mappings;
using OrderService.Application.Validators;
using OrderService.Domain.Services;

namespace OrderService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ServiceCollectionExtensions).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

        // Domain Services
        services.AddScoped<IOrderDomainService, OrderDomainService>();

        return services;
    }
}