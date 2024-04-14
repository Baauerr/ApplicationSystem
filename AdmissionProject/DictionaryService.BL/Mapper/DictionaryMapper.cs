﻿using AutoMapper;
using DictionaryService.Common.DTO;
using DictionaryService.DAL.Entities;
using System.Runtime;

namespace DictionaryService.BL.Mapper
{
    public class DictionaryMapper: Profile
    {
        public DictionaryMapper() {
            CreateMap<DocumentTypeDTO, DocumentType>()
                .ForMember(dest => dest.EducationLevelId, opt => opt.MapFrom(src => src.EducationLevel.Id))
                .ForMember(dest => dest.NextEducationLevels, opt => opt.Ignore());
            CreateMap<ProgramsDTO, Program>()
                .ForMember(dest => dest.FacultyId, opt => opt.MapFrom(src => src.Faculty.Id))
                .ForMember(dest => dest.EducationLevelId, opt => opt.MapFrom(src => src.EducationLevel.Id));
            CreateMap<Faculty, FacultiesResponseDTO>();
            CreateMap<EducationLevel, EducationLevelResponseDTO>();
        }
    }
}