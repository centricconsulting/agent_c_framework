using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCS = Diamond.Common.Services;
using DCO = Diamond.Common.Objects;
using IDS = Insuresoft.DiamondServices;

namespace IFM.DataServicesCore.CommonObjects.Diamond
{
    public class AssignAgencyResult
    {
        public int agencyID { get; set; }
        public bool success { get; set; }
        public DCO.DiamondValidation DiamondValidation { get; set; }
        public System.Exception APIException { get; set; }
        public string ErrorMessage { get; set; }
    }
}
