using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFM.VR.Validation.ObjectValidation.CommLines.LOB.CAP
{
    public class AdditionalInterestListValidator
    {
        //Added 06/06/2021 for CAP Endorsements Task 52974 MLW
        public const string ValidationListID = "{7D9E6C09-A8CF-4342-889B-2A9D02F6819F}";
        public const string AssignAdditionalInterestToVehicle = "{F0EFB325-F617-4127-B791-686FC887ADD1}";

        public static Validation.ObjectValidation.ValidationItemList ValidateAdditionalInterestList(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (valType == Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate)
                {
                    int aiIndex = 0;
                    foreach (var qai in quote.AdditionalInterests)
                    {
                        bool aiAssignedToVehicle = false;
                        aiIndex += 1;
                        QuickQuote.CommonMethods.QuickQuoteHelperClass qqh = new QuickQuote.CommonMethods.QuickQuoteHelperClass();
                        if (qqh.IsQuickQuoteAdditionalInterestNewToImage(qai, quote)) {
                            foreach (var v in quote.Vehicles)
                            {
                                if (v.AdditionalInterests != null)
                                {                               
                                    foreach (var vai in v.AdditionalInterests)
                                    {
                                        if (vai.ListId == qai.ListId)
                                        {
                                            aiAssignedToVehicle = true;
                                        }
                                    }
                                }
                            }
                        } else
                        {
                            aiAssignedToVehicle = true; //We do not trigger message for existing AIs even if they are not assigned to a vehicle
                        }
                        if (aiAssignedToVehicle == false)
                        {
                            valList.Add(new ValidationItem("Must assign Additional Interest #" + aiIndex + " to a vehicle.", AssignAdditionalInterestToVehicle, false));
                        }                        
                    }
                }
            }
            return valList;
        }
    }
}