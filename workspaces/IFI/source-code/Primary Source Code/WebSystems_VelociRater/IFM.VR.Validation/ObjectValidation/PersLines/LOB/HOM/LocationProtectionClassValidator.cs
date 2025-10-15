using QuickQuote.CommonMethods;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class LocationProtectionClassValidator
    {
        public const string ValidationListID = "{113ED98F-7D5C-4223-9C9A-70104829A69D}";

        public const string QuoteIsNull = "{4E62014D-E2AC-443B-B785-00FB9643365F}";
        public const string NoLocations = "{26BB15CD-00D7-4FAD-A086-5290AF00ADB6}";

        public const string LocationProtectionClass = "{630B7BFE-0151-4F00-AE77-916EF19732FA}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMProtectionClass(QuickQuote.CommonObjects.QuickQuoteObject quote, int LocationIndex, ValidationItem.ValidationType valType)
        {
            //Do not believe this is used
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, LocationIndex.ToString());

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any() && quote.Locations.Count > LocationIndex)
                {
                    var MyLocation = quote.Locations[LocationIndex];
                    if (VRGeneralValidations.Val_HasRequiredField(MyLocation.ProtectionClassId, valList, LocationProtectionClass, "Protection Class"))
                    {
                        switch (MyLocation.FormTypeId)
                        {
                            case "6":
                            case "7":
                                if (MyLocation.ProtectionClassId == "11")
                                    valList.Add(new ObjectValidation.ValidationItem("Invalid Protection Class", LocationProtectionClass));
                                break;

                            case "22": //Updated 12/5/17 for HOM Upgrade MLW - added case 22, 25 for new mobile
                            case "25":
                                if (MyLocation.StructureTypeId == "2")
                                {
                                    if (MyLocation.ProtectionClassId == "11")
                                        valList.Add(new ObjectValidation.ValidationItem("Invalid Protection Class", LocationProtectionClass));
                                }
                                break;

                            default: break;
                        }
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