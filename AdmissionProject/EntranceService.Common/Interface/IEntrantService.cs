using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.Interface
{
    public interface IEntrantService
    {
        public Task GetApplicationInfo(Guid applicationId);
        public Task EditApplicationsInfo(Guid applicationId);
    }
}
