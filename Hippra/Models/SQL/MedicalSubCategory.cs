using Hippra.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
	public class MedicalSubCategory
	{
		public int ID { get; set; }

		public string Name { get; set; }
		public MedicalCategory MedicalCategory { get; set; }
	}
}
