Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.CAP
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass

Public Class ctl_CAP_Coverages
    Inherits VRControlBase

#Region "Declarations"

    Private Property DefaultsWereSet() As Boolean
        Get
            Return ViewState("VR_CGLDefaultsSet")
        End Get
        Set(value As Boolean)
            ViewState("VR_CGLDefaultsSet") = value
        End Set
    End Property

    Private ReadOnly Property QuoteHasIllinois() As String
        Get
            If SubQuotesContainsState("IL") Then Return "T" Else Return "F"
        End Get
    End Property


#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Try
            Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hddAccord, "0")
            Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

            ' Coverage checkboxes script
            chkEnhancement.Attributes.Add("onchange", "Cap.EnhancementCheckboxChanged('" & chkEnhancement.ClientID & "', '" & chkBlanketWaiverOfSubro.ClientID & "');")
            chkBlanketWaiverOfSubro.Attributes.Add("onchange", "Cap.BlanketWaiverOfSubroCheckboxChanged('" & chkBlanketWaiverOfSubro.ClientID & "', '" & chkEnhancement.ClientID & "');")

            ' The Hires/Borrowed/Non-Owned binding gets set in UpdateDynamicBindings
            'chkHiredBorrowedNonOwned.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('HBNO','" & chkHiredBorrowedNonOwned.ClientID & "','" & trHiredBorrowedNonOwnedDataRow.ClientID & "', '" & trNonOwnershipLiabilityDataRow.ClientID & "', '" & trHiredCarPhysicalDamageDataRow.ClientID & "','" & trHiredBorrowedLiabilityDataRow.ClientID & "');")

            chkNonOwnershipLiability.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('NONOWN','" & chkNonOwnershipLiability.ClientID & "', '" & trNonOwnershipLiabilityDataRow.ClientID & "', '', '', '');")
            chkHiredBorrowedLiability.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('HB','" & chkHiredBorrowedLiability.ClientID & "', '" & trHiredBorrowedLiabilityDataRow.ClientID & "', '', '', '');")
            chkHiredCarPhysicalDamage.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('HCPD','" & chkHiredCarPhysicalDamage.ClientID & "', '" & trHiredCarPhysicalDamageDataRow.ClientID & "', '', '', '');")

            chkFarmPollutionLiability.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('FP','" & chkFarmPollutionLiability.ClientID & "', '', '', '', '');")
            chkGarageKeepers.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('GK','" & chkGarageKeepers.ClientID & "','" & trGarageKeepersDataRow.ClientID & "', '', '', '');")

            ddUM.Attributes.Add("onchange", "Cap.UMDropdownValueChanged('" & ddUM.ClientID & "','" & txtUIM.ClientID & "');")

            UpdateDynamicBindings()

            Dim strILUMPDDropDownChanged As String = "Cap.UMPDDropDownChanged('" & ddlUMPDLimit.ClientID & "','" & hdnUMPDLimitValue.ClientID & "');"

            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                Select Case Quote.QuickQuoteState
                    Case QuickQuoteState.Indiana
                        chkUMPDCov.Attributes.Add("onchange", "Cap.PolicyLevelUMPDCheckboxChanged('" & chkUMPDCov.ClientID & "','" & divUMPDDedOptions.ClientID & "','" & Quote.HasMultipleQuoteStates.ToString() & "');")
                    Case QuickQuoteState.Illinois
                        If Quote.HasMultipleQuoteStates Then
                            strILUMPDDropDownChanged &= "Cap.PolicyLevelUMPDLimitChanged('" & ddlUMPDLimit.ClientID & "');"
                        End If
                End Select
            End If

            If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                ddlUMPDLimit.Attributes.Add("onchange", strILUMPDDropDownChanged)
            End If

            If Quote.QuickQuoteState = QuickQuoteState.Ohio Then
                Dim isUMPDChangesAvailable As Boolean = UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote)
                btnVehicles.Attributes.Add("onclick", "Cap.VehicleButtonClicked('" & isUMPDChangesAvailable.ToString() & "','" & Quote.HasMultipleQuoteStates.ToString() & "');")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("AddScriptWhenRendered", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' This sub is called to update any control bindings with parameters that can change as data is saved
    ''' The example below is for Hired/Borrowed/Non-Owned.  The user could change the quote so that Illinois is added/removed from the quote so we need to update the 
    ''' script call that handles the checkbox change and displays the info row when the quote has illinois.
    ''' </summary>
    Private Sub UpdateDynamicBindings()
        'chkHiredBorrowedNonOwned.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('HBNO','" & chkHiredBorrowedNonOwned.ClientID & "','" & trHiredBorrowedNonOwnedDataRow.ClientID & "', '" & trNonOwnershipLiabilityDataRow.ClientID & "', '" & trHiredCarPhysicalDamageDataRow.ClientID & "','" & trHiredBorrowedLiabilityDataRow.ClientID & "');")
        chkHiredBorrowedNonOwned.Attributes.Add("onchange", "Cap.HiredBorrowedNonOwnedCheckboxChanged('" & chkHiredBorrowedNonOwned.ClientID & "','" & trHiredNonOwnedInfoRow.ClientID & "','" & QuoteHasIllinois() & "','" & trHiredBorrowedNonOwnedDataRow.ClientID & "','" & trNonOwnershipLiabilityDataRow.ClientID & "','" & trHiredCarPhysicalDamageDataRow.ClientID & "','" & trHiredBorrowedLiabilityDataRow.ClientID & "');")
    End Sub


    Public Overrides Sub LoadStaticData()
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()

        Try
            ' We only want to load the ddl's once
            If ddLiability.Items IsNot Nothing AndAlso ddLiability.Items.Count > 0 Then Exit Sub

            ' UM/UIM combined
            QQHelper.LoadStaticDataOptionsDropDown(ddlLiabilityUMUIM, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, , Quote.LobType)

            ' New Liability / UM / UIM
            LoadLiabilityStaticData() 'moved to its own sub so date crossing logic can also use this

            'Updated 03/12/2021 for CAP Endorsements Task 52979 MLW
            If IsQuoteReadOnly() Then
                QQHelper.LoadStaticDataOptionsDropDown(ddUM, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, ,)
            Else
                LoadUMStaticData() 'moved to its own sub so date crossing logic can also use this
            End If
            'QQHelper.LoadStaticDataOptionsDropDown(ddUM, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, , Quote.LobType)

            LoadUMPDStaticData() 'moved to its own sub so date crossing logic can also use this

            LoadMedPayStaticData() 'moved to its own sub so date crossing logic can also use this

            LoadPolicyCompCollStaticData()

            'Updated 02/15/2021 for CAP Endorsements Task 52972 MLW
            If Not IsQuoteReadOnly() Then
                SetDropDownDefaults()
            End If
            'SetDropDownDefaults()

            Exit Sub
        Catch ex As Exception
            HandleError("LoadStaticData", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub SetDropDownDefaults()
        ' SET DDL DEFAULTS

        If DefaultsWereSet Then Return   ' Don't keep re-setting the defaults

        ' Liability UM/UIM - 1,000,000  (old - combined)
        For Each li As ListItem In ddlLiabilityUMUIM.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 1000000 Then
                    ddlLiabilityUMUIM.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' Liability - $1,000,000  (new - separated)
        For Each li As ListItem In ddLiability.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 1000000 Then
                    ddLiability.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' UM - $1,000,000
        For Each li As ListItem In ddUM.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 1000000 Then
                    ddUM.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' Medical Payments - 5,000
        For Each li As ListItem In ddlMedicalPayments.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 5000 Then
                    ddlMedicalPayments.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' Garage Keepers Basis - Direct Primary (ID 1)
        For Each li As ListItem In ddlBasisType.Items
            If li.Value.IsNumeric Then
                Dim i As Integer = CInt(li.Value)
                If i = 1 Then
                    ddlBasisType.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' Garage Keepers Comprehensive Deductible - Blank
        For Each li As ListItem In ddlGKComprehensiveDeductible.Items
            If li.Text.Trim = "" Then
                ddlGKComprehensiveDeductible.SelectedValue = Nothing
                li.Selected = True
                Exit For
            End If
        Next

        ' Garage Keepers Collision Deductible - 500
        For Each li As ListItem In ddlGKCollisionDeductible.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 500 Then
                    ddlGKCollisionDeductible.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' Hired Car Physical Damage Comprehensive - 0
        For Each li As ListItem In ddlHiredCarComprehensive.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 0 Then
                    ddlHiredCarComprehensive.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        ' Hired Car Physical Damage Collision - 0
        For Each li As ListItem In ddlHiredCarCollision.Items
            If li.Text.IsNumeric Then
                Dim i As Integer = CInt(li.Text)
                If i = 0 Then
                    ddlHiredCarCollision.SelectedValue = Nothing
                    li.Selected = True
                    Exit For
                End If
            End If
        Next

        DefaultsWereSet = True

    End Sub

    Public Overrides Sub Populate()

        PopulateCoverageLayout() 'moved to its own sub so date crossing logic can also use this

        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() Then
            Dim CompletedOpsCount As Integer = 0
            Try
                LoadStaticData()
                'updated 8/8/2018; note: code below updated to use quoteToUse instead of Me.Quote/Quote (would be the same in the case of a single-state quote)
                If Me.Quote IsNot Nothing Then
                    trLiabilityUMUIM_Combined.Attributes.Add("style", "display:none")
                    trLiabilityUMUIM_Separate.Attributes.Add("style", "display:none")

                    ' Ohio Question - if Ohio is on the quote then show this coverage
                    ' Retrieve from the Ohio subquote
                    trOHQuestionRow.Attributes.Add("style", "display:none")
                    If SubQuotesContainsState("OH") Then
                        If Quote.Policyholder.Name.EntityTypeId = "1" Then ' Only show for individual
                            Dim OHQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio)
                            If OHQuote IsNot Nothing Then
                                trOHQuestionRow.Attributes.Add("style", "display:''")
                                Select Case OHQuote.LegalEntityType
                                    Case TriState.UseDefault
                                        SetFromValue(ddlOhioQuestion, "NA")
                                        Exit Select
                                    Case TriState.False
                                        SetFromValue(ddlOhioQuestion, "NO")
                                        Exit Select
                                    Case TriState.True
                                        SetFromValue(ddlOhioQuestion, "YES")
                                        Exit Select
                                    Case Else
                                        SetFromValue(ddlOhioQuestion, "NA")
                                        Exit Select
                                End Select
                            End If
                        End If
                    End If

                    If SubQuoteFirst IsNot Nothing Then
                        If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                            trLiabilityUMUIM_Separate.Attributes.Add("style", "display:''")
                            ' LIABILITY, UM, UIM (new fields - separate)
                            'If SubQuoteFirst.Liability_UM_UIM_LimitId = "" Then
                            '    SetDropDownDefaults()
                            'Else
                            'Updated 02/15/2021 for CAP Endorsements Tasks 52972 and 52979 MLW
                            If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddLiability, SubQuoteFirst.Liability_UM_UIM_LimitId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId)
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddUM, SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId)
                            Else
                                If Not IsNullEmptyorWhitespace(SubQuoteFirst.Liability_UM_UIM_LimitId) Then SetFromValue(ddLiability, SubQuoteFirst.Liability_UM_UIM_LimitId)
                                If Not IsNullEmptyorWhitespace(SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId) Then SetFromValue(ddUM, SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId)
                            End If
                            'If Not IsNullEmptyorWhitespace(SubQuoteFirst.Liability_UM_UIM_LimitId) Then SetFromValue(ddLiability, SubQuoteFirst.Liability_UM_UIM_LimitId)
                            'If Not IsNullEmptyorWhitespace(SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId) Then SetFromValue(ddUM, SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId)
                            ''End If
                            '' UIM is always the same as UM
                            ''Dim umpd As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, ddLiability.SelectedValue)
                            Dim umpd As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, ddUM.SelectedValue)
                            txtUIM.Text = umpd  ' Always set to same value as UM

                            PopulateUMPDCoverage() 'moved to its own sub so date crossing logic can also use this
                        Else
                            trLiabilityUMUIM_Combined.Attributes.Add("style", "display:''")
                            ' UM/UIM (combined)
                            SetFromValue(ddlLiabilityUMUIM, SubQuoteFirst.Liability_UM_UIM_LimitId, "56")
                        End If

                        PopulateMedPayDropDown()

                        ' Enhancement Endorsement
                        If SubQuoteFirst.HasBusinessMasterEnhancement Then
                            chkEnhancement.Checked = True
                        Else
                            chkEnhancement.Checked = False
                        End If

                        ' Blanket Waiver of Subro
                        If SubQuoteFirst.BlanketWaiverOfSubrogation = "3" Then
                            chkBlanketWaiverOfSubro.Checked = True
                        Else
                            chkBlanketWaiverOfSubro.Checked = False
                        End If

                        ' Show the Hired/Borrowed Non-Owned info message if quote has illinois regardless of whether the coverage is selected or not
                        If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso QuoteHasIllinois() = "T" Then
                            trHiredNonOwnedInfoRow.Attributes.Add("style", "display:''")
                        Else
                            trHiredNonOwnedInfoRow.Attributes.Add("style", "display:none")
                        End If

                        'Added 02/16/2021 for CAP Endorsements Task 52972 MLW
                        If IsQuoteReadOnly() Then
                            ddlHiredCarComprehensive.Items.Add(New ListItem("", ""))
                            ddlHiredCarComprehensive.Items.Add(New ListItem("N/A", "0"))
                            ddlHiredCarCollision.Items.Add(New ListItem("", ""))
                            ddlHiredCarCollision.Items.Add(New ListItem("N/A", "0"))
                        End If

                        PopulateHiredBorrowedNonOwned()

                        '8/8/2018 note: following code was moved into this IF block that verifies Quote is something
                        ' Farm Pollution Liability
                        If SubQuoteFirst.HasFarmPollutionLiability Then
                            chkFarmPollutionLiability.Checked = True
                        Else
                            chkFarmPollutionLiability.Checked = False
                        End If

                        PopulateGarageKeepers()

                    End If

                    'Added 11/16/2020 for task 52979 MLW
                    If IsQuoteReadOnly() Then
                        Dim policyNumber As String = Me.Quote.PolicyNumber
                        Dim imageNum As Integer = 0
                        Dim policyId As Integer = 0
                        Dim toolTip As String = "Make a change to this policy"
                        Dim readOnlyViewPageUrl As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                        If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                            readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                        End If

                        divActionButtons.Visible = False
                        divEndorsementButtons.Visible = True

                        btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                        readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                        btnMakeAChange.ToolTip = toolTip
                        btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl

                        lnkCAPHelp_EnhancementEndorsement.Attributes.Add("style", "color:#333333;text-decoration:underline;")
                    Else
                        divActionButtons.Visible = True
                        divEndorsementButtons.Visible = False

                        lnkCAPHelp_EnhancementEndorsement.Attributes.Add("style", "color:blue;")
                        lnkCAPHelp_EnhancementEndorsement.Attributes.Add("href", System.Configuration.ConfigurationManager.AppSettings("CAP_Help_EnhancementSummary"))
                    End If
                    If Not IsQuoteEndorsement() Then
                        Me.PersonalLiabilitylimitTextCAP.Visible = True
                    End If
                End If


                Exit Sub
            Catch ex As Exception
                HandleError("Populate", ex)
                Exit Sub
            End Try
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() Then
            Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured = Nothing
            Dim HasAI As Boolean = False

            Try
                If Me.Quote IsNot Nothing Then
                    If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                        For Each stateQuote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                            If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                                ' Multistate - save from the separated Liability, UM, UIM fields
                                'If stateQuote.Liability_UM_UIM_LimitId = "" Then
                                '    ' The first time through these fields will be empty so we want to set the defaults to whatever 
                                '    stateQuote.Liability_UM_UIM_LimitId = "56"  ' 58 = $1,000,000
                                '    stateQuote.UninsuredMotoristPropertyDamageLimitId = "56"
                                '    stateQuote.UnderinsuredMotoristBodilyInjuryLiabilityLimitId = "56"
                                '    ' These MUST be set here because this save is performed before we come to the page
                                '    SetFromValue(ddLiability, "56")
                                '    SetFromValue(ddUM, "56")
                                '    txtUIM.Text = ddUM.SelectedItem.Text
                                'Else
                                stateQuote.Liability_UM_UIM_LimitId = ddLiability.SelectedValue
                                stateQuote.UninsuredMotoristPropertyDamageLimitId = ddUM.SelectedValue
                                stateQuote.UnderinsuredMotoristBodilyInjuryLiabilityLimitId = ddUM.SelectedValue ' UIM uses the same value as UM                               
                                'End If
                            Else
                                ' Not multistate - save from combined Liability/UM/UIM
                                stateQuote.Liability_UM_UIM_LimitId = ddlLiabilityUMUIM.SelectedValue
                            End If

                            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                                Select Case Quote.QuickQuoteState
                                    Case QuickQuoteHelperClass.QuickQuoteState.Indiana
                                        If chkUMPDCov.Checked Then
                                            stateQuote.UninsuredMotoristPropertyDamageDeductibleId = ddUMPDDedOptions.SelectedValue
                                        Else
                                            stateQuote.UninsuredMotoristPropertyDamageDeductibleId = "0"
                                        End If
                                End Select
                            End If

                            If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) AndAlso stateQuote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                                stateQuote.UninsuredMotoristPropertyDamage_IL_LimitId = ddlUMPDLimit.SelectedValue
                                If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) AndAlso Quote.QuickQuoteState = QuickQuoteState.Illinois AndAlso stateQuote.UninsuredMotoristPropertyDamage_IL_LimitId = "0" Then
                                    stateQuote.UninsuredMotoristPropertyDamage_IL_DeductibleId = "0"
                                Else
                                    stateQuote.UninsuredMotoristPropertyDamage_IL_DeductibleId = "4" '250
                                End If
                                hdnUMPDLimitValue.Value = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, stateQuote.QuickQuoteState, ddlUMPDLimit.SelectedValue, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
                            Else
                                stateQuote.UninsuredMotoristPropertyDamage_IL_LimitId = ""
                                stateQuote.UninsuredMotoristPropertyDamage_IL_DeductibleId = ""
                            End If
                            stateQuote.MedicalPaymentsLimitid = ddlMedicalPayments.SelectedValue
                            If chkEnhancement.Checked Then stateQuote.HasBusinessMasterEnhancement = True Else stateQuote.HasBusinessMasterEnhancement = False
                            If chkBlanketWaiverOfSubro.Checked Then
                                'stateQuote.HasWaiverOfSubrogation = True
                                stateQuote.BlanketWaiverOfSubrogation = "3"
                            Else
                                'stateQuote.HasWaiverOfSubrogation = False
                                stateQuote.BlanketWaiverOfSubrogation = ""
                            End If

                            If chkHiredBorrowedNonOwned.Checked Then
                                stateQuote.HasHiredBorrowedNonOwned = True
                                If chkNonOwnershipLiability.Checked Then
                                    stateQuote.HasNonOwnershipLiability = True
                                    stateQuote.NonOwnershipLiabilityNumberOfEmployees = txtNonOwnedNumberOfEmployees.Text
                                    If ddlNonOwnedExtendedNonOwnedLiability.SelectedIndex > 0 Then
                                        stateQuote.NonOwnership_ENO_RatingTypeId = ddlNonOwnedExtendedNonOwnedLiability.SelectedValue
                                    Else
                                        stateQuote.NonOwnership_ENO_RatingTypeId = ""
                                    End If
                                Else
                                    stateQuote.HasNonOwnershipLiability = False
                                    stateQuote.NonOwnershipLiabilityNumberOfEmployees = ""
                                    stateQuote.NonOwnership_ENO_RatingTypeId = ""
                                End If
                                If chkHiredBorrowedLiability.Checked Then
                                    stateQuote.HasHiredBorrowedLiability = True
                                    If chkHiredCarPhysicalDamage.Checked Then
                                        stateQuote.HasHiredCarPhysicalDamage = True
                                        stateQuote.ComprehensiveDeductibleId = ddlHiredCarComprehensive.SelectedValue
                                        stateQuote.CollisionDeductibleId = ddlHiredCarCollision.SelectedValue
                                    Else
                                        stateQuote.HasHiredCarPhysicalDamage = False
                                        stateQuote.ComprehensiveDeductibleId = ""
                                        stateQuote.CollisionDeductibleId = ""
                                    End If
                                Else
                                    stateQuote.HasHiredBorrowedLiability = False
                                    stateQuote.HasHiredCarPhysicalDamage = False
                                    stateQuote.ComprehensiveDeductibleId = ""
                                    stateQuote.CollisionDeductibleId = ""
                                End If
                            Else
                                stateQuote.HasHiredBorrowedNonOwned = False
                                stateQuote.HasNonOwnershipLiability = False
                                stateQuote.HasHiredBorrowedLiability = False
                                stateQuote.HasHiredCarPhysicalDamage = False
                                stateQuote.NonOwnershipLiabilityNumberOfEmployees = ""
                                stateQuote.NonOwnership_ENO_RatingTypeId = ""
                                stateQuote.ComprehensiveDeductibleId = ""
                                stateQuote.CollisionDeductibleId = ""
                            End If
                            If chkFarmPollutionLiability.Checked Then stateQuote.HasFarmPollutionLiability = True Else stateQuote.HasFarmPollutionLiability = False

                            ' GARAGE KEEPERS
                            If chkGarageKeepers.Checked Then
                                stateQuote.GarageKeepersCollisionBasisTypeId = ddlBasisType.SelectedValue
                                stateQuote.GarageKeepersOtherThanCollisionBasisTypeId = ddlBasisType.SelectedValue
                                ' GK COMPREHENSIVE
                                If txtGKComprehensiveLimit.Text.Trim <> "" Then
                                    stateQuote.HasGarageKeepersOtherThanCollision = True
                                    stateQuote.GarageKeepersOtherThanCollisionManualLimitAmount = txtGKComprehensiveLimit.Text
                                    ' SET 'OTHER THAN COLLISION TYPE' TO 'COMPREHENSIVE'  (ID = 3)
                                    stateQuote.GarageKeepersOtherThanCollisionTypeId = 3
                                    ' SET 'DEDUCTIBLE TYPE' TO 'ALL PERILS' (ID = 3)
                                    stateQuote.GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = 3
                                    stateQuote.GarageKeepersOtherThanCollisionDeductibleId = ddlGKComprehensiveDeductible.SelectedValue
                                Else
                                    stateQuote.HasGarageKeepersOtherThanCollision = False
                                    stateQuote.GarageKeepersOtherThanCollisionManualLimitAmount = ""
                                    stateQuote.GarageKeepersOtherThanCollisionTypeId = ""
                                    stateQuote.GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = ""
                                    stateQuote.GarageKeepersOtherThanCollisionDeductibleId = ""
                                End If
                                ' GK COLLISION
                                If txtGKCollisionLimit.Text.Trim <> "" Then
                                    stateQuote.HasGarageKeepersCollision = True
                                    stateQuote.GarageKeepersCollisionManualLimitAmount = txtGKCollisionLimit.Text
                                    stateQuote.GarageKeepersCollisionDeductibleId = ddlGKCollisionDeductible.SelectedValue
                                Else
                                    stateQuote.HasGarageKeepersCollision = False
                                    stateQuote.GarageKeepersCollisionManualLimitAmount = ""
                                    stateQuote.GarageKeepersCollisionDeductibleId = ""
                                End If
                            Else
                                stateQuote.HasGarageKeepersOtherThanCollision = False
                                stateQuote.HasGarageKeepersCollision = False
                                stateQuote.GarageKeepersCollisionManualLimitAmount = ""
                                stateQuote.GarageKeepersCollisionDeductibleId = ""
                                stateQuote.GarageKeepersOtherThanCollisionManualLimitAmount = ""
                                stateQuote.GarageKeepersOtherThanCollisionTypeId = ""
                                stateQuote.GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = ""
                                stateQuote.GarageKeepersOtherThanCollisionDeductibleId = ""
                            End If

                            ' Ohio Question - save to the Ohio subquote
                            If stateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                                Select Case ddlOhioQuestion.SelectedValue
                                    Case "NA"
                                        stateQuote.LegalEntityType = TriState.UseDefault
                                        Exit Select
                                    Case "NO"
                                        stateQuote.LegalEntityType = TriState.False
                                        Exit Select
                                    Case "YES"
                                        stateQuote.LegalEntityType = TriState.True
                                        Exit Select
                                    Case Else
                                        stateQuote.LegalEntityType = TriState.UseDefault
                                        Exit Select
                                End Select
                            End If

                            '8/8/2018 note: would use similar logic for state-specific items
                            'Select Case stateQuote.QuickQuoteState
                            '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                            '        stateQuote.GarageKeepersOtherThanCollisionManualLimitAmount = Me.txtGarageCompIndiana.text
                            '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                            '        stateQuote.GarageKeepersOtherThanCollisionManualLimitAmount = Me.txtGarageCompIllinois.text
                            'End Select
                        Next
                    End If
                End If

                UpdateDynamicBindings()
                Populate()
                Return True
            Catch ex As Exception
                HandleError("Save", ex)
                Return False
            End Try
        Else
            Return True
        End If
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() Then

            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            Try
                MyBase.ValidateControl(valArgs)

                Me.ValidationHelper.GroupName = "Policy Level Coverages"

                ' Liability / UM / UIM  (Multistate)
                If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                    ' UM cannot be higher than Liability
                    If IsNumeric(ddLiability.SelectedItem.Text) AndAlso IsNumeric(ddUM.SelectedItem.Text) Then
                        Dim Liab As Decimal = CDec(ddLiability.SelectedItem.Text)
                        Dim UM As Decimal = CDec(ddUM.SelectedItem.Text)
                        If UM > Liab Then
                            Me.ValidationHelper.AddError(ddUM, "UM limit selected is higher than the Liability limit, it must be equal or less.", accordList)
                        End If
                    End If
                End If

                If chkBlanketWaiverOfSubro.Checked Then
                    If Not chkEnhancement.Checked Then
                        Me.ValidationHelper.AddError("Blanket Waiver of Subrogation cannot be selected unless the Enhancement is also selected", Nothing)
                        chkBlanketWaiverOfSubro.Checked = False
                    End If
                End If

                If chkHiredBorrowedNonOwned.Checked Then
                    If chkNonOwnershipLiability.Checked Then
                        If txtNonOwnedNumberOfEmployees.Text.Trim = "" Then
                            Me.ValidationHelper.AddError(txtNonOwnedNumberOfEmployees, "Missing Number of Employees", accordList)
                        End If
                    End If
                End If

                If chkGarageKeepers.Checked Then
                    If txtGKComprehensiveLimit.Text.Trim <> "" Then
                        If ddlGKComprehensiveDeductible.SelectedIndex <= 0 Then
                            Me.ValidationHelper.AddError(ddlGKComprehensiveDeductible, "Missing Deductible", accordList)
                        End If
                    End If
                    If txtGKCollisionLimit.Text.Trim <> "" Then
                        If ddlGKCollisionDeductible.SelectedIndex < 0 Then
                            Me.ValidationHelper.AddError(ddlGKCollisionDeductible, "Missing Deductible", accordList)
                        End If
                    End If
                End If

                If Quote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio AndAlso Quote.Policyholder.Name.EntityTypeId = "1" Then
                    If ddlOhioQuestion.SelectedValue = "NA" Then
                        Me.ValidationHelper.AddError("Must answer question about business practices concerning the sale of vehicles, repair shops, or public parking.", Nothing)
                    End If
                End If

                '8/8/2018 note: would use something similar for state-specific controls; could potentially check visibility of state-specific controls since they should be hidden on Populate
                'If Me.Quote IsNot Nothing Then
                '    If QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteHasState(Me.Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) = True Then
                '        'validate the Indiana control here
                '    End If
                '    If QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteHasState(Me.Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois) = True Then
                '        'validate the Illinois control here
                '    End If
                'End If

                'Added 11/23/2020 for CAP Endorsements Task 52972 MLW
                If IsQuoteEndorsement() Then
                    ValidateChildControls(valArgs)
                End If

                Exit Sub
            Catch ex As Exception
                HandleError("ValidateControl", ex)
                Exit Sub
            End Try
        End If
    End Sub

    Public Sub PopulateCoverageLayout()
        If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
            LiabilityCol.Attributes.Add("style", "width:30%;")
            UMCol.Attributes.Add("style", "width:25%;")
            UIMCol.Attributes.Add("style", "width:20%;")
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) AndAlso Quote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                UMPDCol.Attributes.Add("style", "display:none;")
            Else
                UMPDCol.Attributes.Add("style", "display:inline;width:20%;")
            End If
            ddLiability.Attributes.Add("style", "width:60%;")
            ddUM.Attributes.Add("style", "width:70%;")
            txtUIM.Attributes.Add("style", "width:60%;")
            ddlUMPDLimit.Attributes.Add("style", "width:60%;")
        Else
            LiabilityCol.Attributes.Add("style", "width:33%;")
            UMCol.Attributes.Add("style", "width:33%;")
            UIMCol.Attributes.Add("style", "width:33%;")
            UMPDCol.Attributes.Add("style", "display:none;")
            ddLiability.Attributes.Add("style", "width:70%;")
            ddUM.Attributes.Add("style", "width:70%;")
            txtUIM.Attributes.Remove("style")
        End If
    End Sub

    Public Sub LoadLiabilityStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddLiability, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, , Quote.LobType)
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) AndAlso Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
            Dim removeBlank As ListItem = ddLiability.Items.FindByText("")
            If removeBlank IsNot Nothing Then
                ddLiability.Items.Remove(removeBlank)
            End If
        End If
    End Sub

    Public Sub LoadUMStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(ddUM, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, , Quote.LobType)
        ddUM.Items.Remove("")
        If (Me.Quote.StateId = 16) Then 'Removing 750,000 in UIM for Indiana only as per Bug59603
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) = False Then
                Dim itemToRemove = ddUM.Items.FindByText("750,000")
                If itemToRemove IsNot Nothing Then
                    ddUM.Items.Remove(itemToRemove)
                End If
            End If
        End If
    End Sub

    Public Sub LoadMedPayStaticData()
        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
        Dim a1 As New QuickQuoteStaticDataAttribute
        a1.nvp_name = "Version"
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            a1.nvp_value = "After20250801"
        Else
            a1.nvp_value = "Before20250801"
        End If
        optionAttributes.Add(a1)
        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddlMedicalPayments, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, optionAttributes, QuickQuoteStaticDataOption.SortBy.None, Quote.LobType)
    End Sub

    Public Sub LoadUMPDStaticData()
        If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
            QQHelper.LoadStaticDataOptionsDropDownForState(ddlUMPDLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            Dim removeNA As ListItem = ddlUMPDLimit.Items.FindByText("N/A")
            If removeNA IsNot Nothing Then
                ddlUMPDLimit.Items.Remove(removeNA)
            End If
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                ddlUMPDLimit.Items.Insert(0, New ListItem("", "0"))
            Else
                ddlUMPDLimit.Items.Insert(0, "")
            End If
        End If
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) AndAlso Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana Then
            QQHelper.LoadStaticDataOptionsDropDownForState(ddUMPDDedOptions, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Sub LoadPolicyCompCollStaticData()
        'Hired Car Physical Damage Collision Deductible
        QQHelper.LoadStaticDataOptionsDropDown(ddlHiredCarCollision, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageCollisionDeductibleId, , Quote.LobType)
        ' Garage Keepers Comprehensive Deductible
        QQHelper.LoadStaticDataOptionsDropDown(ddlGKComprehensiveDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionDeductibleId, , Quote.LobType)
        ' Garage Keepers Collision Deductible
        QQHelper.LoadStaticDataOptionsDropDown(ddlGKCollisionDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersCollisionDeductibleId, , Quote.LobType)
        '8/8/2018 note: this is an example of a static data call by state; would need this if there are state-specific controls (i.e. different dropdown for each state); would need to also pull state-specific quote when setting
        'QQHelper.LoadStaticDataOptionsDropDownForState(ddlGKCollisionDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersCollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana, lob:=Quote.LobType)

        'Remove N/A for Hired Car Physical Damage Collision Deductible
        Dim removeHCCNA As ListItem = ddlHiredCarCollision.Items.FindByText("N/A")
        If removeHCCNA IsNot Nothing Then
            ddlHiredCarCollision.Items.Remove(removeHCCNA)
        End If

        If CompCollDeductibleHelper.IsCompCollDeductibleAvailable(Quote) Then
            'Remove Hired Car Physical Damage Collision Deductible lower than 500
            Dim removeHCC100 As ListItem = ddlHiredCarCollision.Items.FindByText("100")
            If removeHCC100 IsNot Nothing Then
                ddlHiredCarCollision.Items.Remove(removeHCC100)
            End If
            Dim removeHCC250 As ListItem = ddlHiredCarCollision.Items.FindByText("250")
            If removeHCC250 IsNot Nothing Then
                ddlHiredCarCollision.Items.Remove(removeHCC250)
            End If
            'Remove Garage Keepers Comp Deductible lower than 500
            Dim removeGK100500 As ListItem = ddlGKComprehensiveDeductible.Items.FindByText("100/500")
            If removeGK100500 IsNot Nothing Then
                ddlGKComprehensiveDeductible.Items.Remove(removeGK100500)
            End If
            Dim removeGK2501000 As ListItem = ddlGKComprehensiveDeductible.Items.FindByText("250/1000")
            If removeGK2501000 IsNot Nothing Then
                ddlGKComprehensiveDeductible.Items.Remove(removeGK2501000)
            End If
            'Remove Garage Keepers Coll Deductible lower than 500
            Dim removeGK100 As ListItem = ddlGKCollisionDeductible.Items.FindByText("100")
            If removeGK100 IsNot Nothing Then
                ddlGKCollisionDeductible.Items.Remove(removeGK100)
            End If
            Dim removeGK250 As ListItem = ddlGKCollisionDeductible.Items.FindByText("250")
            If removeGK250 IsNot Nothing Then
                ddlGKCollisionDeductible.Items.Remove(removeGK250)
            End If
        End If
    End Sub

    Public Sub PopulateLiabilityDropDown()
        If SubQuoteFirst IsNot Nothing Then
            SetFromValue(ddLiability, SubQuoteFirst.Liability_UM_UIM_LimitId, 56) '56 = 1,000,000
        End If
    End Sub

    Public Sub PopulateUMDropDown()
        If SubQuoteFirst IsNot Nothing Then
            SetFromValue(ddUM, SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId, 56) '56 = 1,000,000
        End If
    End Sub

    Public Sub PopulateMedPayDropDown()
        If SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.MedicalPaymentsLimitid <> "" Then
            Dim medPayDefaultId = "15" '5,000 prior to 8/1/2025
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                medPayDefaultId = "173" '5,000 on or after 8/1/2025
            End If
            SetFromValue(ddlMedicalPayments, SubQuoteFirst.MedicalPaymentsLimitid, medPayDefaultId)
        End If
    End Sub


    Public Sub PopulateUMPDLimit()
        If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
            Dim IllinoisQuote As QuickQuoteObject = ILSubQuote
            If IllinoisQuote IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId) Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddlUMPDLimit, IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId, QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois))
                hdnUMPDLimitValue.Value = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, ddlUMPDLimit.SelectedValue, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddlUMPDLimit, "")
                hdnUMPDLimitValue.Value = ""
            End If
            If IsQuoteEndorsement() Then
                ddlUMPDLimit.Enabled = False
            End If
        End If
    End Sub

    Public Sub PopulateUMPDCoverage()
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            trLowerLimitsInfoRow.Visible = True
            Select Case Quote.QuickQuoteState
                Case QuickQuoteHelperClass.QuickQuoteState.Indiana
                    trUMPDRow.Visible = True
                    If SubQuoteFirst IsNot Nothing Then
                        If QQHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId) Then
                            chkUMPDCov.Checked = True
                            divUMPDDedOptions.Attributes.Add("style", "display:inline")
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddUMPDDedOptions, SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId)
                        Else
                            chkUMPDCov.Checked = False
                            divUMPDDedOptions.Attributes.Add("style", "display:none")
                        End If
                    End If
                Case QuickQuoteState.Illinois
                    'To compensate for the additional UMPD column on IL quotes
                    tdLowerLimitsInfo.ColSpan = 4
                    If SubQuoteFirst IsNot Nothing Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlUMPDLimit, SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId)
                    End If
                    hdnUMPDLimitValue.Value = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, ddlUMPDLimit.SelectedValue, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
            End Select
        Else
            trUMPDRow.Visible = False 'Needed for date crossing
            trLowerLimitsInfoRow.Visible = False 'Needed for date crossing
            PopulateUMPDLimit() 'moved to its own sub so date crossing logic can also use this
        End If
    End Sub

    Public Sub PopulateGarageKeepers()
        ' Garage Keepers
        ' If either garage keepers coverage is present show the garage keepers section
        If SubQuoteFirst.HasGarageKeepersCollision OrElse SubQuoteFirst.HasGarageKeepersOtherThanCollision Then
            'Added 02/16/2021 for CAP Endorsements Task 52972 MLW
            If IsQuoteReadOnly() Then
                ddlBasisType.Items.Add(New ListItem("", ""))
                ddlBasisType.Items.Add(New ListItem("N/A", "0"))
            End If
            ddlBasisType.SelectedValue = SubQuoteFirst.GarageKeepersBasisTypeId
            chkGarageKeepers.Checked = True
            trGarageKeepersDataRow.Attributes.Add("style", "display:''")
        Else
            ddlBasisType.SelectedIndex = 1  ' Direct Primary is the default
            chkGarageKeepers.Checked = False
            trGarageKeepersDataRow.Attributes.Add("style", "display:none")
        End If

        ' Garage Keepers - Comprehensive (other than collision)
        If SubQuoteFirst.HasGarageKeepersOtherThanCollision Then
            txtGKComprehensiveLimit.Text = SubQuoteFirst.GarageKeepersOtherThanCollisionManualLimitAmount
            'Added 02/16/2021 for CAP Endorsements Task 52972 MLW
            If IsQuoteReadOnly() Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlGKComprehensiveDeductible, SubQuoteFirst.GarageKeepersOtherThanCollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionDeductibleId)
            Else
                ddlGKComprehensiveDeductible.SelectedValue = SubQuoteFirst.GarageKeepersOtherThanCollisionDeductibleId
            End If
            'ddlGKComprehensiveDeductible.SelectedValue = SubQuoteFirst.GarageKeepersOtherThanCollisionDeductibleId
        Else
            txtGKComprehensiveLimit.Text = ""
            ddlGKComprehensiveDeductible.SelectedIndex = 0
        End If

        ' Garage Keepers - Collision
        If IsNullEmptyorWhitespace(SubQuoteFirst.GarageKeepersCollisionDeductibleId) Then
            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlGKCollisionDeductible, "8") '500
        End If
        If SubQuoteFirst.HasGarageKeepersCollision Then
            txtGKCollisionLimit.Text = SubQuoteFirst.GarageKeepersCollisionManualLimitAmount
            'Added 02/16/2021 for CAP Endorsements Task 52972 MLW
            If IsQuoteReadOnly() Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlGKCollisionDeductible, SubQuoteFirst.GarageKeepersCollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersCollisionDeductibleId)
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddlGKCollisionDeductible, SubQuoteFirst.GarageKeepersCollisionDeductibleId)
            End If
            'ddlGKCollisionDeductible.SelectedValue = SubQuoteFirst.GarageKeepersCollisionDeductibleId
        End If
        If Quote.QuickQuoteState = QuickQuoteState.Illinois Then
            'To compensate for the additional UMPD column on IL quotes
            Me.tdMinCombinedSingleLiabilityLimitInfo.ColSpan = 4
        End If
    End Sub

    Public Sub PopulateHiredBorrowedNonOwned()
        ' Hired, Borrowed, Non-Owned
        If SubQuoteFirst.HasHiredBorrowedNonOwned OrElse (SubQuoteFirst.LiabilityAutoSymbolObject IsNot Nothing AndAlso SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1) Then
            ' Only show the info row for multistate
            chkHiredBorrowedNonOwned.Checked = True
            trHiredBorrowedNonOwnedDataRow.Attributes.Add("style", "display:''")
            If SubQuoteFirst.HasNonOwnershipLiability OrElse (SubQuoteFirst.LiabilityAutoSymbolObject IsNot Nothing AndAlso SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1) Then
                chkNonOwnershipLiability.Checked = True
                trNonOwnershipLiabilityDataRow.Attributes.Add("style", "display:''")
                txtNonOwnedNumberOfEmployees.Text = SubQuoteFirst.NonOwnershipLiabilityNumberOfEmployees
                If SubQuoteFirst.NonOwnership_ENO_RatingTypeId <> "" Then
                    ddlNonOwnedExtendedNonOwnedLiability.SelectedValue = 1
                Else
                    ddlNonOwnedExtendedNonOwnedLiability.SelectedValue = 0
                End If
            Else
                chkNonOwnershipLiability.Checked = False
                trNonOwnershipLiabilityDataRow.Attributes.Add("style", "display:none")
                txtNonOwnedNumberOfEmployees.Text = ""
                ddlNonOwnedExtendedNonOwnedLiability.SelectedValue = 0
            End If
            If SubQuoteFirst.HasHiredBorrowedLiability OrElse (SubQuoteFirst.LiabilityAutoSymbolObject IsNot Nothing AndAlso SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1) Then
                chkHiredBorrowedLiability.Checked = True
                trHiredBorrowedLiabilityDataRow.Attributes.Add("style", "display:''")
                If SubQuoteFirst.HasHiredCarPhysicalDamage OrElse (SubQuoteFirst.LiabilityAutoSymbolObject IsNot Nothing AndAlso SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1) Then
                    chkHiredCarPhysicalDamage.Checked = True
                    trHiredCarPhysicalDamageDataRow.Attributes.Add("style", "display:''")
                    ddlHiredCarComprehensive.SelectedValue = SubQuoteFirst.ComprehensiveDeductibleId
                    If IsQuoteReadOnly() Then
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlHiredCarCollision, SubQuoteFirst.CollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageCollisionDeductibleId)
                    Else
                        ddlHiredCarCollision.SelectedValue = SubQuoteFirst.CollisionDeductibleId
                    End If
                Else
                    chkHiredCarPhysicalDamage.Checked = False
                    trHiredCarPhysicalDamageDataRow.Attributes.Add("style", "display:none")
                    'Updated 02/16/2021 for CAP Endorsements Task 52972 MLW
                    If IsQuoteReadOnly() Then
                        ddlHiredCarComprehensive.SelectedValue = SubQuoteFirst.ComprehensiveDeductibleId
                        IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlHiredCarCollision, SubQuoteFirst.CollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageCollisionDeductibleId)
                    Else
                        ' Default is 100
                        ddlHiredCarComprehensive.SelectedValue = "2"
                        ' Default is 500
                        ddlHiredCarCollision.SelectedValue = "8"
                    End If
                End If
            Else
                chkHiredBorrowedLiability.Checked = False
                trHiredBorrowedLiabilityDataRow.Attributes.Add("style", "display:none")
                'Updated 02/16/2021 for CAP Endorsements Task 52972 MLW
                If IsQuoteReadOnly() Then
                    ddlHiredCarComprehensive.SelectedValue = SubQuoteFirst.ComprehensiveDeductibleId
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlHiredCarCollision, SubQuoteFirst.CollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageCollisionDeductibleId)
                Else
                    ' Default is 100
                    ddlHiredCarComprehensive.SelectedValue = "2"
                    ' Default is 500
                    ddlHiredCarCollision.SelectedValue = "8"
                End If
            End If
        Else
            chkHiredBorrowedNonOwned.Checked = False
            chkNonOwnershipLiability.Checked = False
            chkHiredBorrowedLiability.Checked = False
            chkHiredCarPhysicalDamage.Checked = False
            trNonOwnershipLiabilityDataRow.Attributes.Add("style", "display:none")
            trHiredBorrowedNonOwnedDataRow.Attributes.Add("style", "display:none")
            trHiredBorrowedLiabilityDataRow.Attributes.Add("style", "display:none")
            trHiredCarPhysicalDamageDataRow.Attributes.Add("style", "display:none")
            txtNonOwnedNumberOfEmployees.Text = ""
            ddlNonOwnedExtendedNonOwnedLiability.SelectedValue = 0

            ' Default is 100
            ddlHiredCarComprehensive.SelectedValue = "2"
            ' Default is 500
            ddlHiredCarCollision.SelectedValue = "8"
        End If
    End Sub
#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsPostBack = False Then
                Me.MainAccordionDivId = Me.divMain.ClientID

            End If
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub btnVehicles_Click(sender As Object, e As EventArgs) Handles btnVehicles.Click
        Try
            ' Fire the save event before adding a vehicle or else the validation will fail
            Save_FireSaveEvent(True) ' fires Base save requested event

            If Me.ValidationSummmary.HasErrors = False Then
                'Updated 11/18/2020 for CAP Endorsements task 52972 MLW
                If IsQuoteEndorsement() Then
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, "0")
                Else
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "0")
                End If
                'Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.vehicles, "0")
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("btnVehicles_Click", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click, btnSavePolicyLevelCoverages.Click
        Me.Save_FireSaveEvent()
    End Sub

    'Added 11/16/2020 for CAP Endorsements task 52979 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    'Added 11/16/2020 for CAP Endorsements task 52979 MLW
    Protected Sub btnViewGotoDrivers_Click(sender As Object, e As EventArgs) Handles btnViewGotoDrivers.Click
        Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.drivers, "0")
    End Sub

#End Region

End Class