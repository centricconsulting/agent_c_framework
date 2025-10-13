using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;
using IDS = Insuresoft.DiamondServices;

namespace IFM.DataServicesCore.BusinessLogic.OnBase
{
    public class ProviderLookup
    {
        public string LoadProviderFein(string providerName)
        {
            return GetTaxNumber(providerName);            
        }

        private string GetTaxNumber(string providerName)
        {

            using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetProviderTaxNumber", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@providerName", providerName);                    

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            return reader.GetString(1);
                        }                        
                    }
                }
                return null;
            }
        }


    }
}
