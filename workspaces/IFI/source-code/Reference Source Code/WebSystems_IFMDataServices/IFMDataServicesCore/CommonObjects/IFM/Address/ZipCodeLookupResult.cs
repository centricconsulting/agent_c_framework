using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.IFM.Address
{
    public class ZipCodeLookupResult
    {
        public string City { get; set; }
        public string County { get; set; }
        public string StateAbbrev { get; set; }
        public string ZipCode { get; set; }
    }
}
