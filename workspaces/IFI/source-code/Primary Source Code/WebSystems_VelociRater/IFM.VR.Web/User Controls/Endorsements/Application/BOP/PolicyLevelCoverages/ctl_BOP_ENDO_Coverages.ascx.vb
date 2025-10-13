Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_BOP_ENDO_Coverages
    Inherits VRControlBase

#Region "Declarations"

    Private ReadOnly Property HasStopGap As Boolean
        Get
            Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
            If gsQuote IsNot Nothing Then
                If gsQuote.StopGapLimitId IsNot Nothing _
                    AndAlso gsQuote.StopGapLimitId.Trim <> "" _
                    AndAlso IsNumeric(gsQuote.StopGapLimitId) Then
                    Return True
                End If
            End If
            Return False
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
        Dim js As String = "if(this.checked == false) { if (confirm(""Are you sure you want to delete this coverage?"")) {__doPostBack('');return true} else {return false;}}"
        Try
            Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hddAccord, "0")
            Me.VRScript.CreateJSBinding(ddlCrimeTotalLimit.ClientID, "onchange", "var tl = document.getElementById('" & ddlCrimeTotalLimit.ClientID & "');var orow = document.getElementById('" & trForgeryAndAlterationsLimitRow.ClientID & "');if (tl.value != ''){orow.style.display = '';} else {orow.style.display = 'none';}")

            Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

            ' AI Checkboxes Script
            chkAI_TownhouseAssociates.Attributes.Add("onchange", "Bop.AICheckboxChanged('" & chkAI_TownhouseAssociates.ClientID & "','','','','');")
            chkAI_EngineersArchitectsSurveyors.Attributes.Add("onchange", "Bop.AICheckboxChanged('" & chkAI_EngineersArchitectsSurveyors.ClientID & "','','','','');")
            chkOwnersLesseesContractorsAutomatic.Attributes.Add("onchange", "Bop.AICheckboxChanged('" & chkOwnersLesseesContractorsAutomatic.ClientID & "','" & trOwnersLesseesContractorsAutomaticInfoRow.ClientID & "','" & chkOwnersLesseesContractorsWithAddlInsuredReq.ClientID & "','','','" & chkWaiverOfSubrogation.ClientID & "','" & trWaiverOfSubroInfoRow.ClientID & "');")
            chkWaiverOfSubrogation.Attributes.Add("onchange", "Bop.AICheckboxChanged('" & chkWaiverOfSubrogation.ClientID & "','" & trWaiverOfSubroInfoRow.ClientID & "','" & chkOwnersLesseesContractorsWithAddlInsuredReq.ClientID & "','','','" & chkOwnersLesseesContractorsAutomatic.ClientID & "','" & trOwnersLesseesContractorsAutomaticInfoRow.ClientID & "');")
            chkOwnersLesseesContractorsWithAddlInsuredReq.Attributes.Add("onchange", "Bop.AICheckboxChanged('" & chkOwnersLesseesContractorsWithAddlInsuredReq.ClientID & "','','','','');")
            chkOwnersLesseesContractorsCompletedOps.Attributes.Add("onchange", "Bop.AICheckboxChanged('" & chkOwnersLesseesContractorsCompletedOps.ClientID & "','" & trOwnersLesseesContractorsCompletedOpsInfoRow.ClientID & "','','','" & trNumberOfAITextboxRow.ClientID & "');")

            ' Coverage checkboxes script
            chkAdditionalInsured.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkAdditionalInsured.ClientID & "','" & trAdditionalInsuredsRow.ClientID & "','','');")
            chkCLI.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkCLI.ClientID & "','" & trCLIRow.ClientID & "','','');")
            chkEmployeeBenefitsLiability.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkEmployeeBenefitsLiability.ClientID & "','" & trEmployeeBenefitsRow.ClientID & "','','');")
            chkEPLI.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkEPLI.ClientID & "','" & trEPLIRow.ClientID & "','','');")
            chkContractorsEquipmentInstallation.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkContractorsEquipmentInstallation.ClientID & "','" & trContractorsRow.ClientID & "','','');")
            chkCrime.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkCrime.ClientID & "','" & trCrimeRow.ClientID & "','','');")
            chkNonOwned.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkNonOwned.ClientID & "','" & trNonOwnedInfoRow.ClientID & "','','');")
            chkElectronicData.Attributes.Add("onchange", "Bop.CoverageCheckboxChanged('" & chkElectronicData.ClientID & "','" & trElectronicDataRow.ClientID & "','','');")
            chkILContractorsHomeRepairAndRemodeling.Attributes.Add("onchange", "Bop.ILContractorsHomeRepairAndRemodelingCheckboxChanged('" & chkILContractorsHomeRepairAndRemodeling.ClientID & "','" & trILContractorsHomeRepairAndRemodelingDataRow.ClientID & "');")

            ' Contractors Tools & Equipment Blanket Limit - Show Blanket Sub-Limit when checked
            'txtContractorsToolsAndEquipmentBlanketLimit.Attributes.Add("onchange", "var txt1 = document.getElementById('" & txtContractorsToolsAndEquipmentBlanketLimit.ClientID & "');var trRow = document.getElementById('" & trContractorsToolsAndEquipmentSubLimitRow.ClientID & "');if (txt1.value != ''){alert('yes'); trRow.style.display = '';} else {alert('no');trRow.style.display = 'none';}")
            txtContractorsToolsAndEquipmentBlanketLimit.Attributes.Add("onkeyup", "Bop.ContractorsBlanketLimitChanged('" + txtContractorsToolsAndEquipmentBlanketLimit.ClientID & "','" & trContractorsToolsAndEquipmentSubLimitRow.ClientID & "');")

            ' Handle stop gap checkbox clicks
            Me.VRScript.CreateJSBinding(chkStopGap, ctlPageStartupScript.JsEventType.onclick, "Bop.HandleStopGapClicks('" & chkStopGap.ClientID & "','" & trStopGapDataRow.ClientID & "');")

            Exit Sub
        Catch ex As Exception
            HandleError("AddScriptWhenRendered", ex)
            Exit Sub
        End Try
    End Sub

    Public Overrides Sub LoadStaticData()
        ' Stop Gap Limit
        If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
            If ddStopGapLimit.Items Is Nothing OrElse ddStopGapLimit.Items.Count <= 0 Then
                QQHelper.LoadStaticDataOptionsDropDown(ddStopGapLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.StopGapLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            End If
        End If

        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        Dim CompletedOpsCount As Integer = 0
        Try
            If Me.Quote IsNot Nothing Then
                LoadStaticData()
                If Me.SubQuoteFirst IsNot Nothing Then
                    Me.chkAdditionalInsured.Checked = Me.SubQuoteFirst.AdditionalInsuredsCount > 0 Or Me.SubQuoteFirst.HasAdditionalInsuredsCheckboxBOP
                    Me.ddlNumberOfAddlInsureds.SetFromValue(If(Me.SubQuoteFirst.AdditionalInsuredsCount > 4, 4, Me.SubQuoteFirst.AdditionalInsuredsCount))
                    chkAdditionalInsured_CheckedChanged(Me, New EventArgs())

                    chkAI_TownhouseAssociates.Checked = False
                    chkAI_EngineersArchitectsSurveyors.Checked = False
                    chkOwnersLesseesContractorsAutomatic.Checked = False
                    chkWaiverOfSubrogation.Checked = False
                    chkOwnersLesseesContractorsWithAddlInsuredReq.Checked = False
                    chkOwnersLesseesContractorsCompletedOps.Checked = False

                    ' Read the AI info from the top quote!  MGB 1/23/19
                    If Quote.HasAdditionalInsuredsCheckboxBOP Then
                        If Quote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso Quote.AdditionalInsuredsCheckboxBOP.Count > 0 Then
                            For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured In Quote.AdditionalInsuredsCheckboxBOP
                                Select Case AI.AdditionalInsuredType
                                    Case QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations
                                        chkAI_TownhouseAssociates.Checked = True
                                        Exit Select
                                    Case QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                                        chkAI_EngineersArchitectsSurveyors.Checked = True
                                        Exit Select
                                    Case QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver
                                        chkOwnersLesseesContractorsAutomatic.Checked = True
                                        trOwnersLesseesContractorsAutomaticInfoRow.Attributes.Add("style", "display:''")
                                        chkWaiverOfSubrogation.Enabled = True
                                        trWaiverOfSubroInfoRow.Attributes.Add("style", "display:none")
                                        Exit Select
                                    Case QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.WaiverOfSubrogationWhenRequiredByWrittenContract
                                        chkWaiverOfSubrogation.Checked = True
                                        trWaiverOfSubroInfoRow.Attributes.Add("style", "display:''")
                                        chkOwnersLesseesContractorsAutomatic.Enabled = True
                                        trOwnersLesseesContractorsAutomaticInfoRow.Attributes.Add("style", "display:none")
                                        Exit Select
                                    Case QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract
                                        chkOwnersLesseesContractorsWithAddlInsuredReq.Checked = True
                                        If chkWaiverOfSubrogation.Checked OrElse chkOwnersLesseesContractorsAutomatic.Checked Then
                                            chkOwnersLesseesContractorsWithAddlInsuredReq.Enabled = False
                                        Else
                                            chkOwnersLesseesContractorsWithAddlInsuredReq.Enabled = True
                                        End If
                                        Exit Select
                                    Case QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations
                                        chkOwnersLesseesContractorsCompletedOps.Checked = True
                                        trNumberOfAITextboxRow.Attributes.Add("style", "display:''")
                                        trOwnersLesseesContractorsCompletedOpsInfoRow.Attributes.Add("style", "display:''")
                                        CompletedOpsCount += 1
                                        Exit Select
                                End Select
                            Next
                            If CompletedOpsCount > 0 AndAlso chkOwnersLesseesContractorsCompletedOps.Checked Then
                                txtNumOfAI.Text = CompletedOpsCount
                            End If
                        End If
                    End If

                    ' CLI
                    If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.CyberLiabilityUpgrade) Then
                        If IFM.VR.Common.Helpers.BOP.CyberCoverageHelper.IsCyberCoverageAvailable(Quote) Then
                            chkCLI.Text = "Cyber Coverage"
                        Else
                            chkCLI.Text = "Cyber Liability"
                        End If
                        Me.chkCLI.Enabled = Not IFM.VR.Common.Helpers.CGL.CLIHelper.CLINotAvailableDoToClassCode(Me.Quote)
                        If chkCLI.Enabled = False Then
                            chkCLIWrapper.Attributes.Add("style", "display:none")
                            trCLIRow.Attributes.Add("style", "display:none")
                        Else
                            chkCLIWrapper.Attributes.Add("style", "display:''")
                            Me.chkCLI.Checked = IFM.VR.Common.Helpers.CGL.CLIHelper.CLI_Is_Applied(Me.Quote)
                            If chkCLI.Checked Then
                                trCLIRow.Attributes.Add("style", "display:''")
                            End If
                            Me.chkCLI_CheckedChanged(Me, New EventArgs()) 'may not need this
                        End If
                    Else
                        chkCLIWrapper.Attributes.Add("style", "display:none")
                        trCLIRow.Attributes.Add("style", "display:none")
                    End If



                    Me.chkEmployeeBenefitsLiability.Checked = Not Me.SubQuoteFirst.EmployeeBenefitsLiabilityText.IsNullEmptyorWhitespace
                    Me.txtEBLNumberOfEmployees.Text = Me.SubQuoteFirst.EmployeeBenefitsLiabilityText.ReturnEmptyIfLessThanOrEqualToZero
                    chkEmployeeBenefitsLiability_CheckedChanged(Me, New EventArgs())

                    Me.chkEPLI.Enabled = Not IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLINotAvailableDoToClassCode(Me.Quote)
                    Me.chkEPLI.Checked = IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLI_Is_Applied(Me.Quote)
                    Me.lblRetroactiveDate.Text = Me.SubQuoteFirst.EffectiveDate 'always set it in case they select it
                    Me.chkEPLI_CheckedChanged(Me, New EventArgs())

                    ' Stop Gap (OH)
                    If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                        Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                        trStopGapCheckboxRow.Attributes.Add("style", "display:'';")
                        If HasStopGap() Then
                            chkStopGap.Checked = True
                            trStopGapDataRow.Attributes.Add("style", "display:'';width:100%;text-indent:15px;")
                            If gsQuote.StopGapLimitId IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapLimitId) Then
                                ddStopGapLimit.SetFromValue(gsQuote.StopGapLimitId)
                            Else
                                ddStopGapLimit.SetFromValue("0")
                            End If
                            txtStopGapPayroll.Text = ""
                            If gsQuote.StopGapPayroll IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapPayroll) Then
                                txtStopGapPayroll.Text = Format(CDec(gsQuote.StopGapPayroll), "#########")
                            End If
                        Else
                            ' OH effective, but doesn't have stop gap
                            chkStopGap.Checked = False
                            trStopGapDataRow.Attributes.Add("style", "display:none;")
                            txtStopGapPayroll.Text = ""
                            ddStopGapLimit.SelectedIndex = -1
                        End If
                    Else
                        ' OH not effective
                        trStopGapCheckboxRow.Attributes.Add("style", "display:none;")
                        trStopGapDataRow.Attributes.Add("style", "display:none;")
                        chkStopGap.Checked = False
                        txtStopGapPayroll.Text = ""
                        ddStopGapLimit.SelectedIndex = -1
                        IFM.VR.Common.Helpers.MultiState.General.RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
                    End If

                    If Me.SubQuoteFirst.ContractorsEquipmentInstallationLimit.IsNullEmptyorWhitespace = False Or
                    Me.SubQuoteFirst.ContractorsEmployeeTools.IsNullEmptyorWhitespace = False Or
                    Me.SubQuoteFirst.ContractorsToolsEquipmentRented.IsNullEmptyorWhitespace = False Or
                    Me.SubQuoteFirst.ContractorsToolsEquipmentScheduled.IsNullEmptyorWhitespace = False Or
                    Me.SubQuoteFirst.ContractorsToolsEquipmentBlanket.IsNullEmptyorWhitespace = False Then

                        Me.chkContractorsEquipmentInstallation.Checked = True
                        Me.ddlContractorsPropertyLimitAtEachCoveredJobsite.SetFromValue(Me.SubQuoteFirst.ContractorsEquipmentInstallationLimitId)
                        Me.txtContractorsEmployeesToolsLimit.Text = Me.SubQuoteFirst.ContractorsEmployeeTools
                        Me.txtContractorsToolsAndEquipmentBlanketLimit.Text = Me.SubQuoteFirst.ContractorsToolsEquipmentBlanket
                        If Me.SubQuoteFirst.ContractorsToolsEquipmentBlanket <> "" Then
                            trContractorsToolsAndEquipmentSubLimitRow.Attributes.Add("style", "display:''")
                            If SubQuoteFirst.ContractorsToolsEquipmentBlanketSubLimitId <> "" Then
                                ddlContractorsToolsAndEquipmentBlanketSubLimit.SelectedValue = SubQuoteFirst.ContractorsToolsEquipmentBlanketSubLimitId
                            End If
                        Else
                            trContractorsToolsAndEquipmentSubLimitRow.Attributes.Add("style", "display:none")
                        End If
                        Me.txtContractorsToolsAndEquipmentScheduledLimit.Text = Me.SubQuoteFirst.ContractorsToolsEquipmentScheduled
                        Me.txtContractorsRentedLeasedToolsAndEquipmentLimit.Text = Me.SubQuoteFirst.ContractorsToolsEquipmentRented

                    Else
                        Me.chkContractorsEquipmentInstallation.Checked = False
                        Me.ddlContractorsPropertyLimitAtEachCoveredJobsite.SelectedIndex = 0
                        Me.txtContractorsEmployeesToolsLimit.Text = ""
                        Me.txtContractorsToolsAndEquipmentBlanketLimit.Text = ""
                        Me.trContractorsToolsAndEquipmentSubLimitRow.Attributes.Add("style", "display:none")
                        Me.txtContractorsToolsAndEquipmentScheduledLimit.Text = ""
                        Me.txtContractorsRentedLeasedToolsAndEquipmentLimit.Text = ""
                    End If
                    chkContractorsEquipmentInstallation_CheckedChanged(Me, New EventArgs())

                    If Me.SubQuoteFirst.CrimeEmpDisEmployeeText.IsNullEmptyorWhitespace = False Then
                        Me.txtCrimeNumberOfEmployees.Text = Me.SubQuoteFirst.CrimeEmpDisEmployeeText
                        Me.txtCrimeNumberOfLocations.Text = Me.SubQuoteFirst.CrimeEmpDisLocationText
                        ddlCrimeTotalLimit.SetFromValue(Me.SubQuoteFirst.CrimeEmpDisLimitId)
                        If ddlCrimeTotalLimit.SelectedIndex > 0 Then
                            trForgeryAndAlterationsLimitRow.Attributes.Add("style", "display:''")
                            If SubQuoteFirst.CrimeForgeryLimit.IsNullEmptyorWhitespace Then
                                chkForgeryAlterationOptionalLimits.Checked = False
                            Else
                                chkForgeryAlterationOptionalLimits.Checked = True
                            End If
                        Else
                            trForgeryAndAlterationsLimitRow.Attributes.Add("style", "display:none")
                        End If
                        Me.chkCrime.Checked = True
                    Else
                        Me.txtCrimeNumberOfEmployees.Text = ""
                        Me.txtCrimeNumberOfLocations.Text = ""
                        Me.ddlCrimeTotalLimit.SelectedIndex = 0
                        chkForgeryAlterationOptionalLimits.Checked = False
                        Me.chkCrime.Checked = False
                        chkForgeryAlterationOptionalLimits.Checked = False
                        trForgeryAndAlterationsLimitRow.Attributes.Add("style", "display:none")
                    End If
                    chkCrime_CheckedChanged(Me, New EventArgs())

                    Me.chkEarthquake.Checked = Me.SubQuoteFirst.HasEarthquake
                    Me.chkHiredAuto.Checked = Me.SubQuoteFirst.HasHiredAuto
                    If Me.SubQuoteFirst.HasHiredAuto Then
                        chkNonOwned.Checked = True
                        trNonOwnedInfoRow.Attributes.Add("style", "display:''")
                    Else
                        chkNonOwned.Checked = False
                        trNonOwnedInfoRow.Attributes.Add("style", "display:none")
                    End If

                    ' Electronic Data
                    If Me.SubQuoteFirst.HasElectronicData Then
                        chkElectronicData.Checked = True
                        trElectronicDataRow.Attributes.Add("style", "display:''")
                        If SubQuoteFirst.ElectronicDataLimit.Trim <> String.Empty Then txtElectronicDataLimit.Text = SubQuoteFirst.ElectronicDataLimit
                    Else
                        chkElectronicData.Checked = False
                        trElectronicDataRow.Attributes.Add("style", "display:none")
                        txtElectronicDataLimit.Text = String.Empty
                    End If

                    ' IL Contractors - Home Repair & Remodeling
                    ' Only show the checkbox if Illinois is goverining state or there is a Illinois state on the quote
                    trILContractorsHomeRepairAndRemodelingCheckboxRow.Attributes.Add("style", "display:none;")
                    trILContractorsHomeRepairAndRemodelingDataRow.Attributes.Add("style", "display:none;")
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                        If GoverningStateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse SubQuotesContainsState("IL") Then
                            trILContractorsHomeRepairAndRemodelingCheckboxRow.Attributes.Add("style", "display:''")   ' Display checkbox row
                            Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IL")  ' Get the Illinois quote
                            If ILQuote IsNot Nothing Then
                                ' Populate the coverage
                                chkILContractorsHomeRepairAndRemodeling.Checked = ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling
                                If chkILContractorsHomeRepairAndRemodeling.Checked Then
                                    ' Show the data row if checked
                                    trILContractorsHomeRepairAndRemodelingDataRow.Attributes.Add("style", "display:''")
                                End If
                            End If
                        End If
                    End If
                End If
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        Dim AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured = Nothing
        Dim HasAI As Boolean = False


        Try
            If Me.Quote IsNot Nothing Then
                IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(Me.Quote, Me.chkEPLI.Checked)
                IFM.VR.Common.Helpers.CGL.CLIHelper.Toggle_CLI_Is_Applied(Me.Quote, Me.chkCLI.Checked) ' CLI
                Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()

                ' Save the AI info to the top quote!!  MGB 1/23/19
                Quote.AdditionalInsuredsCheckboxBOP = Nothing
                If chkAdditionalInsured.Checked Then
                    Quote.AdditionalInsuredsCount = CInt(Me.ddlNumberOfAddlInsureds.SelectedValue)

                    Quote.AdditionalInsuredsCheckboxBOP = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInsured)

                    If chkAI_TownhouseAssociates.Checked Then
                        Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations))
                        HasAI = True
                    End If
                    If chkAI_EngineersArchitectsSurveyors.Checked Then
                        Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors))
                        HasAI = True
                    End If
                    If chkOwnersLesseesContractorsAutomatic.Checked Then
                        Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver))
                        ' Note that this coverage is ALWAYS added when Lessees/Contractors Automatic is selected
                        Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract))
                        HasAI = True
                    End If
                    If chkWaiverOfSubrogation.Checked Then
                        Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.WaiverOfSubrogationWhenRequiredByWrittenContract))
                        ' Note that this coverage is ALWAYS added when Waiver of Subro is selected
                        Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract))
                        HasAI = True
                    End If
                    If chkOwnersLesseesContractorsCompletedOps.Checked Then
                        If IsNumeric(txtNumOfAI.Text) Then
                            For i As Integer = 1 To CInt(txtNumOfAI.Text)
                                Quote.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations, i))
                            Next
                        End If
                        HasAI = True
                    End If
                Else
                    Quote.AdditionalInsuredsCount = 0
                End If
                Quote.HasAdditionalInsuredsCheckboxBOP = HasAI

                'MODIFIED 08/10/2022 FOR 76291
                '** stop gap will only be added to the governing state quote to prevent overcharging for this coverage 
                ' Add stop gap at the quote level as well as the subquote level
                With gsQuote
                    IFM.VR.Common.Helpers.MultiState.General.RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
                    If chkStopGap.Checked Then
                        .StopGapLimitId = ddStopGapLimit.SelectedValue
                        .StopGapPayroll = txtStopGapPayroll.Text
                    End If
                End With

                For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In Me.SubQuotes
                    'sq.AdditionalInsuredsCheckboxBOP = Nothing
                    'If chkAdditionalInsured.Checked Then
                    '    sq.AdditionalInsuredsCount = CInt(Me.ddlNumberOfAddlInsureds.SelectedValue)

                    '    sq.AdditionalInsuredsCheckboxBOP = New List(Of QuickQuote.CommonObjects.QuickQuoteAdditionalInsured)

                    '    If chkAI_TownhouseAssociates.Checked Then
                    '        sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations))
                    '        HasAI = True
                    '    End If
                    '    If chkAI_EngineersArchitectsSurveyors.Checked Then
                    '        sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors))
                    '        HasAI = True
                    '    End If
                    '    If chkOwnersLesseesContractorsAutomatic.Checked Then
                    '        sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver))
                    '        ' Note that this coverage is ALWAYS added when Lessees/Contractors Automatic is selected
                    '        sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract))
                    '        HasAI = True
                    '    End If
                    '    If chkWaiverOfSubrogation.Checked Then
                    '        sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.WaiverOfSubrogationWhenRequiredByWrittenContract))
                    '        ' Note that this coverage is ALWAYS added when Waiver of Subro is selected
                    '        sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract))
                    '        HasAI = True
                    '    End If
                    '    If chkOwnersLesseesContractorsCompletedOps.Checked Then
                    '        If IsNumeric(txtNumOfAI.Text) Then
                    '            For i As Integer = 1 To CInt(txtNumOfAI.Text)
                    '                sq.AdditionalInsuredsCheckboxBOP.Add(GetAIObject(QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations, i))
                    '            Next
                    '        End If
                    '        HasAI = True
                    '    End If
                    'Else
                    '    sq.AdditionalInsuredsCount = 0
                    'End If
                    'sq.HasAdditionalInsuredsCheckboxBOP = HasAI

                    sq.EmployeeBenefitsLiabilityText = Me.txtEBLNumberOfEmployees.Text
                    sq.EmployeeBenefitsLiabilityOccurrenceLimit = If(Me.chkEmployeeBenefitsLiability.Checked, sq.EmployeeBenefitsLiabilityOccurrenceLimit, String.Empty)
                    sq.EmployeeBenefitsLiabilityOccurrenceLimitId = If(Me.chkEmployeeBenefitsLiability.Checked, sq.EmployeeBenefitsLiabilityOccurrenceLimitId, String.Empty)

                    ' Stop Gap (OH)
                    'MODIFIED 08/10/2022 FOR 76291
                    '** stop gap will only be added to the governing state quote to prevent overcharging for this coverage 
                    'If chkStopGap.Checked Then
                    '    sq.StopGapLimitId = ddStopGapLimit.SelectedValue
                    '    sq.StopGapPayroll = txtStopGapPayroll.Text
                    'Else
                    '    sq.StopGapLimitId = ""
                    '    sq.StopGapPayroll = ""
                    'End If

                    sq.ContractorsEquipmentInstallationLimitId = If(Me.chkContractorsEquipmentInstallation.Checked, ddlContractorsPropertyLimitAtEachCoveredJobsite.SelectedValue, String.Empty)

                    ' Set tools & equipment blanket and tools & equipment blanket sub limit
                    If chkContractorsEquipmentInstallation.Checked Then
                        If txtContractorsToolsAndEquipmentBlanketLimit.Text.Trim <> "" Then
                            sq.ContractorsToolsEquipmentBlanket = txtContractorsToolsAndEquipmentBlanketLimit.Text
                            ' Sub Limit is required when Blanket Limit is entered
                            sq.ContractorsToolsEquipmentBlanketSubLimitId = ddlContractorsToolsAndEquipmentBlanketSubLimit.SelectedValue
                        End If
                        sq.ContractorsToolsEquipmentScheduled = If(Me.chkContractorsEquipmentInstallation.Checked, txtContractorsToolsAndEquipmentScheduledLimit.Text.Trim(), String.Empty)
                        Session($"ContractorsToolsEquipmentScheduled_{Me.QuoteId.ToString()}") = sq.ContractorsToolsEquipmentScheduled 'Matt A 6-8-17

                        sq.ContractorsToolsEquipmentRented = If(Me.chkContractorsEquipmentInstallation.Checked, txtContractorsRentedLeasedToolsAndEquipmentLimit.Text.Trim(), String.Empty)
                        sq.ContractorsEmployeeTools = If(Me.chkContractorsEquipmentInstallation.Checked, txtContractorsEmployeesToolsLimit.Text.Trim(), String.Empty)
                    Else
                        sq.ContractorsToolsEquipmentBlanket = String.Empty
                        sq.ContractorsToolsEquipmentBlanketSubLimitId = String.Empty
                        sq.ContractorsToolsEquipmentScheduled = String.Empty
                        sq.ContractorsToolsEquipmentRented = String.Empty
                        sq.ContractorsEmployeeTools = String.Empty
                        Session.Remove($"ContractorsToolsEquipmentScheduled_{Me.QuoteId.ToString()}")
                    End If

                    sq.CrimeEmpDisEmployeeText = If(Me.chkCrime.Checked, txtCrimeNumberOfEmployees.Text.Trim(), String.Empty)
                    sq.CrimeEmpDisLocationText = If(Me.chkCrime.Checked, txtCrimeNumberOfLocations.Text.Trim(), String.Empty)
                    sq.CrimeEmpDisLimitId = If(Me.chkCrime.Checked, ddlCrimeTotalLimit.SelectedValue, String.Empty)
                    If chkForgeryAlterationOptionalLimits.Checked = True Then
                        sq.CrimeForgeryLimitId = ddlCrimeTotalLimit.SelectedValue
                    Else
                        sq.CrimeForgeryLimitId = String.Empty
                    End If

                    sq.HasEarthquake = Me.chkEarthquake.Checked
                    sq.HasHiredAuto = Me.chkHiredAuto.Checked
                    sq.HasNonOwnedAuto = Me.chkNonOwned.Checked

                    If chkElectronicData.Checked Then
                        sq.HasElectronicData = True
                        If txtElectronicDataLimit.Text.Trim <> String.Empty AndAlso IsNumeric(txtElectronicDataLimit.Text) Then
                            sq.ElectronicDataLimit = txtElectronicDataLimit.Text
                        End If
                    Else
                        sq.HasElectronicData = False
                        sq.ElectronicDataLimit = String.Empty
                    End If

                    ' IL Contractors Home Repair & Remodeling
                    If GoverningStateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse SubQuotesContainsState("IL") Then
                        Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                        If ILQuote IsNot Nothing Then
                            If chkILContractorsHomeRepairAndRemodeling.Checked Then
                                ' Checked
                                ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling = True
                                ILQuote.IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount = "10000"
                            Else
                                ' Not checked
                                ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling = False
                                ILQuote.IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount = ""
                            End If
                        End If
                    End If
                Next
            End If

            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Try
            MyBase.ValidateControl(valArgs)

            Me.ValidationHelper.GroupName = "Policy Level Coverages"

            ' Additional Insureds
            If chkOwnersLesseesContractorsCompletedOps.Checked Then
                If txtNumOfAI.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtNumOfAI, "Missing Number of AI", accordList)
                End If
            End If

            ' Stop Gap
            If chkStopGap.Checked Then
                If txtStopGapPayroll.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtStopGapPayroll, "Missing Payroll Amount", accordList)
                Else
                    If Not txtStopGapPayroll.Text.IsNumeric OrElse CInt(txtStopGapPayroll.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtStopGapPayroll, "Invalid Payroll Amount", accordList)
                    End If
                End If
            End If

            ' Employee Benefits Liability
            If chkEmployeeBenefitsLiability.Checked Then
                If txtEBLNumberOfEmployees.Text.IsNullEmptyorWhitespace Then
                    Me.ValidationHelper.AddError(txtEBLNumberOfEmployees, "Missing Number of Employees", accordList)
                Else
                    If Not txtEBLNumberOfEmployees.Text.IsNumeric OrElse CInt(txtEBLNumberOfEmployees.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtElectronicDataLimit, "Invalid Number of Employees", accordList)
                    End If
                End If
            End If

            ' Crime
            If chkCrime.Checked Then
                If txtCrimeNumberOfEmployees.Text.IsNullEmptyorWhitespace Then
                    Me.ValidationHelper.AddError(txtCrimeNumberOfEmployees, "Missing Number of Employees", accordList)
                Else
                    If Not txtCrimeNumberOfEmployees.Text.IsNumeric OrElse CInt(txtCrimeNumberOfEmployees.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtCrimeNumberOfEmployees, "Invalid Number of Employees", accordList)
                    End If
                End If
                If txtCrimeNumberOfLocations.Text.IsNullEmptyorWhitespace Then
                    Me.ValidationHelper.AddError(txtCrimeNumberOfLocations, "Missing Number of Locations", accordList)
                Else
                    If Not txtCrimeNumberOfLocations.Text.IsNumeric OrElse CInt(txtCrimeNumberOfLocations.Text) < 0 Then
                        Me.ValidationHelper.AddError(txtCrimeNumberOfLocations, "Invalid Number of Locations", accordList)
                    End If
                End If
                If ddlCrimeTotalLimit.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError("Missing Limit", ddlCrimeTotalLimit.ClientID)
                End If
            End If

            ' Electronic Data
            If chkElectronicData.Checked Then
                If txtElectronicDataLimit.Text.IsNullEmptyorWhitespace Then
                    Me.ValidationHelper.AddError(txtElectronicDataLimit, "Missing Limit", accordList)
                Else
                    If Not txtElectronicDataLimit.Text.IsNumeric OrElse CInt(txtElectronicDataLimit.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtElectronicDataLimit, "Invalid Limit", accordList)
                    End If
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateControl", ex)
            Exit Sub
        End Try

    End Sub

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

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If IsPostBack = False Then
                Me.MainAccordionDivId = Me.divMain.ClientID
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub btnLocations_Click(sender As Object, e As EventArgs) Handles btnLocations.Click
        Try
            Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' fires Base save requested event

            '
            ' you don't want the treeview to be able to invoke a prefill request at the wrong time so only do it if they clicked save button on insured list
            If Me.ValidationSummmary.HasErrors = False Then
                ' you only want to attempt prefill only if you have enough information
                ' it will control if it is actually invoked based on if it has been fetched already
                If Me.Quote IsNot Nothing Then
                    Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.location, "0")
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("btnLocations_Click", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub btnSavePolicyLevelCoverages_Click(sender As Object, e As EventArgs) Handles btnSavePolicyLevelCoverages.Click
        Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, True, New IFM.VR.Web.VRValidationArgs(DefaultValidationType))) ' fires Base save requested event
        Populate()
    End Sub

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub

    Private Sub chkAdditionalInsured_CheckedChanged(sender As Object, e As EventArgs) Handles chkAdditionalInsured.CheckedChanged
        If chkAdditionalInsured.Checked Then
            trAdditionalInsuredsRow.Attributes.Add("style", "display:''")
        Else
            trAdditionalInsuredsRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkCLI_CheckedChanged(sender As Object, e As EventArgs) Handles chkCLI.CheckedChanged
        If chkCLI.Checked Then
            trCLIRow.Attributes.Add("style", "display:''")
        Else
            trCLIRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkEmployeeBenefitsLiability_CheckedChanged(sender As Object, e As EventArgs) Handles chkEmployeeBenefitsLiability.CheckedChanged
        If chkEmployeeBenefitsLiability.Checked Then
            trEmployeeBenefitsRow.Attributes.Add("style", "display:''")
        Else
            trEmployeeBenefitsRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkEPLI_CheckedChanged(sender As Object, e As EventArgs) Handles chkEPLI.CheckedChanged
        If chkEPLI.Checked Then
            trEPLIRow.Attributes.Add("style", "display:''")
        Else
            trEPLIRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkContractorsEquipmentInstallation_CheckedChanged(sender As Object, e As EventArgs) Handles chkContractorsEquipmentInstallation.CheckedChanged
        If chkContractorsEquipmentInstallation.Checked Then
            trContractorsRow.Attributes.Add("style", "display:''")
        Else
            trContractorsRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkCrime_CheckedChanged(sender As Object, e As EventArgs) Handles chkCrime.CheckedChanged
        If chkCrime.Checked Then
            trCrimeRow.Attributes.Add("style", "display:''")
            'trForgeryAndAlterationsLimitRow.Visible = False
        Else
            trCrimeRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkElectronicData_CheckedChanged(sender As Object, e As EventArgs) Handles chkElectronicData.CheckedChanged
        If chkElectronicData.Checked Then
            trElectronicDataRow.Attributes.Add("style", "display:''")
        Else
            trElectronicDataRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkNonOwned_CheckedChanged(sender As Object, e As EventArgs) Handles chkNonOwned.CheckedChanged
        If chkNonOwned.Checked Then
            trNonOwnedInfoRow.Attributes.Add("style", "display:''")
        Else
            trNonOwnedInfoRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Private Sub chkOwnersLesseesContractorsCompletedOps_CheckedChanged(sender As Object, e As EventArgs) Handles chkOwnersLesseesContractorsCompletedOps.CheckedChanged
        If chkOwnersLesseesContractorsCompletedOps.Checked Then
            trNumberOfAITextboxRow.Attributes.Add("style", "display:''")
        Else
            trNumberOfAITextboxRow.Attributes.Add("style", "display:none")
        End If
    End Sub

#End Region

End Class