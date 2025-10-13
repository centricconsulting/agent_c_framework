Imports IFM.PrimativeExtensions
Imports PopupMessageClass

Public Class ctl_BOP_NaicsCodeLookup
    Inherits VRControlBase

    Public Property NaicsCodeId As String
        Get
            Return ViewState("vs_NaicsCodeId")
        End Get
        Set(value As String)
            ViewState("vs_NaicsCodeId") = value
        End Set
    End Property

    Public Property NaicsDescriptionId As String
        Get
            Return ViewState("vs_NaicsDescriptionId")
        End Get
        Set(value As String)
            ViewState("vs_NaicsDescriptionId") = value
        End Set
    End Property

    Public Event SelectedClassCodeChanged(ByVal ClassCode As String, ByVal Desc As String, ByVal DiaClass_Id As String)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreatePopupForm(Me.divMain.ClientID, "NAICS Code Lookup", 750, 550, True, True, False, Me.txtFilterValue.ClientID, "")
        'Me.VRScript.CreateJSBinding(Me.btnSearch.ClientID, "click", "VRClassCode.PerformPIOClassCodeLookup(" + Me.Quote.LobId + ",'#" + Me.ddlFilterBy.ClientID + "','#" + Me.txtFilterValue.ClientID + "','#" + Me.divResults.ClientID + "','" + Me.HiddenDescription.ClientID + "','" & Me.HiddenDIAClass_Id.ClientID & "','" & txtClassCodeId & "','" & txtID & "'); return false;")
        Me.VRScript.CreateJSBinding(Me.btnSearch.ClientID, "click", "VRNaicsCode.PerformNaicsCodeLookup('" + "#" + Me.ddlFilterBy.ClientID + "','#" + Me.txtFilterValue.ClientID + "','#" + Me.divResults.ClientID + "','#" + NaicsDescriptionId + "','#" & NaicsCodeId & "'); return false;")
        Me.VRScript.AddVariableLine("function CloseNaicsLookupForm(){$('#" + Me.btnClose.ClientID + "').click();}")

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Hide()


        Me.txtFilterValue.Text = ""
        Me.ddlFilterBy.SelectedIndex = 0

    End Sub

    Public Sub Show()
        Populate()
        Me.Visible = True
    End Sub

    Public Sub Hide()
        Me.Visible = False
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Hide()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Hide()
    End Sub
End Class