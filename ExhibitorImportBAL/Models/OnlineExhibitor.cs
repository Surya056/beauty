using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportBAL.Models
{
   public class OnlineExhibitor
    {
        public string Form { get; set; }
        public string Exhibitor { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime Submitted { get; set; }
        public string Status { get; set; }
        public Response Response { get; set; }
    }
}
