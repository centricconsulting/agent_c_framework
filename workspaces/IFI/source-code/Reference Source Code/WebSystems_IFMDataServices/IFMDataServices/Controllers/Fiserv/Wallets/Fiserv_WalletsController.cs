using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using APIResponse = IFM.DataServices.API.ResponseObjects;

namespace IFM.DataServices.Controllers.Fiserv.Wallets
{
    [RoutePrefix("Fiserv/Wallets")]
    public class Fiserv_WalletsController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForEmailAddressAndPolicyNumbers/{emailAddress}/{policyNumbers?}")]
        public JsonResult WalletsForEmailAddressAndPolicyNumbers(string emailAddress, List<string> policyNumbers = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

                if (emailAddress.IsValidEmail() == true)
                {
                    List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(emailAddress, policyNumbers);
                    
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                    if (wallets != null && wallets.Count > 0)
                    {
                        sr.ResponseData = wallets;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {emailAddress} along with policy numbers");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid email address format. Sent {emailAddress}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No email address provided");
            }

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForEmailAddressAndPolicyNumbers_FromUri/{emailAddress}/{policyNumbers?}")]
        public JsonResult WalletsForEmailAddressAndPolicyNumbers_FromUri(string emailAddress, [System.Web.Http.FromUri] List<string> policyNumbers = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

                if (emailAddress.IsValidEmail() == true)
                {
                    List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(emailAddress, policyNumbers);

                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                    if (wallets != null && wallets.Count > 0)
                    {
                        sr.ResponseData = wallets;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {emailAddress} along with policy numbers");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid email address format. Sent {emailAddress}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No email address provided");
            }

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForEmailAddressAndEachPolicyNumber/{emailAddress}/{policyNumber1?}/{policyNumber2?}/{policyNumber3?}/{policyNumber4?}/{policyNumber5?}/{policyNumber6?}/{policyNumber7?}/{policyNumber8?}/{policyNumber9?}/{policyNumber10?}")]
        public JsonResult WalletsForEmailAddressAndEachPolicyNumber(string emailAddress, string policyNumber1 = null, string policyNumber2 = null, string policyNumber3 = null, string policyNumber4 = null, string policyNumber5 = null, string policyNumber6 = null, string policyNumber7 = null, string policyNumber8 = null, string policyNumber9 = null, string policyNumber10 = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

                if (emailAddress.IsValidEmail() == true)
                {
                    List<string> policyNumbers = null;
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber1, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber2, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber3, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber4, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber5, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber6, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber7, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber8, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber9, ref policyNumbers);
                    IFM_CreditCardProcessing.Common.AddStringToList(policyNumber10, ref policyNumbers);

                    List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(emailAddress, policyNumbers);

                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                    if (wallets != null && wallets.Count > 0)
                    {
                        sr.ResponseData = wallets;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {emailAddress} along with policy numbers");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid email address format. Sent {emailAddress}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No email address provided");
            }

            return Json(sr);
        }


        //added 6/29/2020
        //[AcceptVerbs(HttpVerbs.Post)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //[Route("AddWalletItem")]
        //public JsonResult AddWalletItem(global::IFM.DataServicesCore.CommonObjects.Fiserv.WalletItem item)
        //{
        //    APIResponse.Fiserv.FiservAddWalletItemServiceResult sr = new APIResponse.Fiserv.FiservAddWalletItemServiceResult();

        //    if (item != null)
        //    {
        //        Validation.Fiserv.WalletItemAddValidator val = new Validation.Fiserv.WalletItemAddValidator();
        //        var valResult = val.Validate(item);
        //        if (valResult.IsValid)
        //        {
        //            DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
        //            sr = helper.AddWalletItem(item);
        //        }
        //        else
        //        {
        //            this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
        //        }

        //        CodeOk();
        //    }
        //    else
        //    {
        //        CodeBadRequest();
        //        sr.Messages.CreateErrorMessage("No wallet item data provided");
        //    }
        //    sr.ResponseData = new { Success = sr.Success, FiservWalletItemId = sr.FiservWalletItemId, FundingAccountToken = sr.FundingAccountToken };

        //    return Json(sr);
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("AddWalletItem")]
        public JsonResult AddWalletItem(global::IFM.DataServicesCore.CommonObjects.Fiserv.AddWalletItemBody addBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservAddWalletItemResult>();

            if (addBody != null)
            {
                Validation.Fiserv.AddWalletItemBodyValidator val = new Validation.Fiserv.AddWalletItemBodyValidator();
                var valResult = val.Validate(addBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    sr = helper.AddWalletItem(addBody);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No wallet item data provided");
            }
            //sr.ResponseData = new { Success = sr.Success, FiservWalletItemId = sr.FiservWalletItemId, FundingAccountToken = sr.FundingAccountToken, FiservWalletId = sr.FiservWalletId, FundingMethod = sr.FundingMethod, FundingAccountLastFourDigit = sr.FundingAccountLastFourDigit };

            return Json(sr);
        }



        //added 6/30/2020
        [AcceptVerbs(HttpVerbs.Put)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UpdateWalletItem")]
        public JsonResult UpdateWalletItem(global::IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletItemBody updateBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservUpdateWalletItemResult>();

            if (updateBody != null)
            {
                Validation.Fiserv.UpdateWalletItemBodyValidator val = new Validation.Fiserv.UpdateWalletItemBodyValidator();
                var valResult = val.Validate(updateBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    sr = helper.UpdateWalletItem(updateBody);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No wallet item data provided");
            }
            //sr.ResponseData = new { Success = sr.Success };

            return Json(sr);
        }

        //added 6/30/2020
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UpdateWalletItemTesting")]
        public JsonResult UpdateWalletItemTesting(global::IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletItemBody updateBody)
        {
            return UpdateWalletItem(updateBody);
        }


        [AcceptVerbs(HttpVerbs.Delete)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("DeleteWalletItem")]
        public JsonResult DeleteWalletItem(global::IFM.DataServicesCore.CommonObjects.Fiserv.DeleteWalletItemBody deleteBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservDeleteWalletItemResult>();

            if (deleteBody != null)
            {
                Validation.Fiserv.DeleteWalletItemBodyValidator val = new Validation.Fiserv.DeleteWalletItemBodyValidator();
                var valResult = val.Validate(deleteBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    sr = helper.DeleteWalletItem(deleteBody);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No wallet item data provided");
            }
            //sr.ResponseData = new { Success = sr.Success };

            return Json(sr);
        }


        //added 7/30/2020
        [AcceptVerbs(HttpVerbs.Put)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UpdateWallet")]
        public JsonResult UpdateWallet(global::IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletBody updateBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservUpdateWalletResult>();

            if (updateBody != null)
            {
                Validation.Fiserv.UpdateWalletBodyValidator val = new Validation.Fiserv.UpdateWalletBodyValidator();
                var valResult = val.Validate(updateBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    sr = helper.UpdateWallet(updateBody);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No wallet data provided");
            }
            //sr.ResponseData = new { Success = sr.Success };

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Put)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("UpdateWallets")]
        public JsonResult UpdateWallets(global::IFM.DataServicesCore.CommonObjects.Fiserv.UpdateWalletsBody updateBody)
        {
            var sr = new APIResponse.Common.ServiceResult<APIResponse.Fiserv.FiservUpdateWalletsResult>();

            if (updateBody != null)
            {
                Validation.Fiserv.UpdateWalletsBodyValidator val = new Validation.Fiserv.UpdateWalletsBodyValidator();
                var valResult = val.Validate(updateBody);
                if (valResult.IsValid)
                {
                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    sr = helper.UpdateWallets(updateBody);
                }
                else
                {
                    this.AddFluentErrorsToServiceResult(sr, valResult.Errors);
                }

                CodeOk();
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No wallet data provided");
            }
            //sr.ResponseData = new { Success = sr.Success };

            return Json(sr);
        }



        //added 11/12/2020
        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForUserIdentifierAndPolicyNumbers/{userIdentifier}/{policyNumbers?}")]
        public JsonResult WalletsForUserIdentifierAndPolicyNumbers(string userIdentifier, List<string> policyNumbers = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(userIdentifier) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(userIdentifier); //may not need this

                List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(userIdentifier, policyNumbers);

                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                if (wallets != null && wallets.Count > 0)
                {
                    sr.ResponseData = wallets;
                    CodeOk();
                }
                else
                {
                    CodeNotFound();
                    sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {userIdentifier} along with policy numbers");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No user identifier provided");
            }

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForUserIdentifierAndPolicyNumbers_FromUri/{userIdentifier}/{policyNumbers?}")]
        public JsonResult WalletsForUserIdentifierAndPolicyNumbers_FromUri(string userIdentifier, [System.Web.Http.FromUri] List<string> policyNumbers = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(userIdentifier) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(userIdentifier); //may not need this

                List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(userIdentifier, policyNumbers);

                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                if (wallets != null && wallets.Count > 0)
                {
                    sr.ResponseData = wallets;
                    CodeOk();
                }
                else
                {
                    CodeNotFound();
                    sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {userIdentifier} along with policy numbers");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No user identifier provided");
            }

            return Json(sr);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForUserIdentifierAndEachPolicyNumber/{userIdentifier}/{policyNumber1?}/{policyNumber2?}/{policyNumber3?}/{policyNumber4?}/{policyNumber5?}/{policyNumber6?}/{policyNumber7?}/{policyNumber8?}/{policyNumber9?}/{policyNumber10?}")]
        public JsonResult WalletsForUserIdentifierAndEachPolicyNumber(string userIdentifier, string policyNumber1 = null, string policyNumber2 = null, string policyNumber3 = null, string policyNumber4 = null, string policyNumber5 = null, string policyNumber6 = null, string policyNumber7 = null, string policyNumber8 = null, string policyNumber9 = null, string policyNumber10 = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(userIdentifier) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(userIdentifier); //may not need this

                List<string> policyNumbers = null;
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber1, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber2, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber3, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber4, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber5, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber6, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber7, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber8, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber9, ref policyNumbers);
                IFM_CreditCardProcessing.Common.AddStringToList(policyNumber10, ref policyNumbers);

                List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(userIdentifier, policyNumbers);

                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                if (wallets != null && wallets.Count > 0)
                {
                    sr.ResponseData = wallets;
                    CodeOk();
                }
                else
                {
                    CodeNotFound();
                    sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {userIdentifier} along with policy numbers");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No user identifier provided");
            }

            return Json(sr);
        }



        //added 12/01/2020
        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForEmailAddressAndPolicyNumbers/{emailAddress}")]
        public JsonResult WalletsForEmailAddressAndPolicyNumbersList(string emailAddress, List<string> policyNumbers = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(emailAddress) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(emailAddress); //may not need this

                if (emailAddress.IsValidEmail() == true)
                {
                    List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(emailAddress, policyNumbers);

                    DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                    List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                    if (wallets != null && wallets.Count > 0)
                    {
                        sr.ResponseData = wallets;
                        CodeOk();
                    }
                    else
                    {
                        CodeNotFound();
                        sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {emailAddress} along with policy numbers");
                    }
                }
                else
                {
                    CodeBadRequest();
                    sr.Messages.CreateErrorMessage($"Invalid email address format. Sent {emailAddress}");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No email address provided");
            }

            return Json(sr);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [Route("WalletsForUserIdentifierAndPolicyNumbers/{userIdentifier}")]
        public JsonResult WalletsForUserIdentifierAndPolicyNumbersList(string userIdentifier, List<string> policyNumbers = null)
        {
            var sr = this.CreateServiceResult();

            if (string.IsNullOrWhiteSpace(userIdentifier) == false)
            {
                //emailAddress = IFM_CreditCardProcessing.Common.UrlDecodedValue(userIdentifier); //may not need this

                List<string> emailAndPolicyNumbers = DataServicesCore.BusinessLogic.Fiserv.GeneralHelper.ListForEmailAndPolicyNumbers(userIdentifier, policyNumbers);

                DataServicesCore.BusinessLogic.Fiserv.WalletHelper helper = new DataServicesCore.BusinessLogic.Fiserv.WalletHelper();
                List<DataServicesCore.CommonObjects.Fiserv.Wallet> wallets = helper.WalletsForUserIds(emailAndPolicyNumbers);
                if (wallets != null && wallets.Count > 0)
                {
                    sr.ResponseData = wallets;
                    CodeOk();
                }
                else
                {
                    CodeNotFound();
                    sr.Messages.CreateErrorMessage($"Problem retrieving wallets. Sent {userIdentifier} along with policy numbers");
                }
            }
            else
            {
                CodeBadRequest();
                sr.Messages.CreateErrorMessage("No user identifier provided");
            }

            return Json(sr);
        }
    }
}