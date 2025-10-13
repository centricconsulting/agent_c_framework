Public Class ctlFamFarmCorpItem
    Inherits VRControlBase

    Public Property RowNumber As Integer
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Integer)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Private _validatorGroupName As String
    Public Property ValidatorGroupName() As String
        Get
            Return _validatorGroupName
        End Get
        Set(ByVal value As String)
            _validatorGroupName = value
        End Set
    End Property

    Public Property Description As String
        Get
            If ViewState("vs_FccDescription") Is Nothing Then
                ViewState("vs_FccDescription") = String.Empty
            End If
            Return ViewState("vs_FccDescription").ToString
        End Get
        Set(value As String)
            ViewState("vs_FccDescription") = value
        End Set
    End Property

    Public Event RemoveFfcItem(index As Integer)
    Public Event AddFfcItem(desc As String)


    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Me.txtDescription.Text = Description
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        RaiseEvent AddFfcItem(Me.txtDescription.Text)
    End Function

    Protected Sub OnConfirm(sender As Object, e As EventArgs) Handles lnkDelete.Click
        Dim confirmValue As String = Request.Form("confirmValue")


        If confirmValue = "Yes" Then
            'txtDescription.Text = ""
            RaiseEvent RemoveFfcItem(RowNumber)
            Save_FireSaveEvent(False)
        End If
    End Sub


End Class