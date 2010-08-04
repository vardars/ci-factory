namespace NCover.Framework.Scripting
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;

    public class ScriptArgumentCollectionAttribute : ScriptArgumentAttribute
    {
        public ScriptArgumentCollectionAttribute(string parentName, string childName, Type childType, ScriptOutputType typeAppliesTo) : base(parentName, typeAppliesTo)
        {
            this.ChildrenName = childName;
            this.ChildrenType = childType;
        }

        public virtual string ChildrenName { get; protected set; }

        public virtual Type ChildrenType { get; protected set; }
    }
}
