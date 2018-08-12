using System.Linq;
using System.Reflection;
using System;
using SA_EF;
using SA_EF.Interfaces;
using ChemicalAnalyses.Alumni;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //foreach (PropertyInfo pi in typeof(ISaltAnalysisCalcResults).GetRuntimeProperties())
            //{
            //    CustomDescriptionAttribute customSA = pi.GetCustomAttributes(true).ToList().OfType<CustomDescriptionAttribute>().FirstOrDefault();
            //    string r = customSA?.Description;
            //    if (r?.Length > 0)
            //        Console.WriteLine("{0}  --  {1}", pi.Name, r);
            //    else;
            //}
            CustomDescriptionAttribute ca = (CustomDescriptionAttribute)typeof(ISaltAnalysisCalcResults).GetRuntimeProperty("CaSO4").GetCustomAttribute(typeof(CustomDescriptionAttribute));
            
            Console.WriteLine(ca.Description);
            //?.GetCustomAttribute(true);
        }
    }
}