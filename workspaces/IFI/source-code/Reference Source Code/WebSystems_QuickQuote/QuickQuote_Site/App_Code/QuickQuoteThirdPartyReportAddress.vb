Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store third party report address info
    ''' </summary>
    ''' <remarks>used w/ third party report ordering/retrieving</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyReportAddress 'added 12/11/2014
        Public Property thirdPartyReportAddressId As Integer = 0
        Public Property streetNum As String = String.Empty
        Public Property streetName As String = String.Empty
        Public Property apartmentNum As String = String.Empty
        Public Property city As String = String.Empty
        Public Property state As String = String.Empty
        Public Property zip As String = String.Empty
        Public Property inserted As String = String.Empty 'datetime in db
    End Class
End Namespace
