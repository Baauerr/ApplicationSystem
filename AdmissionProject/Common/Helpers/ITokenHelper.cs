
using Common.Enum;

namespace Common.Helpers
{
    public interface ITokenHelper
    {
        public string GetTokenFromHeader();
        public Guid GetUserIdFromToken(string token);
        public IEnumerable<string> GetUserRolesFromToken(string token);
    }
}
