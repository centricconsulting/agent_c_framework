using IFM.DataServicesCore.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using IFM.PrimitiveExtensions;
using Dapper;
using System.Linq;


#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class UnSweptPayment : ModelBase
    {
        public string PaymentType { get; set; }
        public double PaymentAmount { get; set; }
        public string PaymentDate { get; set; }

        // here for serialization
        public UnSweptPayment()
        {
        }

        public UnSweptPayment(string paymenttype, double paymentamount, string paymentdate)
        {
            this.PaymentType = paymenttype;
            this.PaymentAmount = paymentamount;
            this.PaymentDate = paymentdate;
        }

        public override string ToString()
        {
            return $"{PaymentAmount:C2} - {PaymentDate} ({PaymentType})";
        }

        /// <summary>
        /// Returns payments not yet swept by the payment sweeper for both mainframe and diamond.
        /// </summary>
        /// <param name="polnum"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<UnSweptPayment> GetPendingPaymentList(string polnum)
        {
            List<UnSweptPayment> unSwept = new List<UnSweptPayment>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConfig.ConnDiamondReports))
                {
                    using (SqlCommand cmdSql = new SqlCommand("dbo.usp_PolicyPayments", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        cmdSql.CommandType = CommandType.StoredProcedure;
                        cmdSql.Parameters.Add(new System.Data.SqlClient.SqlParameter("@polnum", polnum));
                        conn.Open();
                        System.Data.SqlClient.SqlDataReader dr = cmdSql.ExecuteReader();
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                if (dr["PaymentStatus"].TryToGetString() == "Pending")
                                {
                                    string paymentType = "";
                                    if (dr["billingcashinsource_id"].TryToGetInt32().EqualsAny(10030))
                                    {
                                        paymentType = "eCheck";
                                    }
                                    else
                                    {
                                        paymentType = "Credit Card";
                                    }
                                    unSwept.Add(new UnSweptPayment(paymentType, Convert.ToDouble(dr.GetDecimal(2)), dr.GetDateTime(3).ToShortDateString()));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#else
                
#endif
            }            

            return unSwept;
        }
    }
}