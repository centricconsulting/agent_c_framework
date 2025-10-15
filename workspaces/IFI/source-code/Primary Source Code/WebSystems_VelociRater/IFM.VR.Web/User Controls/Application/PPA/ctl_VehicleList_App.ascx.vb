Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class ctl_VehicleList_App
    Inherits VRControlBase

    Public Event RequestPageRefresh()

    Public Property ActiveVehicleIndex As String
        Get
            Return ViewState("vs_active")
        End Get
        Set(value As String)
            ViewState("vs_active") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AttachVehicleControlEvents()
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divVehicle_App.ClientID
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(ListAccordionDivId, hiddenActiveVehicle, "0")
    End Sub

    Protected Sub AttachVehicleControlEvents()
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim vehicleControl As ctl_Vehicle_App = cntrl.FindControl("ctl_Vehicle_App")
            AddHandler vehicleControl.RequestPageRefresh, AddressOf vehicleControlRequestPageRefresh
        Next
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing Then
                Me.Repeater1.DataSource = Me.Quote.Vehicles
                Me.Repeater1.DataBind()

                Me.FindChildVrControls() ' finds the just added controls do to the binding
                Dim index As Int32 = 0
                For Each child In Me.ChildVrControls
                    If TypeOf child Is ctl_Vehicle_App Then
                        Dim c As ctl_Vehicle_App = child
                        c.VehicleIndex = index
                        c.Populate()
                        index += 1
                    End If
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

    Private Sub vehicleControlRequestPageRefresh()
        RaiseEvent RequestPageRefresh()
    End Sub

End Class