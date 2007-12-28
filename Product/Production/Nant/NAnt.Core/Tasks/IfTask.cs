// NAnt - A .NET build tool
// Copyright (C) 2002-2003 Scott Hernandez
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Scott Hernandez (ScottHernandez@hotmail.com)
// Jaroslaw Kowalski (jkowalski@users.sourceforge.net)

using System;
using System.Globalization;
using System.IO;

using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace NAnt.Core.Tasks {
    /// <summary>
    /// Checks the conditional attributes and executes the children if
    /// <see langword="true" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     If no conditions are checked, all child tasks are executed. 
    ///     </para>
    ///     <para>
    /// </remarks>
    /// <example>
    /// <para>Tests the value of a property using expressions.</para>
    /// <code>
    /// <if test="${build.configuration='release'}">
    ///     <echo>Build release configuration</echo>
    /// </if>
    /// </code>
    /// </example>
    /// <example>
    /// <para>Tests the the output of a function.</para>
    /// <code>
    /// <if test="${not file::exists(filename) or file::get-length(filename) = 0}">
    ///     <echo message="The version file ${filename} doesn't exist or is empty!" />
    /// </if>
    /// </code>
    /// </example>
    [TaskName("if")]
    public class IfTask : TaskContainer {
        #region Private Instance Fields

        private string _test = null;

        #endregion Private Instance Fields

        #region Public Instance Properties

        /// <summary>
        /// Used to test arbitrary boolean expression.
        /// </summary>
        [TaskAttribute("test", Required = true)]
        [BooleanValidator()]
        public string Test {
            get { return _test; }
            set { _test = StringUtils.ConvertEmptyToNull(value); }
        }

        #endregion Public Instance Properties

        #region Protected Instance Properties

        protected virtual bool ConditionsTrue {
            get {
                bool ret = true;

                if (Test != null) {
                    if (!Convert.ToBoolean(Test, CultureInfo.InvariantCulture)) {
                        return false;
                    }
                }

                return ret;
            }
        }

        #endregion Protected Instance Properties

        #region Override implementation of TaskContainer

        protected override void ExecuteTask() {
            if (ConditionsTrue) {
                base.ExecuteTask();
            }
        }

        #endregion Override implementation of TaskContainer

        #region Override implementation of Task

        protected override void InitializeTask(System.Xml.XmlNode taskNode) {
            base.InitializeTask (taskNode);
            //check that we have something to do.
            if (Test == null) {
                throw new BuildException("At least the condition" +
                        "test must be set:", Location);
            }
        }

        #endregion Override implementation of Task
    }

    /// <summary>
    /// The opposite of the <c>if</c> task.
    /// </summary>
    /// <example>
    ///   <para>Check that a property does not exist.</para>
    ///   <code>
    ///     <![CDATA[
    /// <ifnot propertyexists="myProp">
    ///     <echo message="myProp does not exist."/>
    /// </if>
    ///     ]]>
    ///   </code>
    ///   <para>Check that a property value is not true.</para>
    ///   <code>
    ///     <![CDATA[
    /// <ifnot propertytrue="myProp">
    ///     <echo message="myProp is not true."/>
    /// </if>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>Check that a target does not exist.</para>
    ///   <code>
    ///     <![CDATA[
    /// <ifnot targetexists="myTarget">
    ///     <echo message="myTarget does not exist."/>
    /// </if>
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("ifnot")]
    public class IfNotTask : IfTask {
        #region Override implementation of IfTask

        protected override bool ConditionsTrue {
            get { return !base.ConditionsTrue; }
        }

        #endregion Override implementation of IfTask
    }
}
