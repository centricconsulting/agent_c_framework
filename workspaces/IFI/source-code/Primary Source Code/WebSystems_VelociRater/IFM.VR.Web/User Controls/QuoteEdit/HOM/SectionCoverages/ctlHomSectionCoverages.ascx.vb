Imports IFM.PrimativeExtensions
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports QuickQuote.CommonMethods

Public Class ctlHomSectionCoverages
    Inherits VRControlBase

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.Quote.IsNotNull Then
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property CurrentFormTypeId As Int32
        Get
            'Updated 8/23/18 for multi state MLW
            'If Me.MyLocation.IsNotNull Then
            If Me.MyLocation IsNot Nothing Then
                If Int32.TryParse(Me.MyLocation.FormTypeId, Nothing) Then
                    Return CInt(Me.MyLocation.FormTypeId)
                End If
            End If
            Return -1
        End Get
    End Property

    'added 12/19/17 for HOM Upgrade - MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "false")
        Me.VRScript.CreateConfirmDialog(Me.lnkClear.ClientID, "Clear?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        Dim js As String = "if($('#{0}').val() == '0') {{ $('#{1}').text('-Less'); $('#{2}').show(); $('#{0}').val('1'); }} else {{ $('#{1}').text('+More'); $('#{2}').hide(); $('#{0}').val('0'); }}".FormatIFM(Me.hdnMoreLess.ClientID, Me.lnkMoreLess.ClientID, Me.divMoreCoverages.ClientID)

        Dim js2 As String = "if($('#{0}').val() == '1') {{ $('#{1}').text('-Less'); $('#{2}').show(); }} else {{ $('#{1}').text('+More'); $('#{2}').hide(); }}".FormatIFM(Me.hdnMoreLess.ClientID, Me.lnkMoreLess.ClientID, Me.divMoreCoverages.ClientID)

        Me.VRScript.AddScriptLine(js2)
        Me.VRScript.CreateJSBinding(Me.lnkMoreLess, ctlPageStartupScript.JsEventType.onclick, js + " return false;", False)

        'Added 1/10/18 for HOM Upgrade MLW
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso Me.Quote.Locations(0).StructureTypeId <> "2") Then
            Dim whatsNewPopupMessage As String = "<br /><div><b>New coverages Only available by contacting your Underwriter</b></div><br /><div><a href='vrHelpMe.aspx?p=VR3Home&s=hb' target='_blank'>Home Business (HO 0757)</a></div>"
            Using popupNew As New PopupMessageClass.PopupMessageObject(Me.Page, whatsNewPopupMessage, "New Coverages Available:")
                With popupNew
                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                    .isModal = False
                    .AddButton("OK", True)
                    .width = 400
                    .height = 175
                    .AddControlToBindTo(lnkWhatsNew)
                    .divId = "whatsNewPopup"
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End If

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso Me.Quote.Locations(0).StructureTypeId <> "2") Then
            divWhatsNewLink.Visible = True
        Else
            divWhatsNewLink.Visible = False
        End If

        Dim occ As Boolean = False
        If MyLocation.OccupancyCodeId IsNot Nothing AndAlso MyLocation.OccupancyCodeId = "4" OrElse MyLocation.OccupancyCodeId = "5" Then
            occ = True
        End If
        'Updated 6/9/2022 for task 74187 MLW
        'Me.Repeater1.DataSource = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedPrimaryCoverages(Quote.EffectiveDate, CurrentFormTypeId, occ, Quote.Locations(0).StructureTypeId, Quote.Locations(0).OccupancyCodeId, Quote.QuoteTransactionType, QQHelper.IntegerForString(Quote.VersionId))
        Me.Repeater1.DataSource = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedPrimaryCoverages(Quote.EffectiveDate, CurrentFormTypeId, occ, Quote.Locations(0).StructureTypeId, Quote.Locations(0).OccupancyCodeId, Quote.QuoteTransactionType, QQHelper.IntegerForString(Quote.VersionId), QQHelper.IntegerForString(Quote.RatingVersionId))
        Me.Repeater1.DataBind()

        Dim section1Coverages = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedSectionICoverages(Quote) 'Updated 1/4/18 for HOM Upgrade MLW, added Quote parameter
        If IFM.VR.Common.Helpers.HOM.SectionCoverage.SectionIHasVisibleCoveragesAvailable(Me.Quote) Then
            Me.divSectionI.Visible = True
            Me.Repeater2.DataSource = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedSectionICoverages(Quote) 'Updated 1/4/18 for HOM Upgrade MLW
            Me.Repeater2.DataBind()
        Else
            Me.divSectionI.Visible = False
            Me.Repeater2.DataSource = Nothing
            Me.Repeater2.DataBind()
        End If

        If IFM.VR.Common.Helpers.HOM.SectionCoverage.SectionIIHasVisibleCoveragesAvailable(Me.Quote) Then
            Me.divSectionII.Visible = True
            Me.Repeater3.DataSource = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedSectionIICoverages(Quote) 'Updated 1/4/18 for HOM Upgrade MLW
            Me.Repeater3.DataBind()
        Else
            Me.divSectionII.Visible = False
            Me.Repeater3.DataSource = Nothing
            Me.Repeater3.DataBind()
        End If

        If IFM.VR.Common.Helpers.HOM.SectionCoverage.SectionIandIIHasVisibleCoveragesAvailable(Me.Quote) Then
            Me.divSectionIandII.Visible = True
            Me.Repeater4.DataSource = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedSectionIAndIICoverages(Quote) 'Updated 1/8/18 for HOM Upgrade MLW
            Me.Repeater4.DataBind()
        Else
            Me.divSectionIandII.Visible = False
            Me.Repeater4.DataSource = Nothing
            Me.Repeater4.DataBind()
        End If

        'Added 12/19/17 for HOM Upgrade MLW
        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            'handled in javascript in ctlHomeCoverages
        Else
            Me.divSectionII.Visible = True
            Me.divNASectionII.Visible = False
            Me.divSectionIandII.Visible = True
            Me.divNASectionIandII.Visible = False
        End If


        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        If Me.IsQuoteReadOnly Then
            Dim policyNumber As String = Me.Quote.PolicyNumber
            Dim imageNum As Integer = 0
            Dim policyId As Integer = 0
            Dim toolTip As String = "Make a change to this policy"
            'Dim qqHelper As New QuickQuoteHelperClass
            Dim readOnlyViewPageUrl As String = QQHelper.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
            If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
            End If

            btnSave.Visible = False
            btnRate.Visible = False
            btnMakeAChange.Visible = True
            btnViewGotoNextSection.Visible = True

            btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
            readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
            btnMakeAChange.ToolTip = toolTip
            btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
        Else
            btnMakeAChange.Visible = False
            btnViewGotoNextSection.Visible = False
            btnSave.Visible = True
            btnRate.Visible = True
        End If

        Me.FindChildVrControls()

        For Each control In Me.GatherChildrenOfType(Of ctlSectionCoverageItem)
            control.Populate()
        Next


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

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        If Quote IsNot Nothing AndAlso Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then Exit Sub

        ' Let all the coverage items know the effective date changed
        For Each ri As RepeaterItem In Repeater1.Items
            Dim ci As ctlSectionCoverageItem = ri.FindControl("ctlSectionCoverageItem")
            ci.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next

        For Each ri As RepeaterItem In Repeater2.Items
            Dim ci As ctlSectionCoverageItem = ri.FindControl("ctlSectionCoverageItem")
            ci.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next

        For Each ri As RepeaterItem In Repeater3.Items
            Dim ci As ctlSectionCoverageItem = ri.FindControl("ctlSectionCoverageItem")
            ci.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next

        For Each ri As RepeaterItem In Repeater4.Items
            Dim ci As ctlSectionCoverageItem = ri.FindControl("ctlSectionCoverageItem")
            ci.EffectiveDateChanged(NewEffectiveDate, OldEffectiveDate)
        Next
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        ValidationHelper.GroupName = "Optional Coverages"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Dim valList = Optional_Coverages_Hom_Validator.ValidateHOMOptCoverages(Me.Quote, valArgs.ValidationType)

        If valList.Any() Then
            For Each v In valList
                ' *************************
                ' Optional Policy Coverages
                ' *************************
                Select Case v.FieldId
                    Case Optional_Coverages_Hom_Validator.InflationGuard
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.BackupSewDrain
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.WaterDamage
                        'Added 1/16/18 for HOM Upgrade MLW
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.IdentityFraudHOM
                        'Added 1/17/18 for HOM Upgrade MLW
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.HomePlusEnhance
                        'Added 1/17/18 for HOM Upgrade MLW
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.SpecialCoverageConflict
                        'Added 1/17/18 for HOM Upgrade MLW
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.GreenUpgrades
                        'Added 1/26/18 for HOM Upgrade MLW
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.MineSubsidenceConflict
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                    Case Optional_Coverages_Hom_Validator.CovASpecificed_ACVConflic
                        If v.IsWarning Then
                            Me.ValidationHelper.AddWarning(v.Message)
                        Else
                            Me.ValidationHelper.AddError(v.Message)
                        End If

                        'ValidationHelper.Val_BindValidationItemToControl(txtML_306_IncreasedData, v, divOptCoverages, "0")
                End Select
            Next
        End If



        Me.ValidateChildControls(valArgs)
    End Sub



    'Private Function GetSupportedPrimaryCoverages() As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
    '    ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
    '    Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
    '    If Me.MyLocation.IsNotNull Then
    '        '******************************    Included Coverages   *******************************
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware)
    '        '******************************    Included Coverages   *******************************


    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage) ' Cov. A - Specified Additional Amount of Insurance (29-034)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake)

    '    End If
    '    Return supportedCoverages
    'End Function

    'Private Function GetSupportedSectionICoverages() As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
    '    ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
    '    Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
    '    If Me.MyLocation.IsNotNull Then

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Consent_to_Move_Mobile_Home)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment)


    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles)

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment) 'add With Comp rater Project
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake) 'add With Comp rater Project

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB)

    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MobileHomeLienholdersSingleInterest)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit)

    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty)

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage) '  add With Comp rater Project
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises) 'Specified Other Structures - Off Premises (92-147)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures) 'Specified Other Structures - On Premises (92-049)      add With Comp rater Project
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TripCollision)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageA)


    '        'availableCoverages.Add() 'Outdoor Antennas (ML-49)

    '    End If
    '    Return supportedCoverages
    'End Function


    'Private Function GetSupportedSectionIICoverages() As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)
    '    ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
    '    Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)
    '    If Me.MyLocation.IsNotNull Then

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured) 'add With Comp rater Project
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther) 'add With Comp rater Project


    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.Animal_Collision)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical) 'add With Comp rater Project
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_ExcludingInstallation) ' removed from compRater
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_IncludingInstallation) ' removed from compRater
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment) ' removed from compRater
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment) ' removed from compRater
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment) ' removed from compRater
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment) ' removed from compRater
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured160_500Acres)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsuredOver500Acres)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.HomeDayCareLiability)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured)
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises) 'add With Comp rater Project
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.) 'add With Comp rater Project
    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence) 'add With Comp rater Project
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.WaterbedCoverage)
    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType._3Or4FamilyLiability)

    '        ' need ho-40 ?????????

    '    End If
    '    Return supportedCoverages
    'End Function

    'Private Function GetSupportedSectionIAndIICoverages() As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)

    '    ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
    '    Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)

    '    If Me.MyLocation.IsNotNull Then

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers) 'add With Comp rater Project - HO-40

    '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures) ' HO-42

    '        'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers)

    '    End If
    '    Return supportedCoverages
    'End Function

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate.Click
        Me.Fire_GenericBoardcastEvent(BroadCastEventType.RateRequested)
    End Sub

    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Protected Sub btnViewGotoNextSection_Click(sender As Object, e As EventArgs) Handles btnViewGotoNextSection.Click
        'Added 7/15/2019 for Home Endorsements Project Task 38925 MLW
        Me.Fire_BroadcastWorkflowChangeRequestEvent(IFM.VR.Common.Workflow.Workflow.WorkflowSection.billingInformation, "0")
    End Sub

    Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        'clear the coverages
        Save_FireSaveEvent(False)
        IFM.VR.Common.Helpers.HOM.SectionCoverage.ClearCoverages(Me.Quote, Me.MyLocation)
        Populate_FirePopulateEvent()
        Save_FireSaveEvent(False)
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    'Added 5/16/2022 for task 74106 MLW
    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Dim SectICovAndAssoc As Common.Helpers.HOM.SectionICovTypeAndAssociate = e.Item.DataItem
        Dim ctlSectionCovItem As ctlSectionCoverageItem = e.Item.FindControl("ctlSectionCoverageItem")
        If SectICovAndAssoc IsNot Nothing AndAlso ctlSectionCovItem IsNot Nothing Then
            ctlSectionCovItem.SetSectionICoverageEnumAndAssociate(SectICovAndAssoc.SectionICoverageEnum, SectICovAndAssoc.AssociatedSectionICoverageEnum)
        End If
    End Sub
End Class

