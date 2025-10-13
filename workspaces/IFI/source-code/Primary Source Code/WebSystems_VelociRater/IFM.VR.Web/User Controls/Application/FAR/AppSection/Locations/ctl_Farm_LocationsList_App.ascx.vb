Public Class ctl_Farm_LocationsList_App
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hddnAccord, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            Me.Repeater1.DataSource = Me.Quote.Locations
            Me.Repeater1.DataBind()

            Me.FindChildVrControls()
            Dim index As Int32 = 0
            For Each child In Me.ChildVrControls
                If TypeOf child Is ctl_Farm_Location_App Then
                    Dim c As ctl_Farm_Location_App = child
                    c.MyLocationIndex = index
                    c.Populate()
                    index += 1
                End If
                child.Populate()
            Next

        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.mainDiv.ClientID
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