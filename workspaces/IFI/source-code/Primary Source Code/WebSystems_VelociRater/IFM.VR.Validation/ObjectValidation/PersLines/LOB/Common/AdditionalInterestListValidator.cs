using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
{
    public class AdditionalInterestListValidator
    {
        public const string ValidationListID = "{67AB0230-6876-44EC-BE04-0D07DDDA5BC7}";

        public const string AiListIsNull = "{D1F5E595-633F-47E7-8F90-327A61F00F84}";
        public const string HasThirdMortgagee = "{29AD3D62-6003-4963-BB4F-8C83CEF6ECA0}";
        public const string MortgageeTypeUsedMultipleTimes = "{520FDE07-5AA1-4FD9-8586-703356BEF8E4}";

        public const string MultipleBillToFlagsSet = "{0C2B9BBB-C320-4BAD-89AE-E2B88C7F7A28}";

        public const string VehicleRequiredAiButNone = "{81606753-5BD7-47DC-B8E0-93D4EBC7132C}";

        public const string CompCollRequiredWithAi = "{E01A6D4D-7C6B-401C-B637-FFBA7547F859}";

        public static Validation.ObjectValidation.ValidationItemList ValidateAdditionalInterestList(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType, int vehicleIndex = 0)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                switch (quote.LobType)
                {
                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm: // do same as home
                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal:
                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal:
                        if (quote.Locations != null && quote.Locations.Any())
                        {
                            var l = quote.Locations[0];
                            if (l != null && l.AdditionalInterests != null)
                            {
                                int firstMortgageeCount = 0;
                                int secondMortgageeCount = 0;
                                int thirdMortgageeCount = 0;

                                int BillToCount = 0;

                                foreach (var ai in l.AdditionalInterests)
                                {
                                    if (ai.TypeId == "42")
                                        firstMortgageeCount += 1;
                                    if (ai.TypeId == "11")
                                        secondMortgageeCount += 1;
                                    if (ai.TypeId == "15")
                                        thirdMortgageeCount += 1;
                                    if (ai.BillTo)
                                    {
                                        BillToCount += 1;
                                    }
                                }

                                if (firstMortgageeCount > 1 | secondMortgageeCount > 1 | thirdMortgageeCount > 1)
                                {
                                    // called 'Insured' for Farm and 'Interest' on all other LOBs
                                    valList.Add(new ValidationItem(string.Format("Same Mortgagee Type used in multiple Additional {0}", (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm) ? "Insureds" : "Interests"), MortgageeTypeUsedMultipleTimes, false));
                                }

                                if (BillToCount > 1)
                                {
                                    // called 'Insured' for Farm and 'Interest' on all other LOBs
                                    valList.Add(new ValidationItem(string.Format("BillTo used in multiple Additional {0}", (quote.LobType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm) ? "Insureds" : "Interests"), MultipleBillToFlagsSet, false));
                                }
                                
                                if (valType == ValidationItem.ValidationType.issuance)
                                {
                                    if (thirdMortgageeCount > 0)
                                        valList.Add(new ValidationItem("Property with more than two mortgages. Refer to Underwriting for approval", HasThirdMortgagee, false, true));
                                }
                            }
                            else
                            {
                                valList.Add(new ValidationItem("Ai List is null", AiListIsNull, false));
                            }
                        }
                        else
                        {
                            valList.Add(new ValidationItem("Ai List is null", AiListIsNull, false));
                        }
                        break;

                    case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal:
                        QuickQuote.CommonObjects.QuickQuoteVehicle vehicle = null;
                        if (quote.Vehicles != null && quote.Vehicles.Count > vehicleIndex)
                        {
                            vehicle = quote.Vehicles[vehicleIndex];
                            if (vehicle != null)
                            {
                                if (vehicle.HasAutoLoanOrLease && (vehicle.AdditionalInterests == null || vehicle.AdditionalInterests.Any() == false))
                                {
                                    valList.Add(new ValidationItem("Additional Interest is required when Loan/Lease coverage option is applied to vehicle.", VehicleRequiredAiButNone, false));
                                }
                            }
                        }

                        if (valType == ValidationItem.ValidationType.issuance) // Matt A - added 4-13-15  - Bug 4717
                        {
                            if (quote.Vehicles != null)
                            {
                                string[] ids = new string[] { "53", "54", "8", "32", "14" };
                                int vNum = 0;
                                foreach (var v in quote.Vehicles)
                                {
                                    vNum += 1;
                                    if (v.AdditionalInterests != null)
                                    {
                                        foreach (var ai in v.AdditionalInterests)
                                        {
                                            if (ai != null)
                                            {
                                                if (ids.ToList().Contains(ai.TypeId.Trim()))
                                                {
                                                    if (string.IsNullOrWhiteSpace(v.ComprehensiveDeductibleLimitId) | string.IsNullOrWhiteSpace(v.CollisionDeductibleLimitId))
                                                    {
                                                        valList.Add(new ValidationItem(string.Format("Vehicle #{0} - Collision and Comp Coverage must be added when an Additional Interest has been added.", vNum), CompCollRequiredWithAi, false));
                                                    }
                                                }
                                            }
                                        }
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
                valList.Add(new ValidationItem("Ai List is null", AiListIsNull, false));
            }

            return valList;
        }
    }
}