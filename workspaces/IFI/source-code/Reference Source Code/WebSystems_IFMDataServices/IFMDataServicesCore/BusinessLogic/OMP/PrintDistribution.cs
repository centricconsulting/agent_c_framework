using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIResponses = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServicesCore.BusinessLogic.OMP
{
    public class PrintDistribution
    {
        public static IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution LoadPolicyPrintDistribution(int policyId)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PrintingService.GetPolicyPrintDistribution())
                {
                    DS.RequestData.PolicyId = policyId;
                    var r = DS.Invoke()?.DiamondResponse;
                    return new CommonObjects.OMP.PrintDistribution(r?.ResponseData?.PrintDistribution);
                }
            }
            return null;
        }

        public static List<IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution> BulkLoadPolicyPrintDistribution(List<int> policyIds)
        {
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {

                using (var DS = Insuresoft.DiamondServices.PrintingService.GetPolicyPrintDistribution())
                {
                    List<CommonObjects.OMP.PrintDistribution> PrintDistributions = new List<CommonObjects.OMP.PrintDistribution>();
                    foreach (var policyId in policyIds)
                    {
                        DS.RequestData.PolicyId = policyId;
                        var r = DS.Invoke()?.DiamondResponse;
                        PrintDistributions.Add(new CommonObjects.OMP.PrintDistribution(r?.ResponseData?.PrintDistribution));
                    }
                    return PrintDistributions;
                }
            }
            return null;
        }

        public static Insuresoft.DiamondServices.IFMResponse<global::Diamond.Common.Services.Messages.PrintingService.SavePolicyPrintDistribution.Response> SavePolicyPrintDistribution(IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution settings)
        {

            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PrintingService.SavePolicyPrintDistribution())
                {
                    DS.RequestData.PolicyPrintDistribution = settings.ToDiamondPrintDistibution();
                    return DS.Invoke();
                }
            }
            return null;
        }

        public static APIResponses.Common.ServiceResult SavePolicyPrintDistributionSR(IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution settings)
        {
            var sr = new APIResponses.Common.ServiceResult();
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PrintingService.SavePolicyPrintDistribution())
                {
                    DS.RequestData.PolicyPrintDistribution = settings.ToDiamondPrintDistibution();
                    var response = DS.Invoke();
                    if (response?.DiamondResponse?.ResponseData != null && response.DiamondResponse.ResponseData.Result)
                    {
                        // completed successfully
                        sr.ResponseData = true;
#if DEBUG
                        if (response.DiamondResponse.ResponseData.Result == false)
                            Debugger.Break();
#endif
                    }
                    else
                    {
                        sr.ResponseData = false;
                        sr.Messages.CreateErrorMessage("Failed to save.");
                        // gather any validation errors
                        if (response != null)
                        {
                            if (response.dv != null && response.dv.ValidationItems != null)
                            {
#if DEBUG
                                Debugger.Break();
#endif

                                foreach (var item in response.dv.ValidationItems)
                                {
                                    if (item.ValidationSeverityType == 1) // ERROR
                                        sr.Messages.CreateErrorMessage(item.Message);
                                    if (item.ValidationSeverityType == 2) // WARNING
                                        sr.Messages.CreateGeneralMessage(item.Message);
                                    //if (item.ValidationSeverityType == 0) // NA
                                    //    this.CreateGeneralMessage(item.Message);
                                    //if (item.ValidationSeverityType == 4) // OTHER
                                    //    this.CreateGeneralMessage(item.Message);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                sr.Messages.CreateErrorMessage("Failed to login to diamond.");
            }
            return sr;
        }
        public static List<IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution> BulkSavePolicyPrintDistributionSR(List<IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution> settings)
        {
            var sr = new List<IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution>();
            if (BusinessLogic.OMP.DiamondLogin.OMPLogin())
            {
                using (var DS = Insuresoft.DiamondServices.PrintingService.SavePolicyPrintDistribution())
                {
                    List<CommonObjects.OMP.PrintDistribution> PrintDistributions = new List<CommonObjects.OMP.PrintDistribution>();
                    foreach (var printDistribution in settings)
                    {
                        DS.RequestData.PolicyPrintDistribution = printDistribution.ToDiamondPrintDistibution();
                        var PI = new DataServicesCore.BusinessLogic.Diamond.Policy.PolicyImage();
                        var policyImage = PI.GetPolicyIDAndImageNumByPolicyNumber(printDistribution.PolicyNumber.ToString());
                        DS.RequestData.PolicyImageNum = policyImage.PolicyImageNum;

                        var response = DS.Invoke();
                        if (response?.DiamondResponse?.ResponseData != null && response.DiamondResponse.ResponseData.Result)
                        {
                            // completed successfully
                            sr.Add(new IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution { Response = response?.DiamondResponse.ResponseData.Result + " - " + printDistribution.PolicyNumber});

#if DEBUG
                            if (response.DiamondResponse.ResponseData.Result == false)
                                Debugger.Break();
#endif
                        }
                        else
                        {
                            sr.Add(new IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution()
                            {
                                Response = false + " - " + printDistribution.PolicyNumber,
                            });

                            // gather any validation errors
                            if (response != null)
                            {
                                if (response.dv != null && response.dv.ValidationItems != null)
                                {
#if DEBUG
                                    Debugger.Break();
#endif

                                    foreach (var item in response.dv.ValidationItems)
                                    {
                                        if (item.ValidationSeverityType == 1) // ERROR
                                            //sr.Messages.CreateErrorMessage(item.Message);
                                            sr.Add(new IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution()
                                            {
                                                Response = false + " - " + printDistribution.PolicyId,
                                                ErrorMessage = item.Message
                                            });
                                        if (item.ValidationSeverityType == 2) // WARNING
                                            sr.Add(new IFM.DataServicesCore.CommonObjects.OMP.PrintDistribution()
                                            {
                                                Response = false + " - " + printDistribution.PolicyId,
                                                ErrorMessage = item.Message
                                            });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //sr.Messages.CreateErrorMessage("Failed to login to diamond.");
            }
            return sr;
        }

    }

}