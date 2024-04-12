using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum UserAccountType
    {
        [Display(Name = "NursePrac")]
        Nurse = 0,

        [Display(Name = "Physician")]
        Physician = 1,
    }
}
