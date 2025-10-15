using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public class DiamondUsers
    {
        public static List<int> GetAllActiveAgencyUsers(string agencyCode)
        {
            var results = new List<int>();
            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetDiamondUsersByAgencyCode", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@agencyCode", agencyCode);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                results.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            return results;
        }

    }
}
