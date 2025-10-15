using System.Data;
using IFM.PrimitiveExtensions;

namespace IFM.DataServicesCore.BusinessLogic
{
    public class BusinessLogicBase
    {
        protected static IDbConnection OpenConnection(string connString)
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connString);
            conn.Open();
            return conn;
        }
    }
}