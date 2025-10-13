using IFM.DataServicesCore.BusinessLogic;
using System;
using System.Collections.Generic;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class CoverageCode : ModelBase
    {
        public Int32 CoverageCodeId { get; set; }
        public string Coveragecode { get; set; }
        public string CoverageType { get; set; }
        public string Description { get; set; }

        public CoverageCode() { }

        private CoverageCode(Int32 id, string code, string type, string descript)
        {
            this.CoverageCodeId = id;
            this.Coveragecode = code;
            this.CoverageType = type;
            this.Description = descript;
        }

        private static DateTime cacheExpire = DateTime.MinValue;
        private static Dictionary<Int32, CoverageCode> cache;
        private static object ThreadLock = new object();

        static internal CoverageCode Create(Int32 coverageCodeId)
        {
            lock (ThreadLock)
            {                
                if (cache == null)
                {
                    GetCoverageCodeStaticData();
                }
            }
            return cache[coverageCodeId];
        }

        private static void GetCoverageCodeStaticData()
        {
            if (cache == null || DateTime.Now > cacheExpire)
            {
                cacheExpire = DateTime.Now.AddMinutes(5);
                cache = new Dictionary<int, CoverageCode>();
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetCoverageCodes",conn))
                    {
                        using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    CoverageCode covCode = new CoverageCode(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                                    cache.Add(covCode.CoverageCodeId, covCode);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}