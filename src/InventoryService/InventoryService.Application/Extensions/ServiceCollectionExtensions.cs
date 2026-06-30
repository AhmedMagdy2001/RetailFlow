using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MediatR;
using FluentValidation;
using InventoryService.Application.Mappings;
using InventoryService.Application.Validators;
using InventoryService.Domain.Services;

namespace InventoryService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ServiceCollectionExtensions).Assembly));
        services.AddValidatorsFromAssemblyContaining<AddInventoryValidator>();
        services.AddScoped<IInventoryDomainService, InventoryDomainService>();

        return services;
    }
}