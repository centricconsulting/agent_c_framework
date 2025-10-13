using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;
using DCE = Diamond.Common.Enums;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class Claimant :ModelBase
    {
        public DCE.StatusCode Status { get; set; }

        //public string Firstname { get; set; }

        //public string Middlename { get; set; }

        //public string Lastname { get; set; }

        //public string Housenumber { get; set; }

        //public string Streetname { get; set; }

        //public string City { get; set; }

        //public string State { get; set; }

        //public string Zipcode { get; set; }

        //public string Homephone { get; set; }

        //public string Businessphone { get; set; }

        //public string Cellphone { get; set; }
        public List<CommonObjects.OMP.Email> Email { get; set; }
        public CommonObjects.Enums.Enums DwellingType { get; set; }

        public decimal EstimatedAmount { get; set; }

        public string LocationOfAccident { get; set; }
        public CommonObjects.OMP.FNOLLocation Location { get; set; }

        public int claimantTypeID { get; set; }

        // updated 4/29/2013 to use separate objects to store everything
        public CommonObjects.OMP.Name Name { get; set; }

        public CommonObjects.OMP.Address Address { get; set; }

        public List<CommonObjects.OMP.Phone> Phone { get; set; }
    }
}
