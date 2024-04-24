using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum CaseResponseLevelType
    {
        [Display(Name = "Stat")]
        STAT = 1,
        [Display(Name = "Over Coffee")]
        OC = 2,
        [Display(Name = "Low")]
        LOW = 3
    }
}
