using Hippra.Models.SQL;

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

    }
}
