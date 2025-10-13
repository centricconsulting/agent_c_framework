using IFM.DataServicesCore.BusinessLogic;
using System;
using System.Collections.Generic;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class CoverageLimit : ModelBase
    {
        public Int32 CoverageLimitId { get; set; }
        public string Description { get; set; }
        public double PerPersonLimit { get; set; }
        public double PerOccurrenceLimit { get; set; }
        public double Deductible { get; set; }

        public bool IsDeductible
        {
            get { return Deductible > 0; }
            set { }
        }

        public bool IsNonTypical
        {
            // like percentages or 'Optional' or '1 -4 Weeks' ect when True only the Description is valid
            get { return this.PerPersonLimit == 0 && this.PerOccurrenceLimit == 0 && this.Deductible == 0; }
            set { }
        }

        public CoverageLimit() { }

        private CoverageLimit(Int32 id, string desc, double PP, double PO, double deduc)
        {
            this.CoverageLimitId = id;
            this.Description = desc;
            this.PerPersonLimit = PP;
            this.PerOccurrenceLimit = PO;
            this.Deductible = deduc;
        }

        static internal CoverageLimit Create(Int32 CovLimitId)
        {
            lock (ThreadLock)
            {
                if (cache == null)
                {
                    GetCoverageLimitStaticData();
                }
            }
            return cache[CovLimitId];
        }

        private static DateTime cacheExpire = DateTime.MinValue;
        private static Dictionary<Int32, CoverageLimit> cache;
        private static object ThreadLock = new object();

        private static void GetCoverageLimitStaticData()
        {
            if (cache == null || DateTime.Now > cacheExpire)
            {
                cacheExpire = DateTime.Now.AddMinutes(5);
                cache = new Dictionary<int, CoverageLimit>();
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
                {
                    conn.Open();
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetCoverageLimits", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    CoverageLimit covLimit = new CoverageLimit(reader.GetInt32(0), reader.GetString(1), Convert.ToDouble(reader.GetDecimal(2)), Convert.ToDouble(reader.GetDecimal(3)), Convert.ToDouble(reader.GetDecimal(4)));
                                    cache.Add(covLimit.CoverageLimitId, covLimit);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}