using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM.PrimitiveExtensions;
using QQHC = QuickQuote.CommonMethods.QuickQuoteHelperClass;
using QQOB = QuickQuote.CommonObjects.QuickQuoteObject;

namespace IFM.DataServicesCore.CommonObjects.IFM.StaticData
{
    public static class StaticDataHelper
    {
        public static string GetStaticDataTextForValue(QQHC.QuickQuoteClassName classType,
            QQHC.QuickQuotePropertyName propertyType,
            string id)
        {
            var qqhelper = new QQHC();
            return qqhelper.GetStaticDataTextForValue(classType, propertyType, id);
        }

        public static string GetRelatedStaticDataValueForOptionValue(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string relatedOptionValue, QQHC.QuickQuotePropertyName relatedPropertyType,
            QQOB.QuickQuoteLobType lobType = QQOB.QuickQuoteLobType.None,
            QQHC.PersOrComm PersOrComm = QQHC.PersOrComm.None)
        {
            var qqhelper = new QQHC();
            return qqhelper.GetRelatedStaticDataValueForOptionValue(classType, propertyType, relatedOptionValue, relatedPropertyType, lobType, PersOrComm);
        }

        public static string GetStaticDataOptionValueForValue(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string OptionName, string value, QQOB.QuickQuoteLobType lobType = QQOB.QuickQuoteLobType.None, QQHC.PersOrComm PersOrComm = QQHC.PersOrComm.None)
        {
            var qqhelper = new QQHC();
            var myOptions = qqhelper.GetStaticDataOptions(classType, propertyType, lobType, PersOrComm);
            if(myOptions?.Count > 0)
            {
                var myOption = myOptions.Find(x => x.Value == value);
                if(myOption != null)
                {
                    var myElement = myOption.MiscellaneousElements.Find(x => x.nvp_name.StringsAreEqual(OptionName));
                    if(myElement != null)
                    {
                        return myElement.nvp_value;
                    }
                }
            }
            return "";
        }

        public static string GetStaticDataText2ForValue(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string value)
        {
            return GetStaticDataOptionValueForValue(classType, propertyType, "Text2", value);
        }

        public static int GetStaticDataText2ForValueAsInt(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            var myIntString =  GetStaticDataOptionValueForValue(classType, propertyType, "", text);
            if (myIntString.IsNumeric())
                return int.Parse(myIntString);
            else
                return 0;
        }

        public static string GetStaticDataValueForText(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            var qqhelper = new QQHC();
            return qqhelper.GetStaticDataValueForText(classType, propertyType, text);
        }

        public static int GetStaticDataValueForTextAsInt(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            string myIntString = GetStaticDataValueForText(classType, propertyType, text);
            if (myIntString.IsNumeric())
                return int.Parse(myIntString);
            else
                return 0;
        }

        public static string GetStaticDataValueForText2(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            var qqhelper = new QQHC();
            QQOB.QuickQuoteLobType lobType = QQOB.QuickQuoteLobType.None;
            QQHC.PersOrComm persOrComm = QQHC.PersOrComm.None;
            QQHC.QuickQuotePropertyName relatedPropertyName = QQHC.QuickQuotePropertyName.Text2;
            return qqhelper.GetRelatedStaticDataValueForOptionValue(classType, propertyType, text, relatedPropertyName, lobType, persOrComm);
        }

        public static int GetStaticDataValueForText2AsInt(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            string myIntString = GetStaticDataValueForText2(classType, propertyType, text);
            if (myIntString.IsNumeric())
                return int.Parse(myIntString);
            else
                return 0;
        }
    }
}
