using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCE = Diamond.Common.Enums;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class FNOLVehicle : ModelBase
    {
        public CommonObjects.OMP.Address LocationOfAccidentAddress { get; set; }
        public string LossVehicleOwnerLastName { get; set; }
        public string LossVehicleOwnerMiddleName { get; set; }
        public string LossVehicleOwnerFirstName { get; set; }
        public string LossVehicleOperatorLastName { get; set; }
        public string LossVehicleOperatorMiddleName { get; set; }
        public string LossVehicleOperatorFirstName { get; set; }
       public string LossVehicleOwnerDisplayName { get; set; }
        public DCE.StatusCode Status { get; set; }
        public int Year { get; set; }
        public string VIN { get; set; }
        public bool Drivable { get; set; }
        public string Color { get; set; }
        public bool UsedWithPermission { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public CommonObjects.Enums.Enums.State LicenseState { get; set; }
        public string LicensePlate { get; set; }
        public bool InvolvedInLoss { get; set; }
        public decimal EstimatedAmountOfDamage { get; set; }
        public string DamageDescription { get; set; }
        public CommonObjects.Enums.Enums.ClaimLossIndicatorType LossIndicatorType { get; set; }
        public CommonObjects.OMP.Name  LossVehicleOwnerName { get; set; }
        public CommonObjects.OMP.Name LossVehicleOperatorName { get; set; }
        public string VehicleDamage { get; set; }
        public decimal VehicleDamageAmt { get; set; }

        public int DrivableId { get; set; }

        public int AirbagsDeployedTypeId { get; set; }

        public int CCCEstimateQualificationId { get; set; }
        public int VehicleLocatedInsuredAddressTypeId { get; set; }
        
        public string CCCphone { get; set; }

        public CommonObjects.OMP.Address LossAddress { get; set; }
    }
}
