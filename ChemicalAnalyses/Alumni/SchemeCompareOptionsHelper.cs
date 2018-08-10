using System;
using SA_EF;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChemicalAnalyses.Alumni
{
    public static class SchemeCompareOptionsHelper
    {
        /// <summary>
        /// Works for each scheme marked as realized (by custom attribute) in the corresponding enum
        /// options are grouped in an instance of special class
        /// options are got either from user-level settings or created anew
        /// </summary>
        /// <returns>Dictionary of key-value pairs scheme-scheme comparison options</returns>
        public static IDictionary<SaltCalculationSchemes, SchemeResultsTolerance> GetSchemeCompareOptions()
        {
            Dictionary<SaltCalculationSchemes, SchemeResultsTolerance> dict = 
                new Dictionary<SaltCalculationSchemes, SchemeResultsTolerance>();
            SchemeResultsTolerance sc;
            foreach (var p in Enum.GetValues(typeof(SaltCalculationSchemes))
                .OfType<SaltCalculationSchemes>().Where(p => p.GetAttribute<SchemeRealizedAttribute>() != null))
            {
                try
                {
                    sc = new SchemeResultsTolerance(Properties.Settings.Default[p.ToString() + "_SchemeToleranceValues"].ToString());
                }
                catch (Exception ex)
                {
                    //No setting is present create new one
                    sc = new SchemeResultsTolerance()
                    {
                        IsUniversalTolerance = true,
                        UniversalTolerance = 0.005M,
                        SchemeTolerances = new ObservableCollection<ParameterValuePair>(
                        SchemesHelper.GetPropertiesToCheck(p)
                        .Select(r => new ParameterValuePair() { Item1 = r, Item2 = 0.005M }))
                    };
                }
                dict.Add( p, sc);
            }
            return dict;
        }
    }
}