using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Common.DTO.Dictionary;
using Common.Helpers;
using Exceptions.ExceptionTypes;
using EntranceService.Common.Interface;

namespace EntranceService.BL.Services
{
    public class RequestService: IRequestService
    {

        private readonly HttpClient _httpClient;
        private readonly ITokenHelper _tokenHelper;
        private readonly string _baseUrl = "https://localhost:7135/api/";

        public RequestService(ITokenHelper tokenHelper) { 
            _tokenHelper = tokenHelper;
            _httpClient = CreateHttpClient();
        }

        private HttpClient CreateHttpClient()
        {

            var token = _tokenHelper.GetTokenFromHeader();

            HttpClientHandler handler = new HttpClientHandler();
            handler.Credentials = new NetworkCredential();

            HttpClient client = new HttpClient(handler);

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<ProgramResponseDTO> GetPrograms()
        {
            var programsPath = "dictionary/programs?page=1&pageSize=1000";

            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + programsPath);

            if (response.IsSuccessStatusCode)
            {
                ProgramResponseDTO programs = await response.Content.ReadAsAsync<ProgramResponseDTO>();
                return programs;
            }
            else
            {
                throw new BadRequestException("Ошибка при получении программ");
            }
        }
        public async Task<EducationLevelResponseDTO> GetEducationLevels()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "dictionary/educationLevel");

            if (response.IsSuccessStatusCode)
            {
                EducationLevelResponseDTO educationLevels = await response.Content.ReadAsAsync<EducationLevelResponseDTO>();
                return educationLevels;
            }
            else
            {
                throw new BadRequestException("Ошибка при получении программ");
            }
        }

    }
}
