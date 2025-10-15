using IFM.DataServicesCore.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using IFM.PrimitiveExtensions;
using System.Web;
using System.Threading.Tasks;

#if DEBUG

using System.Diagnostics;

#endif

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [Serializable]
    public class AccountRegistedPolicy
    {
        /// <summary>
        /// This policy was created do to it being registered with the member portal account. This is opposed to it being added dynamically due to being part of an account bill or the like.
        /// </summary>
        /// <returns></returns>
        private const string location = "IFM.DataServicesCore.BusinessLogic.OMP.AccountRegisteredPolicy";

        public bool IsRegisteredPolicy { get; set; }

        public string AccountNumber { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyNickName { get; set; }
        public bool HasInforceImage
        {
            //From p In policyHistory Select p Order By p.Status Ascending, p.TEffDate Descending
            get { return PolicyHistories != null && PolicyHistories.Any() ? (from h in PolicyHistories where h.PolicyStatusCodeId == 1 select h).Any() : false; }
            set { }
        }

        public bool HasFuture
        {
            //From p In policyHistory Select p Order By p.Status Ascending, p.TEffDate Descending
            get {
                // if we have already overwritten 'CurrentImage' with a FutureImage then check for future image otherwise return false
                if (CurrentImage != null && CurrentImage.PolicyStatusId != 2)
                    return PolicyHistories != null && PolicyHistories.Any() ? (from h in PolicyHistories where h.PolicyStatusCodeId == 2 select h).Any() : false;
                return false;
            }
            set { }
        }

        public bool IsFuturePayable
        {
            get
            {
                return HasFuture && FutureImage != null && FutureImage.BillingInformation != null && FutureImage.BillingInformation.IsPayable;
            }
            set { }
        }
        /// <summary>
        /// Returns the InForce or latest History PolicyHistory item - excludes Future
        /// </summary>
        /// <returns></returns>
        public PolicyHistory MostCurrentPolicyHistory
        {
            //1 = InForce, 2 = Future, 3 = History, 4 = Pending, 5 = Renewal Offer, 12 = Quote, 13 = Archived Quote
            get {

                return PolicyHistories != null && PolicyHistories.Any()
                    ? (from p in PolicyHistories where p.PolicyStatusCodeId != 2 orderby p.PolicyStatusCodeId ascending, p.EffectiveDate descending select p).FirstOrDefault()
                    : null; }
            set { }
        }

        public PolicyHistory FuturePolicyHistory
        {
            //1 = InForce, 2 = Future, 3 = History, 4 = Pending, 5 = Renewal Offer, 12 = Quote
            get { return FuturePolicyHistories != null && FuturePolicyHistories.Any() ? (from p in FuturePolicyHistories  orderby p.EffectiveDate descending select p).FirstOrDefault() : null; }
            set { }
        }

        public List<PolicyHistory> FuturePolicyHistories
        {
            //1 = InForce, 2 = Future, 3 = History, 4 = Pending, 5 = Renewal Offer, 12 = Quote
            get { return PolicyHistories != null && PolicyHistories.Any() ? (from p in PolicyHistories where p.PolicyStatusCodeId == 2 orderby p.EffectiveDate descending select p).ToList() : null; }
            set { }
        }

        public List<Int32> AllPolicyIds
        {
            get { return PolicyHistories != null && PolicyHistories.Any() ? (from p in PolicyHistories select p.PolicyId).Distinct().ToList() : null; }
            set { }
        }

        public List<PolicyHistory> PolicyHistories { get; set; }

        public BasicPolicyInformation CurrentImage { get; set; }
        public BasicPolicyInformation FutureImage { get; set; }
        public List<BasicPolicyInformation> FutureImageList { get; set; }
        public AccountRegistedPolicy() { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="nickName"></param>
        /// <param name="isRegistedPolicy">Is this policy actually registed as part of the members registered policues or is it dynamically being added due to an Account Bill or the like.</param>
        internal AccountRegistedPolicy(string policyNumber, string nickName, bool isRegistedPolicy)
        {
            if (policyNumber.IsAccountBillNumber())
            {
                this.AccountNumber = policyNumber;

                var policyAPI = new global::IFM.PolicyAPIModels.Request.AccountBillInquiry(AppConfig.PolicyInquiryAPIEndpoint);
                var response = policyAPI.GetAccountBillInfoByAccountNumber(this.AccountNumber); 
                if (response != null)
                {
                    this.PolicyNumber = response.ResponseData.PoliciesInAccount.FirstOrDefault().PolicyNumber;
                }
            }
            else
            {
                this.PolicyNumber = string.IsNullOrWhiteSpace(policyNumber) == false ? policyNumber.ToUpper().Trim() : string.Empty;
            }
                
            if (this.PolicyNumber.IsPolicyNumber())
            {
                this.PolicyNickName = string.IsNullOrWhiteSpace(nickName) == false ? nickName : string.Empty;
                this.IsRegisteredPolicy = isRegistedPolicy;

                SetPolicyHistories();

                HttpContext ctx = HttpContext.Current;

                var t1 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        HttpContext.Current = ctx;
                        if (this.MostCurrentPolicyHistory != null)
                        {
                            CurrentImage = this.BasicPolicyInformation(this.MostCurrentPolicyHistory);
                        }
                    }
                    catch(Exception ex)
                    {
                        IFMErrorLogging.LogException(ex, $"{location} - Current Image");
                    }
                    
                });

                var t2 = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        HttpContext.Current = ctx;
                       
                        FutureImageList = new List<BasicPolicyInformation>();
                        foreach (var futurePolicyHistory in this.FuturePolicyHistories)
                        {
                            if (futurePolicyHistory != null)
                            {
                                FutureImageList.Add(this.BasicPolicyInformation(futurePolicyHistory));
                            }
                        }
                        if (FutureImageList != null && FutureImageList.Any())
                        {
                            FutureImage = FutureImageList.OrderByDescending(fi => fi.PolicyImageNum).FirstOrDefault();
                            if (!string.IsNullOrEmpty(FutureImage.ReWrittenTo))
                            {
                                FutureImage = FutureImageList.Where(fi=>fi.PolicyNumber == FutureImage.ReWrittenTo).OrderByDescending(fi => fi.PolicyImageNum).FirstOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IFMErrorLogging.LogException(ex, $"{location} - Future Image and Future Image List");
                    }
                });

                try
                {
                    Task.WaitAll(t1, t2);
                }
                catch(Exception ex)
                {
                    IFMErrorLogging.LogException(ex, $"{location} - Current and Future TaskWait.");
                }

                //Just make current image equal to the future image when no current image exists
                if (CurrentImage == null && FutureImage != null)
                {
                    CurrentImage = FutureImage;
                    FutureImage = null;
                }
            }
        }

        private BasicPolicyInformation BasicPolicyInformation(PolicyHistory h)
        {
            if (h != null)
            {

                if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
                {
                    using (var DS = Insuresoft.DiamondServices.PolicyService.LoadImage())
                    {
                        DS.RequestData.PolicyId = h.PolicyId;
                        DS.RequestData.ImageNumber = h.PolicyImageNumber;
                        var image = DS.Invoke()?.DiamondResponse?.ResponseData?.Image;
                        if (image != null)
                        {
                            return new CommonObjects.OMP.BasicPolicyInformation(image, AllPolicyIds);
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
            return null;
        }

        public MinimalPolicyInformation MinimalPolicyInformation(PolicyHistory h)
        {


            if (h != null)
            {

                if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
                {
                    using (var DS = Insuresoft.DiamondServices.PolicyService.LoadImage())
                    {
                        DS.RequestData.PolicyId = h.PolicyId;
                        DS.RequestData.ImageNumber = h.PolicyImageNumber;
                        var image = DS.Invoke()?.DiamondResponse?.ResponseData?.Image;
                        if (image != null)
                        {
                            return new CommonObjects.OMP.MinimalPolicyInformation(image, AllPolicyIds);
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
            return null;

        }

        private void SetPolicyHistories()
        {
            PolicyHistories = DataServicesCore.BusinessLogic.OMP.PolicyAccess.GetPolicyHistories(PolicyNumber);
            //this.PolicyNickName = "Getting policy histories."; //Why would we want this?
        }

        public static int[] UndesireablePolicyStatusIds()
        {
            return new int[] { 4, 12, 13 }; //pending, quote, archived quote
        }

        public override string ToString()
        {
            string txt = $"PolNum: {this.PolicyNumber} HasInforceImage: {this.HasInforceImage} IsRegistered: {this.IsRegisteredPolicy}";

            if (this.CurrentImage?.BillingInformation != null)
            {
                txt += $"{this.CurrentImage.BillingInformation}";
            }
            return txt;
        }

    }
}