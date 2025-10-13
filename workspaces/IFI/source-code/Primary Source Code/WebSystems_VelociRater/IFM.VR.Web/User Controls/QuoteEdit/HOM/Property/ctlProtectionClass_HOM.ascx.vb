Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager

Public Class ctlProtectionClass_HOM
    Inherits VRControlBase

#Region "Declarations"
    Private _selectedView As String
    Public Property SelectedView As String
        Get
            Return _selectedView
        End Get
        Set(value As String)
            _selectedView = value
            ViewState("SelectedView") = value
        End Set
    End Property

    Public ReadOnly Property PCC_StartDate As String
        Get
            Return AppSettings("VR_PCC_StartDate")
        End Get
    End Property

    Public ReadOnly Property PCC_IsActive As Boolean
        Get
            If QuickQuote.CommonMethods.QuickQuoteHelperClass.IsVeriskProtectionClassReportOrderingEnabled() Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property ProtectionClassID() As String
        Get
            Select Case SelectedView
                Case "A"
                    Return ddlProtectionClass.ClientID
                    Exit Select
                Case "B"
                    Return ddlProtectionClassB.ClientID
                    Exit Select
                Case "C"
                    Return txtProtectionClassC.ClientID
                    Exit Select
                Case Else
                    Return ""
            End Select
        End Get
    End Property

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    ''' <summary>
    ''' Adds the script for the accordion control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AddAccordionScript()
        Select Case SelectedView
            Case "A"
                Me.VRScript.CreateAccordion(MainAccordionDivId, HiddenField1, "0")
                Me.VRScript.CreateConfirmDialog(Me.lnkClearProtectionClass.ClientID, "Clear Protection Class?")
                Me.VRScript.StopEventPropagation(Me.lnkSaveProtectionClass.ClientID)
                Exit Select
            Case "B"
                Me.VRScript.CreateAccordion(MainAccordionDivId, Me.HiddenField1, "0")
                Me.VRScript.CreateConfirmDialog(Me.lnkClearProtectionClassB.ClientID, "Clear Protection Class?")
                Me.VRScript.StopEventPropagation(Me.lnkSaveProtectionClassB.ClientID)
                Exit Select
            Case "C"
                Me.VRScript.CreateAccordion(MainAccordionDivId, Me.HiddenField1, "0")
                Me.VRScript.CreateConfirmDialog(Me.lnkClearProtectionClassC.ClientID, "Clear Protection Class?")
                Me.VRScript.StopEventPropagation(Me.lnkSaveProtectionClassC.ClientID)
                Exit Select
        End Select

        Exit Sub
    End Sub

    ''' <summary>
    ''' Adds script to controls
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub AddScriptWhenRendered()
        AddAccordionScript()

        '' Set some js bindings
        Select Case SelectedView
            Case "A"
                ' Protection Class View A
                Me.VRScript.AddVariableLine(String.Format("var pc_ddprotectionId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.ddlProtectionClass.ClientID))
                Me.VRScript.AddVariableLine(String.Format("var pc_txtfeetToHydrantId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.ddlFeetToHydrantA.ClientID))
                Me.VRScript.AddVariableLine(String.Format("var pc_txtMilesToFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.ddlMilesToFireDepartmentA.ClientID))
                'Me.VRScript.AddVariableLine(String.Format("var pc_txtfeetToHydrantId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.txtFeetToHydrant.ClientID))
                'Me.VRScript.AddVariableLine(String.Format("var pc_txtMilesToFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.txtMilesToFireDepartment.ClientID))
                Me.VRScript.AddVariableLine(String.Format("var pc_trFeetId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.trPC_Feet.ClientID))
                Me.VRScript.AddVariableLine(String.Format("var pc_trMilesFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.trPC_Miles.ClientID))
                'Me.VRScript.CreateJSBinding(Me.ddlProtectionClass, ctlPageStartupScript.JsEventType.onchange, "ProtectionClass.ProtectionClassChanged('" + Me.ddlProtectionClass.ClientID + "','" + Me.txtFeetToHydrant.ClientID + "','" + Me.txtMilesToFireDepartment.ClientID + "','" + Me.trPC_Feet.ClientID + "','" + Me.trPC_Miles.ClientID + "');")
                Me.VRScript.CreateJSBinding(Me.ddlProtectionClass, ctlPageStartupScript.JsEventType.onchange, "ProtectionClass.ProtectionClassChanged('" + Me.ddlProtectionClass.ClientID + "','" + Me.ddlFeetToHydrantA.ClientID + "','" + Me.ddlMilesToFireDepartmentA.ClientID + "','" + Me.trPC_Feet.ClientID + "','" + Me.trPC_Miles.ClientID + "');")
                Me.VRScript.CreateJSBinding(Me.ddlMilesToFireDepartmentA, ctlPageStartupScript.JsEventType.onchange, "ProtectionClass.MilesToFDChanged('" + Me.ddlMilesToFireDepartmentA.ClientID + "');")
                Exit Select
            Case "B"
                ' Protection Class View B
                Me.VRScript.AddVariableLine(String.Format("var pc_ddprotectionId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.ddlProtectionClassB.ClientID))
                Me.VRScript.AddVariableLine(String.Format("var pc_txtfeetToHydrantId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.ddlFeetToHydrantB.ClientID))
                Me.VRScript.AddVariableLine(String.Format("var pc_txtMilesToFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.ddlMilesToFireDepartmentB.ClientID))
                'Me.VRScript.AddVariableLine(String.Format("var pc_txtfeetToHydrantId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.txtFeetToHydrantB.ClientID))
                'Me.VRScript.AddVariableLine(String.Format("var pc_txtMilesToFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, Me.txtMilesToFireDepartmentB.ClientID))
                Me.VRScript.CreateJSBinding(Me.ddlProtectionClassB, ctlPageStartupScript.JsEventType.onchange, "ProtectionClass.ProtectionClassChangedB('" + Me.ddlProtectionClassB.ClientID + "','" + Me.ddlFeetToHydrantB.ClientID + "','" + Me.ddlMilesToFireDepartmentB.ClientID + "');")
                Me.VRScript.AddVariableLine(String.Format("var pc_trFeetId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Me.VRScript.AddVariableLine(String.Format("var pc_trMilesFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Me.VRScript.CreateJSBinding(Me.ddlMilesToFireDepartmentB, ctlPageStartupScript.JsEventType.onchange, "ProtectionClass.MilesToFDChanged('" + Me.ddlMilesToFireDepartmentB.ClientID + "');")
                Exit Select
            Case "C"
                ' Protection Class View C
                Me.VRScript.AddVariableLine(String.Format("var pc_ddprotectionId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Me.VRScript.AddVariableLine(String.Format("var pc_txtfeetToHydrantId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Me.VRScript.AddVariableLine(String.Format("var pc_txtMilesToFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Me.VRScript.CreateJSBinding(Me.txtProtectionClassC, ctlPageStartupScript.JsEventType.onchange, "ProtectionClass.ProtectionClassChangedC('" + Me.txtProtectionClassC.ClientID + "');")
                Me.VRScript.AddVariableLine(String.Format("var pc_trFeetId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Me.VRScript.AddVariableLine(String.Format("var pc_trMilesFDId_Loc{0} = ""{1}"";", Me.MyLocationIndex, ""))
                Exit Select
            Case Else
                Exit Sub
        End Select
    End Sub

    Public Overrides Sub LoadStaticData()
        'protection Class
        ' ***** DO NOT NEED TO LOAD PROTECTION CLASS VALUES - DONE VIA JAVASCRIPT ******** Matt A
        'qqHelper.LoadStaticDataOptionsDropDown(Me.ddlProtectionClass, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, QuickQuoteStaticDataOption.SortBy.None, QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteHelperClass.PersOrComm.Comm)
    End Sub

    ''' <summary>
    ''' Populates the form
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Populate()
        SelectedView = "A"
        SetViewBasedOnEffectiveDate()
        LoadMilesToFireDepartmentDDLs()
        Dim FormattedMiles As String = String.Empty
        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()

        ' Make sure the Protection Class fields are all set properly
        qqHelper.VerifyProtectionClassFields(Quote)

        'LoadStaticData()
        Select Case SelectedView
            Case "A", "B"
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Me.MyLocation.ProtectionClassId) AndAlso CInt(MyLocation.ProtectionClassId) > 0 Then
                    hiddenSelectedProtectionClassId.Value = Me.MyLocation.ProtectionClassId
                Else
                    hiddenSelectedProtectionClassId.Value = ""
                End If
                Exit Select
            Case "C"
                If qqHelper.BitToBoolean(ConfigurationManager.AppSettings("Task62741")) = True Then
                    Dim generatedPC As String = If(IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Me.MyLocation.ProtectionClassSystemGeneratedId) AndAlso
                                                    CInt(MyLocation.ProtectionClassSystemGeneratedId) > 0, Me.MyLocation.ProtectionClassSystemGeneratedId, "")

                    Dim currentPC As String = If(IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Me.MyLocation.ProtectionClassId) AndAlso
                                                    CInt(MyLocation.ProtectionClassId) > 0, Me.MyLocation.ProtectionClassId, "")
                    hiddenSelectedProtectionClassId.Value = currentPC

                    lblProtectionClassC.Text = If(currentPC <> generatedPC, "Protection Class", "System Generated Protection Class")

                ElseIf IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Me.MyLocation.ProtectionClassSystemGeneratedId) AndAlso CInt(MyLocation.ProtectionClassSystemGeneratedId) > 0 Then
                        hiddenSelectedProtectionClassId.Value = Me.MyLocation.ProtectionClassSystemGeneratedId
                    Else
                        hiddenSelectedProtectionClassId.Value = ""
                End If
                Exit Select
        End Select

        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.FireDepartmentDistanceId) AndAlso CInt(MyLocation.FireDepartmentDistanceId) > 0 Then
            HiddenSelectedMilesToFireDepartmentID.Value = Me.MyLocation.FireDepartmentDistanceId
            ' Convert any "old value" miles to FD to the correct value
            ' Note that if the id is not 1 or 2 we don't need to do anything
            ' 1 = More than 5 miles
            ' 2 = 5 Miles or less
            'If HiddenSelectedMilesToFireDepartmentID.Value = "1" Then
            '    ' More than 5 miles = 6 Miles (Id 8)
            '    hiddenSelectedProtectionClassId.Value = "8"
            'ElseIf HiddenSelectedMilesToFireDepartmentID.Value = "2" Then
            '    ' 5 miles or less = 4 miles (ID 6)
            '    hiddenSelectedProtectionClassId.Value = "6"
            'End If
        Else
            HiddenSelectedMilesToFireDepartmentID.Value = ""
        End If

        ' Since Miles to FD can come back as a decimal, need to format it for display
        If MyLocation.MilesToFireDepartment IsNot Nothing AndAlso MyLocation.MilesToFireDepartment <> String.Empty AndAlso IsNumeric(MyLocation.MilesToFireDepartment) Then
            If MyLocation.MilesToFireDepartment.Contains(".") Then
                FormattedMiles = CInt(MyLocation.MilesToFireDepartment).ToString
            Else
                FormattedMiles = MyLocation.MilesToFireDepartment
            End If
        End If

        Select Case SelectedView
            Case "A"
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.FireHydrantDistanceId) AndAlso CInt(MyLocation.FireHydrantDistanceId) > 0 Then
                    ddlFeetToHydrantA.SelectedValue = MyLocation.FireHydrantDistanceId
                Else
                    If MyLocation.FeetToFireHydrant IsNot Nothing AndAlso MyLocation.FeetToFireHydrant <> String.Empty AndAlso IsNumeric(MyLocation.FeetToFireHydrant) AndAlso CInt(MyLocation.FeetToFireHydrant) > 0 Then
                        If CInt(MyLocation.FeetToFireHydrant) < 1000 Then
                            ddlFeetToHydrantA.SelectedValue = 4
                        Else
                            ddlFeetToHydrantA.SelectedValue = 1
                        End If
                    Else
                        ddlFeetToHydrantA.SelectedIndex = -1
                    End If
                End If
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(hiddenSelectedProtectionClassId.Value) AndAlso CInt(hiddenSelectedProtectionClassId.Value) > 0 Then
                    ddlProtectionClass.SelectedValue = hiddenSelectedProtectionClassId.Value
                Else
                    ddlProtectionClass.SelectedIndex = -1
                End If
                If ddlMilesToFireDepartmentA.Visible Then
                    If HiddenSelectedMilesToFireDepartmentID.Value IsNot Nothing AndAlso HiddenSelectedMilesToFireDepartmentID.Value <> String.Empty AndAlso IsNumeric(HiddenSelectedMilesToFireDepartmentID.Value) AndAlso CInt(HiddenSelectedMilesToFireDepartmentID.Value) > 0 Then
                        ddlMilesToFireDepartmentA.SelectedValue = HiddenSelectedMilesToFireDepartmentID.Value
                    Else
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.MilesToFireDepartment) AndAlso CInt(MyLocation.MilesToFireDepartment) > 0 Then
                            Dim id As String = GetFireDepartmentDistanceId(MyLocation.MilesToFireDepartment)
                            If CInt(id) = 1 Or CInt(id) = 2 Then
                                ddlMilesToFireDepartmentA.SelectedValue = id
                            Else
                                ddlMilesToFireDepartmentA.SelectedIndex = -1
                            End If
                        Else
                            ddlMilesToFireDepartmentA.SelectedIndex = -1
                        End If
                    End If
                End If
                Exit Sub
            Case "B"
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.FireHydrantDistanceId) AndAlso CInt(MyLocation.FireHydrantDistanceId) > 0 Then
                    ddlFeetToHydrantB.SelectedValue = MyLocation.FireHydrantDistanceId
                Else
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.FeetToFireHydrant) AndAlso CInt(MyLocation.FeetToFireHydrant) > 0 Then
                        If CInt(MyLocation.FeetToFireHydrant) < 1000 Then
                            ddlFeetToHydrantB.SelectedValue = 4
                        Else
                            ddlFeetToHydrantB.SelectedValue = 1
                        End If
                    Else
                        ddlFeetToHydrantB.SelectedIndex = -1
                    End If
                End If
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.ProtectionClassId) AndAlso CInt(MyLocation.ProtectionClassId) > 0 Then
                    ddlProtectionClassB.SelectedValue = hiddenSelectedProtectionClassId.Value
                Else
                    ddlProtectionClassB.SelectedIndex = -1
                End If
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(HiddenSelectedMilesToFireDepartmentID.Value) AndAlso CInt(HiddenSelectedMilesToFireDepartmentID.Value) > 0 Then
                    ddlMilesToFireDepartmentB.SelectedIndex = HiddenSelectedMilesToFireDepartmentID.Value
                Else
                    If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.MilesToFireDepartment) AndAlso CInt(MyLocation.MilesToFireDepartment) > 0 Then
                        Dim id As String = GetFireDepartmentDistanceId(MyLocation.MilesToFireDepartment)
                        If CInt(id) > 0 Then
                            ddlMilesToFireDepartmentB.SelectedValue = id
                        End If
                    Else
                        ddlMilesToFireDepartmentB.SelectedIndex = -1
                    End If
                End If
                Exit Sub
            Case "C"
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Me.MyLocation.FireHydrantDistanceId) AndAlso CInt(MyLocation.FireHydrantDistanceId) > 0 Then
                    ' Feet to Hydrant
                    If Me.MyLocation.FireHydrantDistanceId = "1" Then
                        ddlFeetToHydrantC.SelectedIndex = 1
                    ElseIf Me.MyLocation.FireHydrantDistanceId = "4" Then
                        ddlFeetToHydrantC.SelectedIndex = 2
                    Else
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(Me.MyLocation.FeetToFireHydrant) AndAlso CInt(Me.MyLocation.FeetToFireHydrant) >= 0 Then
                            Dim ft As Integer = CInt(Me.MyLocation.FeetToFireHydrant)
                            If ft <= 1000 Then
                                ddlFeetToHydrantC.SelectedIndex = 1
                            Else
                                ddlFeetToHydrantC.SelectedIndex = 2
                            End If
                        Else
                            ddlFeetToHydrantC.SelectedIndex = -1
                        End If
                    End If
                End If
                ' System Generated Protection Class ID
                If Me.MyLocation.ProtectionClassSystemGenerated IsNot Nothing AndAlso Me.MyLocation.ProtectionClassSystemGenerated <> String.Empty AndAlso MyLocation.ProtectionClassSystemGenerated.ToUpper <> "NONE" Then
                    Me.txtProtectionClassC.Text = MyLocation.ProtectionClassSystemGenerated
                Else
                    txtProtectionClassC.Text = String.Empty
                End If
                ' Miles to Fire Department
                Me.txtMilesToFireDepartmentC.Text = qqHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.FireDepartmentDistanceId, MyLocation.FireDepartmentDistanceId)
                ' Name of Fire Department
                If Me.MyLocation.FireDistrictName IsNot Nothing AndAlso Me.MyLocation.FireDistrictName <> "" Then
                    txtNameOfFireDepartmentC.Text = Me.MyLocation.FireDistrictName
                Else
                    txtNameOfFireDepartmentC.Text = String.Empty
                End If
        End Select
        'End If
    End Sub

    ''' <summary>
    ''' Saves the form data to the quote
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Save() As Boolean
        If hiddenSelectedProtectionClassId.Value.Contains("(") And hiddenSelectedProtectionClassId.Value.Contains(")") Then
            Dim textBetween As String = hiddenSelectedProtectionClassId.Value.Substring(hiddenSelectedProtectionClassId.Value.LastIndexOf("(") + 1, hiddenSelectedProtectionClassId.Value.LastIndexOf(")") - hiddenSelectedProtectionClassId.Value.LastIndexOf("(")).Trim("(").Trim(")").Trim()
            If Not (String.IsNullOrWhiteSpace(textBetween) OrElse textBetween.Contains("*")) Then
                If textBetween.Contains("/") Then
                    ' SPLIT CLASS LOGIC
                    Dim left As String = textBetween.Split("/")(0).Trim()
                    Dim right As String = textBetween.Split("/")(1).Trim()
                    Select Case SelectedView
                        Case "A"
                            MyLocation.FireHydrantDistanceId = ddlFeetToHydrantA.SelectedValue
                            If ddlFeetToHydrantA.SelectedIndex = 1 Then
                                Me.MyLocation.FeetToFireHydrant = "1001"
                            ElseIf ddlFeetToHydrantA.SelectedIndex = 2 Then
                                Me.MyLocation.FeetToFireHydrant = "999"
                            Else
                                Me.MyLocation.FeetToFireHydrant = ""
                            End If
                            MyLocation.FireDepartmentDistanceId = HiddenSelectedMilesToFireDepartmentID.Value
                            MyLocation.MilesToFireDepartment = GetFireDepartmentDistanceMiles(ddlMilesToFireDepartmentA.SelectedValue)

                            Dim feet As Integer = CInt(MyLocation.FeetToFireHydrant)
                            Dim miles As Integer = CInt(MyLocation.MilesToFireDepartment)

                            If (miles <= 5 And miles > 0) And (feet <= 1000 And feet > 0) Then
                                Me.MyLocation.ProtectionClassId = left ' choose lower
                            Else
                                Me.MyLocation.ProtectionClassId = right 'choose higher
                            End If
                            Exit Select
                        Case "B"
                            MyLocation.FireHydrantDistanceId = ddlFeetToHydrantB.SelectedValue
                            If ddlFeetToHydrantB.SelectedIndex = 1 Then
                                Me.MyLocation.FeetToFireHydrant = "1001"
                            ElseIf ddlFeetToHydrantB.SelectedIndex = 2 Then
                                Me.MyLocation.FeetToFireHydrant = "999"
                            Else
                                Me.MyLocation.FeetToFireHydrant = ""
                            End If
                            MyLocation.FireDepartmentDistanceId = HiddenSelectedMilesToFireDepartmentID.Value
                            MyLocation.MilesToFireDepartment = GetFireDepartmentDistanceMiles(ddlMilesToFireDepartmentB.SelectedValue)
                            Dim feet As Int32 = CInt(MyLocation.FeetToFireHydrant)
                            Dim miles As Int32 = CInt(MyLocation.MilesToFireDepartment)
                            If (miles <= 5 And miles > 0) And (feet <= 1000 And feet > 0) Then
                                Me.MyLocation.ProtectionClassId = left ' choose lower
                            Else
                                Me.MyLocation.ProtectionClassId = right 'choose higher
                            End If
                            Exit Select
                        Case "C"
                            ' Feet to Hydrant is the only user-editable field
                            Me.MyLocation.FireHydrantDistanceId = Me.ddlFeetToHydrantC.SelectedValue
                            If ddlFeetToHydrantC.SelectedIndex = 1 Then
                                Me.MyLocation.FeetToFireHydrant = "1001"
                            ElseIf ddlFeetToHydrantC.SelectedIndex = 2 Then
                                Me.MyLocation.FeetToFireHydrant = "999"
                            Else
                                Me.MyLocation.FeetToFireHydrant = ""
                            End If
                            Exit Select
                    End Select
                Else
                    ' NOT A SPLIT CLASS & PARENTHESIS IN PROTECTION CLASS
                    Select Case SelectedView
                        Case "A"
                            ' just use text between (n)
                            Me.MyLocation.ProtectionClassId = textBetween
                            MyLocation.FeetToFireHydrant = ""
                            MyLocation.FireHydrantDistanceId = ""
                            MyLocation.MilesToFireDepartment = ""
                            MyLocation.FireDepartmentDistanceId = ""
                            Exit Select
                        Case "B"
                            ' just use text between (n)
                            Me.MyLocation.ProtectionClassId = textBetween
                            MyLocation.FireHydrantDistanceId = ddlFeetToHydrantB.SelectedValue
                            If ddlFeetToHydrantB.SelectedIndex = 1 Then
                                Me.MyLocation.FeetToFireHydrant = "1001"
                            ElseIf ddlFeetToHydrantB.SelectedIndex = 2 Then
                                Me.MyLocation.FeetToFireHydrant = "999"
                            Else
                                Me.MyLocation.FeetToFireHydrant = ""
                            End If

                            MyLocation.FireDepartmentDistanceId = HiddenSelectedMilesToFireDepartmentID.Value
                            MyLocation.MilesToFireDepartment = GetFireDepartmentDistanceMiles(ddlMilesToFireDepartmentB.SelectedValue)
                            Exit Select
                        Case "C"
                            ' Distance to Hydrant is the only user-editable field
                            Me.MyLocation.FireHydrantDistanceId = ddlFeetToHydrantC.SelectedValue
                            If ddlFeetToHydrantC.SelectedIndex = 1 Then
                                Me.MyLocation.FeetToFireHydrant = "1001"
                            ElseIf ddlFeetToHydrantC.SelectedIndex = 2 Then
                                Me.MyLocation.FeetToFireHydrant = "999"
                            Else
                                Me.MyLocation.FeetToFireHydrant = ""
                            End If
                            Exit Select
                    End Select
                End If
            Else
                ' just use 10
                Me.MyLocation.ProtectionClassId = "10"
            End If

        Else
            ' NOT A SPLIT CLASS & NO PARENTHESIS IN PROTECTION CLASS
            If hiddenSelectedProtectionClassId.Value <> "" AndAlso hiddenSelectedProtectionClassId.Value <> "0" Then
                Me.MyLocation.ProtectionClassId = hiddenSelectedProtectionClassId.Value 'standard just numeric selection
            Else
                MyLocation.ProtectionClassId = ""
            End If

            Select Case SelectedView
                Case "A"
                    Me.MyLocation.FeetToFireHydrant = ""
                    MyLocation.FireHydrantDistanceId = ""
                    MyLocation.FireDepartmentDistanceId = ""
                    MyLocation.MilesToFireDepartment = ""
                    'Me.txtFeetToHydrant.Text = ""
                    'Me.MyLocation.MilesToFireDepartment = ""
                    'Me.txtMilesToFireDepartment.Text = ""
                    Exit Select
                Case "B"
                    ' just use text between (n)
                    MyLocation.FireHydrantDistanceId = ddlFeetToHydrantB.SelectedValue
                    If ddlFeetToHydrantB.SelectedIndex = 1 Then
                        Me.MyLocation.FeetToFireHydrant = "1001"
                    ElseIf ddlFeetToHydrantB.SelectedIndex = 2 Then
                        Me.MyLocation.FeetToFireHydrant = "999"
                    Else
                        Me.MyLocation.FeetToFireHydrant = ""
                    End If
                    'MyLocation.FireDepartmentDistanceId = ddlMilesToFireDepartmentB.SelectedValue
                    MyLocation.FireDepartmentDistanceId = HiddenSelectedMilesToFireDepartmentID.Value
                    If ddlMilesToFireDepartmentB.SelectedIndex = 0 Then
                        MyLocation.FireDepartmentDistanceId = ""
                        MyLocation.MilesToFireDepartment = ""
                    Else
                        MyLocation.MilesToFireDepartment = GetFireDepartmentDistanceMiles(ddlMilesToFireDepartmentB.SelectedValue)
                    End If
                    Exit Select
                Case "C"
                    ' Distance to Hydrant is the only user-editable field
                    Me.MyLocation.FireHydrantDistanceId = ddlFeetToHydrantC.SelectedValue
                    If ddlFeetToHydrantC.SelectedIndex = 1 Then
                        Me.MyLocation.FeetToFireHydrant = "1001"
                    ElseIf ddlFeetToHydrantC.SelectedIndex = 2 Then
                        Me.MyLocation.FeetToFireHydrant = "999"
                    Else
                        Me.MyLocation.FeetToFireHydrant = ""
                    End If
                    Exit Select
            End Select

        End If

        ' Make sure all the protection class fields are set properly
        QQHelper.VerifyProtectionClassFields(Quote)

        ' ALWAYS set the hidden protection class id value at the end of save
        Me.hiddenSelectedProtectionClassId.Value = Me.MyLocation.ProtectionClassId
        Me.HiddenSelectedMilesToFireDepartmentID.Value = MyLocation.FireDepartmentDistanceId

        Return True
    End Function

    ''' <summary>
    ''' Clears the fields on the control
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub ClearControl()
        'hiddenSelectedProtectionClassId.Value = ""
        'HiddenSelectedMilesToFireDepartmentID.Value = ""
        'lblSelectedProtectionClassId.Text = ""

        Select Case SelectedView
            Case "A"
                Me.ddlProtectionClass.SelectedIndex = 0
                ddlMilesToFireDepartmentA.SelectedIndex = 0
                ddlFeetToHydrantA.SelectedIndex = 0
                'Me.txtMilesToFireDepartment.Text = ""
                'Me.txtFeetToHydrant.Text = ""
                hiddenSelectedProtectionClassId.Value = ""
                HiddenSelectedMilesToFireDepartmentID.Value = ""
                Exit Select
            Case "B"
                Me.ddlProtectionClassB.SelectedIndex = 0
                Me.ddlMilesToFireDepartmentB.SelectedIndex = 0
                Me.ddlFeetToHydrantB.SelectedIndex = 0
                'Me.txtMilesToFireDepartmentB.Text = ""
                'Me.txtFeetToHydrantB.Text = ""
                hiddenSelectedProtectionClassId.Value = ""
                HiddenSelectedMilesToFireDepartmentID.Value = ""
                Exit Select
            Case "C"
                Me.ddlFeetToHydrantC.SelectedIndex = 0
                'Me.txtProtectionClassC.Text = ""
                'Me.txtMilesToFireDepartmentC.Text = ""
                'Me.txtNameOfFireDepartmentC.Text = ""
                HiddenSelectedMilesToFireDepartmentID.Value = ""
                Exit Select
        End Select

        MyBase.ClearControl()
    End Sub

    ''' <summary>
    ''' Validates the data on the control
    ''' </summary>
    ''' <param name="valArgs"></param>
    ''' <remarks></remarks>
    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Property Protection Class"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        'Dim valList = LocationProtectionClassValidator.ValidateHOMProtectionClass(Me.Quote, Me.MyLocationIndex, valArgs.ValidationType)
        'If valList.Any() Then
        '    For Each v In valList
        '        Select Case v.FieldId
        '            Case LocationProtectionClassValidator.LocationProtectionClass
        '                'Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlTieDown, v, divMobile, "0")
        '        End Select
        '    Next
        'End If

        ' Protection Class
        If Not String.IsNullOrWhiteSpace(hiddenSelectedProtectionClassId.Value) Then ' just checking because you don't want jump logic jumping to hidden field
            Select Case SelectedView
                Case "A"
                    ' On view A, feet and miles are only required if split which we'll check for below
                    Exit Select
                Case "B"
                    ' Need protection class always
                    If Not IsNumeric(hiddenSelectedProtectionClassId.Value) OrElse CInt(hiddenSelectedProtectionClassId.Value) <= 0 Then
                        Me.ValidationHelper.AddError("Missing Protection Class", ddlProtectionClassB.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If
                    If ddlFeetToHydrantB.SelectedIndex <= 0 AndAlso Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlFeetToHydrantB.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If
                    If ddlMilesToFireDepartmentB.SelectedIndex <= 0 Then
                        Me.ValidationHelper.AddError("Missing Miles from Department", ddlMilesToFireDepartmentB.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If
                    ' need valid feet and miles always
                    'If String.IsNullOrWhiteSpace(Me.txtFeetToHydrantB.Text) Then
                    '    Me.ValidationHelper.AddError("Missing Feet from Hydrant", txtFeetToHydrantB.ClientID)
                    '    With Me.ValidationHelper.GetLastError()
                    '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '    End With
                    'Else
                    '    'has feet from hydrant but is it valid
                    '    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtFeetToHydrantB.Text) = False Then
                    '        Me.ValidationHelper.AddError("Invalid Feet from Hydrant", txtFeetToHydrantB.ClientID)
                    '        With Me.ValidationHelper.GetLastError()
                    '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '        End With
                    '    End If
                    'End If

                    'If String.IsNullOrWhiteSpace(Me.txtMilesToFireDepartmentB.Text) Then
                    '    Me.ValidationHelper.AddError("Missing Miles from Department", txtMilesToFireDepartmentB.ClientID)
                    '    With Me.ValidationHelper.GetLastError()
                    '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '    End With
                    'Else
                    '    'has miles to fire dept but is it valid
                    '    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtMilesToFireDepartmentB.Text) = False Then
                    '        Me.ValidationHelper.AddError("Invalid Miles from Department", txtMilesToFireDepartmentB.ClientID)
                    '        With Me.ValidationHelper.GetLastError()
                    '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '        End With
                    '    End If
                    'End If

                    'Updated 12/5/17 for HOM Upgrade MLW
                    If MyLocation.FormTypeId = "6" Or MyLocation.FormTypeId = "7" Or (MyLocation.FormTypeId = "22" AndAlso MyLocation.StructureTypeId = "2") Or (MyLocation.FormTypeId = "25" AndAlso MyLocation.StructureTypeId = "2") Then ' added 12-24-14 because you don't want to wait until rate to tell them
                        If hiddenSelectedProtectionClassId.Value.Trim() = "11" Then
                            Me.ValidationHelper.AddError("Invalid Protection Class for Mobile Home.", ddlProtectionClassB.ClientID)
                            With Me.ValidationHelper.GetLastError()
                                .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                                .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                            End With
                        End If
                    End If
                    Exit Select
                Case "C"
                    ' Need protection class always
                    'If Not IsNumeric(hiddenSelectedProtectionClassId.Value) OrElse CInt(hiddenSelectedProtectionClassId.Value) <= 0 Then
                    '    Me.ValidationHelper.AddError("Missing Protection Class", txtProtectionClassC.ClientID)
                    '    With Me.ValidationHelper.GetLastError()
                    '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '    End With
                    'End If
                    ' need valid feet and miles always
                    If ddlFeetToHydrantC.SelectedIndex <= 0 AndAlso Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlFeetToHydrantC.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If

                    'If String.IsNullOrWhiteSpace(Me.txtMilesToFireDepartmentC.Text) Then
                    '    Me.ValidationHelper.AddError("Missing Miles from Department", txtMilesToFireDepartmentC.ClientID)
                    '    With Me.ValidationHelper.GetLastError()
                    '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '    End With
                    'Else
                    '    'has miles to fire dept but is it valid
                    '    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtMilesToFireDepartmentC.Text) = False Then
                    '        Me.ValidationHelper.AddError("Invalid Miles from Department", txtMilesToFireDepartmentC.ClientID)
                    '        With Me.ValidationHelper.GetLastError()
                    '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '        End With
                    '    End If
                    'End If
                    'If MyLocation.FormTypeId = "6" Or MyLocation.FormTypeId = "7" Then ' added 12-24-14 because you don't want to wait until rate to tell them
                    '    If hiddenSelectedProtectionClassId.Value.Trim() = "11" Then
                    '        Me.ValidationHelper.AddError("Invalid Protection Class for Mobile Home.", txtProtectionClassC.ClientID)
                    '        With Me.ValidationHelper.GetLastError()
                    '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '        End With
                    '    End If
                    'End If
                    Exit Select
            End Select

            ' SPLIT PROTECTION CLASS
            If hiddenSelectedProtectionClassId.Value.Contains("(") And hiddenSelectedProtectionClassId.Value.Contains(")") Then
                Dim textBetween As String = hiddenSelectedProtectionClassId.Value.Substring(hiddenSelectedProtectionClassId.Value.IndexOf("(") + 1, hiddenSelectedProtectionClassId.Value.IndexOf(")") - hiddenSelectedProtectionClassId.Value.IndexOf("("))
                If textBetween.Contains("/") Then
                    Select Case SelectedView
                        Case "A"
                            ' need valid feet and miles
                            If ddlFeetToHydrantA.SelectedIndex <= 0 AndAlso Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlFeetToHydrantA.ClientID)
                                With Me.ValidationHelper.GetLastError()
                                    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                                    .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                                End With
                            End If
                            If ddlMilesToFireDepartmentA.SelectedIndex <= 0 Then
                                Me.ValidationHelper.AddError("Missing Miles to Fire Department", ddlMilesToFireDepartmentA.ClientID)
                                With Me.ValidationHelper.GetLastError()
                                    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                                    .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                                End With
                            End If

                            'If String.IsNullOrWhiteSpace(Me.txtFeetToHydrant.Text) Then
                            '    Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlProtectionClass.ClientID)
                            '    With Me.ValidationHelper.GetLastError()
                            '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                            '    End With
                            'Else
                            '    'has feet from hydrant but is it valid
                            '    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtFeetToHydrant.Text) = False Then
                            '        Me.ValidationHelper.AddError("Invalid Feet from Hydrant", ddlProtectionClass.ClientID)
                            '        With Me.ValidationHelper.GetLastError()
                            '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                            '        End With
                            '    End If
                            'End If

                            'If String.IsNullOrWhiteSpace(Me.txtMilesToFireDepartment.Text) Then
                            '    Me.ValidationHelper.AddError("Missing Miles from Department", ddlProtectionClass.ClientID)
                            '    With Me.ValidationHelper.GetLastError()
                            '        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            '        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                            '    End With
                            'Else
                            '    'has miles to fire dept but is it valid
                            '    If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(Me.txtMilesToFireDepartment.Text) = False Then
                            '        Me.ValidationHelper.AddError("Invalid Miles from Department", ddlProtectionClass.ClientID)
                            '        With Me.ValidationHelper.GetLastError()
                            '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                            '        End With
                            '    End If
                            'End If

                            'Updated 12/5/17 for HOM Upgrade MLW
                            If MyLocation.FormTypeId = "6" Or MyLocation.FormTypeId = "7" Or (MyLocation.FormTypeId = "22" AndAlso MyLocation.StructureTypeId = "2") Or (MyLocation.FormTypeId = "25" AndAlso MyLocation.StructureTypeId = "2") Then ' added 12-24-14 because you don't want to wait until rate to tell them
                                If hiddenSelectedProtectionClassId.Value.Trim() = "11" Then
                                    Me.ValidationHelper.AddError("Invalid Protection Class for Mobile Home.", ddlProtectionClass.ClientID)
                                    With Me.ValidationHelper.GetLastError()
                                        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                                        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                                    End With
                                End If
                            End If

                            Exit Select
                        Case "B"
                            ' Feet & Miles always req'd on B
                            Exit Select
                        Case "C"
                            ' Feet & Miles always req'd on C
                            Exit Select
                    End Select
                End If
            End If
        Else
            ' Protection class is missing
            Select Case SelectedView
                Case "A"
                    ' don't want jump logic jumping to hidden field and hidden field is the test case not the control directly
                    Me.ValidationHelper.AddError("Missing Protection Class", ddlProtectionClass.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    End With
                    ' If protection class is missing we don't need to check miles and feet
                    'If ddlFeetToHydrantA.Visible Then
                    '    If ddlFeetToHydrantA.SelectedIndex <= 0 Then
                    '        Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlFeetToHydrantA.ClientID)
                    '        With Me.ValidationHelper.GetLastError()
                    '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '        End With
                    '    End If
                    'End If
                    'If ddlMilesToFireDepartmentA.Visible Then
                    '    If ddlMilesToFireDepartmentA.SelectedIndex <= 0 Then
                    '        Me.ValidationHelper.AddError("Missing Miles to Fire Department", ddlMilesToFireDepartmentA.ClientID)
                    '        With Me.ValidationHelper.GetLastError()
                    '            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    '        End With
                    '    End If
                    'End If
                    Exit Select
                Case "B"
                    ' don't want jump logic jumping to hidden field and hidden field is the test case not the control directly
                    Me.ValidationHelper.AddError("Missing Protection Class", ddlProtectionClassB.ClientID)
                    With Me.ValidationHelper.GetLastError()
                        .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                        .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    End With
                    If ddlFeetToHydrantB.SelectedIndex <= 0 AndAlso Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlFeetToHydrantB.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If
                    If ddlMilesToFireDepartmentB.SelectedIndex <= 0 Then
                        Me.ValidationHelper.AddError("Missing Miles from Department", ddlMilesToFireDepartmentB.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If
                    Exit Select
                Case "C"
                    ' PROTECTION CLASS NOT REQ'D ON VIEW C - SYSTEM GENERATED
                    ' don't want jump logic jumping to hidden field and hidden field is the test case not the control directly
                    If ddlFeetToHydrantC.SelectedIndex <= 0 AndAlso Quote.QuoteTransactionType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Me.ValidationHelper.AddError("Missing Feet from Hydrant", ddlFeetToHydrantC.ClientID)
                        With Me.ValidationHelper.GetLastError()
                            .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                            .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                        End With
                    End If

                    'Me.ValidationHelper.AddError("Missing Protection Class", txtProtectionClassC.ClientID)
                    'With Me.ValidationHelper.GetLastError()
                    '    .ScriptCollection.Add(WebHelper_Personal.SetAccordionOpenTabIndex(accordList(0).AccordDivId, accordList(0).AccordIndex))
                    '    .ScriptCollection.Add(WebHelper_Personal.AddValidationJumpLogic(.SenderClientId))
                    'End With
                    Exit Select
            End Select
        End If

    End Sub

    ''' <summary>
    ''' Set the control view based on the policy effective date
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetViewBasedOnEffectiveDate(Optional ByVal EffDt As String = Nothing)
        ' This block executed when a date is passed in
        If EffDt IsNot Nothing AndAlso IsDate(EffDt) Then
            If CDate(EffDt).Date < CDate(PCC_StartDate).Date Then
                ' Effective date less than the PCC Start Date: VIEW A
                Me.MainAccordionDivId = Me.ProtectionClassDiv.ClientID
                ProtectionClassDiv.Visible = True
                ProtectionClassDivB.Visible = False
                ProtectionClassDivC.Visible = False
                SelectedView = "A"
            ElseIf CDate(EffDt) >= CDate(PCC_StartDate).Date AndAlso PCC_IsActive Then
                ' Effective date greater than PCC Start Date and PCC is active: VIEW C
                Me.MainAccordionDivId = Me.ProtectionClassDivC.ClientID
                ProtectionClassDiv.Visible = False
                ProtectionClassDivB.Visible = False
                ProtectionClassDivC.Visible = True
                SelectedView = "C"
            Else
                ' Effective date greater than PCC Start Date and PCC is NOT active: VIEW B
                Me.MainAccordionDivId = Me.ProtectionClassDivB.ClientID
                ProtectionClassDiv.Visible = False
                ProtectionClassDivB.Visible = True
                ProtectionClassDivC.Visible = False
                SelectedView = "B"
            End If
            Exit Sub
        End If

        ' This code executed when no date is passed in
        If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
            If CDate(Me.Quote.EffectiveDate).Date < CDate(PCC_StartDate).Date Then
                ' Effective date less than the PCC Start Date: VIEW A
                Me.MainAccordionDivId = Me.ProtectionClassDiv.ClientID
                ProtectionClassDiv.Visible = True
                ProtectionClassDivB.Visible = False
                ProtectionClassDivC.Visible = False
                SelectedView = "A"
            ElseIf CDate(Me.Quote.EffectiveDate) >= CDate(PCC_StartDate).Date AndAlso PCC_IsActive Then
                ' Effective date greater than PCC Start Date and PCC is active: VIEW C
                Me.MainAccordionDivId = Me.ProtectionClassDivC.ClientID
                ProtectionClassDiv.Visible = False
                ProtectionClassDivB.Visible = False
                ProtectionClassDivC.Visible = True
                SelectedView = "C"
            Else
                ' Effective date greater than PCC Start Date and PCC is NOT active: VIEW B
                Me.MainAccordionDivId = Me.ProtectionClassDivB.ClientID
                ProtectionClassDiv.Visible = False
                ProtectionClassDivB.Visible = True
                ProtectionClassDivC.Visible = False
                SelectedView = "B"
            End If
        Else
            ' No effective date on quote - use today's date
            If DateTime.Now.Date < CDate(PCC_StartDate).Date Then
                Me.MainAccordionDivId = Me.ProtectionClassDiv.ClientID
                ProtectionClassDiv.Visible = True
                ProtectionClassDivB.Visible = False
                ProtectionClassDivC.Visible = False
                SelectedView = "A"
            ElseIf DateTime.Now.Date >= CDate(PCC_StartDate).Date AndAlso PCC_IsActive Then
                Me.MainAccordionDivId = Me.ProtectionClassDivC.ClientID
                ProtectionClassDiv.Visible = False
                ProtectionClassDivB.Visible = False
                ProtectionClassDivC.Visible = True
                SelectedView = "C"
            Else
                Me.MainAccordionDivId = Me.ProtectionClassDivB.ClientID
                ProtectionClassDiv.Visible = False
                ProtectionClassDivB.Visible = True
                ProtectionClassDivC.Visible = False
                SelectedView = "B"
            End If
        End If

        Exit Sub
    End Sub

    Private Function GetFireDepartmentDistanceId(ByVal Miles As Integer) As String
        Try
            If Miles <= 0 Then Throw New Exception("Miles less than zero")

            Select Case Miles
                Case 1
                    Return "3" ' 1 mile or less
                Case 2
                    Return "4" ' Greater than 1 to 2 miles
                Case 3
                    Return "5"  ' Greater than 2 to 3 miles
                Case 4
                    Return "6"  ' Greater than 3 to 4 miles
                Case 5
                    Return "7"  ' Greater than 4 to 5 miles
                Case 6
                    Return "8"  ' Greater than 5 to 6 miles
                Case 7
                    Return "9"  ' Greater than 6 to 7 miles
                Case 8
                    Return "10"  ' Greater than 7 to 8 miles
                Case 9
                    Return "11"  ' Greater than 8 to 9 miles
                Case 10
                    Return "12"  ' Greater than 9 to 10 miles
                Case 11
                    Return "13"  ' Greater than 10 to 11 miles
                Case 12
                    Return "14"  ' Greater than 11 to 12 miles
                Case 13
                    Return "15"  ' Greater than 12 to 13 miles
                Case 14
                    Return "16"  ' Greater than 13 to 14 miles
                Case 15
                    Return "17"  ' Greater than 14 to 15 miles
                Case 16
                    Return "18"  ' Greater than 15 to 16 miles
                Case Else
                    Return "19"  ' Greater than 16 miles
            End Select
        Catch ex As Exception
            Return "0"  ' N/A
        End Try
    End Function

    Private Function GetFireDepartmentDistanceMiles(ByVal Id As Integer) As String
        Try
            If Id <= 0 Then Throw New Exception("Passed id is invalid")

            Select Case Id
                Case 1
                    Return "6"  ' More than 5 miles
                Case 2
                    Return "4"  ' 5 Miles or less
                Case 3
                    Return "1" ' 1 mile or less
                Case 4
                    Return "2" ' Greater than 1 to 2 miles
                Case 5
                    Return "3"  ' Greater than 2 to 3 miles
                Case 6
                    Return "4"  ' Greater than 3 to 4 miles
                Case 7
                    Return "5"  ' Greater than 4 to 5 miles
                Case 8
                    Return "6"  ' Greater than 5 to 6 miles
                Case 9
                    Return "7"  ' Greater than 6 to 7 miles
                Case 10
                    Return "8"  ' Greater than 7 to 8 miles
                Case 11
                    Return "9"  ' Greater than 8 to 9 miles
                Case 12
                    Return "10"  ' Greater than 9 to 10 miles
                Case 13
                    Return "11"  ' Greater than 10 to 11 miles
                Case 14
                    Return "12"  ' Greater than 11 to 12 miles
                Case 15
                    Return "13"  ' Greater than 12 to 13 miles
                Case 16
                    Return "14"  ' Greater than 13 to 14 miles
                Case 17
                    Return "15"  ' Greater than 14 to 15 miles
                Case 18
                    Return "16"  ' Greater than 15 to 16 miles
                Case Else
                    Return "17"  ' Greater than 16 miles
            End Select
        Catch ex As Exception
            Return "0"  ' N/A
        End Try
    End Function

    Private Sub LoadMilesToFireDepartmentDDLs()
        Dim li As New ListItem()
        Dim lis As New List(Of ListItem)

        'ddlMilesToFireDepartmentA.Items.Clear()
        ddlMilesToFireDepartmentB.Items.Clear()

        li = New ListItem()
        li.Text = "1 Mile or less"
        li.Value = 3
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 1 to 2 miles"
        li.Value = 4
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 2 to 3 miles"
        li.Value = 5
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 3 to 4 miles"
        li.Value = 6
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 4 to 5 miles"
        li.Value = 7
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 5 to 6 miles"
        li.Value = 8
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 6 to 7 miles"
        li.Value = 9
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 7 to 8 miles"
        li.Value = 10
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 8 to 9 miles"
        li.Value = 11
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 9 to 10 miles"
        li.Value = 12
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 10 to 11 miles"
        li.Value = 13
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 11 to 12 miles"
        li.Value = 14
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 12 to 13 miles"
        li.Value = 15
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 13 to 14 miles"
        li.Value = 16
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 14 to 15 miles"
        li.Value = 17
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 15 to 16 miles"
        li.Value = 18
        lis.Add(li)

        li = New ListItem()
        li.Text = "Greater than 16 miles"
        li.Value = 19
        lis.Add(li)

        'ddlMilesToFireDepartmentA.Items.Add(New ListItem("", ""))
        ddlMilesToFireDepartmentB.Items.Add(New ListItem("", ""))
        For Each itm As ListItem In lis
            'ddlMilesToFireDepartmentA.Items.Add(itm)
            ddlMilesToFireDepartmentB.Items.Add(itm)
        Next

        Exit Sub
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If ViewState("SelectedView") IsNot Nothing Then SelectedView = ViewState("SelectedView")

    End Sub

    ''' <summary>
    ''' Handler for all 3 'Clear' link buttons
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub lnkClearProtectionClass_Click(sender As Object, e As EventArgs) Handles lnkClearProtectionClass.Click, lnkClearProtectionClassB.Click, lnkClearProtectionClassC.Click
        Me.ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
    End Sub

    ''' <summary>
    ''' Handler for all 3 'Save' link buttons
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub lnkSaveProtectionClass_Click(sender As Object, e As EventArgs) Handles lnkSaveProtectionClass.Click, lnkSaveProtectionClassB.Click, lnkSaveProtectionClassC.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        'If Quote.EffectiveDate <> NewEffectiveDate Then Quote.EffectiveDate = NewEffectiveDate
        ''SetViewBasedOnEffectiveDate(NewEffectiveDate)
        'MyLocation.FeetToFireHydrant = ""
        'MyLocation.FireHydrantDistanceId = ""
        'MyLocation.FireDepartmentDistanceId = ""
        'MyLocation.MilesToFireDepartment = ""
        'MyLocation.FireDistrictName = ""

        'Exit Sub

        QQHelper.VerifyProtectionClassFields(Quote) 'Updated 4/5/2018 - DJG
    End Sub

#End Region

End Class