using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportBAL.Models
{
   public class Secondaryuser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public string Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
    }
}
