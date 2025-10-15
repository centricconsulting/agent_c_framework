using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Request.Objects
{
    public class ComPropertyPrefill
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string UnitNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
