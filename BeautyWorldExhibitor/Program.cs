using ExhibitorImportBAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyWorldExhibitor
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiProcess apiProcess = new ApiProcess();// search exhibiters
            apiProcess.DoProcess();
        }
    }
}
