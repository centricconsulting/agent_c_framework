Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.MultiState.General

Public Class ctl_App_AdditionalInsureds_CPP
    Inherits VRControlBase

    Private Enum AIGridCellName
        Num = 0
        NumType = 1
        Type = 2
        Name = 3
        DesigPrem = 4
        Premium = 5
        Products = 6
        Waiver = 7
        EditButtom = 8
        DeleteButton = 9
    End Enum

    Private _PremiumPerAInsured = "25"

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
        DTadditionalInsured.Columns.Add("Num")
        DTadditionalInsured.Columns.Add("NumType")
        DTadditionalInsured.Columns.Add("Type")
        DTadditionalInsured.Columns.Add("Name")
        DTadditionalInsured.Columns.Add("DesigPrem")
        DTadditionalInsured.Columns.Add("Premium")
        DTadditionalInsured.Columns.Add("Waiver")
        DTadditionalInsured.Columns.Add("Products")
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

                'First load from App Side - we add N/A to description on edit save, so if this is not present than this hasn't been edited.
                If ai.AdditionalInsuredType = "21021" AndAlso ai.CoverageCodeName = "Vendors" AndAlso (ai.NameOfPersonOrOrganization = String.Empty OrElse ai.NameOfPersonOrOrganization = "n/a") Then
                    aiGridText = String.Empty
                    ai.CoverageCodeId = "0"
                    ai.AdditionalInsuredType = "0"
                End If

                ' Update the manual premium amount
                If IsCheckboxAI(ai) Then
                    ' Checkboxed ai's - no manual premium
                    ai.ManualPremiumAmount = "0"
                Else
                    ai.ManualPremiumAmount = "25"
                End If

                DTadditionalInsured.Rows.Add((count), CInt(ai.AdditionalInsuredType).ToString(), aiGridText, ai.NameOfPersonOrOrganization, ai.DesignationOfPremises, CInt(ai.ManualPremiumAmount), waiver, ai.ProductDescription)

                count += 1
            Next

            Me.DataGrid_additionalInsured.DataSource = DTadditionalInsured
            Me.DataGrid_additionalInsured.DataBind()
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Quote.AdditionalInsureds IsNot Nothing AndAlso Me.Quote.AdditionalInsureds.Count > 0 Then
            Me.Visible = True
        Else
            Me.Visible = False
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        If Me.Visible Then
            saveAdditionalInsured()

            Dim dt As New DataTable
            dt = Me.Session.Item("DTadditionalInsured")
            Dim total = 0
            Me.Quote.AdditionalInsureds = Nothing
            Dim addInsureds As New Generic.List(Of QuickQuoteAdditionalInsured)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dtrow As DataRow In dt.Rows
                    Dim ai As New QuickQuoteAdditionalInsured

                    ai.NameOfPersonOrOrganization = dtrow.Item("Name")
                    ai.DesignationOfPremises = dtrow.Item("DesigPrem")
                    ai.ManualPremiumAmount = _PremiumPerAInsured 'Do I need to set this?
                    ai.ProductDescription = dtrow.Item("Products") 'Where is this data stored and displayed?
                    ai.Description = "n/a"
                    If dtrow.Item("Waiver").ToString = "Yes" Then
                        ai.HasWaiverOfSubrogation = True
                    Else
                        ai.HasWaiverOfSubrogation = False
                    End If

                    ' Note that you MUST pass CGL here because CPP and CGL both use the same ai type but CPP is not set up in this call.  
                    ' If you change this to CPP it will fail.
                    ai.AdditionalInsuredType = CType(dtrow.Item("NumType"), QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType)

                    ' Non-checkbox ai's get a $25 manual premium
                    If IsCheckboxAI(ai) Then
                        ai.ManualPremiumAmount = "0"
                    Else
                        ai.ManualPremiumAmount = _PremiumPerAInsured.ToString
                    End If

                    addInsureds.Add(ai)
                Next

                Me.Quote.AdditionalInsureds = addInsureds

                Me.Session.Item("DTadditionalInsured") = dt
            End If
        End If
        Return True
    End Function

    Protected Sub DropDownList_additonalInsuredType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles DropDownList_additonalInsuredType.SelectedIndexChanged

        Me.addInameROW.Visible = False
        Me.addIpremisesROW.Visible = False
        Me.addIvendorProductsROW.Visible = False
        Me.addIwaiverROW.Visible = False

        spnName.InnerText = "*Name of Person or Organization"
        spnPremises.InnerText = "*Location of Premises"
        spnVendorProd.InnerText = "*Your Products"
        spnWaiver.InnerText = "Waiver of Subrogation"

        spnAIStatus.Visible = True
        spnAIStatusText.InnerText = "Adding New Additional Insured"

        If Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "CO-OWNER OF INSURED PREMISES" Then
            Me.addInameROW.Visible = True
            spnPremises.InnerText = "*Location of Premises"
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "CONTROLLING INTERESTS" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "DESIGNATED PERSON OR ORGANIZATION" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "ENGINEERS, ARCHITECTS OR SURVEYORS" Then
            Me.addInameROW.Visible = False
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "ENGINEERS, ARCHITECTS OR SURVEYORS NOT ENGAGED BY THE NAMED INSURED" Then
            spnName.InnerText = "*Name of Additional Insured Engineers, Architects or Surveyors Not Engaged by the Named Insured"
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "LESSOR OF LEASED EQUIPMENT" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "MANAGERS OR LESSORS OF PREMISES" Then
            Me.addInameROW.Visible = True
            spnPremises.InnerText = "*Designation of Premises (Part Leased by You)"
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "MORTGAGEE, ASSIGNEE OR RECEIVER" Then
            Me.addInameROW.Visible = True
            spnPremises.InnerText = "*Designation of Premises"
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "OWNER OR OTHER INTERESTS FROM WHOM LAND HAS BEEN LEASED" Then
            Me.addInameROW.Visible = True
            spnPremises.InnerText = "*Designation of Premises (Part Leased by You)"
            Me.addIpremisesROW.Visible = True
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "OWNERS, LESSEES OR CONTRACTORS" Then
            Me.addInameROW.Visible = True
            spnPremises.InnerText = "*Location of Covered Operations"
            Me.addIpremisesROW.Visible = True
            'Me.addIpremisesROW.Disabled = True
            Me.TextBox_addIdesignation.Enabled = False
            Me.TextBox_addIdesignation.Text = "Any job sites where work is performed by the named insured for the designated additional insured."
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "STATE OR POLITICAL SUBDIVISION - PERMITS RELATING TO PREMISES" Then
            spnName.InnerText = "*State or Political Subdivision"
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "STATE OR POLITICAL SUBDIVISIONS - PERMITS" Then
            spnName.InnerText = "*State or Political Subdivision"
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "TOWNHOUSE ASSOCIATIONS" Then
            Me.addInameROW.Visible = False
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "VENDORS" Then
            Me.addInameROW.Visible = True
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = True
            Me.addIvendorProductsROW.Visible = True
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "CITY OF CHICAGO - SCAFFOLDING" Then
            ' For the scaffolding coverage, no additional input is required
            Me.addInameROW.Visible = False
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = False
            Me.addIvendorProductsROW.Visible = False
        ElseIf Me.DropDownList_additonalInsuredType.SelectedItem.Text.ToUpper = "SELECT" Then
            Me.addInameROW.Visible = False
            Me.addIpremisesROW.Visible = False
            Me.addIwaiverROW.Visible = False
            Me.addIvendorProductsROW.Visible = False
        End If

        'holds dropdown menu
        Me.additionalInsuredTYPErow.Visible = True
        'buttons
        Me.addIbuttonROW.Visible = True

    End Sub

    Protected Sub DataGrid_additionalInsured_modDelCommand(ByVal sender As Object, ByVal e As DataGridCommandEventArgs) Handles DataGrid_additionalInsured.ItemCommand
        Dim dt As New DataTable
        Dim isCheckBoxAI As Boolean = False

        dt = Me.Session.Item("DTadditionalInsured")

        If e.CommandName = "edit" Then
            isCheckBoxAI = AddlInterestIsCheckBox(e.Item.Cells(2).Text.Trim)

            If isCheckBoxAI Then
                ' Checkboxed AI's - loads the ddl, formats the inpiut fields
                LoadAdditionalInterestTypeDropdownForCheckboxAI(e.Item.Cells(2).Text)
            Else
                ' Non-checkboxed AI's - loads the ddl
                LoadAdditionalInterestTypeDropdownForNonCheckboxAI()
            End If

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

            ' POPULATE THE INPUT CONTROLS
            Me.additionalInsuredTYPErow.Visible = True
            If (Not isCheckBoxAI) Then
                ' Applies to non-checkboxed AI's only.
                ' Control formatting/visibility and loading of type ddl for checkboxed AI's gets set in LoadAdditionalInterestTypeDropdownForCheckboxAI
                Me.addInameROW.Visible = False
                Me.addIpremisesROW.Visible = False
                Me.addIvendorProductsROW.Visible = False
                Me.addIvendorProductsROW.Visible = False

                'type
                If e.Item.Cells(AIGridCellName.Type).Text <> "" Then
                    'this will not be empty
                    '21081 - 
                    If IsNumeric(e.Item.Cells(AIGridCellName.NumType).Text) And Not String.IsNullOrEmpty(e.Item.Cells(AIGridCellName.Name).Text) And e.Item.Cells(AIGridCellName.Name).Text <> "&nbsp;" And Not e.Item.Cells(AIGridCellName.NumType).Text = "21081" Then
                        Me.DropDownList_additonalInsuredType.SelectedValue = e.Item.Cells(AIGridCellName.NumType).Text
                        DropDownList_additonalInsuredType_SelectedIndexChanged(Me, New EventArgs)
                    Else
                        Me.DropDownList_additonalInsuredType.SetFromValue("0")
                    End If

                End If
            End If

            'name
            If e.Item.Cells(AIGridCellName.Name).Text <> "" AndAlso e.Item.Cells(AIGridCellName.Name).Text <> "&nbsp;" Then
                'if nothing is entered the datagrid has a space
                'Me.addInameROW.Visible = True
                Me.TextBox_addInameOfOrg.Text = e.Item.Cells(AIGridCellName.Name).Text
            Else
                'Me.addInameROW.Visible = True
                Me.TextBox_addInameOfOrg.Text = ""
            End If

            If e.Item.Cells(AIGridCellName.DesigPrem).Text <> "" AndAlso e.Item.Cells(AIGridCellName.DesigPrem).Text <> "&nbsp;" Then
                'Me.addIpremisesROW.Visible = True
                Me.TextBox_addIdesignation.Text = e.Item.Cells(AIGridCellName.DesigPrem).Text
            Else
                'Me.addIpremisesROW.Visible = False
                Me.TextBox_addIdesignation.Text = ""
            End If

            'Product
            If e.Item.Cells(AIGridCellName.Products).Text <> "" AndAlso e.Item.Cells(AIGridCellName.Products).Text <> "&nbsp;" Then
                'Me.addIvendorProductsROW.Visible = True
                Me.TextBox_addIvendorProducts.Text = e.Item.Cells(AIGridCellName.Products).Text
            Else
                'Me.addIvendorProductsROW.Visible = False
                Me.TextBox_addIvendorProducts.Text = ""
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
            '    Me.addIpremROW.Visible = True
            '    Me.TextBox_addIprem.Text = e.Item.Cells(7).Text
            'Else
            '    Me.addIpremROW.Visible = False
            'End If

            'Me.addIbuttonROW.Visible = True
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

    Private Function IsCheckboxAI(ByVal AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured) As Boolean
        If AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest _
            OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises _
            OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors _
            OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver _
            OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased Then
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Returns true if the AI is a checkboxed AI (it has a checkbox on the quote side), false if not
    ''' </summary>
    ''' <param name="AIName"></param>
    ''' <returns></returns>
    Private Function AddlInterestIsCheckBox(ByVal AIName As String) As Boolean
        Select Case AIName.ToUpper()
            Case "CO-OWNER OF INSURED PREMISES"
                Return True
                Exit Select
            Case "CONTROLLING INTERESTS"
                Return True
                Exit Select
            Case "ENGINEERS, ARCHITECTS OR SURVEYORS"
                Return True
                Exit Select
            'Case "ENGINEERS, ARCHITECTS OR SURVEYORS NOT ENGAGED BY THE NAMED INSURED"
            '    Return True
            '    Exit Select
            Case "MORTGAGEE, ASSIGNEE OR RECEIVER"
                Return True
                Exit Select
            Case "OWNER OR OTHER INTERESTS FROM WHOM LAND HAS BEEN LEASED"
                Return True
                Exit Select
            Case Else
                Exit Select
        End Select
        Return False
    End Function

    ''' <summary>
    ''' Loads the type dropdown with a specific checkbox AI (AI that has a checkbox on the quote side).
    ''' Used when editing a checkbox AI.
    ''' </summary>
    ''' <param name="AIName"></param>
    Private Sub LoadAdditionalInterestTypeDropdownForCheckboxAI(ByVal AIName As String)
        Dim val As String = Nothing
        Dim reqName As Boolean = False
        Dim txtName As String = Nothing
        Dim reqPremesis As Boolean = False
        Dim txtPremesis As String = Nothing
        Dim reqWaiver As String = Nothing
        Dim reqVendor As String = Nothing

        Select Case AIName.ToUpper()
            Case "CO-OWNER OF INSURED PREMISES"
                val = "21018"
                reqName = True
                txtName = Nothing
                reqPremesis = True
                txtPremesis = "*Location of Premises"
                reqWaiver = True
                reqVendor = False
                Exit Select
            Case "CONTROLLING INTERESTS"
                val = "926"
                reqName = True
                txtName = Nothing
                reqPremesis = False
                txtPremesis = Nothing
                reqWaiver = True
                reqVendor = False
                Exit Select
            Case "ENGINEERS, ARCHITECTS OR SURVEYORS"
                val = "21019"
                reqName = False
                txtName = Nothing
                reqPremesis = False
                txtPremesis = Nothing
                reqWaiver = True
                reqVendor = False
                Exit Select
            'Case "ENGINEERS, ARCHITECTS OR SURVEYORS NOT ENGAGED BY THE NAMED INSURED"
            '    val = "21023"
            '    reqName = True
            '    txtName = "*Name of Additional Insured Engineers, Architects or Surveyors Not Engaged by the Named Insured"
            '    reqPremesis = False
            '    txtPremesis = Nothing
            '    reqWaiver = True
            '    reqVendor = False
            '    Exit Select
            Case "MORTGAGEE, ASSIGNEE OR RECEIVER"
                val = "21054"
                reqName = True
                txtName = Nothing
                reqPremesis = True
                txtPremesis = "*Designation of Premises"
                reqWaiver = True
                reqVendor = False
                Exit Select
            Case "OWNER OR OTHER INTERESTS FROM WHOM LAND HAS BEEN LEASED"
                val = "21055"
                reqName = True
                txtName = Nothing
                reqPremesis = True
                txtPremesis = "*Designation of Premises (Part Leased by You)"
                reqWaiver = True
                reqVendor = False
                Exit Select
            Case Else
                Exit Select
        End Select

        If val IsNot Nothing Then
            DropDownList_additonalInsuredType.Items.Clear()
            Dim li As New ListItem
            li.Text = AIName
            li.Value = val
            DropDownList_additonalInsuredType.Items.Add(li)
            'DropDownList_additonalInsuredType_SelectedIndexChanged(Me, New EventArgs())

            ' Set the input fields for the selected AI
            If reqName Then Me.addInameROW.Visible = True
            If txtName IsNot Nothing Then Me.spnName.InnerText = txtName
            If reqPremesis Then
                Me.addIpremisesROW.Visible = True
                Me.spnPremises.InnerText = txtPremesis
            End If
            If reqWaiver Then Me.addIwaiverROW.Visible = True
            If reqVendor Then Me.addIvendorProductsROW.Visible = True
            Me.addIbuttonROW.Visible = True
        End If

        Exit Sub
    End Sub

    ''' <summary>
    ''' Loads the type dropdown with all non-checkbox AI's (AI's that don't have a checkbox po the quote side)
    ''' </summary>
    Private Sub LoadAdditionalInterestTypeDropdownForNonCheckboxAI()
        DropDownList_additonalInsuredType.Items.Clear()

        Dim li As New ListItem
        li.Text = "Select"
        li.Value = "0"
        DropDownList_additonalInsuredType.Items.Add(li)

        ' Add 'City of Chicago - Scaffolding' when multistate is active and quote has illinois on it
        If SubQuotesContainsState("IL") Then
            li = New ListItem()
            li.Text = "City of Chicago - Scaffolding"
            li.Value = "80537"
            DropDownList_additonalInsuredType.Items.Add(li)
        End If

        li = New ListItem()
        li.Text = "Designated Person Or Organization"
        li.Value = "21022"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "Lessor of Leased Equipment"
        li.Value = "21020"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "Managers or Lessors of Premises"
        li.Value = "21053"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "Owners, Lessees or Contractors"
        li.Value = "21024"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "Engineers, Architects or Surveyors Not Engaged by the Named Insured"
        li.Value = "21023"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "State or Political Subdivision - Permits Relating to Premises"
        li.Value = "21016"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "State or Political Subdivisions - Permits"
        li.Value = "21026"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "Townhouse Associations"
        li.Value = "21017"
        DropDownList_additonalInsuredType.Items.Add(li)

        li = New ListItem()
        li.Text = "Vendors"
        li.Value = "21021"
        DropDownList_additonalInsuredType.Items.Add(li)

        Exit Sub
    End Sub

    Protected Sub Button_addInsSaveAdd_Click(sender As Object, e As System.EventArgs) Handles Button_addInsSaveAdd.Click
        saveAdditionalInsured()

        Me.ValidateControl(New VRValidationArgs)
    End Sub

    Protected Sub Button_addInsCancel_Click(sender As Object, e As System.EventArgs) Handles Button_addInsCancel.Click

        Me.spnAIStatusText.InnerText = ""
        Me.spnAIStatus.Visible = False
        Me.addInameROW.Visible = False
        Me.addIpremisesROW.Visible = False
        Me.addIvendorProductsROW.Visible = False
        Me.addIwaiverROW.Visible = False
        Me.addIbuttonROW.Visible = False

        Me.TextBox_addInameOfOrg.Text = ""
        Me.TextBox_addIdesignation.Text = ""
        Me.TextBox_addIvendorProducts.Text = ""
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
        'Me.ValidateAIEdit(New VRValidationArgs)

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
                Dim prem = GetPremForAIValue(Me.DropDownList_additonalInsuredType.SelectedValue)
                dr.Item("Premium") = prem
                'dr.Item("Premium") = _PremiumPerAInsured 
                dr.Item("Waiver") = addInsuredcheckbox
                dr.Item("Products") = Me.TextBox_addIvendorProducts.Text

                Me.Session.Item("isEditing") = False
                Me.ValidateAIEdit(New VRValidationArgs)
                UpdateAIEditBox()
            Else
                If Me.addIwaiverROW.Visible AndAlso Not Me.ValidationHelper.HasErrros Then
                    Dim prem = GetPremForAIValue(Me.DropDownList_additonalInsuredType.SelectedValue)
                    If DTadditionalInsured.Rows.Count = 0 Then
                        'first
                        DTadditionalInsured.Rows.Add(0, Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, prem, addInsuredcheckbox, Me.TextBox_addIvendorProducts.Text.Trim)
                        'DTadditionalInsured.Rows.Add(0, Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, _PremiumPerAInsured, addInsuredcheckbox, Me.TextBox_addIvendorProducts.Text.Trim)
                    Else
                        '2nd, 3rd, etc
                        DTadditionalInsured.Rows.Add((DTadditionalInsured.Rows.Count), Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, prem, addInsuredcheckbox, Me.TextBox_addIvendorProducts.Text.Trim)
                        'DTadditionalInsured.Rows.Add((DTadditionalInsured.Rows.Count), Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, _PremiumPerAInsured, addInsuredcheckbox, Me.TextBox_addIvendorProducts.Text.Trim)
                    End If
                    Me.ValidateAIEdit(New VRValidationArgs)
                    UpdateAIEditBox()
                End If
                ' (count), CInt(ai.AdditionalInsuredType).ToString(), aiGridText, ai.NameOfPersonOrOrganization, ai.DesignationOfPremises, ai.ManualPremiumAmount, waiver, ai.ProductDescription
            End If
        End If


        'UpdateAIEditBox()
    End Sub

    Private Function GetPremForAIValue(ByVal AIValue As String) As String
        ' Do not add the $25 premium for the following additional insured types:
        ' 21018 Co Owner of insured premises
        ' 926 Controlling Interests
        ' 21019 Engineers, Architects or Sureveyors
        ' 21054 Mortgagee, Assignee or Receiver
        ' 21055 Owner or other interests from whom land has been leased
        If Me.DropDownList_additonalInsuredType.SelectedValue = "21018" _
        OrElse Me.DropDownList_additonalInsuredType.SelectedValue = "926" _
        OrElse Me.DropDownList_additonalInsuredType.SelectedValue = "21019" _
        OrElse Me.DropDownList_additonalInsuredType.SelectedValue = "21054" _
        OrElse Me.DropDownList_additonalInsuredType.SelectedValue = "21055" Then
            Return "0"
        Else
            Return _PremiumPerAInsured
        End If
    End Function

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
        Me.ValidateControl(New VRValidationArgs)
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

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each dtrow As DataRow In dt.Rows
                        If ValidateByType(dtrow, False) Then
                            '??
                        End If
                    Next
                End If
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Function ValidateAIEdit(valArgs As VRValidationArgs) As Boolean
        Me.ValidationHelper.GroupName = "Additional Insureds"
        Dim waiver = ""
        Dim testResult = False
        ' Check Type dropdown if editing
        If Me.DropDownList_additonalInsuredType.SelectedItem.Text = "Select" And Me.TextBox_addInameOfOrg.Visible Then
            Me.ValidationHelper.AddError("Missing Type", Me.DropDownList_additonalInsuredType.ClientID)
            testResult = True
        End If

        'Check Required Fields
        Dim DTadditionalInsured As New DataTable
        DTadditionalInsured.Columns.Add("Num")
        DTadditionalInsured.Columns.Add("NumType")
        DTadditionalInsured.Columns.Add("Type")
        DTadditionalInsured.Columns.Add("Name")
        DTadditionalInsured.Columns.Add("DesigPrem")
        DTadditionalInsured.Columns.Add("Premium")
        DTadditionalInsured.Columns.Add("Waiver")
        DTadditionalInsured.Columns.Add("Products")

        If Me.CheckBox_addIwaiver.Checked = True Then
            waiver = "Yes"
        ElseIf Me.CheckBox_addIwaiver.Checked = False Then
            waiver = "No"
        End If

        DTadditionalInsured.Rows.Add(0, Me.DropDownList_additonalInsuredType.SelectedValue, Me.DropDownList_additonalInsuredType.SelectedItem.Text, Me.TextBox_addInameOfOrg.Text, Me.TextBox_addIdesignation.Text, 25, waiver, Me.TextBox_addIvendorProducts.Text)

        If ValidateByType(DTadditionalInsured.Rows.Item(0), True) Then
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
        Me.addIvendorProductsROW.Visible = False
        Me.addIwaiverROW.Visible = False
        'Me.addIpremROW.Visible = False
        Me.addIbuttonROW.Visible = True

        Me.TextBox_addInameOfOrg.Text = ""
        Me.TextBox_addIdesignation.Text = ""
        Me.TextBox_addIvendorProducts.Text = ""
        Me.CheckBox_addIwaiver.Checked = False
        'Me.TextBox_addIprem.Text = ""
        Me.addIbuttonROW.Visible = False
        Me.DropDownList_additonalInsuredType.SetFromValue("0")
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

    ''' <summary>
    ''' Validates Additional Insureds items by row
    ''' </summary>
    ''' <param name="dtrow">datarow containing item data</param>
    ''' <param name="editorValidation">True if validating editor boxes, False if validating the list</param>
    ''' <returns>True if errors, False if no errors</returns>
    Protected Function ValidateByType(dtrow As DataRow, editorValidation As Boolean) As Boolean
        '21018 > Co - Owner of Insured Premises
        '501 > Controlling Interests
        '21022 > Designated Person Or Organization
        '21019 > Engineers, Architects or Surveyors
        '21023 > Engineers, Architects Or Surveyors Not Engaged by the Named Insured
        '21020 > Lessor of Leased Equipment
        '21053 > Managers Or Lessors of Premises
        '21054 > Mortgagee, Assignee Or Receiver
        '21055 > Owner Or Other Interests From Whom Land has been Leased
        '21024 > Owners, Lessees or Contractors
        '21016 > State Or Political Subdivision - Permits Relating to Premises
        '21026 > State or Political Subdivisions - Permits
        '21017 > Townhouse Associations
        '21021 > Vendors

        Dim Row = dtrow.Item("Num")
        Dim TypeNumber = dtrow.Item("NumType")
        Dim Name = dtrow.Item("Name")
        Dim Prem = dtrow.Item("DesigPrem")
        Dim Products = dtrow.Item("Products")

        Dim errorMsg As String = String.Empty
        Dim hasError As Boolean

        'Type 0 - No selection
        Select Case TypeNumber
            Case 0
                If Name Is Nothing OrElse String.IsNullOrEmpty(Name) = True Then
                    errorMsg = "Missing Insured Type"
                End If
                If errorMsg IsNot Nothing AndAlso String.IsNullOrEmpty(errorMsg) = False Then
                    If editorValidation Then
                        Return hasError
                    Else
                        errorMsg += " - Row: " & (CInt(Row) + 1)
                        Me.ValidationHelper.AddError(errorMsg, Me.DataGrid_additionalInsured.ClientID & " multi")
                    End If
                    errorMsg = String.Empty
                    hasError = True
                    Return hasError
                End If
        End Select

        'Name
        Select Case TypeNumber
            Case "21018", "926", "21022", "21020", "21053", "21054", "21055", "21024", "21021"
                If Name Is Nothing OrElse String.IsNullOrEmpty(Name) = True Then
                    errorMsg = "Missing Name of Person or Organization"
                End If
            Case "21023"
                If Name Is Nothing OrElse String.IsNullOrEmpty(Name) = True Then
                    errorMsg = "Missing Additional Insured Engineers, Architects or Surveyors Not Engaged by the Named Insured"
                End If
            Case "21016", "21026"
                If Name Is Nothing OrElse String.IsNullOrEmpty(Name) = True Then
                    errorMsg = "Missing State or Political Subdivision"
                End If
        End Select
        If errorMsg IsNot Nothing AndAlso String.IsNullOrEmpty(errorMsg) = False Then
            If editorValidation Then
                Me.ValidationHelper.AddError(errorMsg, Me.TextBox_addInameOfOrg.ClientID)
            Else
                errorMsg += " - Row: " & (CInt(Row) + 1)
                Me.ValidationHelper.AddError(errorMsg, Me.DataGrid_additionalInsured.ClientID & " multi")
            End If
            errorMsg = String.Empty
            hasError = True
        End If


        'Premises
        Select Case TypeNumber
            Case "21018"
                If Prem Is Nothing OrElse String.IsNullOrEmpty(Prem) = True Then
                    errorMsg = "Missing Location of Premises"
                End If
            Case "21054"
                If Prem Is Nothing OrElse String.IsNullOrEmpty(Prem) = True Then
                    errorMsg = "Missing Designation of Premises"
                End If
            Case "21053", "21055"
                If Prem Is Nothing OrElse String.IsNullOrEmpty(Prem) = True Then
                    errorMsg = "Missing Designation of Premises (Part Leased by You)"
                End If
            Case "21024"
                If Prem Is Nothing OrElse String.IsNullOrEmpty(Prem) = True Then
                    errorMsg = "Missing Location of Covered Operations"
                End If
        End Select
        If errorMsg IsNot Nothing AndAlso String.IsNullOrEmpty(errorMsg) = False Then
            If editorValidation Then
                Me.ValidationHelper.AddError(errorMsg, Me.TextBox_addIdesignation.ClientID)
            Else
                errorMsg += " - Row: " & (CInt(Row) + 1)
                Me.ValidationHelper.AddError(errorMsg, Me.DataGrid_additionalInsured.ClientID & " multi")
            End If
            errorMsg = String.Empty
            hasError = True
        End If


        'Products
        Select Case TypeNumber
            Case "21021"
                If Products Is Nothing OrElse String.IsNullOrEmpty(Products) = True Then
                    errorMsg = "Missing Your Products"
                End If
        End Select
        If errorMsg IsNot Nothing AndAlso String.IsNullOrEmpty(errorMsg) = False Then
            If editorValidation Then
                Me.ValidationHelper.AddError(errorMsg, Me.TextBox_addIvendorProducts.ClientID)
            Else
                errorMsg += " - Row: " & (CInt(Row) + 1)
                Me.ValidationHelper.AddError(errorMsg, Me.DataGrid_additionalInsured.ClientID & " multi")
            End If
            errorMsg = String.Empty
            hasError = True
        End If

        Return hasError
    End Function

End Class