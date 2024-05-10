using Hippra.Models.Enums;
using Hippra.Models.SQL;
using Hippra.Pages.Home;
using System.Collections.Generic;
using System.Linq;

namespace Hippra.Models.DTO
{
    public class ParsedCase : Case
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

        public static ParsedCase FromEntity(Case entity)
        {

            ParsedCase result = new ParsedCase
            {
                ID = entity.ID,
                Topic = entity.Topic,
                Description = entity.Description,
                DateCreated = entity.DateCreated,
                Priority = EnumsHelper.GetDisplayName(entity.ResponseNeeded),
                ParsedCategory = EnumsHelper.GetDisplayName(entity.MedicalCategory),
                ParsedSubCategory = entity.MedicalSubCategory != null ? entity.MedicalSubCategory.Name : "",
                PosterID = entity.PosterID,
                PosterName = entity.PosterName,
                PosterSpecialty = entity.PosterSpecialty,
                DateLastUpdated = entity.DateLastUpdated,



            };
            return result;
        }

        public static IList<ParsedCase> FromEntityList(ICollection<Case> items)
        {
            return items.Select(x => FromEntity(x)).ToList();
        }
    }
}
