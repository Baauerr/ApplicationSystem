using AutoMapper;
using EntranceService.Common.DTO;
using EntranceService.DAL.Entity;

namespace EntranceService.BL.Mapper
{
    public class MapperConfig: Profile
    {
        public MapperConfig() {
            CreateMap<Application, CreateApplicationDTO>();
            CreateMap<ApplicationPrograms, ProgramDTO>();
            CreateMap<PassportData, PassportInfoDTO>();
            CreateMap<EducationDocumentData, EducationDocumentDTO>();
        }
    }
}
