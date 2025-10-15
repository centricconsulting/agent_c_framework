using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.CommLines.LOB.CAP
{
    public class PolicyLevelValidations
    {
        public const string ValidationListID = "{4A7B83BE-65BC-44FD-BDEC-2A3EE075F5A7}";
        public const string quoteIsNull = "{A83919DA-A002-4BBC-B8C4-CA65895BF33F}";

        public const string notAllSubQuotesHaveLocations = "{89725B9D-8766-4B5F-99E7-D2DD076DD97F}";
        public const string hasLocationNotAvailabletoQuote = "{81041320-36C6-40E3-8D85-FBEFAB81DEFF}";

        public static Validation.ObjectValidation.ValidationItemList ValidatePolicyLevel(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                switch (valType)
                {
                    case ValidationItem.ValidationType.issuance:
                    case ValidationItem.ValidationType.appRate:
                    case ValidationItem.ValidationType.quoteRate:
                        if (IFM.VR.Common.Helpers.MultiState.Locations.HasIneligibleLocationsForQuote(quote))
                        {
                            string locationStatesThatAreIneligible = String.Join(",", IFM.VR.Common.Helpers.States.GetStateNamesFromStateIds(IFM.VR.Common.Helpers.MultiState.Locations.IneligibleLocationStateIdsForQuote(quote)));
                            if (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(quote.EffectiveDate))
                            {
                                valList.Add(new ValidationItem($"There are vehicle(s) in {locationStatesThatAreIneligible}. Please remove the vehicle(s) or add additional non-governing state(s) on the policyholder page.", hasLocationNotAvailabletoQuote));
                            }
                            else
                            {
                                valList.Add(new ValidationItem($"There are vehicle(s) in {locationStatesThatAreIneligible}. Please remove the vehicle(s).", hasLocationNotAvailabletoQuote));
                            }
                        }

                        // If the quote has garagekeepers we do NOT need a vehicle Bug 30942 - MGB 1-18-19
                        if (quote.GarageKeepersCollisionManualLimitAmount == "" && quote.GarageKeepersOtherThanCollisionManualLimitAmount == "")
                        {
                            if (!IFM.VR.Common.Helpers.MultiState.Vehicles.HasVehicleForEachSubQuote(quote))
                            {
                                string statesWithNoVehicles = String.Join(",", IFM.VR.Common.Helpers.States.GetStateNamesFromStateIds(IFM.VR.Common.Helpers.MultiState.Vehicles.StateIdsWithOutAGaragedVehicle(quote)));
                                valList.Add(new ValidationItem($"There are no vehicles garaged in {statesWithNoVehicles}. Please add vehicle(s) or remove additional non-governing state(s) on the policyholder page.", notAllSubQuotesHaveLocations));
                            }
                            else
                            {
                                // do you have a location for each sub quote?
                                // should never happen so little effort in the msg details
                                if (IFM.VR.Common.Helpers.MultiState.Locations.DoesEachSubQuoteContainALocation(quote) == false)
                                {
                                    valList.Add(new ValidationItem("Not all states contain a garaged vehicle.", notAllSubQuotesHaveLocations));
                                }
                            }
                        }

                        if (IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(quote) && !IFM.VR.Common.Helpers.MultiState.Vehicles.AreUMPDVehiclesOkay(quote))
                        {
                            //valid only for CAP and IL state quote
                            var UMPDMsg = "At least one vehicle must include UMPD coverage for limit selected.";
                            if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) {

                                UMPDMsg += " If this coverage is no longer desired, please contact your underwriter for assistance.";
                            }
                            valList.Add(new ValidationItem(UMPDMsg));
                        }
                        break;                        
                    default:
                        break;
                }
            }
            else
            {
                valList.Add(new ValidationItem("Quote is null.", quoteIsNull));
            }

            return valList;

        }

    }
}