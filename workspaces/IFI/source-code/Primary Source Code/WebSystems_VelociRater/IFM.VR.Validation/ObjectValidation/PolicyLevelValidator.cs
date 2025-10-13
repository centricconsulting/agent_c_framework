using System;
using Microsoft.VisualBasic;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation
{
    public class PolicyLevelValidator
    {
        public const string ValidationListID = "{941E8C55-E7FA-4BC1-945B-B666C3F72C80}";

        public const string EffectiveDate = "{BA4F9B7E-5C69-40CC-98C6-95EA84F92863}";

        public static Validation.ObjectValidation.ValidationItemList PolicyValidation(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, string originalDate = "")
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            if (quote != null)
            {
                //added 8/1/2019; some taken from below
                string qqoEffDate = quote.EffectiveDate;
                if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    qqoEffDate = quote.TransactionEffectiveDate;
                }

                //if (VRGeneralValidations.Val_IsValidDate(quote.EffectiveDate, valList, EffectiveDate, "Effective Date"))
                //updated 8/1/2019
                if (VRGeneralValidations.Val_IsValidDate(qqoEffDate, valList, EffectiveDate, "Effective Date"))
                {
                    double maximum = QuickQuote.CommonMethods.QuickQuoteHelperClass.MaximumEffectiveDateDaysFromToday();
                    double minimum = QuickQuote.CommonMethods.QuickQuoteHelperClass.MinimumEffectiveDateDaysFromToday();

                    DateTime systemDate = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate();

                    //added 2/22/2019
                    bool effDateValidationIsWarning = false;
                    //string qqoEffDate = quote.EffectiveDate; //removed 8/1/2019; now above
                    if (quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                    {
                        //qqoEffDate = quote.TransactionEffectiveDate; //removed 8/1/2019; now above
                        effDateValidationIsWarning = true;
                        maximum = Common.Helpers.Endorsements.EndorsementHelper.EndorsementDaysForward();
                        minimum = Common.Helpers.Endorsements.EndorsementHelper.EndorsementDaysBack();
                    }

                    //if (VRGeneralValidations.Val_IsDateInRange(quote.EffectiveDate, valList, EffectiveDate, "fieldname", systemDate.AddDays(minimum).ToShortDateString(), systemDate.AddDays(maximum).ToShortDateString(), true) == false)
                    //updated 2/22/2019
                    //if (VRGeneralValidations.Val_IsDateInRange(qqoEffDate, valList, EffectiveDate, "fieldname", systemDate.AddDays(minimum).ToShortDateString(), systemDate.AddDays(maximum).ToShortDateString(), true) == false)
                    //{
                    //    valList.Add(new ValidationItem("Effective date must be within the last " + Math.Abs(minimum) + " days or next " + maximum + " days", EffectiveDate, effDateValidationIsWarning));
                    //}
                    //else
                    //updated 8/1/2019
                    bool effDateOkay = VRGeneralValidations.Val_IsDateInRange(qqoEffDate, valList, EffectiveDate, "fieldname", systemDate.AddDays(minimum).ToShortDateString(), systemDate.AddDays(maximum).ToShortDateString(), true);
                    if (effDateOkay == false)
                    {
                        valList.Add(new ValidationItem("Effective date must be within the last " + Math.Abs(minimum) + " days or next " + maximum + " days", EffectiveDate, effDateValidationIsWarning));
                    }
                    if (effDateOkay || effDateValidationIsWarning)
                    {
                        // need to keep people from rating quotes/apps with an effective date prior to the date below on farm
                        // this should be a swicth no an IF - Matt A 1-20-2017
                        if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                        {
                            if (System.Configuration.ConfigurationManager.AppSettings["VR_Farm_EarliestAllowedEffectiveDate"] != null)
                            {
                                if (DateTime.Parse(quote.EffectiveDate) < DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["VR_Farm_EarliestAllowedEffectiveDate"]))
                                {
                                    valList.Add(new ValidationItem("Effective date must be " + System.Configuration.ConfigurationManager.AppSettings["VR_Farm_EarliestAllowedEffectiveDate"] + " or later for this line of business.", EffectiveDate, false));
                                }
                            }

                            DateTime FarmPollutionUpdatedDDLValuesEffectiveDate;
                            if (System.Configuration.ConfigurationManager.AppSettings["VR_Farm_EarliestAllowedEffectiveDate"] != null && DateTime.TryParse(System.Configuration.ConfigurationManager.AppSettings["VR_FarmPollutionLiabilityUpdate_EffectiveDate"], out FarmPollutionUpdatedDDLValuesEffectiveDate))
                            {
                                //tryparse succesfull
                            }
                            else
                            {
                                //tryparse not succesfull or key is null or the key does not exist
                                FarmPollutionUpdatedDDLValuesEffectiveDate = Convert.ToDateTime("10/01/2016");
                            }
                            if (Convert.ToDateTime(quote.EffectiveDate) < FarmPollutionUpdatedDDLValuesEffectiveDate)
                            {
                                if (quote.Locations != null && quote.Locations.Count > 0)
                                {
                                    foreach (QuickQuote.CommonObjects.QuickQuoteLocation loc in quote.Locations)
                                    {
                                        if (loc.SectionIICoverages != null && loc.SectionIICoverages.Count > 0)
                                        {
                                            foreach (QuickQuote.CommonObjects.QuickQuoteSectionIICoverage sc in loc.SectionIICoverages)
                                            {
                                                switch (sc.CoverageType)
                                                {
                                                    case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement:
                                                    case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability:
                                                        if (sc.IncreasedLimitId == "54" || sc.IncreasedLimitId == "390")
                                                        {
                                                            string FarmPollutionMessage = "";
                                                            sc.IncreasedLimitId = "50";
                                                            sc.TotalLimit = "100,000";
                                                            //Updated 9/18/18 for multi state MLW - quote to SubQuoteFirst
                                                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                                                            var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                                                            if (parts != null)
                                                            {
                                                                var SubQuoteFirst = parts.GetItemAtIndex(0);
                                                                if (SubQuoteFirst != null)
                                                                {                                                                
                                                                    if (SubQuoteFirst.LiabilityOptionId == "1")
                                                                {
                                                                    FarmPollutionMessage += "Limited Farm Pollution - ";
                                                                }
                                                                else if (SubQuoteFirst.LiabilityOptionId == "2")
                                                                {
                                                                    FarmPollutionMessage += "Liability Enhancement Endorsement - ";
                                                                }
                                                                FarmPollutionMessage += "The values '250,000' and '500,000' can only be selected with an effective date on or after " + FarmPollutionUpdatedDDLValuesEffectiveDate.ToShortDateString() + ". The value has been switched to '100,000'";
                                                                valList.Add(new ObjectValidation.ValidationItem(FarmPollutionMessage, EffectiveDate, true));
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            if (valType == ValidationItem.ValidationType.appRate || valType == ValidationItem.ValidationType.issuance)
                            {
                                // if date < 6/1/2016 and this new coverage exists then
                                // valList.Add(new ValidationItem("Effective date must be " + System.Configuration.ConfigurationManager.AppSettings["VR_Farm_NewCoverageEffectiveDate"] + " or later for this new coverage.", EffectiveDate, false));
                                // Can be removed once policies can not be quoted with an effective date earlier than 6/1/2016
                                if(QuickQuote.CommonMethods.QuickQuoteHelperClass.IsValidEffectiveDateForFarmMachinerySpecialCoverageG(quote.EffectiveDate) == false)
                                {
                                    //Updated 9/18/18 for multi state MLW - quote to SubQuoteFirst
                                    QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                                    var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                                    if (parts != null)
                                    {
                                        var SubQuoteFirst = parts.GetItemAtIndex(0);
                                        if (SubQuoteFirst != null)
                                        {
                                            if (SubQuoteFirst.UnscheduledPersonalPropertyCoverage != null)
                                            {
                                                if (SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage == true)
                                                {
                                                    valList.Add(new ValidationItem("Effective date must be " + System.Configuration.ConfigurationManager.AppSettings["VR_FarmMachinerySpecialCoverageG_EffectiveDate"] + " or later for the Farm Machinery - Special Coverage - Coverage G.", EffectiveDate, false));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if(quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        {
                            if(QuickQuote.CommonMethods.QuickQuoteHelperClass.IsValidEffectiveDateForAutoPlusEnhancement(quote.EffectiveDate) == false)
                            {
                                //This is PPA, which is not multi state, does not need updating MLW
                                if(quote.HasAutoPlusEnhancement == true)
                                {
                                    valList.Add(new ValidationItem("Auto Plus Enhancement Endorsement requires the effective date to be on or after " + QuickQuote.CommonMethods.QuickQuoteHelperClass.AutoPlusEnhancement_EffectiveDate() + ".Auto Plus Enhancement Endorsement has been removed.", EffectiveDate, false));
                                }
                            }

                            // Added 1-20-2017 BUG 8170 Matt A
                            if (quote.EffectiveDate.IsDate() && Convert.ToDateTime(quote.EffectiveDate) >= Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["VR_PPA_PropertyLiab_MinChange_StartDate"]))
                            {
                                if (quote.Vehicles.IsLoaded())
                                {
                                    if (quote.Vehicles[0].PropertyDamageLimitId.EqualsAny("7", "48")) //7 = 10,000    48 = 15,000
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("Property Damage must be greater than or equal to 25,000 when selected.", EffectiveDate));
                                    }
                                }
                            }

                        }

                        if(originalDate.NoneAreNullEmptyorWhitespace())
                        {
                            QuickQuote.CommonMethods.QuickQuoteHelperClass qqHelper = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                            string errorMessage = "";
                            if (qqHelper.IsEffectiveDateChangeCrossingUncrossableDateLine(quote, originalDate, quote.EffectiveDate, ref errorMessage) == true)
                            {
                                valList.Add(new ObjectValidation.ValidationItem(errorMessage, EffectiveDate));
                            }

                            if (originalDate.IsDate() && quote.EffectiveDate.IsDate())
                            {
                                var currentEffectiveDate = originalDate.ToDateTime();
                                var newEffectiveDate = quote.EffectiveDate.ToDateTime();
                                if (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(currentEffectiveDate) && IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(newEffectiveDate) == false){
                                    // went from multistate to single state
                                    valList.Add(new ObjectValidation.ValidationItem($"Minimum effective date is {IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate().ToShortDateString()}", EffectiveDate));
                                }
                                if (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(currentEffectiveDate) == false && IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(newEffectiveDate))
                                {
                                    // went from single state to multistate
                                    valList.Add(new ObjectValidation.ValidationItem($"Maximum effective date is {IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate().ToShortDateString()}", EffectiveDate));
                                }

                            }
                        }
                        
                        string OnOrAfterDate = System.Configuration.ConfigurationManager.AppSettings["VR_" + quote.LobType.ToString() + "_StopQuoteRateByDate_DatesOnOrAfter"];
                        string OnOrBeforeDate = System.Configuration.ConfigurationManager.AppSettings["VR_" + quote.LobType.ToString() + "_StopQuoteRateByDate_DatesOnOrBefore"];
                        DateTime myDate;
                        DateTime quoteEffDate;

                        if (quote.EffectiveDate.IsDate() && DateTime.TryParse(quote.EffectiveDate, out quoteEffDate))
                        {
                            if (OnOrAfterDate.NoneAreNullEmptyorWhitespace() && Information.IsDate(OnOrAfterDate))
                            {
                                if (DateTime.TryParse(OnOrAfterDate, out myDate))
                                {
                                    if (quoteEffDate.Date >= myDate.Date)
                                    {
                                        string msg = System.Configuration.ConfigurationManager.AppSettings["VR_" + quote.LobType.ToString() + "_StopQuoteRateByDate_DatesOnOrAfter_ErrorMessage"];
                                        if (msg.NoneAreNullEmptyorWhitespace())
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem(msg, EffectiveDate));
                                        }
                                        else
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("The effective date is currently locked to dates before " + myDate.ToShortDateString() + ". Please select a date before this.", EffectiveDate));
                                        }
                                    }
                                }
                            }

                            if (OnOrBeforeDate.NoneAreNullEmptyorWhitespace() && Information.IsDate(OnOrBeforeDate))
                            {
                                if (DateTime.TryParse(OnOrBeforeDate, out myDate))
                                {
                                    if (quoteEffDate.Date <= myDate.Date)
                                    {
                                        string msg = System.Configuration.ConfigurationManager.AppSettings["VR_" + quote.LobType.ToString() + "_StopQuoteRateByDate_DatesOnOrBefore_ErrorMessage"];
                                        if (msg.NoneAreNullEmptyorWhitespace())
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem(msg, EffectiveDate));
                                        }
                                        else
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("The effective date is currently locked to dates after " + myDate.ToShortDateString() + ". Please select a date after this.", EffectiveDate));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return valList;
        }
    }
}