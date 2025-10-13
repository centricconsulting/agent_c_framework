using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using IFM.DataServicesCore.CommonObjects.OMP;
using QuickQuote.CommonObjects;
using Diamond.Common.Enums;
using IFM.DataServicesCore.CommonObjects.OMP.PPA;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class EndorsementsValidator : BusinessLogicBase
    {
        public static bool ValidateEndorsementInfo(Endorsement endorsement)
        {
            bool returnVar = false;
            if (endorsement.PolicyId > 0)
            {
                switch (endorsement.TransactionType)
                {
                    case Endorsement.EndorsementTransactionType.Delete:
                        if (endorsement.PolicyImageNum > 0)
                            returnVar = true;
                        else
                            endorsement.ErrorMessage = "PolicyImageNum must be greater than 0 when deleting an endorsement";
                        break;
                    case Endorsement.EndorsementTransactionType.Rate:
                        if (endorsement.TransactionDate.HasValue())
                            returnVar = ValidateObject(endorsement);
                        else
                            endorsement.ErrorMessage = "TransactionDate must be a valid date.";
                        break;
                    case Endorsement.EndorsementTransactionType.Finalize:
                        if (endorsement.PolicyImageNum > 0)
                        {
                            if (endorsement.TransactionDate.HasValue() == false)
                                endorsement.ErrorMessage += "TransactionDate must be a valid date.";
                            //else if (endorsement.UserID <= 0)
                                //endorsement.ErrorMessage += "UserID must be greater than zero.";
                            else if (endorsement.Username.HasValue() == false)
                                endorsement.ErrorMessage += "Must have username.";

                            if (endorsement.ErrorMessage.HasValue() == false)
                                returnVar = true;
                        }
                        else
                        {
                            endorsement.ErrorMessage = "PolicyImageNum must be greater than 0 when finalizing an endorsement";
                        }
                        break;
                    default:
                        endorsement.ErrorMessage = "Must send a transaction type.";
                        returnVar = false;
                        break;
                }
            }
            else
            {
                endorsement.ErrorMessage = "PolicyId must be greater than 0";
            }
            return returnVar;
        }

        public static List<Endorsement.EndorsementLOBType> GetValidEndorsementLOBTypes(Endorsement.EndorsementObjectType objectType)
        {
            List<Endorsement.EndorsementLOBType> allowedLOBs = new List<Endorsement.EndorsementLOBType>();
            switch (objectType)
            {
                case Endorsement.EndorsementObjectType.Driver:
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.PPA);
                    break;
                case Endorsement.EndorsementObjectType.Lienholder:
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.PPA);
                    break;
                case Endorsement.EndorsementObjectType.LoanLenderInfo:
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.HOM);
                    break;
                case Endorsement.EndorsementObjectType.MailingAddress:
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.PPA);
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.HOM);
                    break;
                case Endorsement.EndorsementObjectType.PayPlan:
                    //allowedLOBs.Add(Endorsement.EndorsementLOBType.PPA);
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.HOM);
                    break;
                case Endorsement.EndorsementObjectType.Vehicle:
                    allowedLOBs.Add(Endorsement.EndorsementLOBType.PPA);
                    break;
            }
            return allowedLOBs;
        }

        private static bool ValidateObject(Endorsement endorsement, bool isFinalize = false)
        {
            if (ValidateLOB(endorsement))
            {
                switch (endorsement.ObjectType)
                {
                    case Endorsement.EndorsementObjectType.Driver:
                        return ValidateDriver(endorsement, isFinalize);
                    case Endorsement.EndorsementObjectType.Lienholder:
                        return ValidateLienholder(endorsement, isFinalize);
                    case Endorsement.EndorsementObjectType.Vehicle:
                        return ValidateVehicle(endorsement, isFinalize);
                    case Endorsement.EndorsementObjectType.MailingAddress:
                        return ValidateMailingAddress(endorsement, isFinalize);
                    case Endorsement.EndorsementObjectType.LoanLenderInfo:
                        return ValidateLoanLenderInfo(endorsement, isFinalize);
                    case Endorsement.EndorsementObjectType.PayPlan:
                        return ValidatePayPlan(endorsement, isFinalize);
                    default:
                        endorsement.ErrorMessage = "Must send an object type.";
                        return false;
                }
            }
            return false;
        }

        private static bool ValidateLOB(Endorsement endorsement)
        {
            List<Endorsement.EndorsementLOBType> allowedLOBs = GetValidEndorsementLOBTypes(endorsement.ObjectType);
            if(allowedLOBs.IsLoaded())
            {
                if (allowedLOBs.Contains(endorsement.LOBType))
                {
                    return true;
                }
                else
                {
                    endorsement.ErrorMessage = "Endorsements on " + endorsement.ObjectType.ToString() + " are only available on " + String.Join(", ", allowedLOBs) + " policies.";
                    return false;
                }
            }
            else
            {
                endorsement.ErrorMessage = "Couldn't load allowedLOBs list.";
                return false;
            }
        }

        private static bool ValidateDriver(Endorsement endorsement, bool isFinalize = false)
        {
            if (endorsement.Drivers?.Count > 0)
                switch (endorsement.ActionType)
                {
                    case Endorsement.EndorsementActionType.Add:
                    //return ValidateAddDriverInfo(endorsement);
                    case Endorsement.EndorsementActionType.Edit:
                        if (endorsement.PolicyImageNum <= 0 || (endorsement.PolicyImageNum > 0 && endorsement.Drivers[0].DriverNum >= 0))
                            return ValidateAddEditDriverInfo(endorsement);
                        else
                            endorsement.ErrorMessage = "Must provide the driver num of the driver you are adding/editing when working on an existing endorsement.";
                        break;
                    case Endorsement.EndorsementActionType.Delete:
                        if (endorsement.Drivers[0].DriverNum > -1)
                            return true;
                        else
                            if (endorsement.Drivers[0].DriverNum < 0)
                            endorsement.ErrorMessage = "Must provide the driver num of the driver you are deleting.";
                        break;
                }
            else
                endorsement.ErrorMessage = "Must send a driver object in order to do an endorsement on a driver.";
            return false;
        }

        private static bool ValidateAddEditDriverInfo(Endorsement endorsement)
        {
            string parentObjectInfo = "";
            if (endorsement.Drivers[0].DriverNum.HasValue())
                parentObjectInfo = "Driver " + endorsement.Drivers[0].DriverNum;
            else
                parentObjectInfo = "New ";

            if (endorsement.Drivers[0].LicenseStatusId.HasValue() == false)
                endorsement.ErrorMessage = "Driver must have drivers license status id.";
            else if (endorsement.Drivers[0].RelationshipTypeId.HasValue() == false)
                endorsement.ErrorMessage = "Driver must have relationship status id.";
            else if (endorsement.Drivers[0].RatedExcludedTypeId.HasValue() == false)
                endorsement.ErrorMessage = "Driver must have rated excluded type id.";

            if(endorsement.ErrorMessage.HasValue() == false)
                endorsement.ErrorMessage = ValidateDriverName(endorsement.Drivers[0].Name, endorsement.ObjectType, parentObjectInfo);

            return endorsement.ErrorMessage.HasValue() ? false : true;
        }

        private static bool ValidateLienholder(Endorsement endorsement, bool isFinalize = false)
        {
            bool returnVar = false;
            if (endorsement.Vehicles?.Count > 0)
            {
                foreach (var v in endorsement.Vehicles)
                {
                    returnVar = ValidatorEachLienholderVehicle(endorsement, v, isFinalize);
                    if (returnVar == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
                endorsement.ErrorMessage = "Must send a vehicle object in order to do an endorsement on a vehicle.";
            return false;
        }

        private static bool ValidatorEachLienholderVehicle(Endorsement endorsement, Vehicle v, bool isFinalize = false)
        {
            if (v.VehicleNum > -1)
                switch (endorsement.ActionType)
                {
                    case Endorsement.EndorsementActionType.Add:
                    case Endorsement.EndorsementActionType.Edit:
                        if (v.AdditionalInterests?.Count > 0)
                            if (endorsement.PolicyImageNum <= 0 || (endorsement.PolicyImageNum > 0 && v.AdditionalInterests.Last().AdditionalInterestNum > -1))
                                return ValidateAddEditLienholderInfo(endorsement, v, isFinalize);
                            else
                                endorsement.ErrorMessage = "Must provide the additionalInterestNum of the lienholder you are adding/editing when working on an existing endorsement.";
                        else
                            endorsement.ErrorMessage = "Must send an additionalInterest object in the vehicle object in order to do an endorsement on a lienholder.";
                        break;
                    case Endorsement.EndorsementActionType.Delete:
                        if (v.AdditionalInterests.Last().AdditionalInterestNum > -1)
                            return true;
                        else
                            if (v.AdditionalInterests == null || v.AdditionalInterests.Count == 0)
                            endorsement.ErrorMessage = "Must send an additionalInterest object (lienholder) in order to do an endorsement on a lienholder.";
                        else
                            endorsement.ErrorMessage = "Must provide the lienholder num of the lienholder you are deleting.";
                        break;
                }
            else
                endorsement.ErrorMessage = "Must provide the vehicle num of the vehicle which the lienholder is associated with.";

            return false;
        }

        private static bool ValidateAddEditLienholderInfo(Endorsement endorsement, Vehicle v, bool isFinalize = false)
        {
            endorsement.ErrorMessage = ValidateAdditionalInterest(v.AdditionalInterests, endorsement.ObjectType);
            return endorsement.ErrorMessage.HasValue() ? false : true;
        }

        private static bool ValidateVehicle(Endorsement endorsement, bool isFinalize = false)
        {
            if (endorsement.Vehicles?.Count > 0)
                switch (endorsement.ActionType)
                {
                    case Endorsement.EndorsementActionType.Add:
                    case Endorsement.EndorsementActionType.Edit:
                        if (endorsement.PolicyImageNum <= 0 || (endorsement.PolicyImageNum > 0 && endorsement.Vehicles[0].VehicleNum >= 0))
                            return ValidateAddEditVehicleInfo(endorsement);
                        else
                            endorsement.ErrorMessage = "Must provide the vehicle num of the vehicle you are adding/editing when working on an existing endorsement.";
                        break;
                    case Endorsement.EndorsementActionType.Delete:
                        if (endorsement.Vehicles[0].VehicleNum > -1)
                            return true;
                        else
                            endorsement.ErrorMessage = "Must provide the vehicle num of the vehicle you are deleting.";
                        break;
                }
            else
                endorsement.ErrorMessage = "Must send a vehicle object in order to do an endorsement on a vehicle.";
            return false;
        }

        private static bool ValidateAddEditVehicleInfo(Endorsement endorsement, bool isFinalize = false)
        {
            var v = endorsement.Vehicles[0];
            var parentObjectInfo = "Vehicle " + v.VehicleNum;
            if (v.BodyTypeId.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have body type id.";
            else if (v.AnnualMileage.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have annual mileage.";
            else if (v.Make.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have make.";
            else if (v.Model.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have model.";
            else if (v.Year.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have year.";
            else if (isFinalize && v.VIN.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have VIN at finalize.";
            else if (isFinalize && v.Year > 1980 && v.VIN.Length != 17)
                endorsement.ErrorMessage = "Vehicle must have VIN length of 17 characters.";
            else if (isFinalize && v.Year < 1981 && (v.VIN.Length < 11 || v.VIN.Length > 17))
                endorsement.ErrorMessage = "Vehicles before 1981 must have a VIN length between 11 and 17 characters.";
            else if (v.UseTypeId.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have use type id.";
            else if (v.PerformanceTypeId.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have performance type id.";
            else if ((v.CompSymbol.HasValue() == false || v.CollSymbol.HasValue() == false) && v.CostNew.HasValue() == false)
                endorsement.ErrorMessage = "Vehicle must have Comp/Coll symbols or cost new.";
            else if (v.Year.HasValue() && (DateTime.Now.Year - v.Year) > 5 && v.HasLoanLeaseCoverage == true)
                endorsement.ErrorMessage = "Vehicles 6 years or older are not eligible for Loan/Lease coverage.";
            else if (v.ComprehensiveDeductibleLimitId.HasValue() == false && IsVehicleWithLiabilityOnlySetCorrectly(v, ref endorsement.EndorsementStatus.refErrorMessage))
            {
                endorsement.SetErrorMessageByReferenceVar();
            }
            else if (v.TransportationExpenseLimitId.HasValue() && CanVehicleHaveTransportationExpense(v) == false)
            {
                endorsement.ErrorMessage = "Vehicle's Body Type excludes it from having Rental Reimbursement (Transportation Expense).";
            }
            else
            {
                //bool isRequired = endorsement.ActionType == Endorsement.EndorsementActionType.Add; //Turns out, this is not required.
                endorsement.ErrorMessage = ValidateAddress(v.GaragingAddress, true, endorsement.ObjectType, parentObjectInfo, false); //only required if we are adding a vehicle, if we are editing and have entered nothing, we will re-use existing vehicle info.
                if (endorsement.ErrorMessage.HasValue() == false)
                    endorsement.ErrorMessage = ValidateAdditionalInterest(v.AdditionalInterests, endorsement.ObjectType, parentObjectInfo, false);
            }

            return endorsement.ErrorMessage.HasValue() ? false : true;
        }

        private static string ValidateDriverName(Name endorsementName, Endorsement.EndorsementObjectType objectType, string parentObjectHierarchyInfo = "")
        {
            string stateAbbr = "";
            if(endorsementName.DLStateId.HasValue())
            {
                stateAbbr = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForDiamondStateId(endorsementName.DLStateId, true);
            }

            List<string> errorMsg = new List<string>();
            string errString = "";
            parentObjectHierarchyInfo = parentObjectHierarchyInfo.IsNotNull() ? parentObjectHierarchyInfo + " " : "";
            string errorMsgObjectHierarchy = parentObjectHierarchyInfo + objectType.ToString();
            if (endorsementName != null)
            {
                if (endorsementName.SexId.HasValue() == false)
                    errorMsg.Add(errorMsgObjectHierarchy + " must have sex id.");
                else if (endorsementName.MartialStatusId.HasValue() == false)
                    errorMsg.Add(errorMsgObjectHierarchy + " must have marital status id.");
                else if (endorsementName.DLN.HasValue() == false)
                    errorMsg.Add(errorMsgObjectHierarchy + " must have driver's license number.");
                else if (endorsementName.DLN.HasValue() && endorsementName.DLN.IsValidDriversLicenseNumber(stateAbbr, ref errString) == false)
                    errorMsg.Add(errorMsgObjectHierarchy + " must have a valid driver's license");
                else if (endorsementName.DLStateId.HasValue() == false)
                    errorMsg.Add(errorMsgObjectHierarchy + " must have driver's license state id.");
                else
                    errorMsg.Add(ValidatePersonalName(endorsementName, objectType, parentObjectHierarchyInfo));
            }
            else
            {
                    errorMsg.Add(objectType.ToString() + " name object must be passed with object.");
            }

            return errorMsg.Count > 0 ? errorMsg[0] : "";
        }

        private static string ValidatePersonalName(Name endorsementName, Endorsement.EndorsementObjectType objectType, string parentObjectHierarchyInfo = "", bool isRequired = true)
        {
            List<string> errorMsg = new List<string>();
            parentObjectHierarchyInfo = parentObjectHierarchyInfo.IsNotNull() ? parentObjectHierarchyInfo + " " : "";
            string errorMsgObjectHierarchy = parentObjectHierarchyInfo + objectType.ToString();
            if (endorsementName != null)
            {
                int totalCount = 0;
                if (endorsementName.FirstName.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + " must have first name.");
                else if (endorsementName.LastName.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + " must have last name.");

                if (isRequired == false && totalCount == errorMsg.Count) //All or nothing, if you have entered info we need to validate based on all criteria. If you have entered nothing and we don't deem it as required, only accept if nothing was entered (thus everything was false).
                {
                    return "";
                }
            }
            else
            {
                if(isRequired)
                    errorMsg.Add(objectType.ToString() + " name object must be passed with object.");
            }

            return errorMsg.Count > 0 ? errorMsg[0] : "";
        }

        private static string ValidateBusinessName(Name endorsementName, Endorsement.EndorsementObjectType objectType, string parentObjectHierarchyInfo = "", bool isRequired = true)
        {
            List<string> errorMsg = new List<string>();
            parentObjectHierarchyInfo = parentObjectHierarchyInfo.IsNotNull() ? parentObjectHierarchyInfo + " " : "";
            string errorMsgObjectHierarchy = parentObjectHierarchyInfo + objectType.ToString();
            if (endorsementName != null)
            {
                int totalCount = 0;
                if (endorsementName.CommercialName.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + " must have commercial name.");

                if (isRequired == false && totalCount == errorMsg.Count) //All or nothing, if you have entered info we need to validate based on all criteria. If you have entered nothing and we don't deem it as required, only accept if nothing was entered (thus everything was false).
                {
                    return "";
                }
            }
            else
            {
                if (isRequired)
                    errorMsg.Add(objectType.ToString() + " name object must be passed with object.");
            }

            return errorMsg.Count > 0 ? errorMsg[0] : "";
        }

        private static string ValidateAddress(Address endorsementAddress, bool requireCounty, Endorsement.EndorsementObjectType objectType, string parentObjectHierarchyInfo = "", bool isRequired = true)
        {
            List<string> errorMsg = new List<string>();
            int totalCount = 0;
            parentObjectHierarchyInfo = parentObjectHierarchyInfo.IsNotNull() ? parentObjectHierarchyInfo + " " : "";
            string errorMsgObjectHierarchy = parentObjectHierarchyInfo + objectType.ToString();
            if (endorsementAddress != null)
            {
                if(endorsementAddress.PoBox.HasValue() == false)
                {
                    if (endorsementAddress.HouseNumber.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                        errorMsg.Add(errorMsgObjectHierarchy + " must have house number.");
                    if (endorsementAddress.StreetName.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                        errorMsg.Add(errorMsgObjectHierarchy + "  must have street name.");
                }
                if (endorsementAddress.City.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + "  must have city.");
                if (endorsementAddress.StateId.HasValue() == false & endorsementAddress.StateAbbrev.HasValue() == false & endorsementAddress.State.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + "  must have state, state abbreviation, or state ID.");
                if (endorsementAddress.Zip.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + "  must have zip.");
                if (requireCounty == true && endorsementAddress.County.HasValue() == false & TotalCheckIncrementor(ref totalCount))
                    errorMsg.Add(errorMsgObjectHierarchy + "  must have county.");

                if(isRequired == false && totalCount == errorMsg.Count) //All or nothing, if you have entered info we need to validate based on all criteria. If you have entered nothing and we don't deem it as required, only accept if nothing was entered (thus everything was false).
                {
                    return "";
                }
            }
            else
            {
                if (isRequired)
                {
                    errorMsg.Add(errorMsgObjectHierarchy + " address object must be passed with object.");
                }
            }

            return errorMsg.Count > 0 ? errorMsg[0] : "";
        }

        private static string ValidateAdditionalInterest(List<AdditionalInterest> endorsementAIs, Endorsement.EndorsementObjectType objectType, string parentObjectHierarchyInfo = "", bool isRequired = true)
        {
            string errorMsg = "";
            parentObjectHierarchyInfo = parentObjectHierarchyInfo.IsNotNull() ? parentObjectHierarchyInfo + " " : "";
            string errorMsgObjectHierarchy = "";

            if (objectType == Endorsement.EndorsementObjectType.Lienholder)
            {
                errorMsgObjectHierarchy = parentObjectHierarchyInfo + objectType.ToString();
            }
            else
            {
                errorMsgObjectHierarchy = parentObjectHierarchyInfo + "Additional Interest ";
            }

            if (endorsementAIs.IsLoaded())
            {
                var endorsementAI = endorsementAIs.Last(); //Since there could be more than one. Typically, there will be only one for adding, editing, deleting. However, there is a workflow for adding multiple lienholders so we need to support multiple here. They should come in one at a time. (Add one, rate, add one, rate, finalize)
                if (endorsementAI != null)
                {
                    int totalCount = 0;
                    if (endorsementAI.TypeId.HasValue() == false && TotalCheckIncrementor(ref totalCount))
                        return errorMsgObjectHierarchy + " must have type id.";

                    if (Enum.IsDefined(typeof(QuickQuoteAdditionalInterest.AdditionalInterestType), endorsementAI.TypeId) != true)
                        return errorMsgObjectHierarchy + " must have valid type id.";

                    if (endorsementAI.TypeId == 8)
                        errorMsg = ValidatePersonalName(endorsementAI.Name, objectType, parentObjectHierarchyInfo, isRequired);
                    else
                        errorMsg = ValidateBusinessName(endorsementAI.Name, objectType, parentObjectHierarchyInfo, isRequired);

                    if (errorMsg.HasValue())
                        return errorMsg;

                    if (errorMsg.HasValue() == false)
                        errorMsg = ValidateAddress(endorsementAI.Address, false, objectType, parentObjectHierarchyInfo, isRequired);
                }
                else
                {
                    if (isRequired)
                        errorMsg = errorMsgObjectHierarchy + " additional interest object must be passed with object.";
                }
            }
            else
            {
                if (isRequired)
                    errorMsg = errorMsgObjectHierarchy + " additional interest object must be passed with object.";
            }

            return errorMsg;
        }

        private static bool ValidateMailingAddress(Endorsement endorsement, bool isFinalize = false)
        {
            if (endorsement.Policyholders?.Count > 0)
                switch (endorsement.ActionType)
                {
                    case Endorsement.EndorsementActionType.Edit:
                        if (endorsement.PolicyImageNum <= 0 || (endorsement.PolicyImageNum > 0))
                            endorsement.ErrorMessage = ValidateAddress(endorsement.Policyholders[0].Address, false, Endorsement.EndorsementObjectType.MailingAddress);
                        if (endorsement.ErrorMessage.IsNotNull())
                            return true;
                        else
                            endorsement.ErrorMessage = "Must provide the vehicle num of the vehicle you are adding/editing when working on an existing endorsement.";
                        break;
                    default:
                        endorsement.ErrorMessage = "Mailing Address only supports the 'edit' action.";
                        break;
                }
            else
                endorsement.ErrorMessage = "Must send a vehicle object in order to do an endorsement on a vehicle.";
            return false;
        }

        private static bool ValidateLoanLenderInfo(Endorsement endorsement, bool isFinalize = false)
        {
            switch (endorsement.ActionType)
            {
                case Endorsement.EndorsementActionType.Edit:
                    if (endorsement.PolicyImageNum <= 0 || (endorsement.PolicyImageNum > 0))
                        endorsement.ErrorMessage = ValidateAddress(endorsement.Policyholders[0].Address, false, Endorsement.EndorsementObjectType.MailingAddress);
                    if (endorsement.ErrorMessage.IsNotNull())
                        return true;
                    else
                        endorsement.ErrorMessage = "Must provide the vehicle num of the vehicle you are adding/editing when working on an existing endorsement.";
                    break;
                default:
                    endorsement.ErrorMessage = "Loan Lender Info only supports the 'edit' action.";
                    break;
            }
            return false;
        }

        private static bool ValidatePayPlan(Endorsement endorsement, bool isFinalize = false)
        {
            switch (endorsement.ActionType)
            {
                case Endorsement.EndorsementActionType.Edit:
                    List<int> validPayPlanIds = new List<int>() { 12, 13, 14, 15, 18, 19 };

                    if (validPayPlanIds.Contains(endorsement.BillingInformation.PayPlanId))
                        return true;
                    else
                        endorsement.ErrorMessage = "Must select a valid Pay Plan Id.";
                    break;
                default:
                    endorsement.ErrorMessage = "Pay Plan only supports the 'edit' action.";
                    break;
            }

            return false;
        }

        private static bool TotalCheckIncrementor(ref int count)
        {
            count = count + 1;
            return true;
        }

        private static bool CanVehicleHaveTransportationExpense(Vehicle v)
        {
            List<int> invalidTransExpBodyTypes = new List<int> { 22, 24, 42, 18, 20, 19 }; //22=AntiqueAuto, 24=AntiqueAuto, 42=Motorcycle, 18=MotorHome, 20=OtherTrailer, 19=RecTrailer
            if (invalidTransExpBodyTypes.Contains(v.BodyTypeId))
            {
                return false;
            }
            return true;
        }

        private static bool IsVehicleWithLiabilityOnlySetCorrectly(Vehicle v, ref string errorMessage)
        {
            if(errorMessage.HasValue() == false && v.ComprehensiveDeductibleLimitId.HasValue() == false)
            {
                if(v.HasLoanLeaseCoverage == true)
                {
                    errorMessage = "Auto Loan Lease Coverage not allowed when ComprehensiveDeductibleLimitId is 0.";
                }
                else if (v.CollisionDeductibleLimitId.HasValue())
                {
                    errorMessage = "Collision is not allowed when ComprehensiveDeductibleLimitId is 0";
                }
                else if (v.TowingAndLaborDeductibleLimitId.HasValue())
                {
                    errorMessage = "Towing and Labor is not allowed when ComprehensiveDeductibleLimitId is 0";
                }
                else if (v.TransportationExpenseLimitId.HasValue())
                {
                    errorMessage = "Rental Reimbursement (Transportation Expense) is not allowed when ComprehensiveDeductibleLimitId is 0";
                }
            }
            if (errorMessage.HasValue())
                return false;
            else
                return true;
        }
    }
}
