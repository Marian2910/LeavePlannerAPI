using AutoMapper;
using Common.DTOs;
using LeavePlanner.Domain.Models;

namespace LeavePlanner.Api.MappingProfiles
{
    public class BackendMappingProfiles : Profile
    {
        public BackendMappingProfiles()
        {
            CreateMap<CustomerDto, Customer>().ReverseMap();
            CreateMap<UpdateCustomerDto, Customer>().ReverseMap();
            CreateMap<PersonalEventDto, PersonalEvent>().ReverseMap();
            CreateMap<EventDto, Event>().ReverseMap();
        }
    }
}
