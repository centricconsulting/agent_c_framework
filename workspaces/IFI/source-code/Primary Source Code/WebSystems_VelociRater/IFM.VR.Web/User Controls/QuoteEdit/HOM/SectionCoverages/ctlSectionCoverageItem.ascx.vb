Imports IFM.PrimativeExtensions
Imports IFM.Common.InputValidation.InputHelpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM.SectionCoverage
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption

Public Class ctlSectionCoverageItem
    Inherits ctlSectionCoverageControlBase


    'Do Not check the coverage checkbox for checked/notchecked it will be disabled so you can not rely on its value... used the hndhomOptionalCovChk.Value (0 or 1 for false/True) instead

    'added 12/21/17 for HOM Upgrade - MLW
    Dim _qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim _chc As New CommonHelperClass

    Public Sub New()

    End Sub

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
            If _qqh.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Protected ReadOnly Property HomCyberEffDate As DateTime = CDate("9/1/2020")


    Public Overrides Sub AddScriptAlways()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    Public Overrides Sub AddScriptWhenRendered()
        Me.ValidationHelper.ControlWasRendered = True

        ' confirm on un-checks
        If Me.SectionCoverageIEnum <> QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains AndAlso Me.SectionCoverageIEnum <> QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage AndAlso Me.SectionCoverageIEnum <> QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades AndAlso Me.SectionCoverageIAndIIEnum <> QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment AndAlso Me.SectionCoverageIAndIIEnum <> QuickQuoteSectionICoverage.SectionICoverageType.Farm_Fire_Department_Service_Charge AndAlso Me.SectionCoverageIAndIIEnum <> QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers Then
            'backup&sewer is only checked and uncheck by the system so you don't want this logic executed by system checks/unchecks
            'Updated 1/16/18 for HOM Upgrade - water damage is only checked and unchecked by the system, so you do not want this logic executed by system checks/unchecks
            'Updated 1/26/18 for HOM Upgrade - added Green Upgrades
            'Updated 3/23/18 for HOM Upgrade - added Fire Department Service Charge and Loss Assessment
            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                    "if($('#{0}').is(':checked') == false){{ if(confirm('Are you sure you want to delete this item?') == false){{ $('#{0}').prop('checked', true);}}  }}".FormatIFM(Me.chkCov.ClientID), False)
        End If

        ' counts the checkboxes that have a specific css class and that are checked to set the section header text
        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                    "$('#lblHomOptionCoverageHeader').text('(' + $('.homOptionalCovChk input:checked').length.toString() +')');".FormatIFM(Me.chkCov.ClientID), True)

        'since the checkboxes can be disabled you need to hold the isChecked value in a hidden field 
        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked')){{$('#{1}').val('1');}}else{{ $('#{1}').val('0'); }}".FormatIFM(Me.chkCov.ClientID, Me.hndhomOptionalCovChk.ClientID), True)


        If Me.MyDisplayType <> DisplayType.justCheckBox Then

            ' hide and show details div on check change
            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                                "if($('#{0}').is(':checked')){{$('#{1}').show();}}else{{ $('#{1}').hide(); }}".FormatIFM(Me.chkCov.ClientID, Me.divDetails.ClientID), True)

            Select Case Me.MyDisplayType
                Case DisplayType.hasIncreaseWithFreeForm, DisplayType.hasIncreaseWithLocation
                    Dim script As String = "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                    Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onkeyup, script, True)
                    'Updated 12/27/17 for HOM Upgrade MLW
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        'If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge Then
                        '    Dim scriptFDSC As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNearest100($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest100(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                        '    Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptFDSC, True)
                        'Else
                        '    'Added 1/19/18 for HOM Upgrade MLW
                        '    If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities Then
                        '        Dim scriptPPSS As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val((($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                        '        Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptPPSS, True)
                        '    Else
                        '        Me.VRScript.CreateTextBoxFormatter(Me.txtIncreaseLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
                        '    End If
                        'End If
                        Dim autoRunAtStartup As Boolean = True
                        If IsEndorsementRelated() Then
                            autoRunAtStartup = False
                        End If
                        Select Case Me.SectionCoverageIEnum
                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge
                                Dim scriptFDSC As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNearest100($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest100(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                                Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptFDSC, autoRunAtStartup)
                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities
                                Dim scriptPPSS As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val((($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                                Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptPPSS, autoRunAtStartup)
                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms
                                If IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.IsHOMTheftOfFirearmsIncreaseAvailable(quote) Then
                                    Dim scriptFirearms As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNearest100($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest100(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                                    Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptFirearms, autoRunAtStartup)
                                End If
                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs
                                If IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.IsHOMTheftOfJewelryIncreaseAvailable(quote) Then
                                    Dim scriptJewelry As String = "$('#{2}').val(ifm.vr.stringFormating.asRoundToNearest100($('#{2}').val()));$('#{2}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{2}').val()));$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest100(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.txtIncreaseLimit.ClientID)
                                    Me.VRScript.CreateJSBinding(Me.txtIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptJewelry, autoRunAtStartup)
                                End If
                            Case Else
                                Me.VRScript.CreateTextBoxFormatter(Me.txtIncreaseLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
                        End Select
                    Else
                        Me.VRScript.CreateTextBoxFormatter(Me.txtIncreaseLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
                    End If
                    ' used to clear the control when Unchecked
                    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); {2} }}".FormatIFM(Me.chkCov.ClientID, Me.txtIncreaseLimit.ClientID, script))

                Case DisplayType.hasIncreasewithDropDown
                    Dim script As String = "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(($('#{1}').val().toFloat() + $('#{2} option:selected').text().toFloat()).toString()));".FormatIFM(Me.txtTotalLimit.ClientID, Me.txtIncludedLimit.ClientID, Me.ddIncreasedLimit.ClientID)
                    Me.VRScript.CreateJSBinding(Me.ddIncreasedLimit, ctlPageStartupScript.JsEventType.onchange, script, True)
                    ' used to clear the control when Unchecked
                    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); {2} }}".FormatIFM(Me.chkCov.ClientID, Me.ddIncreasedLimit.ClientID, script))

                Case DisplayType.justEffectiveDate
                    If IsDate(Me.Quote.EffectiveDate) Then
                        Dim effectiveDate = CDate(Me.Quote.EffectiveDate)
                        ' you know the effective date so limit this date between the effective date and term end
                        Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, effectiveDate, effectiveDate.AddYears(1))
                    Else
                        ' effective date is not valid so you can't limit it
                        Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, False)
                    End If
                    ' used to clear the control when Unchecked
                    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                    "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtEffectiveDate.ClientID))
                Case DisplayType.justLimit
                    Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
                    Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)
                    'Updated 4/26/18 for Bug 26128 MLW
                    If Me.SectionCoverageIEnum <> QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine Then
                        ' used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtLimit.ClientID))
                    End If
                Case DisplayType.hasLimitAndDescription
                    Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
                    Me.VRScript.CreateTextBoxFormatter(Me.txtLimit, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)
                    ' used to clear the control when Unchecked
                    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                    "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtLimit.ClientID))
                    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                    "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtDescription.ClientID))
                Case DisplayType.justDescription
                    ' used to clear the control when Unchecked
                    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                    "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtDescription.ClientID))

                Case DisplayType.justDeductible
                    'updated 12/21/17 to auto select 5% for earthquake deductible for HOM Upgrade MLW
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) AndAlso Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake Then
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                            "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }} else {{ $('#{1}').val('224'); }}".FormatIFM(Me.chkCov.ClientID, Me.ddDeductible.ClientID))
                    Else
                        'used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                            "if($('#{0}').is(':checked') == false){{ $('#{1}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.ddDeductible.ClientID))
                    End If
                Case DisplayType.hasEffectiveAndExpirationDates
                    'Added 1/15/18 for HOM Upgrade MLW
                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        If IsDate(Me.Quote.EffectiveDate) Then
                            Dim effectiveDate = CDate(Me.Quote.EffectiveDate)
                            ' you know the effective date so limit this date between the effective date and term end
                            Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, effectiveDate, effectiveDate.AddYears(1))
                            Me.VRScript.CreateDatePicker(Me.txtExpirationDate.ClientID, effectiveDate, effectiveDate.AddYears(1))
                        Else
                            ' effective date is not valid so you can't limit it
                            Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, False)
                            Me.VRScript.CreateDatePicker(Me.txtExpirationDate.ClientID, False)
                        End If
                        Me.txtEffectiveDate.CreateMask("00/00/0000")
                        Me.txtExpirationDate.CreateMask("00/00/0000")
                        ' used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val('');$('#{2}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtEffectiveDate.ClientID, Me.txtExpirationDate.ClientID))
                    End If
                Case DisplayType.hasEffAndExpDatesWithLimit
                    'Added 1/22/18 for HOM Upgrade MLW
                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        If IsDate(Me.Quote.EffectiveDate) Then
                            Dim effectiveDate = CDate(Me.Quote.EffectiveDate)
                            ' you know the effective date so limit this date between the effective date and term end
                            Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, effectiveDate, effectiveDate.AddYears(1))
                            Me.VRScript.CreateDatePicker(Me.txtExpirationDate.ClientID, effectiveDate, effectiveDate.AddYears(1))
                        Else
                            ' effective date is not valid so you can't limit it
                            Me.VRScript.CreateDatePicker(Me.txtEffectiveDate.ClientID, False)
                            Me.VRScript.CreateDatePicker(Me.txtExpirationDate.ClientID, False)
                        End If
                        Me.txtEffectiveDate.CreateMask("00/00/0000")
                        Me.txtExpirationDate.CreateMask("00/00/0000")

                        Dim scriptPPSS As String = "$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{0}').val()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()))".FormatIFM(Me.txtLimit.ClientID)
                        Me.VRScript.CreateJSBinding(Me.txtLimit, ctlPageStartupScript.JsEventType.onchange, scriptPPSS, True)

                        ' used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val('');$('#{2}').val('');$('#{3}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtEffectiveDate.ClientID, Me.txtExpirationDate.ClientID, Me.txtLimit.ClientID))

                    End If
                Case DisplayType.isBusinessPursuits
                    'Added 1/24/18 for HOM Upgrade MLW
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        ' used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val('');$('#{2}').val('');$('#{3}').val('');$('#{4}').val('');$('#{5}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtInsuredFirstName.ClientID, Me.txtInsuredMiddleName.ClientID, Me.txtInsuredLastName.ClientID, Me.ddInsuredSuffixName.ClientID, Me.txtInsuredBusinessName.ClientID))
                    End If
                Case DisplayType.isGreenUpgrades
                    'Added 1/25/18 for HOM Upgrade MLW
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim scriptGUmax As String = "$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{0}').val()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()))".FormatIFM(Me.txtMaximumAmount.ClientID)
                        Me.VRScript.CreateJSBinding(Me.txtMaximumAmount, ctlPageStartupScript.JsEventType.onchange, scriptGUmax, True)
                        Dim scriptGUlimit As String = "$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{0}').val()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()))".FormatIFM(Me.txtExpRelCovLimit.ClientID)
                        Me.VRScript.CreateJSBinding(Me.txtExpRelCovLimit, ctlPageStartupScript.JsEventType.onchange, scriptGUlimit, True)
                    End If
                Case DisplayType.isBusinessStructure
                    'Added 1/29/18 for HOM Upgrade MLW
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim scriptBusStr As String = "$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{0}').val()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val()))".FormatIFM(Me.txtBuildingLimit.ClientID)
                        Me.VRScript.CreateJSBinding(Me.txtBuildingLimit, ctlPageStartupScript.JsEventType.onchange, scriptBusStr, True)
                        'only way to know if Other Structures should be checked is if either textbox is filled in since both are required if this checkbox is checked
                        Me.VRScript.CreateJSBinding(Me.txtBuildingDescription.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').val() != """"){{ $('#{2}').prop('checked', true); }} else {{ if($('#{1}').val() != """") {{ $('#{2}').prop('checked', true); }} else {{ $('#{2}').prop('checked', false); }} }}".FormatIFM(Me.txtBuildingDescription.ClientID, Me.txtBuildingLimit.ClientID, Me.chkOtherStructures.ClientID))
                        Me.VRScript.CreateJSBinding(Me.txtBuildingLimit.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{1}').val() != """"){{ $('#{2}').prop('checked', true); }} else {{ if($('#{0}').val() != """") {{ $('#{2}').prop('checked', true); }} else {{ $('#{2}').prop('checked', false); }} }}".FormatIFM(Me.txtBuildingDescription.ClientID, Me.txtBuildingLimit.ClientID, Me.chkOtherStructures.ClientID))
                        'Updated 7/3/2019 for Home Endorsements Project Task 38914, 38925 MLW
                        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                            Me.VRScript.CreateJSBinding(Me.chkOtherStructures.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked')){{ if($('#{1}').val() == """") {{ $('#{1}').val('BUILDING'); }} }} else {{$('#{1}').val('');$('#{2}').val(''); }}".FormatIFM(Me.chkOtherStructures.ClientID, Me.txtBuildingDescription.ClientID, Me.txtBuildingLimit.ClientID))
                        End If
                        ' used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val('');$('#{2}').prop('checked', false);$('#{3}').val('');$('#{4}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtBusinessDescription.ClientID, Me.chkOtherStructures.ClientID, Me.txtBuildingDescription.ClientID, Me.txtBuildingLimit.ClientID))
                    End If


                Case DisplayType.isAdditionlResidence, DisplayType.isFarmLand, DisplayType.isOtherStructures, DisplayType.isCanine, DisplayType.isOtherMembers, DisplayType.isAdditionalInterests, DisplayType.isTrust, DisplayType.isAdditionalInsured '1/23/18 Added new display type isCanine, 1/30/18 added isLossAssessment, 1/31/18 added isOtherMembers and isAdditionalInterests, 2/14/18 added isTrust for HOM Upgrade MLW '3/16/18 - no longer using isLossAssessment, no longer capturing multiples and address '4/30/18 added isAdditionalInsured MLW
                    ' need to disable the checkbox once it is checked

                    'Updated 1/2/18 form number changed from 92-049 to HO 0448 for the HOM Upgrade MLW
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        If Me.lblHeader.Text.Contains("(HO 0448)") = False Then 'horribly ugly hack here - Created by Matt A - hopefully I will find a better way to do this at some point
                            ' disables the checkbox because it is a list - on these you remove the coverage by removing all items
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                                    "if($('#{0}').is(':checked')){{$('#{0}').attr('disabled','disabled');$('#{1}').val('1');}}".FormatIFM(Me.chkCov.ClientID, Me.hndhomOptionalCovChk.ClientID), True)
                        End If

                        'Updated 1/2/18 form number changed from 92-049 to HO 448 for the HOM Upgrade MLW
                        If Me.MyDisplayType = DisplayType.isOtherStructures AndAlso Me.lblHeader.Text.Contains("(HO 0448)") Then
                            ' need clear logic on uncheck
                            ' used to clear the control when Unchecked
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                            "if($('#{0}').is(':checked') == false){{ {1} }}".FormatIFM(Me.chkCov.ClientID, Me.ctlHomSpecifiedStructureList.JsClearControl_92049))
                        End If

                        If Me.MyDisplayType = DisplayType.isAdditionalInterests Then
                            'Added 2/5/18 for HOM Upgrade MLW
                            If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then
                                Dim scriptALCCpropTotal As String = "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(($('#{1}').val().toFloat() + $('#{2}').val().toFloat()).toString()));".FormatIFM(Me.txtPropTotalLimit.ClientID, Me.txtPropIncludedLimit.ClientID, Me.txtPropIncreaseLimit.ClientID)
                                Me.VRScript.CreateJSBinding(Me.txtPropIncreaseLimit, ctlPageStartupScript.JsEventType.onkeyup, scriptALCCpropTotal, True)
                                Dim scriptALCCliabTotal As String = "$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas(($('#{1}').val().toFloat() + $('#{2} option:selected').text().toFloat()).toString()));".FormatIFM(Me.txtLiabTotalLimit.ClientID, Me.txtLiabIncludedLimit.ClientID, Me.ddLiabIncreaseLimit.ClientID)
                                Me.VRScript.CreateJSBinding(Me.ddLiabIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptALCCliabTotal, True)
                                Dim scriptALCCpropFormat As String = "$('#{0}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{0}').val()));$('#{0}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{0}').val())); $('#{1}').val(ifm.vr.stringFormating.asRoundToNearest1000($('#{1}').val()));$('#{1}').val(ifm.vr.stringFormating.asNumberWithCommas($('#{1}').val()))".FormatIFM(Me.txtPropIncreaseLimit.ClientID, Me.txtPropTotalLimit.ClientID)
                                Me.VRScript.CreateJSBinding(Me.txtPropIncreaseLimit, ctlPageStartupScript.JsEventType.onchange, scriptALCCpropFormat, True)
                            End If
                        End If
                    Else
                        If Me.lblHeader.Text.Contains("(92-049)") = False Then 'horribly ugly hack here - Created by Matt A - hopefully I will find a better way to do this at some point
                            '    ' disables the checkbox because it is a list - on these you remove the coverage by removing all items
                            '    Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                            '                            "If($('#{0}').is(':checked')){{$('#{0}').attr('disabled','disabled');$('#{1}').val('1');}}".FormatIFM(Me.chkCov.ClientID, Me.hndhomOptionalCovChk.ClientID), True)
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                                    "if($('#{0}').is(':checked')){{$('#{0}').attr('disabled','disabled');$('#{1}').val('1');}}".FormatIFM(Me.chkCov.ClientID, Me.hndhomOptionalCovChk.ClientID), True)

                        End If

                        If Me.MyDisplayType = DisplayType.isOtherStructures AndAlso Me.lblHeader.Text.Contains("(92-049)") Then
                            ' need clear logic on uncheck
                            ' used to clear the control when Unchecked
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                            "if($('#{0}').is(':checked') == false){{ {1} }}".FormatIFM(Me.chkCov.ClientID, Me.ctlHomSpecifiedStructureList.JsClearControl_92049))
                        End If
                    End If

                Case DisplayType.isSpecialEventCoverage
                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then

                        ' used to clear the control when Unchecked
                        Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ $('#{1}').val('');$('#{2}').val('');$('#{3}').val('');$('#{4}').val('');$('#{5}').val('');$('#{6}').val('');$('#{7}').val('');$('#{8}').val('');$('#{9}').val(''); }}".FormatIFM(Me.chkCov.ClientID, Me.txtSpecialEventDescription.ClientID, Me.txtSpecialEventStreetNumber.ClientID, Me.txtSpecialEventStreetName.ClientID, Me.txtSpecialEventAptSuiteNumber.ClientID, Me.txtSpecialEventZip.ClientID, Me.txtSpecialEventCity.ClientID, Me.ddlSpecialEventState.ClientID, Me.txtSpecialEventCounty.ClientID, Me.ddInsuredSuffixName.ClientID, Me.txtInsuredBusinessName.ClientID))
                        ' Zip code lookup script
                        Me.VRScript.CreateJSBinding(Me.txtSpecialEventZip, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtSpecialEventZip.ClientID + "','" + Me.ddlSpecialEventCity.ClientID + "','" + Me.txtSpecialEventCity.ClientID + "','" + Me.txtSpecialEventCounty.ClientID + "','" + Me.ddlSpecialEventState.ClientID + "');")

                        ' Script to handle add and delete buttons
                        Me.VRScript.CreateJSBinding(Me.lbSpecialEventAddAddress, ctlPageStartupScript.JsEventType.onclick, "HandleSpecialEventAddButtonClick('" & trSpecialEventAddress.ClientID & "','" & Me.lbSpecialEventAddAddress.ClientID & "','" & Me.lbSpecialEventDeleteAddress.ClientID & "');")
                        'Me.VRScript.CreateJSBinding(Me.lbSpecialEventDeleteAddress, ctlPageStartupScript.JsEventType.onclick, "HandleSpecialEventDeleteButtonClick('" & trSpecialEventAddress.ClientID & "','" & txtSpecialEventStreetNumber.ClientID & "','" & txtSpecialEventStreetName.ClientID & "','" & txtSpecialEventAptSuiteNumber.ClientID & "','" & txtSpecialEventZip.ClientID & "','" & txtSpecialEventCity.ClientID & "','" & ddlSpecialEventState.ClientID & "','" & txtSpecialEventCounty.ClientID & "','" & lbSpecialEventAddAddress.ClientID & "','" & lbSpecialEventDeleteAddress.ClientID & "');")
                    End If
                    Exit Select

            End Select
        End If
        Dim usedPopupIds As New List(Of String)
        Select Case Me.SectionType
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                'Dim usedPopupIds As New List(Of String)
                Select Case Me.SectionCoverageIEnum
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                        'Added 1/25/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.VRScript.AddVariableLine("var checkBoxId_persPropRepl = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            'Green Upgrades is only available when Personal Property Replacement is selected
                            If ((MyLocation.FormTypeId = "22" AndAlso MyLocation.StructureTypeId = "2") OrElse MyLocation.FormTypeId = "25") Then
                                'no Green Upgrades for these coverages, so do not need to add script
                            Else
                                Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                        "if($('#' + checkBoxId_persPropRepl).is(':checked')) { $('#' + checkBoxId_greenUpgrades).show();$('#' + labelId_greenUpgrades).show();$('#' + linkId_greenUpgrades).show();} else { $('#' + checkBoxId_greenUpgrades).prop('checked',false);$('#' + checkBoxId_greenUpgrades).change();$('#' + checkBoxId_greenUpgrades).hide();$('#' + labelId_greenUpgrades).hide();$('#' + linkId_greenUpgrades).hide();$('#' + divDetails_greenUpgrades).hide();$('#' + txtId_MaximumAmount).val('');$('#' + ddId_IncreasedCostOfLoss).val('');$('#' + checkBoxId_vegetatedRoof).prop('checked',false);$('#' + checkBoxId_expRelCov).prop('checked',false);$('#' + txtId_expRelCovLimit).val('');  }", True)
                            End If
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades
                        'Added 1/25/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            'only available when Personal Property Replacement is selected - below variables are used to hide and clear Green Upgrades when Personal Property Replacement not selected
                            Me.VRScript.AddVariableLine("var checkBoxId_greenUpgrades = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var labelId_greenUpgrades = '{0}';".FormatIFM(Me.lblHeader.ClientID))
                            Me.VRScript.AddVariableLine("var linkId_greenUpgrades = '{0}';".FormatIFM(Me.lnkHelp.ClientID))
                            Me.VRScript.AddVariableLine("var divDetails_greenUpgrades = '{0}';".FormatIFM(Me.divDetails.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_MaximumAmount = '{0}';".FormatIFM(Me.txtMaximumAmount.ClientID))
                            Me.VRScript.AddVariableLine("var ddId_IncreasedCostOfLoss = '{0}';".FormatIFM(Me.ddIncreasedCostOfLoss.ClientID))
                            Me.VRScript.AddVariableLine("var checkBoxId_vegetatedRoof = '{0}';".FormatIFM(Me.chkVegetatedRoofApplies.ClientID))
                            Me.VRScript.AddVariableLine("var checkBoxId_expRelCov = '{0}';".FormatIFM(Me.chkExpRelCov.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_expRelCovLimit = '{0}';".FormatIFM(Me.txtExpRelCovLimit.ClientID))

                            'check ExpRelCov if a limit is supplied, clear limit if unchecked
                            Me.VRScript.CreateJSBinding(Me.chkExpRelCov, ctlPageStartupScript.JsEventType.onchange,
                                                        "if($('#' + checkBoxId_expRelCov).is(':checked')) { if($('#' +  txtId_expRelCovLimit).val() == """") { } else { $('#' + checkBoxId_expRelCov).prop('checked',true); } } else { $('#' + checkBoxId_expRelCov).prop('checked',false);$('#' + txtId_expRelCovLimit).val('');  }", True)
                            Me.VRScript.CreateJSBinding(Me.txtExpRelCovLimit, ctlPageStartupScript.JsEventType.onchange,
                                                        "if($('#' +  txtId_expRelCovLimit).val() == """") { } else { $('#' + checkBoxId_expRelCov).prop('checked',true); }", True)

                            Dim greenPopupMessage As String = "<br /><div>Provides coverage for green upgrades to the residence premises, other building structures, personal property, and for some related expenses.</div>"
                            Using popupGreen As New PopupMessageClass.PopupMessageObject(Me.Page, greenPopupMessage, "Green Upgrades Note:")
                                With popupGreen
                                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                                    .isModal = False
                                    .AddButton("OK", True)
                                    .width = 325
                                    .height = 175
                                    .AddControlToBindTo(lnkHelp)
                                    .divId = "greenPopup"
                                    .CreateDynamicPopUpWindow()
                                End With
                            End Using
                            'usedPopupIds.Add(Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv("Green Upgrades Note:", greenPopupMessage, 325, 175, False, True, True, lnkHelp.ClientID, Nothing, usedPopupIds))

                            ' used to clear the control when Unchecked
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                        "if($('#{0}').is(':checked') == false){{ if($('#' + checkBoxId_persPropRepl).is(':checked') == true) {{ if(confirm('Are you sure you want to delete this item?') == false){{  $('#{0}').prop('checked', true);$('#{1}').show(); }} else {{ $('#{1}').hide();$('#{2}').val('');$('#{3}').val('');$('#{4}').prop('checked', false);$('#{5}').prop('checked', false);$('#{6}').val(''); }} }}  }} else {{ $('#{1}').show(); }}".FormatIFM(Me.chkCov.ClientID, Me.divDetails.ClientID, Me.txtMaximumAmount.ClientID, Me.ddIncreasedCostOfLoss.ClientID, Me.chkVegetatedRoofApplies.ClientID, Me.chkExpRelCov.ClientID, Me.txtExpRelCovLimit.ClientID), False)
                        End If
                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage
                        'Not available with QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement selected
                        'do not bind js to HO 0002 MOBILE HOME, HO 0003 SPECIAL FORM, and HO 0005 COMPREHENSIVE FORM - doing so causes coverages to disappear. BD
                        If (Quote.Locations(0).StructureTypeId <> "20" AndAlso Not Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24")) Then
                            Me.VRScript.AddVariableLine("var checkBoxId_29_034 = '{0}';".FormatIFM(Me.chkCov.ClientID)) 'used to make sure you don't have ACV with this coverage
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                        "if($('#' + checkBoxId_29_034).is(':checked')){ $('#' + checkBoxId_ho41_81).attr('disabled',true);} else { $('#' + checkBoxId_ho41_81).removeAttr('disabled'); }", True)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement
                        ' not available with SectionICoverageType.Home_CoverageASpecialCoverage selected
                        'Updated 12/28/17 for HOM Upgrade MLW - HO 0002 Mobile has ACV auto checked, but does not have Cov A. Need to not bind this for HO 002 Mobile so coverage page displays without errors/explosions
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId = "22" AndAlso Quote.Locations(0).StructureTypeId = "2") Then
                            'do not bind js to HO 0002 Mobile - doing so causes coverages to disappear. MLW
                        Else
                            If (Quote.Locations(0).StructureTypeId <> "20" AndAlso Not Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24")) Then
                                Me.VRScript.AddVariableLine("var checkBoxId_ho41_81 = '{0}';".FormatIFM(Me.chkCov.ClientID)) ' used to make sure you don't have Cov A with this coverage
                                Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                            "if($('#' + checkBoxId_ho41_81).is(':checked')) {$('#' + checkBoxId_29_034).attr('disabled',true);} else {$('#' + checkBoxId_29_034).removeAttr('disabled');}", True)
                            End If
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge
                        Me.VRScript.AddVariableLine("var checkBoxId_FireDeptSvcCharge = '{0}';".FormatIFM(Me.chkCov.ClientID))
                        Me.VRScript.AddVariableLine("var divDetails_FireDeptSvcCharge = '{0}';".FormatIFM(Me.divDetails.ClientID))
                        Me.VRScript.AddScriptLine("$('#' + checkBoxId_FireDeptSvcCharge).attr('disabled',true);$('#' + checkBoxId_FireDeptSvcCharge).prop('checked',true);$('#' + divDetails_FireDeptSvcCharge).show()")
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement,
                         QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019
                        Me.VRScript.AddVariableLine("var checkBoxId_enhancement = '{0}';".FormatIFM(Me.chkCov.ClientID))

                        'Added 1/10/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange, "HandlePersonalInjuryCheckbox('" & chkCov.ClientID & "');")
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_PersonalInjury).prop('checked',true); $('#' + checkBoxId_PersonalInjury).attr('disabled',true);}", True)


                            Dim PreCyber As String = "0"
                            If CDate(Quote.EffectiveDate) < HomCyberEffDate Then PreCyber = "1"
                            Dim SeasonalOrSecondary As String = "0"
                            If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then SeasonalOrSecondary = "1"
                            VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange, "HandleHOEnhancementCheckboxClicks('" & chkCov.ClientID & "','" & PreCyber & "','" & SeasonalOrSecondary & "');", True)

                            ' REPLACED THE FOLLOWING LOGIC WITH THE ABOVE FUNCTION
                            ' if selected you must have water backup, cannot have homeowners plus enhancement and water damage, cannot have identity fraud expense
                            ' if not selected you can not have water backup, can have homeowners plus or identity fraud
                            'If (CDate(Quote.EffectiveDate) >= HomCyberEffDate AndAlso Quote.Locations(0).FormTypeId <> "25") Then
                            '    ' All quotes after cyber eff date cannot have identity fraud unless HO-4
                            '    If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then
                            '        ' When seasonal or secondary we need to remove the Homeowners Plus and water damage coverages from the script, it's not available
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                            "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();}", True)
                            '    Else
                            '        'Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '        '"if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();}", True)
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                            "alert('hello!'); if( $('#' + checkBoxId_enhancement).is(':checked') )  	{	if (typeof checkBoxId_plusenhancement !== 'undefined' && checkBoxId_plusenhancement !== null)	{	$('#' + checkBoxId_plusenhancement).prop('checked',false);	$('#' + checkBoxId_plusenhancement).attr('disabled',true);	}	if (typeof checkBoxId_waterDamage !== 'undefined' && checkBoxId_waterDamage !== null)	{	$('#' + checkBoxId_waterDamage).prop('checked',false);	}	$('#' + checkBoxId_backup).prop('checked',true);	$('#' + checkBoxId_backup).change();	$('#' + checkBoxId_backup).attr('disabled',true);	$('#' + divDetails_backup).show(); 	}  else  	{	if (typeof checkBoxId_plusenhancement !== 'undefined' && checkBoxId_plusenhancement !== null)	{	$('#' + checkBoxId_plusenhancement).attr('disabled',false);	}	$('#' + checkBoxId_backup).prop('checked',false); 	$('#' + checkBoxId_backup).change();	$('#' + checkBoxId_backup).attr('disabled',true);	$('#' + divDetails_backup).hide(); 	}", True)
                            '    End If
                            'Else
                            '    ' HO-4 or eff date is pre-cyber
                            '    ' Include identity fraud
                            '    'Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '    '                        "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                "if( $('#' + checkBoxId_enhancement).is(':checked') )  { 	if (typeof checkBoxId_plusenhancement !== 'undefined' && checkBoxId_plusenhancement !== null) 	{	$('#' + checkBoxId_plusenhancement).prop('checked',false);	$('#' + checkBoxId_plusenhancement).attr('disabled',true); 	} 	$('#' + checkBoxId_identityFraud).prop('checked',false); 	$('#' + checkBoxId_identityFraud).attr('disabled',true); 	if (typeof checkBoxId_waterDamage !== 'undefined' && checkBoxId_waterDamage !== null) 	{	$('#' + checkBoxId_waterDamage).prop('checked',false); 	} 	$('#' + checkBoxId_backup).prop('checked',true);  	$('#' + checkBoxId_backup).change();  	$('#' + checkBoxId_backup).attr('disabled',true); 	$('#' + divDetails_backup).show(); }  else  { 	$('#' + checkBoxId_backup).prop('checked',false); 	$('#' + checkBoxId_backup).change(); 	$('#' + checkBoxId_backup).attr('disabled',true); 	$('#' + divDetails_backup).hide(); 	if (typeof checkBoxId_plusenhancement !== 'undefined' && checkBoxId_plusenhancement !== null) 	{	$('#' + checkBoxId_plusenhancement).attr('disabled',false);	if( $('#' + checkBoxId_plusenhancement).is(':checked') ) 	{	$('#' + checkBoxId_identityFraud).prop('checked',false);	$('#' + checkBoxId_identityFraud).attr('disabled',true);	}	else	{	$('#' + checkBoxId_identityFraud).attr('disabled',false);	} 	} 	 }", True)
                            'End If
                            ' END OF REPLACED CODE

                            'If (CDate(Quote.EffectiveDate) >= HomCyberEffDate OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5")) AndAlso (Quote.Locations(0).StructureTypeId <> "2" AndAlso Quote.Locations(0).FormTypeId <> "25") Then
                            '    ' After the cyber eff date cutoff we removed identity fraud from the script.
                            '    ' Also applies for seasonal or secondary occupancy as these are not eligible for identity fraud
                            '    If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then
                            '        ' When seasonal or secondary we need to remove the Homeowners Plus and water damage coverages from the script, it's not available
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();}", True)
                            '    Else
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();}", True)
                            '    End If
                            'Else
                            '    ' Include identity fraud
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            'End If

                            'If CDate(Quote.EffectiveDate) < HomCyberEffDate OrElse (Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso (Quote.Locations(0).FormTypeId = "25" OrElse Quote.Locations(0).StructureTypeId = "2")) Then
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            'Else
                            '    ' After the cyber eff date cutoff we removed identity fraud from teh script.
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();}", True)
                            'End If

                            Dim homeEnPopupMessage As String = ""
                            Dim homeEnPopupTitle As String = ""

                            If CDate(Quote.EffectiveDate) < HomCyberEffDate OrElse (Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso (Quote.Locations(0).FormTypeId = "25" OrElse Quote.Locations(0).StructureTypeId = "2" OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5"))) Then
                                ' Popup text for HOM1010 (old enhancement)
                                homeEnPopupTitle = "Homeowners Enhancement Endorsement Note:"
                                homeEnPopupMessage = "<br /><div><b>The Homeowners Enhancement provides additional valuable coverages such as:</b></div><div><ul>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>The peril of Landslide is included for coverages A, B and C.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$500 for spoilage of refrigerated or frozen food.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$100 for expenses incurred for the accidental activation of a premises alarm system.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$500 to replace locks on exterior doors and to replace portable garage door transmitters if any of these are lost or stolen.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$500 for additional living expenses if your home becomes uninhabitable due to an off-premises power outage.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$100 for coverage of any domestic animal.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$5,000 for damage caused by water backup through sewers or drains and overflow from within a sump pump. For an added charge, this amount can be increased.</li>"
                                'homeEnPopupMessage = homeEnPopupMessage & "<li>$10,000 for the expense incurred to recover your identity if it is stolen.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$15,000 for the expense incurred to recover your identity if it is stolen.</li>"  ' MGB 6/18/20 Task 49456
                                homeEnPopupMessage = homeEnPopupMessage & "<li>Coverage for direct physical loss to personal property items up to $500.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>Including coverage for personal injury liability coverage.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>Policies with loss payments $500 or less are not subject to loss surcharges.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "</ul></div>"
                            Else
                                ' Popup text for HOM1019 (new enhancement)
                                homeEnPopupTitle = "Home Enhancement Endorsement Note:"
                                homeEnPopupMessage = "<br /><div><b>The Home Enhancement provides additional valuable coverages such as:</b></div><div><ul>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>The peril of Landslide is included for coverages A, B and C.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$500 for spoilage of refrigerated or frozen food.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$100 for expenses incurred for the accidental activation of a premises alarm system.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$500 to replace locks on exterior doors and to replace portable garage door transmitters if any of these are lost or stolen.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$500 for additional living expenses if your home becomes uninhabitable due to an off-premises power outage.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$100 for coverage of any domestic animal.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>$5,000 for damage caused by water backup through sewers or drains and overflow from within a sump pump. For an added charge, this amount can be increased.</li>"
                                'homeEnPopupMessage = homeEnPopupMessage & "<li>$10,000 for the expense incurred to recover your identity if it is stolen.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>Coverage for direct physical loss to personal property items up to $500.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>Including coverage for personal injury liability coverage.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "<li>Policies with loss payments $500 or less are not subject to loss surcharges.</li>"
                                homeEnPopupMessage = homeEnPopupMessage & "</ul></div>"
                            End If
                            Using popupHome As New PopupMessageClass.PopupMessageObject(Me.Page, homeEnPopupMessage, homeEnPopupTitle)
                                With popupHome
                                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                                    .isModal = False
                                    .AddButton("OK", True)
                                    .width = 500
                                    .height = 375
                                    .AddControlToBindTo(lnkHelp)
                                    .divId = "homePopup"
                                    .CreateDynamicPopUpWindow()
                                End With
                            End Using
                            'usedPopupIds.Add(Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv("Homeowner Enhancement Endorsement Note:", homeEnPopupMessage, 500, 375, False, True, True, lnkHelp.ClientID, Nothing, usedPopupIds))

                        Else
                            ' if selected you must have sew and drain
                            ' if not selected you can not have sew and drain
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                    "if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();}", True) ' $('#' + checkBoxId_backup).removeAttr('disabled');
                        End If

                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains
                        'Updated 5/13/2022 for task 74106 MLW, Updated 6/13/2022 for task 74187 MLW
                        '' set a variable to the checkboxID
                        'Me.VRScript.AddVariableLine("var checkBoxId_backup = '{0}';".FormatIFM(Me.chkCov.ClientID))
                        'Me.VRScript.AddVariableLine("var divDetails_backup = '{0}';".FormatIFM(Me.divDetails.ClientID))
                        'If AssociatedSectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 AndAlso HOM_General.HPEEWaterBUEnabled() AndAlso Me.Quote.EffectiveDate >= HOM_General.HPEEWaterBUEffDate() then
                        If AssociatedSectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 AndAlso HOM_General.OkayForHPEEWaterBU(Quote) then
                            Me.VRScript.AddVariableLine("var checkBoxId_HPEEWaterBackup = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var divDetails_HPEEWaterBackup = '{0}';".FormatIFM(Me.divDetails.ClientID))
                            Me.VRScript.AddVariableLine("var txtIncludedLimit_HPEEWaterBackup = '{0}';".FormatIFM(Me.txtIncludedLimit.ClientID))
                            Me.VRScript.AddVariableLine("var ddIncreasedLimit_HPEEWaterBackup = '{0}';".FormatIFM(Me.ddIncreasedLimit.ClientID))
                            Me.VRScript.AddVariableLine("var txtTotalLimit_HPEEWaterBackup = '{0}';".FormatIFM(Me.txtTotalLimit.ClientID))
                            Me.VRScript.AddVariableLine("var trBatteryBackupText_HPEEWaterBackup = '{0}';".FormatIFM(Me.trBatteryBackupText.ClientID))
                            VRScript.CreateJSBinding(Me.ddIncreasedLimit.ClientID, ctlPageStartupScript.JsEventType.onchange, "ToggleHPEELimits('WaterBackup');ShowBatteryBackupMsg('HPEE');", True)
                        Else
                            ' set a variable to the checkboxID
                            Me.VRScript.AddVariableLine("var checkBoxId_backup = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var divDetails_backup = '{0}';".FormatIFM(Me.divDetails.ClientID))
                            Me.VRScript.AddVariableLine("var ddIncreasedLimit_backup = '{0}';".FormatIFM(Me.ddIncreasedLimit.ClientID))
                            Me.VRScript.AddVariableLine("var trBatteryBackupText_backup = '{0}';".FormatIFM(Me.trBatteryBackupText.ClientID))
                            VRScript.CreateJSBinding(Me.ddIncreasedLimit.ClientID, ctlPageStartupScript.JsEventType.onchange, "ShowBatteryBackupMsg('HEE');", True)
                        End If

                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement,
                            QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020
                        Me.VRScript.AddVariableLine("var checkBoxId_plusenhancement = '{0}';".FormatIFM(Me.chkCov.ClientID))

                        'Added 1/16/18 for HOM Upgrade MLW

                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            ' if selected you must have water damage, cannot have homeowner enhancement and water backup, cannot have identity fraud
                            ' if not selected you can not have water backup, can have homeowners plus or identity fraud
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_PersonalInjury).prop('checked',true); $('#' + checkBoxId_PersonalInjury).attr('disabled',true);}", True)
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange, "HandlePersonalInjuryCheckbox('" & chkCov.ClientID & "');")

                            Dim PreCyber As String = "0"
                            If CDate(Quote.EffectiveDate) < HomCyberEffDate Then PreCyber = "1"
                            Dim SeasonalOrSecondary As String = "0"
                            If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then SeasonalOrSecondary = "1"
                            'Added 5/2/2022 for task 74106 MLW, Updated 6/13/2022 for task 74187 MLW
                            Dim PreHPEEWaterBackup As String = "0"
                            If (CDate(Quote.EffectiveDate) < HOM_General.HPEEWaterBUEffDate() OrElse HOM_General.HPEEWaterBackupValidForEndorsements(Quote.QuoteTransactionType, QQHelper.IntegerForString(Quote.RatingVersionId)) = False) Then PreHPEEWaterBackup = "1"
                            'Updated 5/2/2022 for task 74106 MLW
                            'VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange, "HandleHOPLusCheckboxClicks('" & chkCov.ClientID & "','" & PreCyber & "','" & SeasonalOrSecondary & "');", True)
                            VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange, "HandleHOPLusCheckboxClicks('" & chkCov.ClientID & "','" & PreCyber & "','" & SeasonalOrSecondary & "','" & HOM_General.HPEEWaterBUEnabled() & "','" & PreHPEEWaterBackup & "');", True)


                            ' REPLACED THIS NEXT BLOCK OF CODE WITH THE ABOVE FUNCTION CALL
                            'If (CDate(Quote.EffectiveDate) >= HomCyberEffDate AndAlso Quote.Locations(0).FormTypeId <> "25") Then
                            '    ' All quotes after cyber eff date cannot have identity fraud unless HO-4
                            '    If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then
                            '        ' When seasonal or secondary we need to remove the Homeowners Plus and water damage coverages from the script, it's not available
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                            "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_enhancement).prop('checked',false);$('#' + checkBoxId_enhancement).attr('disabled',true);$('#' + checkBoxId_backup).prop('checked',false);} else {$('#' + checkBoxId_enhancement).attr('disabled',false);}", True)
                            '    Else
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                            "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_enhancement).prop('checked',false);$('#' + checkBoxId_enhancement).attr('disabled',true);$('#' + checkBoxId_backup).prop('checked',false);$('#' + checkBoxId_waterDamage).prop('checked',true); $('#' + checkBoxId_waterDamage).change(); $('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).show();} else {$('#' + checkBoxId_enhancement).attr('disabled',false);$('#' + checkBoxId_waterDamage).prop('checked',false); $('#' + checkBoxId_waterDamage).change();$('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).hide();}", True)
                            '    End If
                            'Else
                            '    ' HO-4 or eff date is pre-cyber
                            '    ' Include identity fraud
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            'End If
                            ' END OF REPLACED CODE


                            'If (CDate(Quote.EffectiveDate) >= HomCyberEffDate OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5")) AndAlso (Quote.Locations(0).StructureTypeId <> "2" AndAlso Quote.Locations(0).FormTypeId <> "25") Then
                            '    ' After the cyber eff date cutoff we removed identity fraud from the script.
                            '    ' Also applies for seasonal or secondary occupancy as these are not eligible for identity fraud
                            '    If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then
                            '        ' When seasonal or secondary we need to remove the Homeowners Plus and water damage coverages from the script, it's not available
                            '        ' The enhancement variable may not be set at this point, check it
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                            "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_enhancement).prop('checked',false);$('#' + checkBoxId_enhancement).attr('disabled',true);$('#' + checkBoxId_backup).prop('checked',false);} else {$('#' + checkBoxId_enhancement).attr('disabled',false);}", True)
                            '    Else
                            '        Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                            "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_enhancement).prop('checked',false);$('#' + checkBoxId_enhancement).attr('disabled',true);$('#' + checkBoxId_backup).prop('checked',false);$('#' + checkBoxId_waterDamage).prop('checked',true); $('#' + checkBoxId_waterDamage).change(); $('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).show();} else {$('#' + checkBoxId_enhancement).attr('disabled',false);$('#' + checkBoxId_waterDamage).prop('checked',false); $('#' + checkBoxId_waterDamage).change();$('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).hide();if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            '    End If
                            'Else
                            '    ' Include identity fraud
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_plusenhancement).prop('checked',false);$('#' + checkBoxId_plusenhancement).attr('disabled',true);$('#' + checkBoxId_waterDamage).prop('checked',false);$('#' + checkBoxId_backup).prop('checked',true); $('#' + checkBoxId_backup).change(); $('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).show();} else {$('#' + checkBoxId_plusenhancement).attr('disabled',false);$('#' + checkBoxId_backup).prop('checked',false); $('#' + checkBoxId_backup).change();$('#' + checkBoxId_backup).attr('disabled',true);$('#' + divDetails_backup).hide();if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            'End If

                            'If CDate(Quote.EffectiveDate) < HomCyberEffDate OrElse (Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso (Quote.Locations(0).FormTypeId = "25" OrElse Quote.Locations(0).StructureTypeId = "2" OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5"))) Then
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);$('#' + checkBoxId_enhancement).prop('checked',false);$('#' + checkBoxId_enhancement).attr('disabled',true);$('#' + checkBoxId_backup).prop('checked',false);$('#' + checkBoxId_waterDamage).prop('checked',true); $('#' + checkBoxId_waterDamage).change(); $('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).show();} else {$('#' + checkBoxId_enhancement).attr('disabled',false);$('#' + checkBoxId_waterDamage).prop('checked',false); $('#' + checkBoxId_waterDamage).change();$('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).hide();if($('#' + checkBoxId_enhancement).is(':checked')) {$('#' + checkBoxId_identityFraud).prop('checked',false);$('#' + checkBoxId_identityFraud).attr('disabled',true);}else{$('#' + checkBoxId_identityFraud).attr('disabled',false);}}", True)
                            'Else
                            '    ' After the cyber eff date cutoff we removed identity fraud from the script.
                            '    Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                            '                        "if($('#' + checkBoxId_plusenhancement).is(':checked')) {$('#' + checkBoxId_enhancement).prop('checked',false);$('#' + checkBoxId_enhancement).attr('disabled',true);$('#' + checkBoxId_backup).prop('checked',false);$('#' + checkBoxId_waterDamage).prop('checked',true); $('#' + checkBoxId_waterDamage).change(); $('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).show();} else {$('#' + checkBoxId_enhancement).attr('disabled',false);$('#' + checkBoxId_waterDamage).prop('checked',false); $('#' + checkBoxId_waterDamage).change();$('#' + checkBoxId_waterDamage).attr('disabled',true);$('#' + divDetails_waterDamage).hide();}", True)
                            'End If

                            Dim homePlusPopupMessage As String = ""
                            Dim homePlusPopupTitle As String = ""

                            If CDate(Quote.EffectiveDate) < HomCyberEffDate OrElse (Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso (Quote.Locations(0).FormTypeId = "25" OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5"))) Then
                                ' Old HO+ (1017)
                                homePlusPopupTitle = "Homeowners Plus Enhancement Endorsement Note:"
                                homePlusPopupMessage = "<br /><div><b>The Homeowners Plus Enhancement provides additional valuable coverages such as:</b></div><div><ul>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>The peril of Landslide is included for coverages A, B and C.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for spoilage of refrigerated or frozen food.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$100 for expenses incurred for the accidental activation of a premises alarm system.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 to replace locks on exterior doors and to replace portable garage door transmitters if any of these are lost or stolen.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for additional living expenses if your home becomes uninhabitable due to an off-premises power outage.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$100 for coverage of any domestic animal.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$5,000 for damage caused by the peril of water. For an added charge, this amount can be increased.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$15,000 for the expense incurred to recover your identity if it is stolen.</li>"  ' MGB 6/18/20 Task 49456
                                'homePlusPopupMessage = homePlusPopupMessage & "<li>$10,000 for the expense incurred to recover your identity if it is stolen.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>Coverage for direct physical loss to personal property items up to $500.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for loss including theft or misplacing of a cell phone.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for loss including theft or misplacing of non-owned laptops or portable electronic devices.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>Including coverage for personal injury liability coverage.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>Policies with loss payments $500 or less are not subject to loss surcharges.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "</ul></div>"
                            Else
                                ' New HO+ (1020)
                                homePlusPopupTitle = "Home Plus Enhancement Endorsement Note:"
                                homePlusPopupMessage = "<br /><div><b>The Home Plus Enhancement provides additional valuable coverages such as:</b></div><div><ul>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>The peril of Landslide is included for coverages A, B and C.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for spoilage of refrigerated or frozen food.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$100 for expenses incurred for the accidental activation of a premises alarm system.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 to replace locks on exterior doors and to replace portable garage door transmitters if any of these are lost or stolen.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for additional living expenses if your home becomes uninhabitable due to an off-premises power outage.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$100 for coverage of any domestic animal.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$5,000 for damage caused by the peril of water. For an added charge, this amount can be increased.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>Coverage for direct physical loss to personal property items up to $500.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for loss including theft or misplacing of a cell phone.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>$500 for loss including theft or misplacing of non-owned laptops or portable electronic devices.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>Including coverage for personal injury liability coverage.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "<li>Policies with loss payments $500 or less are not subject to loss surcharges.</li>"
                                homePlusPopupMessage = homePlusPopupMessage & "</ul></div>"

                            End If

                            Using popupPlus As New PopupMessageClass.PopupMessageObject(Me.Page, homePlusPopupMessage, homePlusPopupTitle)
                                With popupPlus
                                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                                    .isModal = False
                                    .AddButton("OK", True)
                                    .width = 525
                                    .height = 425
                                    .AddControlToBindTo(lnkHelp)
                                    .divId = "homePlusPopup"
                                    .CreateDynamicPopUpWindow()
                                End With
                            End Using
                            'usedPopupIds.Add(Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv("Homeowners Plus Enhancement Endorsement Note:", homePlusPopupMessage, 500, 425, False, True, True, lnkHelp.ClientID, Nothing, usedPopupIds))                       
                        End If

                        'B43874 Coverage not for Seasonal or Secondary Occ. CAH
                        If Me.MyLocation.OccupancyCodeId = "4" Or Me.MyLocation.OccupancyCodeId = "5" Then
                            Me.Visible = False
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage
                        'Added 1/16/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            ' set a variable to the checkboxID
                            Me.VRScript.AddVariableLine("var checkBoxId_waterDamage = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var divDetails_waterDamage = '{0}';".FormatIFM(Me.divDetails.ClientID))
                            'Me.VRScript.AddVariableLine("var trBatteryBackupText_waterDamage = '{0}';".FormatIFM(Me.trBatteryBackupText.ClientID))
                            'Added 5/2/2022 for task 74106 MLW, Updated 6/13/2022 for task 74187 MLW
                            'If AssociatedSectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 AndAlso HOM_General.HPEEWaterBUEnabled() AndAlso Me.Quote.EffectiveDate >= HOM_General.HPEEWaterBUEffDate() then
                            If AssociatedSectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 AndAlso HOM_General.OkayForHPEEWaterBU(Quote) then
                                Me.VRScript.AddVariableLine("var txtIncludedLimit_waterDamage = '{0}';".FormatIFM(Me.txtIncludedLimit.ClientID))
                                Me.VRScript.AddVariableLine("var ddIncreasedLimit_waterDamage = '{0}';".FormatIFM(Me.ddIncreasedLimit.ClientID))
                                VRScript.CreateJSBinding(Me.ddIncreasedLimit.ClientID, ctlPageStartupScript.JsEventType.onchange, "ToggleHPEELimits('WaterDamage');", True)
                                Me.VRScript.AddVariableLine("var txtTotalLimit_waterDamage = '{0}';".FormatIFM(Me.txtTotalLimit.ClientID))
                            End If
                        End If
                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                        'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                        'Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                        'Added 1/2/18 for HOM Upgrade
                        'Used to check Other Structures on the Residence Premises if MineAB Checked
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.VRScript.AddVariableLine("var checkBoxId_OtherOn = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var divDetails_OtherOn = '{0}';".FormatIFM(Me.divDetails.ClientID))
                            Me.VRScript.AddVariableLine("var checkBoxId_OtherOnHidden = '{0}';".FormatIFM(Me.hndhomOptionalCovChk.ClientID))
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.IdentityFraudExpenseHOM0455
                        'Added 1/17/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            ' set a variable to the checkboxID
                            Me.VRScript.AddVariableLine("var checkBoxId_identityFraud = '{0}';".FormatIFM(Me.chkCov.ClientID))
                        End If

                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA
                        'Not available with QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement selected
                        Me.VRScript.AddVariableLine("var checkBoxId_MineA = '{0}';".FormatIFM(Me.chkCov.ClientID)) 'used to make sure you don't have ACV with this coverage
                        'Updated 12/27/17 to not use for form HO-6 since it causes the coverage page to not load - MLW
                        If CurrentFormTypeId <> "5" Then
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                    "if($('#' + checkBoxId_MineA).is(':checked')){ $('#' + checkBoxId_MineAB).attr('disabled',true);} else { $('#' + checkBoxId_MineAB).removeAttr('disabled'); }", True)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB
                        ' not available with SectionICoverageType.Home_CoverageASpecialCoverage selected
                        'Updated 1/2/18 for HOM Upgrade MLW - HO 0002 Mobile, HO 0004 home and mobile do not have Mine A. Need to not bind this for these three so coverage page displays without errors/explosions
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso ((Quote.Locations(0).FormTypeId = "22" AndAlso Quote.Locations(0).StructureTypeId = "2") OrElse Quote.Locations(0).FormTypeId = "25")) Then
                            'do not bind MineA js to HO 0002 Mobile and HO 0004 home and mobile - doing so causes coverages to disappear. MLW
                            If Quote.Locations(0).FormTypeId = "25" Then
                                Me.VRScript.AddVariableLine("var checkBoxId_MineAB = '{0}';".FormatIFM(Me.chkCov.ClientID)) ' used to make sure you don't have Cov A with this coverage
                                Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                            "if($('#' + checkBoxId_MineAB).is(':checked')) {$('#' + checkBoxId_OtherOnHidden).val('1');$('#' + checkBoxId_OtherOn).attr('disabled',true);if($('#' + checkBoxId_OtherOn).is(':checked')) {$('#' + divDetails_OtherOn).show();}else{$('#' + checkBoxId_OtherOn).prop('checked',true);$('#' + divDetails_OtherOn).show();alert('Other Structures on the Residence Premises (HO 0448) is required in conjunction with Mine Subsidence Cov A&B (HO 1009). The coverage has also been added.');}} else {$('#' + checkBoxId_OtherOnHidden).val('0');$('#' + checkBoxId_OtherOn).removeAttr('disabled');$('#' + checkBoxId_OtherOn).prop('checked',false);$('#' + divDetails_OtherOn).hide();}", False)
                                Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                            "if($('#' + checkBoxId_MineAB).is(':checked')) {$('#' + checkBoxId_OtherOn).attr('disabled',true);$('#' + checkBoxId_OtherOn).prop('checked',true);$('#' + divDetails_OtherOn).show();} else {}", True)
                            End If
                        Else
                            Me.VRScript.AddVariableLine("var checkBoxId_MineAB = '{0}';".FormatIFM(Me.chkCov.ClientID)) ' used to make sure you don't have Cov A with this coverage
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                    "if($('#' + checkBoxId_MineAB).is(':checked')) {$('#' + checkBoxId_MineA).attr('disabled',true);} else {$('#' + checkBoxId_MineA).removeAttr('disabled');}", True)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities
                        'Added 1/19/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.VRScript.AddVariableLine("var checkBoxId_PersPropSelfStor = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_IncludedLimit = '{0}';".FormatIFM(Me.txtIncludedLimit.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_IncreaseLimit = '{0}';".FormatIFM(Me.txtIncreaseLimit.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_TotalLimit = '{0}';".FormatIFM(Me.txtTotalLimit.ClientID))
                            'updated 4/10/18 for Bugs 26085 and 26086 MLW
                            Dim scriptPPSSLimit As String = "var PPLimit = $('[id*=txtPPTotalLimit]').val();PPLimit = ifm.vr.stringFormating.asNumberNoCommas(PPLimit);var PPLimit10 = PPLimit * .1;"
                            scriptPPSSLimit = scriptPPSSLimit + "if(PPLimit10 > 1000) { PPLimit10 = PPLimit10.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","");$('#' + txtId_IncludedLimit).val(PPLimit10);$('#' + txtId_TotalLimit).val((PPLimit10.toFloat()+$('#' + txtId_IncreaseLimit).val().toFloat()).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","")); } else { $('#' + txtId_IncludedLimit).val('1,000');var PPLimitBase='1000';$('#' + txtId_TotalLimit).val((PPLimitBase.toFloat()+$('#' + txtId_IncreaseLimit).val().toFloat()).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","")); }"
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                "if($('#{0}').is(':checked') == true){{ {1} }}".FormatIFM(Me.chkCov.ClientID, scriptPPSSLimit), True)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            'Me.VRScript.AddVariableLine("var checkBoxId_PersPropOtherRes = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_IncludedLimit_PersPropOtherRes = '{0}';".FormatIFM(Me.txtIncludedLimit.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_IncreaseLimit_PersPropOtherRes = '{0}';".FormatIFM(Me.txtIncreaseLimit.ClientID))
                            Me.VRScript.AddVariableLine("var txtId_TotalLimit_PersPropOtherRes = '{0}';".FormatIFM(Me.txtTotalLimit.ClientID))
                            Dim scriptPPORLimit As String = "var PPLimit = $('[id*=txtPPTotalLimit]').val();PPLimit = ifm.vr.stringFormating.asNumberNoCommas(PPLimit);var PPLimit10 = PPLimit * .1;"
                            scriptPPORLimit &= "if(PPLimit10 > 1000) { PPLimit10 = PPLimit10.toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","");$('#' + txtId_IncludedLimit_PersPropOtherRes).val(PPLimit10);$('#' + txtId_TotalLimit_PersPropOtherRes).val((PPLimit10.toFloat()+$('#' + txtId_IncreaseLimit_PersPropOtherRes).val().toFloat()).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","")); } else { $('#' + txtId_IncludedLimit_PersPropOtherRes).val('1,000');var PPLimitBase='1000';$('#' + txtId_TotalLimit_PersPropOtherRes).val((PPLimitBase.toFloat()+$('#' + txtId_IncreaseLimit_PersPropOtherRes).val().toFloat()).toString().replace(/\B(?=(\d{3})+(?!\d))/g, "","")); }"
                            Me.VRScript.CreateJSBinding(Me.chkCov.ClientID, ctlPageStartupScript.JsEventType.onchange,
                                "if($('#{0}').is(':checked') == true){{ {1} }}".FormatIFM(Me.chkCov.ClientID, scriptPPORLimit), True)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialPersonalProperty
                        'Not available with SectionICoverageType.SpecialComputerCoverage selected only for HO 0004
                        'Added 1/4/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso (Quote.Locations(0).FormTypeId = "25" AndAlso Quote.Locations(0).StructureTypeId <> "2")) Then
                            Me.VRScript.AddVariableLine("var checkBoxId_SpecPersProp = '{0}';".FormatIFM(Me.chkCov.ClientID)) ' used to make sure you don't have Special Computer Coverage with this coverage
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                        "if($('#' + checkBoxId_SpecPersProp).is(':checked')) {$('#' + checkBoxId_SpecPCCov).attr('disabled',true);$('#' + checkBoxId_SpecPCCov).prop('checked',false);} else {$('#' + checkBoxId_SpecPCCov).removeAttr('disabled');}", True)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage
                        'Not available with SectionICoverageType.SpecialPersonalProperty selected only for HO 0004
                        'Added 1/4/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso (Quote.Locations(0).FormTypeId = "25" AndAlso Quote.Locations(0).StructureTypeId <> "2")) Then
                            Me.VRScript.AddVariableLine("var checkBoxId_SpecPCCov = '{0}';".FormatIFM(Me.chkCov.ClientID)) ' used to make sure you don't have Special Personal Property Coverage with this coverage
                            Me.VRScript.CreateJSBinding(Me.chkCov, ctlPageStartupScript.JsEventType.onchange,
                                                        "if($('#' + checkBoxId_SpecPCCov).is(':checked')) {$('#' + checkBoxId_SpecPersProp).attr('disabled',true);$('#' + checkBoxId_SpecPersProp).prop('checked',false);} else {$('#' + checkBoxId_SpecPersProp).removeAttr('disabled');}", True)
                        End If

                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Dim CyberPopupTitle As String = ""
                            Dim CyberPopupMessage As String = ""

                            'Updated 10/6/2022 for task 51260 MLW
                            'If CDate(Quote.EffectiveDate) < HomCyberEffDate OrElse (Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso (Quote.Locations(0).FormTypeId = "25" OrElse Quote.Locations(0).StructureTypeId = "2" OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5"))) Then
                            If CDate(Quote.EffectiveDate) < HomCyberEffDate OrElse (Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso (Quote.Locations(0).FormTypeId = "25" OrElse (Quote.Locations(0).StructureTypeId = "2" AndAlso Quote.Locations(0).FormTypeId <> "22") OrElse Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5"))) Then
                                'FormTypeId 25 = HO4, FormTypeId 22 = FO2, StructureTypeId 2 = Mobile, OccupancyCodeId 4, 5 = Seasonal, Secondary
                                'do nothing
                            Else
                                ' Popup text for HOM1019 (new enhancement)
                                CyberPopupTitle = "Family Cyber Protection (HOM 1018)  Note:"
                                CyberPopupMessage = "<br /><div><b>The Family Cyber Protection provides valuable coverages such as:</b></div><div><ul>"
                                CyberPopupMessage = CyberPopupMessage & "<li>Online Extortion for expenses and ransom paid for threats to create a home network disruption or a privacy breach.</li>"
                                CyberPopupMessage = CyberPopupMessage & "<li>Social Engineering for reimbursement for direct loss of money that is a result of another person intentionally misleading an insured to willingly give money to an imposter.</li>"
                                CyberPopupMessage = CyberPopupMessage & "<li>Cyber Bullying pays for fees and expenses, such as professional counseling, temporary relocation, tuition expense, and tutoring cost, if you are a victim of cyber bullying.</li>"
                                CyberPopupMessage = CyberPopupMessage & "<li>Identity Theft refunds fees and expenses incurred by an insured when personal information is obtained and used fraudulently. Hels to restore your identity.</li>"
                                CyberPopupMessage = CyberPopupMessage & "<li>System Compromise covers costs to recover data and/or restore your home computer if it is damaged by malware or a hacker.</li>"
                                CyberPopupMessage = CyberPopupMessage & "<li>Internet Clean Up Affords protection for expenses associated with removing false statements about you on the internet.</li>"
                                CyberPopupMessage = CyberPopupMessage & "<li>Breach Costs pays expenses if you lose or unintentional disclose personal information that you have that belongs to others. Assists with the notification, investigation and monitoring when there is a breach.</li>"
                                CyberPopupMessage = CyberPopupMessage & "</ul></div>"
                            End If
                            If FamilyCyberProtectionHelper.IsFamilyCyberProtectionAvailable(Quote) = False Then
                                Using popupHome As New PopupMessageClass.PopupMessageObject(Me.Page, CyberPopupMessage, CyberPopupTitle)
                                    With popupHome
                                        .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                                        .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                                        .isModal = False
                                        .AddButton("OK", True)
                                        .width = 500
                                        .height = 375
                                        .AddControlToBindTo(lnkHelp)
                                        .divId = "cyberPopup"
                                        .CreateDynamicPopUpWindow()
                                    End With
                                End Using
                            End If
                        End If


                End Select

            'Added 7/8/2019 for Home Endorsements Project Task 34542, 38925 MLW
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                Select Case Me.SectionCoverageIIEnum
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                        If Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

                            txtDescription.Attributes.Add("onfocus", "this.select()")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury

                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            ' set a variable to the checkboxID
                            Me.VRScript.AddVariableLine("var checkBoxId_PersonalInjury = '{0}';".FormatIFM(Me.chkCov.ClientID))
                        End If
                End Select

            'Added 1/30/18 for HOM Upgrade MLW
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                Select Case Me.SectionCoverageIAndIIEnum
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                        'CoverageName = "Assisted Living Care Coverage (HO 0459)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment
                        Me.VRScript.AddVariableLine("var checkBoxId_LossAssessment = '{0}';".FormatIFM(Me.chkCov.ClientID))
                        Me.VRScript.AddVariableLine("var divDetails_LossAssessment = '{0}';".FormatIFM(Me.divDetails.ClientID))
                        Me.VRScript.AddScriptLine("$('#' + checkBoxId_LossAssessment).attr('disabled',true);$('#' + checkBoxId_LossAssessment).prop('checked',true);$('#' + divDetails_LossAssessment).show();")
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Dim OtherMembersPopupMessage As String = "<br /><div>This coverage provides insured status to others (over 21 years old) in your household that "
                            OtherMembersPopupMessage = OtherMembersPopupMessage & "are not tenants or not included in the definition of insured, but this is their permanent residence.</div><div><ul>"
                            'usedPopupIds.Add(Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv("Other Members of Your Household Note:", OtherMembersPopupMessage, 325, 175, False, True, True, lnkHelp.ClientID, Nothing, usedPopupIds))
                            Using popupOther As New PopupMessageClass.PopupMessageObject(Me.Page, OtherMembersPopupMessage, "Other Members of Your Household Note:")
                                With popupOther
                                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onmouseup
                                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                                    .isModal = False
                                    .AddButton("OK", True)
                                    .width = 325
                                    .height = 175
                                    .AddControlToBindTo(lnkHelp)
                                    .divId = "otherPopup"
                                    .CreateDynamicPopUpWindow()
                                End With
                            End Using
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                        'Added 7/8/2019 for Home Endorsements Project Task 38915, 38925 MLW
                        If Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                            Dim scriptDescCount As String = "CountDescLength(""" + txtDescription.ClientID + """, """ + lblMaxCharCount.ClientID + """, """ + hiddenMaxCharCount.ClientID + """);"
                            VRScript.CreateJSBinding(Me.txtDescription.ClientID, ctlPageStartupScript.JsEventType.onkeyup, scriptDescCount, True)

                            'txtDescription.Attributes.Add("onfocus", "this.Select()")
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                        If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.Quote.Locations(0).FormTypeId = "26" AndAlso Me.MyLocation.OccupancyCodeId = "9" Then
                            Me.VRScript.AddVariableLine("var checkBoxId_UnitOwnersRentalToOthers = '{0}';".FormatIFM(Me.chkCov.ClientID))
                            Me.VRScript.AddScriptLine("$('#' + checkBoxId_UnitOwnersRentalToOthers).attr('disabled',true);$('#' + checkBoxId_UnitOwnersRentalToOthers).prop('checked',true);")
                        End If
                End Select
        End Select

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub CustomizeByCoverageType()

        '#If DEBUG Then
        '        If CoverageName = "Unknown" Then
        '            Debugger.Break()
        '        End If
        '#End If
        Me.lblHeader.Text = CoverageName

        Dim CurrentForm As String = QQHelper.GetShortFormName(Quote)


        ' HERE YOU SET INCLUDED LIMITS TEXTBOXES, DROPDOWN ITEMS(STATIC DATA)

        Select Case Me.SectionType
            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                Select Case Me.SectionCoverageIEnum

                        '******************************    Included Coverages   *******************************
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased
                        'CoverageName = "Business Property Increased Limits (HO-312)"
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim attribute As New QuickQuoteStaticDataAttribute
                        With attribute
                            .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                            .nvp_value = "20098"
                        End With
                        optionAttributes.Add(attribute)
                        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms
                        'CoverageName = "Firearms (HO-65 / HO-221)"
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs
                        'CoverageName = "Jewelry, Watches & Furs (HO-61)"
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money
                        'CoverageName = "Money (HO-65 / HO-221)"
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities
                        'CoverageName = "Securities (HO-65 / HO-221)"
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware
                        'CoverageName = "Silverware, Goldware, Pewterware (HO-61)"
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                            '******************************    END  Included Coverages   *******************************



                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage
                        'CoverageName = "Equipment Breakdown Coverage (92-132)" -Old forms
                        'CoverageName = "Equipment Breakdown Coverage (HOM 1011)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                        'CoverageName = "Personal Property Replacement Cost  (HO-290 / 92-195)" IF ML "Personal Property Replacement Cost  (ML-55)" - Old forms
                        'CoverageName = "Personal Property Replacement Cost (HO 0490)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019
                        'If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso (Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") AndAlso Quote.Locations(0).StructureTypeId <> "2") AndAlso (Not Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5"))) Then
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso (Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") AndAlso Quote.Locations(0).StructureTypeId <> "2")) Then
                            Me.lnkHelp.CssClass = "clickableLink"
                        Else
                            Me.lnkHelp.NavigateUrl = ResolveUrl("~/vrHelpMe.aspx?p=VR3Home&s=hee")
                        End If
                        Me.lnkHelp.Text = CoverageName
                        Me.lblHeader.Text = ""
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement
                        'CoverageName = "Homeowner Enhancement Endorsement  (92-267)" - Old forms
                        'CoverageName = "Homeowner Enhancement Endorsement (HOM 1010)" - New forms
                        'Updated 12/22/17 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.lnkHelp.CssClass = "clickableLink"
                        Else
                            Me.lnkHelp.NavigateUrl = ResolveUrl("~/vrHelpMe.aspx?p=VR3Home&s=hee")
                        End If
                        Me.lnkHelp.Text = CoverageName
                        Me.lblHeader.Text = ""
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains
                        'CoverageName = "Backup Of Sewer Or Drain (92-173)" - Old forms
                        'CoverageName = "Water Backup" (N/A)" - New forms
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                        'Updated 5/6/2022 for task 74106 MLW, Updated 6/13/2022 for task 74187 MLW
                        'Me.chkCov.ToolTip = "Only available with Home Owners Enhancement"
                        'If HOM_General.HPEEWaterBUEnabled() AndAlso quote.EffectiveDate >= HOM_General.HPEEWaterBUEffDate() Then
                        If HOM_General.OkayForHPEEWaterBU(Quote) Then
                            If AssociatedSectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 
                                Me.lblHeader.ToolTip = "Water Damage and Water Backup must have the same limit up to $20,000. Only Water Backup may then be increased over $20,000."
                            End If
                        Else
                            Me.chkCov.ToolTip = "Only available with Home Owners Enhancement"
                        End If

                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim attribute As New QuickQuoteStaticDataAttribute
                        With attribute
                            .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                            .nvp_value = "144"
                        End With
                        optionAttributes.Add(attribute)
                        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso (Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") AndAlso Quote.Locations(0).StructureTypeId <> "2" AndAlso Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") = False)) Then
                            Me.lnkHelp.CssClass = "clickableLink"
                            Me.lnkHelp.Text = CoverageName
                            Me.lblHeader.Text = ""
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement 'Added 1/16/18 for HOM Upgrade MLW
                        'CoverageName = "Homeowners Plus Enhancement Endorsement (HOM 1017)" - New forms
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.lnkHelp.CssClass = "clickableLink"
                            Me.lnkHelp.Text = CoverageName
                            Me.lblHeader.Text = ""
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage 'Added 1/16/18 for HOM Upgrade MLW
                        'CoverageName = "Water Damage" (N/A)" - New forms
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.txtIncludedLimit.Text = Me.IncludedLimit
                            'Updated 4/29/2022 for task 74106 MLW, Updated 6/13/2022 for task 74187 MLW
                            'Me.chkCov.ToolTip = "Only available with Home Owners Plus Enhancement"
                            'If HOM_General.HPEEWaterBUEnabled() AndAlso quote.EffectiveDate >= HOM_General.HPEEWaterBUEffDate() Then
                            If HOM_General.OkayForHPEEWaterBU(Quote) Then
                                If AssociatedSectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 
                                    Me.lblHeader.ToolTip = "Water Damage and Water Backup must have the same limit up to $20,000. Only Water Backup may then be increased over $20,000."
                                End If
                            Else
                                Me.chkCov.ToolTip = "Only available with Home Owners Plus Enhancement"
                            End If

                            Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                            Dim attribute As New QuickQuoteStaticDataAttribute
                            With attribute
                                .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                                .nvp_value = "80520"
                            End With
                            optionAttributes.Add(attribute)
                            QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                        End If
                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage
                        'Not available with QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement selected
                        'CoverageName = "Cov.A - Specified Additional Amount Of Insurance (29-034)" - Old forms
                        'CoverageName = "Cov.A - Specified Additional Amount Of Insurance" (HO 0420)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake
                        'CoverageName = "Earthquake  (HO-315B)" if ML "Earthquake  (ML-54)" - Old forms
                        'CoverageName = "Earthquake (HOM 1014)" - New forms

                        'Updated 12/21/17 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            QQHelper.LoadStaticDataOptionsDropDown(Me.ddDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId)
                        Else
                            'Updated 11/29/17 for HOM Upgrade MLW
                            'If Me.CurrentFormTypeId.EqualsAny(ML2, ML4) Then
                            If CurrentForm = "ML-2" Or CurrentForm = "ML-4" Then
                                Me.ddDeductible.Items.Clear()
                                Dim TwoPercentId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, "2%", Me.Quote.LobType)
                                Me.ddDeductible.Items.Add(New ListItem("2%", TwoPercentId))
                            Else
                                QQHelper.LoadStaticDataOptionsDropDown(Me.ddDeductible, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId)
                            End If
                        End If


                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement
                        ' not available with SectionICoverageType.Home_CoverageASpecialCoverage selected
                        'CoverageName = "Actual Cash Value Loss Settlement (HO-04 81)" - Old forms
                        'CoverageName = "Actual Cash Value Loss Settlement (HO 0481)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                        'CoverageName = "Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing (HO-04 93)" - Old forms
                        'CoverageName = "Actual Cash Value Loss Settlement/Windstorm Or Hail Losses To Roof Surfacing (HO 1013)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse
                        'CoverageName = "Sinkhole Collapse (HO-99)" - Old forms
                        'CoverageName = "Sinkhole Collapse (HO 0499)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc
                        'CoverageName = "Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage (HO-53)" - Old forms
                        'CoverageName = Credit Card, Fund Transfer Card, Forgery And Counterfeit Money Coverage" (HO 0453)"" - New forms
                        'Me.chkCov.Checked = True

                        'Updated 12/26/17 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.chkCov.Enabled = True
                        Else
                            Me.chkCov.Enabled = False
                        End If

                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim attribute As New QuickQuoteStaticDataAttribute
                        With attribute
                            .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                            .nvp_value = "20126"
                        End With
                        optionAttributes.Add(attribute)
                        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)


                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment
                        'CoverageName = "Functional Replacement Cost Loss Settlement (HO-05 30)" - Old forms
                        'CoverageName = "Functional Replacement Cost Loss Settlement (HO 0530)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations
                        'CoverageName = "Building Additions and Alterations (HO-51)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge
                        'CoverageName = "Fire Department Service Charge (ML-306)"
                        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.txtIncludedLimit.Text = Me.IncludedLimit
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades
                        'Added 1/25/18 for HOM Upgrade MLW
                        'CoverageName = "Green Upgrades (HOM 0631)" - New forms
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.lnkHelp.CssClass = "clickableLink"
                            Me.lnkHelp.Text = CoverageName
                            Me.lblHeader.Text = ""

                            QQHelper.LoadStaticDataOptionsDropDown(Me.ddIncreasedCostOfLoss, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedCostOfLossId)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment
                        'CoverageName = "Loss Assessment (HO-35)" - Old forms
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            'moved to section I & II
                        Else
                            Me.txtIncludedLimit.Text = Me.IncludedLimit
                            Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                            Dim attribute As New QuickQuoteStaticDataAttribute
                            With attribute
                                .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                                .nvp_value = "70259"
                            End With
                            optionAttributes.Add(attribute)
                            QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                        End If

                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake
                        'CoverageName = "Loss Assessment - Earthquake (HO-35B)" - Old forms
                        'CoverageName = "Loss Assessment - Earthquake (HO 0436)" - New forms
                        'removed 3/8/18 for HOM Upgrade MLW
                        ''added 12/26/17 for HOM Upgrade MLW
                        'If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        '    Me.txtIncludedLimit.Text = Me.IncludedLimit
                        '    Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        '    Dim attribute As New QuickQuoteStaticDataAttribute
                        '    With attribute
                        '        .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                        '        .nvp_value = "70260"
                        '    End With
                        '    optionAttributes.Add(attribute)
                        '    QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                        'End If

                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA
                        'CoverageName = "Mine Subsidence Cov A (92-074)" - Old forms
                        'CoverageName = ""Mine Subsidence Cov A (HOM 1009)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB
                        'CoverageName = "Mine Subsidence Cov A & B (92-074)" - Old forms
                        'CoverageName = "Mine Subsidence Cov A & B (HOM 1009)" - New forms
                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                        'CoverageName = "Specified Other Structures - On Premises (92-049)" - Old forms
                        'CoverageName = "Other Structures On the Residence Premises (HO 0448)" - New forms
                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                        'CoverageName = "Other Structures On the Residence Premises (HO 0448)" - New forms
                        'Replaces Cov_B_Related_Private_Structures 
                        'Update 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                    Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                        'CoverageName = "Specified Other Structures - Off Premises (92-147)" - Old forms
                        'CoverageName = "Specific Structures Away from Residence Premises (HO 0492)"" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial
                        'CoverageName = "Theft of Building Materials (92-367)" - Old forms
                        'CoverageName = "Theft Of Building Materials (HOM 1002)" - New forms
                    'Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine 'Added 1/9/18 for HOM Upgrade MLW
                    '    'CoverageName = "Underground Service Line Coverage (HO 1016)" - New forms only. Not valid on old forms.
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Consent_to_Move_Mobile_Home
                        'CoverageName = "Consent to Move Mobile Home (ML-25)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval
                        'CoverageName = "Debris Removal (92-267)"
                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim attribute As New QuickQuoteStaticDataAttribute
                        With attribute
                            .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                            .nvp_value = "40116"
                        End With
                        optionAttributes.Add(attribute)
                        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)


                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles
                        'CoverageName = "Increased Limits Motorized Vehicles (ML-65)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw
                        'CoverageName = "Ordinance or Law (HOM 1000)"
                        'Case Outdoor Antennas (ML-49)
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit
                        'CoverageName = "Personal Property - Other Residence (HO-50)" 'before 2018 home upgrade
                        'CoverageName = "Personal Property at Other Residences (HO 0450)" 'after 2018 home upgrade
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities
                        'CoverageName = "Personal Property Self Storage Facilities (HO 0614)"
                        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.txtIncludedLimit.Text = Me.IncludedLimit
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty
                        'CoverageName = "Refrigerated Food Products (92-267)"
                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim attribute As New QuickQuoteStaticDataAttribute
                        With attribute
                            .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                            .nvp_value = "20127"
                        End With
                        optionAttributes.Add(attribute)
                        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)


                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage
                        'CoverageName = "Special Computer Coverage (HO-314)" - Old forms
                        'CoverageName = "Special Computer Coverage (HO 0414)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TripCollision
                        'CoverageName = "Trip Collision (ML-26)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine
                        'CoverageName = "Underground Service Line (HO 1016)"
                        Me.txtLimit.Text = Me.IncludedLimit 'Added 4/26/18 for Bug 26128 MLW
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageA
                        'CoverageName = "Unit Owners Coverage A - Special Coverage (HO 1732)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageCSpecialCoverage
                        'CoverageName = "Unit Owners Coverage C - Special Coverage (HO 1731)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MobileHomeLienholdersSingleInterest
                        'CoverageName = "Vendor's Single Interest (ML-27)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso (Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") AndAlso Quote.Locations(0).FormTypeId <> "2" AndAlso Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") = False)) Then
                            If FamilyCyberProtectionHelper.IsFamilyCyberProtectionAvailable(Quote) Then
                                Me.lnkHelp.CssClass = "clickableLink"
                                Me.lnkHelp.Text = CoverageName
                                Me.lblHeader.Text = ""
                                Me.lnkHelp.Attributes.Add("target", "_blank")
                                Me.lnkHelp.NavigateUrl = ConfigurationManager.AppSettings("VRHelpDoc_HOMCyberCoverageSummary")
                            Else
                                Me.lnkHelp.CssClass = "clickableLink"
                                Me.lnkHelp.Text = CoverageName
                                Me.lblHeader.Text = ""
                            End If
                        End If
                End Select ' END SECTIONI




            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                Select Case Me.SectionCoverageIIEnum
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType._3Or4FamilyLiability
                        'CoverageName = "3 or 4 Family Liability (HO-74)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                        'CoverageName = "Incidental Farming Personal Liability (HO-72)" - Old forms
                        'CoverageName = "Incidental Farming Personal Liability - On Premises (HO 2472)" - New forms 
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                        'CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres (HO-73)" - Old forms
                        'CoverageName = "Farm Owned and Operated By Insured: 0-100 Acres (HO 2446)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury
                        'CoverageName = "Personal Injury (HO-82)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                        'CoverageName = "Additional Residence - Rented to Others (HO-70)" - Old forms
                        'CoverageName = "Additional Residence - Rented to Others (HO 2470)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                        'CoverageName = "Additional Residence – Occupied by Insured (N/A)" - Old & New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical
                        'CoverageName = "Business Pursuits - Clerical (HO-71)" - Old forms
                        'CoverageName = "Business Pursuits - Clerical (HO 2471)" - New forms
                        'Added 1/25/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            QQHelper.LoadStaticDataOptionsDropDown(Me.ddInsuredSuffixName, QuickQuoteClassName.QuickQuoteName, QuickQuotePropertyName.SuffixName, SortBy.None, Me.Quote.LobType)
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_ExcludingInstallation
                        'CoverageName = "Sales Person Excluding Installation (HO-71)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_IncludingInstallation
                        'CoverageName = "Sales Person Including Installation (HO-71)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment
                        'CoverageName = "Teacher Lab Etc. Excluding Corporal Punishment (HO-71)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment
                        'CoverageName = "Teacher Lab Etc. Including Corporal Punishment (HO-71)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment
                        'CoverageName = "Teacher Other Excluding Corporal Punishment (HO-71)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment
                        'CoverageName = "Teacher Other Including Corporal Punishment (HO-71)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                        'CoverageName = "Farm Owned and Operated By Insured: 160-500 Acres (HO-73)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsuredOver500Acres
                        'CoverageName = "Farm Owned and Operated By Insured: Over 500 Acres (HO-73)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.HomeDayCareLiability
                        'CoverageName = "Home Day Care (HO-323)"
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises
                        'CoverageName = "Permitted Incidental Occupancies - Residence Premises (HO-42)" - Old forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                        'CoverageName = "Permitted Incidental Occupancies Other Residence (HO-43)" - Old forms
                        'CoverageName = "Permitted Incidental Occupancies Other Residence (HO 2443)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage
                        ' SPECIAL EVENT COVERAGE
                        'CoverageName = "Special Event Coverage (92-347)"
                        ' State dropdown
                        If ddlSpecialEventState.Items Is Nothing OrElse ddlSpecialEventState.Items.Count <= 0 Then
                            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlSpecialEventState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
                        End If
                        ' Bind zip code field to lookup script
                        Me.VRScript.AddScriptLine("$(""#" + Me.ddlSpecialEventCity.ClientID + """).hide();")
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.WaterbedCoverage
                        'CoverageName = "Waterbed Liability (HO-85)"
                End Select ' END SECTIONII





            Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                Select Case Me.SectionCoverageIAndIIEnum
                    'Added 2/5/18 for HOM Upgrade MLW
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                        'CoverageName = "Assisted Living Care Coverage (HO 0459)" - New forms
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.txtPropIncludedLimit.Text = "10,000"
                            Me.txtLiabIncludedLimit.Text = "100,000"
                            Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                            Dim attribute As New QuickQuoteStaticDataAttribute
                            With attribute
                                .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                                .nvp_value = "80270" 'this is cov code id for liability '80269 main cov code id
                            End With
                            optionAttributes.Add(attribute)
                            QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddLiabIncreaseLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                        End If
                    'Added 1/22/18 for HOM Upgrade MLW
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment
                        'CoverageName = "Loss Assessment (HO 0435)" - New forms
                        'old forms still use Section I
                        'added 3/16/18 for new Loss Assessment requirements for HOM 2011 Upgrade MLW
                        Me.txtIncludedLimit.Text = Me.IncludedLimit
                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                        Dim attribute As New QuickQuoteStaticDataAttribute
                        With attribute
                            .nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId
                            .nvp_value = "80524" '80524? 80093 is cov code id for liability '70259 main cov code id
                            '.nvp_value = "70259" '80524? 80093 is cov code id for liability '70259 main cov code id
                        End With
                        optionAttributes.Add(attribute)
                        QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                        'QQHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddIncreasedLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, optionAttributes)
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                        'CoverageName = "Other Members of Your Household (HOM 0458)" - New forms
                        'added 1/31/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            Me.lnkHelp.CssClass = "clickableLink"
                            Me.lnkHelp.Text = CoverageName
                            Me.lblHeader.Text = ""
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                        'CoverageName = "Permitted Incidental Occupancies Residence Premises - Other Structures (HO-42)" - Old forms
                        'CoverageName = "Permitted Incidental Occupancies – Residence Premises (HO 0442)" - New forms
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                        'CoverageName = "Structures Rented To Others (HO-40)" - Old forms
                        'CoverageName = "Structures Rented to Others - Residence Premises (HO 0440)" - New forms

                        'Case QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                        '    CoverageName = "Permitted Incidental Occupancies – Other Residence (HO-43)"
                        '    Me.MyDisplayType = DisplayType.isPermittedIncidentalOccupied43
                End Select ' END SECTIONI&II


        End Select



    End Sub

    Private Sub RenderBasedOndisplayType()
        Me.Visible = True
        Me.divDetails.Visible = True

        Me.trStandard.Visible = False
        Me.trSingleInputs.Visible = False
        Me.trBusinessPursuits.Visible = False 'Added 1/24/18 for HOM Upgrade MLW
        Me.trGreenUpgrades.Visible = False 'Added 1/25/18 for HOM Upgrade MLW
        Me.trBusinessStructure.Visible = False 'Added 1/29/18 for HOM Upgrade MLW

        Me.trStructures.Visible = False
        Me.trFarmLand.Visible = False
        Me.trAdditionalResidenceRenterToOthers.Visible = False
        Me.trMultipleNames.Visible = False 'Added 1/23/18 for HOM Upgrade MLW
        'Me.trLossAssessment.Visible = False 'Added 1/30/18 for HOM Upgrade MLW 'removed 3/16/18 for HOM Upgrade MLW
        Me.trOtherMembers.Visible = False 'Added 1/31/18 for HOM Upgrade MLW
        Me.trAdditionalInterests.Visible = False 'Added 1/31/18 for HOM Upgrade MLW
        Me.divHdnCoverageCode.Visible = False 'Added 2/12/18 for HOM Upgrade MLW
        Me.trTrust.Visible = False 'Added 2/14/18 for HOM Upgrade MLW
        Me.trAdditionalInsured.Visible = False 'Added 4/30/18 for HOM Upgrade Bug 26102 MLW
        trSpecialEventCoverage.Visible = False ' Added 10/25/21
        Me.trLocation.Visible = False

        Select Case Me.MyDisplayType
            Case DisplayType.included, DisplayType.notAvailable
                Me.Visible = False
            Case DisplayType.justCheckBox
                Me.divDetails.Visible = False
            Case DisplayType.hasIncreaseWithFreeForm, DisplayType.hasIncreaseWithLocation
                Me.trStandard.Visible = True
                Me.txtIncludedLimit.Enabled = False
                Me.txtIncreaseLimit.Visible = True
                Me.ddIncreasedLimit.Visible = False
                Me.txtTotalLimit.Enabled = False

                'Added 2/12/18 for HOM Upgrade MLW
                If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities Then
                        Me.divHdnCoverageCode.Visible = True
                        Me.hdnCoverageCode.Value = "80260"
                    End If
                End If

                If Me.MyDisplayType = DisplayType.hasIncreaseWithLocation Then
                    Me.trLocation.Visible = True
                End If

            Case DisplayType.hasIncreasewithDropDown
                Me.txtIncludedLimit.Enabled = False
                Me.trStandard.Visible = True
                Me.ddIncreasedLimit.Visible = True
                Me.txtIncreaseLimit.Visible = False
                Me.txtTotalLimit.Enabled = False
            Case DisplayType.justEffectiveDate
                Me.trSingleInputs.Visible = True
                Me.divEffectiveDate.Visible = True
            Case DisplayType.hasEffectiveAndExpirationDates
                'Added 1/15/18 for HOM Upgrade MLW
                If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trSingleInputs.Visible = True
                    Me.divEffectiveDate.Visible = True
                    Me.divExpirationDate.Visible = True
                    Me.lblEffectiveDate.Text = "*Inception Date"
                    Me.lblExpirationDate.Text = "*Termination Date"
                End If
            Case DisplayType.hasEffAndExpDatesWithLimit
                'Added 1/22/18 for HOM Upgrade MLW
                If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trSingleInputs.Visible = True
                    Me.divEffectiveDate.Visible = True
                    Me.divExpirationDate.Visible = True
                    Me.lblEffectiveDate.Text = "*Begin Date"
                    Me.lblExpirationDate.Text = "*End Date"
                    Me.divLimit.Visible = True
                    lblLimit.Text = "*Limit"
                End If
            Case DisplayType.justDeductible
                Me.trSingleInputs.Visible = True
                Me.divDeductible.Visible = True
            Case DisplayType.justDescription
                Me.trSingleInputs.Visible = True
                Me.divDescription.Visible = True
            Case DisplayType.justLimit
                Me.trSingleInputs.Visible = True
                Me.divLimit.Visible = True
                If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    If (Me.SectionCoverageIEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine) Then
                        lblLimit.Text = "Included Limit"
                        Me.txtLimit.Enabled = False
                    End If
                End If
            Case DisplayType.hasLimitAndDescription
                Me.trSingleInputs.Visible = True
                Me.divLimit.Visible = True
                Me.divDescription.Visible = True
            Case DisplayType.isBusinessPursuits
                'Added 1/24/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trBusinessPursuits.Visible = True
                    Me.divInsuredFirstName.Visible = True
                    Me.divInsuredMiddleName.Visible = True
                    Me.divInsuredLastName.Visible = True
                    Me.divInsuredSuffixName.Visible = True
                    Me.divInsuredBusinessName.Visible = True
                End If
            Case DisplayType.isGreenUpgrades
                'Added 1/25/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trGreenUpgrades.Visible = True
                    Me.divMaximumAmount.Visible = True
                    Me.divIncreasedCostOfLoss.Visible = True
                    Me.divGreenCheck.Visible = True
                    Me.divExpRelCovLimit.Visible = True
                End If
            Case DisplayType.isBusinessStructure
                'Added 1/29/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trBusinessStructure.Visible = True
                    Me.divBusinessDescription.Visible = True
                    Me.divOtherStructures.Visible = True
                    Me.divBuildingDescription.Visible = True
                    Me.divBuildingLimit.Visible = True
                End If




                'These are the odd ones out
            Case DisplayType.isFarmLand
                Me.trFarmLand.Visible = True
            Case DisplayType.isOtherStructures
                Me.trStructures.Visible = True
            Case DisplayType.isAdditionlResidence
                Me.trAdditionalResidenceRenterToOthers.Visible = True
            Case DisplayType.isCanine 'Added 1/23/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trMultipleNames.Visible = True
                End If
                'removed 3/16/18 - Loss Assessment no longer multiple, no longer capturing address MLW
            'Case DisplayType.isLossAssessment
            '    'Added 1/30/18 for HOM Upgrade MLW
            '    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            '        Me.trLossAssessment.Visible = True
            '    End If
            Case DisplayType.isOtherMembers
                'Added 1/31/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trOtherMembers.Visible = True
                End If
            Case DisplayType.isAdditionalInterests
                'Added 1/31/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trAdditionalInterests.Visible = True
                    If (Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence) Then
                        divAILimits.Visible = False
                    End If
                    If (Me.SectionCoverageIAndIIEnum = QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage) Then
                        ddPropIncreaseLimit.Visible = False
                        txtLiabIncreaseLimit.Visible = False
                    End If
                    Me.txtPropIncludedLimit.Enabled = False
                    Me.txtPropTotalLimit.Enabled = False
                    Me.txtLiabIncludedLimit.Enabled = False
                    Me.txtLiabTotalLimit.Enabled = False
                End If
            Case DisplayType.isTrust
                'Added 2/14/18 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trTrust.Visible = True
                End If
            Case DisplayType.isAdditionalInsured
                'Added 4/30/18 for HOM Upgrade Bug 26102 MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Me.trAdditionalInsured.Visible = True
                End If
            Case DisplayType.isSpecialEventCoverage
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                    Me.trSpecialEventCoverage.Visible = True
                    CollapseSpecialEventAddressSection()
                End If
        End Select

    End Sub

    Public Overrides Sub Populate()
        Me.hndhomOptionalCovChk.Value = "0"
        'Updated 8/23/18 for multi state MLW
        'If Me.MyLocation.IsNotNull Then
        If Me.MyLocation IsNot Nothing Then
            CustomizeByCoverageType()

            RenderBasedOndisplayType()
            Dim shouldSetSectionICoverageDefaults As Boolean = If(Me.MyLocation IsNot Nothing AndAlso Me.MyLocation.SectionICoverages Is Nothing OrElse Me.MyLocation.SectionICoverages.Any() = False, True, False) 'use this to auto add coverages to new quotes

            'Added 11/1/2022 for bug 78012 MLW
            If Quote.Locations(0).FormTypeId = "22" AndAlso Quote.Locations(0).StructureTypeId = "2" Then
                shouldSetSectionICoverageDefaults = CheckSectionICoverageFromDiamond(shouldSetSectionICoverageDefaults)
                'HO 0002 - Mobile Home Broad Form: FormTypeId 22 is HO2, StructureTypeId 2 is Mobile - Diamond adds Inflation Guard at Property Save instead of rate which then skips the defaulting since a SectionICoverage is found, so need to check that only the VR available coverages are in the list in order to default coverages
            End If

            'populate fields based on the DisplayType and not the coverage

            'Updated 8/23/18 for multi state MLW
            'If Me.MySectionCoverage.IsNotNull Then
            If Me.MySectionCoverage IsNot Nothing Then
                Me.chkCov.Checked = True
                Me.hndhomOptionalCovChk.Value = "1"
                Me.hdnCoverageCode.Value = ""

                'MySectionCoverage.IncludedLimit = Me.txtIncludedLimit.Text ' yes I want to set this so that the calculate total logic has it
                'updated 2/24/2023 - just calling what we want w/o inadvertently overwriting IncludedLimit whenever txtIncludedLimit wasn't set in CustomizeByCoverageType
                MySectionCoverage.ForceCalcTotal()

                Select Case Me.MyDisplayType
                    Case DisplayType.justCheckBox
                        ' Family Cyber Liability MGB 6/4/20
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection Then
                            If shouldSetSectionICoverageDefaults Then
                                Me.chkCov.Checked = True
                            End If
                        End If
                        If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.Quote.Locations(0).FormTypeId = "26" AndAlso Me.MyLocation.OccupancyCodeId = "9" AndAlso Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers Then
                            Me.chkCov.Checked = True
                        End If
                        Exit Select
                    Case DisplayType.hasIncreaseWithFreeForm, DisplayType.hasIncreaseWithLocation
                        Me.txtIncreaseLimit.Text = MySectionCoverage.IncreasedLimit
                        Me.txtTotalLimit.Text = MySectionCoverage.TotalLimit
                        'Added 2/12/18 for HOM Upgrade MLW
                        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities Then
                                Me.divHdnCoverageCode.Visible = True
                                Me.hdnCoverageCode.Value = "80260"
                            End If
                        End If
                    Case DisplayType.hasIncreasewithDropDown
                        'Updated 3/16/18 for HOM Upgrade MLW
                        If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddIncreasedLimit, MySectionCoverage.IncreasedLimit)
                        Else
                            If MySectionCoverage.IncreasedLimitId.IsNullEmptyorWhitespace() = False Then
                                'Updated 3/18/2022 for Bug 71442 MLW
                                If IsEndorsementRelated() AndAlso QQHelper.BitToBoolean(System.Configuration.ConfigurationManager.AppSettings("Bug71442_HOMEnd_ForcedIncreasedLimit")) Then
                                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(Me.ddIncreasedLimit, MySectionCoverage.IncreasedLimitId, MySectionCoverage.IncreasedLimit)
                                Else
                                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddIncreasedLimit, MySectionCoverage.IncreasedLimitId)
                                End If
                                'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddIncreasedLimit, MySectionCoverage.IncreasedLimitId)
                                'else it keeps whatever value it was defaulted to above
                            End If
                        End If
                        Me.txtTotalLimit.Text = MySectionCoverage.TotalLimit
                    Case DisplayType.justDeductible
                        If MySectionCoverage.DeductibleId.IsNullEmptyorWhitespace() = False Then
                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddDeductible, MySectionCoverage.DeductibleId)
                            'else it keeps whatever value it was defaulted to above
                        End If
                    Case DisplayType.justEffectiveDate
                        If MySectionCoverage.EffectiveDate.IsNullEmptyorWhitespace() = False Then
                            Me.txtEffectiveDate.Text = MySectionCoverage.EffectiveDate
                            'else it keeps whatever value it was defaulted to above
                        End If
                    Case DisplayType.hasEffectiveAndExpirationDates
                        'Added 1/15/18 for HOM Upgrade MLW
                        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If MySectionCoverage.EventEffDate.IsNullEmptyorWhitespace() = False Then
                                Me.txtEffectiveDate.Text = MySectionCoverage.EventEffDate
                            End If
                            If MySectionCoverage.EventExpDate.IsNullEmptyorWhitespace() = False Then
                                Me.txtExpirationDate.Text = MySectionCoverage.EventExpDate
                            End If
                        End If
                    Case DisplayType.hasEffAndExpDatesWithLimit
                        'Added 1/22/18 for HOM Upgrade MLW
                        If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If MySectionCoverage.EventEffDate.IsNullEmptyorWhitespace() = False Then
                                Me.txtEffectiveDate.Text = MySectionCoverage.EventEffDate
                            End If
                            If MySectionCoverage.EventExpDate.IsNullEmptyorWhitespace() = False Then
                                Me.txtExpirationDate.Text = MySectionCoverage.EventExpDate
                            End If
                            Me.txtLimit.Text = MySectionCoverage.IncreasedLimit
                        End If
                    Case DisplayType.justDescription
                        If MySectionCoverage.Description.IsNullEmptyorWhitespace() = False Then
                            Me.txtDescription.Text = MySectionCoverage.Description
                        End If
                    Case DisplayType.justLimit
                        'Added 4/9/18 for HOM Upgrade Bug 26128 map to included limit not increased limit MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine Then
                                If MySectionCoverage.IncludedLimit.IsNullEmptyorWhitespace() = False Then
                                    Me.txtLimit.Text = MySectionCoverage.IncludedLimit
                                End If
                            Else
                                If MySectionCoverage.IncreasedLimit.IsNullEmptyorWhitespace() = False Then
                                    Me.txtLimit.Text = MySectionCoverage.IncreasedLimit
                                End If
                            End If
                        Else
                            Me.txtLimit.Text = MySectionCoverage.IncreasedLimit
                        End If
                    Case DisplayType.hasLimitAndDescription
                        If MySectionCoverage.IncreasedLimit.IsNullEmptyorWhitespace() = False Then
                            Me.txtLimit.Text = MySectionCoverage.IncreasedLimit
                        End If
                        If MySectionCoverage.Description.IsNullEmptyorWhitespace() = False Then
                            Me.txtDescription.Text = MySectionCoverage.Description
                        End If
                    Case DisplayType.isBusinessPursuits
                        'Added 1/24/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If MySectionCoverage.Name.FirstName.IsNullEmptyorWhitespace() = False Then
                                Me.txtInsuredFirstName.Text = MySectionCoverage.Name.FirstName
                            End If
                            If MySectionCoverage.Name.MiddleName.IsNullEmptyorWhitespace() = False Then
                                Me.txtInsuredMiddleName.Text = MySectionCoverage.Name.MiddleName
                            End If
                            If MySectionCoverage.Name.LastName.IsNullEmptyorWhitespace() = False Then
                                Me.txtInsuredLastName.Text = MySectionCoverage.Name.LastName
                            End If
                            If MySectionCoverage.Name.SuffixName.IsNullEmptyorWhitespace() = False Then
                                'Me.ddInsuredSuffixName.SetFromValue(MySectionCoverage.Name.SuffixName)
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddInsuredSuffixName, MySectionCoverage.Name.SuffixName)
                            End If
                            If MySectionCoverage.Description.IsNullEmptyorWhitespace() = False Then
                                Me.txtInsuredBusinessName.Text = MySectionCoverage.Description
                            End If
                        End If
                    Case DisplayType.isGreenUpgrades
                        'Added 1/25/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If MySectionCoverage.IncreasedLimit.IsNullEmptyorWhitespace() = False Then
                                Me.txtMaximumAmount.Text = MySectionCoverage.IncreasedLimit
                            End If
                            If MySectionCoverage.IncreasedCostOfLossId.IsNullEmptyorWhitespace() = False Then
                                'Me.ddIncreasedCostOfLoss.SelectedValue = MySectionCoverage.IncreasedCostOfLoss
                                'Me.ddIncreasedCostOfLoss.SetFromValue(MySectionCoverage.IncreasedCostOfLossId)
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddIncreasedCostOfLoss, MySectionCoverage.IncreasedCostOfLossId)
                            End If
                            If MySectionCoverage.VegetatedRoofApplied = True Then
                                Me.chkVegetatedRoofApplies.Checked = True
                            End If
                            If MySectionCoverage.ExpRelCoverageLimit.IsNullEmptyorWhitespace() = False AndAlso _chc.NumericStringComparison(MySectionCoverage.ExpRelCoverageLimit, CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                                Me.txtExpRelCovLimit.Text = MySectionCoverage.ExpRelCoverageLimit
                                Me.chkExpRelCov.Checked = True
                            End If
                        End If
                    Case DisplayType.isBusinessStructure
                        'Added 1/29/18 for HOM Upgrade MLW
                        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            If MySectionCoverage.Description.IsNullEmptyorWhitespace() = False Then
                                'Business description and building description both use the description field
                                Dim txtDescr As String = MySectionCoverage.Description
                                Dim txtBusinessDescr As String = Nothing
                                Dim txtBuildingDescr As String = Nothing
                                If txtDescr.Contains(vbNewLine) Then
                                    Dim txtDescrSplit As Array = Split(txtDescr, vbNewLine)
                                    txtBusinessDescr = txtDescrSplit(0)
                                    txtBuildingDescr = txtDescrSplit(1)
                                Else
                                    txtBusinessDescr = txtDescr
                                    txtBuildingDescr = ""
                                End If
                                Me.txtBusinessDescription.Text = txtBusinessDescr
                                Me.txtBuildingDescription.Text = txtBuildingDescr
                            End If
                            If _chc.NumericStringComparison(MySectionCoverage.BuildingLimit, CommonHelperClass.ComparisonOperators.GreaterThan, 0) Then
                                Me.txtBuildingLimit.Text = MySectionCoverage.BuildingLimit
                            Else
                                Me.txtBuildingLimit.Text = ""
                            End If
                        End If
                    Case DisplayType.isSpecialEventCoverage
                        Dim sc2Ndx As Integer = -1
                        Dim mysc2 As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = GetNewSpecialEventCoverage(sc2Ndx)
                        If mysc2 IsNot Nothing Then
                            ' COVERAGE IS NEW TO IMAGE - POPULATE AND ALLOW EDITS
                            ' Populate fields
                            lbSpecialEventAddAddress.Text = "Add Address"
                            If mysc2.Description IsNot Nothing Then txtSpecialEventDescription.Text = mysc2.Description
                            If IsDate(mysc2.EventEffDate) Then bdpSpecialEventFromDate.SelectedDate = CDate(mysc2.EventEffDate)
                            If IsDate(mysc2.EventExpDate) Then bdpSpecialEventToDate.SelectedDate = CDate(mysc2.EventExpDate)
                            ' If there is any address data, populate the address and change the add button text
                            If SpecialEventCoverageHasAnyAddressData() Then
                                lbSpecialEventAddAddress.Text = "View/Edit Address"
                                txtSpecialEventStreetNumber.Text = mysc2.Address.HouseNum
                                txtSpecialEventStreetName.Text = mysc2.Address.StreetName
                                txtSpecialEventAptSuiteNumber.Text = mysc2.Address.ApartmentNumber
                                txtSpecialEventZip.Text = mysc2.Address.Zip
                                txtSpecialEventCity.Text = mysc2.Address.City
                                ddlSpecialEventState.SetFromValue(mysc2.Address.StateId)
                                txtSpecialEventCounty.Text = mysc2.Address.County
                            End If
                        Else
                            ' EXISTING COVERAGE, NOT NEW TO IMAGE. DO NOT POPULATE,
                            ' ALLOW USER TO ENTER ANOTHER SPECIAL EVENT COVERAGE
                            chkCov.Checked = False
                            chkCov.Enabled = True
                            Me.hndhomOptionalCovChk.Value = "0"
                            ClearSpecialEventAddressControls()
                            CollapseSpecialEventAddressSection()
                        End If

                        ' Set the label text on the checkbox - shows number of previusly added events
                        Dim txt As String = GetSpecialEventCheckboxLabelText()
                        lblHeader.Text = txt

                        Exit Select
                End Select
            Else
                ' SECTION COVERAGE IS NOTHING
                Me.ClearControl()
                'Updated 12/26/17 for HOM Upgrade MLW
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    If shouldSetSectionICoverageDefaults And IsDefaultCoverage Then
                        Me.chkCov.Checked = True
                        Me.hndhomOptionalCovChk.Value = "1"
                    Else
                        Me.chkCov.Checked = False
                        Me.hndhomOptionalCovChk.Value = "0"
                    End If
                Else
                    If shouldSetSectionICoverageDefaults And IsDefaultCoverage Or Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Then
                        Me.chkCov.Checked = True
                        Me.hndhomOptionalCovChk.Value = "1"
                    Else
                        Me.chkCov.Checked = False
                        Me.hndhomOptionalCovChk.Value = "0"
                    End If
                End If
            End If

            If MyDisplayType = DisplayType.justCheckBox Then
            End If

            ' you always need to populate these sub controls so they will be ready if they are selected
            If MyDisplayType = DisplayType.isOtherStructures Then
                'load sub control
                ctlHomSpecifiedStructureList.InitFromExisting(Me)
                ctlHomSpecifiedStructureList.Populate()
            End If

            If MyDisplayType = DisplayType.isFarmLand Then
                'load sub control
                ctlHomFarmLandList.InitFromExisting(Me)
                ctlHomFarmLandList.Populate()
            End If

            If MyDisplayType = DisplayType.isAdditionlResidence Then
                'load sub control
                ctlHomAdditionalResidenceList.InitFromExisting(Me)
                ctlHomAdditionalResidenceList.Populate()
            End If

            'Added 1/23/18 for HOM Upgrade MLW
            If MyDisplayType = DisplayType.isCanine Then
                'load sub control
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    ctlHomMultipleNamesList.InitFromExisting(Me)
                    ctlHomMultipleNamesList.Populate()
                End If
            End If
            'removed 3/16/18 - Loss Assessment no longer multiple, no longer capturing address MLW
            'Added 1/30/18 for HOM Upgrade MLW
            'If MyDisplayType = DisplayType.isLossAssessment Then
            '    'load sub control
            '    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            '        ctlHomLossAssessmentList.InitFromExisting(Me)
            '        ctlHomLossAssessmentList.Populate()
            '    End If
            'End If
            'Added 1/31/18 for HOM Upgrade MLW
            If MyDisplayType = DisplayType.isOtherMembers Then
                'load sub control
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    ctlHomOtherMembersList.InitFromExisting(Me)
                    ctlHomOtherMembersList.Populate()
                End If
            End If
            'Added 1/31/18 for HOM Upgrade MLW
            If MyDisplayType = DisplayType.isAdditionalInterests Then
                'load sub control
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then 'limits for Assisted Living only
                        If MySectionCoverage IsNot Nothing Then
                            If MySectionCoverage.IncreasedLimit.IsNullEmptyorWhitespace() = False Then
                                Me.txtPropIncreaseLimit.Text = MySectionCoverage.IncreasedLimit
                            End If
                            If MySectionCoverage.LiabilityIncreasedLimit.IsNullEmptyorWhitespace() = False Then
                                'Updated for Bug 26491 MLW
                                'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddLiabIncreaseLimit, MySectionCoverage.LiabilityIncreasedLimit)
                                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddLiabIncreaseLimit, MySectionCoverage.LiabilityIncreasedLimit)
                            End If
                        End If
                    End If
                    ctlHomAdditionalInterestsList.InitFromExisting(Me)
                    ctlHomAdditionalInterestsList.Populate()
                End If
            End If
            'Added 2/14/18 for HOM Upgrade MLW
            If MyDisplayType = DisplayType.isTrust Then
                'load sub control
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    ctlHomTrustList.InitFromExisting(Me)
                    ctlHomTrustList.Populate()
                End If
            End If
            'Added 4/30/18 for HOM Upgrade Bug 26102 MLW
            If MyDisplayType = DisplayType.isAdditionalInsured Then
                'load sub control
                If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    ctlHomAdditionalInsuredList.InitFromExisting(Me)
                    ctlHomAdditionalInsuredList.Populate()
                End If
            End If
            If MyDisplayType = DisplayType.hasIncreaseWithLocation Then
                ctlHomLocation.InitFromExisting(Me)
                ctlHomLocation.Populate()
            End If

        End If

        'Added 1/8/18 for HOM Upgrade MLW
        If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Select Case Me.SectionType
                Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                    'Added 1/9/18 for HOM Upgrade MLW
                    Select Case Me.SectionCoverageIEnum
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.IdentityFraudExpenseHOM0455
                            Me.txtIncludedLimit.Text = "15,000"
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine
                            Me.txtLimit.Text = "10,000"
                    End Select
                Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                    Select Case Me.SectionCoverageIIEnum
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                            'Incidental Farmers Personal Liability - On Premises
                            'Updated 7/3/2019 for Home Endorsements Project Task 38908, 38925 MLW
                            lblMaxCharCount.Visible = False
                            lblMaxChar.Visible = False
                            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                                If Me.txtDescription.Text = "" Then
                                    Me.txtDescription.Text = "PERSONAL LIABILITY"
                                End If
                            Else
                                lblDescription.Text = "*Description" 'Added 7/9/2019 for Home Endorsements Project Task 38908 MLW
                            End If
                    End Select
                Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage

                    Select Case Me.SectionCoverageIAndIIEnum
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                            'Structures Rented to Others - Residence Premises
                            'Added 7/8/2019 for Home Endorsements Project Task 38915 MLW
                            lblMaxCharCount.Visible = False
                            lblMaxChar.Visible = False
                            'Updated 7/3/2019 for Home Endorsements Project Task 38915, 38925 MLW
                            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                                If String.IsNullOrWhiteSpace(Me.txtDescription.Text) Then
                                    Me.txtDescription.Text = "STRUCTURE"
                                End If
                            Else
                                lblDescription.Text = "*Description" 'Added 7/9/2019 for Home Endorsements Project Task 38915 MLW
                                lblLimit.Text = "*Limit" 'Added 7/10/2019 for Home Endorsements Project Task 38915 MLW
                            End If
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                            'Permitted Incidental Occupancies Residence Premises 
                            'Added 1/29/18 for HOM Upgrade
                            'Updated 7/3/2019 for Home Endorsements Project Task 38914, 38925 MLW
                            If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                                If String.IsNullOrWhiteSpace(Me.txtBusinessDescription.Text) Then
                                    Me.txtBusinessDescription.Text = "BUSINESS"
                                End If
                            Else
                                Me.txtBusinessDescription.MaxLength = 100
                            End If

                            If (txtBuildingDescription.Text.NoneAreNullEmptyorWhitespace() AndAlso txtBuildingDescription.Text <> "Building") OrElse txtBuildingLimit.Text.NoneAreNullEmptyorWhitespace() Then
                                chkOtherStructures.Checked = True
                                'Updated 7/3/2019 for Home Endorsements Project Task 38914, 38925 MLW
                                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                                    If txtBuildingDescription.Text.IsNullEmptyorWhitespace() Then 'Just incase the user wiped out the description on AppGap and came back to quote side.
                                        txtBuildingDescription.Text = "Building"
                                    End If
                                Else
                                    Me.txtBuildingDescription.MaxLength = 100
                                End If
                            End If
                    End Select
            End Select
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.chkCov.Checked = CBool(Me.hndhomOptionalCovChk.Value) 'I know it seems weird that you need this but you do - It is because of the disabled checkboxes - Matt A 4-7-16
    End Sub

    Public Overrides Function Save() As Boolean
        If MyDisplayType = DisplayType.notAvailable Then
            'remove
            'Dim updatechktext As Boolean = False
            'If MyDisplayType = DisplayType.isSpecialEventCoverage Then updatechktext = True

            Me.DeleteMySectionCoverage()

            'If updatechktext Then
            '    Dim txt As String = GetSpecialEventCheckboxLabelText()
            '    lblHeader.Text = updatechktext
            'End If
        Else
            If Me.MyDisplayType = DisplayType.included Then
                If Me.MySectionCoverage.IsNull Then
                    'create and add to list
                    CreateMySectionCoverage()
                End If
                MySectionCoverage.IncludedLimit = Me.txtIncludedLimit.Text
            Else
                If CBool(Me.hndhomOptionalCovChk.Value) Then
                    If Me.MySectionCoverage.IsNull Then
                        'create and add to list
                        CreateMySectionCoverage()
                    End If
                    Select Case Me.MyDisplayType
                        Case DisplayType.justCheckBox
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.IdentityFraudExpenseHOM0455 OrElse
                                    Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage Then
                                    MySectionCoverage.IncludedLimit = txtIncludedLimit.Text
                                End If
                                If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection Then
                                    MySectionCoverage.IncludedLimit = "50,000"
                                    MySectionCoverage.IncreasedLimitId = "22"  ' Needs to be set to 22 ($500) to set the CoverageLimitID on the coverage
                                End If
                            End If
                        Case DisplayType.justDeductible
                            MySectionCoverage.DeductibleId = Me.ddDeductible.SelectedValue
                        Case DisplayType.justEffectiveDate
                            MySectionCoverage.EffectiveDate = Me.txtEffectiveDate.Text.Trim()
                        Case DisplayType.hasIncreaseWithFreeForm, DisplayType.hasIncreaseWithLocation
                            MySectionCoverage.IncludedLimit = Me.txtIncludedLimit.Text
                            MySectionCoverage.IncreasedLimit = Me.txtIncreaseLimit.Text
                            If Me.MyDisplayType = DisplayType.hasIncreaseWithLocation Then
                                Me.ctlHomLocation.Save()
                            End If
                        Case DisplayType.hasIncreasewithDropDown
                            'Updated 1/16/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                MySectionCoverage.IncludedLimit = Me.txtIncludedLimit.Text
                                'removed 3/8/18 for HOM Upgrade MLW
                                ''Added for Loss Assessment - Earthquake rating error - it is looking for the Increased Limit and not the Increased Limit Id - 2/27/18 MLW
                                'If Me.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake Then
                                '    MySectionCoverage.IncreasedLimit = Me.ddIncreasedLimit.SelectedItem.Text
                                'End If
                            Else
                                If Me.SectionCoverageIEnum <> QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains Then
                                    MySectionCoverage.IncludedLimit = Me.txtIncludedLimit.Text
                                Else
                                    MySectionCoverage.IncludedLimit = "" ' always nothing even if it says 5,000
                                End If
                            End If
                            'Updated 3/16/18 for HOM Upgrade MLW
                            If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment Then
                                MySectionCoverage.IncreasedLimit = Me.ddIncreasedLimit.SelectedItem.Text
                            Else
                                MySectionCoverage.IncreasedLimitId = Me.ddIncreasedLimit.SelectedValue
                            End If
                        Case DisplayType.justLimit
                            'Added 4/9/18 for HOM Upgrade Bug 26128 map to included limit not increased limit MLW
                            If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                If SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine Then
                                    MySectionCoverage.IncludedLimit = Me.txtLimit.Text
                                    MySectionCoverage.IncreasedLimit = "0" 'Added 5/1/18 for Bug 26128 - need to clear out old quote's increased limit - MLW
                                Else
                                    MySectionCoverage.IncreasedLimit = Me.txtLimit.Text
                                End If
                            Else
                                MySectionCoverage.IncreasedLimit = Me.txtLimit.Text
                            End If
                        Case DisplayType.justDescription
                            MySectionCoverage.Description = Me.txtDescription.Text.ToMaxLength(250)
                        Case DisplayType.hasLimitAndDescription
                            MySectionCoverage.IncreasedLimit = Me.txtLimit.Text
                            MySectionCoverage.Description = Me.txtDescription.Text.ToMaxLength(250)
                        Case DisplayType.hasEffectiveAndExpirationDates
                            'Added 1/15/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                MySectionCoverage.EventEffDate = Me.txtEffectiveDate.Text.Trim()
                                MySectionCoverage.EventExpDate = Me.txtExpirationDate.Text.Trim()
                            End If
                        Case DisplayType.hasEffAndExpDatesWithLimit
                            'Added 1/22/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                MySectionCoverage.EventEffDate = Me.txtEffectiveDate.Text.Trim()
                                MySectionCoverage.EventExpDate = Me.txtExpirationDate.Text.Trim()
                                MySectionCoverage.IncreasedLimit = Me.txtLimit.Text
                            End If
                        Case DisplayType.isBusinessPursuits
                            'Added 1/24/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                MySectionCoverage.Name.FirstName = Me.txtInsuredFirstName.Text.Trim()
                                MySectionCoverage.Name.MiddleName = Me.txtInsuredMiddleName.Text.Trim()
                                MySectionCoverage.Name.LastName = Me.txtInsuredLastName.Text.Trim()
                                MySectionCoverage.Name.SuffixName = Me.ddInsuredSuffixName.SelectedValue.Trim()
                                MySectionCoverage.Description = Me.txtInsuredBusinessName.Text.Trim()
                            End If
                        Case DisplayType.isGreenUpgrades
                            'Added 1/25/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                MySectionCoverage.IncreasedLimit = Me.txtMaximumAmount.Text.Trim()
                                MySectionCoverage.IncreasedCostOfLossId = Me.ddIncreasedCostOfLoss.SelectedValue.Trim()
                                MySectionCoverage.VegetatedRoofApplied = Me.chkVegetatedRoofApplies.Checked
                                'Session("ExpenseRelatedCoverage") = Me.chkExpRelCov.Checked 'Doesn't appear to be used
                                If chkExpRelCov.Checked = True Then
                                    MySectionCoverage.ExpRelCoverageLimit = Me.txtExpRelCovLimit.Text.Trim()
                                End If
                            End If
                        Case DisplayType.isBusinessStructure
                            'Added 1/29/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                'Business description and building description both use the description field
                                'Matt A - Bug 26340 - Added .ToMaxLength(125)
                                MySectionCoverage.Description = Me.txtBusinessDescription.Text.Trim().ToMaxLength(125) & vbNewLine & Me.txtBuildingDescription.Text.Trim().ToMaxLength(125)
                                MySectionCoverage.BuildingLimit = Me.txtBuildingLimit.Text.Trim()
                            End If
                        Case DisplayType.isOtherStructures
                            Me.ctlHomSpecifiedStructureList.Save()
                        ' have subcontrols
                        Case DisplayType.isAdditionlResidence
                            ' have subcontrols
                            Me.ctlHomAdditionalResidenceList.Save()
                        Case DisplayType.isFarmLand
                            ' have subcontrols
                            Me.ctlHomFarmLandList.Save()
                        Case DisplayType.isCanine 'Added 1/23/18 for HOM Upgrade MLW
                            ' have subcontrols
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                Me.ctlHomMultipleNamesList.Save()
                            End If
                            '3/16/18 - no longer using for Loss Assessment, no longer capturing multiples and address
                        'Case DisplayType.isLossAssessment 'Added 1/30/18 for HOM Upgrade MLW
                        '    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        '        Me.ctlHomLossAssessmentList.Save()
                        '    End If
                        Case DisplayType.isOtherMembers 'Added 1/31/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                Me.ctlHomOtherMembersList.Save()
                            End If
                        Case DisplayType.isAdditionalInterests 'Added 1/31/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                'Added 1/25/18 for HOM Upgrade MLW
                                If Me.SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage Then 'limits for Assisted Living only
                                    MySectionCoverage.IncludedLimit = Me.txtPropIncludedLimit.Text.Trim()
                                    MySectionCoverage.IncreasedLimit = Me.txtPropIncreaseLimit.Text.Trim()
                                    MySectionCoverage.LiabilityIncludedLimit = Me.txtLiabIncludedLimit.Text.Trim()
                                    MySectionCoverage.LiabilityIncreasedLimit = Me.ddLiabIncreaseLimit.SelectedItem.Text 'Updated for Bug 26104 MLW
                                    'MySectionCoverage.LiabilityIncreasedLimit = Me.ddLiabIncreaseLimit.SelectedValue
                                End If
                                Me.ctlHomAdditionalInterestsList.Save()
                            End If
                        Case DisplayType.isTrust 'Added 2/14/18 for HOM Upgrade MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                Me.ctlHomTrustList.Save()
                            End If
                        Case DisplayType.isAdditionalInsured 'Added 4/30/18 for HOM Upgrade Bug 26102 MLW
                            If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                Me.ctlHomAdditionalInsuredList.Save()
                            End If
                        Case DisplayType.isSpecialEventCoverage
                            ' We want to be able to save a new special event coverage on each endorsement
                            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                Dim S2Index As Integer = -1
                                Dim newcov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = GetNewSpecialEventCoverage(S2Index)
                                If newcov Is Nothing Then
                                    If chkCov.Checked Then
                                        ' Checkbox is checked
                                        ' No new special event section II coverages found
                                        ' Need to add a new Special Event Section II coverage
                                        newcov = New QuickQuoteSectionIICoverage()
                                        newcov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.SpecialEventCoverage
                                        newcov.Description = txtSpecialEventDescription.Text
                                        newcov.EventEffDate = bdpSpecialEventFromDate.SelectedDateFormatted
                                        newcov.EventExpDate = bdpSpecialEventToDate.SelectedDateFormatted
                                        newcov.Address = New QuickQuoteAddress()
                                        newcov.Address.HouseNum = txtSpecialEventStreetNumber.Text
                                        newcov.Address.StreetName = txtSpecialEventStreetName.Text
                                        newcov.Address.ApartmentNumber = txtSpecialEventAptSuiteNumber.Text
                                        newcov.Address.Zip = txtSpecialEventZip.Text
                                        newcov.Address.City = txtSpecialEventCity.Text
                                        newcov.Address.StateId = ddlSpecialEventState.SelectedValue
                                        newcov.Address.County = txtSpecialEventCounty.Text
                                        MyLocation.SectionIICoverages.Add(newcov)
                                        'Else
                                        '    ' CHECKBOX NOT CHECKED, DELETE THE COVERAGE!!  (Don't do anything here vecause newcov is nothing)
                                    End If
                                Else
                                    If chkCov.Checked Then
                                        ' Checkbox is checked
                                        ' New special event section II coverage found
                                        ' Update the existing special event section II coverage
                                        MyLocation.SectionIICoverages(S2Index).Description = txtSpecialEventDescription.Text
                                        MyLocation.SectionIICoverages(S2Index).EventEffDate = bdpSpecialEventFromDate.SelectedDateFormatted
                                        MyLocation.SectionIICoverages(S2Index).EventExpDate = bdpSpecialEventToDate.SelectedDateFormatted
                                        MyLocation.SectionIICoverages(S2Index).Address.HouseNum = txtSpecialEventStreetNumber.Text
                                        MyLocation.SectionIICoverages(S2Index).Address.StreetName = txtSpecialEventStreetName.Text
                                        MyLocation.SectionIICoverages(S2Index).Address.ApartmentNumber = txtSpecialEventAptSuiteNumber.Text
                                        MyLocation.SectionIICoverages(S2Index).Address.Zip = txtSpecialEventZip.Text
                                        MyLocation.SectionIICoverages(S2Index).Address.City = txtSpecialEventCity.Text
                                        MyLocation.SectionIICoverages(S2Index).Address.StateId = ddlSpecialEventState.SelectedValue
                                        MyLocation.SectionIICoverages(S2Index).Address.County = txtSpecialEventCounty.Text
                                        'Else
                                        '    ' CHECKBOX NOT CHECKED, DELETE THE COVERAGE!! 
                                        '    Quote.Locations(0).SectionIICoverages.RemoveAt(S2Index)
                                    End If
                                End If
                                lblHeader.Text = GetSpecialEventCheckboxLabelText()
                            End If
                    End Select
                Else
                    'remove
                    'Updated 5/19/2022 for task 74106 MLW, Updated 6/13/2022 for task 74187 MLW
                    'Me.DeleteMySectionCoverage()
                    'If HOM_General.HPEEWaterBUEnabled() AndAlso quote.EffectiveDate >= HOM_General.HPEEWaterBUEffDate() AndAlso MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains Then                       
                    If HOM_General.OkayForHPEEWaterBU(Quote) AndAlso MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains Then
                        Dim hasEnhancement As Boolean = False
                        Dim hasPlusEnhancement As Boolean = False
                        hasEnhancement = MyLocation.SectionICoverages.FindAll(Function(c) c.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement).Any()
                        If Not hasEnhancement Then
                            hasEnhancement = MyLocation.SectionICoverages.FindAll(Function(c) c.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019).Any()
                        End If
                        hasPlusEnhancement = MyLocation.SectionICoverages.FindAll(Function(c) c.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020).Any()
                        If Not hasEnhancement AndAlso Not hasPlusEnhancement Then
                            Me.DeleteMySectionCoverage()
                        End If
                    Else
                        Me.DeleteMySectionCoverage()
                    End If

                End If
            End If
        End If

        ' Me.SaveChildControls()

        Return True
    End Function

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        Me.txtDescription.Text = ""
        Me.txtEffectiveDate.Text = ""
        Me.txtExpirationDate.Text = "" 'Added 1/23/18 for HOM Upgrade MLW
        Me.txtIncreaseLimit.Text = ""
        Me.txtLimit.Text = ""
        Me.ddDeductible.SelectedIndex = -1
        Me.ddIncreasedLimit.SelectedIndex = -1
        Me.txtInsuredFirstName.Text = "" 'Added 1/24/18 for HOM Upgrade MLW
        Me.txtInsuredMiddleName.Text = "" 'Added 1/24/18 for HOM Upgrade MLW
        Me.txtInsuredLastName.Text = "" 'Added 1/24/18 for HOM Upgrade MLW
        Me.ddInsuredSuffixName.SelectedIndex = -1 'Added 1/24/18 for HOM Upgrade MLW
        Me.txtInsuredBusinessName.Text = "" 'Added 1/24/18 for HOM Upgrade MLW
        Me.txtMaximumAmount.Text = "" 'Added 1/25/18 for HOM Upgrade MLW
        Me.ddIncreasedCostOfLoss.SelectedIndex = -1 'Added 1/25/18 for HOM Upgrade MLW
        Me.chkVegetatedRoofApplies.Checked = False 'Added 1/25/18 for HOM Upgrade MLW
        Me.chkExpRelCov.Checked = False 'Added 1/25/18 for HOM Upgrade MLW
        'Session("ExpenseRelatedCoverage") = False 'Added 2/13/18 for HOM Upgrade MLW 'Doesn't appear to be used
        Me.txtExpRelCovLimit.Text = "" 'Added 1/25/18 for HOM Upgrade MLW
        Me.txtPropIncreaseLimit.Text = "" 'Added 2/5/18 for HOM Upgrade MLW
        Me.ddLiabIncreaseLimit.SelectedIndex = -1 'Added 2/5/18 for HOM Upgrade MLW
        ' special event controls
        txtSpecialEventDescription.Text = ""
        bdpSpecialEventFromDate.SelectedDate = Nothing
        bdpSpecialEventToDate.SelectedDate = Nothing
        ClearSpecialEventAddressControls()
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        '#If DEBUG Then
        '        If Me.MyDisplayType = DisplayType.hasLimitAndDescription Then
        '            Debugger.Break()
        '        End If
        '#End If
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = Me.CoverageName
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        If Me.MySectionCoverage IsNot Nothing Then
            Select Case Me.MyDisplayType
                Case DisplayType.hasIncreasewithDropDown, DisplayType.hasIncreaseWithFreeForm, DisplayType.included, DisplayType.justCheckBox, DisplayType.justDeductible, DisplayType.justDescription, DisplayType.justEffectiveDate, DisplayType.justLimit, DisplayType.hasLimitAndDescription, DisplayType.hasEffectiveAndExpirationDates, DisplayType.hasEffAndExpDatesWithLimit, DisplayType.isBusinessPursuits, DisplayType.isGreenUpgrades, DisplayType.isBusinessStructure, DisplayType.hasIncreaseWithLocation
                    Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                    If Me.MyDisplayType = DisplayType.isGreenUpgrades Then
                        If chkExpRelCov.Checked = True Then
                            If _chc.NumericStringComparison(txtExpRelCovLimit.Text, CommonHelperClass.ComparisonOperators.GreaterThan, 0) = False Then
                                'IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField(Me.txtExpRelCovLimit.Text, valList, "{63DAEF9C-7AA0-4380-B8BB-236A28AD78C4}", "Limit")
                                IFM.VR.Validation.VRGeneralValidations.Val_HasRequiredField_Int(Me.txtExpRelCovLimit.Text, valList, "{63DAEF9C-7AA0-4380-B8BB-236A28AD78C4}", "Limit")
                            End If
                        End If
                    End If
                    If valList.Any() Then
                        For Each v In valList
                            Select Case v.FieldId
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.IncludedLimit
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtIncludedLimit, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.IncreasedLimit
                                    Select Case Me.MyDisplayType
                                        Case DisplayType.hasIncreaseWithFreeForm, DisplayType.hasIncreaseWithLocation
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtIncreaseLimit, v, accordList)
                                        Case DisplayType.hasIncreasewithDropDown
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddIncreasedLimit, v, accordList)
                                        Case DisplayType.justLimit, DisplayType.hasLimitAndDescription, DisplayType.hasEffAndExpDatesWithLimit
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLimit, v, accordList)
                                    End Select
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.TotalLimit
                                    'Updated 1/29/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isBusinessStructure Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBuildingLimit, v, accordList)
                                        End If
                                    Else
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtTotalLimit, v, accordList)
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Deductible
                                    If Me.MyDisplayType = DisplayType.justDeductible Then
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddDeductible, v, accordList)
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                                    If Me.MyDisplayType = DisplayType.justDescription Or Me.MyDisplayType = DisplayType.hasLimitAndDescription Then
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescription, v, accordList)
                                    End If
                                    'Added 1/29/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isBusinessStructure Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBusinessDescription, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.EffectiveDate
                                    If Me.MyDisplayType = DisplayType.justEffectiveDate Then
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEffectiveDate, v, accordList)
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.EventEffDate 'Added 1/15/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.hasEffectiveAndExpirationDates OrElse Me.MyDisplayType = DisplayType.hasEffAndExpDatesWithLimit Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEffectiveDate, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.EventExpDate 'Added 1/15/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.hasEffectiveAndExpirationDates OrElse Me.MyDisplayType = DisplayType.hasEffAndExpDatesWithLimit Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtExpirationDate, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ConstructionType
                                    'Me.ValidationHelper.Val_BindValidationItemToControl(Me.dd, v, accordList)
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.InsuredFirstName
                                    'Added 1/24/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isBusinessPursuits Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtInsuredFirstName, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.InsuredLastName
                                    'Added 1/24/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isBusinessPursuits Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtInsuredLastName, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.InsuredBusinessName
                                    'Added 1/24/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isBusinessPursuits Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtInsuredBusinessName, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.MaximumAmount
                                    'Added 1/25/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isGreenUpgrades Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMaximumAmount, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.IncreasedCostOfLoss
                                    'Added 1/29/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isGreenUpgrades Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddIncreasedCostOfLoss, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ExpenseRelatedCoverageLimit
                                    'Added 1/25/18 for HOM Upgrade MLW
                                    'Required only if ExpRelCov checkbox is checked
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isGreenUpgrades Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtExpRelCovLimit, v, accordList)
                                        End If
                                    End If
                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.BuildingDescription
                                    'Added 1/29/18 for HOM Upgrade MLW
                                    If (Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If Me.MyDisplayType = DisplayType.isBusinessStructure Then
                                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtBuildingDescription, v, accordList)
                                        End If
                                    End If

                                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                    Me.ValidationHelper.AddError(v.Message)

                            End Select

                        Next

                        If Me.MyDisplayType = DisplayType.hasIncreaseWithLocation Then
                            Me.ctlHomLocation.ValidateControl(valArgs)
                        End If
                    End If
                Case DisplayType.isAdditionlResidence
                    Me.ctlHomAdditionalResidenceList.ValidateControl(valArgs)
                    'Added 1/11/18 for HOM Upgrade MLW - to show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListAR = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListAR.Any() Then
                            For Each vAR In valListAR
                                Select Case vAR.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vAR.Message)
                                End Select
                            Next
                        End If
                    End If

                Case DisplayType.isFarmLand
                    Me.ctlHomFarmLandList.ValidateControl(valArgs)
                    'Added 1/11/18 for HOM Upgrade MLW - to show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListFL = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListFL.Any() Then
                            For Each vFL In valListFL
                                Select Case vFL.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vFL.Message)
                                End Select
                            Next
                        End If
                    End If

                Case DisplayType.isOtherStructures
                    Me.ctlHomSpecifiedStructureList.ValidateControl(valArgs)
                    'Added 1/11/18 for HOM Upgrade MLW - to show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListOS = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListOS.Any() Then
                            For Each vOS In valListOS
                                Select Case vOS.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vOS.Message)
                                End Select
                            Next
                        End If
                    End If

                Case DisplayType.isCanine 'Added 1/23/18 for HOM Upgrade MLW
                    Me.ctlHomMultipleNamesList.ValidateControl(valArgs)
                    'show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListCanine = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListCanine.Any() Then
                            For Each vCanine In valListCanine
                                Select Case vCanine.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vCanine.Message)
                                End Select
                            Next
                        End If
                    End If
                    '3/16/18 - no longer using for Loss Assessment, no longer capturing multiples and address
                'Case DisplayType.isLossAssessment 'Added 1/30/18 for HOM Upgrade MLW
                '    Me.ctlHomLossAssessmentList.ValidateControl(valArgs)
                '    'show coverage not valid message when applicable
                '    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                '        Dim valListLA = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                '        If valListLA.Any() Then
                '            For Each vLA In valListLA
                '                Select Case vLA.FieldId
                '                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                '                        Me.ValidationHelper.AddError(vLA.Message)
                '                End Select
                '            Next
                '        End If
                '    End If
                Case DisplayType.isOtherMembers 'Added 1/31/18 for HOM Upgrade MLW
                    Me.ctlHomOtherMembersList.ValidateControl(valArgs)
                    'show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListOM = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListOM.Any() Then
                            For Each vOM In valListOM
                                Select Case vOM.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vOM.Message)
                                End Select
                            Next
                        End If
                    End If
                Case DisplayType.isAdditionalInterests 'Added 1/31/18 for HOM Upgrade MLW
                    Me.ctlHomAdditionalInterestsList.ValidateControl(valArgs)
                    'show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListAI = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListAI.Any() Then
                            For Each vAI In valListAI
                                Select Case vAI.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.IncludedLimit 'PropertyIncludedLimit
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtPropIncludedLimit, vAI, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.LiabilityIncludedLimit
                                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLiabIncludedLimit, vAI, accordList)
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vAI.Message)
                                End Select
                            Next
                        End If
                    End If
                Case DisplayType.isTrust 'Added 2/14/18 for HOM Upgrade MLW
                    Me.ctlHomTrustList.ValidateControl(valArgs)
                    'show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListTrust = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListTrust.Any() Then
                            For Each vTrust In valListTrust
                                Select Case vTrust.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vTrust.Message)
                                End Select
                            Next
                        End If
                    End If
                Case DisplayType.isAdditionalInsured 'Added 4/30/18 for HOM Upgrade MLW
                    Me.ctlHomAdditionalInsuredList.ValidateControl(valArgs)
                    'show coverage not valid message when applicable
                    If (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        Dim valListAI = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                        If valListAI.Any() Then
                            For Each vAI In valListAI
                                Select Case vAI.FieldId
                                    Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.CoverageNotValidForFormType
                                        Me.ValidationHelper.AddError(vAI.Message)
                                End Select
                            Next
                        End If
                    End If
                Case DisplayType.isSpecialEventCoverage
                    ' Validate the screen fields not MySectionCoverage
                    Dim valcnt As Integer = 0
                    Dim AddressError As Boolean = False
                    If chkCov.Checked Then

                        If txtSpecialEventDescription.Text.Trim = "" Then
                            Me.ValidationHelper.AddError(txtSpecialEventDescription, "Description is required", accordList)
                            valcnt += 1
                        End If

                        If bdpSpecialEventFromDate.SelectedDateFormatted Is Nothing OrElse bdpSpecialEventFromDate.SelectedDateFormatted = "" Then
                            Me.ValidationHelper.AddError(bdpSpecialEventFromDate, "From Date is required", accordList)
                            valcnt += 1
                        End If
                        If bdpSpecialEventFromDate.SelectedDate < DateTime.Today Then
                            Me.ValidationHelper.AddError(bdpSpecialEventFromDate, "From Date cannot be in the past", accordList)
                            valcnt += 1
                        End If
                        If bdpSpecialEventToDate.SelectedDateFormatted Is Nothing OrElse bdpSpecialEventToDate.SelectedDateFormatted = "" Then
                            Me.ValidationHelper.AddError(bdpSpecialEventToDate, "To Date is required", accordList)
                            valcnt += 1
                        End If
                        If bdpSpecialEventFromDate.SelectedDate > bdpSpecialEventToDate.SelectedDate Then
                            Me.ValidationHelper.AddError(bdpSpecialEventToDate, "To Date must be equal or greater then From date", accordList)
                            valcnt += 1
                        End If
                        If bdpSpecialEventToDate.SelectedDate > bdpSpecialEventFromDate.SelectedDate.AddDays(2) Then
                            Me.ValidationHelper.AddError(bdpSpecialEventToDate, "To Date must be within 2 days of From Date", accordList)
                            valcnt += 1
                        End If
                        If bdpSpecialEventFromDate.SelectedDate > CDate(Quote.EffectiveDate).AddYears(1) Then
                            Me.ValidationHelper.AddError(bdpSpecialEventFromDate, "From Date is outside of current policy term", accordList)
                            valcnt += 1
                        End If
                        If bdpSpecialEventToDate.SelectedDate > CDate(Quote.EffectiveDate).AddYears(1) Then
                            Me.ValidationHelper.AddError(bdpSpecialEventToDate, "To Date is outside of current policy term", accordList)
                            valcnt += 1
                        End If

                        If txtSpecialEventStreetNumber.Text.Trim = "" Then
                            Me.ValidationHelper.AddError(txtSpecialEventStreetNumber, "Street # is required", accordList)
                            valcnt += 1
                            AddressError = True
                        End If
                        If txtSpecialEventStreetName.Text.Trim = "" Then
                            Me.ValidationHelper.AddError(txtSpecialEventStreetName, "Street Name is required", accordList)
                            valcnt += 1
                            AddressError = True
                        End If
                        If txtSpecialEventZip.Text.Trim = "" Then
                            Me.ValidationHelper.AddError(txtSpecialEventZip, "Zip code is required", accordList)
                            valcnt += 1
                            AddressError = True
                        End If
                        If Me.txtSpecialEventCity.Text = "" Then
                            Me.ValidationHelper.AddError(txtSpecialEventCity, "City is required", accordList)
                            valcnt += 1
                            AddressError = True
                        End If
                        If Me.ddlSpecialEventState.SelectedIndex = -1 Then
                            Me.ValidationHelper.AddError(ddlSpecialEventState, "State is required", accordList)
                            valcnt += 1
                            AddressError = True
                        End If
                        If Me.txtSpecialEventCounty.Text.Trim = "" Then
                            Me.ValidationHelper.AddError(txtSpecialEventCounty, "County is required", accordList)
                            valcnt += 1
                            AddressError = True
                        End If

                        If valcnt > 0 Then
                            If AddressError Then
                                OpenSpecialEventAddressSection()
                            Else
                                CollapseSpecialEventAddressSection()
                            End If
                        Else
                            ' If no validation items, collapse the address section
                            CollapseSpecialEventAddressSection()
                        End If
                    End If

                    'If Me.MySectionCoverage IsNot Nothing Then
                    '    Dim valList = IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.ValidateHOMSectionCoverage(Me.Quote, Me.MySectionCoverage, Me.CoverageIndex, Me.DefaultValidationType)
                    '    Dim AddressError As Boolean = False
                    '    If valList.Any() Then
                    '        For Each v In valList
                    '            Select Case v.FieldId
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.Description
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventDescription, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.EventEffDate
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.bdpSpecialEventFromDate, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.EventExpDate
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.bdpSpecialEventToDate, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressStreetNumber
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventStreetNumber, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressStreetName
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventStreetName, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressAptNumber
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventAptSuiteNumber, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressZipCode
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventZip, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressCity
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventCity, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressState
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddlSpecialEventState, v, accordList)
                    '                Case IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM.SectionCoverageValidator.AddressCountyID
                    '                    AddressError = True
                    '                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSpecialEventCounty, v, accordList)
                    '            End Select
                    '        Next
                    '        ' If there were any address validations then leave the address section open, 
                    '        ' otherwise close it
                    '        If AddressError Then
                    '            OpenSpecialEventAddressSection()
                    '        Else
                    '            CollapseSpecialEventAddressSection()
                    '        End If
                    '    Else
                    '        ' If no validation items, collapse the address section
                    '        CollapseSpecialEventAddressSection()
                    '    End If
                    'End If
                    Exit Select
                Case Else
#If DEBUG Then
                    ' you have a type that was not handled for you need to fix this
                    Debugger.Break()
#End If
            End Select

        End If


        'Me.ValidateChildControls(valArgs) ' use a more targeted approach above to 

    End Sub

    Public Overrides Sub EffectiveDateChanged(NewEffectiveDate As String, OldEffectiveDate As String)
        Dim CyberEffDate As DateTime = Nothing
        If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing _
            AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then
            CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
        Else
            CyberEffDate = CDate("9/1/2020")
        End If

        If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))

        ' Family Cyber Protection, HO Enhancement 1019 and HO Plus Enhancement 1020 all have a crossover date of 9/1/2020.  
        If SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection _
            OrElse SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement _
            OrElse SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement _
            OrElse SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019 _
            OrElse SectionCoverageIEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020 Then

            ' There's no date crossing logic required for HO-4, it always uses the old coverages
            If CurrentFormTypeId = "25" OrElse Quote.Locations(0).StructureTypeId = "2" Then Exit Sub

            ' There's no date crossing logic required for Seasonal or Secondary occupancy, if one of these it always uses the old coverages
            If Quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then Exit Sub

            'Removed 10/6/2022 - can no longer cross the date line for CyberEffDate
            'Dim DateLineCrossed As Boolean = False
            'Dim CrossDirection As String = Nothing

            'If CDate(OldEffectiveDate) >= CyberEffDate AndAlso CDate(NewEffectiveDate) < CyberEffDate Then
            '    DateLineCrossed = True
            '    CrossDirection = "BACK"
            'ElseIf CDate(OldEffectiveDate) < CyberEffDate AndAlso CDate(NewEffectiveDate) >= CyberEffDate Then
            '    DateLineCrossed = True
            '    CrossDirection = "FORWARD"
            'End If

            Select Case SectionCoverageIEnum
'                Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection
'                    If DateLineCrossed Then
'                        If CrossDirection = "BACK" Then
'                            'Me.DeleteMySectionCoverage()
'cyberloop:
'                            ' Make sure we remove all cyber entries
'                            For Each sc As QuickQuote.CommonObjects.QuickQuoteSectionICoverage In MyLocation.SectionICoverages
'                                If sc.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection Then
'                                    MyLocation.SectionICoverages.Remove(sc)
'                                    GoTo cyberloop
'                                End If
'                            Next
'                        Else
'                            ' When crossing forward to a post-cyber date, we don't need to do anything as the coverage will automatically be checked on the page  --MB
'                            ' Above is incorrect. If cyber isn't in the coverages list it won't be checked on a date jump. The selection logic is skipped. --CAH

'                            Dim newcov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
'                            newcov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
'                            newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection
'                            newcov.Address = New QuickQuoteAddress()
'                            newcov.IncludedLimit = "50,000"
'                            newcov.IncreasedLimitId = "22"
'                            newcov.Address.StateId = "0"
'                            Me.MyLocation.SectionICoverages.CreateIfNull()
'                            Me.MyLocation.SectionICoverages.Add(newcov)
'                        End If
'                    End If
'                    Exit Select
'                Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019, QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement
'                    If DateLineCrossed Then
'                        'Me.DeleteMySectionCoverage()    ' Remove 1010
'HOloop:
'                        ' Remove all 1019's, 1010's, and Water Backup - Coverage codes 80570, 80057, and 144
'                        For Each sc As QuickQuote.CommonObjects.QuickQuoteSectionICoverage In MyLocation.SectionICoverages
'                            If sc.CoverageCodeId = "80570" OrElse sc.CoverageCodeId = "80057" OrElse sc.CoverageCodeId = "144" Then
'                                MyLocation.SectionICoverages.Remove(sc)
'                                GoTo HOloop
'                            End If
'                        Next

'                        If CrossDirection = "BACK" Then
'                            ' When crossing back to a pre cyber date (before 9/1/2020) from a post-cyber date and the quote has the HO enhancement 1019, remove the 1019 HO Enhancement and add the old 1010
'                            ' Add 1010 and water backup
'                            If Me.chkCov.Checked Then
'                                Dim newcov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
'                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement
'                                newcov.Address = New QuickQuoteAddress()
'                                newcov.Address.StateId = "0"
'                                Me.MyLocation.SectionICoverages.CreateIfNull()
'                                Me.MyLocation.SectionICoverages.Add(newcov)

'                                newcov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
'                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains
'                                newcov.Address = New QuickQuoteAddress()
'                                newcov.Address.StateId = "0"
'                                Me.MyLocation.SectionICoverages.CreateIfNull()
'                                Me.MyLocation.SectionICoverages.Add(newcov)
'                            End If
'                        Else
'                            ' When crossing forward to a post-cyber date from a pre-cyber date and the quote has the HO enhancement 1010, remove the 1010 enhancement and add the 1019
'                            ' Add 1019 and water backup
'                            If Me.chkCov.Checked Then
'                                Dim newcov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
'                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019
'                                newcov.Address = New QuickQuoteAddress()
'                                newcov.Address.StateId = "0"
'                                Me.MyLocation.SectionICoverages.CreateIfNull()
'                                Me.MyLocation.SectionICoverages.Add(newcov)

'                                newcov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
'                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains
'                                newcov.Address = New QuickQuoteAddress()
'                                newcov.Address.StateId = "0"
'                                Me.MyLocation.SectionICoverages.CreateIfNull()
'                                Me.MyLocation.SectionICoverages.Add(newcov)
'                            End If
'                        End If
'                    End If
'                    Exit Select
                Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020, QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement
                    '                    If DateLineCrossed Then
                    '                        'Me.DeleteMySectionCoverage()    ' Remove 1020
                    'HOPloop:
                    '                        ' Remove all 1020's, 1017's and water damage - coverage codes 80571, 80508, and 80520
                    '                        For Each sc As QuickQuote.CommonObjects.QuickQuoteSectionICoverage In MyLocation.SectionICoverages
                    '                            If sc.CoverageCodeId = "80571" OrElse sc.CoverageCodeId = "80508" OrElse sc.CoverageCodeId = "80520" Then
                    '                                MyLocation.SectionICoverages.Remove(sc)
                    '                                GoTo HOPloop
                    '                            End If
                    '                        Next
                    '                        If CrossDirection = "BACK" Then
                    '                            ' When crossing back to a pre cyber date (before 9/1/2020) from a post-cyber date and the quote has the HO+ enhancement 1020, remove the 1020 HO+ Enhancement and add the old 1017
                    '                            ' Add 1017 & water damage
                    '                            If Me.chkCov.Checked Then
                    '                                Dim newcov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                    '                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement
                    '                                newcov.Address = New QuickQuoteAddress()
                    '                                newcov.Address.StateId = "0"
                    '                                Me.MyLocation.SectionICoverages.CreateIfNull()
                    '                                Me.MyLocation.SectionICoverages.Add(newcov)

                    '                                newcov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                    '                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage
                    '                                newcov.Address = New QuickQuoteAddress()
                    '                                newcov.IncludedLimit = "5,000"
                    '                                newcov.Address.StateId = "0"
                    '                                Me.MyLocation.SectionICoverages.CreateIfNull()
                    '                                Me.MyLocation.SectionICoverages.Add(newcov)
                    '                            End If
                    '                        Else
                    '                            ' When crossing forward to a post-cyber date from a pre-cyber date and the quote has the HO+ enhancement 1017, remove the HO+ 1017 enhancement and add the  HO+ 1020
                    '                            ' Add 1020 & water damage
                    '                            If chkCov.Checked Then
                    '                                Dim newcov As New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                    '                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020
                    '                                newcov.Address = New QuickQuoteAddress()
                    '                                newcov.Address.StateId = "0"
                    '                                Me.MyLocation.SectionICoverages.CreateIfNull()
                    '                                Me.MyLocation.SectionICoverages.Add(newcov)

                    '                                newcov = New QuickQuote.CommonObjects.QuickQuoteSectionICoverage()
                    '                                newcov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage
                    '                                newcov.Address = New QuickQuoteAddress()
                    '                                newcov.IncludedLimit = "5,000"
                    '                                newcov.Address.StateId = "0"
                    '                                Me.MyLocation.SectionICoverages.CreateIfNull()
                    '                                Me.MyLocation.SectionICoverages.Add(newcov)
                    '                            End If
                    '                        End If
                    '                    End If                     

                    'Added 6/2/2022 for task 74147 MLW
                    Helpers.EffectiveDateHelper.CheckDateCrossing(Quote, NewEffectiveDate, OldEffectiveDate)

                    Exit Select
            End Select
        End If

        Exit Sub
    End Sub

    Private Sub btnSpecialEventOK_Click(sender As Object, e As EventArgs) Handles btnSpecialEventOK.Click
        Save_FireSaveEvent()
    End Sub

    Private Sub btnSpecialEventCancel_Click(sender As Object, e As EventArgs) Handles btnSpecialEventCancel.Click
        If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.Address IsNot Nothing Then
            ' Revert back to the saved address in the object if there is one
            txtSpecialEventStreetNumber.Text = MySectionCoverage.Address.HouseNum
            txtSpecialEventStreetName.Text = MySectionCoverage.Address.StreetName
            txtSpecialEventAptSuiteNumber.Text = MySectionCoverage.Address.ApartmentNumber
            txtSpecialEventZip.Text = MySectionCoverage.Address.Zip
            txtSpecialEventCity.Text = MySectionCoverage.Address.City
            ddlSpecialEventState.SetFromValue(MySectionCoverage.Address.StateId)
            txtSpecialEventCounty.Text = MySectionCoverage.Address.County
        Else
            ' if no address then just clear all of the address controls
            ClearSpecialEventAddressControls()
        End If
        ' Close the address section
        CollapseSpecialEventAddressSection()

        Exit Sub
    End Sub

    Private Sub ClearSpecialEventAddressControls()
        txtSpecialEventStreetNumber.Text = ""
        txtSpecialEventStreetName.Text = ""
        txtSpecialEventAptSuiteNumber.Text = ""
        txtSpecialEventZip.Text = ""
        txtSpecialEventCity.Text = ""
        ddlSpecialEventState.SelectedIndex = -1
        txtSpecialEventCounty.Text = ""

        Exit Sub
    End Sub

    Private Sub CollapseSpecialEventAddressSection()
        trSpecialEventAddress.Attributes.Add("style", "display:none")
        ' Show the add/delete links when the section is closed
        lbSpecialEventAddAddress.Attributes.Add("style", "display:''")
        lbSpecialEventDeleteAddress.Attributes.Add("style", "display:''")


        ' Set the add button text 
        If SpecialEventCoverageHasAnyAddressData() Then
            lbSpecialEventAddAddress.Text = "View/Edit Address"
        Else
            lbSpecialEventAddAddress.Text = "Add Address"
        End If

        Exit Sub
    End Sub

    Private Sub OpenSpecialEventAddressSection()
        trSpecialEventAddress.Attributes.Add("style", "display:''")
        ' Hide the add/delete links while the section is open
        lbSpecialEventAddAddress.Attributes.Add("style", "display:none")
        lbSpecialEventDeleteAddress.Attributes.Add("style", "display:none")

        Exit Sub
    End Sub

    Private Function SpecialEventCoverageHasAnyAddressData() As Boolean
        If MySectionCoverage IsNot Nothing AndAlso MySectionCoverage.Address IsNot Nothing Then
            If MySectionCoverage.Address.HouseNum.Trim <> "" _
                OrElse MySectionCoverage.Address.StreetName.Trim <> "" _
                OrElse MySectionCoverage.Address.ApartmentNumber.Trim <> "" _
                OrElse MySectionCoverage.Address.Zip.Trim <> "" _
                OrElse MySectionCoverage.Address.City.Trim <> "" _
                OrElse MySectionCoverage.Address.StateId.Trim <> "" _
                OrElse MySectionCoverage.Address.County.Trim <> "" Then Return True
        End If
        Return False
    End Function

    Private Sub lbSpecialEventDeleteAddress_Click(sender As Object, e As EventArgs) Handles lbSpecialEventDeleteAddress.Click
        ClearSpecialEventAddressControls()
        Save_FireSaveEvent(False)
        lblHeader.Text = GetSpecialEventCheckboxLabelText()
        Exit Sub
    End Sub

    'Private Sub lbSpecialEventAddAddress_Click(sender As Object, e As EventArgs) Handles lbSpecialEventAddAddress.Click
    '    trSpecialEventAddress.Attributes.Add("style", "display:''")
    '    lbSpecialEventAddAddress.Attributes.Add("style", "display:none")
    '    lbSpecialEventDeleteAddress.Attributes.Add("style", "display:none")

    '    Exit Sub
    'End Sub

    'Added 11/1/2022 for bug 78012 MLW
    Private Function CheckSectionICoverageFromDiamond(shouldSetSectionICoverageDefaults As Boolean) As Boolean
        'For HO 0002 - Mobile Home Broad Form: FormTypeId 22 is HO2, StructureTypeId 2 is Mobile - Diamond adds Inflation Guard at Property Save instead of rate which then skips the defaulting since a SectionICoverage is found, so need to check that only the VR available coverages are in the list in order to default coverages
        If MyLocation.SectionICoverages IsNot Nothing AndAlso MyLocation.SectionICoverages.Count > 0 Then
            shouldSetSectionICoverageDefaults = True
            Dim skipNextCheck As Boolean = False

            'Primary
            Dim occ As Boolean = False
            If MyLocation.OccupancyCodeId IsNot Nothing AndAlso MyLocation.OccupancyCodeId = "4" OrElse MyLocation.OccupancyCodeId = "5" Then
                occ = True
            End If
            Dim supportedPrimaryCoverages = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetSupportedPrimaryCoverages(Quote.EffectiveDate, CurrentFormTypeId, occ, Quote.Locations(0).StructureTypeId, Quote.Locations(0).OccupancyCodeId, Quote.QuoteTransactionType, QQHelper.IntegerForString(Quote.VersionId), QQHelper.IntegerForString(Quote.RatingVersionId))
            If supportedPrimaryCoverages IsNot Nothing AndAlso supportedPrimaryCoverages.Count > 0 Then
                For Each supportedPrimaryCov In supportedPrimaryCoverages
                    If MyLocation.SectionICoverages.FindAll(Function(sc) sc.HOM_CoverageType = supportedPrimaryCov.SectionICoverageEnum).IsLoaded Then
                        shouldSetSectionICoverageDefaults = False
                        skipNextCheck = True
                        Exit For
                    End If
                Next
            End If

            'Section I
            Dim supportedSectionICoverages = GetSupportedSectionICoverages(Quote)
            If skipNextCheck = False AndAlso supportedSectionICoverages IsNot Nothing AndAlso supportedSectionICoverages.Count > 0 Then
                For Each supportedCov In supportedSectionICoverages
                    If MyLocation.SectionICoverages.FindAll(Function(sc) sc.HOM_CoverageType = supportedCov).IsLoaded Then
                        shouldSetSectionICoverageDefaults = False
                        Exit For
                    End If
                Next
            End If
        Else
            shouldSetSectionICoverageDefaults = True
        End If
        Return shouldSetSectionICoverageDefaults
    End Function

End Class


