Imports IFM.PrimativeExtensions

Public Class ctl_BOP_App_LocationList
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then

            Me.Repeater1.DataSource = Me.Quote.Locations
            Me.Repeater1.DataBind()

            Me.FindChildVrControls()

            Dim lIndex As Int32 = 0
            For Each cnt In Me.GatherChildrenOfType(Of ctl_BOP_App_Location)
                cnt.LocationIndex = lIndex
                cnt.Populate()
                lIndex += 1
            Next
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divMainList.ClientID
            Me.hdnAccord.Value = 0
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

End Class