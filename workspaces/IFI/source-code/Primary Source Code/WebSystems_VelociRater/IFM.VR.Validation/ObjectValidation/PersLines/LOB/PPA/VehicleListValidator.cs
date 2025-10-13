using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class VehicleListValidator
    {
        //This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{AA47863A-4806-4BE4-9C35-2975502933B5}";

        public const string VehicleListNoHasMotorCycleVehicles = "{66A037AA-CB39-459E-B8A0-308B0AADA84E}";
        public const string VehicleListNotAllDriversAreAssignedToAVehicle = "{646B851E-E94C-49EE-ABA2-46E4D6DAA6B4}";
        public const string VehicleListNoVehicles = "{1ACC117C-34A6-4E10-B607-F780AAEE5CFB}";

        public static Validation.ObjectValidation.ValidationItemList ValidateVehicleList(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.Vehicles != null && quote.Vehicles.Any())
                {
                    bool hasBodyTypeOtherThanMotorhomeAndMotorCycle = false;

                    foreach (QuickQuote.CommonObjects.QuickQuoteVehicle v in quote.Vehicles)
                    {
                        if (v.BodyTypeId != "42")
                            hasBodyTypeOtherThanMotorhomeAndMotorCycle = true;
                    }

                    if (hasBodyTypeOtherThanMotorhomeAndMotorCycle == false)
                        valList.Add(new ObjectValidation.ValidationItem("Motorcycle only body type will require Underwriting review prior to issuance.", VehicleListNoHasMotorCycleVehicles, true));

                    if (!IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(quote))
                    {
                        // make sure all rated drivers have are assigned to a vehicle either as prim or occ
                        if (quote.Drivers != null && quote.Vehicles != null)
                        {
                            Int32 driverNum = 0;

                            foreach (QuickQuote.CommonObjects.QuickQuoteDriver dr in quote.Drivers)
                            {
                                driverNum += 1;
                                // is rated
                                if (dr.DriverExcludeTypeId == "1")
                                {
                                    bool driverIsOnAVehicle = false;
                                    foreach (QuickQuote.CommonObjects.QuickQuoteVehicle v in quote.Vehicles)
                                    {
                                        if (driverNum.ToString() == v.PrincipalDriverNum | driverNum.ToString() == v.OccasionalDriver1Num | driverNum.ToString() == v.OccasionalDriver2Num | driverNum.ToString() == v.OccasionalDriver3Num)
                                            driverIsOnAVehicle = true;
                                    }
                                    if (driverIsOnAVehicle == false)
                                        valList.Add(new ObjectValidation.ValidationItem(string.Format("Driver #{0} Must be Assigned", driverNum), VehicleListNotAllDriversAreAssignedToAVehicle));
                                }
                            }
                        }
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("No vehicle information", VehicleListNoVehicles));
                }
            }

            return valList;
        }
    }
}