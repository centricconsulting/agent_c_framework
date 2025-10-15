
Partial Class VR_StateChangeInfo
    Inherits System.Web.UI.Page

    Private Sub VR_StateChangeInfo_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Dim strStateChangeInfo As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.sessionVariableValueAsString("VR_StateChangeInfo")
            If String.IsNullOrWhiteSpace(strStateChangeInfo) = False Then
                strStateChangeInfo = strStateChangeInfo.Replace(vbCrLf, "<br />")
            End If
            Me.lblStateChangeInfo.Text = strStateChangeInfo
        End If
    End Sub
End Class
