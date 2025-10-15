using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.CommonObjects.Diamond
{
    public class VersionInfo
    {
        public int VersionId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int LobId { get; set; }
        public string LobName { get; set; }
        public string LobAbbreviation { get; set; }
        public string State { get; set; }
        public int StateId { get; set; }
        public string StateAbbreviation { get; set; }
    }
}
