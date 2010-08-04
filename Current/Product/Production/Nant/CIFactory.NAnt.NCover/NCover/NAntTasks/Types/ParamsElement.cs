namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NCover.Interfaces;
    using System;

    [ElementName("param")]
    public class ParamsElement : Element
    {
        private NameValuePair nvp = new NameValuePair();

        [StringValidator(AllowEmpty=false), TaskAttribute("name")]
        public string Name
        {
            get
            {
                return this.nvp.Name;
            }
            set
            {
                this.nvp.Name = value.ToLowerInvariant();
            }
        }

        public NameValuePair Pair
        {
            get
            {
                return this.nvp;
            }
        }

        [TaskAttribute("value")]
        public string Value
        {
            get
            {
                return this.nvp.Value;
            }
            set
            {
                this.nvp.Value = value;
            }
        }
    }
}
