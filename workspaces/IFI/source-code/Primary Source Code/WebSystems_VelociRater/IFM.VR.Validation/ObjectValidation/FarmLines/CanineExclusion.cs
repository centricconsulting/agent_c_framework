using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IFM.PrimativeExtensions;
using IFM.VR.Common.Helpers.HOM;
using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;

namespace IFM.VR.Validation.ObjectValidation.FarmLines
{
    public class CanineExclusion
    {
        public const string ValidationListID = "{1E9B7F4D-C6EC-49DD-A902-A07F595B1E04}";

        public const string Description = "{71EDE79A-5A04-4C4A-849B-6777ECE983E9}";
        public const string Name = "{F06F36A6-0674-465F-8ECF-8283562758FA}";



        public static Validation.ObjectValidation.ValidationItemList ValidateCanineExclusionCoverage(QuickQuote.CommonObjects.QuickQuoteObject quote, QuickQuoteSectionIICoverage sectionCoverage, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                //CoverageName = "Canine Liability Exclusion"
                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Name.FirstName, valList, Name, "Canine Name");
                VRGeneralValidations.Val_HasRequiredField(sectionCoverage.Description, valList, Description, "Canine Description");

                if (valType == ValidationItem.ValidationType.appRate || valType == ValidationItem.ValidationType.issuance || quote.QuoteTransactionType == QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote)
                {
                    if (sectionCoverage.Description.Length >= 8 && sectionCoverage.Description.ToUpper().Substring(0, 8) == "CANINE #")
                    {
                        valList.Add(new ValidationItem("Missing Canine Description", Description));
                    }
                }
            }
            return valList;
        }

                                          
    }
}