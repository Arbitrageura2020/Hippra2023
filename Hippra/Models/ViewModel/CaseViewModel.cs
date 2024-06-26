using Hippra.Extensions;
using Hippra.Models.Enums;
using Hippra.Models.SQL;
using Hippra.Pages.Home;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hippra.Models.ViewModel
{
    public class CaseViewModel
    {

        public CaseViewModel()
        {
            Tags = new List<CaseTagViewModel>();
            Comments = new List<CaseCommentViewModel>();
        }
        public string Priority { get; set; } = "";
        public string ParsedCategory { get; set; } = "";
        public string ParsedSubCategory { get; set; } = "";
        public string ParsedGender { get; set; } = "";
        public string ParsedRace { get; set; } = "";
        public string ParsedEthnicity { get; set; } = "";
        public string ParsedStatus { get; set; } = "";
        public string CaseCssClass
        {
            get
            {
                if (Type == CaseType.AskForHelp) return "orange";
                else return "";
            }
        }

        public int ID { get; set; }
        // poster 
        public int PosterID { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string PosterName { get; set; }
        public string PosterSpecialty { get; set; }
        public string PosterImg { get; set; }

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
        public CaseResponseLevelType ResponseNeeded { get; set; } // 0: high, 1: mid, 2: low 

        public MedicalCategory MedicalCategoryOld { get; set; } // => should create enum for this
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

        public List<CaseTagViewModel> Tags { get; set; }
        public List<CaseCommentViewModel> Comments { get; set; }
        public static CaseViewModel FromEntity(Case tCase)
        {

            var pCase = new CaseViewModel();
            pCase.ID = tCase.ID;

            // TODO: fill the rest
            pCase.DateCreated = tCase.DateCreated;
            pCase.DateLastUpdated = tCase.DateLastUpdated;
            pCase.Description = tCase.Description;
            pCase.Topic = tCase.Topic;
            pCase.PosterID = tCase.PosterID;
            pCase.PosterName = tCase.PosterName;
            pCase.Race = tCase.Race;
            pCase.Gender = tCase.Gender;
            pCase.Ethnicity = tCase.Ethnicity;
            pCase.ResponseNeeded = tCase.ResponseNeeded;
            pCase.MedicalCategory = tCase.MedicalCategory;
            pCase.PosterSpecialty = tCase.PosterSpecialty;
            pCase.MedicalSubCategory = tCase.MedicalSubCategory;
            if (tCase.Tags != null)
            {
                pCase.Tags = CaseTagViewModel.FromEntityList(tCase.Tags).ToList();
            }
            //if (tCase.Comments != null)
            //{
            //    pCase.Comments = tCase.Comments;
            //}
            if (tCase.User != null)
            {
                pCase.User = tCase.User;
            }

            pCase.PatientAge = tCase.PatientAge;
            pCase.CurrentStageOfDisease = tCase.CurrentStageOfDisease;
            pCase.CurrentTreatmentAdministered = tCase.CurrentTreatmentAdministered;
            pCase.TreatmentOutcomes = tCase.TreatmentOutcomes;
            pCase.LabValues = tCase.LabValues;
            pCase.ParsedCategory = EnumsHelper.GetDisplayName(tCase.MedicalCategory);
            pCase.ParsedSubCategory = tCase.MedicalSubCategory!.Name;
            pCase.ParsedGender = EnumsHelper.GetDisplayName(tCase.Gender);
            pCase.ParsedEthnicity = EnumsHelper.GetDisplayName(tCase.Ethnicity);
            pCase.Priority = EnumsHelper.GetDisplayName(tCase.ResponseNeeded);
            pCase.ParsedRace = EnumsHelper.GetDisplayName(tCase.Race);
            pCase.Status = tCase.Status;
            pCase.ParsedStatus = ConversionExtensions.ParseStatus(tCase.Status);

            return pCase;
        }

        public static IList<CaseViewModel> FromEntityList(ICollection<Case> items)
        {
            return items.Select(x => FromEntity(x)).ToList();
        }
    }

    public class CaseTagViewModel
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        public static CaseTagViewModel FromEntity(PostTags tag)
        {

            return new CaseTagViewModel()
            {
                TagId = tag.TagId,
                Name = tag.Tag.Name
            };
        }

        public static IList<CaseTagViewModel> FromEntityList(ICollection<PostTags> items)
        {
            return items.Select(x => FromEntity(x)).ToList();
        }
    }
}
