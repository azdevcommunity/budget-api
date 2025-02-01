using AutoMapper;
using BudgetApi.Dto.Customer;
using BudgetApi.Entities;

namespace BudgetApi.Mapper;

public class CustomerMapper : Profile
{
    public CustomerMapper()
    {
        CreateMap<CustomerCreateRequest, Customer>();
        CreateMap<CustomerUpdateRequest, Customer>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); 
        
        CreateMap<Customer, CustomerResponse>();
            
        CreateMap<Customer, CustomerDebtResponse>()
            .ForMember(dest => dest.DebtEvents, opt => opt.MapFrom(src => src.DebtEvents));
        
    }
}