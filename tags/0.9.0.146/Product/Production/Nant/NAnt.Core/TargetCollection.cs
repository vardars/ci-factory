// NAnt - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
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
// Gerry Shaw (gerry_shaw@yahoo.com)
// Scott Hernandez (ScottHernandez@hotmail.com)

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using NAnt.Core.Util;

namespace NAnt.Core {
    [Serializable()]
    public class TargetCollection : KeyedCollection<string, Target> 
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void InsertItem(int index, Target item)
        {
            this.ValidateEntry(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, Target item)
        {
            this.ValidateEntry(item);
            base.SetItem(index, item);
        }
        private void ValidateEntry(Target item){
            // throw an exception if an attempt is made to add a null target
            if (item == null) {
                throw new BuildException("Null Target!");
            }

            logger.Debug(string.Format(
                CultureInfo.InvariantCulture,
                ResourceUtils.GetString("String_AddingTarget"), 
                item.Name));
            
            // check for existing target with same name.
            if (this.Contains(item.Name)) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                    ResourceUtils.GetString("NA1073"), item.Name));
            }
        }

        /// <summary>
        /// Finds a target by name.
        /// </summary>
        /// <param name="targetName">The name of the target to find.</param>
        /// <returns>
        /// The <see cref="Target" /> with the specified name, or 
        /// <see langword="null" /> if no <see cref="Target" /> exists with
        /// the given name.
        /// </returns>
        public Target Find(string targetName) {
            if (this.Contains(targetName))
                return this[targetName];
            return null;
        }

        /// <summary>
        /// Gets the names of the targets in the <see cref="TargetCollection" />
        /// combined into one list separated by the given <see cref="string" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that contains a list of the names of the 
        /// targets in the <see cref="TargetCollection" />, separated by
        /// the specified <paramref name="separator" />.
        /// </returns>
        public string ToString(string separator) {
            string[] targetNames = new string[Count];

            for (int i = 0; i < Count; i++) {
                targetNames[i] = ((Target) this.Items[i]).Name;
            }

            return string.Join(separator, targetNames);
        }

        #region Override implementation of Object

        /// <summary>
        /// Gets the names of the targets in the <see cref="TargetCollection" />
        /// combined into one comma-separated list.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that contains a comma-separated list of the
        /// names of the targets in the <see cref="TargetCollection" />.
        /// </returns>
        public override string ToString() {
            return ToString(", ");
        }

        #endregion Override implementation of Object

        protected override string GetKeyForItem(Target item)
        {
            return item.Name;
        }
    }
}