using System;

using System.Data;
#if DEBUG
using System.Diagnostics;
#endif

using Dapper;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public class SystemDate : BusinessLogicBase
    {

        DateTime? systemDate = null;

        public DateTime GetSystemDate()
        {
            if (!systemDate.HasValue)
            {
                try
                {
                    using (IDbConnection conn = OpenConnection(AppConfig.ConnDiamondReports))
                    {
                        systemDate = DateTime.Parse(conn.ExecuteScalar<DateTime>("usp_GetDiamondSystemDate", commandType: CommandType.StoredProcedure).ToString()); //why can't it return a date?
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#else
                global::IFM.IFMErrorLogging.LogException(ex,"IFMDATASERVICES -> SystemDate.cs -> Function GetSystemDate - Error getting system date.");
#endif
                }
            }
            return systemDate.Value;
        }

        public void ForceRecheckOnNextGet()
        {
            systemDate = null;
        }


    }
}
