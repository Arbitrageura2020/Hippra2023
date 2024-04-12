using Hippra.Models.Enums;
using Hippra.Models.SQL;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hippra.Models.ViewModel
{
    public class CaseViewModel
    {
        public CaseViewModel() { 
        this.Tags = new List<string>();
        }
        public int ID { get; set; }

        public int PosterID { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string PosterName { get; set; }
        public string PosterSpecialty { get; set; }


        // case 
        public bool Status { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateClosed { get; set; }

        // info

        [Required]
        public string Topic { get; set; }
        [Required]
        public string Description { get; set; }
        public CaseResponseLevelType ResponseNeeded { get; set; } // 0: high, 1: mid, 2: low 
        public MedicalCategory MedicalCategory { get; set; } 
        public int MedicalSubCategoryId { get; set; } = 1;
        public MedicalSubCategory MedicalSubCategory { get; set; }
        [Required]
        public int PatientAge { get; set; }
        public GenderType Gender { get; set; } // 0 Male, 1, Female, 2 Neutral 
        public RaceType Race { get; set; }
        public EthnicityType Ethnicity { get; set; }
        public string LabValues { get; set; }
        public string CurrentStageOfDisease { get; set; }
        public string CurrentTreatmentAdministered { get; set; }
        public string TreatmentOutcomes { get; set; }
        public string imgUrl { get; set; }
        public List<string> Tags { get; set; }
        public List<CaseComment> Comments { get; set; }

    }
}
