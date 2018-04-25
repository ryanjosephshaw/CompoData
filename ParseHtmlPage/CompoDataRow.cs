using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseHtmlPage
{
    public class CompoDataRow
    {
        public int Id { get; set; }
        public string Case { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string SubContractor { get; set; }
        public string Team { get; set; }
        public string AssignedAt { get; set; }
        public string DaysSinceAssigned { get; set; }
        public string CompletedAt { get; set; }
        public string Status { get; set; }
    }
}
