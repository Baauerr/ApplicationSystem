using AutoMapper;
using DocumentService.Common.DTO;
using DocumentService.DAL.Entity;

namespace DocumentService.BL.Mapper
{
    public class DocumentMapper : Profile
    {
        public DocumentMapper()
        {
            CreateMap<PassportForm, PassportFormDTO>().ReverseMap();
            CreateMap<PassportForm, GetPassportFormDTO>().ReverseMap();
            CreateMap<EducationDocumentForm, EducationDocumentFormDTO>().ReverseMap();
            CreateMap<EducationDocumentForm, GetEducationDocumentFormDTO>().ReverseMap();
        }
    }
}
