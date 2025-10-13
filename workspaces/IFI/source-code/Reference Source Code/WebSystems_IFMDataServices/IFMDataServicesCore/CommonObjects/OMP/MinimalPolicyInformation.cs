using IFM.DataServicesCore.BusinessLogic;
using IFM.PrimitiveExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DCO = Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    [System.Serializable]
    public class MinimalPolicyInformation : ModelBase
    {
        public Int32 ClientId { get; set; }
        public string PolicyNumber { get; set; }
        public Int32 PolicyId { get; set; }
        public Int32 PolicyImageNum { get; set; }

        public string LobName { get; set; }
        public Int32 LobId { get; set; }
        public DateTime EffectiveDate { get; set; }

        public DateTime TermEndDate { get; set; }
        public Int32 PolicyStatusId { get; set; }

        public string PolicyStatus { get; set; }

        public string PolicyholderName { get; set; }

        public Agent Agency { get; set; }
        protected internal BillingInformation _BillingInformation { get; set; }

        public double OutstandingBalance { get; set; }

        public double PayInFullBalance { get; set; }

        public string ReWrittenTo { get; set; }

        public string ReWrittenFrom { get; set; }

        public List<Int32> AllPolicyIds { get; set; }
        public int VersionId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public MinimalPolicyInformation() { }
        public MinimalPolicyInformation(DCO.Policy.Image image, List<int> AllPolicyIds)
        {
            this.AllPolicyIds = AllPolicyIds;
            this.PolicyNumber = image.PolicyNumber;
            this.PolicyId = image.PolicyId;
            this.PolicyImageNum = image.PolicyImageNum;
            this.ClientId = image.Policy.ClientId;
            this.VersionId = image.VersionId;

            var versionInfo = SetInfoByVersion(image.VersionId);
            if (versionInfo != null)
            {
                this.LobId = versionInfo.LobId;
                this.LobName = versionInfo.LobName;
                this.CompanyId = versionInfo.CompanyId;
                this.CompanyName = versionInfo.CompanyName;
            }

            this.EffectiveDate = image.EffectiveDate;
            this.TermEndDate = image.ExpirationDate;

            this.PolicyStatusId = image.PolicyStatusCodeId;
            this.PolicyStatus = GetPolicyStatusFromId(this.PolicyStatusId);

            this._BillingInformation = new BillingInformation(image, AllPolicyIds, LobId);
            this.OutstandingBalance = (this._BillingInformation != null) ? this._BillingInformation.OutstandingBalance : 0;
            this.PayInFullBalance = (this._BillingInformation != null) ? this._BillingInformation.PayInFullAmount : 0;

            this.Agency = new Agent(image);

            if (image.PolicyHolder?.Name != null)
            {
                this.PolicyholderName = image.PolicyHolder?.Name?.DisplayName;

                if (image.PolicyHolder2?.Name != null && string.IsNullOrWhiteSpace(image.PolicyHolder2?.Name.ToString()) == false)
                {
                    this.PolicyholderName = $"{this.PolicyholderName.Trim()}/{image.PolicyHolder2.Name.ToString()}";
                }
            }

            var t1 = Task.Factory.StartNew(() =>
            {
                GetReWrittenToPolicy();
            });
            var t2 = Task.Factory.StartNew(() =>
            {
                GetReWrittenFromPolicy();
            });
            Task.WaitAll(t1, t2);

        }

        public override string ToString()
        {
            return this.PolicyNumber + " " + this.LobName + " " + EffectiveDate.ToShortDateString();
        }

        private void GetReWrittenToPolicy()
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetRewrittenPolicyInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@policyNumber", PolicyNumber);
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var rTo = reader["RewrittenToPolicyNumber"].TryToGetString().ToUpper().Trim();
                            this.ReWrittenTo = string.IsNullOrWhiteSpace(rTo) ? null : rTo; //null out of empty
                        }
                    }
                }
            }
        }

        private void GetReWrittenFromPolicy()
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetRewrittenPolicyInfo", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@policyNumber", PolicyNumber);
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            var rFRom = reader["RewrittenFromPolicyNumber"].TryToGetString().ToUpper().Trim();
                            this.ReWrittenFrom = string.IsNullOrWhiteSpace(rFRom) ? null : rFRom; //null out of empty
                        }
                    }
                }
            }
        }

        //private void SetLob(Int32 versionId, ref Int32 lobId, ref string lobName)
        //{
        //    // could use the static data file if it was updated
        //    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
        //    {
        //        conn.Open();
        //        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetLobInfoByVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure })
        //        {
        //            // cmd.CommandText = "SELECT L.lob_id,L.lobname FROM  dbo.[Version] V INNER JOIN dbo.CompanyStateLob CSL ON V.companystatelob_id = CSL.companystatelob_id INNER JOIN dbo.CompanyLob CL ON CSL.companylob_id = CL.companylob_id inner Join [Diamond].[dbo].[Lob] L On l.lob_id = cl.lob_id WHERE V.version_id = @versionId";
        //            cmd.Parameters.AddWithValue("@versionId", versionId);
        //            using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    reader.Read();
        //                    lobId = reader["lob_id"].TryToGetInt32();
        //                    lobName = reader["lobname"].TryToGetString();
        //                }
        //            }
        //        }
        //    }
        //}

        private Diamond.VersionInfo SetInfoByVersion(Int32 versionId)
        {
            var versionInfo = new Diamond.VersionInfo();
            // could use the static data file if it was updated
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(AppConfig.ConnDiamondReports))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("dbo.usp_GetLobInfoByVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@versionId", versionId);
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            versionInfo.LobId = reader["lob_id"].TryToGetInt32();
                            versionInfo.LobName = reader["lobname"].TryToGetString();
                            versionInfo.CompanyId = reader["company_id"].TryToGetInt32();
                            versionInfo.CompanyName = reader["company_name"].TryToGetString();
                        }
                    }
                }
            }
            return versionInfo;
        }

        public static string GetPolicyStatusFromId(int PolicyStatusId)
        {
            switch (PolicyStatusId)
            {
                case 1:
                    return "In-Force";

                case 2:
                    return "Future";

                case 3:
                    return "History";

                case 4:
                    return "Pending";

                default:
                    break;
            }
            return "Unknown/Canceled";
        }
    }

    [System.Serializable]
    public class BasicPolicyInformation : MinimalPolicyInformation
    {
        public List<Policyholder> Policyholders { get; set; }

        public BillingInformation BillingInformation { get; set; }

        public EndorsementInformation EndorsementInformation { get; set; }

        public bool HasMultiplePolicIds
        {
            get { return AllPolicyIds == null ? false : AllPolicyIds.Count > 1; }
            set { }
        }

        public List<CoverageBase> PolicyCoverages { get; set; }

        public List<PrintForm> PrintForms { get; set; }


        /// <summary>
        /// Provides print for all the policyids associated with this image.
        /// </summary>
        /// <returns></returns>
        internal List<PrintForm> PrintFormsHistory { get; set; }

        public List<PrintForm> Declarations { get; set; }


        public List<Applicant> Applicants { get; set; }

        public List<AccountLinkedPolicy> LinkedAccountPolicies { get; set; }

        public object SupplementalInformation { get; set; }

        public List<OMP.ClaimInformation> Claims { get; set; }

        public bool HasAutoEnhancement { get; set; }
        public bool HasAutoEnhancementPlus { get; set; }
        public List<PackagePart> PackageParts { get; set; }
        public BasicPolicyInformation() { }

        internal BasicPolicyInformation(DCO.Policy.Image image, List<Int32> AllPolicyIds)
            : base(image, AllPolicyIds)
        {
            BillingInformation = _BillingInformation; // here to allow serialization
            PrintForms = BusinessLogic.OMP.PrintForms.GetForms(this.PolicyId).ToList();

            EndorsementInformation = new EndorsementInformation(image);

            if (image.LOB.PolicyLevel.Coverages.IsLoaded())
            {
                foreach (var cov in image.LOB.PolicyLevel.Coverages)
                {
                    if (PolicyCoverages == null) { PolicyCoverages = new List<CoverageBase>(); };
                    var myCov = new CoverageBase(cov);
                    PolicyCoverages.Add(myCov);
                    CheckIfCoverageIsNeededForClassProperty(myCov);
                }
            }

            if (HasMultiplePolicIds)
            {
                PrintFormsHistory = new List<PrintForm>();

                HttpContext ctx = HttpContext.Current;
                var printCollection = new Dictionary<int, List<PrintForm>>();
                Parallel.For(0, AllPolicyIds.Count,
                   index =>
                   {
                       HttpContext.Current = ctx;
                       var pId = AllPolicyIds[index];
                       //don't re-pull current print forms you already have those
                       if (pId != this.PolicyId)
                       {
                           printCollection.Add(index, BusinessLogic.OMP.PrintForms.GetForms(pId).ToList());
                       }
                       else
                       {
                           printCollection.Add(index, this.PrintForms);
                       }
                   });
                foreach (var list in from p in printCollection orderby p.Key ascending select p)
                {
                    PrintFormsHistory.AddRange(list.Value);
                }

                //foreach (var pId in AllPolicyIds)
                //{
                //    //don't re-pull current print forms you already have those
                //    if (pId != this.PolicyId)
                //    {
                //        PrintFormsHistory.AddRange(BusinessLogic.OMP.PrintForms.GetForms(pId).ToList());
                //    }
                //    else
                //    {
                //        PrintFormsHistory.AddRange(this.PrintForms);
                //    }
                //}
                PrintForms = PrintFormsHistory;//Matt A 5-1-18
            }

            if (this.HasMultiplePolicIds && this.PrintFormsHistory != null)
            {
                Declarations = (from f in this.PrintFormsHistory where f.Description.ToLower().Contains("declaration") select f).ToList();
            }
            else
            {
                if (this.PrintForms != null)
                {
                    Declarations = (from f in this.PrintForms where f.Description.ToLower().Contains("declaration") select f).ToList();
                }
            }


            this.Policyholders = new List<Policyholder>();
            if (image.PolicyHolder != null)
            {
                this.Policyholders.Add(new Policyholder(image.PolicyHolder.Name, image.PolicyHolder.Address));
            }
            if (image.PolicyHolder2 != null && string.IsNullOrWhiteSpace(image.PolicyHolder2.Name.ToString()) == false)
            {
                this.Policyholders.Add(new Policyholder(image.PolicyHolder2.Name, image.PolicyHolder.Address));
            }

            if (image.LOB.RiskLevel.Applicants != null && image.LOB.RiskLevel.Applicants.Any())
            {
                this.Applicants = new List<Applicant>();
                foreach (var a in image.LOB.RiskLevel.Applicants)
                {
                    Applicants.Add(new Applicant(a));
                }
            }
            if (image.PackageParts != null && image.PackageParts.Any())
            {
                this.PackageParts = PackagePart.GetPackagePart(image.PolicyId, image.PolicyImageNum, image.PackageParts);
            }

            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.BillingService.LoadBillingAccountsByClient())
                {
                    DS.RequestData.ClientId = this.ClientId;
                    var PolicyLinks = DS.Invoke()?.DiamondResponse?.ResponseData?.BillingAccountInfo?.PolicyLinks;
                    if (PolicyLinks != null)
                    {
                        this.LinkedAccountPolicies = this.LinkedAccountPolicies ?? new List<AccountLinkedPolicy>();
                        foreach (var linkedPolicy in PolicyLinks)
                        {
                            if (linkedPolicy.PolicyCurrentStatusId.NotEqualsAny(AccountRegistedPolicy.UndesireablePolicyStatusIds()))
                                this.LinkedAccountPolicies.Add(new AccountLinkedPolicy(linkedPolicy));
                        }

                    }
                }

                //using (var DS = Insuresoft.DiamondServices.PolicyService.GetClientPolicies())
                //{
                //    DS.RequestData.PolicyId = PolicyId;
                //    var PolicyLinks = DS.Invoke()?.DiamondResponse?.ResponseData?.Policies;
                //    if (PolicyLinks != null)
                //    {
                //        this.LinkedAccountPolicies = this.LinkedAccountPolicies ?? new List<AccountLinkedPolicy>();
                //        foreach (var linkedPolicy in PolicyLinks)
                //        {
                //            if (linkedPolicy.PolicyCurrentStatusId.NotEqualsAny(AccountRegistedPolicy.UndesireablePolicyStatusIds()))
                //                this.LinkedAccountPolicies.Add(new AccountLinkedPolicy(linkedPolicy.Adapt<DCO.Billing.BillingAccountPolicyLink>()));
                //        }
                //    }
                //}

            }

            BusinessLogic.OMP.ClaimsManager cm = new BusinessLogic.OMP.ClaimsManager();
            this.Claims = cm.LoadClaims(this.PolicyNumber);

            switch (this.LobId)
            {
                case 1: //PPA
                    SupplementalInformation = new PPA.PPASupplementalInfo(this, image, false);
                    break;
                case 51://Mutli-State PPA
                    SupplementalInformation = new PPA.PPASupplementalInfo(this, image, true);
                    break;
                case 2: //HOM
                case 3: //DFR
                    SupplementalInformation = new HOM.HOMSupplementalInfo(image);
                    break;

                case 17: //FARM
                    SupplementalInformation = new FAR.FARSupplementalInfo(image, false);
                    break;
                case 52: //Multi-State FARM
                    SupplementalInformation = new FAR.FARSupplementalInfo(image, true);
                    break;

                case 20: //CAP
                    SupplementalInformation = new CAP.CAPSupplementalInfo(this, image, false);
                    break;
                case 44: //Multi-State CAP
                    SupplementalInformation = new CAP.CAPSupplementalInfo(this, image, true);
                    break;
                case 24:
                    SupplementalInformation = new GAR.GARSupplementalInfo(this, image);
                    break;
                case 14: //PUP          
                case 53: //Multi-State PUP
                    //SupplementalInformation = new PUP.PUPSupplementalInfo(this, image);
                    SupplementalInformation = null;
                    break;
                default: //Everything Else
                    SupplementalInformation = null;
                    break;
            }
        }

        private void CheckIfCoverageIsNeededForClassProperty(CoverageBase cov)
        {
            if (cov != null && (cov.Checkbox == true || cov.WrittenPremium > 0))
            {
                switch (cov.CoverageCode.CoverageCodeId)
                {
                    case 80094:
                        HasAutoEnhancement = true;
                        break;
                    case 80443:
                        HasAutoEnhancementPlus = true;
                        break;
                }
            }
        }
        

    }
   
}