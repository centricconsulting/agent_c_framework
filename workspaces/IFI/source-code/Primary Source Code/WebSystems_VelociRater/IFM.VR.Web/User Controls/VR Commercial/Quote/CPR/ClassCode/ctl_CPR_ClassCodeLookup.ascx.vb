Imports IFM.PrimativeExtensions
Imports PopupMessageClass

Public Class ctl_CPR_ClassCodeLookup
    Inherits VRControlBase

    Public Property txtClassCodeId As String
        Get
            Return ViewState("vs_txtCCId")
        End Get
        Set(value As String)
            ViewState("vs_txtCCId") = value
        End Set
    End Property

    Public Property txtID As String
        Get
            Return ViewState("vs_lblId")
        End Get
        Set(value As String)
            ViewState("vs_lblId") = value
        End Set
    End Property

    Public Event SelectedClassCodeChanged(ByVal ClassCode As String, ByVal Desc As String, ByVal DiaClass_Id As String)

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreatePopupForm(Me.divMain.ClientID, "Special Class Code Lookup", 750, 550, True, True, False, Me.txtFilterValue.ClientID, "")
        'Me.VRScript.CreateJSBinding(Me.btnSearch.ClientID, "click", "VRClassCode.PerformPIOClassCodeLookup(" + Me.Quote.LobId + ",'#" + Me.ddlFilterBy.ClientID + "','#" + Me.txtFilterValue.ClientID + "','#" + Me.divResults.ClientID + "','" + Me.HiddenDescription.ClientID + "','" & Me.HiddenDIAClass_Id.ClientID & "','" & txtClassCodeId & "','" & txtID & "'); return false;")
        Me.VRScript.CreateJSBinding(Me.btnSearch.ClientID, "click", "VRClassCode.PerformPIOClassCodeLookup(" + Me.Quote.LobId + "," + Me.Quote.StateId + ",'" + Me.Quote.EffectiveDate + "','#" + Me.ddlFilterBy.ClientID + "','#" + Me.txtFilterValue.ClientID + "','#" + Me.divResults.ClientID + "','" + Me.HiddenDescription.ClientID + "','" & Me.HiddenDIAClass_Id.ClientID & "','" & txtClassCodeId & "','" & txtID & "'," & Me.Quote.CompanyId & "); return false;")

        ' These variables will all be used by the Class Code selection script
        ' Hidden Fields
        Me.VRScript.AddVariableLine("var hdnPIODescription = '" & HiddenDescription.ClientID & "';")
        Me.VRScript.AddVariableLine("var hdnPIOID = '" & HiddenDIAClass_Id.ClientID & "';")

        Me.VRScript.AddVariableLine("function CloseCCLookupForm(){$('#" + Me.btnClose.ClientID + "').click();}")

        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Hide()

        Me.txtFilterValue.Text = ""
        Me.ddlFilterBy.SelectedIndex = 0

        PopulateChildControls()
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