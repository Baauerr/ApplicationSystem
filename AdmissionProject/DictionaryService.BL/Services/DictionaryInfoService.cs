using AutoMapper;
using Common.DTO.Dictionary;
using Common.DTO.Entrance;
using Common.Enum;
using DictionaryService.Common.Interfaces;
using DictionaryService.DAL;
using DictionaryService.DAL.Entities;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.WebSockets;

namespace DictionaryService.BL.Services
{
    public class DictionaryInfoService : IDictionaryInfoService
    {
        private readonly DictionaryDbContext _db;
        private readonly IMapper _mapper;

        public DictionaryInfoService(DictionaryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<DocumentTypeResponseDTO> GetDocumentTypes(string? documentName)
        {
            IQueryable<DocumentType> query = _db.DocumentTypes;


            if (documentName != null)
            {
                query = query.Where(e => e.Name.Contains(documentName));
            }

            var documentTypes = await query.ToListAsync();

            List<DocumentTypeDTO> documentTypesList = new List<DocumentTypeDTO>();
            var documentTypeResponseDTO = new DocumentTypeResponseDTO();

            foreach (var document in documentTypes)
            {
                var nextEducationLevels = await _db.NextEducationLevelDocuments
                    .Where(level => level.DocumentTypeId == document.Id)
                    .ToListAsync();
                var nextEducationLevelsList = new List<EducationLevelDTO>();

                foreach (var level in nextEducationLevels)
                {
                    var educationLevel = new EducationLevelDTO
                    {
                        Id = level.EducationLevelId,
                        Name = level.EducationLevelName,
                    };
                    nextEducationLevelsList.Add(educationLevel);
                }

                var currentEducationLevel = 
                    await _db.EducationLevels.FirstOrDefaultAsync(level => level.Id == document.EducationLevelId);
                var currentEducationLevelDTO = _mapper.Map<EducationLevelDTO>(currentEducationLevel);


                var documentType = _mapper.Map<DocumentTypeDTO>(document);
                documentType.EducationLevel = currentEducationLevelDTO;
                documentType.NextEducationLevels = nextEducationLevelsList;


                documentTypesList.Add(documentType);
            }

            documentTypeResponseDTO.DocumentTypes = documentTypesList;


            return documentTypeResponseDTO;
        }

        public async Task<EducationLevelResponseDTO> GetEducationLevel(string? educationLevelName)
        {
            IQueryable<EducationLevel> query = _db.EducationLevels;


            if (educationLevelName != null)
            {
                query = query.Where(e => e.Name.Contains(educationLevelName));
            }

            var educationLevels = await query.ToListAsync();

            List<EducationLevelDTO> educationLevelsList = new List<EducationLevelDTO>();
            var educationLevelsDTO = new EducationLevelResponseDTO();

            foreach (var level in educationLevels)
            {
                var educationLevelDTO = _mapper.Map<EducationLevelDTO>(level);
                educationLevelsList.Add(educationLevelDTO);
            }

            educationLevelsDTO.EducationLevel = educationLevelsList;

            return educationLevelsDTO;
        }

        public async Task<FacultiesResponseDTO> GetFaculties(string? facultyName)
        {
            IQueryable<Faculty> query = _db.Faculties;


            if (facultyName != null)
            {
                query = query.Where(e => e.Name.Contains(facultyName));
            }

            var facuilties = await query.ToListAsync();

            var facultiesResponseDTO = new FacultiesResponseDTO();

            var facultiesDTO = new List<FacultyDTO>();

            foreach(var faculty in facuilties)
            {
                var facultyDTO = _mapper.Map<FacultyDTO>(faculty);

                facultiesDTO.Add(facultyDTO);
            }

            facultiesResponseDTO.Faculties = facultiesDTO;

            return facultiesResponseDTO;
        }

        public async Task<ProgramResponseDTO> GetPrograms(
            string? name,
            string? code,
            List<Language>? language,
            List<string>? educationForm,
            List<string>? educationLevel,
            List<string>? faculty,
            int page,
            int pageSize
            )
        {
            List<Program> program = new List<Program>();
            program = await FilterPrograms(
                program,
                name,
                code,
                language,
                educationForm,
                educationLevel,
                faculty,
                page,
                pageSize
                );

            var programsCount = program.Count();

            program = FilterByPagination(program, page, pageSize);

            var programDTO = await CreateProgramDTO(program, pageSize, page, programsCount);

            return programDTO;
        }



        private async Task<List<Program>> FilterPrograms(
            List<Program> program,
            string? name,
            string? code,
            List<Language>? language,
            List<string>? educationForm,
            List<string>? educationLevel,
            List<string>? faculty,
            int page = 1,
            int pageSize = 10
            )
        {
            program = await FilterByNameAndCode(program, name, code);
            program = FilterByLanguage(program, language);
            program = FilterByEducationForm(program, educationForm);
            program = FilterByEducationLevel(program, educationLevel);
            program = FilterByFaculty(program, faculty);
            return program;
        }




        private async Task<List<Program>> FilterByNameAndCode(
            List<Program> program, 
            string? name,
            string? code
            )
        {
            var query = _db.Programs.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(f => f.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(f => f.Code.Contains(code));
            }

            var programs = 
                await (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(code) 
                ? _db.Programs.ToListAsync() : query.ToListAsync());

            return programs;
        }



        private  List<Program> FilterByLanguage(
            List<Program> programs,
            List<Language>? language
            )
        {
            
            if (language != null && language.Count() != 0)
            {
                programs = programs.Where(p => language.Contains(p.Language)).ToList();
            }
            return programs;
        }


        private  List<Program> FilterByEducationForm(
            List<Program> program,
            List<string>? educationForm
            )
        {
            if (educationForm != null && educationForm.Any())
            {
                program = program.Where(p => educationForm.Contains(p.EducationForm)).ToList();
            }
            return program;
        }



        private  List<Program> FilterByEducationLevel(
            List<Program> program,
            List<string>? educationLevel
            )
        {
            if (educationLevel != null && educationLevel.Any())
            {
                program = program.Where(p => educationLevel.Contains(p.EducationLevelId)).ToList();
            }
            return program;
        }

        private List<Program> FilterByFaculty(
            List<Program> program,
            List<string>? Faculty
            )
        {
            if (Faculty != null && Faculty.Any())
            {
                program = program.Where(p => Faculty.Contains(p.FacultyId)).ToList();
            }
            return program;
        }
        private Pagination GetPagination(int size, int page, int elementsCount)
        {
            var pagination = new Pagination
            {
                Count = (int)Math.Ceiling((double)elementsCount / size),
                Current = page,
                Size = size
            };
            return pagination;
        }

        private async Task<ProgramResponseDTO> CreateProgramDTO(
            List<Program> program,
            int size, 
            int page,
            int programsCount
            )
        {

            List<ProgramsDTO> programsElements = new List<ProgramsDTO>();
            
            foreach(var programElement in program)
            {
                var faculty = await _db.Faculties.FirstOrDefaultAsync(f => f.Id == programElement.FacultyId);
                var educationLevel = await _db.EducationLevels.FirstOrDefaultAsync(f => f.Id == programElement.EducationLevelId);

                var facultyDTO = _mapper.Map<FacultyDTO>(faculty);


                if ( faculty == null )
                {
                    throw new NotFoundException($"Такого факультета не существует") ;
                }

                if (educationLevel == null)
                {
                    throw new NotFoundException($"Такого уровня образования не существует");
                }

                EducationLevelDTO educationLevelDTO = new EducationLevelDTO
                {
                    Name = educationLevel.Name,
                    Id = educationLevel.Id,
                };

                var programDTO = new ProgramsDTO
                {
                    Code = programElement.Code,
                    Name = programElement.Name,
                    Language = programElement.Language,
                    EducationForm = programElement.EducationForm,
                    Faculty = facultyDTO,
                    EducationLevel = educationLevelDTO,
                    Id = programElement.Id,
                    CreateTime = programElement.CreateTime,
                };

                programsElements.Add(programDTO);
            }


            var programResponseDTO = new ProgramResponseDTO
            {
                Programs = programsElements,
                Pagination = GetPagination(size, page, programsCount)
            };

            return programResponseDTO;

        }


        private List<Program> FilterByPagination(List<Program> program, int page, int sizeOfPage)
        {
            if (page <= 0)
            {
                page = 1;
            }

            var countOfPages = (int)Math.Ceiling((double)program.Count() / sizeOfPage);

            if (page <= countOfPages)
            {
                var lowerBound = page == 1 ? 0 : (page - 1) * sizeOfPage;
                if (page < countOfPages)
                {
                    program = program.GetRange(lowerBound, sizeOfPage);
                }
                else
                {
                    program = program.GetRange(lowerBound, program.Count() - lowerBound);
                }
                return program;
            }
            else
            {
                throw new BadRequestException("Такой страницы нет");
            }
        }
    }
}
