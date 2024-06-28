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
        //public static CaseViewModel ToParsedCase(this Case tCase)
        //{
        //    var pCase = new CaseViewModel();
        //    pCase.ID = tCase.ID;

        //    // TODO: fill the rest
        //    pCase.DateCreated = tCase.DateCreated;
        //    pCase.DateLastUpdated = tCase.DateLastUpdated;
        //    pCase.Description = tCase.Description;
        //    pCase.Topic = tCase.Topic;
        //    pCase.PosterID = tCase.PosterID;
        //    pCase.PosterName = tCase.PosterName;
        //    pCase.Race = tCase.Race;
        //    pCase.Gender = tCase.Gender;
        //    pCase.Ethnicity = tCase.Ethnicity;
        //    pCase.ResponseNeeded = tCase.ResponseNeeded;
        //    pCase.MedicalCategory = tCase.MedicalCategory;
        //    pCase.PosterSpecialty = tCase.PosterSpecialty;
        //    pCase.MedicalSubCategory = tCase.MedicalSubCategory;
        //    if (tCase.Tags != null)
        //    {
        //        pCase.Tags = tCase.Tags;
        //    }
        //    if (tCase.Comments != null)
        //    {
        //        pCase.Comments = tCase.Comments;
        //    }
        //    if (tCase.User != null)
        //    {
        //        pCase.User = tCase.User;
        //    }
        //    pCase.imgUrl = tCase.imgUrl;
        //    pCase.PatientAge = tCase.PatientAge;
        //    pCase.CurrentStageOfDisease = tCase.CurrentStageOfDisease;
        //    pCase.CurrentTreatmentAdministered = tCase.CurrentTreatmentAdministered;
        //    pCase.TreatmentOutcomes = tCase.TreatmentOutcomes;
        //    pCase.LabValues = tCase.LabValues;
        //    pCase.ParsedCategory = EnumsHelper.GetDisplayName(tCase.MedicalCategory);
        //    pCase.ParsedSubCategory = tCase.MedicalSubCategory!.Name;
        //    pCase.ParsedGender = EnumsHelper.GetDisplayName(tCase.Gender);
        //    pCase.ParsedEthnicity = EnumsHelper.GetDisplayName(tCase.Ethnicity);
        //    pCase.Priority = EnumsHelper.GetDisplayName(tCase.ResponseNeeded);
        //    pCase.ParsedRace = EnumsHelper.GetDisplayName(tCase.Race);
        //    pCase.Status = tCase.Status;
        //    pCase.ParsedStatus = ParseStatus(tCase.Status);

        //    return pCase;
        //}


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
            viewModel.Race = @case.Race;
            viewModel.Gender = @case.Gender;
            viewModel.Ethnicity = @case.Ethnicity;
            viewModel.ResponseNeeded = @case.ResponseNeeded;
            viewModel.MedicalCategory = @case.MedicalCategory;
            //viewModel.PosterSpecialty = @case.PosterSpecialty;
            viewModel.MedicalSubCategory = @case.MedicalSubCategory;
            viewModel.MedicalSubCategoryId = @case.MedicalSubCategoryId;
            if (@case.Tags != null)
            {
                viewModel.SelectedTags = @case.Tags.Select(x => x.Tag.ID).ToArray();
            }
            //if (@case.Comments != null)
            //{
            //    viewModel.Comments = @case.Comments;
            //}
            viewModel.imgUrl = @case.imgUrl;
            viewModel.PatientAge = @case.PatientAge;
            viewModel.CurrentStageOfDisease = @case.CurrentStageOfDisease;
            viewModel.CurrentTreatmentAdministered = @case.CurrentTreatmentAdministered;
            viewModel.TreatmentOutcomes = @case.TreatmentOutcomes;
            viewModel.LabValues = @case.LabValues;
            viewModel.Status = @case.Status;
            return viewModel;
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

    }
}
