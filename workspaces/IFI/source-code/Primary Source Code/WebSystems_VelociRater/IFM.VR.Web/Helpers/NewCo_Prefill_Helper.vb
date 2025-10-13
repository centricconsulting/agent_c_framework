Imports IFM.VR.Common.Helpers
Imports IFM.VR.Validation.ObjectValidation
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.QuickQuoteObject

Namespace Helpers
    Public Class NewCo_Prefill_Helper

        Public Shared Function ShouldReturnToQuoteForPreFillOrNewCo(Quote As QuickQuoteObject, DefaultValidationType As ValidationItem.ValidationType, QuoteIdOrPolicyIdPipeImageNumber As String) As Boolean
            Dim NewCoTestValItems = PolicyLevelValidator.PolicyValidation(Quote, DefaultValidationType)
            If Quote.QuoteTransactionType = QuickQuoteTransactionType.NewBusinessQuote _
                AndAlso ((NewCompanyIdHelper.IsNewCompanyIdAvailable(Quote) AndAlso Not NewCompanyIdHelper.isDiamondNewCompany(Quote)) _
                    OrElse (NewCompanyIdHelper.IsNewCompanyIdAvailable(Quote) AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillPopupAvailableForQuote(Quote, qIdOrPIdAndImgNum:=QuoteIdOrPolicyIdPipeImageNumber))) _
                AndAlso Not NewCoTestValItems.ListHasValidationId(PolicyLevelValidator.EffectiveDate) Then
                'is new business, 'is NewCo elidgible and in OldCo, 'is NewCo Eligible and Needs Prefill,
                'effective date is valid (it has no errors)
                Return True
            End If
            Return False
        End Function

    End Class
End Namespace
