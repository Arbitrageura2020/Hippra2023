using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.Enums
{
    public enum EthnicityType
    {
		[Display(Name = "Hispanic or Latino")]
		HISPANICORLATINO =0,
		[Display(Name = "Not Hispanic or Latino")]
		NOTHISPANICORLATINO=1,
		[Display(Name = "Declined")]
		DECLINED=2,
		[Display(Name = "Unknown")]
		UNKNOWN=3
    }
}
