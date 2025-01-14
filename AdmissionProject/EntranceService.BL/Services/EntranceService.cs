﻿using EntranceService.Common.Interface;
using EntranceService.DAL;
using AutoMapper;
using EntranceService.DAL.Entity;
using Exceptions.ExceptionTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Common.DTO.User;
using Common.Enum;
using NotificationService.DTO;
using Common.DTO.Dictionary;
using Common.Const;
using Common.DTO.Entrance;
using Common.DTO.Copy;


namespace EntranceService.BL.Services
{
    public class EntrancingService : IEntranceService
    {
        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly QueueSender _queueSender;


        public EntrancingService(
            EntranceDbContext entranceDbContext,
            IMapper mapper,
            IConfiguration configuration,
            QueueSender queueSender
            )
        {
            _db = entranceDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _queueSender = queueSender;
        }

        public async Task AddProgramsToApplication(List<ProgramDTO> programsDTO, Guid aplicationId, Guid userId)
        {

            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == aplicationId);

            if (application != null && application.ApplicationStatus == ApplicationStatus.Closed)
            {
                throw new BadRequestException("Нельзя добавлять программы, если заявление закрыто");
            }

            List<ApplicationPrograms> newPrograms = new List<ApplicationPrograms>();

            var programsFilter = new ProgramsFilterDTO();
            programsFilter.pageSize = 1000;

            var availablePrograms = await _queueSender.GetAllPrograms(programsFilter, Guid.NewGuid());

            await ValidatePrograms(programsDTO, userId, availablePrograms, aplicationId);

            programsDTO.ForEach(program =>
            {

                var allProgramInfo = availablePrograms.Programs.FirstOrDefault(ap => ap.Id == program.ProgramId);

                var applicationPrograms = new ApplicationPrograms
                {
                    ApplicationId = aplicationId,
                    Priority = program.Priority,
                    ProgramId = program.ProgramId,
                    FacultyId = program.FacultyId,
                    ProgramName = allProgramInfo.Name,
                    FacultyName = allProgramInfo.Faculty.Name,
                    
                };

                newPrograms.Add(applicationPrograms);
            });

            await _db.ApplicationsPrograms.AddRangeAsync(newPrograms);
            await _db.SaveChangesAsync();
        }

        private async Task ValidatePrograms(List<ProgramDTO> programsDTO, Guid userId, ProgramResponseDTO availablePrograms, Guid applicationId)
        {

            var maxProgramsCount = _configuration.GetValue<int>("Programs:MaxProgramsCount");

            var applicationPrograms = _db.ApplicationsPrograms
                .Where(application => application.ApplicationId == applicationId);

            var applicationProgramsCount = applicationPrograms.Count();

            var newProgramsCount = programsDTO.Count();

            if (newProgramsCount + applicationProgramsCount > maxProgramsCount)
            {
                throw new BadRequestException($"Число программ в одном заявлении не может быть больше {maxProgramsCount}");
            }

            var maxPriority = programsDTO.OrderByDescending(program => program.Priority).FirstOrDefault();

            if (maxPriority.Priority > (newProgramsCount + applicationProgramsCount) || maxPriority.Priority < 1)
            {
                throw new BadRequestException("Приоритет программы не может быть больше количества программ в заявке и не может быть меньше 1");
            }

            var hasDublicatePriority = programsDTO.GroupBy(x => x.Priority)
                .Where(program => program.Count() > 1)
                .Select(program => program.Key)
                .Any();

            var list1Priorities = programsDTO.Select(item => item.Priority);
            var list2Priorities = applicationPrograms.Select(item => item.Priority);

            var hasDuplicatesWithExistsPrograms = list1Priorities.Intersect(list2Priorities).Any();

            if (hasDublicatePriority || hasDuplicatesWithExistsPrograms)
            {
                throw new BadRequestException("Нельзя добавить программы с одинаковыми приоритетами");
            }

            availablePrograms.Programs.RemoveAll(program => !programsDTO.Any(newProgram => newProgram.ProgramId == program.Id));

            var firstEducationLevelId = availablePrograms.Programs.First().EducationLevel.Id;

            bool allSameId = availablePrograms.Programs.All(program => program.EducationLevel.Id == firstEducationLevelId);

            if (!allSameId)
            {
                throw new BadRequestException("Нельзя добавить в одно заявление программы с разным уровнем образования");
            }

            var isProgramsExist = availablePrograms.Programs.Count() == programsDTO.Count();

            if (!isProgramsExist)
            {
                throw new NotFoundException("В списке есть несуществующая программа или программы повторяются");
            }

            var hasBublicatePrograms = programsDTO
                .GroupBy(x => x)
                .Where(program => program.Count() > 1)
                .Select(program => program.Key)
                .ToList();

            if (hasBublicatePrograms.Any())
            {
                throw new BadRequestException("Группы не могут повторяться в заявлении");
            }

            var edcuationLevelIsNormal = await CheckEducationLevel(firstEducationLevelId, userId );

            if (!edcuationLevelIsNormal)
            {
                throw new BadRequestException("Ваш уровень образования не подходит для данной программы");
            }
        }

        public async Task SyncProgramsWithEducationDocument(Guid userId)
        {
            var applicationId = await _db.Applications
                .Where(ap => ap.OwnerId == userId)
                .Select(ap => ap.Id)
                .FirstOrDefaultAsync();

            var programs = _db.ApplicationsPrograms.Where(ap => ap.ApplicationId == applicationId);

            var firstProgram = await programs.FirstOrDefaultAsync();

            if (programs.Any())
            {
                var programsFilter = new ProgramsFilterDTO();
                programsFilter.faculty = new List<string> { firstProgram.FacultyId };

                var allPrograms = await _queueSender.GetAllPrograms(programsFilter,userId);
                var educationLevel = allPrograms.Programs.Select(p => p.EducationLevel).FirstOrDefault();
                        

                var programsAreNormal = await CheckEducationLevel(educationLevel.Id, userId);

                if (!programsAreNormal)
                {
                    _db.ApplicationsPrograms.RemoveRange(programs);
                    await _db.SaveChangesAsync();
                }
            }
            
        }

        public async Task ChangeApplicationStatus(ApplicationStatus status, Guid applicationId)
        {
            var application = await _db.Applications
                .Where(ap => ap.Id == applicationId)
                .FirstOrDefaultAsync();

            if (application == null)
            {
                throw new NotFoundException("Заявления с таким id не существует");
            }

            application.ApplicationStatus = status;
            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();

            _db.Applications.Update(application);

            await _db.SaveChangesAsync();

            var message = new MailStructure
            {
                Recipient = application.OwnerEmail,
                Body = $"У вашего заявления изменился статус на {status}",
                Subject = "Изменение статуса заявления"
            };

            await SendNotification(message);
        }

        public async Task ChangeProgramPriority(ChangeProgramPriorityDTO changeProgramPriorityDTO, Guid ownerId, Guid? managerId)
        {
            var application = _db.Applications.FirstOrDefault(ap => ap.OwnerId == ownerId);

            if (application == null)
            {
                throw new NotFoundException("Заявления с таким id не существует"); 
            }

            if (application.OwnerId != ownerId || application.ManagerId != managerId)
            {
                throw new ForbiddenException("Нет доступа к этой заявке");
            }

            var applicationId = await _db.Applications
                .Where(ap => ap.OwnerId == ownerId)
                .Select(ap => ap.Id)
                .FirstOrDefaultAsync();

            var programs = await _db.ApplicationsPrograms
                .Where(ap => ap.ApplicationId == applicationId)
                .ToListAsync();

            var programToEdit = programs.FirstOrDefault(p => p.ProgramId == changeProgramPriorityDTO.ProgramId);

            
            if (programToEdit == null)
            {
                throw new NotFoundException("В этой заявке нет такой программы");
            }

            var maxProgramsCount = _configuration.GetValue<int>("Programs:MaxProgramsCount");

            if (programs.Count() < changeProgramPriorityDTO.Priority && changeProgramPriorityDTO.Priority < 1)
            {
                throw new BadRequestException("Приоритет не может быть больше количества программ и меньше 1");
            }

            var programForSwap = programs.FirstOrDefault(p => p.Priority == changeProgramPriorityDTO.Priority);

            programForSwap.Priority = programToEdit.Priority;

            programToEdit.Priority = changeProgramPriorityDTO.Priority;

            _db.ApplicationsPrograms.Update(programToEdit);
            _db.ApplicationsPrograms.Update(programForSwap);

            await _db.SaveChangesAsync();

        }

        public async Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO)
        {  
            await CheckPasportExist(userId);

            var applicationId = Guid.NewGuid();
      
            await AddProgramsToApplication(applicationDTO.Programs, applicationId, userId);

            var userInfo = await _queueSender.GetProfileInfo(userId);

            var application = new Application
            {
                Id = applicationId,
                OwnerId = userId,
                OwnerEmail = userInfo.Email,
                LastChangeDate = DateTime.UtcNow.ToUniversalTime(),
                ApplicationStatus = ApplicationStatus.Created,
                OwnerName = userInfo.FullName,
            };
          

            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();

            await _db.Applications.AddAsync(application);
            await _db.SaveChangesAsync();
            await GiveEntrantRole(userId);
        }

        public async Task DeleteProgram(DeleteProgramDTO deleteProgramDTO, Guid ownerId, Guid? managerId)
        {

            var applicationId = await _db.Applications
                .Where(ap => ap.OwnerId == ownerId)
                .Select(ap => ap.Id)
                .FirstOrDefaultAsync();


            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == applicationId);

            if (application == null)
            {
                throw new NotFoundException("Такой заявки не существует");
            }

                if (application.OwnerId != ownerId || application.ManagerId != managerId)
                {
                    throw new ForbiddenException("Пользователь не имеет права редактировать это заявление");
                }     

            var applicationProgram = await _db.ApplicationsPrograms.FirstOrDefaultAsync(
                ap => ap.ProgramId == deleteProgramDTO.ProgramId
                && ap.ApplicationId == applicationId);

            if (application.ApplicationStatus == ApplicationStatus.Closed)
            {
                throw new BadRequestException("Пользователь не может удалять программы, если заявление закрыто");
            }
            
            if (applicationProgram == null)
            {
                throw new NotFoundException("В данном заявлении нет такой программы или заявление не существует");
            }

            var applicationPrograms = await _db.ApplicationsPrograms
                .Where(ap => ap.ProgramId == deleteProgramDTO.ProgramId)
                .ToListAsync();

            for (int i = applicationProgram.Priority; i < applicationPrograms.Count(); i++)
            {
                applicationPrograms[i].Priority = applicationPrograms[i].Priority--;
            }

            _db.ApplicationsPrograms.UpdateRange(applicationPrograms);

            _db.ApplicationsPrograms.Remove(applicationProgram);
            await _db.SaveChangesAsync();
        }

        private async Task CheckPasportExist(Guid userId)
        {
                var passportInfo = await _queueSender.GetUserPassport(userId);

                if (passportInfo == null)
                {
                    throw new BadRequestException("Невозможно создать заявление, если не загружен паспорт");
                }
        }

        private async Task<bool> CheckEducationLevel(string programEducationLevelId, Guid userId)
        {

            var educationDocument = await _queueSender.GetUserEducationDocument(userId);

            var educationDocumentLevelId = educationDocument.EducationLevelId;

            Dictionary<string, HashSet<string>> allowedEducationLevelsByDocumentLevel = new Dictionary<string, HashSet<string>>
            {
                { "0", new HashSet<string> { "0", "3", "5", "6", "2" } },
                { "1", new HashSet<string> { "0", "1", "2", "3", "5", "6" } },
                { "2", new HashSet<string> { "0", "1", "2", "3", "5", "6" } },
                { "3", new HashSet<string> { "0", "1", "3", "5", "6" } },
                { "5", new HashSet<string> { "5", "6", "0", "3" } },
                { "6", new HashSet<string> { "5", "6", "0", "3" } }
            };

            if (!allowedEducationLevelsByDocumentLevel.TryGetValue(educationDocumentLevelId, out HashSet<string> allowedEducationLevels)
                || !allowedEducationLevels.Contains(programEducationLevelId))
            {
                return false;
            }
            return true;
        }

        public async Task<GetApplicationPrograms> GetApplicationPrograms(Guid userId)
        {
            var applicationId = await _db.Applications
                .Where(ap => ap.OwnerId == userId)
                .Select(ap => ap.Id)
                .FirstOrDefaultAsync();

            var applicationPrograms = new GetApplicationPrograms();

            if (Guid.Empty != applicationId)
            {
                var programApplications = _db.ApplicationsPrograms
               .Where(ap => ap.ApplicationId == applicationId)
               .AsQueryable();
              
                foreach (var applicationProgram in programApplications)
                {
                    var program = _mapper.Map<GetProgramDTO>(applicationProgram);

                    applicationPrograms.Programs.Add(program);
                }
            }

            return applicationPrograms;
        }

        public async Task<ApplicationsResponseDTO> GetApplications(
            string? entrantName, 
            Guid? programsGuid, 
            List<string>? faculties, 
            ApplicationStatus? status, 
            bool? hasManager,
            bool? onlyMyManaging,
            Guid? managerId, 
            SortingTypes? sortingTypes,
            int page, 
            int pageSize,
            Guid myId
            )
        {
            var applications = await FilterApplications(
                entrantName, 
                programsGuid, 
                faculties, 
                status,
                hasManager,
                onlyMyManaging,
                managerId,
                sortingTypes,
                page,
                pageSize,
                myId
                ).ToListAsync();

            var applcationDTO = CreateApplicationDTO(applications, pageSize, page);

            return applcationDTO;

        }

        private IQueryable<Application> FilterApplications(
            string? entrantName,
            Guid? programGuid,
            List<string>? faculties,
            ApplicationStatus? status,
            bool? hasManager,
            bool? onlyMyManaging,
            Guid? managerId,
            SortingTypes? sortingTypes,
            int page,
            int pageSize,
            Guid myId
            )
        {
            var applications =  FilterByEntrantName(entrantName);
            applications =  FilterByFaculties(applications, faculties);
            applications =  FilterByPrograms(applications, programGuid);
            applications =  FilterByStatus(applications, status);
            applications =  FilterByHavingManager(applications, hasManager);
            applications =  FilterByManagerId(applications, managerId);
            applications = FilterByMyManaging(applications, onlyMyManaging, myId);
            applications =  FilterByPagination(applications, page, pageSize);
            applications =  SortApplications(applications, sortingTypes);

            

            return applications;
        }

        private IQueryable<Application> FilterByEntrantName(string? entrantName)
        {
            var query = _db.Applications.AsQueryable();

            if (!string.IsNullOrEmpty(entrantName))
            {
                query = query.Where(ap => ap.OwnerName.Contains(entrantName));
            }

            return query;
        }

        private IQueryable<Application> FilterByPrograms(
            IQueryable<Application> applications,
            Guid? programGuid
            )
        {

            var programApplicationsGuid = _db.ApplicationsPrograms
                .Where(ap => ap.ProgramId == programGuid)
                .Select(ap => ap.ApplicationId)
                .AsQueryable();

            if (programGuid != null)
            {
                applications = applications.Where(ap => programApplicationsGuid.Contains(ap.Id));
            }

            return applications;
        }

        private IQueryable<Application> FilterByFaculties(
            IQueryable<Application> applications,
            List<string>? faculties
            )
        {

            if (faculties != null && faculties.Any())
            {
                var applicationWithFaculties = _db.ApplicationsPrograms
                    .Where(ap => faculties.Contains(ap.FacultyId))
                    .Select(ap => ap.ApplicationId)
                    .AsQueryable();
                
                applications = applications.Where(ap => applicationWithFaculties.Contains(ap.Id));
            }
            return applications;

        }
        private IQueryable<Application> FilterByStatus(
            IQueryable<Application> applications,
            ApplicationStatus? status
            )
        {
            if (status != null)
            {
                applications = applications.Where(ap => ap.ApplicationStatus == status);
            }
            return applications;
        }

        private IQueryable<Application> FilterByHavingManager(
            IQueryable<Application> applications,
            bool? havingManager
            )
        {
            if (havingManager != null)
            {
                if (havingManager == true)
                {
                    applications = applications.Where(ap => ap.ManagerId != Guid.Empty);
                }
                else
                {
                    applications = applications.Where(ap => ap.ManagerId == Guid.Empty);
                }
            }
            return applications;
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

        private IQueryable<Application> FilterByManagerId(
            IQueryable<Application> applications,
            Guid? managerId
            )
        {
            if (managerId != null)
            {
                applications = applications.Where(ap => ap.ManagerId == managerId);
            }

            return applications;
        }

        private IQueryable<Application> FilterByMyManaging(
            IQueryable<Application> applications,
            bool? onlyMyManaging,
            Guid myId
            )
        {
            if (onlyMyManaging != null)
            {
                if (onlyMyManaging == true)
                {
                    applications = applications.Where(ap => ap.ManagerId == myId);
                }
                else
                {
                    applications = applications.Where(ap => ap.ManagerId != myId);
                }
            }
            return applications;
        }

        private IQueryable<Application> FilterByPagination(
            IQueryable<Application> applications,
            int page, 
            int pageSize
            )
        {
            if (page <= 0)
            {
                page = 1;
            }

            var count = applications.Count();

            var countOfPages = (int)Math.Ceiling((double)applications.Count() / pageSize);

            if (countOfPages == 0)
            {
                return applications;
            }

            if (page <= countOfPages)
            {
                var lowerBound = page == 1 ? 0 : (page - 1) * pageSize;
                if (page < countOfPages)
                {
                    applications = applications.Skip(lowerBound).Take(pageSize);
                }
                else
                {
                    applications = applications.Skip(lowerBound).Take(applications.Count() - lowerBound);
                }
                return applications;
            }
            else
            {
                throw new BadRequestException("Такой страницы нет");
            }
        }

        private ApplicationsResponseDTO CreateApplicationDTO(
            List<Application> applications,
            int pageSize,
            int page
            )
        {
            var programsCount = applications.Count();

            List<GetApplicationDTO> applicationsDTO = new List<GetApplicationDTO>();

            foreach (var application in applications)
            {
                var managerInfo = _db.Managers.FirstOrDefault(m => m.Id == application.ManagerId);

                var applicationDTO = _mapper.Map<GetApplicationDTO>(application);
                
                if (managerInfo != null)
                {
                    applicationDTO.ManagerEmail = managerInfo.Email;
                    applicationDTO.ManagerName = managerInfo.FullName;
                    applicationDTO.ManagerId = managerInfo.Id;
                }
                

                applicationsDTO.Add(applicationDTO);
            }

            var applicationsResponseDTO = new ApplicationsResponseDTO
            {
                Applications = applicationsDTO,
                Pagination = GetPagination(pageSize, page, programsCount)
            };
            return applicationsResponseDTO;
        }

        private IQueryable<Application> SortApplications(
            IQueryable<Application> application,
            SortingTypes? sortingType
            )
        {
            switch ( sortingType )
            {
                case SortingTypes.ChangeTimeDesc:
                    application = application.OrderByDescending(d => d.LastChangeDate);
                    break;
                case SortingTypes.ChangeTimeAsc:
                    application = application.OrderBy(d => d.LastChangeDate);
                    break;
            }

            return application;
        }
        private async Task GiveEntrantRole(Guid userId)
        {

            var message = new UserRoleActionDTO
            {
                UserId = userId,
                Role = Roles.ENTRANT
            };

            await _queueSender.SendMessage(message, QueueConst.SetRoleQueue);      
        }

        public async Task SetManager(TakeApplication addManagerDTO)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == addManagerDTO.ApplicationId);

            var managerInfo = await _queueSender.GetProfileInfo(addManagerDTO.ManagerId);

            if (application == null){
                throw new NotFoundException("Такой заявки не существует");
            }
            if (application.ManagerId == Guid.Empty)
            {
                application.ApplicationStatus = ApplicationStatus.InProcess;
                application.ManagerId = addManagerDTO.ManagerId;
                application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();
                _db.Applications.Update(application);
                await _db.SaveChangesAsync();

                var entrantMessage = new MailStructure
                {
                    Recipient = application.OwnerEmail,
                    Body = $"На ваше заявление был назначен менеджер {managerInfo.FullName}",
                    Subject = "Назначение менеджера"
                };
                await SendNotification(entrantMessage);

                var managerMessage = new MailStructure
                {
                    Recipient = managerInfo.Email,
                    Body = $"Вы были назначены менеджером {application.OwnerEmail}",
                    Subject = "Назначение менеджера"
                };
                await SendNotification(managerMessage);
            }
            else
            {
                throw new BadRequestException("У этой заявки уже есть менеджер");
            }
        }
        private async Task SendNotification(MailStructure message)
        {
            await _queueSender.SendMessage(message, QueueConst.NotificationQueue);
        }

        public async Task RefuseApplication(RefuseApplication refuseApplicationDTO)
        {
            var application = await _db.Applications.FirstOrDefaultAsync
                (ap => ap.Id == refuseApplicationDTO.ApplicationId);

            if (application == null)
            {
                throw new NotFoundException("Такой заявки не существует");
            }

            if (application.ManagerId != refuseApplicationDTO.ManagerId)
            {
                throw new ForbiddenException("Эта заявка не принадлежит этому менеджеру");
            }

            if (application.ManagerId != Guid.Empty)
            {
                application.ApplicationStatus = ApplicationStatus.Created;
                application.ManagerId = Guid.Empty;
                application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();
                _db.Applications.Update(application);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException("У этой заявки нет менеджера");
            }
        }

        public async Task RemoveManager(Guid managerId)
        {
            var applications = _db.Applications.Where(ap => ap.ManagerId == managerId);

            if (applications.Any())
            {
                foreach (var application in applications)
                {
                    application.ApplicationStatus = ApplicationStatus.Created;
                    application.ManagerId = Guid.Empty;                   
                }
                _db.Applications.UpdateRange(applications);
            }
            
            var manager = await _db.Managers.FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager != null) {
                _db.Managers.Remove(manager);
            }
            
            
            await _db.SaveChangesAsync();
        }

        public async Task CreateManager(ManagerDTO managerInfo)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var managerExists = await _db.Managers.AnyAsync(m => m.Id == managerInfo.Id);

                    if (!managerExists)
                    {
                        var newManager = new Manager
                        {
                            FullName = managerInfo.FullName,
                            Email = managerInfo.Email,
                            Id = managerInfo.Id,
                            Role = managerInfo.Role
                        };

                        await _db.Managers.AddAsync(newManager);
                        await _db.SaveChangesAsync();
                    }

                    var managerMessage = new MailStructure
                    {
                        Recipient = managerInfo.Email,
                        Body = $"Вам была выдана роль менеджера",
                        Subject = "Вы теперь менеджер"
                    };
                    await SendNotification(managerMessage);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }


        public ManagersListDTO GetAllManagers()
        {
            var managers = _db.Managers.AsQueryable();
            var managersListDTO = new ManagersListDTO();
            foreach (var manager in managers)
            {
                var managerDTO = _mapper.Map<ManagerDTO>(manager);
                managersListDTO.ManagerDTO.Add(managerDTO);
            }

            return managersListDTO;

        }

        public async Task<GetApplicationManagerId> GetApplicationManagerId(Guid userId)
        {
            var managerId = await _db.Applications
                .Where(ap => ap.OwnerId == userId)
                .Select(ap => ap.ManagerId)
                .FirstOrDefaultAsync();

            var applicationManagerId = new GetApplicationManagerId
            {
                ManagerId = managerId
            };

            return applicationManagerId;
        }
        
        public async Task SyncApplicationsWithPrograms(List<Program> deletedPrograms) 
        {
            foreach(var program in deletedPrograms)
            {
                var applicationPrograms = _db.ApplicationsPrograms.Where(ap => ap.ProgramId == program.Id);
                _db.RemoveRange(applicationPrograms);
            }

            await _db.SaveChangesAsync();

        }
        
    }
    
}
