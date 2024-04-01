using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum GenderType
    {
        [Display(Name = "Male")]
        MALE = 0,
        [Display(Name = "Female")]
        FEMALE = 1,
        [Display(Name = "Neutral")]
        NEUTRAL = 2
    }
}
