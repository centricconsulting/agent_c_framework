using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.DFR
{
    public class ResidenceCoverageValidator
    {
        public const string ValidationListID = "{E8B21EF1-4039-4BE8-82EB-2552DBF56E00}";

        public const string QuoteIsNull = "{87D7B94F-36D4-4AFD-8B1F-9408C9CD0616}";
        public const string NoLocations = "{48265284-EFF1-4BEF-A98A-7A551558ED3E}";

        public const string CoverageAMin = "{F61D476E-0983-4FE1-9B98-808FEB05662E}";
        public const string MaxCovBTotal = "{5E784481-B8ED-4453-ACD2-67F4E58D9EBD}";

        public const string ReqPresLiab = "{3904FC58-D4C9-42C9-9617-F9C8CBD2775C}";
        public const string ReqMedPay = "{AAA013C6-2530-49FB-AB0C-20701EC19ECD}";
        public const string MissingEarthquake = "{71C191B4-17D1-4402-9AEF-58D7DEC442FC}";
        public const string MissingCoverageA = "{CB404CCC-CB3F-4FF7-9C91-4D3F13286316}";

        public static Validation.ObjectValidation.ValidationItemList ValidateDFRLocationResidence(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType)
        {
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, LocationIndex.ToString());

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];

                    switch (valType)
                    {
                        case ValidationItem.ValidationType.quoteRate:
                        case ValidationItem.ValidationType.appRate:
                            bool covAError = false;

                            switch (MyLocation.FormTypeId)
                            {
                                case "8":   // Fire
                                case "9":   // Fire and EC
                                case "10":  // EC and V&MM
                                    if (MyLocation.A_Dwelling_LimitIncreased != "")
                                    {
                                        //removed 10/28/2020 (Interoperability)
                                        //if (MyLocation.UsageTypeId != "1")
                                        //{
                                        //    if (MyLocation.NumberOfFamiliesId != "3" && MyLocation.NumberOfFamiliesId != "4" && int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) < 25000)
                                        //    {
                                        //        covAError = true;
                                        //        valList.Add(new ObjectValidation.ValidationItem("Minimum Cov A Limit is $25,000", CoverageAMin));
                                        //    }

                                        //    if (MyLocation.NumberOfFamiliesId != "1" && MyLocation.NumberOfFamiliesId != "2")
                                        //    {
                                        //        if (int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) < 50000)
                                        //        {
                                        //            covAError = true;
                                        //            valList.Add(new ObjectValidation.ValidationItem("Minimum Cov A Limit is $50,000", CoverageAMin));
                                        //        }
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    if (int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) < 25000)
                                        //    {
                                        //        covAError = true;
                                        //        valList.Add(new ObjectValidation.ValidationItem("Minimum Cov A Limit is $25,000", CoverageAMin));
                                        //    }
                                        //}

                                        if (!covAError)
                                        {
                                            if (int.Parse(MyLocation.B_OtherStructures_Limit.Replace(",", "")) > (int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) * .30))
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage B limit exceeds 30% of Coverage A limit. Refer to Underwriting for approval", MaxCovBTotal));
                                        }
                                    }
                                    else
                                        valList.Add(new ObjectValidation.ValidationItem("Required Field", MissingCoverageA));
                                    break;

                                case "11":  // Broad
                                case "12":  // Special
                                    if (MyLocation.A_Dwelling_LimitIncreased != "")
                                    {
                                        //removed 10/28/2020 (Interoperability)
                                        //if (MyLocation.UsageTypeId != "1")
                                        //{
                                        //    if (MyLocation.NumberOfFamiliesId != "3" && MyLocation.NumberOfFamiliesId != "4" && int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) < 40000)
                                        //    {
                                        //        covAError = true;
                                        //        valList.Add(new ObjectValidation.ValidationItem("Minimum Cov A Limit is $40,000", CoverageAMin));
                                        //    }

                                        //    if (MyLocation.NumberOfFamiliesId != "1" && MyLocation.NumberOfFamiliesId != "2")
                                        //    {
                                        //        if (int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) < 80000)
                                        //        {
                                        //            covAError = true;
                                        //            valList.Add(new ObjectValidation.ValidationItem("Minimum Cov A Limit is 80,000", CoverageAMin));
                                        //        }
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    if (int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) < 40000)
                                        //    {
                                        //        covAError = true;
                                        //        valList.Add(new ObjectValidation.ValidationItem("Minimum Cov A Limit is $40,000", CoverageAMin));
                                        //    }
                                        //}

                                        if (!covAError)
                                        {
                                            if (int.Parse(MyLocation.B_OtherStructures_Limit.Replace(",", "")) > (int.Parse(MyLocation.A_Dwelling_LimitIncreased.Replace(",", "")) * .30))
                                                valList.Add(new ObjectValidation.ValidationItem("Coverage B limit exceeds 30% of Coverage A limit. Refer to Underwriting for approval", MaxCovBTotal));
                                        }
                                    }
                                    else
                                        valList.Add(new ObjectValidation.ValidationItem("Required Field", MissingCoverageA));
                                    break;
                            }

                            //No updates needed for multi-state since this is for DFR only
                            if (quote.PersonalLiabilityLimitId != "0" && quote.MedicalPaymentsLimitid == "0")
                                valList.Add(new ObjectValidation.ValidationItem("Medical Payment is required when Personal Liability is selected.", ReqMedPay));

                            //No updates needed for multi-state since this is for DFR only
                            if (quote.PersonalLiabilityLimitId == "0" && quote.MedicalPaymentsLimitid != "0")
                                valList.Add(new ObjectValidation.ValidationItem("Personal Liability is required when Medical Payment is selected.", ReqPresLiab));

                            if (quote.Locations[0].SectionICoverages != null)
                            {
                                QuickQuoteSectionICoverage sectionICoverage = MyLocation.SectionICoverages.Find(p => p.CoverageType == QuickQuoteSectionICoverage.SectionICoverageType.Earthquake);
                                if (sectionICoverage != null)
                                {
                                    if (sectionICoverage.DeductibleLimitId == "0")
                                        valList.Add(new ObjectValidation.ValidationItem("Missing Earthquake Deductible", MissingEarthquake));
                                }
                            }
                            break;
                    }
                }
                else
                {
                    // no location
                    valList.Add(new ObjectValidation.ValidationItem("No locations", NoLocations));
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