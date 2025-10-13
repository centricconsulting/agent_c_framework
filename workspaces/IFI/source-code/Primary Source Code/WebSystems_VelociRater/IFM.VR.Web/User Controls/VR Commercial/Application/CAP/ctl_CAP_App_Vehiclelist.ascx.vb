Public Class ctl_CAP_App_Vehiclelist
    Inherits VRControlBase

    Public Property ActiveVehicleIndex As String
        Get
            Return Me.hidden_VehicleListActive.Value
        End Get
        Set(value As String)
            Me.hidden_VehicleListActive.Value = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.ListAccordionDivId = Me.divVehicles.ClientID
            LoadStaticData()
            Populate()
        End If

        'AttachVehicleControlEvents()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    'Protected Sub AttachVehicleControlEvents()
    'End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then
            Me.Repeater1.DataSource = Nothing
            If Me.Quote IsNot Nothing Then
                Me.Repeater1.DataSource = Me.Quote.Vehicles
                Me.Repeater1.DataBind()

                Me.FindChildVrControls() ' finds the just added controls do to the binding
                Dim index As Int32 = 0
                For Each child In Me.ChildVrControls
                    If TypeOf child Is ctl_CAP_App_Vehicle Then
                        Dim c As ctl_CAP_App_Vehicle = child
                        c.VehicleIndex = index
                        c.Populate()
                        index += 1
                    End If
                Next

            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle") Then
            MyBase.ValidateControl(valArgs)
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle") Then
            Me.SaveChildControls()
        End If
        Return True
    End Function

    Public Sub PopulateRACASymbols()
        For Each cntl In Me.GatherChildrenOfType(Of ctl_CAP_App_Vehicle)
            cntl.PopulateRACASymbols()
        Next
    End Sub
End Class