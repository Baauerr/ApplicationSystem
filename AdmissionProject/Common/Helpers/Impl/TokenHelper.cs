using Microsoft.AspNetCore.Http;

namespace Common.Helpers.Impl
{
    public class TokenHelper: ITokenHelper
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTokenFromHeader()
        {

            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("HttpContext is not available");
            }

            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            return token;
        }
    }
}
