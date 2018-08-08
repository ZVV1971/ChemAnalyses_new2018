using System.Linq;
using System.Reflection;
using System;
using SA_EF;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SaltCalculationSchemes SchemeToCheck = SaltCalculationSchemes.Chloride;

            foreach(string s in SchemesHelper.GetPropertiesToCheck())
            {
                Console.WriteLine(s + " has to be checked for " + SchemeToCheck + " scheme");
            }

            //SaltAnalysisData sa = new SaltAnalysisData();
            //Type type = sa.GetType();
            //PropertyInfo pi = type.GetProperty("CaSO4");
            //pi.SetValue(sa, 12);
        }
    }
}