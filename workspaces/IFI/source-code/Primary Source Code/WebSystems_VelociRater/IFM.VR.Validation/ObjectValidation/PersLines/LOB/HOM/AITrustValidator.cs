using IFM.PrimativeExtensions;
using QuickQuote.CommonMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class AITrustValidator
    {
        public const string ValidationListID = "{23CE8695-B88E-419C-ADD1-969AF50295A3}";
        public const string AiIsNull = "{7C4FD126-B7DF-4E54-84FC-27A702B8FCC7}";
        public const string TrustName = "{4F49E467-22AB-4411-A62B-589A0461B874}";
        //public const string TrusteeFirstName = "{BFE00B47-BF9D-44C4-8516-706E4BC73728}";
        //public const string TrusteeLastName = "{FACC88B7-2138-4B06-8293-A62B5FA40E30}";
        public const string TrusteeName = "{BFE00B47-BF9D-44C4-8516-706E4BC73728}";

        public static Validation.ObjectValidation.ValidationItemList AdditionalInterestTrustValidation(QuickQuote.CommonObjects.QuickQuoteAdditionalInterest MyAdditionalInterest, ValidationItem.ValidationType valType, IFM.VR.Common.Helpers.HOM.SectionCoverage sectionCoverage, QuickQuote.CommonObjects.QuickQuoteObject quote = null)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (MyAdditionalInterest != null)
            {
                if (MyAdditionalInterest.TypeId == "79") //Trust
                {
                    VRGeneralValidations.Val_HasRequiredField(MyAdditionalInterest.Name.CommercialName1, valList, TrustName, "Trust Name");
                }

                if (MyAdditionalInterest.TypeId == "80") //Trustee
                {
                    if (MyAdditionalInterest.Name.CommercialName1.IsNullEmptyorWhitespace())
                    {
                        valList.Add(new ObjectValidation.ValidationItem("Missing Name", TrusteeName));
                    }
                    //var fullName = MyAdditionalInterest.Name.CommercialName1;
                    //if (fullName != "" || fullName != null) {
                    //    if (fullName.Contains('|'))
                    //    {
                    //        string[] arrName = fullName.Split('|');
                    //        if (arrName.Length > 0)
                    //        {
                    //            if (arrName[0] == "")
                    //            {
                    //                valList.Add(new ObjectValidation.ValidationItem("Missing First Name", TrusteeFirstName));
                    //            }
                    //        }
                    //        else
                    //        {
                    //            valList.Add(new ObjectValidation.ValidationItem("Missing First Name", TrusteeFirstName));
                    //        }
                    //        if (arrName.Length > 2)
                    //        {
                    //            if (arrName[2] == "")
                    //            {
                    //                valList.Add(new ObjectValidation.ValidationItem("Missing Last Name", TrusteeLastName));
                    //            }
                    //        }
                    //        else
                    //        {
                    //            valList.Add(new ObjectValidation.ValidationItem("Missing Last Name", TrusteeLastName));
                    //        }
                    //    }
                    //    else
                    //    {
                    //        valList.Add(new ObjectValidation.ValidationItem("Missing Last Name", TrusteeLastName));
                    //    }
                    //} else {
                    //    valList.Add(new ObjectValidation.ValidationItem("Missing First Name", TrusteeFirstName));
                    //    valList.Add(new ObjectValidation.ValidationItem("Missing Last Name", TrusteeLastName));
                    //}
                }

            }
            else
            {
                valList.Add(new ValidationItem("Additional Interest is null", AiIsNull, false));
            }

            return valList;
        }
    }
}