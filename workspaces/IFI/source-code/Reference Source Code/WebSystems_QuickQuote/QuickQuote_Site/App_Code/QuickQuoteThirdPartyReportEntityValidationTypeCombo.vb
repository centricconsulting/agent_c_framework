Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store third party report entity/validationType combo info
    ''' </summary>
    ''' <remarks>used w/ third party report ordering/retrieving</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyReportEntityValidationTypeCombo 'added 12/11/2014
        Public Property entityType As QuickQuoteThirdPartyReportHelperClass.ThirdPartyReportEntityType = QuickQuoteThirdPartyReportHelperClass.ThirdPartyReportEntityType.None
        Public Property validationType As QuickQuoteThirdPartyReportHelperClass.ThirdPartyValidationType = QuickQuoteThirdPartyReportHelperClass.ThirdPartyValidationType.None
    End Class
End Namespace
