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
            Files = new List<CaseFileViewModel>();
        }

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
        public string PosterId { get; set; }
        public AppUser User { get; set; }
        public string PosterName { get; set; }
        public string PosterSpeciality { get; set; }
        public string PosterImg { get; set; } = "/img/hippra/blank-profile.png";
        public bool IsAnonymos { get; set; }
        public bool ImOwner { get; set; }

        // case 
        public bool Status { get; set; } // true: open, false: closed   => should create enum for this
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateClosed { get; set; }
        public CaseType Type { get; set; }
        public int NrOfLikes { get; set; }
        public int NrOfComments { get; set; }
        public bool LikedByCurrentUser { get; set; }

        public string TimeDifferenceText
        {
            get
            {
                var daysDifference = (DateTime.UtcNow - this.DateLastUpdated).Days;
                if (daysDifference < 7)
                    return daysDifference + " Days Ago";
                else
                    return daysDifference / 7 + " Weeks Ago";
            }
        }

        // info

        public string Topic { get; set; }
        public string Description { get; set; }

        public int PatientAge { get; set; }
        public GenderType Gender { get; set; } // 0 Male, 1, Female, 2 Neutral 
        public RaceType Race { get; set; }
        public EthnicityType Ethnicity { get; set; }
        public string LabValues { get; set; }
        public string CurrentStageOfDisease { get; set; }
        public string CurrentTreatmentAdministered { get; set; }
        public string TreatmentOutcomes { get; set; }

        public List<CaseTagViewModel> Tags { get; set; }
        public IList<CaseFileViewModel> Files { get; set; }
        public static CaseViewModel FromEntity(Case tCase)
        {

            var pCase = new CaseViewModel();
            pCase.ID = tCase.ID;

            // TODO: fill the rest
            pCase.DateCreated = tCase.DateCreated;
            pCase.DateLastUpdated = tCase.DateLastUpdated;
            pCase.Description = tCase.Description;
            pCase.Topic = tCase.Topic;
            pCase.Race = tCase.Race;
            pCase.Gender = tCase.Gender;
            pCase.Ethnicity = tCase.Ethnicity;
            if (tCase.Tags != null)
            {
                pCase.Tags = CaseTagViewModel.FromEntityList(tCase.Tags).ToList();
            }
            if (tCase.Files != null)
            {
                pCase.Files = CaseFileViewModel.FromEntityList(tCase.Files).ToList();
            }
            pCase.PosterId = tCase.UserId;

            if (tCase.User != null)
            {
                pCase.User = tCase.User;
                pCase.PosterName = tCase.User.FullName;
                pCase.PosterSpeciality = EnumsHelper.GetDisplayName(tCase.User.MedicalSpecialty);
            }

            pCase.PatientAge = tCase.PatientAge;
            pCase.CurrentStageOfDisease = tCase.CurrentStageOfDisease;
            pCase.CurrentTreatmentAdministered = tCase.CurrentTreatmentAdministered;
            pCase.TreatmentOutcomes = tCase.TreatmentOutcomes;
            pCase.LabValues = tCase.LabValues;
            pCase.ParsedGender = EnumsHelper.GetDisplayName(tCase.Gender);
            pCase.ParsedEthnicity = EnumsHelper.GetDisplayName(tCase.Ethnicity);
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

        public static CaseTagViewModel FromEntity(Tag tag)
        {

            return new CaseTagViewModel()
            {
                TagId = tag.ID,
                Name = tag.Name
            };
        }

        public static IList<CaseTagViewModel> FromEntityList(ICollection<Tag> items)
        {
            return items.Select(x => FromEntity(x)).ToList();
        }
    }

    public class CaseFileViewModel
    {
        public long ID { get; set; }
        public string UploadedByUserId { get; set; }
        public string Container { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }

        public static CaseFileViewModel FromEntity(CaseFiles file)
        {

            return new CaseFileViewModel()
            {
                ID = file.ID,
                Container = file.Container,
                FileName = file.FileName,
                FileType = file.FileType,
                UploadDate = file.UploadDate,
                FileLink = file.FileLink,
                UploadedByUserId = file.UploadedByUserId
            };
        }

        public static IList<CaseFileViewModel> FromEntityList(ICollection<CaseFiles> items)
        {
            return items.Select(x => FromEntity(x)).ToList();
        }
    }
}
