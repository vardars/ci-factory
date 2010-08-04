namespace NCover.Framework.Scripting
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited=true, AllowMultiple=true)]
    public class ScriptArgumentAttribute : Attribute
    {
        protected ScriptArgumentAttribute()
        {
            this.UseValue = true;
            this.ForVersionsPriorTo = 0x7fffffff;
        }

        public ScriptArgumentAttribute(string argName, ScriptOutputType typeAppliesTo)
        {
            this.ArgName = argName;
            this.TypeAppliesTo = typeAppliesTo;
            this.UseValue = true;
            this.ForVersionsPriorTo = 0x7fffffff;
        }

        public virtual string ArgName { get; protected set; }

        public virtual Type Converter { get; set; }

        public virtual int ForVersionsLaterThan { get; set; }

        public virtual int ForVersionsPriorTo { get; set; }

        public virtual bool IgnoreWhenGeneratingBuildTask { get; set; }

        public virtual ScriptOutputType TypeAppliesTo { get; protected set; }

        public virtual bool UseValue { get; set; }
    }
}
