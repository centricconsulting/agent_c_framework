using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL
{
    public class PolicyLevelValidations
    {
        public const string ValidationListID = "{4A7B83BE-65BC-44FD-BDEC-2A3EE075F5A7}";
        public const string quoteIsNull = "{A83919DA-A002-4BBC-B8C4-CA65895BF33F}";

        public const string notAllSubQuotesHaveLocations = "{89725B9D-8766-4B5F-99E7-D2DD076DD97F}";
        public const string hasLocationNotAvailabletoQuote = "{81041320-36C6-40E3-8D85-FBEFAB81DEFF}";

        public const string notAllSubQuotesHaveGlClassifications = "{61c507c4-502c-4947-8e9e-faf812839acf}";

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
                        if (IFM.VR.Common.Helpers.MultiState.Locations.DoesEachSubQuoteContainALocation(quote) == false)
                        {
                            if (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(quote.EffectiveDate))
                            {
                                string subQuotesWihtoutALocation = String.Join(",", IFM.VR.Common.Helpers.States.GetStateAbbreviationsFromStateIds(IFM.VR.Common.Helpers.MultiState.Locations.SubQuoteStateIdsWithNoLocation(quote)));
                                valList.Add(new ValidationItem($"There are no locations in {subQuotesWihtoutALocation}. Please add the location(s) or remove additional non-governing state(s) on the policyholder page if selected.", notAllSubQuotesHaveLocations));
                            }
                            else
                            {
                                valList.Add(new ValidationItem($"There are no locations.", notAllSubQuotesHaveLocations));
                            }
                        }

                        if (IFM.VR.Common.Helpers.MultiState.Locations.HasIneligibleLocationsForQuote(quote))
                        {
                            string locationStatesThatAreIneligible = String.Join(",", IFM.VR.Common.Helpers.States.GetStateAbbreviationsFromStateIds(IFM.VR.Common.Helpers.MultiState.Locations.IneligibleLocationStateIdsForQuote(quote)));
                            if (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(quote.EffectiveDate))
                            {
                                valList.Add(new ValidationItem($"There are location(s) in {locationStatesThatAreIneligible}. Please remove the location(s) or add additional non-governing state(s) on the policyholder page if available.", hasLocationNotAvailabletoQuote));
                            }
                            else
                            {
                                valList.Add(new ValidationItem($"There are location(s) in {locationStatesThatAreIneligible}. Please remove the location(s).", hasLocationNotAvailabletoQuote));
                            }
                        }

                        if (IFM.VR.Common.Helpers.MultiState.GlClassifications.SubQuoteStateIdsWithNoGlClassifications(quote).Any())
                        {
                            valList.Add(new ValidationItem($"You have entered a multi-state quote but have not entered a classification for each state. Each state must have at least one classification.", notAllSubQuotesHaveGlClassifications));
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