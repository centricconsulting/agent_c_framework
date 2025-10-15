using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFM.DataServicesCore.BusinessLogic.Diamond.Policy
{
    public class QuoteImage : PolicyImage
    {

        public QuoteImage(string policyNumber)
        {
            var ph = new QuoteHistoryLookup(policyNumber);
            if (ph.MostCurrentPolicyHistory != null)
            {
                GetImage(ph.MostCurrentPolicyHistory.PolicyId, ph.MostCurrentPolicyHistory.PolicyImageNum);
            }
        }


    }
}
