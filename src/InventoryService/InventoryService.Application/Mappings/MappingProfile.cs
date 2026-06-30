using AutoMapper;
using InventoryService.Application.DTOs;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Inventory, InventoryDto>();
        CreateMap<AddInventoryDto, Inventory>()
            .ConstructUsing(src => new Inventory(src.ProductName, src.Quantity, src.Price));
    }
}