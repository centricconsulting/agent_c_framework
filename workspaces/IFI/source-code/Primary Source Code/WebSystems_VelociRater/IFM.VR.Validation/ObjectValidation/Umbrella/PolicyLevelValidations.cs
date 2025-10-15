using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.PrimativeExtensions;


namespace IFM.VR.Validation.ObjectValidation.Umbrella
{
    public class PolicyLevelValidations
    {
        public const string ValidationListID = "{4A8988EA-C78E-41E2-9F07-571C1933C0F4}";
        public const string ddlUmbrellaLimit = "{4901693B-6E1B-4B2A-8601-6F3A9EEC1D02}";
        public const string ddlUmbrellaUmUimLimit = "{6B10276A-9532-422E-BBA5-FE6FB4543A7C}";
        public static Validation.ObjectValidation.ValidationItemList ValidatePolicyLevel(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            { 
                QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                var parts = qqh.MultiStateQuickQuoteObjects(ref quote);
                if (parts != null)
                {
                    //Updated 11/05/2019 for Bug 33751 MLW - SubQuoteFirst to GoverningStateQuote
                    var stateType = quote.QuickQuoteState;
                    var GoverningStateQuote = parts.FirstOrDefault(x => x.QuickQuoteState == stateType);
                    if (GoverningStateQuote == null)
                    {
                        GoverningStateQuote = parts.GetItemAtIndex(0);
                    }
                    if (GoverningStateQuote != null)
                    {

                        switch (valType)
                        {
                            case ValidationItem.ValidationType.issuance:
                            case ValidationItem.ValidationType.appRate:
                            case ValidationItem.ValidationType.quoteRate:
                            case ValidationItem.ValidationType.endorsement:

                                VRGeneralValidations.Val_HasRequiredField_DD(GoverningStateQuote.UmbrellaCoverageLimitId, valList, ddlUmbrellaLimit, "Umbrella Limit");

                                if (GoverningStateQuote.UmbrellaUmUimLimitId != GoverningStateQuote.UmbrellaCoverageLimitId && GoverningStateQuote.UmbrellaUmUimLimitId != "0") // Zero is allowed for N/A.
                                {
                                    valList.Add(new ObjectValidation.ValidationItem("UM/UIM must be N/A or match Umbrella Limit.", ddlUmbrellaUmUimLimit));
                                }

                                break;
                            default:
                                break;
                        }
                    }
                }
                
            }
            return valList;

        }

    }
}