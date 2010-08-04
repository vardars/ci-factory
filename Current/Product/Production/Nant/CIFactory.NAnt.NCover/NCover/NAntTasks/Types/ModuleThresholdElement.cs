namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;

    [ElementName("moduleThreshold"), Obsolete]
    public class ModuleThresholdElement : Element
    {
        private string _moduleName;
        private float _satisfactoryCoverage;

        public ModuleThresholdElement() : this(string.Empty, 100f)
        {
        }

        public ModuleThresholdElement(string moduleName, float satisfactoryCoverage)
        {
            this._moduleName = moduleName;
            this._satisfactoryCoverage = satisfactoryCoverage;
        }

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
