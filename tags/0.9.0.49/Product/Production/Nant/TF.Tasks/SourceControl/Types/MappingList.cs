using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;

namespace TF.Tasks.SourceControl.Types
{

    [ElementName("mappings")]
    public class MappingList : DataTypeBase
    {
        private Mapping[] _Mappings;

        [BuildElementArray("mapping", ElementType = typeof(Mapping))]
        public Mapping[] Mappings
        {
            get
            {
                return _Mappings;
            }
            set
            {
                _Mappings = value;
            }
        }

        public WorkingFolder[] GetMappings()
        {
            List<WorkingFolder> ListOfMaps = new List<WorkingFolder>();

            foreach (Mapping MapElement in this.Mappings)
            {
                ListOfMaps.Add(MapElement.GetWorkingFolder());
            }

            return ListOfMaps.ToArray();
        }

    }

}
