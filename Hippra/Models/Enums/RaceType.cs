using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

//https://www.hcup-us.ahrq.gov/datainnovations/raceethnicitytoolkit/nm2.jsp
namespace Hippra.Models.Enums
{
    public enum RaceType
    {
        [Display(Name = "American Indian or Alaska Native")]
        AMERICANINDIANORALASKANATIVE = 0,
        [Display(Name = "Asian")]
        ASIAN = 1,
        [Display(Name = "Black or African American")]
        BLACKORAFRICANAMERICAN = 2,
        [Display(Name = "Native Hawaiian or Other Pacific Islander")]
        NATIVEHAWAIIANOROTHERPACIFICISLANDER = 3,
        [Display(Name = "White")]
        WHITE = 4,
        [Display(Name = "Declined")]
        DECLINED = 5,
        [Display(Name = "Unknown")]
        UNKNOWN = 6,
        [Display(Name = "Other Race")]
        OTHERRACE = 7
    }
}
