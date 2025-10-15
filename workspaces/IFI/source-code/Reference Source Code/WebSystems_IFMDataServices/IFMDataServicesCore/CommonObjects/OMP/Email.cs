using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Email
    {
        
        public string EmailId { get; set; }
        public Int32 TypeID { get; set; }
        public string Type { get; set; }
    }
}
