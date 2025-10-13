using IFM.DataServicesCore.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using APIRequests = IFM.DataServices.API.RequestObjects;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public class UserInfo
    {
        private const string location = "IFM.DataServicesCore.BusinessLogic.Diamond.UserInfo";
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserLoginDomain { get; set; }
        public int LegacyUserId { get; set; }

        public void GetUserInfo(string UsernameToLookup)
        {
            if (UsernameToLookup.HasValue())
            {
                using (var sqlSO = new SQLselectObject(AppConfig.ConnDiamond))
                {
                    sqlSO.queryOrStoredProc = "select U.users_id, U.login_domain from Users as U with (nolock) where U.login_name = '" + UsernameToLookup + "'";

                    var dr = sqlSO.GetDataReader();
                    if (dr != null && dr.HasRows)
                    {
                        dr.Read();
                        UserId = dr["users_id"].TryToGetInt32();
                        UserLoginDomain = dr["login_domain"].ToString().Trim();
                        Username = UsernameToLookup;
                    }
                    else if (sqlSO.hasError)
                    {
                        //Error caught in db
                        IFMErrorLogging.LogIssue(sqlSO.errorMsg, location + ".GetUserInfo(string UsernametoLookup)");
                    }

                    if (UserId <= 0)
                    {
                        UserId = 0;
                        UserLoginDomain = "";
                        Username = "";
                    }
                }
            }
            GetLegacyUserInfo(UserId);
        }

        public void GetUserInfo(int UserIdToLookup)
        {
            if (UserIdToLookup > 0)
            {
                var t1 = Task.Factory.StartNew(() => {
                    using (var sqlSO = new SQLselectObject(AppConfig.ConnDiamond))
                    {
                        sqlSO.queryOrStoredProc = "select U.login_name, U.login_domain from Users as U with (nolock) where U.users_id = " + UserIdToLookup.ToString();

                        var dr = sqlSO.GetDataReader();
                        if (dr != null && dr.HasRows)
                        {
                            dr.Read();
                            UserId = UserIdToLookup;
                            UserLoginDomain = dr["login_domain"].ToString().Trim();
                            Username = dr["login_name"].ToString().Trim();
                        }
                        else if (sqlSO.hasError)
                        {
                            //Error caught in db
                            IFMErrorLogging.LogIssue(sqlSO.errorMsg, location + ".GetUserInfo(int UserIdToLookup)");
                        }

                        if (UserId <= 0)
                        {
                            UserId = 0;
                            UserLoginDomain = "";
                            Username = "";
                        }
                    }
                });

                var t2 = Task.Factory.StartNew(() => {
                    GetLegacyUserInfo(UserIdToLookup);
                });

                Task.WaitAll(t1, t2);
            }
        }

        public void GetLegacyUserInfo(int DiamondUserId)
        {
            if (DiamondUserId > 0)
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(AppConfig.Conn))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetLegacyUserInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmd.Parameters.AddWithValue("@DiamondUserId", DiamondUserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    LegacyUserId = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}