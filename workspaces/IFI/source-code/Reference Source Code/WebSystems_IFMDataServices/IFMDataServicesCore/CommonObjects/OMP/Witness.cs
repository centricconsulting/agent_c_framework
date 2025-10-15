using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Witness : Person
    {
        public string Remarks { get; set; }
        public string RelationshipId { get; set; }
    }
}
