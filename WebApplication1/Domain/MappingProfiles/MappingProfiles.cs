using AutoMapper;
using Domain.Models;
namespace Domain.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Customer, Infrastructure.Entities.Customer>().ForMember(dest => dest.Date, opt => opt
                                                                   .MapFrom(src => DateTime.Now));
            CreateMap<Infrastructure.Entities.Customer, Customer>();
            CreateMap<Employee, Infrastructure.Entities.Employee>().ReverseMap();
            CreateMap<Job, Infrastructure.Entities.Job>().ReverseMap();
            CreateMap<Department, Infrastructure.Entities.Department>().ReverseMap();
            CreateMap<PersonalEvent, Infrastructure.Entities.PersonalEvent>().ReverseMap();
            CreateMap<Document, Infrastructure.Entities.Document>().ReverseMap();
            CreateMap<Event, Infrastructure.Entities.Event>().ReverseMap();
        }
    }
}
