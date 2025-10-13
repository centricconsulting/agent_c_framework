using System;
using System.Collections.Generic;
using System.Text;

namespace IFI.Integrations.Response
{
    public  class LNComFirm
    {
        //LexisNexis Commercial Firmographics
        public string BusinessName { get; set; }
        public string DBA { get; set; }
        public string FEIN { get; set; }
        public string NAICS { get; set; }
        public string LegalEntity { get; set; }
        public string OtherLegalEntity { get; set; }
        public string Phone { get; set; }
        public string URL { get; set; }
        public string YearStarted { get; set; }
    }
}
