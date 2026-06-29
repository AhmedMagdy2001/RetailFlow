using AutoMapper;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.GetTotalPrice()));

        CreateMap<CreateOrderDto, Order>()
            .ConstructUsing(src => new Order(src.ProductName, src.Quantity, src.Price));
    }
}