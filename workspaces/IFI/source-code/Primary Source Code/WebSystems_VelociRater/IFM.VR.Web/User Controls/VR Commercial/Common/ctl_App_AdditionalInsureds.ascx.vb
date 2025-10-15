Imports QuickQuote.CommonObjects

Public Class ctl_App_AdditionalInsureds
    Inherits VRControlBase

    Private Enum AIGridCellName
        Num = 0
        NumType = 1
        Type = 2
        Name = 3
        DesigPrem = 4
        Desc = 5
        Waiver = 6
        EditButtom = 7
        DeleteButton = 8
    End Enum

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddScriptLine("$(""#aInsdHeader"").accordion({collapsible: false, heightStyle: ""content""});")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID, False)
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        Dim DTadditionalInsured As New DataTable
        'OwnersLesseesOrContractorsCompletedOperations
        Dim AICheckBOPListMinusOLCCompletedOps As New List(Of QuickQuoteAdditionalInsured)
        DTadditionalInsured.Columns.Add("Num")
        DTadditionalInsured.Columns.Add("NumType")
        DTadditionalInsured.Columns.Add("Type")
        DTadditionalInsured.Columns.Add("Name")
        DTadditionalInsured.Columns.Add("DesigPrem")
        DTadditionalInsured.Columns.Add("Desc")
        DTadditionalInsured.Columns.Add("Waiver")
        Me.Session.Add("DTadditionalInsured", DTadditionalInsured)

        If Me.Quote.AdditionalInsureds IsNot Nothing AndAlso Me.Quote.AdditionalInsureds.Count > 0 OrElse Me.Quote.AdditionalInsuredsManualCharge <> "" Then
            'already been saved previously from quote or appGap (if quote, description for each ai will be Additional Insured 1, etc.)
            'Me.Quote.AdditionalInsureds.Count should match Me.Quote.AdditionalInsuredsCount; Me.Quote.AdditionalInsuredsManualCharge not used w/ BOP

            Dim count = 0
            For Each ai As QuickQuoteAdditionalInsured In Me.Quote.AdditionalInsureds
                Dim aiGridText As String = ai.CoverageCodeName

                Dim waiver As String = ""

                If ai.HasWaiverOfSubrogation = True Then
                    waiver = "Yes"
                ElseIf ai.HasWaiverOfSubrogation = False Then
                    waiver = "No"
                End If

                DTadditionalInsured.Rows.Add((count), CInt(ai.AdditionalInsuredType).ToString(), aiGridText, ai.NameOfPersonOrOrganization, ai.DesignationOfPremises, ai.Description, waiver)

                count += 1
            Next

            If Me.Quote.HasAdditionalInsuredsCheckboxBOP AndAlso Me.Quote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso Me.Quote.AdditionalInsuredsCheckboxBOP.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInsured In Me.Quote.AdditionalInsuredsCheckboxBOP
                    If ai.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations Then
                        Dim aiGridType As String = ai.CoverageCodeName

                        Dim waiver As String = ""

                        If ai.HasWaiverOfSubrogation = True Then
                            waiver = "Yes"
                        ElseIf ai.HasWaiverOfSubrogation = False Then
                            waiver = "No"
                        End If


                        ' Remove defaulted values if they are present
                        Dim aiGridName As String = ai.NameOfPersonOrOrganization
                        Dim pattern As String = "Name\s\d+"
                        Dim replacement As String = ""
                        Dim rgx As New Regex(pattern, RegexOptions.IgnoreCase)
                        aiGridName = rgx.Replace(aiGridName, replacement)

                        Dim aiGridPrem As String = ai.DesignationOfPremises
                        pattern = "Address\s\d+"
                        replacement = ""
                        rgx = New Regex(pattern, RegexOptions.IgnoreCase)
                        aiGridPrem = rgx.Replace(aiGridPrem, replacement)

                        Dim aiGridDesc = ai.Description
                        pattern = "Description\s\d+"
                        replacement = ""
                        rgx = New Regex(pattern, RegexOptions.IgnoreCase)
                        aiGridDesc = rgx.Replace(aiGridDesc, replacement)


                        DTadditionalInsured.Rows.Add((count), CInt(ai.AdditionalInsuredType).ToString(), aiGridType, aiGridName, aiGridPrem, aiGridDesc, waiver)
                        count += 1
                    End If
                Next
            End If

            Me.DataGrid_additionalInsured.DataSource = DTadditionalInsured
            Me.DataGrid_additionalInsured.DataBind()
        End If


        'CheckBox filling process
        additionalInsuredAIsWithoutSchedule.Visible = True
        trOwnersLesseesOrContractorsAutomaticWithCompletedOpsandWaiver.Visible = False
        'trOwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherPartiesInConstructionContract.Visible = False
        trOwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract.Visible = False
        trWaiverOfSubrogationWhenRequiredByWrittenContractOrAgreement.Visible = False
        If Me.Quote.HasAdditionalInsuredsCheckboxBOP = True Then
            If Me.Quote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso Me.Quote.AdditionalInsuredsCheckboxBOP.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInsured In Me.Quote.AdditionalInsuredsCheckboxBOP
                    If ai IsNot Nothing Then
                        Select Case ai.AdditionalInsuredType
                            Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations
                                chkTownhouseAssociations.Checked = True
                            Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                                chkEngineersArchitectsOrSurveyors.Checked = True
                            'Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherParties
                            '    trOwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherPartiesInConstructionContract.Visible = True
                            '    chkOwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherPartiesInConstructionContract.Checked = True
                            Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract
                                trOwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract.Visible = True
                                chkOwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract.Checked = True
                            Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.WaiverOfSubrogationWhenRequiredByWrittenContract
                                trWaiverOfSubrogationWhenRequiredByWrittenContractOrAgreement.Visible = True
                                chkWaiverOfSubrogationWhenRequiredByWrittenContractOrAgreement.Checked = True
                            Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver
                                trOwnersLesseesOrContractorsAutomaticWithCompletedOpsandWaiver.Visible = True
                                chkOwnersLesseesOrContractorsAutomaticWithCompletedOpsandWaiver.Checked = True
                        End Select
                    End If
                Next
            End If
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Quote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso Me.Quote.AdditionalInsuredsCheckboxBOP.Count > 0 Or Me.Quote.AdditionalInsureds IsNot Nothing AndAlso Me.Quote.AdditionalInsureds.Count > 0 Then
            Me.Visible = True
        Else
            Me.Visible = False
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        If Me.Visible Then
            'Me.ValidateControl(New VRValidationArgs)

            saveAdditionalInsured()

            Dim dt As New DataTable
            dt = Me.Session.Item("DTadditionalInsured")
            Dim total = 0
            Me.Quote.AdditionalInsureds = Nothing
            Dim addInsureds As New Generic.List(Of QuickQuoteAdditionalInsured)

            'Remove these types in preperation of updating values
            If Me.Quote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso Me.Quote.AdditionalInsuredsCheckboxBOP.Count > 0 Then
                Me.Quote.AdditionalInsuredsCheckboxBOP.RemoveAll(Function(ai As QuickQuoteAdditionalInsured) ai.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations)
                Me.Quote.AdditionalInsuredsCheckboxBOP.RemoveAll(Function(ai As QuickQuoteAdditionalInsured) ai.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors)
                Me.Quote.AdditionalInsuredsCheckboxBOP.RemoveAll(Function(ai As QuickQuoteAdditionalInsured) ai.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations)
            End If

            ' Add this item based on App side setting
            If chkTownhouseAssociations.Checked Then
                Me.Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations))
            End If

            ' Add this item based on App side setting
            If chkEngineersArchitectsOrSurveyors.Checked Then
                Me.Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors))
            End If


            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dtrow As DataRow In dt.Rows
                    Dim ai As New QuickQuoteAdditionalInsured

                    ai.NameOfPersonOrOrganization = dtrow.Item("Name")
                    ai.DesignationOfPremises = dtrow.Item("DesigPrem")
                    ai.Description = dtrow.Item("Desc")
                    If dtrow.Item("Waiver").ToString = "Yes" Then
                        ai.HasWaiverOfSubrogation = True
                    Else
                        ai.HasWaiverOfSubrogation = False
                    End If

                    ai.AdditionalInsuredType = ai.GetAITypeFromCoverageName(dtrow.Item("Type"), Me.Quote.LobType)
                    If ai.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations Then
                        Me.Quote.AdditionalInsuredsCheckboxBOP.Add(ai)
                    Else
                        addInsureds.Add(ai)
                    End If
                Next









                Me.Quote.AdditionalInsureds = addInsureds

                Me.Session.Item("DTadditionalInsured") = dt
            End If
        End If
    End Function

    Protected Sub DropDownList_additonalInsuredType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DropDownList_additonalInsuredType.SelectedIndexChanged

        Me.addInameROW.Visible = False
        Me.addIpremisesROW.Visible = False
        Me.addIDescROW.Visible = False
        Me.addIwaiverROW.Visible = False
        'Me.addIpremROW.Visible = False

        spnName.InnerText = "Name of Person or Organization"
        spnPremises.InnerText = "Designation of Premises"
        spnDesc.InnerText = "Description"
        spnWaiver.InnerText = "Waiver of Subrogation"

        spnAIStatus.Visible = True
        spnAIStatusText.InnerText = "Adding New Additional Insured"

        If Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Co-Owner of Insured Premises" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Controlling Interests" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Designated Person or Organization" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Engineers, Architects or Surveyors" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Engineers, Architects or Surveyors Not Engaged by the Named Insured" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Grantor of Franchise" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Lessor of Leased Equipment" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            Me.addIDescROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Managers or Lessors of Premises" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Mortgagee, Assignee Or Receiver" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Owner or Other Interests From Whom Land has been Leased" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Owners, Lessees or Contractors" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Owners, Lessees or Contractors - Completed Operations" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            spnPremises.InnerText = "Location/Description"
            'Me.addIDescROW.Visible = True
            Me.addIwaiverROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Owners, Lessees or Contractors - With Additional Insured Requirement in Construction Contract" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "State or Political Subdivision - Permits Relating to Premises" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "State or Political Subdivisions - Permits" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Townhouse Associations" Then
            Me.addInameROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Vendors" Then
            Me.addInameROW.Visible = True
            Me.addIDescROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Select" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = True
            Me.addIDescROW.Visible = True
            Me.addIwaiverROW.Visible = True
            'Me.addIpremROW.Visible = True
            Me.addIbuttonROW.Visible = True
        End If

        'holds dropdown menu
        Me.additionalInsuredTYPErow.Visible = True
        'buttons
        Me.addIbuttonROW.Visible = True

    End Sub

    Protected Sub DataGrid_additionalInsured_modDelCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid_additionalInsured.ItemCommand

        Dim dt As New DataTable
        dt = Me.Session.Item("DTadditionalInsured")

        If e.CommandName = "edit" Then
            Me.DropDownList_additonalInsuredType.Enabled = True
            spnAIStatus.Visible = True
            spnAIStatusText.InnerText = "Editing Additional Insured #" & (CInt(e.Item.Cells(AIGridCellName.Num).Text) + 1).ToString()
            Me.Session.Item("isEditing") = True
            Me.Session.Item("additionalInsuredRowID") = e.Item.Cells(AIGridCellName.Num).Text

            For Each dgi As DataGridItem In DataGrid_additionalInsured.Items
                If CInt(dgi.Cells(AIGridCellName.Num).Text) Mod 2 = 0 Then
                    dgi.BackColor = Drawing.Color.White
                Else
                    dgi.BackColor = Drawing.Color.FromArgb(221, 221, 221)
                End If
            Next
            e.Item.BackColor = Drawing.Color.LightBlue

            'type
            If e.Item.Cells(AIGridCellName.Type).Text <> "" Then
                'this will not be empty
                Me.additionalInsuredTYPErow.Visible = True

                '21081 - 

                If IsNumeric(e.Item.Cells(AIGridCellName.NumType).Text) And Not String.IsNullOrEmpty(e.Item.Cells(AIGridCellName.Name).Text) And e.Item.Cells(AIGridCellName.Name).Text <> "&nbsp;" And Not e.Item.Cells(AIGridCellName.NumType).Text = "21081" Then
                    Me.DropDownList_additonalInsuredType.SelectedValue = e.Item.Cells(AIGridCellName.NumType).Text
                Else
                    Me.DropDownList_additonalInsuredType.SelectedValue = 0
                End If

                If IsNumeric(e.Item.Cells(AIGridCellName.NumType).Text) And e.Item.Cells(AIGridCellName.NumType).Text = "21081" Then
                    Me.DropDownList_additonalInsuredType.Items.Add(New ListItem("Owners, Lessees or Contractors - Completed Operations", "21081"))
                    Me.DropDownList_additonalInsuredType.SelectedValue = e.Item.Cells(AIGridCellName.NumType).Text
                    Me.DropDownList_additonalInsuredType.Enabled = False
                Else
                    Dim removeItem As ListItem = Me.DropDownList_additonalInsuredType.Items.FindByValue("21081")
                    Me.DropDownList_additonalInsuredType.Items.Remove(removeItem)
                End If
            End If

            'name
            If e.Item.Cells(AIGridCellName.Name).Text <> "" AndAlso e.Item.Cells(AIGridCellName.Name).Text <> "&nbsp;" Then
                'if nothing is entered the datagrid has a space
                Me.addInameROW.Visible = True
                Me.TextBox_addInameOfOrg.Text = e.Item.Cells(AIGridCellName.Name).Text
            Else
                Me.addInameROW.Visible = True
                Me.TextBox_addInameOfOrg.Text = ""
            End If

            If e.Item.Cells(AIGridCellName.DesigPrem).Text <> "" AndAlso e.Item.Cells(AIGridCellName.DesigPrem).Text <> "&nbsp;" Then
                Me.addIpremisesROW.Visible = True
                Me.TextBox_addIdesignation.Text = e.Item.Cells(AIGridCellName.DesigPrem).Text
            Else
                Me.addIpremisesROW.Visible = False
                Me.TextBox_addIdesignation.Text = ""
            End If

            'desc
            If e.Item.Cells(AIGridCellName.Desc).Text <> "" AndAlso e.Item.Cells(AIGridCellName.Desc).Text <> "&nbsp;" Then
                Me.addIDescROW.Visible = True
                Me.TextBox_addIdesc.Text = e.Item.Cells(AIGridCellName.Desc).Text
            Else
                Me.addIDescROW.Visible = False
                Me.TextBox_addIdesc.Text = ""
            End If

            'waiver
            If e.Item.Cells(AIGridCellName.Waiver).Text <> "" Then
                Me.addIwaiverROW.Visible = True
                If e.Item.Cells(AIGridCellName.Waiver).Text = "Yes" Then
                    Me.CheckBox_addIwaiver.Checked = True
                Else
                    Me.CheckBox_addIwaiver.Checked = False
                End If
            Else
                Me.addIwaiverROW.Visible = False
                Me.CheckBox_addIwaiver.Checked = False
            End If

            'premium
            'If e.Item.Cells(7).Text <> "" AndAlso e.Item.Cells(7).Text <> "&nbsp;" Then
            'Me.addIpremROW.Visible = True
            'Me.TextBox_addIprem.Text = e.Item.Cells(7).Text
            'Else
            'Me.addIpremROW.Visible = False
            'End If

            Me.addIbuttonROW.Visible = True

        ElseIf e.CommandName = "delete" Then

            Dim i = e.Item.Cells(AIGridCellName.Num).Text

            Dim j As Integer = 0
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each drow As DataRow In dt.Rows
                    If drow.Item("Num") = i Then
                        dt.Rows(j).Delete()
                        Exit For
                    End If
                    j += 1
                Next

                Dim reorder As Integer = 0
                For Each drow As DataRow In dt.Rows
                    drow.Item("Num") = reorder
                    reorder += 1
                Next

                Me.Session.Item("DTadditionalInsured") = dt

                If dt.Rows.Count = 0 Then
                    'user deleted last AI from grid
                    Me.addIdgROW.Visible = False
                Else
                    Me.DataGrid_additionalInsured.DataSource = dt
                    Me.DataGrid_additionalInsured.DataBind()
                    Me.additionalInsuredsAREA.Visible = True
                End If

                Me.Session.Item("isEditing") = False

            End If
        End If
    End Sub

    Protected Sub Button_addInsSaveAdd_Click(sender As Object, e As System.EventArgs) Handles Button_addInsSaveAdd.Click
        saveAdditionalInsured()

        'UpdateAIEditBox()

        'Me.spnAIStatusText.InnerText = ""
        'Me.spnAIStatus.Visible = False
        'Me.addInameROW.Visible = False
        'Me.addIpremisesROW.Visible = False
        'Me.addIDescROW.Visible = False
        'Me.addIwaiverROW.Visible = False
        ''Me.addIpremROW.Visible = False
        'Me.addIbuttonROW.Visible = True

        'Me.TextBox_addInameOfOrg.Text = ""
        'Me.TextBox_addIdesignation.Text = ""
        'Me.TextBox_addIdesc.Text = ""
        'Me.CheckBox_addIwaiver.Checked = False
        ''Me.TextBox_addIprem.Text = ""
        'Me.addIbuttonROW.Visible = False
        'Me.DropDownList_additonalInsuredType.SelectedValue = 0
        'Me.additionalInsuredTYPErow.Visible = True

        'Dim dt As New DataTable
        'dt = Me.Session.Item("DTadditionalInsured")

        'If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
        '    Me.DataGrid_additionalInsured.DataSource = dt
        '    Me.DataGrid_additionalInsured.DataBind()
        '    Me.addIdgROW.Visible = True
        'End If

        'ErrorMessageItem.InnerText = ""
        Me.ValidateControl(New VRValidationArgs)
    End Sub

    Protected Sub Button_addInsCancel_Click(sender As Object, e As System.EventArgs) Handles Button_addInsCancel.Click

        Me.spnAIStatusText.InnerText = ""
        Me.spnAIStatus.Visible = False
        Me.addInameROW.Visible = False
        Me.addIpremisesROW.Visible = False
        Me.addIDescROW.Visible = False
        Me.addIwaiverROW.Visible = False
        Me.addIbuttonROW.Visible = False

        Me.TextBox_addInameOfOrg.Text = ""
        Me.TextBox_addIdesignation.Text = ""
        Me.TextBox_addIdesc.Text = ""
        Me.CheckBox_addIwaiver.Checked = False
        Me.DropDownList_additonalInsuredType.SelectedValue = 0
        Me.additionalInsuredTYPErow.Visible = True
        Me.Session.Item("isEditing") = False

        For Each dgi As DataGridItem In DataGrid_additionalInsured.Items
            If CInt(dgi.Cells(AIGridCellName.Num).Text) Mod 2 = 0 Then
                dgi.BackColor = Drawing.Color.White
            Else
                dgi.BackColor = Drawing.Color.FromArgb(221, 221, 221)
            End If
        Next

        ErrorMessageItem.InnerText = ""

    End Sub

    Private Sub saveAdditionalInsured()
        Me.ValidateAIEdit(New VRValidationArgs)

        Dim DTadditionalInsured As DataTable
        DTadditionalInsured = Me.Session.Item("DTadditionalInsured")

        If DTadditionalInsured IsNot Nothing Then
            Dim addInsuredcheckbox As String = ""
            If Me.CheckBox_addIwaiver.Checked = True Then
                addInsuredcheckbox = "Yes"
            ElseIf Me.CheckBox_addIwaiver.Checked = False Then
                addInsuredcheckbox = "No"
            End If

            If Me.Session.Item("isEditing") IsNot Nothing AndAlso Me.Session.Item("isEditing") = True _
                AndAlso Me.Session.Item("additionalInsuredRowID") IsNot Nothing AndAlso Me.Session.Item("additionalInsuredRowID") <> "" AndAlso Not Me.ValidationHelper.HasErrros Then

                Dim dr As DataRow = DTadditionalInsured.Rows(Me.Session.Item("additionalInsuredRowID"))
                dr.Item("NumType") = Me.DropDownList_additonalInsuredType.SelectedValue
                dr.Item("Type") = Me.DropDownList_additonalInsuredType.SelectedItem.Text
                dr.Item("Name") = Me.TextBox_addInameOfOrg.Text
                dr.Item("DesigPrem") = Me.TextBox_addIdesignation.Text
                dr.Item("Desc") = Me.TextBox_addIdesc.Text.Trim
                dr.Item("Waiver") = addInsuredcheckbox

                Me.Session.Item("isEditing") = False
                UpdateAIEditBox()
            Else
                If Me.addInameROW.Visible AndAlso Not Me.ValidationHelper.HasErrros Then
                    If DTadditionalInsured.Rows.Count = 0 Then
                        'first
                        DTadditionalInsured.Rows.Add(0, Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, Me.TextBox_addIdesc.Text.Trim, addInsuredcheckbox)
                    Else
                        '2nd, 3rd, etc
                        DTadditionalInsured.Rows.Add((DTadditionalInsured.Rows.Count), Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, Me.TextBox_addIdesc.Text.Trim, addInsuredcheckbox)
                    End If
                    UpdateAIEditBox()
                End If

            End If
        End If

        'UpdateAIEditBox()
    End Sub

    Protected Sub DataGrid_additionalInsured_ItemCreated(sender As Object, e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles DataGrid_additionalInsured.ItemCreated
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim lossesEditButton As Button = CType(e.Item.Cells(AIGridCellName.EditButtom).Controls(0), Button)

            lossesEditButton.CssClass = "roundedContainer StandardButton"
            lossesEditButton.ForeColor = Drawing.Color.White
            lossesEditButton.Font.Name = "Calibri"
            lossesEditButton.Font.Size = 10
            lossesEditButton.Width = 40
            lossesEditButton.Height = 20
            lossesEditButton.Font.Bold = False

            Dim lossesDeleteButton As Button = CType(e.Item.Cells(AIGridCellName.DeleteButton).Controls(0), Button)

            lossesDeleteButton.CssClass = "roundedContainer StandardButton"
            lossesDeleteButton.ForeColor = Drawing.Color.White
            lossesDeleteButton.Font.Name = "Calibri"
            lossesDeleteButton.Font.Size = 10
            lossesDeleteButton.Width = 55
            lossesDeleteButton.Height = 20
            lossesDeleteButton.Font.Bold = False

        End If
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As System.EventArgs) Handles lnkSave.Click
        Save()
    End Sub

    'Protected Sub AddCssClassToElement(element As HtmlControl, newClass As String)
    '    Dim classes As String = element.Attributes("class")
    '    element.Attributes.Add("class", classes & If(classes = String.Empty, newClass, " " + newClass))
    'End Sub

    'Protected Sub RemoveCssClassFromElement(element As HtmlControl, removeClass As String)
    '    Dim classes As String = element.Attributes("class")
    '    element.Attributes.Add("class", Regex.Replace(classes, "(^|\s)" + removeClass + "($|\s)", " ").Trim())
    'End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)

        If Me.Visible Then
            MyBase.ValidateControl(valArgs)


            Me.ValidationHelper.GroupName = "Additional Insureds"

            'ValidateAIEdit(valArgs)

            'Check Grid to see if all names are present if no errors in the edit box.
            If Not Me.ValidationHelper.HasErrros Then
                Dim dt As New DataTable
                dt = Me.Session.Item("DTadditionalInsured")
                Dim total = 0

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each dtrow As DataRow In dt.Rows
                        If dtrow.Item("Name") Is Nothing Or dtrow.Item("Name") = "" Then
                            total += 1
                        End If
                    Next
                End If

                If total > 0 Then
                    Me.ValidationHelper.AddError("All additional insureds must have a name.", Me.DataGrid_additionalInsured.ClientID)
                End If
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Function ValidateAIEdit(valArgs As VRValidationArgs) As Boolean
        Me.ValidationHelper.GroupName = "Additional Insureds"
        Dim testResult = False
        ' Check Type dropdown if editing
        If Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Select" And Me.TextBox_addInameOfOrg.Visible Then
            Me.ValidationHelper.AddError("Missing Type", Me.DropDownList_additonalInsuredType.ClientID)
            testResult = True
        End If

        'Check Name if visible if editing
        If Me.TextBox_addInameOfOrg.Text = "" And Me.TextBox_addInameOfOrg.Visible Then
            Me.ValidationHelper.AddError("Missing Name", Me.TextBox_addInameOfOrg.ClientID)
            testResult = True
        End If
        Return testResult
    End Function

    Protected Function GetAIObject(ClassCodeID As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType, Optional count As Integer = 0) As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured
        Dim ai As New QuickQuote.CommonObjects.QuickQuoteAdditionalInsured()
        ai.AdditionalInsuredType = ClassCodeID
        ai.CoverageCodeId = ClassCodeID
        If ClassCodeID = QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations Then
            ai.Description = UCase("Description " & count.ToString())
            ai.DesignationOfPremises = UCase("Address " & count.ToString())
            ai.NameOfPersonOrOrganization = UCase("Name " & count.ToString())
        End If

        Return ai
    End Function

    Public Sub UpdateAIEditBox()
        Me.spnAIStatusText.InnerText = ""
        Me.spnAIStatus.Visible = False
        Me.addInameROW.Visible = False
        Me.addIpremisesROW.Visible = False
        Me.addIDescROW.Visible = False
        Me.addIwaiverROW.Visible = False
        'Me.addIpremROW.Visible = False
        Me.addIbuttonROW.Visible = True

        Me.TextBox_addInameOfOrg.Text = ""
        Me.TextBox_addIdesignation.Text = ""
        Me.TextBox_addIdesc.Text = ""
        Me.CheckBox_addIwaiver.Checked = False
        'Me.TextBox_addIprem.Text = ""
        Me.addIbuttonROW.Visible = False
        Me.DropDownList_additonalInsuredType.SelectedValue = 0
        Me.DropDownList_additonalInsuredType.Enabled = True
        Me.additionalInsuredTYPErow.Visible = True

        Dim dt As New DataTable
        dt = Me.Session.Item("DTadditionalInsured")

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Me.DataGrid_additionalInsured.DataSource = dt
            Me.DataGrid_additionalInsured.DataBind()
            Me.addIdgROW.Visible = True
        End If

        ErrorMessageItem.InnerText = ""
    End Sub

End Class