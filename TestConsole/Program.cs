using System.Linq;
using System.Reflection;
using System;
using SA_EF;
using ChemicalAnalyses.Alumni;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var p in Enum.GetValues(typeof(SaltCalculationSchemes))
                .OfType<SaltCalculationSchemes>().Where(p=> p.GetAttribute<SchemeRealizedAttribute>() != null))
            {
                
                Console.WriteLine(p.ToString());
            }
        }
    }
}