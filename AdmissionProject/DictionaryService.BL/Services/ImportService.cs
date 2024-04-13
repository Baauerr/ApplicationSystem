

using DictionaryService.Common.DTO;
using DictionaryService.Common.Interfaces;
using System.IO;
using System.Net;
using System.Net.Http;

namespace DictionaryService.BL.Services
{
    public class ImportService : IImportService
    {
        private readonly HttpClient _httpClient;
        private readonly string username = "";
        private readonly string password = "";
        private readonly string _baseUrl = "https://1c-mockup.kreosoft.space/api/dictionary/";

   //     public ImportService(string username, string password)
   //     {
   //         _httpClient = CreateHttpClient(username, password);
  //      }
  //      private HttpClient CreateHttpClient(string username, string password)
  //      {
  //          HttpClientHandler handler = new HttpClientHandler();
  //          handler.Credentials = new NetworkCredential(username, password);
  //          return new HttpClient(handler);
  //      }

        public Task GetImportStatus()
        {
            throw new NotImplementedException();
        }

        public async Task ImportDocumentTypes()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "document_types"); ;
            if (response.IsSuccessStatusCode)
            {
                DocumentTypeResponseDTO documentTypes = await response.Content.ReadAsAsync<DocumentTypeResponseDTO>();
            }
            else
            {

            }
        }

        public async Task ImportEducationLevels()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "education_levels");
            if (response.IsSuccessStatusCode)
            {
                EducationLevelResponseDTO educationLevels = await response.Content.ReadAsAsync<EducationLevelResponseDTO>();
            }
            else
            {

            }
        }

        public async Task ImportFaculties()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "faculties");
            if (response.IsSuccessStatusCode)
            {
                FacultiesResponseDTO faculties = await response.Content.ReadAsAsync<FacultiesResponseDTO>();

            }
            else
            {

            }
        }

        public async Task ImportPrograms()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + $"programs?page={10}size={10}");
            if (response.IsSuccessStatusCode)
            {
                ProgramResponseDTO faculties = await response.Content.ReadAsAsync<ProgramResponseDTO>();
            }
            else
            {

            }
        }
        public async Task ImportAll()
        {
            
        }
    }
}
