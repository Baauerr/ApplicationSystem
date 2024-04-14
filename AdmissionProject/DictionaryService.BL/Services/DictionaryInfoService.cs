using DictionaryService.Common.DTO;
using DictionaryService.Common.Enums;
using DictionaryService.Common.Interfaces;
using DictionaryService.DAL;
using DictionaryService.DAL.Entities;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using System;

namespace DictionaryService.BL.Services
{
    public class DictionaryInfoService : IDictionaryInfoService
    {
        private readonly DictionaryDbContext _db;

        public DictionaryInfoService(DictionaryDbContext db)
        {
            _db = db;
        }

        public async Task<DocumentTypeDTO> GetDocumentTypes(string? documentName)
        {
            throw new NotImplementedException();
        }

        public async Task<EducationLevelResponseDTO> GetEducationLevel(string? educationLevelNamme)
        {
            throw new NotImplementedException();
        }

        public async Task<FacultiesResponseDTO> GetFaculties(string? facultyName)
        {
            throw new NotImplementedException();
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

            Console.WriteLine(program.Count());

            var programDTO = await CreateProgramDTO(program, pageSize, page);
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
            program = FilterByPagination(program, page, pageSize);
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
                count = (int)Math.Ceiling((double)elementsCount / size),
                current = page,
                size = size
            };
            return pagination;
        }

        private async Task<ProgramResponseDTO> CreateProgramDTO(
            List<Program> program,
            int size, 
            int page
            )
        {

            List<ProgramsDTO> programsElements = new List<ProgramsDTO>();
            
            foreach(var programElement in program)
            {
                var faculty = await _db.Faculties.FirstOrDefaultAsync(f => f.Id == programElement.Id);
                var educationLevel = await _db.EducationLevels.FirstOrDefaultAsync(f => f.Id == programElement.Id);

                if ( faculty != null )
                {
                    throw new NotFoundException($"Такого факультета не существует") ;
                }

                if (educationLevel != null)
                {
                    throw new NotFoundException($"Такого уровня образования не существует");
                }

                var programDTO = new ProgramsDTO
                {
                    Code = programElement.Code,
                    Name = programElement.Name,
                    Language = programElement.Language,
                    EducationForm = programElement.EducationForm,
                    Faculty = faculty,
                    EducationLevel = educationLevel,
                    Id = programElement.Id             
                };

                programsElements.Add(programDTO);
            }


            var programResponseDTO = new ProgramResponseDTO
            {
                programs = programsElements,
                pagination = GetPagination(size, page, programsElements.Count())
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
