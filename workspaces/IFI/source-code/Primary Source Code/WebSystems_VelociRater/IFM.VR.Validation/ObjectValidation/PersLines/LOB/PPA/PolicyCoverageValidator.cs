using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System.Text.RegularExpressions;
using IFM.PrimativeExtensions;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class PolicyCoverageValidator
    {
        //This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{60BDFAC5-72CE-4F34-BE10-43AA396F1D4E}";
        public const string QuoteIsNull = "{00D05B52-7ADC-4CB9-9085-CC00EDA59A08}";
        public const string UnExpectedLobType = "{236A5DDB-1348-4DD6-830C-2D5037CD1DD5}";
        public const string NoVehicles = "{7DE0D8C7-3F5A-4979-B3E0-104EE4C8F8E3}";

        public const string MinLiab = "{79B7BB2E-F79E-4CBF-91D9-F329DD4C8F07}";
        public const string CSLExceedLimit = "{06484D2F-4F4F-44EE-B953-91695B0E7CB2}";
        public const string CSLExceedLimitStateExpansion = "{CC9EF2BC-0E63-4F0F-A489-D14D2A4C98EE}";
        public const string UMPDDeductRequired = "{88BB3DA7-9DAA-4883-9181-3CE57C23C861}";
        public const string MinBI = "{5D986A80-B656-4EF1-8F1C-E5B656D43EA8}";
        public const string ExceedBI = "{CD6BCA39-2C04-4ADC-96AF-8D2C8FD9331C}";
        public const string ExceedBIStateExpansion = "{12514687-0BFE-46C2-B4CD-F599B1504CAE}";
        public const string UMBIRequired = "{69762642-7B62-4594-941A-8E290C8FCA41}";
        public const string UMPDRequired = "{AFCF56D7-AA17-423D-8C14-A4E5A26E568F}";
        public const string ExceedPD = "{2B76706F-D4E3-4994-9EEF-C11EB1FDDA3B}";
        public const string NotEligibleForAutoEnhance = "{54522B95-B36E-4A34-8D02-D228B450071A}";
        public const string AutoEnhanceRequiresGreaterTransportation = "{766DAA62-4564-44BD-A13D-20AC247B05DE}";
        public const string AutoPlusEnhanceAndAutoEnhanceCantBothBeSelected = "{7A0EF7A0-45AA-4A8C-B7B1-1937B040A044}";
        public const string PropertyLimit = "{95d1331a-b5d3-44c1-b52f-9f1147aded81}"; // - 1-20-2017 Matt A
        public const string PrioBiLimit = "{C65A53AC-B4B3-4C3E-AE30-B995F41DA169}";
        public const string QuotePolicyInfo = "{AC75C62D-4343-4E50-A532-1BAB6BB0E9D3}";

        //Updated 10/8/18 for multi state MLW - added isAutoHomeChecked, homeFarmQuotePolicy
        public static Validation.ObjectValidation.ValidationItemList ValidatePPAPolicyCoverage(QuickQuote.CommonObjects.QuickQuoteObject quote, string ddlLiabType,
            string ddlSingleLimit, string ddlUmUimSSL, string ddlUMCSL, string ddUmPd, string ddUmPdDeductible, string ddUmUmiBi, string ddlUMBI, string ddBodilyInjury, string ddPropertyDamage, ValidationItem.ValidationType valType,
            string hiddenCompOnly, bool isAutoEnhanceChecked, bool isAutoPlusEnhanceChecked, bool isAutoHomeChecked, string homeFarmQuotePolicy)
        {
            QuickQuoteHelperClass qqHelper = new QuickQuoteHelperClass();
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                {
                    switch (valType)
                    {
                        case ValidationItem.ValidationType.appRate:
                        case ValidationItem.ValidationType.quoteRate:
                            //10/4/18 moved from appRate to quoteRate per Matt - MLW
                            if (hiddenCompOnly != "True")
                            {
                                if (ddlLiabType == "1") //Updated 10/4/18 from CSL to 1 - MLW
                                {
                                    //converted if-elese branch to switch case to prepare for pending changes for Ohio
                                    switch (quote.QuickQuoteState)
                                    {
                                        case QuickQuoteHelperClass.QuickQuoteState.Illinois:
                                            if (ddlUMCSL != "0")
                                            {
                                                if (ddlSingleLimit == "0")
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem("UM CSL limit selected is higher than the Single Limit Liability, it must be equal or less.", CSLExceedLimitStateExpansion));
                                                }
                                                else
                                                {
                                                    if (int.Parse(qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, quote.QuickQuoteState, ddlUMCSL).Replace_NullSafe(",", "")) >
                                                        int.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, ddlSingleLimit, quote.LobType).Replace_NullSafe(",", "")))
                                                        valList.Add(new ObjectValidation.ValidationItem("UM CSL limit selected is higher than the Single Limit Liability, it must be equal or less.", CSLExceedLimitStateExpansion));
                                                }
                                            }
                                            break;
                                        //Added 1/17/2022 for OH task 66101 MLW
                                        case QuickQuoteHelperClass.QuickQuoteState.Ohio:
                                            if (ddlUMCSL != "0")
                                            {
                                                if (ddlSingleLimit == "122" || ddlSingleLimit == "0")
                                                    valList.Add(new ObjectValidation.ValidationItem("The minimum limits of Liability for Single Limit Liability must be $75,000 unless UM/UIM coverage is waived", MinLiab));

                                                if (ddlSingleLimit == "0")
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem("UM CSL limit selected is higher than the Single Limit Liability, it must be equal or less.", CSLExceedLimitStateExpansion));
                                                }
                                                else
                                                {
                                                    if (int.Parse(qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, quote.QuickQuoteState, ddlUMCSL).Replace_NullSafe(",", "")) >
                                                        int.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, ddlSingleLimit, quote.LobType).Replace_NullSafe(",", "")))
                                                        valList.Add(new ObjectValidation.ValidationItem("UM CSL limit selected is higher than the Single Limit Liability, it must be equal or less.", CSLExceedLimitStateExpansion));
                                                }
                                            }
                                            break;
                                        default: //states not listed above
                                            if ((ddlSingleLimit == "122" || ddlSingleLimit == "50" || ddlSingleLimit == "0") && ddlUmUimSSL != "0")
                                                valList.Add(new ObjectValidation.ValidationItem("The minimum limits of Liability for Single Limit Liability must be $100,000 unless UM/UIM coverage is waived", MinLiab));

                                            if (ddlUmUimSSL != "0")
                                            {
                                                if (int.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredCombinedSingleLimitId, ddlUmUimSSL).Replace_NullSafe(",", "")) >
                                                    int.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, ddlSingleLimit, quote.LobType).Replace_NullSafe(",", "")))
                                                    valList.Add(new ObjectValidation.ValidationItem("UM/UIM CSL cannot exceed Single Limit Liability limit", CSLExceedLimit));
                                            }

                                            if (ddlUmUimSSL != "0" && ddUmPdDeductible == "0")
                                                valList.Add(new ObjectValidation.ValidationItem("UMPD Deductible is required with UM/UIM CSL Coverage", UMPDDeductRequired));
                                            break;
                                    }
                                }
                                else //SPLIT LIMIT
                                {
                                    //converted if-elese branch to switch case to prepare for pending changes for Ohio
                                    switch (quote.QuickQuoteState)
                                    {
                                        //Updated 1/17/2022 for OH task 66101 MLW
                                        case QuickQuoteHelperClass.QuickQuoteState.Illinois:
                                        case QuickQuoteHelperClass.QuickQuoteState.Ohio:
                                            if (ddlUMBI != "0" && ddBodilyInjury != "0")
                                            {
                                                // 9/4/19 ZTS - modified commented out code to check dropdown values are numeric (Bug 30909)
                                                var strUMBI = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quote.QuickQuoteState, ddlUMBI).Replace_NullSafe(",", "").Replace("/", "");
                                                var strBodilyInjury = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, ddBodilyInjury).Replace_NullSafe(",", "").Replace("/", "");
                                                //if (double.TryParse(strUMBI, out double numUMBI))
                                                //updated 9/10/2019 to work w/ VS2015
                                                double numUMBI;
                                                if (double.TryParse(strUMBI, out numUMBI))
                                                {
                                                    //if (double.TryParse(strBodilyInjury, out double numBodilyInjury))
                                                    //updated 9/10/2019 to work w/ VS2015
                                                    double numBodilyInjury;
                                                    if (double.TryParse(strBodilyInjury, out numBodilyInjury))
                                                    {
                                                        if (numUMBI > numBodilyInjury) valList.Add(new ObjectValidation.ValidationItem("UM BI limit selected is higher than the Bodily Injury Limit, it must be equal or less.", ExceedBIStateExpansion));
                                                    }
                                                    else valList.Add(new ObjectValidation.ValidationItem("Unable to convert Bodily Injury limit of '" + strBodilyInjury + "' to a numeric value.", ExceedBIStateExpansion));
                                                }
                                                else valList.Add(new ObjectValidation.ValidationItem("Unable to convert UM BI limit of '" + strUMBI + "' to a numeric value.", ExceedBIStateExpansion));
                                           
                                            }
                                            break;
                                        default: //states not listed above
                                            if (ddBodilyInjury == "2")
                                                valList.Add(new ObjectValidation.ValidationItem("The minimum limits of Liability for Bodily Injury must be $50,000/$100,000 unless UM/UIM coverage is waived", MinBI));

                                            if (ddUmUmiBi != "0" && ddBodilyInjury != "0")
                                            {
                                                // 9/4/19 ZTS - modified commented out code to check dropdown values are numeric (Bug 30909)
                                                //var strUmUmiBi = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quote.QuickQuoteState, ddlUMBI).Replace_NullSafe(",", "").Replace("/", "");
                                                //corrected static data enum for IN 9/10/2019; was inadvertently updated to use IL enum
                                                var strUmUmiBi = qqHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, quote.QuickQuoteState, ddUmUmiBi).Replace_NullSafe(",", "").Replace("/", "");
                                                var strBodilyInjury = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, ddBodilyInjury).Replace_NullSafe(",", "").Replace("/", "");
                                                //if (double.TryParse(strUmUmiBi, out double numUmUmiBi))
                                                //updated 9/10/2019 to work w/ VS2015
                                                double numUmUmiBi;
                                                if (double.TryParse(strUmUmiBi, out numUmUmiBi))
                                                {
                                                    //if (double.TryParse(strBodilyInjury, out double numBodilyInjury))
                                                    //updated 9/10/2019 to work w/ VS2015
                                                    double numBodilyInjury;
                                                    if (double.TryParse(strBodilyInjury, out numBodilyInjury))
                                                    {
                                                        if (numUmUmiBi > numBodilyInjury) valList.Add(new ObjectValidation.ValidationItem("UM/UIM BI limit cannot exceed Bodily Injury limit", ExceedBIStateExpansion));
                                                    }
                                                    else valList.Add(new ObjectValidation.ValidationItem("Unable to convert Bodily Injury limit of '" + strBodilyInjury + "' to a numeric value.", ExceedBIStateExpansion));
                                                }
                                                else valList.Add(new ObjectValidation.ValidationItem("Unable to convert UM/UIM BI limit of '" + strUmUmiBi + "' to a numeric value.", ExceedBIStateExpansion));
                                                //if (double.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, ddUmUmiBi).Replace_NullSafe(",", "").Replace("/", "")) >
                                                //    double.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, ddBodilyInjury).Replace_NullSafe(",", "").Replace("/", "")))
                                                //    valList.Add(new ObjectValidation.ValidationItem("UM/UIM BI limit cannot exceed Bodily Injury limit", ExceedBI));
                                            }

                                            if (ddUmPd != "0" && ddUmUmiBi == "0")
                                                valList.Add(new ObjectValidation.ValidationItem("UM/UIM BI is required when selecting UMPD coverage", UMBIRequired));

                                            if (ddUmPd == "0" && ddUmUmiBi != "0")
                                                valList.Add(new ObjectValidation.ValidationItem("UMPD is required when UM/UIM BI coverage", UMPDRequired));

                                            if (ddUmPd != "0" && ddUmPdDeductible == "0")
                                                valList.Add(new ObjectValidation.ValidationItem("UMPD Deductible is required with UMPD Coverage", UMPDDeductRequired));

                                            if (ddUmPd != "0")
                                            {
                                                try
                                                {
                                                    if (int.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, ddUmPd).Replace_NullSafe(",", "")) >
                                                        int.Parse(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, ddPropertyDamage).Replace_NullSafe(",", "")))
                                                        valList.Add(new ObjectValidation.ValidationItem("UMPD limit cannot exceed Property Damage limit", ExceedPD));
                                                }
                                                catch (Exception e)
                                                {
                                                }
                                            }

                                            // Added 1-20-2017 BUG 8170 Matt A
                                            if (quote.EffectiveDate.IsDate() && Convert.ToDateTime(quote.EffectiveDate) >= Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["VR_PPA_PropertyLiab_MinChange_StartDate"]))
                                            {

                                                if (ddPropertyDamage.EqualsAny("7", "48")) //7 = 10,000    48 = 15,000
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem("Property Damage must be greater than or equal to 25000 when selected.", PropertyLimit));
                                                }
                                            }
                                            break;
                                    }
                                }

                                if (isAutoPlusEnhanceChecked == true || isAutoEnhanceChecked == true)
                                {
                                    string selectedEnhancement = "";
                                    if (isAutoEnhanceChecked == true && isAutoPlusEnhanceChecked == true)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("You can not select both the Auto Enhancement Endorsement and the Auto Plus Enhancement Endorsement at the same time. Please select only one.", AutoPlusEnhanceAndAutoEnhanceCantBothBeSelected));
                                    }
                                    else
                                    {
                                        if (isAutoEnhanceChecked == true)
                                        {
                                            selectedEnhancement = "Auto Enhancement Endorsement";
                                        }
                                        if (isAutoPlusEnhanceChecked == true)
                                        {
                                            selectedEnhancement = "Auto Plus Enhancement Endorsement";
                                        }
                                    }
                                    bool compAndCollisionOnVehicle = false;
                                    bool transportationTooLow = false;
                                    foreach (QuickQuoteVehicle veh in quote.Vehicles)
                                    {
                                        if (compAndCollisionOnVehicle == false)
                                        {
                                            if (Regex.IsMatch(veh.CollisionDeductibleLimitId, @"^\d+$") == true && Regex.IsMatch(veh.ComprehensiveDeductibleLimitId, @"^\d+$")) //checks if both are numbers
                                            {
                                                if (int.Parse(veh.CollisionDeductibleLimitId) > 0 && int.Parse(veh.ComprehensiveDeductibleLimitId) > 0)
                                                {
                                                    compAndCollisionOnVehicle = true;
                                                }
                                            }
                                        }

                                        if (isAutoPlusEnhanceChecked == true)
                                        {
                                            if (Regex.IsMatch(veh.TransportationExpenseLimitId, @"^\d+$") == true && veh.ComprehensiveDeductibleLimitId != "0")
                                            {
                                                if (int.Parse(veh.TransportationExpenseLimitId) < 2)
                                                {
                                                    //Updated 5/31/18 for Bugs 20129 and 24888 MLW
                                                    //Transportation Expense coverage is not available for Vehicle Types of Motorcycle, Motorhome, Antique Auto, Classic Auto, Other Trailer and Rec Trailer. 
                                                    if (qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, veh.BodyTypeId) == "Rec. Trailer"
                                                        || qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, veh.BodyTypeId) == "Other Trailer"
                                                        || qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, veh.BodyTypeId) == "Motorcycle"
                                                        || qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, veh.BodyTypeId) == "Motor Home"
                                                        || qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, veh.BodyTypeId) == "Antique Auto"
                                                        || qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, veh.BodyTypeId) == "Classic Auto")
                                                    {
                                                        transportationTooLow = false;
                                                    }
                                                    else
                                                    {
                                                        transportationTooLow = true;
                                                    }
                                                    //transportationTooLow = true;
                                                }
                                            }
                                        }
                                    }
                                    if (compAndCollisionOnVehicle == false)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("At least one vehicle must have Comprehensive and Collision coverage in order to be eligible for the " + selectedEnhancement + ".", NotEligibleForAutoEnhance));
                                    }
                                    if (transportationTooLow == true)
                                    {
                                        valList.Add(new ObjectValidation.ValidationItem("The " + selectedEnhancement + " requires that each vehicle have a Transportation Expense of 40/1200 or higher.", AutoEnhanceRequiresGreaterTransportation));
                                    }
                                }

                                if (VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(quote))
                                {
                                    VRGeneralValidations.Val_HasRequiredField_DD(quote.PriorBodilyInjuryLimitId, valList, PrioBiLimit, "Prior BI Limit");
                                }
                            }


                            //Added 10/8/18 for multi state MLW, Updated 1/17/2022 for OH task 66101 MLW
                            if (quote.QuickQuoteState == QuickQuoteHelperClass.QuickQuoteState.Illinois || quote.QuickQuoteState == QuickQuoteHelperClass.QuickQuoteState.Ohio)
                            {
                                if (isAutoHomeChecked == true)
                                {
                                    if (quote.PolicyUnderwritings != null && quote.PolicyUnderwritings.Count > 0)
                                    {
                                        foreach (QuickQuotePolicyUnderwriting item in quote.PolicyUnderwritings)
                                        {
                                            if (item.PolicyUnderwritingCodeId == "9290")
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(item.PolicyUnderwritingExtraAnswer, valList, QuotePolicyInfo, "Quote/Policy Info");

                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    valList.Add(new ObjectValidation.ValidationItem("Invalid LOB type", UnExpectedLobType));
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null", QuoteIsNull));
            }
            return valList;
        }
    }
}