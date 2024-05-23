using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Const
{
    public static class QueueConst
    {
        public const string GetProfileInfoQueue = "GetProfileInfoQueue";
        public const string GetProgramsQueue = "GetProgramsQueue";
        public const string GetEducationLevelsQueue = "GetEducationLevelsQueue";
        public const string GetEducationDocumentsFormsQueue = "GetEducationDocumentsFormsQueue";
        public const string GetPassportFormQueue = "GetPassportFormQueue";
        public const string SetRoleQueue = "SetRoleQueue";
        public const string NotificationQueue = "NotificationQueue";
        public const string UpdateUserDataQueue = "UpdateUserDataQueue";
        public const string CreateManagerQueue = "CreateManagerQueue";


        //MVC
        public const string LoginQueue = "LoginQueue";
        public const string ChangePasswordQueue = "ChangePasswordQueue";
        public const string ChangeProfileQueue = "ChangeProfileQueue";
        public const string GetApplicationsQueue = "GetApplicationsQueue";
        public const string ChangeApplicationStatusQueue = "ChangeApplicationStatusQueue";
        public const string SetManagerQueue = "SetManagerQueue";
        public const string RemoveApplicationManagerQueue = "RemoveApplicationManagerQueue";
        public const string GetAllManagersQueue = "GetAllManagersQueue";
        public const string GetAllFacultiesQueue = "GetAllFacultiesQueue";
        public const string GetImportHistoryQueue = "GetImportHistoryQueue";
        public const string MakeImportQueue = "MakeImportQueue";
    }
}
