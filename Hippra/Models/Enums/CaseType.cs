using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum CaseType
    {
		[Display(Name = "Ask for help")]
		AskForHelp =0,
		[Display(Name = "Clnical Knowledge")]
		ClinicalKnowledge=1,
    }
}
