using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Response
{
    public  class LNComProp
    {
        //LexisNexis Commercial Property
        public int stories { get; set; }
        public bool hasFireplaces { get; set; }
        public int pools { get; set; }
        public int yearBuilt { get; set; }
        public string municipalityOrTownship { get; set; }
        public int squareFeet { get; set; }
    }
}
