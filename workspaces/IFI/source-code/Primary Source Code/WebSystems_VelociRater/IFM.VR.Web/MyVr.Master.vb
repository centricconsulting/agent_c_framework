Public Class MyVr1
    Inherits BaseMasterPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim MPR As New MasterPageRoutines
            Dim webScrollContent As String = MPR.GetWebScrollContentForMasterPageType(MasterPageRoutines.MasterPageType.VelociRaterMasterPage)
            If webScrollContent <> "" Then
                scrollRow.Visible = True
                Me.WebScrollArea.InnerHtml = webScrollContent
            End If
        End If
    End Sub

End Class