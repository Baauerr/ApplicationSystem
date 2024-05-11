using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntranceService.DAL.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortingTypes
    {
        ChangeTimeAsc,
        ChangeTimeDesc
    }
}
