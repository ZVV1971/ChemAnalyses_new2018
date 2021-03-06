﻿using System;

namespace SA_EF
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SchemesToCheckAttibute: Attribute
    {
        public SaltCalculationSchemes Scheme;
        public SchemesToCheckAttibute(SaltCalculationSchemes sch) { Scheme = sch; }
        public SchemesToCheckAttibute():this(SaltCalculationSchemes.Chloride){}
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class SchemeRealizedAttribute : Attribute
    {
        public SchemeRealizedAttribute() { }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class CustomDescriptionAttribute : Attribute
    {
        public string Description;
        public CustomDescriptionAttribute(string description) { Description = description; }
        public override string ToString() { return Description; }
    }
}