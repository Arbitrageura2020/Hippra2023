using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
    public class PostTags
    {
        public long ID { get; set; }
        public int CaseID { get; set; }
        public Case Case { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
