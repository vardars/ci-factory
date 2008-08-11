using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;

namespace CIFactory.TargetProcess.NAnt.Tasks
{
    public class Entity
    {
        #region Fields

        private string _Description = String.Empty;

        private string _HyperLink = String.Empty;

        private int _Id;
        private string _Name = String.Empty;

        private string _Type;

        #endregion

        #region Constructors

        public Entity(string description, string hyperLink, string name, string type, int id)
        {
            _Description = description;
            _HyperLink = hyperLink;
            _Name = name;
            _Type = type;
            _Id = id;
        }

        public Entity()
        {
        }

        #endregion

        #region Properties

        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        public string HyperLink
        {
            get
            {
                return this._HyperLink;
            }
            set
            {
                this._HyperLink = value;
            }
        }

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
            }
        }

        #endregion

    }
}
