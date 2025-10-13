Imports IFM.PrimativeExtensions

Public Class ctl_CPR_App_BuildingList
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)

                If loc.IsNotNull Then
                    If loc.Buildings.IsNotNull AndAlso loc.Buildings.Count > 0 Then
                        Me.Repeater1.DataSource = loc.Buildings
                        Me.Repeater1.DataBind()
                    End If
                End If

                Me.FindChildVrControls()
                Dim bIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_CPR_App_Building)
                    cnt.LocationIndex = Me.LocationIndex
                    cnt.BuildingIndex = bIndex
                    cnt.Populate()
                    bIndex += 1
                Next
            End If
        End If
    End Sub

    Private Sub ctl_CPR_BuildingList_Init(sender As Object, e As EventArgs) Handles Me.Init
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.divBuildingList.ClientID
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