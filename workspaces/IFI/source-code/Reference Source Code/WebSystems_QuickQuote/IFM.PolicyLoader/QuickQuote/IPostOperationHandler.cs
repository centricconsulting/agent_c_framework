using QuickQuote.CommonObjects;
using QuickQuote.CommonObjects.Umbrella;
using System;
using System.Collections.Generic;

namespace IFM.PolicyLoader.QuickQuote
{
    public interface IPostOperationHandler
    {
        bool CanHandle(QuickQuoteObject requestData);
        OperationResult PerformHandling(OperationRequest opRequest);
    }

    public interface IPostOperationHandlerProvider
    {
       bool CanProvideFor(QuickQuoteObject testData);
        IPostOperationHandler RetrieveHandlerForData(QuickQuoteObject testData);
    }
    public class OperationResult:ServiceResult<QuickQuoteUnderlyingPolicy>// where OperationResultData: class
    {

        public OperationResult PreviousResult()
        {
            OperationResult retval = null;

            if (this.Previous?.Data is QuickQuotePolicyUnderwriting)
            {
                retval = (OperationResult)this.Previous;
            }
            return retval;
        }

        public OperationResult NextResult()
        {
            OperationResult retval = null;

            if(this.Next?.Data is QuickQuotePolicyUnderwriting)
            {
                retval = (OperationResult) this.Next;
            }
            return retval;
        }
        
        public static explicit operator OperationResult(ServiceResult<object> boxedSource)
        {
            OperationResult retval = null;

            if (boxedSource != null)
            {
                if (boxedSource?.Data is QuickQuotePolicyUnderwriting)
                {
                    retval = new OperationResult
                    {
                        Data = (QuickQuoteUnderlyingPolicy)boxedSource.Data,
                        Success = boxedSource.Success,
                        Next = boxedSource.Next,
                        Previous = boxedSource.Previous
                    };

                    retval.AddMessages(boxedSource.Messages);
                }
                else
                    throw new ArgumentException("Cannot convert ServiceResult to OperationResult if Data property is not of a type compatible with QuickQuoteUnderlyingPolicy");
            }
            return retval;
        }
    }
    public class OperationRequest:ServiceRequest<QuickQuoteObject>// where OperationRequestData:class
    {
        public QuickQuoteObject Context { get; set; }
    }
}