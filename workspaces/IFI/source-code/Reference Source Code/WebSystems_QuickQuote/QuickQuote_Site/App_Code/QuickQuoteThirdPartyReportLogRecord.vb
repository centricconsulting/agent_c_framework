Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store third party report log info
    ''' </summary>
    ''' <remarks>used w/ third party report ordering/retrieving</remarks>
    <Serializable()> _
    Public Class QuickQuoteThirdPartyReportLogRecord

        Public Property thirdPartyReportLogId As Integer = 0
        Public Property thirdPartyReportTypeId As Integer = 0
        Public Property policyId As Integer = 0
        Public Property policyImageNum As Integer = 0
        Public Property unitNum As Integer = 0
        'Public Property name1First As String = String.Empty
        'Public Property name1Middle As String = String.Empty
        'Public Property name1Last As String = String.Empty
        'Public Property name1DOB As String = String.Empty 'date in db
        'Public Property name1SSN As String = String.Empty
        'Public Property name2First As String = String.Empty
        'Public Property name2Middle As String = String.Empty
        'Public Property name2Last As String = String.Empty
        'Public Property name2DOB As String = String.Empty 'date in db
        'Public Property name2SSN As String = String.Empty
        'Public Property addressStreetNum As String = String.Empty
        'Public Property addressStreetName As String = String.Empty
        'Public Property addressApartmentNum As String = String.Empty
        'Public Property addressCity As String = String.Empty
        'Public Property addressState As String = String.Empty
        'Public Property addressZip As String = String.Empty
        Public Property inserted As String = String.Empty 'datetime in db

        'added 12/11/2014
        Public Property thirdPartyReportEntities As List(Of QuickQuoteThirdPartyReportEntity) = Nothing
        Public Property policyNumber As String = String.Empty
        Public Property userId As Integer = 0
        Public Property agencyId As Integer = 0
        Public Property agencyCode As String = String.Empty
        Public Property quoteId As Integer = 0 'added 12/19/2014

    End Class
End Namespace
