using AutoMapper;
using CitizenAppeals.Server.Model;
using CitizenAppeals.Shared.Dto;

namespace CitizenAppeals.Server.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Appeal, AppealDto>().ForMember(dto => dto.FullName, opt => opt.MapFrom(a => $"{a.Citizen.LastName} {a.Citizen.FirstName} {a.Citizen.MiddleName}"))
                .ForMember(dto => dto.Executors, opt => opt.MapFrom(a => string.Join(", ", a.Executors.Select(e => $"{e.LastName} {e.FirstName} {e.MiddleName}")))); ;
        }
    }
}
