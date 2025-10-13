using System.Data;

namespace IFM.DataServicesCore.CommonObjects.OnBase
{
    [System.Serializable]
    public class ModelBase
    {
        private QuickQuote.CommonMethods.QuickQuoteHelperClass qqhelper { get { return new QuickQuote.CommonMethods.QuickQuoteHelperClass(); } }

        protected static IDbConnection OpenConnection(string connString)
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString);
            conn.Open();
            return conn;
        }

        protected string GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName classType,
           QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName propertyType,
           string id)
        {
            return qqhelper.GetStaticDataTextForValue(classType, propertyType, id);
        }

        protected string GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName classType,
            QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName propertyType,
            int id)
        {
            return GetStaticDataTextForValue(classType, propertyType, id.ToString());
        }
    }
}