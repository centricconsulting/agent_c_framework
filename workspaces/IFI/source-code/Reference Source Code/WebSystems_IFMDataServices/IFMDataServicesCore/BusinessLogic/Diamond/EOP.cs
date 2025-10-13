using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDS = Insuresoft.DiamondServices;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.Diamond
{
    public class EOP
    {
        public static bool? IsEOPRunning()
        {
            using (var DS = IDS.EOPProcessService.IsEndOfPeriodRunning())
            {
                return DS.Invoke()?.DiamondResponse?.ResponseData?.Result;
            }
        }

        public static DCO.EndOfPeriod.ExecutionLogDetail EOPStatusDetails()
        {
            using (var DS = IDS.EOPProcessService.GetProcessStatus())
            {
                var r = DS.Invoke()?.DiamondResponse;
                return r?.ResponseData?.ExecutionLogDetailEntry;
            }
        }

        public static bool? IsRunningEOM(DCO.EndOfPeriod.ExecutionLogDetail status)
        {
            // status.ConfigurationId Day = 1, Month = 2, Year = 3, Day(Everyday) = 4
            return status?.ConfigurationId == 2;
        }

        public static bool? IsRunningEOY(DCO.EndOfPeriod.ExecutionLogDetail status)
        {
            // status.ConfigurationId Day = 1, Month = 2, Year = 3, Day(Everyday) = 4
            return status?.ConfigurationId == 3;
        }

        public static bool? IsRunningPaymentBlockingPeriod()
        {
            var isEOPRunning = IsEOPRunning();
            if (isEOPRunning ?? false)
            {
                return IsRunningPaymentBlockingPeriod(EOPStatusDetails());
            }

            return null;
        }

        public static bool? IsRunningPaymentBlockingPeriod(DCO.EndOfPeriod.ExecutionLogDetail status)
        {
            if (status != null)
            {
                bool? eom = IsRunningEOM(status);
                bool? eoy = IsRunningEOY(status);
                return (eom ?? false) || (eoy ?? false);
            }
            return null;
        }



    }
}
