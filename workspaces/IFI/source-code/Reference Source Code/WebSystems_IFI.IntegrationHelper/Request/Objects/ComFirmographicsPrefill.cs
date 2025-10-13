using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Request.Objects
{
    public class ComFirmographicsPrefill
    {
        public string BusinessName { get; set; }
        public string Address1 { get; set; }
        public string OtherInfo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
