using AutoMapper;
using Domain.Models;
namespace Domain.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Customer, LeavePlanner.Infrastructure.Entities.Customer>().ForMember(dest => dest.Date, opt => opt
                                                                   .MapFrom(src => DateTime.Now));
            CreateMap<LeavePlanner.Infrastructure.Entities.Customer, Customer>();
            CreateMap<Employee, LeavePlanner.Infrastructure.Entities.Employee>().ReverseMap();
            CreateMap<Job, LeavePlanner.Infrastructure.Entities.Job>().ReverseMap();
            CreateMap<Department, LeavePlanner.Infrastructure.Entities.Department>().ReverseMap();
            CreateMap<PersonalEvent, LeavePlanner.Infrastructure.Entities.PersonalEvent>().ReverseMap();
            CreateMap<Document, LeavePlanner.Infrastructure.Entities.Document>().ReverseMap();
            CreateMap<Event, LeavePlanner.Infrastructure.Entities.Event>().ReverseMap();
        }
    }
}
