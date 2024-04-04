using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
	public enum MedicalCategory
	{
		[Display(Name = "Medicine")]
		MED = 1,
		[Display(Name = "Surgery")]
		SUR = 2,
		[Display(Name = "Diagnostics")]
		DIA = 3,
	}
}
