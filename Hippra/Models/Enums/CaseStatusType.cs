﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum CaseStatusType
    {
        [Display(Name = "Hispanic or Latino")]
        OPEN,
        [Display(Name = "Hispanic or Latino")] 
        CLOSED
    }
}
