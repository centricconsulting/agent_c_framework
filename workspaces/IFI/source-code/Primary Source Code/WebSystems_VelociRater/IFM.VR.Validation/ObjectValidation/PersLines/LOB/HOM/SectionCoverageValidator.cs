using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.PrimativeExtensions;
using IFM.VR.Common.Helpers.HOM;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class SectionCoverageValidator
    {
        public const string ValidationListID = "{DD0E2F29-DBD3-4C4C-853C-35E4A3571BDB}";


        public const string QuoteIsNull = "{A4CC2572-07DA-406E-8F22-5800C7388249}";        
        public const string UnExpectedLobType = "{52013457-5977-48BD-9293-6613376EAF6E}";        
        public const string NoLocation = "{D517E098-BA49-412E-9DEC-3C1205868535}";
        public const string NoFormTypeId = "{60561B34-7525-452F-9A0D-5829F88E9B81}";
        public const string CovRequiresYearBuilt = "{BF10E079-7E51-45C1-A796-C47088FFA9FE}";
        public const string CovRequiresCountyName = "{4347D0D1-751F-4B60-941D-033009107123}";

        public const string CoverageNotValidForFormType = "{B7BC2875-EEEA-4CBE-B9D1-2A4EBBDA5DBD}";
        public const string IncludedLimit = "{FFC3B33A-8726-428F-85D3-5DE33347F79D}";
        public const string IncreasedLimit = "{68124D26-3D21-425B-88FB-6E5516A68F31}";
        public const string TotalLimit = "{92B69E60-45E6-4A81-A785-DF0AED9C70DA}";
        public const string Deductible = "{B53CDEE5-CBF8-48DA-897F-8A811100C4EE}";
        public const string EffectiveDate = "{4D824923-C108-4FEA-AE7B-1B6EA7C8CD2A}";
        public const string Description = "{9DB14CED-2FFC-4CC3-B2BC-5167F8C41A5A}";
        public const string ConstructionType = "{1BA39B77-E212-4439-8C1C-64306C48E059}";
        public const string NumberOfFamilies = "{B06DA4AD-9212-480A-88CB-0E8F20FC5E29}";
        public const string MineSubsidence = "{15935B03-4F18-442B-A1E4-7F34FF9E4C33}";
        public const string EventEffDate = "{F4F7F019-7B7A-49AB-B5B5-D9D3CE551033}"; //Added 1/15/18 for HOM Upgrade MLW
        public const string EventExpDate = "{DFD6FEB2-545D-47FB-8E0E-8357067DF54A}"; //Added 1/15/18 for HOM Upgrade MLW
        public const string Name = "{2EC321C0-E82A-4289-9784-FB8E14EFAC96}"; //Added 1/24/18 for HOM Upgrade MLW
        public const string InsuredFirstName = "{0FDD1F5D-29DD-4D0D-A3B9-C2873915EB9E}"; //Added 1/24/18 for HOM Upgrade MLW
        public const string InsuredLastName = "{040CD164-63B5-4B2C-BA95-58B3541102BD}"; //Added 1/24/18 for HOM Upgrade MLW
        public const string InsuredBusinessName = "{8454A3F1-85B6-40D2-B0BA-5CE174921523}"; //Added 1/24/18 for HOM Upgrade MLW
        public const string MaximumAmount = "{3CEA7B43-212B-4A09-BB4A-02840259FA03}"; //Added 1/25/18 for HOM Upgrade MLW
        public const string IncreasedCostOfLoss = "{F8D52811-A77D-43D9-B1C7-CD28C5C19D85}"; //Added 1/25/18 for HOM Upgrade MLW
        public const string BuildingDescription = "{2BD47F08-E90D-4D70-BBD7-7640975D4E02}"; //Added 1/30/18 for HOM Upgrade MLW
        public const string OtherFirstName = "{FC32939D-6DDA-4AE5-AC39-451E3CB4F598}"; //Added 1/31/18 for HOM Upgrade MLW
        public const string OtherLastName = "{7ECF978A-E782-48E7-B880-23DF4A085EA5}"; //Added 1/31/18 for HOM Upgrade MLW
        public const string LiabilityIncludedLimit = "{304D3AD5-D7EF-49CC-8569-0C165731493F}"; //Added 2/6/18 for HOM Upgrade MLW
        public const string ExpenseRelatedCoverageLimit = "{63DAEF9C-7AA0-4380-B8BB-236A28AD78C4}"; //Added 2/13/18 for HOM Upgrade MLW

        public const string AddressStreetNumber = "{72BC8191-B561-43DF-AAEF-ADB31470D265}";
        public const string AddressStreetName = "{F6704878-EEC3-497D-A576-729F34D49445}";
        public const string AddressAptNumber = "{ABE64B45-B270-44B2-A1CE-67ADE5EEEC50}";
        public const string AddressZipCode = "{9858D080-0604-4F9D-A495-255CB07C9377}";
        public const string AddressCity = "{3321DB63-4D95-442A-870A-716D9E71F464}";
        public const string AddressState = "{3EBE3790-7837-4272-98B1-72511AD194C2}";
        public const string AddressSatetNotIndiana = "{E74D5CDD-5CB0-417B-807F-FC6F2CC5A358}";
        public const string AddressCountyID = "{3B1871B6-76BB-4C6E-9E74-5F099978B714}";

        public static string GetHomeVersion(QuickQuote.CommonObjects.QuickQuoteObject quote)
        {
            QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
            DateTime effectiveDate = DateTime.Today;
            string eDate = "";
            string HomeVersion = "";
            if (quote != null)
            {
                if (quote.EffectiveDate != null && quote.EffectiveDate != "")
                {
                    effectiveDate = Convert.ToDateTime(quote.EffectiveDate);
                }
                eDate = Convert.ToString(effectiveDate);
                if (qqh.doUseNewVersionOfLOB(quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, Convert.ToDateTime("7/1/2018")))
                {
                    HomeVersion = "After20180701";
                }
                else
                {
                    HomeVersion = "Before20180701";
                }
            }
            return HomeVersion;
        }

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMSectionCoverage(QuickQuote.CommonObjects.QuickQuoteObject quote, IFM.VR.Common.Helpers.HOM.SectionCoverage sectionCoverage, int coverageIndex,  ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            string invalidCoverageForFormTypeMessage = "Coverage not valid for current form type.";
            bool validateAddress = false;

            if (quote != null)
            {
                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                {
                    if (quote.Locations != null && quote.Locations.Any() && quote.Locations[0] != null)
                    {
                        QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();
                        var MyLocation = quote.Locations[0];
                        int formTypeId = 0;
                        int.TryParse(MyLocation.FormTypeId, out formTypeId);
                        int effectiveDateYear = (quote.EffectiveDate.IsDate()) ? Convert.ToDateTime(quote.EffectiveDate).Year : DateTime.Now.Year;

                        //added 11/28/17 for HOM Upgrade MLW
                        string CurrentForm = QQHelper.GetShortFormName(quote);
                        string HomeVersion = GetHomeVersion(quote);

                        //updated 11/20/17 for HOM Upgrade MLW
                        if (formTypeId.EqualsAny(1, 2, 3, 4, 5, 6, 7, 22, 23, 24, 25, 26))
                        {
                            int yearBuilt = 0;
                            Int32.TryParse(MyLocation.YearBuilt, out yearBuilt);

                            if (yearBuilt > 0)
                            {
                                if (MyLocation.Address.County.IsNullEmptyorWhitespace() == false)
                                {

                                    bool isMineCounty = IFM.VR.Common.Helpers.MineSubsidenceHelper.LocationAllowsMineSubsidence(MyLocation);


                                    //switch (valType)
                                    //{
                                    //    case ValidationItem.ValidationType.quoteRate:
                                    //        break;
                                    //}


                                    if (sectionCoverage.SectionCoverageIEnum != QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None)
                                    {
                                        switch (sectionCoverage.SectionCoverageIEnum)
                                        {

                                            #region Included Coverages                                   
                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased:
                                            //    //CoverageName = "Business Property Increased Limits (HO-312)"

                                            //    //updated 11/28/17 for HOM Upgrade MLW
                                            //    //if (formTypeId.NotEqualsAny(6, 7))
                                            //    if (CurrentForm.NotEqualsAny("ML-2", "ML-4"))
                                            //    {
                                            //        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "2500", "2500");
                                            //        // requires an increase limit ??
                                            //    }
                                            //    else
                                            //    {
                                            //        // this coverage is not available for this form
                                            //        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            //    }
                                            //    break;

                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms:
                                            //    //CoverageName = "Firearms (HO-65 / HO-221)"

                                            //    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                            //    {
                                            //        //updated 11/28/17 for HOM Upgrade MLW
                                            //        //if (formTypeId.EqualsAny(6, 7))
                                            //        if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "500", "500");
                                            //        else
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "2000", "2000");
                                            //    }

                                            //    break;

                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs:
                                            //    //CoverageName = "Jewelry, Watches & Furs (HO-61)"

                                            //    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                            //    {
                                            //        //updated 11/28/17 for HOM Upgrade MLW
                                            //        //if (formTypeId.EqualsAny(6, 7))
                                            //        if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "500", "500");
                                            //        else
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "1000", "1000");
                                            //    }
                                            //    break;

                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money:
                                            //    //CoverageName = "Money (HO-65 / HO-221)"

                                            //    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                            //    {
                                            //        //updated 11/28/17 for HOM Upgrade MLW
                                            //        //if (formTypeId.EqualsAny(6, 7))
                                            //        if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "100", "100");
                                            //        else
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "200", "200");
                                            //    }
                                            //    break;

                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities:
                                            //    //CoverageName = "Securities (HO-65 / HO-221)"
                                            //    break;

                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware:
                                            //    //CoverageName = "Silverware, Goldware, Pewterware (HO-61)"
                                            //    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                            //    {

                                            //        //updated 11/28/17 for HOM Upgrade MLW MLW
                                            //        //if (formTypeId.EqualsAny(6, 7))
                                            //        if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "1000", "1000");
                                            //        else
                                            //            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "2500", "2500");
                                            //    }
                                            //    break;

                                            #endregion

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage:
                                                //CoverageName = "Equipment Breakdown Coverage (92-132)" - Old forms
                                                //CoverageName = "Equipment Breakdown Coverage (HOM 1011)" - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(1, 4, 6, 7))
                                                if (CurrentForm.EqualsAny("HO-2", "HO-4", "ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;


                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement:
                                                //CoverageName = "Personal Property Replacement Cost  (HO-290 / 92-195)" - Old forms
                                                // if ML2or4 then name is "Personal Property Replacement Cost  (ML-55)" - Old forms
                                                //CoverageName = "Personal Property Replacement Cost (HO 0490)" - New forms
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement:
                                                //CoverageName = "Homeowner Enhancement Endorsement  (92-267)" - Old forms
                                                //CoverageName = "Homeowner Enhancement Endorsement  (HOM 1010)" - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(6, 7))
                                                if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains:
                                                //CoverageName = "Backup of Sewer or Drain (92-173)" - Old forms
                                                //CoverageName = "Water Backup (N/A)" - New forms

                                                //Added 2/26/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        //VRGeneralValidations.Val_HasIneligibleField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"); // must always be nothing - the quote/print summaries will say 5,000 but we want to send nothing to diamond because it adds it automatically
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "5000", "5000");
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(6, 7))
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasIneligibleField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"); // must always be nothing - the quote/print summaries will say 5,000 but we want to send nothing to diamond because it adds it automatically
                                                    }
                                                    // has an increased limit but it is not required
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement:
                                                //CoverageName = "Homeowner Plus Enhancement Endorsement  (HOM 1017)" - New forms
                                                //Added 1/16/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                } else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage:
                                                //CoverageName = "Water Damage (N/A)" - New forms
                                                //Added 1/16/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        //VRGeneralValidations.Val_HasIneligibleField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"); // must always be nothing - the quote/print summaries will say 5,000 but we want to send nothing to diamond because it adds it automatically
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "5000", "5000");
                                                    }
                                                } 
                                                else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                // has an increased limit but it is not required
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage:
                                                //CoverageName = "Cov.A - Specified Additional Amount Of Insurance (29-034)" - Old forms
                                                //CoverageName = "Cov.A - Specified Additional Amount Of Insurance (HO 0420)" - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake:
                                                //CoverageName = "Earthquake  (HO-315B)" - Old forms
                                                //CoverageName = "Earthquake  (ML-54)" - Old forms
                                                //CoverageName = "Earthquake  ((HOM 1014)" - New forms
                                                if (VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.DeductibleId, valList, Deductible, "Deductible"))
                                                {

                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.NotEqualsAny(6, 7))
                                                    if (CurrentForm.NotEqualsAny("ML-2", "ML-4"))
                                                    {
                                                        string TwoPercentId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, "2%", quote.LobType);
                                                        // if not ML then 2% is not a valid value
                                                        if (sectionCoverage.DeductibleId == TwoPercentId)
                                                            valList.Add(new ObjectValidation.ValidationItem("Invalid Deductible", Deductible));
                                                    }
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement:
                                                //CoverageName = "Actual Cash Value Loss Settlement (HO-04 81)" - Old forms
                                                //CoverageName = "Actual Cash Value Loss Settlement (HO 0481) " - New forms

                                                //updated 12/28/17 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing:
                                                //CoverageName = "Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing (HO-04 93)" - Old forms
                                                //CoverageName = "Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing (HO 1013)" - New forms

                                                //updated 12/28/17 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(4, 6, 7))
                                                    if (CurrentForm.EqualsAny("HO-4", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                break;

                                            //coverage added 1/15/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BroadenedResidencePremisesDefinition:
                                                //CoverageName = "Broadened Residence Premises Definition"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    //if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.EventEffDate, valList, EventEffDate, "Inception Date"))
                                                    if (sectionCoverage.EventEffDate == "")
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem("Inception Date Required", EventEffDate));
                                                    }
                                                    else
                                                    {
                                                        if (sectionCoverage.EventEffDate.Length < 10)
                                                        {
                                                            valList.Add(new ObjectValidation.ValidationItem("Invalid Inception Date", EventEffDate));
                                                        }
                                                        else
                                                        {
                                                            if (quote.EffectiveDate.IsDate())
                                                                VRGeneralValidations.Val_IsDateInRange(sectionCoverage.EventEffDate, valList, EventEffDate, "Inception Date", quote.EffectiveDate, DateTime.Parse(quote.EffectiveDate).AddYears(1).ToShortDateString());
                                                        }
                                                    }
                                                    //VRGeneralValidations.Val_HasRequiredField(sectionCoverage.EventExpDate, valList, EventExpDate, "Termination Date");
                                                    //if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.EventExpDate, valList, EventExpDate, "Termination Date"))
                                                    if (sectionCoverage.EventExpDate == "")
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem("Termination Date Required", EventExpDate));
                                                    }
                                                    else
                                                    {
                                                        if (sectionCoverage.EventExpDate.Length < 10)
                                                        {
                                                            valList.Add(new ObjectValidation.ValidationItem("Invalid Termination Date", EventExpDate));
                                                        }
                                                        else
                                                        {
                                                            if(sectionCoverage.EventEffDate.IsDate() && sectionCoverage.EventExpDate.IsDate())
                                                            {
                                                                if (sectionCoverage.EventEffDate.ToDateTime() > sectionCoverage.EventExpDate.ToDateTime())
                                                                {
                                                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Termination Date", EventExpDate));
                                                                }
                                                                else
                                                                {
                                                                    if (quote.EffectiveDate.IsDate())
                                                                        VRGeneralValidations.Val_IsDateInRange(sectionCoverage.EventExpDate, valList, EventExpDate, "Termination Date", quote.EffectiveDate, DateTime.Parse(quote.EffectiveDate).AddYears(1).ToShortDateString());
                                                                }
                                                            } else
                                                            {
                                                                if (!sectionCoverage.EventExpDate.IsDate())
                                                                {
                                                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Termination Date", EventExpDate));
                                                                }
                                                            }
                                                            
                                                            
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            //coverage added 1/9/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CovBOtherStructuresAwayFromTheResidencePremises:
                                                //CoverageName = "Cov B Other Structures Away from the Residence Premises"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //new field not valid on old form types
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse:
                                                //CoverageName = "Sinkhole Collapse (HO-99)" - Old forms
                                                //CoverageName = "Sinkhole Collapse (HO 0499)" - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc:
                                                //CoverageName = "Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53)" - Old forms
                                                ///CoverageName = "Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO 0453)" - New forms

                                                //updated 11/28/17 for HOM Upgrade - switched back since coverage not valid for old mobile form types only - MLW
                                                if (formTypeId.EqualsAny(6, 7))
                                                //if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                        VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "2500", "2500");
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment:
                                                //CoverageName = "Functional Replacement Cost Loss Settlement (HO-05 30)" - Old forms
                                                //CoverageName = "Functional Replacement Cost Loss Settlement (HO 0530)" - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    // available for this form but only if the build year of the location is >= 1947
                                                    if (yearBuilt > 1947)
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem("Coverage requires structure to be built prior to 1948.", CoverageNotValidForFormType));
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations:
                                                //CoverageName = "Building Additions and Alterations (HO-51)"
                                                //updated 11/20/17 for HOM Upgrade MLW
                                                if (formTypeId.EqualsAny(1, 2, 3, 4, 5, 6, 7, 22, 23, 24, 25, 26))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge:
                                                //CoverageName = "Fire Department Service Charge (ML-306)"

                                                //Updated 12/27/17 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    //VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit must be 100 or more", "100", Int32.MaxValue.ToString());
                                                    if (sectionCoverage.IncreasedLimit != "")
                                                        VRGeneralValidations.Val_IsNonNegativeWholeNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit");
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(1, 2, 3, 4, 5))
                                                    if (CurrentForm.EqualsAny("HO-2", "HO-3", "HO-4", "HO-6"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit must be 100 or more", "100", Int32.MaxValue.ToString());
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment:
                                                //CoverageName = "Loss Assessment (HO-35)" - Old forms

                                                //Updated 1/22/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    //Moved to Section I & II
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(6, 7))
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "1000", "1000");

                                                        //Updated 12/5/17 for HOM Upgrade MLW
                                                        //if (formTypeId.NotEqualsAny(5)) // the increased limit is optional on HO-6 - Matt A 9-30-2016 Bug 7701
                                                        if (CurrentForm.NotEqualsAny("HO-6")) // the increased limit is optional on HO-6 - Matt A 9-30-2016 Bug 7701
                                                        {
                                                            VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.IncreasedLimitId, valList, IncreasedLimit, "Increased Limit");
                                                        }

                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake:
                                                //CoverageName = "Loss Assessment - Earthquake (HO-35B)" - Old forms
                                                //CoverageName = "Loss Assessment - Earthquake (HO 0436)" - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(6, 7))
                                                if (CurrentForm.EqualsAny("ML-2","ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    //Updated 12/27/17 for HOM Upgrade MLW
                                                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                    {
                                                        //Removed included limit dd for HOM Upgrade MLW - added increase limit range - 3/8/18 MLW
                                                        //VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.IncreasedLimitId, valList, IncreasedLimit, "Increased Limit");
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit", "1", "100000");
                                                    }
                                                    else
                                                    {
                                                        
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");
                                                    }
                                                }
                                                break;

                                            //Added 1/25/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades:
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    } 
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, MaximumAmount, "Maximum Amount");
                                                        if(sectionCoverage.IncreasedCostOfLoss.IsNullEmptyorWhitespace() && (sectionCoverage.IncreasedCostOfLossId.IsNullEmptyorWhitespace() || (sectionCoverage.IncreasedCostOfLossId.IsNumeric() && Decimal.Parse(sectionCoverage.IncreasedCostOfLossId) < 1)))
                                                        {
                                                            VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedCostOfLoss, valList, IncreasedCostOfLoss, "Increased Cost of Loss");
                                                        }
                                                        //bool expRelCov = (bool)HttpContext.Current.Session["ExpenseRelatedCoverage"];
                                                        //if (expRelCov)
                                                        //{
                                                        //    VRGeneralValidations.Val_HasRequiredField(sectionCoverage.ExpRelCoverageLimit, valList, ExpenseRelatedCoverageLimit, "Limit");
                                                        //    HttpContext.Current.Session["ExpenseRelatedCoverage"] = false;
                                                        //}
                                                    }
                                                }
                                                else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA:
                                                //CoverageName = "Mine Subsidence Cov A (92-074)" - Old forms
                                                //CoverageName = "Mine Subsidence Cov A (HOM 1009)" - New Forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(4, 6, 7))
                                                if (CurrentForm.EqualsAny("HO-4", "ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    // available but on in specific counties
                                                    if (!isMineCounty)
                                                        valList.Add(new ObjectValidation.ValidationItem("Mine subsidence is not available in the county of this location.", MineSubsidence));
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB:
                                                //CoverageName = "Mine Subsidence Cov A & B (92-074)" - Old forms
                                                //CoverageName = "Mine Subsidence Cov A & B (HOM 1009)" - New forms

                                                //Updated 1/3/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    // available but on in specific counties
                                                    if (!isMineCounty)
                                                        valList.Add(new ObjectValidation.ValidationItem("Mine subsidence is not available in the county of this location.", MineSubsidence));
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        // available but on in specific counties
                                                        if (!isMineCounty)
                                                            valList.Add(new ObjectValidation.ValidationItem("Mine subsidence is not available in the county of this location.", MineSubsidence));
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures:
                                                //CoverageName = "Specified Other Structures - On Premises (92-049)" - Old forms
                                                
                                                //Updated 1/3/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    //Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                                    //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    if (sectionCoverage.IncreasedLimit == "")
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem("Limit Required", IncreasedLimit));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");
                                                    }
                                                    VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(4, 5))
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        // requires limit(increased Limit),Description,Construction Type
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                        {
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");
                                                        }

                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                        VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.ConstructionTypeId, valList, ConstructionType, "Construction Type");

                                                    }
                                                }
                                                break;
                                            //Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW - MOVED TO ABOVE
                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises:
                                            //    //CoverageName = "Other Structures On the Residence Premises (HO 0448)" - New forms

                                            //    //Updated 1/3/18 for HOM Upgrade MLW
                                            //    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                            //    {
                                            //        if (sectionCoverage.IncreasedLimit == "")
                                            //        {
                                            //            valList.Add(new ObjectValidation.ValidationItem("Limit Required", IncreasedLimit));
                                            //        }
                                            //        else
                                            //        {
                                            //            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");
                                            //        }
                                            //        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            //    }
                                            //    else
                                            //    {
                                            //        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            //    }
                                            //    break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises:
                                                //CoverageName = "Specified Other Structures - Off Premises (92-147)"
                                                //CoverageName = "Specific Structures Away from Residence Premises (HO 0492) - New forms

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(6, 7))
                                                if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    // requires limit(increased Limit),Description,Construction Type, Address
                                                    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                    {
                                                        VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");
                                                    }

                                                    VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    //Updated 1/30/18 for HOM Upgrade MLW
                                                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                    {
                                                        //construction type not used on new forms
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.ConstructionTypeId, valList, ConstructionType, "Construction Type");
                                                    }
                                                    validateAddress = true;

                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities:
                                                //CoverageName = "Personal Property Self Storage Facilities (HO 0614)"

                                                //Added 1/19/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (sectionCoverage.IncreasedLimit != "")
                                                        VRGeneralValidations.Val_IsNonNegativeWholeNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit");
                                                } else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            //added 11/20/17 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ReplacementCostForNonBuildingStructures:
                                                //CoverageName = "Replacement Cost for Non-Building Structures (HO 0443)"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //new field not valid on old form types
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            //added 1/18/17 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialPersonalProperty:
                                                //CoverageName = "Special Personal Property Coverage (HO 0524)"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.NotEqualsAny("HO-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //new field not valid on old form types
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            

                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial:
                                                //CoverageName = "Theft of Building Materials (92-367)" - Old forms
                                                //CoverageName = "Theft of Building Materials (HOM 1002)" - New forms

                                                //updated 11/28/17 and 12/26/17 for HOM Upgrade MLW
                                                //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                if (CurrentForm.EqualsAny("HO-4", "ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    //Updated 12/26/17 for HOM Upgrade MLW
                                                    if (formTypeId == 5)
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        if (yearBuilt < effectiveDateYear)
                                                        {
                                                            valList.Add(new ObjectValidation.ValidationItem("Coverage requires structure to be built in current year.", CoverageNotValidForFormType));
                                                        }
                                                        else
                                                        {                                                        
                                                            if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                                VRGeneralValidations.Val_IsNonNegativeWholeNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");                                                      
                                                        }
                                                    }
                                                    
                                                }
                                                break;

                                            //added 1/22/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftOfPersonalPropertyInDwellingUnderConstruction:
                                                //CoverageName = "Theft of Personal Property in Dwelling Under Construction (HO 0607)"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        //need validation for BeginDate, EndDate, and Limit for home upgrade
                                                        //if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.EventEffDate, valList, EventEffDate, "Begin Date"))
                                                        if (sectionCoverage.EventEffDate == "")
                                                        {
                                                            valList.Add(new ObjectValidation.ValidationItem("Begin Date Required", EventEffDate));
                                                        }
                                                        else
                                                        {
                                                            if (sectionCoverage.EventEffDate.Length < 10)
                                                            {
                                                                valList.Add(new ObjectValidation.ValidationItem("Invalid Begin Date", EventEffDate));
                                                            }
                                                            else
                                                            {
                                                                if (quote.EffectiveDate.IsDate())
                                                                    VRGeneralValidations.Val_IsDateInRange(sectionCoverage.EventEffDate, valList, EventEffDate, "Begin Date", quote.EffectiveDate, DateTime.Parse(quote.EffectiveDate).AddYears(1).ToShortDateString());
                                                            }
                                                        }
                                                        //if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.EventExpDate, valList, EventExpDate, "End Date"))
                                                        if (sectionCoverage.EventExpDate == "")
                                                        {
                                                            valList.Add(new ObjectValidation.ValidationItem("End Date Required", EventExpDate));
                                                        }
                                                        else
                                                        {
                                                            if (sectionCoverage.EventExpDate.Length < 10)
                                                            {
                                                                valList.Add(new ObjectValidation.ValidationItem("Invalid End Date", EventExpDate));
                                                            }
                                                            else
                                                            {
                                                                if (sectionCoverage.EventEffDate.IsDate() && sectionCoverage.EventExpDate.IsDate())
                                                                {
                                                                    if (sectionCoverage.EventEffDate.ToDateTime() > sectionCoverage.EventExpDate.ToDateTime())
                                                                    {
                                                                        valList.Add(new ObjectValidation.ValidationItem("Invalid End Date", EventExpDate));
                                                                    }
                                                                    else
                                                                    {
                                                                        if (quote.EffectiveDate.IsDate())
                                                                            VRGeneralValidations.Val_IsDateInRange(sectionCoverage.EventExpDate, valList, EventExpDate, "End Date", quote.EffectiveDate, DateTime.Parse(quote.EffectiveDate).AddYears(1).ToShortDateString());
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (!sectionCoverage.EventExpDate.IsDate())
                                                                    {
                                                                        valList.Add(new ObjectValidation.ValidationItem("Invalid End Date", EventExpDate));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        //VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");
                                                        if (sectionCoverage.IncreasedLimit == "")
                                                        {
                                                            valList.Add(new ObjectValidation.ValidationItem("Limit Required", IncreasedLimit));
                                                        }
                                                    }
                                                
                                                }
                                                else
                                                {
                                                    //not valid on old form types
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;



                                            //added 1/9/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine:
                                                //CoverageName = "Underground Service Line Coverage (HOM 1016)"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //not valid on old form types
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;



                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Consent_to_Move_Mobile_Home:
                                                //CoverageName = "Consent to Move Mobile Home (ML-25)"

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.NotEqualsAny(6, 7))
                                                if (CurrentForm.NotEqualsAny("ML-2", "ML-4"))
                                                {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.EffectiveDate, valList, EffectiveDate, "Effective Date"))
                                                        {
                                                            if (quote.EffectiveDate.IsDate())
                                                                VRGeneralValidations.Val_IsDateInRange(sectionCoverage.EffectiveDate, valList, EffectiveDate, "Effective Date", quote.EffectiveDate, DateTime.Parse(quote.EffectiveDate).AddYears(1).ToShortDateString());
                                                        }
                                                    }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval:
                                                //CoverageName = "Debris Removal (92-267)"
                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                {
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit");
                                                }

                                                VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.IncreasedLimitId, valList, IncreasedLimit, "Increased Limit");

                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit"))
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit");

                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles:
                                                //CoverageName = "Increased Limits Motorized Vehicles (ML-65)"

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.NotEqualsAny(6, 7))
                                                if (CurrentForm.NotEqualsAny( "ML-2", "ML-4"))
                                                {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                        {
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit");
                                                        }
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit"))
                                                            VRGeneralValidations.Val_IsNonNegativeWholeNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit");

                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit"))
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit");
                                                    }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw:
                                                //CoverageName = "Ordinance or Law (HOM 1000)"
                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                {
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit");
                                                }
                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit"))
                                                    VRGeneralValidations.Val_IsNonNegativeWholeNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit");

                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit"))
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit");
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit:
                                                //CoverageName = "Personal Property - Other Residence (HO-50)" 'prior to 2018 Home Upgrade
                                                //CoverageName = "Personal Property - Other Residences (HO 0450)" 'after 2018 Home Upgrade
                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                {
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit");
                                                }
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Increased Limit", true))
                                                    {
                                                        validateAddress = true;
                                                    }
                                                    
                                                }                                                   
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty:
                                                //CoverageName = "Refrigerated Food Products (92-267)"
                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                {
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit");
                                                }

                                                VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.IncreasedLimitId, valList, IncreasedLimit, "Increased Limit");

                                                if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit"))
                                                    VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.TotalLimit, valList, TotalLimit, "Total Limit");
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage:
                                                //CoverageName = "Special Computer Coverage (HO-314)" - Old forms
                                                //CoverageName = "Special Computer Coverage (HO 0414)" - New forms

                                                //Updated 1/4/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-3", "ML-2", "ML-4"))
                                                    {
                                                        if (formTypeId != 2 && formTypeId != 23) //since formTypeIds 2, 3, 23, and 24 all return HO-3 for the CurrentForm and we do not want 2 and 23 - 11/28/17 MLW
                                                            valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(3, 5, 6, 7))
                                                    //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    if (CurrentForm.EqualsAny("HO-3", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        if (formTypeId != 2 && formTypeId != 23) //since formTypeIds 2, 3, 23, and 24 all return HO-3 for the CurrentForm and we do not want 2 and 23 - 11/28/17 MLW
                                                            valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                } 
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TripCollision:
                                                //CoverageName = "Trip Collision (ML-26)"

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.NotEqualsAny(6, 7))
                                                if (CurrentForm.NotEqualsAny("ML-2", "ML-4"))
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageA:
                                                // CoverageName = "Unit Owners Coverage A - Special Coverage (HO 1732)"

                                                //updated 11/28/17 for HOM Upgrade MLW
                                                //if (formTypeId.NotEqualsAny(5))
                                                if (CurrentForm.NotEqualsAny("HO-6"))
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MobileHomeLienholdersSingleInterest:
                                                //CoverageName = "Vendor's Single Interest (ML-27)"
                                                break;


                                            default:
                                                break;

                                        } // END of SectionI switch

                                    } // END is a sectionI

                                    if (sectionCoverage.SectionCoverageIIEnum != QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None)
                                    {
                                        switch (sectionCoverage.SectionCoverageIIEnum)
                                        {
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType._3Or4FamilyLiability:
                                                //CoverageName = "3 or 4 Family Liability (HO-74)";
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability:
                                                //CoverageName = "Incidental Farming Personal Liability (HO-72)" - Old forms
                                                //CoverageName = "Incidental Farming Personal Liability - On Premises (HO 2472)" - New forms

                                                //Updated 1/5/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises:
                                                //CoverageName = "Incidental Farming Personal Liability - Off Premises (HO 2472)" - New forms

                                                //Added 1/17/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                        validateAddress = true;
                                                    }
                                                }
                                                else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres:
                                                //CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres (HO-73)" - Old forms
                                                //CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres (HO-2446)" - New forms

                                                //Updated 1/11/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                        validateAddress = true;
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                        validateAddress = true;
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury:
                                                //CoverageName = "Personal Injury (HO-82)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther:
                                                // CoverageName = "Additional Residence - Rented to Others (HO-70)" - Old forms
                                                //CoverageName = "Additional Residence - Rented to Others (HO 2470)" - New forms

                                                //Updated 1/5/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    validateAddress = true;
                                                    //Added 2/23/2022 for task 64829 MLW
                                                    bool isNew = true;
                                                    if (IFM.VR.Common.Helpers.Endorsements.EndorsementHelper.AdditionalResRentedToOthers_Task64829FeatureFlag() && quote.QuoteTransactionType == QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                                    {
                                                        isNew = IsSectionIICoverageNewToImage(isNew, quote, sectionCoverage);
                                                    }
                                                    //Updated 2/23/2022 for task 64829 MLW
                                                    if (isNew)
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    } else { 
                                                        validateAddress = false;
                                                    }
                                                    //VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    if (VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.NumberOfFamilies, valList, NumberOfFamilies, "Number of Families"))
                                                    {
                                                        VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.NumberOfFamilies, valList, NumberOfFamilies, "Number of Families", "1", "1"); // must be one always
                                                    }
                                                    //validateAddress = true; //2/23/2022 moved above
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade MLW
                                                    //if (formTypeId.EqualsAny(6, 7))
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                        if (VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.NumberOfFamilies, valList, NumberOfFamilies, "Number of Families"))
                                                        {
                                                            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.NumberOfFamilies, valList, NumberOfFamilies, "Number of Families", "1", "1"); // must be one always
                                                        }
                                                        validateAddress = true;
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured:
                                                //CoverageName = "Additional Residence – Occupied by Insured (N/A)"
                                                // CoverageName = "Additional Residence - Occupied by Insured (ML-67)" if a ML FormType      

                                                //updated 11/28/17 for HOM Upgrade           
                                                //if (formTypeId.EqualsAny(6, 7))
                                                if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.NumberOfFamilies, valList, NumberOfFamilies, "Number of Families"))
                                                        VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.NumberOfFamilies, valList, NumberOfFamilies, "Number of Families", "1", "1"); // must be one always

                                                }
                                                validateAddress = true;
                                                //Added 2/24/2022 for task 62956 MLW
                                                if (IFM.VR.Common.Helpers.Endorsements.EndorsementHelper.OtherInsuredLoc_Task62956FeatureFlag() && quote.QuoteTransactionType == QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                                {
                                                    if (IsSectionIICoverageNewToImage(validateAddress, quote, sectionCoverage) == false)
                                                    {
                                                        validateAddress = false;
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical:
                                                //CoverageName = "Business Pursuits - Clerical (HO-71)" - Old Forms
                                                //CoverageName = "Business Pursuits - Clerical (HO 2471)" - New forms
                                                //Added 1/24/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Name.FirstName, valList, InsuredFirstName, "Insured First Name");
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Name.LastName, valList, InsuredLastName, "Insured Last Name");
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, InsuredBusinessName, "Name of Business");

                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_ExcludingInstallation:
                                                //CoverageName = "Sales Person Excluding Installation (HO-71)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_IncludingInstallation:
                                                // CoverageName = "Sales Person Including Installation (HO-71)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment:
                                                //CoverageName = "Teacher Lab Etc. Excluding Corporal Punishment (HO-71)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment:
                                                //CoverageName = "Teacher Lab Etc. Including Corporal Punishment (HO-71)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment:
                                                //CoverageName = "Teacher Other Excluding Corporal Punishment (HO-71)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment:
                                                //CoverageName = "Teacher Other Including Corporal Punishment (HO-71)"
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion:
                                                //Added 1/24/18 for HOM Upgrade MLW
                                                //CoverageName = "Canine Liability Exclusion (HO 2477)"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Name.FirstName, valList, Name, "Canine Name");
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Canine Description");
                                                    }
                                                }
                                                else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured160_500Acres:
                                                //CoverageName = "Farm Owned and Operated By Insured: 160-500 Acres (HO-73)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsuredOver500Acres:
                                                //CoverageName = "Farm Owned and Operated By Insured: Over 500 Acres (HO-73)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.HomeDayCareLiability:
                                                //CoverageName = "Home Day Care (HO-323)"
                                                break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.LowPowerRecreationalMotorVehicleLiability:
                                                //Added 1/17/18 for HOM Upgrade MLW
                                                //CoverageName = "Low Power Recreational Motor Vehicle Liability  (HO 2413)"
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                    break;

                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises:
                                                //CoverageName = "Permitted Incidental Occupancies - Residence Premises (HO-42)"
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence:
                                                //CoverageName = "Permitted Incidental Occupancies Other Residence (HO-43)" - Old forms
                                                //CoverageName = "Permitted Incidental Occupancies Other Residence (HO 2443)" - New forms 

                                                ////Updated 1/8/18 for HOM Upgrade MLW
                                                //if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                //{
                                                //    VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                //    validateAddress = true;
                                                //}
                                                //else
                                                //{
                                                //updated 11/28/17 for HOM Upgrade
                                                //if (formTypeId.EqualsAny(6, 7))
                                                if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {                                                   
                                                    VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    //VRGeneralValidations.Val_HasRequiredField_DD(sectionCoverage.ConstructionTypeId, valList, ConstructionType, "Construction Type");
                                                    //address
                                                    validateAddress = true;
                                                    //Added 2/24/2022 for task 62956 MLW
                                                    if (IFM.VR.Common.Helpers.Endorsements.EndorsementHelper.PermittedIncidentalOther_Task62956FeatureFlag() && quote.QuoteTransactionType == QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                                                    {
                                                        if (IsSectionIICoverageNewToImage(validateAddress, quote, sectionCoverage) == false)
                                                        {
                                                            validateAddress = false;
                                                        }
                                                    }
                                                }
                                                //}
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage:
                                                //CoverageName = "Special Event Coverage (92-347)"
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.WaterbedCoverage:
                                                //CoverageName = "Waterbed Liability (HO-85)"
                                                break;


                                            default:
                                                break;
                                        }
                                    }

                                    if (sectionCoverage.SectionCoverageIAndIIEnum != QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None)
                                    {
                                        switch (sectionCoverage.SectionCoverageIAndIIEnum)
                                        {
                                            //Added 1/31/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence:
                                                //CoverageName = "Loss Assessment (HO 0435)" - New forms
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    //validation in its user control
                                                } else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            //Added 2/1/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage:
                                                //CoverageName = "Loss Assessment (HO 0435)" - New forms
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Property Included Limit"))
                                                        VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Property Included Limit", "10000", "10000");
                                                    if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.LiabilityIncludedLimit, valList, LiabilityIncludedLimit, "Liability Included Limit"))
                                                        VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.LiabilityIncludedLimit, valList, LiabilityIncludedLimit, "Liability Included Limit", "100000", "100000");
                                                } else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;

                                            //Added 1/22/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment:
                                                //CoverageName = "Loss Assessment (HO 0435)" - New forms
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit"))
                                                            VRGeneralValidations.Val_IsNumberInRange(sectionCoverage.IncludedLimit, valList, IncludedLimit, "Included Limit", "1000", "1000");
                                                        //validateAddress = true; //3/16/18 Loss Assessment changed, no longer capturing address information
                                                    }
                                                }
                                                break;
                                            //Added 1/31/18 for HOM Upgrade MLW
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold:
                                                //CoverageName = "Other Members of Your Household (HO 0458)" - New forms
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Name.FirstName, valList, OtherFirstName, "First Name");
                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Name.LastName, valList, OtherLastName, "Last Name");
                                                } else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures:
                                                //CoverageName = "Permitted Incidental Occupancies Residence Premises - Other Structures (HO-42)" - Old forms
                                                //CoverageName = "Permitted Incidental Occupancies Residence Premises (HO-0442)" - New forms     

                                                //updated 11/28/17 for HOM Upgrade                                      
                                                //if (formTypeId.EqualsAny(6, 7))
                                                if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                else
                                                {
                                                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                    {
                                                        string txtDescr = sectionCoverage.Description;
                                                        int iBusiness = txtDescr.IndexOf("\r\n");
                                                        int iBuilding = iBusiness + 2;
                                                        string txtBusinessDescr = txtDescr.Substring(0, iBusiness);
                                                        string txtBuildingDescr = txtDescr.Substring(iBuilding);
                                                        VRGeneralValidations.Val_HasRequiredField(txtBusinessDescr, valList, Description, "Description");
                                                        if (txtBuildingDescr != "" || sectionCoverage.BuildingLimit != "")
                                                        {
                                                            VRGeneralValidations.Val_HasRequiredField(txtBuildingDescr, valList, BuildingDescription, "Description");
                                                            //VRGeneralValidations.Val_HasRequiredField(sectionCoverage.BuildingLimit, valList, TotalLimit, "Total Limit");
                                                            VRGeneralValidations.Val_HasRequiredField_Int(sectionCoverage.BuildingLimit, valList, TotalLimit, "Total Limit");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");

                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    }
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers:
                                                //CoverageName = "Structures Rented To Others (HO-40)" - Old forms
                                                //CoverageName = "Structures Rented To Others - Residence Premises (HO 0440)" - New forms

                                                //Updated 1/8/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {

                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");

                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    }
                                                }
                                                else
                                                {
                                                    //updated 11/28/17 for HOM Upgrade
                                                    //if (formTypeId.EqualsAny(4, 5, 6, 7))
                                                    if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-2", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                    else
                                                    {

                                                        if (VRGeneralValidations.Val_HasRequiredField(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit"))
                                                            VRGeneralValidations.Val_IsGreaterThanZeroNumber(sectionCoverage.IncreasedLimit, valList, IncreasedLimit, "Limit");

                                                        VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                    }
                                                }
                                        
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement:
                                                //CoverageName = "Trust Endorsement (HO 0615)" - New forms
                                                //Added 2/14/18 for HOM Upgrade MLW
                                                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                                {
                                                    if (CurrentForm.EqualsAny("HO-4", "ML-4"))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                    }
                                                }
                                                else
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                                }
                                                break;
                                            case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers:
                                                //
                                                break;

                                            default:
                                                break;
                                        }
                                    }

                                }
                                else
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("Missing County Name of the Home Location", CovRequiresCountyName));
                                }
                            }
                            else
                            {
                                valList.Add(new ObjectValidation.ValidationItem("Missing/Invalid Year Built of the Home", CovRequiresYearBuilt));
                            }
                        }
                        else
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Missing/Invalid FormTypeId", NoFormTypeId));
                        }
                        
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Missing Location", NoLocation));
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

            // only a few coverage actually need an address so set the 'validateAddress' flag in the coverage if you need the address validated
            if (validateAddress)
            {
                var addressVals = IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressValidation(sectionCoverage.Address, valType, true, true);
                //Added 2/23/18 for HOM Upgrade MLW
                if (quote != null)
                {
                    var MyLocation = quote.Locations[0];
                    string HomeVersion = GetHomeVersion(quote);
                    if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                    {
                        addressVals = IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressValidation(sectionCoverage.Address, valType, true, true, quote);
                    }
                }

                foreach (var val in addressVals)
                {
                    switch (val.FieldId)
                    {
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetAndPoBoxEmpty:
                            valList.Add(new ObjectValidation.ValidationItem("Missing Street #", AddressStreetNumber));
                            valList.Add(new ObjectValidation.ValidationItem("Missing Street Name", AddressStreetName));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.HouseNumberID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressStreetNumber));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StreetNameID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressStreetName));
                            break;
                        //case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.apt:
                        //    valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressAptNumber));
                        //    break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CityID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressCity));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.StateID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressState));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.ZipCodeID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressZipCode));
                            break;
                        case IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.CountyID:
                            valList.Add(new ObjectValidation.ValidationItem(val.Message, AddressCountyID));
                            break;
                    }
                }
            }

            return valList;
        }

        //Added 2/23/2022 for Task 64829 MLW
        private static bool IsSectionIICoverageNewToImage(bool isNew, QuickQuoteObject quote, SectionCoverage sectionCoverage)
        {
            if (quote != null && sectionCoverage != null) { 
                QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();
                QuickQuote.CommonObjects.QuickQuoteSectionIICoverage sc = sectionCoverage.UnderlyingSectionIICoverage();
                if (sc != null && QQHelper.IsQuickQuoteSectionIICoverageNewToImage(sc, quote) == false)
                {
                    isNew = false;
                }
            }
            return isNew;
        }
    }
}