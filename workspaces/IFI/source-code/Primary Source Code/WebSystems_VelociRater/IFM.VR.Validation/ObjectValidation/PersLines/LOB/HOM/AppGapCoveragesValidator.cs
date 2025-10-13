using IFM.PrimativeExtensions;
using QuickQuote.CommonMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class AppGapCoveragesValidator
    {
        public const string ValidationListID = "{67133D25-DE59-4A02-A06E-321AB96020CA}";
        public const string Description = "{5835A592-3794-44BC-97DB-5C5F48338D43}";
        public const string BuildingDescription = "{89C1BE49-F656-458D-99F9-A216E32E5173}";
        public const string CoverageNotValidForFormType = "{822FA13E-082B-4DB8-96F5-09AD2141BE4C}";
        public const string AddressStreetNumber = "{4A3F8340-5CE9-4AAA-9D8F-B741472A30DF}";
        public const string AddressStreetName = "{42BB9A4C-B12D-4A85-8030-9651D10E2276}";
        public const string AddressAptNumber = "{8A92BFE0-2CEF-49B1-8236-1E30DFB09C84}";
        public const string AddressZipCode = "{39E7BCBF-A32B-41C9-9A29-6D258BEBAD0A}";
        public const string AddressCity = "{16B6A138-BD4B-49BE-9B79-59D13EA60525}";
        public const string AddressState = "{CD8E0978-C09C-4F54-B066-486B03A7882D}";
        public const string AddressSatetNotIndiana = "{3B5CB2D0-5D0A-4306-AC37-37C0D491DF68}";
        public const string AddressCountyID = "{7EB2C4F8-00D8-4B5F-B5FA-BC7A39EDA5AB}";


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
                if (qqh.doUseNewVersionOfLOB(quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade)) //New QQ dll uses LOBType
                //if (qqh.doUseNewVersionOfLOB(eDate, quote.LobType, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade))
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

        //public static Validation.ObjectValidation.ValidationItemList ValidateAppGapCoverage(QuickQuote.CommonObjects.QuickQuoteObject quote, IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper sectionCoverage, int coverageIndex, ValidationItem.ValidationType valType)
        public static Validation.ObjectValidation.ValidationItemList ValidateAppGapCoverage(QuickQuote.CommonObjects.QuickQuoteObject quote, IFM.VR.Common.Helpers.HOM.HomAppGapCoveragesHelper sectionCoverage, int coverageIndex, ValidationItem.ValidationType valType)
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
                        if (formTypeId.EqualsAny(22, 23, 24, 25, 26) && HomeVersion == "After20180701")
                        {
                            if (sectionCoverage != null)
                            {
                                if (sectionCoverage.SectionCoverageIEnum != QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None)
                                {
                                    switch (sectionCoverage.SectionCoverageIEnum)
                                    {
                                        case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures:
                                            //Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                            //case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises:
                                            //CoverageName = "Other Structures On the Residence Premises (HO 0448)" - New forms
                                            VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises:
                                            //CoverageName = "Specific Structures Away from Residence Premises (HO 0492) - New forms
                                            if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                validateAddress = true;
                                            }
                                            break;
                                    } // END of SectionI switch

                                } // END is a sectionI

                                if (sectionCoverage.SectionCoverageIIEnum != QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None)
                                {
                                    switch (sectionCoverage.SectionCoverageIIEnum)
                                    {
                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured:
                                            //CoverageName = "Additional Residence – Occupied by Insured (N/A)"     
                                            validateAddress = true;
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther:
                                            //CoverageName = "Additional Residence - Rented to Others (HO 2470)" - New forms
                                            VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            validateAddress = true;
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion:
                                            //CoverageName = "Canine Liability Exclusion (HO 2477)"
                                            if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            }
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres:
                                            //CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres (HO-2446)" - New forms
                                            if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                validateAddress = true;
                                            }

                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability:
                                            //CoverageName = "Incidental Farming Personal Liability - On Premises (HO 2472)" - New forms
                                            if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            }
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises:
                                            //CoverageName = "Incidental Farming Personal Liability - Off Premises (HO 2472)" - New forms
                                            if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                                validateAddress = true;
                                            }
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence:
                                            //CoverageName = "Permitted Incidental Occupancies Other Residence (HO-43)"     
                                            VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            validateAddress = true;                                       
                                            break;
                                    } //END of SectionII switch
                                } // END is a sectionII

                                if (sectionCoverage.SectionCoverageIAndIIEnum != QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None)
                                {
                                    switch (sectionCoverage.SectionCoverageIAndIIEnum)
                                    {
                                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence:
                                            //CoverageName = "Loss Assessment (HO 0435)" - New forms
                                            if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal && HomeVersion == "After20180701" && MyLocation.FormTypeId.EqualsAny("22", "23", "24", "25", "26"))
                                            {
                                                //validation in its user control
                                            }
                                            else
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage:
                                            //CoverageName = "Loss Assessment (HO 0435)" - New forms
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold:
                                            //CoverageName = "Other Members of Your Household (HO 0458)" - New forms                                        
                                            VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures:
                                            //CoverageName = "Permitted Incidental Occupancies Residence Premises (HO 0442)" - New forms     
                                            if (CurrentForm.EqualsAny("ML-2", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                string txtDescr = sectionCoverage.Description;
                                                int iBusiness = txtDescr.IndexOf("\r\n");
                                                int iBuilding = iBusiness + 2;
                                                string txtBusinessDescr = txtDescr.Substring(0, iBusiness);
                                                string txtBuildingDescr = txtDescr.Substring(iBuilding);
                                                VRGeneralValidations.Val_HasRequiredField(txtBusinessDescr, valList, Description, "Description");
                                                if (sectionCoverage.BuildingLimit != "" && sectionCoverage.BuildingLimit.IsNumeric() && Decimal.Parse(sectionCoverage.BuildingLimit) > 0 )
                                                {
                                                    VRGeneralValidations.Val_HasRequiredField(txtBuildingDescr, valList, BuildingDescription, "Description");
                                                    if(txtBuildingDescr.Equals("Building", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem("Missing Description", BuildingDescription));
                                                    }
                                                }
                                            }
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers:
                                            //CoverageName = "Structures Rented To Others - Residence Premises (HO 0440)" - New forms
                                            if (CurrentForm.EqualsAny("HO-4", "HO-6", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            else
                                            {
                                                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Description");
                                            }
                                            break;

                                        case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement:
                                            //CoverageName = "Trust Endorsement (HO 0615)" - New forms
                                            if (CurrentForm.EqualsAny("HO-4", "ML-4"))
                                            {
                                                //valList.Add(new ObjectValidation.ValidationItem(invalidCoverageForFormTypeMessage, CoverageNotValidForFormType));
                                            }
                                            break;

                                        default:
                                            break;
                                    } //END of SectionI&II switch
                                } // END is a sectionI&II
                            } //end sectionCoverage not null
                        } //End form and version types


                    
                    } //end quote.locations
                } //end hom lob type
            } //end quote null



            // only a few coverage actually need an address so set the 'validateAddress' flag in the coverage if you need the address validated
            if (validateAddress)
            {
                var addressVals = IFM.VR.Validation.ObjectValidation.AllLines.AddressValidator.AddressValidation(sectionCoverage.Address, valType, true, true);
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

    }
}