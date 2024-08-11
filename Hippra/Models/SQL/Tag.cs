using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hippra.Models.SQL
{
    public class Tag
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<Case> Cases { get; set; }

    }
}
