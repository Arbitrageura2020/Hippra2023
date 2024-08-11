using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
    public class CaseTag
    {
        public long ID { get; set; }
        public int CasesID { get; set; }
        public Case Case { get; set; }
        public int TagsId { get; set; }
        public Tag Tag { get; set; }
    }
}
