using AutoMapper;
using Common.DTO.Dictionary;
using DictionaryService.DAL.Entities;

namespace DictionaryService.BL.Mapper
{
    public class DictionaryMapper: Profile
    {
        public DictionaryMapper() {
            CreateMap<DocumentTypeDTO, DocumentType>()
                .ForMember(dest => dest.EducationLevelId, opt => opt.MapFrom(src => src.EducationLevel.Id))
                .ForMember(dest => dest.NextEducationLevels, opt => opt.Ignore());
            CreateMap<DocumentType, DocumentTypeDTO>()
                .ForMember(dest => dest.EducationLevel, opt => opt.Ignore())
                .ForMember(dest => dest.NextEducationLevels, opt => opt.Ignore());
            CreateMap<ProgramsDTO, Program>()
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.Faculty.Id))
                .ForMember(dest => dest.EducationLevelId, opt => opt.MapFrom(src => src.EducationLevel.Id));
            CreateMap<Faculty, FacultiesResponseDTO>();

            CreateMap<EducationLevelDTO, EducationLevel>()
                .ReverseMap();
            CreateMap<Faculty, FacultyDTO>()
                .ReverseMap();

            CreateMap<Faculty, FacultyDTO>()
                .ReverseMap();
        }
    }
}
