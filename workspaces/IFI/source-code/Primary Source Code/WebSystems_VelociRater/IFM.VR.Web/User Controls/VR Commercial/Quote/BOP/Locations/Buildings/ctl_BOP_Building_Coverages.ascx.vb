Imports IFM.PrimativeExtensions

Public Class ctl_BOP_Building_Coverages
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyBuilding As QuickQuote.CommonObjects.QuickQuoteBuilding
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) AndAlso Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.HasItemAtIndex(Me.BuildingIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex)
            End If
            Return Nothing
        End Get

    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divMain.ClientID
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