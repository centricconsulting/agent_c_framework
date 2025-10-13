Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteCancelWarning 'added 8/2/2019

        Public Property PolicyNumber As String = ""
        Public Property PolicyId As Integer = 0
        Public Property AgencyCode As String = ""
        Public Property Description As String = ""
        Public Property InsertTimeStamp As String = ""
        Public Property PrintProcessId As Integer = 0
        Public Property CancelDate As String = ""
        Public Property NoticeSystemDate As String = ""

        Public Sub ResetProperties()
            _PolicyNumber = ""
            _PolicyId = 0
            _AgencyCode = ""
            _Description = ""
            _InsertTimeStamp = ""
            _PrintProcessId = 0
            _CancelDate = ""
            _NoticeSystemDate = ""
        End Sub
    End Class
End Namespace
