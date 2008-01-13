using System;
using System.Collections.Generic;
using System.Text;
using CIFactory.NAnt.Types;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using MbUnit;
using MbUnit.Framework;

namespace CIFactory.NAnt.UnitTest.Tasks
{
    [TaskName("assertequal")]
    public class AssertEqualTask : Task
    {
        #region Fields

        private string _Actual = String.Empty;

        private string _Expected = String.Empty;

        private string _Message = String.Empty;
        public string Variable;

        #endregion

        #region Properties

        [TaskAttribute("actual", Required = true)]
        public string Actual
        {
            get
            {
                return _Actual;
            }
            set
            {
                _Actual = value;
            }
        }

        [TaskAttribute("expected", Required = true)]
        public string Expected
        {
            get
            {
                return _Expected;
            }
            set
            {
                _Expected = value;
            }
        }

        [TaskAttribute("message", Required = true)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            try
            {
                Assert.AreEqual(this.Expected, this.Actual, this.Message, this.Expected, this.Actual);
            }
            catch (Exception ex)
            {
                throw new BuildException(ex.Message, this.Location, ex);
            }
        }

        #endregion

    }

    [TaskName("assertnotequal")]
    public class AssertNotEqualTask : Task
    {
        #region Fields

        private string _Actual = String.Empty;

        private string _Expected = String.Empty;

        private string _Message = String.Empty;
        public string Variable;

        #endregion

        #region Properties

        [TaskAttribute("actual", Required = true)]
        public string Actual
        {
            get
            {
                return _Actual;
            }
            set
            {
                _Actual = value;
            }
        }

        [TaskAttribute("expected", Required = true)]
        public string Expected
        {
            get
            {
                return _Expected;
            }
            set
            {
                _Expected = value;
            }
        }

        [TaskAttribute("message", Required = true)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            try
            {
                Assert.AreNotEqual(this.Expected, this.Actual, this.Message, this.Expected, this.Actual);
            }
            catch (Exception ex)
            {
                throw new BuildException(ex.Message, this.Location, ex);
            }
        }

        #endregion

    }


    [TaskName("assertgreaterthan")]
    public class AssertGreaterThanTask : Task
    {
        #region Fields

        private string _Right = String.Empty;

        private string _Left = String.Empty;

        private string _Message = String.Empty;

        #endregion

        #region Properties

        [TaskAttribute("right", Required = true)]
        public string Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
            }
        }

        [TaskAttribute("left", Required = true)]
        public string Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
            }
        }

        [TaskAttribute("message", Required = true)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            try
            {
                Assert.GreaterThan(this.Left, this.Right, this.Message, this.Left, this.Right);
            }
            catch (Exception ex)
            {
                throw new BuildException(ex.Message, this.Location, ex);
            }
        }

        #endregion

    }

    [TaskName("assertfail")]
    public class AssertFail : Task
    {
        #region Fields

        private string _Message = String.Empty;

        #endregion

        #region Properties

        [TaskAttribute("message", Required = true)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            try
            {
                Assert.Fail(this.Message);
            }
            catch (Exception ex)
            {
                throw new BuildException(ex.Message, this.Location, ex);
            }
        }

        #endregion

    }
}
