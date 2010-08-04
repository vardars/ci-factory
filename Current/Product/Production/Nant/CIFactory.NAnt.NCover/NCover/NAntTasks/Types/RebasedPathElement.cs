namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;
    using System.Runtime.CompilerServices;

    [ElementName("path")]
    public class RebasedPathElement : Element
    {
        [StringValidator(AllowEmpty=false), TaskAttribute("from")]
        public string FromPath { get; set; }

        [StringValidator(AllowEmpty=false), TaskAttribute("to")]
        public string ToPath { get; set; }
    }
}
