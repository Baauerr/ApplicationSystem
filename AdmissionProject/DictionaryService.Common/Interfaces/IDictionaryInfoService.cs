using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Common.Interfaces
{
    public interface IDictionaryInfoService
    {
        public Task GetPrograms();
        public Task GetFaculties();
        public Task GetDocumentTypes();
        public Task GetEducationLevel();

    }
}
