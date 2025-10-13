using System;
using System.Data;
using IFM.PrimitiveExtensions;
using QQHC = QuickQuote.CommonMethods.QuickQuoteHelperClass;
using QQOB = QuickQuote.CommonObjects.QuickQuoteObject;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class ModelBase
    {
        protected static IDbConnection OpenConnection(string connString)
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString);
            conn.Open();
            return conn;
        }

        protected static string GetStaticDataTextForValue(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string id)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataTextForValue(classType, propertyType, id);
        }

        protected static string GetRelatedStaticDataValueForOptionValue(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string relatedOptionValue, QQHC.QuickQuotePropertyName relatedPropertyType,
            QQOB.QuickQuoteLobType lobType = QQOB.QuickQuoteLobType.None,
            QQHC.PersOrComm PersOrComm = QQHC.PersOrComm.None)
        {
            return IFM.StaticData.StaticDataHelper.GetRelatedStaticDataValueForOptionValue(classType, propertyType, relatedOptionValue, relatedPropertyType, lobType, PersOrComm);
        }

        protected static string GetStaticDataValueForText2(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataValueForText2(classType, propertyType, text);
        }

        protected static string GetStaticDataText2ForValue(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string value)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataText2ForValue(classType, propertyType, value.ToString());
        }

        protected static string GetStaticDataValueForText(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataValueForText(classType, propertyType, text);
        }

        protected static int GetStaticDataValueForTextAsInt(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataValueForTextAsInt(classType, propertyType, text);
        }

        protected static string GetStaticDataTextForText2(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, int id)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataTextForValue(classType, propertyType, id.ToString());
        }

        protected static int GetStaticDataValueForText2AsInt(QQHC.QuickQuoteClassName classType, QQHC.QuickQuotePropertyName propertyType, string text)
        {
            return IFM.StaticData.StaticDataHelper.GetStaticDataValueForTextAsInt(classType, propertyType, text);
        }
    }
}