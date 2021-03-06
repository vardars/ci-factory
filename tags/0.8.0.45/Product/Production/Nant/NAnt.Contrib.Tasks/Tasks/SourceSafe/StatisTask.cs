#region GNU General Public License
//
// NAntContrib
// Copyright (C) 2001-2003 Gerry Shaw
//
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307 USA
//

// Jason Reimer, Diversant Inc. (jason.reimer@diversant.net)
#endregion

using System;
using System.IO;

using SourceSafeTypeLib;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAnt.Contrib.Tasks.SourceSafe {
    /// <summary>
    /// Task used to checkout files from Visual Source Safe.
    /// </summary>
    /// <example>
    ///   <para>Checkout the latest files from a local sourcesafe database.</para>
    ///   <code><![CDATA[
    ///     <vsscheckout 
    ///       user="myusername" 
    ///       password="mypassword" 
    ///       localpath="C:\Dev\Latest"
    ///       recursive="true"
    ///       writable="true"
    ///       dbpath="C:\VSS\srcsafe.ini"
    ///       path="$/MyProduct"
    ///     />
    ///   ]]></code>
    /// </example>
    /// <example>
    ///   <para>Checkout a file from a remote sourcesafe database.  Put it in a relative directory.</para>
    ///   <code><![CDATA[
    ///     <vsscheckout 
    ///       user="myusername" 
    ///       password="mypassword" 
    ///       localpath="Latest"
    ///       recursive="false"
    ///       writable="true"
    ///       dbpath="\\MyServer\VSS\srcsafe.ini"
    ///       path="$/MyProduct/myFile.cs"
    ///     />
    ///   ]]></code>
    /// </example>
    [TaskName("vssstatis")]
    public sealed class StatisTask : BaseTask {
        #region Private Instance Fields

        private string _PropertyName;

        #endregion Private Instance Fields

        #region Public Instance Properties

        [TaskAttribute("property")]
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                _PropertyName = value;
            }
        }

        #endregion Public Instance Properties

        #region Override implementation of Task

        protected override void ExecuteTask() {
            Open();

            try 
            {
                this.Properties[this.PropertyName] = (Item.IsCheckedOut > 0).ToString();

            } catch (Exception ex) {
                throw new BuildException("The statis operation failed.", 
                    Location, ex);
            }
        }

        #endregion Override implementation of Task
    }
}
