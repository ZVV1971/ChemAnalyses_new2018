using System.Collections.Generic;
using System.Linq;
using SA_EF.Interfaces;
using System.Reflection;

namespace SA_EF
{
    public static class SchemesHelper
    {
        public static IEnumerable<string> GetPropertiesToCheck(SaltCalculationSchemes scheme = SaltCalculationSchemes.Chloride)
        {
            foreach (PropertyInfo pi in typeof(ISaltAnalysisCalcResults).GetRuntimeProperties())
            {
                SchemesToCheckAttibute customSA = pi.GetCustomAttributes(true).ToList().OfType<SchemesToCheckAttibute>().FirstOrDefault();
                SaltCalculationSchemes? r = customSA?.Scheme;
                if (r.HasValue && (r.Value & scheme) != 0)
                    yield return pi.Name;
            }

            foreach (PropertyInfo pi in typeof(ISaltAnalysisDryData).GetRuntimeProperties())
            {
                SchemesToCheckAttibute customSA = pi.GetCustomAttributes(true).ToList().OfType<SchemesToCheckAttibute>().FirstOrDefault();
                SaltCalculationSchemes? r = customSA?.Scheme;
                if (r.HasValue && (r.Value & scheme) != 0)
                    yield return pi.Name;
            }
        }
    }
}