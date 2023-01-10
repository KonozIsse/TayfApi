using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Enums
{
    [EnumBindResource(typeof(ResourcesLib.EnumResources.Status))]
    public enum Status
    {
        NotActive,
        Active
    }
}
