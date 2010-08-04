namespace NCover.Interfaces
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, AllowMultiple=false)]
    public sealed class ForNCoverFeatureAttribute : Attribute
    {
        public ForNCoverFeatureAttribute(NCoverFeature feature)
        {
            this.Feature = feature;
        }

        public NCoverFeature Feature { get; private set; }
    }
}
