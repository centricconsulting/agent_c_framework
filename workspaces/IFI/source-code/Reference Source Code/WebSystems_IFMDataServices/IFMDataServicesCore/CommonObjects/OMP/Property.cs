using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCE = Diamond.Common.Enums;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Property
    {

        public CommonObjects.Enums.Enums.ClaimLocationOfLoss LocationOfLossType { get; set; }
        public string DamageDescription { get; set; }
        public DCE.StatusCode Status { get; set; }
        public CommonObjects.Enums.Enums.DwellingType DwellingType { get; set; }
        public decimal EstimatedAmount { get; set; }
        public string LocationOfAccident { get; set; }
        public CommonObjects.OMP.FNOLLocation Location { get; set; }
        public CommonObjects.OMP.Address Address { get; set; }
    }
}
