// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gerry Shaw (gerry_shaw@yahoo.com)
// Scott Hernandez (ScottHernandez@hotmail.com)

using System;
using System.Collections;
using System.Globalization;
using CIScript.Core.Util;

namespace CIScript.Core {
    [Serializable()]
    public class TargetCollection : ArrayList {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public virtual int Add(Target t){
            // throw an exception if an attempt is made to add a null target
            if (t == null) {
                throw new BuildException("Null Target!");
            }

            logger.Debug(string.Format(
                CultureInfo.InvariantCulture,
                ResourceUtils.GetString("String_AddingTarget"), 
                t.Name));
            
            // check for existing target with same name.
            if (Find(t.Name) == null) {
                return base.Add(t);
            } else {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                    ResourceUtils.GetString("NA1073"), t.Name));
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
            foreach (Target target in this) {
                if (target.Name == targetName)
                    return target;
            }
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
                targetNames[i] = ((Target) this[i]).Name;
            }

            return string.Join(separator, targetNames);
        }

        public override int Add(object value) {
            // call typed version above.
            return Add(value as Target);
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
    }
}