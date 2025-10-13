using System.Linq;

namespace IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
{
    public class DriverListValidator
    {
        //This is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        public const string ValidationListID = "{1D6F4568-B84D-4616-A1B8-2A33B2CB6D86}";

        public const string QuoteIsNull = "{B4004E15-A718-4F4E-9A85-47982B502F66}";

        public const string DriverListNoDrivers = "{51DF2C18-C894-4410-A54F-16D851385A7B}";
        public const string DriverListNoRatedDrivers = "{E4C10EE8-310C-43D3-BF38-D6FF87058D0A}";

        public static Validation.ObjectValidation.ValidationItemList ValidateDriverList(QuickQuote.CommonObjects.QuickQuoteObject quote, ValidationItem.ValidationType valType)
        {
            Validation.ObjectValidation.ValidationItemList valList = new ValidationItemList(ValidationListID);

            if (quote != null)
            {
                if (quote.Drivers == null || quote.Drivers.Any() == false)
                {
                    valList.Add(new ObjectValidation.ValidationItem("No driver information.", DriverListNoDrivers));
                }
                else
                {
                    if (quote.Drivers != null && quote.Drivers.Any())
                    {
                        var hasRatedDrivers = (from QuickQuote.CommonObjects.QuickQuoteDriver driver in quote.Drivers where driver.DriverExcludeTypeId == "1" select driver).Any();
                        if (hasRatedDrivers == false)
                            valList.Add(new ObjectValidation.ValidationItem("No rated drivers.", DriverListNoRatedDrivers));
                    }
                }
            }
            else
            {
                valList.Add(new ObjectValidation.ValidationItem("Quote is null.", QuoteIsNull));
            }
            return valList;
        }
    }
}