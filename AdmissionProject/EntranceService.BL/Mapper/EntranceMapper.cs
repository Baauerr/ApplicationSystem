using AutoMapper;
using EntranceService.Common.DTO;
using EntranceService.DAL.Entity;

namespace EntranceService.BL.Mapper
{
    public class EntranceMapper: Profile
    {
        public EntranceMapper() {
            CreateMap<Application, CreateApplicationDTO>();
            CreateMap<PassportData, PassportInfoDTO>().ReverseMap();
            CreateMap<EducationDocumentData, EducationDocumentDTO>().ReverseMap();
        }
    }
}
