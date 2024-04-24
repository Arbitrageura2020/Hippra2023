using Hippra.Components;
using Hippra.Models.DTO;
using Hippra.Models.Enums;
using Hippra.Models.SQL;
using Hippra.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Extensions
{
    public static class ConversionExtensions
    {
        public static ParsedCase ToParsedCase(this Case tCase)
        {
            var pCase = new ParsedCase();
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
            pCase.imgUrl = tCase.imgUrl;
            pCase.PatientAge = tCase.PatientAge;
            pCase.CurrentStageOfDisease = tCase.CurrentStageOfDisease;
            pCase.CurrentTreatmentAdministered = tCase.CurrentTreatmentAdministered;
            pCase.TreatmentOutcomes = tCase.TreatmentOutcomes;
            pCase.LabValues = tCase.LabValues;
            pCase.ParsedCategory = Enums.GetDisplayName(tCase.MedicalCategory);
            pCase.ParsedSubCategory = tCase.MedicalSubCategory!.Name;
            pCase.ParsedGender = Enums.GetDisplayName(tCase.Gender);
            pCase.ParsedEthnicity = Enums.GetDisplayName(tCase.Ethnicity);
            pCase.Priority = Enums.GetDisplayName(tCase.ResponseNeeded);
            pCase.ParsedRace = Enums.GetDisplayName(tCase.Race);
            pCase.Status = tCase.Status;
            pCase.ParsedStatus = ParseStatus(tCase.Status);

            return pCase;
        }


        public static AddEditCaseViewModel ToViewModelCase(this Case @case)
        {
            var viewModel = new AddEditCaseViewModel();
            viewModel.ID = @case.ID;

            // TODO: fill the rest
            viewModel.DateCreated = @case.DateCreated;
            viewModel.DateLastUpdated = @case.DateLastUpdated;
            viewModel.Description = @case.Description;
            viewModel.Topic = @case.Topic;
            viewModel.PosterID = @case.PosterID;
            viewModel.PosterName = @case.PosterName;
            viewModel.Race = @case.Race;
            viewModel.Gender = @case.Gender;
            viewModel.Ethnicity = @case.Ethnicity;
            viewModel.ResponseNeeded = @case.ResponseNeeded;
            viewModel.MedicalCategory = @case.MedicalCategory;
            viewModel.PosterSpecialty = @case.PosterSpecialty;
            viewModel.MedicalSubCategory = @case.MedicalSubCategory;
            viewModel.MedicalSubCategoryId = @case.MedicalSubCategoryId;
            if (@case.Tags != null)
            {
                viewModel.Tags = @case.Tags.Select(x => x.Tag).ToList();
            }
            if (@case.Comments != null)
            {
                viewModel.Comments = @case.Comments;
            }
            viewModel.imgUrl = @case.imgUrl;
            viewModel.PatientAge = @case.PatientAge;
            viewModel.CurrentStageOfDisease = @case.CurrentStageOfDisease;
            viewModel.CurrentTreatmentAdministered = @case.CurrentTreatmentAdministered;
            viewModel.TreatmentOutcomes = @case.TreatmentOutcomes;
            viewModel.LabValues = @case.LabValues;
            viewModel.Status = @case.Status;
            return viewModel;
        }

        public static string ParsePriority(CaseResponseLevelType priority)
        {
            string rValue = "";
            switch (priority)
            {
                case CaseResponseLevelType.OC:
                    rValue = "Over Coffee";
                    break;
                /*case 2:
                    rValue = "Low";
                    break;*/
                default:
                    rValue = "Stat";
                    break;
            }


            return rValue;
        }
        public static int PriorityToInt(CaseResponseLevelType priority)
        {
            int rValue = 0;
            switch (priority)
            {
                case CaseResponseLevelType.STAT:
                    rValue = 1;
                    break;
                case CaseResponseLevelType.OC:
                    rValue = 2;
                    break;
                default:
                    rValue = 0;
                    break;
            }
            return rValue;
        }
        public static string ParseStatus(bool tStatus)
        {
            if (tStatus)
            {
                return "Open";
            }
            else
            {
                return "Closed";
            }
        }

        public static string ParseRace(int race)
        {
            string rValue = "";
            switch (race)
            {
                case 1:
                    rValue = "Asian";
                    break;
                case 2:
                    rValue = "Black or African American";
                    break;
                case 3:
                    rValue = "Native Hawaiian or Other Pacific Islander";
                    break;
                case 4:
                    rValue = "White";
                    break;
                case 5:
                    rValue = "Declined";
                    break;
                case 6:
                    rValue = "Unknown";
                    break;
                case 7:
                    rValue = "Other Race";
                    break;
                default:
                    rValue = "American Indian or Alaska Native";
                    break;
            }


            return rValue;
        }
        public static string ParseEthnicity(int ethnicity)
        {
            string rValue = "";
            switch (ethnicity)
            {
                case 1:
                    rValue = "Not Hispanic or Latino";
                    break;
                case 2:
                    rValue = "Declined";
                    break;
                case 3:
                    rValue = "Unknown";
                    break;
                default:
                    rValue = "Hispanic or Latino";
                    break;
            }


            return rValue;
        }
        public static string ParseSubCategory(int subCategory)
        {

            string rValue = "";
            switch (subCategory)
            {
                case 0:
                    rValue = "Diseases & Disorders of the Nervous System";
                    break;
                case 1:
                    rValue = "Diseases & Disorders of the Eye";
                    break;
                case 2:
                    rValue = "Diseases & Disorders of the Ear, Nose, Mouth & Throat";
                    break;
                case 3:
                    rValue = "Diseases & Disorders of the Respiratory System";
                    break;
                case 4:
                    rValue = "Diseases & Disorders of the Circulatory System";
                    break;
                case 5:
                    rValue = "Diseases & Disorders of the Digestive System";
                    break;
                case 6:
                    rValue = "Diseases & Disorders of the Hepatobiliary System & Pancreas";
                    break;
                case 7:
                    rValue = "Diseases & Disorders of the Musculoskeletal System & Connective Tissue";
                    break;
                case 8:
                    rValue = "Diseases & Disorders of the Skin, Subcutaneous Tissue & Breast";
                    break;
                case 9:
                    rValue = "Endocrine, Nutritional & Metabolic Diseases & Disorders";
                    break;

                case 10:
                    rValue = "Diseases & Disorders of the Kidney & Urinary Tract";
                    break;
                case 11:
                    rValue = "Diseases & Disorders of the Male Reproductive System";
                    break;
                case 12:
                    rValue = "Diseases & Disorders of the Female Reproductive System";
                    break;
                case 13:
                    rValue = "Pregnancy, Childbirth & the Puerperium";
                    break;
                case 14:
                    rValue = "Newborns & Other Neonates with Conditions Originating in Perinatal Period";
                    break;
                case 15:
                    rValue = "Diseases & Disorders of the Blood, Blood Forming Organs, Immunologic Disorders";
                    break;
                case 16:
                    rValue = "Myeloproliferative Diseases & Disorders, Poorly Differentiated Neoplasms";
                    break;
                case 17:
                    rValue = "Infectious & Parasitic Diseases, Systemic or Unspecified Sites";
                    break;
                case 18:
                    rValue = "Mental Diseases & Disorders";
                    break;
                case 19:
                    rValue = "Alcohol/Drug Use & Alcohol/Drug Induced Organic Mental Disorders";
                    break;

                case 20:
                    rValue = "Injuries, Poisonings & Toxic Effects of Drugs";
                    break;
                case 21:
                    rValue = "Burns";
                    break;
                case 22:
                    rValue = "Factors Influencing Health Status & Other Contacts with Health Services";
                    break;
                case 23:
                    rValue = "Multiple Significant Trauma";
                    break;
                case 24:
                    rValue = "Human Immunodeficiency Virus Infections";
                    break;
                case 25:
                    rValue = "Others";
                    break;
                case 26:
                    rValue = "Allegy and Immunology";
                    break;
                case 27:
                    rValue = "Anesthesiology";
                    break;
                case 28:
                    rValue = "Colon and Rectal Surgery";
                    break;
                case 29:
                    rValue = "Dermatology";
                    break;

                case 30:
                    rValue = "Emergency Medicine";
                    break;
                case 31:
                    rValue = "Family Medicine";
                    break;
                case 32:
                    rValue = "Internal Medicine";
                    break;
                case 33:
                    rValue = "Medical Genetics";
                    break;
                case 34:
                    rValue = "Neurology";
                    break;
                case 35:
                    rValue = "Neurosurgery";
                    break;
                case 36:
                    rValue = "Nuclear Medicine";
                    break;
                case 37:
                    rValue = "Obstetrics and Gynecology";
                    break;
                case 38:
                    rValue = "Ophthalmology";
                    break;
                case 39:
                    rValue = "OrthopedicSurgery";
                    break;

                case 40:
                    rValue = "Otolaryngology";
                    break;
                case 41:
                    rValue = "Anatomic Pathology and Clinical Pathology";
                    break;
                case 42:
                    rValue = "Pediatrics";
                    break;
                case 43:
                    rValue = "Physical Medicine and Rehibilitation";
                    break;
                case 44:
                    rValue = "Plastic Surgery";
                    break;
                case 45:
                    rValue = "Public Health and General Preventive";
                    break;
                case 46:
                    rValue = "Psychiatry";
                    break;
                case 47:
                    rValue = "Radiology";
                    break;
                case 48:
                    rValue = "Hospice and Palliative Medicine";
                    break;
                case 49:
                    rValue = "Medical Nuclear Physics";
                    break;

                case 50:
                    rValue = "Surgery";
                    break;
                case 51:
                    rValue = "Vascular Surgery";
                    break;
                case 52:
                    rValue = "Thoracic Surgery";
                    break;
                case 53:
                    rValue = "Urology";
                    break;
                case 54:
                    rValue = "Others";
                    break;
                case 55:
                    rValue = "General Surgery";
                    break;
                case 56:
                    rValue = "Thoracic Surgery";
                    break;
                case 57:
                    rValue = "Colon and Rectal Surgery";
                    break;
                case 58:
                    rValue = "Obstetrics and Gynecology";
                    break;
                case 59:
                    rValue = "Gynecologic Oncology";
                    break;

                case 60:
                    rValue = "Neurological Surgery";
                    break;
                case 61:
                    rValue = "Ophthalmic Surgery";
                    break;
                case 62:
                    rValue = "Oral and Maxillofacial Surgery";
                    break;
                case 63:
                    rValue = "Orthopaedic Surgery";
                    break;
                case 64:
                    rValue = "Otolaryngology";
                    break;
                case 65:
                    rValue = "Pediatric Surgery";
                    break;
                case 66:
                    rValue = "Plastic and Maxillofacial Surgery";
                    break;
                case 67:
                    rValue = "Urology";
                    break;
                case 68:
                    rValue = "Vascular Surgery";
                    break;
                case 69:
                    rValue = "Others";
                    break;

                default:
                    // shouldn't get here
                    rValue = "";
                    break;
            }


            return rValue;
        }

        public static string ParseCategoryFromSub(int subCategory)
        {
            string rValue = "";


            if (subCategory < 26)
            {
                rValue = "Diagnostics";
            }
            else if (subCategory < 55)
            {
                rValue = "Medicine";
            }
            else
            {
                rValue = "Surgery";
            }
            return rValue;
        }

    }
}
