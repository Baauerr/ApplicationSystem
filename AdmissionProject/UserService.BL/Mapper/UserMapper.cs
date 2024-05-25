using AutoMapper;
using Common.DTO.Profile;
using Common.DTO.User;
using UserService.Common.DTO.Auth;
using UserService.Common.DTO.Profile;
using UserService.DAL.Entity;

namespace UserService.BL.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegistrationRequestDTO, User>();
            CreateMap<ChangeProfileRequestDTO, User>();
            CreateMap<User, ProfileResponseDTO>();
            CreateMap<User, UserDTO>();
        }
    }
}
