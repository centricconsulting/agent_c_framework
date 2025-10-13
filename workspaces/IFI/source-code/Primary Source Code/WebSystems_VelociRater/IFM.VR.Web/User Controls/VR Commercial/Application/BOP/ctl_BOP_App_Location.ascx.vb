Imports IFM.PrimativeExtensions
Public Class ctl_BOP_App_Location
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
            Me.ctl_BOP_App_BuildingList.LocationIndex = value
        End Set
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.LocationIndex
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim title As String = Nothing
        Dim titleLen As Integer = 40

        If Me.MyLocation.IsNotNull Then
            ' Set the title bar
            If MyLocation.Address IsNot Nothing AndAlso MyLocation.Address.DisplayAddress IsNot Nothing AndAlso MyLocation.Address.DisplayAddress <> String.Empty Then
                title = "Location #" & (Me.LocationIndex + 1).ToString & " - "
                If MyLocation.Address.DisplayAddress.Length > titleLen Then
                    title += MyLocation.Address.DisplayAddress.Substring(0, titleLen) & "..."
                Else
                    title += MyLocation.Address.DisplayAddress
                End If
            Else
                title = "Location #" & (Me.LocationIndex + 1).ToString
            End If
            lblAccordHeader.Text = title

            'removed 6/8/2017; moved to ctl_BOP_App_Building
            '' Square Feet
            'If MyLocation.SquareFeet <> "0" Then txtSquareFeet.Text = MyLocation.SquareFeet Else txtSquareFeet.Text = ""
            '' Year Roof Updated
            'If MyLocation.Updates.RoofUpdateYear <> "0" Then txtYearRoofUpdated.Text = MyLocation.Updates.RoofUpdateYear Else txtYearRoofUpdated.Text = ""
            '' Year Plumbing Updated
            'If MyLocation.Updates.PlumbingUpdateYear <> "0" Then txtYearPlumbingUpdated.Text = MyLocation.Updates.PlumbingUpdateYear Else txtYearPlumbingUpdated.Text = ""
            '' Year Built
            'If MyLocation.YearBuilt <> "0" Then txtYearBuilt.Text = MyLocation.YearBuilt Else txtYearBuilt.Text = ""
            '' Year Wiring Updated
            'If MyLocation.Updates.ElectricUpdateYear <> "0" Then txtYearWiringUpdated.Text = MyLocation.Updates.ElectricUpdateYear Else txtYearWiringUpdated.Text = ""
            '' Year Heat Updated
            'If MyLocation.Updates.CentralHeatUpdateYear <> "0" Then txtYearHeatUpdated.Text = MyLocation.Updates.CentralHeatUpdateYear Else txtYearHeatUpdated.Text = ""
        End If

        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        If MyLocation IsNot Nothing Then
            'removed 6/8/2017; moved to ctl_BOP_App_Building
            'MyLocation.SquareFeet = txtSquareFeet.Text
            'MyLocation.Updates.RoofUpdateYear = txtYearRoofUpdated.Text
            'MyLocation.Updates.PlumbingUpdateYear = txtYearPlumbingUpdated.Text
            'MyLocation.YearBuilt = txtYearBuilt.Text
            'MyLocation.Updates.ElectricUpdateYear = txtYearWiringUpdated.Text
            'MyLocation.Updates.CentralHeatUpdateYear = txtYearHeatUpdated.Text
        End If

        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        ValidationHelper.GroupName = "Location #" & LocationIndex + 1

        'removed 6/8/2017; moved to ctl_BOP_App_Building
        'If txtSquareFeet.Text.Trim = "" Then
        '    Me.ValidationHelper.AddError(txtSquareFeet, "Missing Square Feet", accordList)
        'Else
        '    If Not IsNumeric(txtSquareFeet.Text) Then
        '        Me.ValidationHelper.AddError(txtSquareFeet, "Invalid Square Feet", accordList)
        '    Else
        '        Dim sf As Integer = CInt(txtSquareFeet.Text)
        '        If sf <= 0 Then
        '            Me.ValidationHelper.AddError(txtSquareFeet, "Invalid Square Feet", accordList)
        '        End If
        '    End If
        'End If

        'If txtYearRoofUpdated.Text.Trim = "" Then
        '    Me.ValidationHelper.AddError(txtYearRoofUpdated, "Missing Year Roof Updated", accordList)
        'Else
        '    If Not ValidYear(txtYearRoofUpdated.Text) Then
        '        Me.ValidationHelper.AddError(txtYearRoofUpdated, "Invalid Year Roof Updated", accordList)
        '    End If
        'End If

        'If txtYearPlumbingUpdated.Text.Trim = "" Then
        '    Me.ValidationHelper.AddError(txtYearPlumbingUpdated, "Missing Year Plumbing Updated", accordList)
        'Else
        '    If Not ValidYear(txtYearPlumbingUpdated.Text) Then
        '        Me.ValidationHelper.AddError(txtYearPlumbingUpdated, "Invalid Year Plumbing Updated", accordList)
        '    End If
        'End If

        'If txtYearBuilt.Text.Trim = "" Then
        '    Me.ValidationHelper.AddError(txtYearBuilt, "Missing Year Built", accordList)
        'Else
        '    If Not ValidYear(txtYearBuilt.Text) Then
        '        Me.ValidationHelper.AddError(txtYearBuilt, "Invalid Year Built", accordList)
        '    End If
        'End If

        'If txtYearWiringUpdated.Text.Trim = "" Then
        '    Me.ValidationHelper.AddError(txtYearWiringUpdated, "Missing Year Wiring Updated", accordList)
        'Else
        '    If Not ValidYear(txtYearWiringUpdated.Text) Then
        '        Me.ValidationHelper.AddError(txtYearWiringUpdated, "Invalid Year Wiring Updated", accordList)
        '    End If
        'End If

        'If txtYearHeatUpdated.Text.Trim = "" Then
        '    Me.ValidationHelper.AddError(txtYearHeatUpdated, "Missing Year Heat Updated", accordList)
        'Else
        '    If Not ValidYear(txtYearHeatUpdated.Text) Then
        '        Me.ValidationHelper.AddError(txtYearHeatUpdated, "Invalid Year Heat Updated", accordList)
        '    End If
        'End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    '6/8/2017 note: shouldn't be needed anymore; copied to ctl_BOP_App_Building
    Private Function ValidYear(ByVal testYear As String) As Boolean
        If Not IsNumeric(testYear) Then Return False
        Dim yr As Integer = CInt(testYear)
        If yr > DateTime.Now.Year Then Return False
        If yr < 1900 Then Return False
        Return True
    End Function

End Class