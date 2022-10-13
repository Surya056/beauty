using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportBAL.Models
{
  public class ExhibitorsListResponse
    {
        public List<Exhibitor> Exhibitors { get; set; }
        public int TotalCount { get; set; }
        public int Count { get; set; }
    }
}
