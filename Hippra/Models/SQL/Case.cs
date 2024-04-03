using Hippra.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
    public class Case
    {
        public int ID { get; set; }
        // poster 
        public int PosterID { get; set; }
        public string PosterName { get; set; }
        public string PosterSpecialty { get; set; }


        // case 
        public bool Status { get; set; } // true: open, false: closed   => should create enum for this
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateClosed { get; set; }

        // info

        public string  Topic { get; set; }
        public string  Description { get; set; }
        public int ResponseNeeded { get; set; } // 0: high, 1: mid, 2: low  => should create enum for this

        public int MedicalCategory { get; set; } // => should create enum for this
        public int PatientAge { get; set; }
        public GenderType Gender { get; set; } // 0 Male, 1, Female, 2 Neutral => should create enum for this
        public RaceType Race { get; set; } // => should create enum for this
        public int Ethnicity { get; set; } // => should create enum for this
        public string LabValues { get; set; }
        public string CurrentStageOfDisease { get; set; }
        public string CurrentTreatmentAdministered { get; set; }
        public string TreatmentOutcomes { get; set; }
        public string imgUrl { get; set; }
        public List<CaseTags> Tags { get; set; }
        public List<CaseComment> Comments { get; set; }

        //public List<Tag> Tags { get; set; } //  many to many relations (let's use Phat's implementation for now)
    }
}
