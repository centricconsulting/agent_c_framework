Public Class Summary3column
    Inherits VRControlBase

    Public Property HeaderText As String
    Public Property repeaterData As ArrayList

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If repeaterData IsNot Nothing AndAlso repeaterData.Count > 0 Then
            Me.Visible = True
            repeater.DataSource = repeaterData
            repeater.DataBind()
            SectionHeader.InnerText = HeaderText
        Else
            Me.Visible = False
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean

    End Function
End Class