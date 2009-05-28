using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using System.Text.RegularExpressions;

namespace CIFactory.NAnt.Types
{
    [ElementName("package")]
    public class PackageElement : Element
    {
        [TaskAttribute("name", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string PackageName { get; set; }

        [TaskAttribute("type", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string PackageType { get; set; }

        private bool _If = true;
        [TaskAttribute("if")]
        [BooleanValidator]
        public virtual bool If
        {
            get
            {
                return _If;
            }
            set
            {
                _If = value;
            }
        }

        private bool _Unless = false;
        [TaskAttribute("unless")]
        [BooleanValidator]
        public virtual bool Unless
        {
            get
            {
                return _Unless;
            }
            set
            {
                _Unless = value;
            }
        }

    }
}
