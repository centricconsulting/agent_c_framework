using QuickQuote.CommonObjects;
using System;
using IFM.PrimativeExtensions;
using IFM.VR.Common.Helpers.FARM;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class PolicyCoverageValidator
    {
        public const string ValidationListID = "{7D19AE5B-4BF1-4AA0-A433-BFF5DB00E638}";

        public const string QuoteIsNull = "{12F6CCD4-5F7C-47B6-BB07-EAE3BEA89544}";
        public const string UnExpectedLobType = "{639C84B7-585F-47C0-B22A-FC5F1C0BDECF}";
        public const string NoLocation = "{45CCFCDC-EDFD-4F5B-BD32-6F283FF0F4A8}";
        public const string MSQuoteNotFound = "{3A745535-39FE-451D-904A-9CA5657B9F0B}";
        public const string MSQuoteIsNull = "{AE49E3C2-DC16-4F97-B109-05646E55751F}";

        public const string CovLRequired = "{D577BEF0-32BE-4719-BEA7-47C4C701A7BB}";
        public const string CovMRequired = "{708F6A7E-A24C-4CEA-BA20-E2C7CE0C3C57}";

        public const string FTEmpRequired = "{025E38F4-AAB4-4E2E-A1AB-CB6E233B0FD4}";
        public const string PT40Required = "{8B8B6A65-E824-4C88-896E-ED8D8E223DD1}";
        public const string PT41Required = "{E56FD40F-D6B4-4015-8471-0944307294E1}";
        public const string NumEmpRequired = "{25A7A4E2-BB0B-4C2E-B134-0BDE252BC8DE}";

        public const string FirstNameRequired = "{D3176B9B-1FB0-4DE4-A7A6-7F3DD9174F22}";
        public const string LastNameRequired = "{FB669490-A56B-417F-8065-C293D021955C}";

        public const string CustFarmTypeRequired = "{FAB9C8FC-479A-4BF3-BFF1-97314AC718EE}";
        public const string CustFarmReciptsRequired = "{028E440A-ED39-4C92-ABF2-1368FA929F0A}";

        public const string ExtraExpenseLimitRequired = "{11D13977-3A57-49F7-8A2F-3D7721110468}";
        public const string ExtraExpenseZero = "{4F99B26C-B4C2-4FCF-9720-42E6FE5EB998}";

        public const string FMPNumPersRequired = "{9E52CFBE-F716-4D37-A921-FC393D67BB4F}";

        public const string BusinessTypeRequired = "{7AC4FB6A-F439-44C9-B2AA-31B304D07338}";
        public const string BusinessReceiptsRequired = "{C610D329-C304-4621-9811-24BC837AB69D}";

        public const string InvalidValueForFarmPollutionLiabilityUpdate = "{9AE472EE-54B3-43D4-BACC-F1DAA9DAFA0B}";
        public const string OccurrenceLiabilityIsLessThanFarmPollution = "{AFFA20E1-6ABF-46C3-B1A6-C19003072466}";

        public const string PayrollRequired = "{AFFA30EB-6A11-46C3-B1A6-C19003072495}";
        public const string InvalidPayrollAmount = "{A00A20E1-4EBF-56C4-B1B6-C19113072452}";

        public const string StopGapLimitRequired = "{AFFA30AE-6A23-47D3-B1E6-C19113172497}";

        public static Validation.ObjectValidation.ValidationItemList ValidateFARCoverages(QuickQuote.CommonObjects.QuickQuoteObject quote, Int32 farmLocationInx, ValidationItem.ValidationType valType, bool cfError, string cfReceipts)
        {
            //FAR uses this
            //bool cfError = (bool)HttpContext.Current.Session["sess_CustomFarmingErr"];
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                var gsQuote = quote.GoverningStateQuoteFor();

                if (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                {
                    //Updated 9/7/18 for multi state MLW - qqh, parts, SubQuoteFirst, Quote to SubQuoteFirst
                    QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                    var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                    if (parts != null)
                    {
                        var SubQuoteFirst = parts.GetItemAtIndex(0);
                        if (SubQuoteFirst != null)
                        {                      
                            switch (valType)
                            {
                                case ValidationItem.ValidationType.issuance:
                                case ValidationItem.ValidationType.appRate:
                                    if (quote.Locations != null && quote.Locations.Count > farmLocationInx && quote.Locations[farmLocationInx].SectionIICoverages != null)
                                        foreach (QuickQuoteSectionIICoverage sc in quote.Locations[farmLocationInx].SectionIICoverages)
                                        {
                                            switch (sc.CoverageType)
                                            {
                                                case QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments:

                                                    //sc.NumberOfPersonsReceivingCare // might need to make sure that the count on loc.ResidentNames is the same
                                                    break;

                                                //case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures:
                                                //    VRGeneralValidations.Val_HasRequiredField_DD(sc.BusinessPursuitTypeId, valList, BusinessTypeRequired, "Type");
                                                //        break;

                                                case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying:
                                                case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying:
                                                    //VRGeneralValidations.Val_HasRequiredField(sc.Description, valList, CustFarmDescriptionRequired, "Description");
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                    goto case ValidationItem.ValidationType.quoteRate; // fall through

                                case ValidationItem.ValidationType.quoteRate:
                                    // This is evaluated on FO, FL & SOM where Liability Coverage Form is NOT "None"
                                    if (SubQuoteFirst.LiabilityOptionId != "6")
                                    {
                                        if (SubQuoteFirst.OccurrenceLiabilityLimitId == "0")
                                            valList.Add(new ObjectValidation.ValidationItem("Missing Coverage L", CovLRequired));

                                        if (SubQuoteFirst.MedicalPaymentsLimitid == "0")
                                            valList.Add(new ObjectValidation.ValidationItem("Missing Coverage M", CovMRequired));

                                        if (SubQuoteFirst.OptionalCoverages != null)
                                        {
                                            QuickQuoteOptionalCoverage extraExpense = SubQuoteFirst.OptionalCoverages.Find(p => p.CoverageType == QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense);
                                            //if (extraExpense != null)
                                            if (extraExpense != null && (!FarmExtenderHelper.IsFarmExtenderAvailable(quote) || (FarmExtenderHelper.IsFarmExtenderAvailable(quote) && !gsQuote.HasFarmExtender)))
                                            {
                                                if (extraExpense.IncreasedLimit == "" || extraExpense.IncreasedLimit == "0")
                                                    valList.Add(new ObjectValidation.ValidationItem("Missing Extra Expense", ExtraExpenseLimitRequired));
                                                else
                                                {
                                                    if (int.Parse(extraExpense.IncreasedLimit.Replace(",", "")) < 0)
                                                        valList.Add(new ObjectValidation.ValidationItem("Must be greater than 0", ExtraExpenseZero));
                                                }
                                            }
                                        }

                                        if (SubQuoteFirst.HasFarmEmployersLiability)
                                        {
                                            if ((SubQuoteFirst.EmployeesFullTime == "" || SubQuoteFirst.EmployeesFullTime == "0") && (SubQuoteFirst.EmployeesPartTime1To40Days == "" || SubQuoteFirst.EmployeesPartTime1To40Days == "0") && (SubQuoteFirst.EmployeesPartTime41To179Days == "" || SubQuoteFirst.EmployeesPartTime41To179Days == "0"))
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Employees", NumEmpRequired));
                                        }

                                        // Stop Gap validation
                                        if (gsQuote.StopGapLimitId != null && gsQuote.StopGapLimitId.Trim() != "")
                                        {
                                            // Stop Gap Limit
                                            //9/8/2022 StopGapLimitId validation removed for Bug 62410 MLW
                                            //// Check for N/A value (0)
                                            //if (gsQuote.StopGapLimitId == "0")
                                            //{
                                            //    valList.Add(new ObjectValidation.ValidationItem("Must select Stop Gap Coverage Limit", StopGapLimitRequired));
                                            //}

                                            // Stop Gap Payroll Amount
                                            // Check for null or empty string
                                            double mynum = 0;
                                            if (gsQuote.StopGapPayroll == null || gsQuote.StopGapPayroll.Trim() == "")
                                            {
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Payroll Amount", PayrollRequired));
                                            }
                                            else
                                            {
                                                // Check for numeric
                                                if (!double.TryParse(gsQuote.StopGapPayroll, out mynum))
                                                {
                                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Payroll Amount", InvalidPayrollAmount));
                                                }
                                                else
                                                {
                                                    // Check for value over 0
                                                    //Updated 9/9/2022 for task 62410 MLW
                                                    //if (mynum <= 0)
                                                    if (mynum <= 0 && gsQuote.StopGapLimitId != "0")
                                                    {
                                                        valList.Add(new ObjectValidation.ValidationItem("Invalid Payroll Amount", InvalidPayrollAmount));
                                                    }
                                                }
                                            }
                                        }

                                        // Put validation for Incidental Business Pursuit Type Here

                                        bool cfAnnualReceiptsErr = false;

                                        if (cfError)
                                        {
                                            valList.Add(new ObjectValidation.ValidationItem("Missing Custom Spray Type", CustFarmTypeRequired));
                                            cfAnnualReceiptsErr = true;

                                            if (cfReceipts == "")
                                            {
                                                valList.Add(new ObjectValidation.ValidationItem("Missing Annual Receipts", CustFarmReciptsRequired));
                                            }
                                        }

                                        if (quote.Locations[farmLocationInx].SectionIICoverages != null)
                                        {
                                            foreach (QuickQuoteSectionIICoverage sc in quote.Locations[farmLocationInx].SectionIICoverages)
                                            {
                                                switch (sc.CoverageType)
                                                {
                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9:
                                                        if (sc.Name.FirstName == "")
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing First Name for Personal Liability", FirstNameRequired));

                                                        if (sc.Name.LastName == "")
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing Last Name for Personal Liability", LastNameRequired));
                                                        break;

                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying:
                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying:
                                                        if (sc.EstimatedReceipts == "")
                                                        {
                                                            cfAnnualReceiptsErr = false;
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing Annual Receipts", CustFarmReciptsRequired));
                                                        }
                                                        break;

                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments:
                                                        if (sc.NumberOfPersonsReceivingCare == "")
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing Number of Persons", FMPNumPersRequired));
                                                        break;

                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures:
                                                        if (sc.BusinessPursuitTypeId == "")
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing Type", BusinessTypeRequired));
                                                        if (sc.EstimatedReceipts == "")
                                                            valList.Add(new ObjectValidation.ValidationItem("Missing Annual Receipts", BusinessReceiptsRequired));
                                                        break;
                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement:
                                                    case QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability:
                                                        DateTime FarmPollutionUpdatedDDLValuesEffectiveDate;
                                                        if (System.Configuration.ConfigurationManager.AppSettings["VR_Farm_EarliestAllowedEffectiveDate"] != null && DateTime.TryParse(System.Configuration.ConfigurationManager.AppSettings["VR_FarmPollutionLiabilityUpdate_EffectiveDate"], out FarmPollutionUpdatedDDLValuesEffectiveDate))
                                                        {
                                                            //tryparse succesfull
                                                        }
                                                        else
                                                        {
                                                            //tryparse not succesfull
                                                            FarmPollutionUpdatedDDLValuesEffectiveDate = Convert.ToDateTime("10/01/2016");
                                                        }

                                                        if (Convert.ToDateTime(quote.EffectiveDate) < FarmPollutionUpdatedDDLValuesEffectiveDate)
                                                        {
                                                            if (sc.IncreasedLimitId == "54" || sc.IncreasedLimitId == "390")
                                                            {
                                                                string FarmPollutionMessage = "";
                                                                sc.IncreasedLimitId = "50";
                                                                sc.TotalLimit = "100,000";
                                                                if (SubQuoteFirst.LiabilityOptionId == "1")
                                                                {
                                                                    FarmPollutionMessage += "Limited Farm Pollution - ";
                                                                }
                                                                else if (SubQuoteFirst.LiabilityOptionId == "2")
                                                                {
                                                                    FarmPollutionMessage += "Liability Enhancement Endorsement - ";
                                                                }
                                                                FarmPollutionMessage += "The values '250,000' and '500,000' can only be selected with an effective date on or after " + FarmPollutionUpdatedDDLValuesEffectiveDate.ToShortDateString() + ". The value has been switched to '100,000'";
                                                                valList.Add(new ObjectValidation.ValidationItem(FarmPollutionMessage, InvalidValueForFarmPollutionLiabilityUpdate, true));
                                                            }
                                                        }

                                                        decimal occLiability;
                                                        decimal totalLimit;
                                                        if (decimal.TryParse(SubQuoteFirst.OccurrenceLiabilityLimit, out occLiability) && decimal.TryParse(sc.TotalLimit, out totalLimit) && occLiability < totalLimit)
                                                        {
                                                            string errMessage = "Pollution coverage limit cannot be greater than policy occurrence limit.";
                                                            valList.Add(new ObjectValidation.ValidationItem(errMessage, OccurrenceLiabilityIsLessThanFarmPollution, false));
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        else
                        {
                            valList.Add(new ObjectValidation.ValidationItem("Multi State quote is null", MSQuoteIsNull));
                        }
                    }
                    else
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Multi State quote not found", MSQuoteNotFound));
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