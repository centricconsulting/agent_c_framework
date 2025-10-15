Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class ctl_DriverList_App
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Property ActiveDriverPane As String
        Get
            Return Me.hidden_divDriver_App.Value
        End Get
        Set(value As String)
            Me.hidden_divDriver_App.Value = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divDriver_App.ClientID
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(ListAccordionDivId, hidden_divDriver_App, "0")
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing Then
                Me.Repeater1.DataSource = Me.Quote.Drivers
                Me.Repeater1.DataBind()

                Me.FindChildVrControls() ' finds the just added controls do to the binding
                Dim index As Int32 = 0
                For Each child In Me.GatherChildrenOfType(Of ctl_Driver_App)
                    child.DriverIndex = index
                    child.Populate()
                    index += 1
                Next
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Vehicle List"

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

End Class