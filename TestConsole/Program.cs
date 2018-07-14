using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SA_EF;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new ChemicalAnalysesEntities())
            {
                var dp = context.DataPoints.Add(new DataPoint()
                { IDCalibration = 11, Concentration = 0.13M, Value = 0.13M, Diapason = 1 });
                context.SaveChanges();
            }
        }
    }
}
