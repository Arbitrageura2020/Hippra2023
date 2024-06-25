using Hippra.Extensions;
using Hippra.Models.Enums;
using Hippra.Models.SQL;
using Hippra.Pages.Home;
using System.Collections.Generic;
using System.Linq;

namespace Hippra.Models.DTO
{
    public class CaseViewModel : Case
    {
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
                if (this.Type == CaseType.AskForHelp) return "orange";
                else return "";
            }
        }

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
                pCase.Tags = tCase.Tags;
            }
            if (tCase.Comments != null)
            {
                pCase.Comments = tCase.Comments;
            }
            if (tCase.User != null)
            {
                pCase.User = tCase.User;
            }
            pCase.imgUrl = tCase.imgUrl;
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
}
