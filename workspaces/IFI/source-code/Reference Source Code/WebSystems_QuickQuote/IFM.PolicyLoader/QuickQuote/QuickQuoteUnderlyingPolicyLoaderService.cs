using QuickQuote.CommonMethods;
using QuickQuote.CommonObjects;
using System;
using QuickQuote.CommonObjects.Umbrella;
using System.Collections.Generic;
using System.Linq;

namespace IFM.PolicyLoader.QuickQuote
{
    public class QuickQuoteUnderlyingPolicyLoaderService
    {
        private readonly QuickQuoteXML _policyAccess;
        private readonly IPostOperationHandlerProvider _postOpProvider = null;

        //TODO: add logging reference
        public QuickQuoteUnderlyingPolicyLoaderService(QuickQuoteXML policyAccess, IPostOperationHandlerProvider opProvider)
        {
            _policyAccess = policyAccess;
            _postOpProvider = opProvider;
        }

        public LoadPolicyResult<QuickQuoteUnderlyingPolicy> LoadPolicy(LoadPolicyRequest<QuickQuoteObject> request)
        {
            return LoadPolicy(request, null);
        }

        public LoadPolicyResult<QuickQuoteUnderlyingPolicy> LoadPolicy(LoadPolicyRequest<QuickQuoteObject> request, Func<QuickQuoteObject, (bool IsValid, List<string> Messages)> validator)
        {
            var loadResult = _InternalLoadPolicy(request, validator);
            var retval = new LoadPolicyResult<QuickQuoteUnderlyingPolicy>() { Success = false };

            retval.PolicyLoaded = loadResult.Data != null;
            retval.PassedValidation = loadResult.Success;
            retval.Previous = loadResult.AsObjectResult();
            retval.AddMessages(loadResult.Messages);

            if (loadResult.Success && _postOpProvider.CanProvideFor(loadResult.Data))
            {
                var handler =
                    _postOpProvider.RetrieveHandlerForData(loadResult.Data);

                var handlerResult = handler.PerformHandling(new OperationRequest { Data = loadResult.Data, 
                                                                                   Context = request.ContextData });

                retval.Data = handlerResult.Data;
                retval.Next = handlerResult.Next;

                for (var resultPtr = handlerResult; resultPtr != null; resultPtr = handlerResult.NextResult())
                {
                    retval.Success = resultPtr.Success;
                    retval.TranslationMessages.AddRange(resultPtr.Messages);
                }
            }

            return retval;
        }

        private ServiceResult<QuickQuoteObject> _InternalLoadPolicy(LoadPolicyRequest<QuickQuoteObject> request, Func<QuickQuoteObject, (bool IsValid, List<string> Messages)> validator)
        {
            if (string.IsNullOrWhiteSpace(request.PolicyNumber))
                throw new ArgumentException(nameof(request.PolicyNumber), $"{nameof(request.PolicyNumber)} cannot be empty");

            var retval = new ServiceResult<QuickQuoteObject> { Success = false };
            string errMessages = string.Empty;
            QuickQuoteObject loadedPolicy = null;

            try
            {
                //loadedPolicy = _policyAccess.ReadOnlyQuickQuoteObjectForPolicyInfo(request.PolicyNumber, errorMessage: ref errMessages);

                QuickQuotePolicyLookupInfo policyLookupInfo = new QuickQuotePolicyLookupInfo();
                policyLookupInfo.PolicyNumber = request.PolicyNumber;
                policyLookupInfo.PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage;
                policyLookupInfo.LookupDate = request.ContextData.TransactionEffectiveDate;

                //note: new priority logic on issued images will make it so when using default sort (last), that it would return the endorsement image in the case that the lookupDate is texp_date for current image and teff_date for endorsement; using first sort would return the current image
                loadedPolicy = _policyAccess.QuickQuoteObjectForPolicyLookupInfo_WithOptionalQuoteTransactionTypeToForce(policyLookupInfo, ref errMessages);
                
                QuickQuoteHelperClass qqh = new QuickQuoteHelperClass();
                if (loadedPolicy == null && qqh.IsValidDateString(policyLookupInfo.LookupDate, mustBeGreaterThanDefaultDate: true))
                {
                    policyLookupInfo.PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy;
                    policyLookupInfo.LookupDate = "";
                    loadedPolicy = _policyAccess.QuickQuoteObjectForPolicyLookupInfo_WithOptionalQuoteTransactionTypeToForce(policyLookupInfo, ref errMessages);
                }

                if (String.IsNullOrWhiteSpace(errMessages) == false)
                {
                    retval.Previous = new ServiceResult<object>();
                    retval.Previous.Success = loadedPolicy != null;
                    retval.Previous.AddMessage(errMessages);
                }
                
                (bool passedValidation, List<string> valMessages) = validator?.Invoke(loadedPolicy) ?? (true, Enumerable.Empty<string>().ToList());

                if(valMessages.Any())
                    retval.AddMessages(valMessages);

                retval.Success = passedValidation;
                
            }
            catch (Exception ex)
            {
                retval.Success = false;
                retval.AddMessage(ex.Message);
            }
            finally
            {
                retval.Data = loadedPolicy;
            }

            return retval;
        }
    }
}
