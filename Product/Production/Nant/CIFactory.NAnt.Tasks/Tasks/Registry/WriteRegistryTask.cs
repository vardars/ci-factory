// Marcin Hoppe ([EMAIL PROTECTED])

using System;
using System.Globalization;
using System.Security.Permissions;

using Microsoft.Win32;

using NAnt.Core;
using NAnt.Core.Attributes;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,
Unrestricted = true)]

namespace CIFactory.NAnt.Tasks
{
    /// <summary>
    /// Writes a specified value to the Windows Registry.
    /// </summary>
    /// <example>
    /// <para>Write a single value to the Registry.</para>
    ///   <code>
    ///     <![CDATA[
    /// <writeregistry value="A test value" key="SOFTWARE\NAnt-Test\SampleValue" hive="LocalMachine" />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("writeregistry")]
    public class WriteRegistryTask : Task
    {

        #region Private Instance Fields

        private string _regKeyValue = null;
        private string _regKey = null;
        private string _regKeyValueName = null;
        private RegistryHive[] _regHive = new RegistryHive[] { RegistryHive.LocalMachine };
        private string _regHiveString = RegistryHive.LocalMachine.ToString();

        #endregion

        #region Public Instance Properties

        /// <summary>
        /// Value to be stored in the Registry.
        /// </summary>
        [TaskAttribute("value", Required = true)]
        public virtual string RegistryKeyValue
        {
            get { return _regKeyValue; }
            set { _regKeyValue = value; }
        }

        /// <summary>
        /// The registry key to write to, including the path.
        /// </summary>
        /// <example>
        /// SOFTWARE\NAnt-Test
        /// </example>
        [TaskAttribute("key", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public virtual string RegistryKey
        {
            get { return _regKey; }
            set
            {
                string key = value;
                if (value.StartsWith("\\"))
                {
                    key = value.Substring(1);
                }
                string[] pathParts = key.Split("\\".ToCharArray(0, 1)[0]);
                //split the key/path apart.
                _regKeyValueName = pathParts[pathParts.Length - 1];
                _regKey = key.Substring(0, (value.Length - _regKeyValueName.Length));
            }
        }

        /// <summary>
        /// Space separated list of registry hives to search for <see cref="RegistryKey" />.
        /// For a list of possible values, see <see cref="RegistryHive" />. The 
        /// default is <see cref="RegistryHive.LocalMachine" />.
        /// </summary>
        /// <remarks>
        /// <seealso cref="RegistryHive" />
        /// </remarks>
        [TaskAttribute("hive")]
        public virtual string RegistryHiveName
        {
            get { return _regHiveString; }
            set
            {
                _regHiveString = value;
                string[] tempRegHive = _regHiveString.Split(" ".ToCharArray()[0]);
                _regHive = (RegistryHive[])Array.CreateInstance(typeof(RegistryHive), tempRegHive.Length);
                for (int x = 0; x < tempRegHive.Length; x++)
                {
                    _regHive[x] = (RegistryHive)Enum.Parse(typeof(RegistryHive), tempRegHive[x], true);
                }
            }
        }

        #endregion Public Instance Properties

        #region Override implementation of Task

        /// <summary>
        /// Write the specified value to the Registry.
        /// </summary>
        protected override void ExecuteTask()
        {
            if (_regKey == null)
            {
                throw new BuildException("Missing registry key!");
            }

            if (_regKeyValue == null)
            {
                throw new BuildException("Missing value!");
            }

            try
            {
                foreach (RegistryHive hive in _regHive)
                {
                    RegistryKey regKey = GetHiveKey(hive);

                    if (regKey != null)
                    {
                        RegistryKey newKey = regKey.CreateSubKey(_regKey);

                        if (newKey != null)
                        {
                            newKey.SetValue(_regKeyValueName, _regKeyValue);

                            string infoMessage = string.Format(CultureInfo.InvariantCulture,
                                "{0}{1} set to {2}.",
                                _regKey,
                                _regKeyValueName,
                                _regKeyValue);
                            Log(Level.Info, infoMessage);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new BuildException("Writing to registry failed!", e);
            }
        }

        #endregion

        #region Protected Instance Methods

        /// <summary>
        /// Returns the key for a given registry hive.
        /// </summary>
        /// <param name="hive">The registry hive to return the key for.</param>
        /// <returns>
        /// The key for a given registry hive.
        /// </returns>
        protected RegistryKey GetHiveKey(RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;
                case RegistryHive.Users:
                    return Registry.Users;
                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;
                default:
                    Log(Level.Verbose, "Registry not found for {0}.", hive.ToString());
                    return null;
            }
        }

        #endregion Protected Instance Methods

    }
}