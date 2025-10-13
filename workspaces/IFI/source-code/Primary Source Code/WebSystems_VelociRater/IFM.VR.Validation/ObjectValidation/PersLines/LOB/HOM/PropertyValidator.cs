using QuickQuote.CommonMethods;
using System;
using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
{
    public class PropertyValidator
    {
        public const string ValidationListID = "{9A617FFD-1584-4E0B-9994-E91E1B5D842D}";

        public const string QuoteIsNull = "{EB70B503-B31B-4E73-A806-CE96A3779FC0}";
        public const string NoLocations = "{DE3806F5-B5A6-4D88-A79D-58CAC3EFFED0}";

        public const string FirstWrittenDate = "{7D81D362-BDA0-41F0-BD89-3DC6C8B714B1}";

        public static Validation.ObjectValidation.ValidationItemList ValidateHOMLocation(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            QuickQuoteHelperClass QQHelper = new QuickQuoteHelperClass();

            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);
            valList.AddBreadCrum(ValidationBreadCrum.BCType.LocationIndex, "0");

            if (quote != null)
            {
                if (quote.Locations != null && quote.Locations.Any())
                {
                    var MyLocation = quote.Locations[0];

                    // 12-1-14 Matt A
                    switch (quote.LobType)
                    {
                        case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal:
                            if (VRGeneralValidations.Val_IsValidDate(quote.FirstWrittenDate, valList, FirstWrittenDate, "First Written Date"))
                            {
                                if (Microsoft.VisualBasic.Information.IsDate(quote.EffectiveDate))
                                    VRGeneralValidations.Val_IsDateInRange(quote.FirstWrittenDate, valList, FirstWrittenDate, "First Written Date", "01/01/1800", quote.EffectiveDate);
                                else
                                    VRGeneralValidations.Val_IsDateInRange(quote.FirstWrittenDate, valList, FirstWrittenDate, "First Written Date", "01/01/1800", DateTime.Now.ToShortDateString());
                            }
                            break;

                        default:
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