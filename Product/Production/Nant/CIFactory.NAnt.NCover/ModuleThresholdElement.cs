#region Copyright � 2006 Grant Drake. All rights reserved.
/*
Copyright � 2006 Grant Drake. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;
using NCoverExplorer.NAntTasks.Types;

namespace NCoverExplorer.NAntTasks
{
    [ElementName("moduleThreshold")]
    public class ModuleThresholdElement : Element
    {
        // Fields
        private string _moduleName;
        private float _satisfactoryCoverage;

        // Methods
        public ModuleThresholdElement()
            : this(string.Empty, 100f)
        {
        }

        public ModuleThresholdElement(string moduleName, float satisfactoryCoverage)
        {
            this._moduleName = moduleName;
            this._satisfactoryCoverage = satisfactoryCoverage;
        }

        // Properties
        [TaskAttribute("moduleName")]
        public string ModuleName
        {
            get
            {
                return this._moduleName;
            }
            set
            {
                this._moduleName = value;
            }
        }

        [TaskAttribute("satisfactoryCoverage")]
        public float SatisfactoryCoverage
        {
            get
            {
                return this._satisfactoryCoverage;
            }
            set
            {
                this._satisfactoryCoverage = value;
            }
        }
    }
}
