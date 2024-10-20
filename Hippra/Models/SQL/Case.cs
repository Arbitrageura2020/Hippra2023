﻿using Hippra.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
    public class Case
    {
        public long ID { get; set; }
        // poster 
        public int PosterID { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public bool PostedAnonymosley { get; set; }
        // case 
        public bool Status { get; set; } // true: open, false: closed   => should create enum for this
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateClosed { get; set; }
        public CaseType Type { get; set; }
        public int Votes { get; set; }

        // info

        public string Topic { get; set; }
        public string Description { get; set; }

        public MedicalCategory MedicalCategoryOld { get; set; } = MedicalCategory.MED;
        public int MedicalSubCategoryId { get; set; } = 1;
        public MedicalSubCategory MedicalSubCategory { get; set; }

        public MedicalCategory MedicalCategory { get; set; }

        public int PatientAge { get; set; }
        public GenderType Gender { get; set; } // 0 Male, 1, Female, 2 Neutral 
        public RaceType Race { get; set; }
        public EthnicityType Ethnicity { get; set; }
        public string LabValues { get; set; }
        public string CurrentStageOfDisease { get; set; }
        public string CurrentTreatmentAdministered { get; set; }
        public string TreatmentOutcomes { get; set; }
        public string imgUrl { get; set; }
        public IList<CaseFile> Files { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public List<CaseLike> Likes { get; set; }
        public List<CaseComment> Comments { get; set; }

    }
}
