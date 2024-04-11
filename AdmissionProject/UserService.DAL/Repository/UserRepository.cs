using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using UserService.DAL.Entity;

namespace UserService.DAL.Repository
{
    public class UserRepository
    {
        private readonly UserDbContext _db;
        public UserRepository(UserDbContext db)
        {
            _db = db;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Email == email);

            if (user == null)
            {
                throw new NotFoundException("Такого пользователя не существует");
            }
            return user;
        }
    }
}
