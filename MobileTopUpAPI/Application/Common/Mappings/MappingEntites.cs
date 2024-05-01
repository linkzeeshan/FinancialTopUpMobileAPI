using Application.Common.Models.Dtos;
using AutoMapper;
using MobileTopUpAPI.Application.Common.Models.Dtos;
using MobileTopUpAPI.Domain.Entities;

namespace Application.Common.Mappings
{
    public class MappingEntites : Profile
    {
        public MappingEntites() 
        {
            CreateMap<User, UserCreateDto>().ReverseMap();
            CreateMap<User, UserReadDto>().ReverseMap();
            CreateMap<UserCreateDto, UserReadDto>().ReverseMap();

            CreateMap<Beneficiary, BeneficiaryCreateDto>().ReverseMap();
            CreateMap<Beneficiary, BeneficiaryReadDto>().ReverseMap();
            CreateMap<BeneficiaryCreateDto, BeneficiaryReadDto>().ReverseMap();


            CreateMap<TopUpTransaction, TopUpTransactionCreateDto>().ReverseMap();
            CreateMap<TopUpTransaction, TopUpTransactionReadDto>().ReverseMap();
            CreateMap<TopUpTransactionCreateDto, BeneficiaryReadDto>().ReverseMap();
            CreateMap<TopUpTransactionCreateDto, TopUpTransactionRequest>().ReverseMap();

            CreateMap<Balance, BalanceCreateDto>().ReverseMap();
            CreateMap<Balance, BalanceReadDto>().ReverseMap();
            CreateMap<BalanceCreateDto, BalanceReadDto>().ReverseMap();


            //CreateMap<ChatAttachment, AttachmentCreateDto>().ReverseMap();

            //CreateMap<ChatRoom, ChatRoomCreateDto>().ReverseMap();


            //CreateMap<Order, OrderCreateDto>().ReverseMap();
            //CreateMap<Order, OrderReadDto>().ReverseMap()
            //    .ForMember(dest => dest.Designs, opt => opt.MapFrom(src => src.Designs))
            //    .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            //    .ReverseMap();

            //CreateMap<Design, DesignCreateDto>().ReverseMap();
            //CreateMap<Design, DesignReadDto>().ReverseMap()
            //    .ForMember(dest => dest.ProductItems, opt => opt.MapFrom(src => src.ProductItems))
            //    .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
            //    .ReverseMap();

            //CreateMap<ProductItem, ProductItemCreateDto>().ReverseMap();
            //CreateMap<ProductItem, ProductItemReadDto>().ReverseMap();

        }
    }
}
