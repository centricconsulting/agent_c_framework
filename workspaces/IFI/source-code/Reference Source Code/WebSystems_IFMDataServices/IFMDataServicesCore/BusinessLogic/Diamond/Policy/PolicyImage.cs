using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.BusinessLogic.Diamond.Policy
{
    public class PolicyImage
    {

        private DCO.Policy.Image _image;
        public DCO.Policy.Image Image { get { return _image; } }

        public PolicyImage()
        {
        }

        public PolicyImage(string policyNumber)
        {
            var ph = new PolicyHistoryLookup(policyNumber);
            var history = ph?.MostCurrentPolicyHistory ?? ph?.FuturePolicyHistory ?? null;
            if (history != null)
            {
                GetImage(history.PolicyId,history.PolicyImageNum);
            }
        }

        public PolicyImage(int policyId, int imageNumber)
        {
            this.GetImage(policyId, imageNumber);
        }

        protected void GetImage(int polId, int imageNum)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PolicyService.LoadImage())
                {
                    DS.RequestData.PolicyId = polId;
                    DS.RequestData.ImageNumber = imageNum;
                    var image = DS.Invoke()?.DiamondResponse?.ResponseData?.Image;
                    if (image != null)
                    {
                        this._image = image;
                    }
#if DEBUG
                    else
                    {
                        Debugger.Break();
                    }
#endif
                }
            }
#if DEBUG
            else
            {
                Debugger.Break();
            }
#endif
        }

        public DCO.Policy.QuickLookup GetPolicyIDAndImageNumByPolicyNumber(string policyNum)
        {
            var policy = new DCO.Policy.QuickLookup();
            if(this._image?.PolicyNumber.Equals(policyNum, StringComparison.OrdinalIgnoreCase) == true && this._image?.PolicyImageNum > 0)
            {
                policy.PolicyImageNum = _image.PolicyImageNum;
                policy.PolicyId = _image.PolicyId;
            }
            else
            {
                if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
                {
                    using (var DS = Insuresoft.DiamondServices.PolicyService.GetPolicyIdAndNumForPolicyNumber())
                    {
                        DS.RequestData.PolicyNumber = policyNum;
                        var invoke = DS.Invoke();
                        var diamondResponse = invoke.DiamondResponse;
                        if (diamondResponse?.ResponseData?.Policies?.Count > 0)
                        {
                            policy = diamondResponse.ResponseData.Policies[0];
                        }
                    }
                }
            }

            return policy;
        }
    }
}
