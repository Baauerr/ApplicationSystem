namespace Common.DTO.Auth
{
    public class RefreshTokenDTO
    {
        public string Token { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
