using AutoMapper;
using EntranceService.Common.DTO;
using EntranceService.DAL.Entity;

namespace EntranceService.BL.Mapper
{
    public class EntranceMapper: Profile
    {
        public EntranceMapper() {
            CreateMap<Application, CreateApplicationDTO>();
            CreateMap<Application, GetApplicationDTO>().ReverseMap();
            CreateMap<ApplicationPrograms, GetProgramDTO>();
        }
    }
}
