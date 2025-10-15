Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store third party report name info
    ''' </summary>
    ''' <remarks>used w/ third party report ordering/retrieving</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyReportName 'added 12/11/2014
        Public Property thirdPartyReportNameId As Integer = 0
        Public Property first As String = String.Empty
        Public Property middle As String = String.Empty
        Public Property last As String = String.Empty
        Public Property DOB As String = String.Empty 'date in db
        Public Property SSN As String = String.Empty
        Public Property inserted As String = String.Empty 'datetime in db
    End Class
End Namespace
