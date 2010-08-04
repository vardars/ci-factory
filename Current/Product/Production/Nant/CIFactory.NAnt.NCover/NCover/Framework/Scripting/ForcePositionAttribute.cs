namespace NCover.Framework.Scripting
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Property, Inherited=false, AllowMultiple=false)]
    public sealed class ForcePositionAttribute : Attribute
    {
        public const int FIRST = 0;
        public const int FOURTH = 3;
        public const int SECOND = 1;
        public const int THIRD = 2;

        public ForcePositionAttribute(int position)
        {
            this.Position = position;
        }

        public int Position { get; private set; }
    }
}
