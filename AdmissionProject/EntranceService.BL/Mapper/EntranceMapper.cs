using AutoMapper;
using Common.DTO.Entrance;
using EntranceService.DAL.Entity;

namespace EntranceService.BL.Mapper
{
    public class EntranceMapper: Profile
    {
        public EntranceMapper() {
            CreateMap<Application, CreateApplicationDTO>();
            CreateMap<Application, GetApplicationDTO>().ReverseMap();
            CreateMap<ApplicationPrograms, GetProgramDTO>();
            CreateMap<ManagerDTO, Manager>().ReverseMap();
        }
    }
}
