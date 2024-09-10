using Hippra.Models.Enums;
using Hippra.Models.SQL;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace Hippra.Models.ViewModel
{
    public class AddEditCaseViewModel
    {
        public AddEditCaseViewModel()
        {
            this.SelectedTagsObjects = new List<Tag>();
            this.Files = new List<CaseFileViewModel>();
        }

        public int ID { get; set; }

        public int PosterID { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string PosterName { get; set; }
        public string PosterSpecialty { get; set; }
        public bool PostAnonymosly { get; set; }

        // case 
        public bool Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateClosed { get; set; }

        public CaseType Type { get; set; }
        public int Votes { get; set; }

        // info

        [Required]
        public string Topic { get; set; }
        [Required]
        public string Description { get; set; }



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

        [MinLength(1, ErrorMessage = "Please select at least one tag")]
        public IEnumerable<Tag> SelectedTagsObjects { get; set; }
        public List<CaseFileViewModel> Files { get; set; }

    }
}
