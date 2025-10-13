using QuickQuote.CommonMethods;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class LocationMobileHomeValidator
    {
        public const string ValidationListID = "{20A8DE28-A45D-4609-83A2-90122BE21E56}";

        public const string QuoteIsNull = "{56795C14-C81D-4ECD-8241-32EBCA4F98DC}";
        public const string NoLocations = "{AFC13532-0BE9-499F-BFD1-DE49372C15AD}";

        public const string LocationTieDown = "{9CA5086D-466C-44B8-856E-C3D972B34C83}";
        public const string LocationSkirting = "{71516022-1645-4CA2-88D4-E1386A23EF1F}";
        public const string LocationFoundationType = "{1B9AC905-B0A8-4BD2-A40E-3EDACB72DCDF}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMMobileHome(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType)
        {
            //HOM, DFR uses this
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, LocationIndex.ToString());

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];
                    switch (MyLocation.FormTypeId)
                    {
                        case "6":
                        case "7":
                            VRGeneralValidations.Val_HasRequiredField(MyLocation.MobileHomeTieDownTypeId, valList, LocationTieDown, "Tie Down");
                            VRGeneralValidations.Val_HasRequiredField(MyLocation.MobileHomeSkirtTypeId, valList, LocationSkirting, "Skirting");
                            VRGeneralValidations.Val_HasRequiredField(MyLocation.FoundationTypeId, valList, LocationFoundationType, "Foundation Type");
                            break;

                        case "22": //Updated 12/5/17 for HOM Upgrade MLW - added case 22, 25
                        case "25":
                            if (MyLocation.StructureTypeId == "2")
                            {
                                VRGeneralValidations.Val_HasRequiredField(MyLocation.MobileHomeTieDownTypeId, valList, LocationTieDown, "Tie Down");
                                VRGeneralValidations.Val_HasRequiredField(MyLocation.MobileHomeSkirtTypeId, valList, LocationSkirting, "Skirting");
                                VRGeneralValidations.Val_HasRequiredField(MyLocation.FoundationTypeId, valList, LocationFoundationType, "Foundation Type");
                            }
                            break;

                        default: break;
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