Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteCoverageLookupInfo 'added 10/18/2016
        Public Property CoverageCodeId As String = String.Empty
        Public Property Description As String = String.Empty
        Public Property NumberOfOccurrences As Integer = 0 'added 10/19/2016
    End Class
End Namespace