using System;

namespace SA_EF
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SchemesToCheckAttibute: Attribute
    {
        public SaltCalculationSchemes Scheme;
        public SchemesToCheckAttibute(SaltCalculationSchemes sch) { Scheme = sch; }
        public SchemesToCheckAttibute():this(SaltCalculationSchemes.Chloride){}
    }
}