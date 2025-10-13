Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store third party report entity info
    ''' </summary>
    ''' <remarks>used w/ third party report ordering/retrieving</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyReportEntity 'added 12/11/2014
        Public Property thirdPartyReportEntityId As Integer = 0
        Public Property thirdPartyReportEntityTypeId As Integer = 0
        Public Property thirdPartyReportName As QuickQuoteThirdPartyReportName = Nothing
        Public Property thirdPartyReportAddress As QuickQuoteThirdPartyReportAddress = Nothing
        Public Property inserted As String = String.Empty 'datetime in db

        'added 12/18/2014
        Public Property nameSaved As Boolean = False
        Public Property addressSaved As Boolean = False
        'added 12/23/2014
        Public Property isPolicyholder1 As Boolean = False
        Public Property isPolicyholder2 As Boolean = False
    End Class
End Namespace
