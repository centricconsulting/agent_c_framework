Imports System.Globalization
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CAP
Imports IFM.VR.Common.Helpers.MultiState.General
Imports IFM.VR.Web.Helpers
Imports PopupMessageClass
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class ctl_CAP_Vehicle
    Inherits VRControlBase

    Public ReadOnly CAPEndorsementsDictionaryName = "CAPEndorsementsDetails" 'Added 04/01/2021 for CAP Endorsements Task 52974 MLW

    Public Event NeedToRepopulateTopLevelAIs() 'added 6/14/2021 for CAP Endorsements Task 52974 MLW

    'Added 04/01/2021 for CAP Endorsements Task 52974 MLW
    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(CAPEndorsementsDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, CAPEndorsementsDictionaryName, Quote.LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Public Property VehicleIndex As Int32
        Get
            Return ViewState.GetInt32("vs_VehicleIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_VehicleIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return Me.VehicleIndex
        End Get
    End Property

    Public ReadOnly Property ScrollToControlId As String
        Get
            If Me.txtVIN.Enabled = True Then
                Return Me.txtVIN.ClientID
            Else
                Return Me.txtVehicleYear.ClientID
            End If
        End Get
    End Property

    Public Event NewVehicleRequested()
    Public Event CopyVehicleRequested(ByRef VehIndex As Integer)
    Public Event DeleteVehicleRequested(ByRef VehIndex As Integer)

    Dim VehicleIsValid As Boolean = False
    Dim _badCAPCodeMsg = "THIS CLASS CODE IS NOT ELIGIBLE FOR QUOTING IN VELOCIRATER. PLEASE CONTACT YOUR UNDERWRITER."

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Dim statestext As String = Nothing
        'If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
        If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
            statestext = "IL,IN,OH"
        Else
            statestext = "IL,IN"
        End If
        'Else
        '    Select Case Quote.QuickQuoteState
        '        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
        '            statestext = "IN"
        '            Exit Select
        '        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
        '            statestext = "IL"
        '            Exit Select
        '        Case Else
        '            Throw New Exception("CAP Vehicle Control: Unsupported state on quote!")
        '    End Select
        'End If
        ' now you will never have to touch this line of code again for the next state and it is 100% backward compatible
        'Dim statestext As String = String.Join(",", IFM.VR.Common.Helpers.States.GetStateAbbreviationsFromStateIds((From sq In Me.SubQuotes Select sq.StateId.TryToGetInt32())))

        ' Create all the section accordions
        'Updated 05/25/2021 for CAP Endorsements Task 52974 MLW
        Dim defaultIndexToShowExpandedAccordion As String = "0"
        If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" Then
            defaultIndexToShowExpandedAccordion = "false"
        End If
        Me.VRScript.CreateAccordion(divClassCodeLookup.ClientID, hdnCC, defaultIndexToShowExpandedAccordion, False)
        Me.VRScript.CreateAccordion(divGaragingAddress.ClientID, hdnGaraging, defaultIndexToShowExpandedAccordion, False)
        Me.VRScript.CreateAccordion(divVehicleCoverages.ClientID, hdnVehCov, defaultIndexToShowExpandedAccordion, False)
        'Me.VRScript.CreateAccordion(divClassCodeLookup.ClientID, hdnCC, "0", False)
        'Me.VRScript.CreateAccordion(divGaragingAddress.ClientID, hdnGaraging, "0", False)
        'Me.VRScript.CreateAccordion(divVehicleCoverages.ClientID, hdnVehCov, "0", False)
        Me.VRScript.CreateAccordion(Me.divVinLookup.ClientID, Me.hiddenVinLookup, "0") 'Added 07/01/2021 for CAP Endorsements Tasks 53028 and 53030 MLW

        ' Script buttons
        Me.VRScript.CreateConfirmDialog(Me.lnkDelete.ClientID, "Delete Vehicle?")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave_ClassCode.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave_Garaging.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave_VehCovs.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkNew.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkCopy.ClientID)

        ' Add script to coverage checkboxes
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            chkLiability.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('LIAB','" & chkLiability.ClientID & "', '', '', '', '');")
            chkUM.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('UM','" & chkUM.ClientID & "', '', '', '', '');")
            chkUM.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('UIM','" & chkUIM.ClientID & "', '', '', '', '');")
        Else
            chkLiabilityUMUIM.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('UMUIM','" & chkLiabilityUMUIM.ClientID & "', '', '', '', '');")
        End If
        chkMedicalPayments.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('MEDPAY','" & chkMedicalPayments.ClientID & "', '', '', '', '');")

        Dim comprehensiveOrCollisionChangedScript As String = ""
        If CAPCustomWrapOrPaintJobHelper.IsCustomWrapOrPaintJobAvailable(Quote) Then
            comprehensiveOrCollisionChangedScript = " Cap.ComprehensiveOrCollisionChanged('" & chkCollision.ClientID & "', '" & chkComprehensive.ClientID & "', '" & trCustomPaintJobOrWrapCollision.ClientID & "', '" & trCustomPaintJobOrWrapComprehensive.ClientID & "', '" & chkCustomPaintJobOrWrapCollision.ClientID & "', '" & chkCustomPaintJobOrWrapComprehensive.ClientID & "', '" & txtCostNew.ClientID & "', '" & hdnOriginalCostNew.ClientID & "');"
        End If

        chkComprehensive.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('COMP','" & chkComprehensive.ClientID & "', '" & trComprehensiveDeductibleRow.ClientID & "', '', '', '','" & chkRentalReimbursement.ClientID & "','" & chkTowingAndLabor.ClientID & "');" + comprehensiveOrCollisionChangedScript)
        chkCollision.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('COLL','" & chkCollision.ClientID & "', '" & trCollisionDeductibleRow.ClientID & "', '', '', '');" + comprehensiveOrCollisionChangedScript)

        chkRentalReimbursement.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('RENT','" & chkRentalReimbursement.ClientID & "', '" & trRentalReimbursementDataRow.ClientID & "', '', '', '');")
        chkTowingAndLabor.Attributes.Add("onchange", "Cap.CoverageCheckboxChanged('TOW','" & chkTowingAndLabor.ClientID & "', '', '', '', '');")

        'chkUseGaragingAddress.Attributes.Add("onchange", "Cap.UsePrimaryGaragingAddressCheckboxChanged('" & chkUseGaragingAddress.ClientID & "','" & txtZip.ClientID & "','" & txtCity.ClientID & "','" & txtCounty.ClientID & "')")

        'Variables for Class Code Lookup and Reverse Class Code Lookup (used in LoadUseTypeValues and bad CAP Code)
        'Added 2/10/2022 for bug 63488 MLW 
        Dim removeAntiqueAutoFlag As String = QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63488_CAPRemoveAntiqueAutoUseCode"))
        Dim myVehicleIsNewVehicleOnEndorsement As String = "False"
        If IsNewVehicleOnEndorsement(MyVehicle) Then
            myVehicleIsNewVehicleOnEndorsement = "True"
        End If
        Me.VRScript.AddVariableLine("var removeAntiqueAutoFlag ='" + removeAntiqueAutoFlag + "';")
        Me.VRScript.AddVariableLine("var myVehicleIsNewVehicleOnEndorsement ='" + myVehicleIsNewVehicleOnEndorsement + "';")

        ' Vehicle Class Code Lookup
        'Updated 07/22/2021 for CAP Endorsements VIN lookup tasks 53028 and 53030 - added hdnSize: stored vin lookup size value in hidden field to use as default when size not yet set
        'Updated 11/29/2021 for bug 66920 MLW - hdnValidVin
        ddlVehicleRatingType.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','VRT', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlUseCode.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','USE', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlOperatorType.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','OP', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlOperatorUse.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','OPTYP', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlSize.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','SIZE', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlTrailerType.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','TT', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlRadius.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','RD', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlSecondaryClass.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','SC', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")
        ddlSecondaryClassType.Attributes.Add("onchange", "Cap.LookupVehicleClassCode('" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','SCT', '" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & lblCostNew.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "');")

        ' Reverse Class Code lookup
        txtClassCode.Attributes.Add("onkeyup", "Cap.ReverseClassCodeLookup('" & txtClassCode.ClientID & "','" & lblClassCode.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlOperatorType.ClientID & "','" & ddlOperatorUse.ClientID & "','" & ddlSize.ClientID & "','" & ddlTrailerType.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & trUseCodeRow.ClientID & "','" & trOperatorTypeRow.ClientID & "','" & trOperatorUseRow.ClientID & "','" & trSizeRow.ClientID & "','" & trTrailerTypeRow.ClientID & "','" & trRadiusRow.ClientID & "','" & trSecondaryClassRow.ClientID & "','" & trSecondaryClassTypeRow.ClientID & "','" & chkDumpingOperations.ClientID & "','" & chkSeasonalFarmUse.ClientID & "','" & divDumping.ClientID & "','" & divSeasonalFarmUse.ClientID & "');")

        ' UMPD
        chkUMPD.Attributes.Add("onchange", "Cap.UMPDCheckboxChanged('" & chkUMPD.ClientID & "','" & trUMPDLimitRow.ClientID & "')")

        ' Zip code lookup
        Dim ILUMPDLimit As String = String.Empty
        If SubQuoteFirst IsNot Nothing Then
            ILUMPDLimit = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
        End If
        txtZip.Attributes.Add("onkeyup", "Cap.DoMultiStateCityCountyLookup('" + Me.txtZip.ClientID + "','" + Me.txtCity.ClientID + "','" + Me.txtState.ClientID & "','" + Me.txtCounty.ClientID + "','" & statestext & "','" & trUMPDRow.ClientID & "','" & trUMPDLimitRow.ClientID & "','" & chkUMPD.ClientID & "','" & txtUMPDLimit.ClientID & "','" & UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote).ToString() & "','" & Quote.QuickQuoteState.ToString() & "','" & ILUMPDLimit & "');")

        ' Add Bad Class Code List and Text
        Me.VRScript.AddVariableLine("var badCAPCodeMsg='" + _badCAPCodeMsg + "';")

        'Added 07/01/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
        ' VIN Lookup
        Dim versionId As String = Quote.VersionId
        Dim policyId As String = Quote.PolicyId
        Dim policyImageNumber As String = Quote.PolicyImageNum
        Dim vehicleNum As String = "0"
        If MyVehicle IsNot Nothing Then
            vehicleNum = MyVehicle.VehicleNum
        End If
        Dim isRACASymbolAvailable As Boolean = RACASymbolHelper.IsRACASymbolsAvailable(Quote)
        btnVinLookup.Attributes.Add("onclick", "Cap.SetupVinSearch('" & txtVIN.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & txtVehicleYear.ClientID & "','" & txtVehicleModel.ClientID & "','" & divVinLookup.ClientID & "','" & divVinLookupContents.ClientID & "','" & ddlUseCode.ClientID & "','" & ddlSize.ClientID & "','" & hdnSize.ClientID & "','" & ddlRadius.ClientID & "','" & ddlSecondaryClass.ClientID & "','" & ddlSecondaryClassType.ClientID & "','" & txtCostNew.ClientID & "','" & policyId & "','" & policyImageNumber & "','" & vehicleNum & "','" & versionId & "','" & divVinLookupValidation.ClientID & "','" & hdnValidVin.ClientID & "','" & lblClassCode.ClientID & "','" & txtClassCode.ClientID & "','" & ddlVehicleRatingType.ClientID & "','" & hdnOriginalCostNew.ClientID & "','" & chkCustomPaintJobOrWrapComprehensive.ClientID & "','" & chkCustomPaintJobOrWrapCollision.ClientID & "','" & isRACASymbolAvailable.ToString() & "','" & hdnOtherThanCollisionSymbol.ClientID & "','" & hdnCollisionSymbol.ClientID & "','" & hdnLiabilitySymbol.ClientID & "','" & hdnOtherThanCollisionOverride.ClientID & "','" & hdnCollisionOverride.ClientID & "','" & hdnLiabilityOverride.ClientID & "');$(""#" + HiddenLookupWasFired.ClientID + """).val('1'); return false;")
        ' The OK button on the VIN Lookup Validation popup
        btnVLVOK.Attributes.Add("onclick", "Cap.CloseVINValidationPopup('" & divVinLookupValidation.ClientID & "'); return false;")
        If IsQuoteEndorsement() Then
            'Only clear out lookup values on endorsement when VIN, year, make, or model changes (except for trailer model since it comes back blank on VIN lookup)
            txtVIN.Attributes.Add("onchange", "Cap.ClearVINLookupFields('" & txtVIN.ClientID & "','" & txtVIN.ClientID & "','" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & txtCostNew.ClientID & "','" & ddlSize.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "','" & lblClassCode.ClientID & "','" & txtClassCode.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            txtVehicleYear.Attributes.Add("onchange", "Cap.ClearVINLookupFields('" & txtVehicleYear.ClientID & "','" & txtVIN.ClientID & "','" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & txtCostNew.ClientID & "','" & ddlSize.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "','" & lblClassCode.ClientID & "','" & txtClassCode.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            txtVehicleMake.Attributes.Add("onchange", "Cap.ClearVINLookupFields('" & txtVehicleMake.ClientID & "','" & txtVIN.ClientID & "','" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & txtCostNew.ClientID & "','" & ddlSize.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "','" & lblClassCode.ClientID & "','" & txtClassCode.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            txtVehicleModel.Attributes.Add("onchange", "Cap.ClearVINLookupFields('" & txtVehicleModel.ClientID & "','" & txtVIN.ClientID & "','" & txtVehicleYear.ClientID & "','" & txtVehicleMake.ClientID & "','" & txtVehicleModel.ClientID & "','" & txtCostNew.ClientID & "','" & ddlSize.ClientID & "','" & hdnSize.ClientID & "','" & hdnValidVin.ClientID & "','" & lblClassCode.ClientID & "','" & txtClassCode.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
        Else
            'NB quote side, enable size and cost new when VIN changes or year/make/model changes (except for trailer model since it comes back blank on VIN lookup)
            txtVIN.Attributes.Add("onchange", "Cap.EnableLookupFields('" & txtVIN.ClientID & "','" & txtVIN.ClientID & "','" & ddlSize.ClientID & "','" & txtCostNew.ClientID & "','" & hdnValidVin.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            txtVehicleYear.Attributes.Add("onchange", "Cap.EnableLookupFields('" & txtVehicleYear.ClientID & "','" & txtVIN.ClientID & "','" & ddlSize.ClientID & "','" & txtCostNew.ClientID & "','" & hdnValidVin.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            txtVehicleMake.Attributes.Add("onchange", "Cap.EnableLookupFields('" & txtVehicleMake.ClientID & "','" & txtVIN.ClientID & "','" & ddlSize.ClientID & "','" & txtCostNew.ClientID & "','" & hdnValidVin.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            txtVehicleModel.Attributes.Add("onchange", "Cap.EnableLookupFields('" & txtVehicleModel.ClientID & "','" & txtVIN.ClientID & "','" & ddlSize.ClientID & "','" & txtCostNew.ClientID & "','" & hdnValidVin.ClientID & "','" & ddlVehicleRatingType.ClientID & "');")
            'Added 11/29/2021 for bug 66920 MLW
            'if class code begins with 6 (i.e. trailer) and the VIN was entered, allow the user to continue to rate by setting the valid vin flag to true.
            Me.VRScript.AddScriptLine("Cap.ForceVehicleValidVIN('" & hdnValidVin.ClientID & "','" & txtClassCode.ClientID & "');")
        End If

        'Check to see if the vehicle has a custom paint job or wrap. If it does add 5000 to the vehicle's cost new.
        If CAPCustomWrapOrPaintJobHelper.IsCustomWrapOrPaintJobAvailable(Quote) Then
            Dim increaseVehicleCostNewScript As String = "Cap.IncreaseVehicleCostNew('" & chkCustomPaintJobOrWrapCollision.ClientID & "','" & chkCustomPaintJobOrWrapComprehensive.ClientID & "','" & txtCostNew.ClientID & "','" & hdnOriginalCostNew.ClientID & "');"
            Dim handleCostNewChangeScript As String = "Cap.HandleCostNewChange('" & txtCostNew.ClientID & "','" & hdnOriginalCostNew.ClientID & "');"
            txtCostNew.Attributes.Add("onchange", handleCostNewChangeScript)
            chkCustomPaintJobOrWrapCollision.Attributes.Add("onchange", increaseVehicleCostNewScript)
            chkCustomPaintJobOrWrapComprehensive.Attributes.Add("onchange", increaseVehicleCostNewScript)
        End If
        Exit Sub
    End Sub



    Public Overrides Sub LoadStaticData()
        LoadCompCollStaticData()

        'Updated 02/17/2021 for CAP Endorsements Task 52974 MLW
        If Not IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso IsNewVehicleOnEndorsement(MyVehicle)) Then
            ' Set Collision Default
            For Each li As ListItem In ddlCollisionDeductible.Items
                If li.Text = "500" Then
                    li.Selected = True
                    Exit For
                End If
            Next
        End If

        'Added 2/7/2022 for bug 63488 MLW
        If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63488_CAPRemoveAntiqueAutoUseCode")) = True Then
            If Not IsEndorsementRelated() OrElse (IsQuoteEndorsement() AndAlso IsNewVehicleOnEndorsement(MyVehicle)) Then
                Dim removeItem = (From i As ListItem In Me.ddlUseCode.Items Where i.Value = "22" Select i).FirstOrDefault()
                If removeItem IsNot Nothing Then
                    Me.ddlUseCode.Items.Remove(removeItem)
                End If
            End If
        End If

        'Dim startingStateValue = Me.ddState.SelectedValue
        'Me.ddState.Items.Clear()
        'Me.ddState.Items.Add(New ListItem("", ""))
        'For Each sq In SubQuotes
        '    Dim stateInfo = IFM.VR.Common.Helpers.States.GetStateInfoFromId(sq.StateId.TryToGetInt32())
        '    Me.ddState.Items.Add(New ListItem(stateInfo.Abbreviation, stateInfo.StateId))
        'Next
        'Me.ddState.SetFromValue(startingStateValue)

    End Sub

    Public Sub LoadCompCollStaticData()
        ' Comprehensive ddl
        QQHelper.LoadStaticDataOptionsDropDown(ddlComprehensiveDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleId, , Quote.LobType)

        ' Collision ddl
        QQHelper.LoadStaticDataOptionsDropDown(ddlCollisionDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleId, , Quote.LobType)

        If CompCollDeductibleHelper.IsCompCollDeductibleAvailable(Quote) Then
            Dim removeComp250 As ListItem = ddlComprehensiveDeductible.Items.FindByText("250")
            If removeComp250 IsNot Nothing Then
                ddlComprehensiveDeductible.Items.Remove(removeComp250)
            End If
            Dim removeColl250 As ListItem = ddlCollisionDeductible.Items.FindByText("250")
            If removeColl250 IsNot Nothing Then
                ddlCollisionDeductible.Items.Remove(removeColl250)
            End If
        End If
    End Sub

    Private Sub UpdateAccordHeader()
        If MyVehicle IsNot Nothing Then
            ' Update header
            'Updated 05/05/2021 for CAP Endorsements bug 61606 MLW
            Dim txt As String
            If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                Dim lastFourVin As String = MyVehicle.Vin.ToUpper
                If Not String.IsNullOrWhiteSpace(MyVehicle.Vin) AndAlso MyVehicle.Vin.Length > 4 Then
                    lastFourVin = MyVehicle.Vin.Substring(MyVehicle.Vin.Length - 4).ToUpper()
                End If
                txt = MyVehicle.Year & " " & MyVehicle.Make.ToUpper & " " & MyVehicle.Model.ToUpper & " " & lastFourVin
                If IsNullEmptyorWhitespace(txt) Then
                    txt = "0"
                End If
                If txt.Length > 37 Then txt = txt.Substring(0, 37) & "..."
                lblAccordHeader.ToolTip = MyVehicle.Vin.ToUpper
            Else
                txt = "Vehicle #" & VehicleIndex + 1.ToString & " - " & MyVehicle.Year & " " & MyVehicle.Make.ToUpper & " " & MyVehicle.Model.ToUpper
                If txt.Length > 27 Then txt = txt.Substring(0, 27) & "..."
            End If
            'Dim txt As String = "Vehicle #" & VehicleIndex + 1.ToString & " - " & MyVehicle.Year & " " & MyVehicle.Make.ToUpper & " " & MyVehicle.Model.ToUpper
            'If txt.Length > 27 Then txt = txt.Substring(0, 27) & "..."
            lblAccordHeader.Text = txt
        End If
    End Sub

    Public Overrides Sub Populate()

        ' Use popup library to show the Use Code dialog
        ' moved here for bug 74284 04/06/22
        Dim msg As String = "<b>Commercial</b> - All vehicles that do not fit in the Service or Retail classes.  This should be your default classification. <br />"
        msg += "<b>Service</b> - For contractors and other businesses where vehicles are primarily parked at a job site location most of the day. <br />"
        msg += "<b>Retail</b> - Vehicles used to pick up or deliver property to individual households."
        Using popup As New PopupMessageObject(Me.Page, msg, "Use Code Info")
            With popup
                .ControlEvent = PopupMessageObject.ControlEvents.onmouseup
                .BindScript = PopupMessageObject.BindTo.Control
                .isFixedPositionOnScreen = True
                .ZIndexOfPopup = 2
                .isModal = True
                .Image = PopupMessageObject.ImageOptions.None
                .hideCloseButton = True
                .AddButton("OK", True)
                .AddControlToBindTo(lbUseCode)
                .CreateDynamicPopUpWindow()
            End With
        End Using

        'Added 12/23/2020 for CAP Endorsements Task 52974 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso (TypeOfEndorsement() = "Add/Delete Vehicle" OrElse TypeOfEndorsement() = "Add/Delete Additional Interest")) Then

            Dim title As String = Nothing
            Dim titleLen As Integer = 40

            LoadStaticData()

            If MyVehicle IsNot Nothing Then
                ' Hide the Dumping Operations and Seasonal Farm Use checkboxes
                divDumping.Attributes.Add("style", "display:none")
                divSeasonalFarmUse.Attributes.Add("style", "display:none")

                ' Reset & Hide all the lookup ddl's & rows
                ddlVehicleRatingType.SelectedIndex = -1
                ddlUseCode.SelectedIndex = -1
                trUseCodeRow.Attributes.Add("style", "display:none")
                ddlOperatorType.SelectedIndex = -1
                trOperatorTypeRow.Attributes.Add("style", "display:none")
                ddlOperatorUse.SelectedIndex = -1
                trOperatorUseRow.Attributes.Add("style", "display:none")
                ddlSize.SelectedIndex = -1
                trSizeRow.Attributes.Add("style", "display:none")
                ddlTrailerType.SelectedIndex = -1
                trTrailerTypeRow.Attributes.Add("style", "display:none")
                ddlRadius.SelectedIndex = -1
                trRadiusRow.Attributes.Add("style", "display:none")
                ddlSecondaryClass.SelectedIndex = -1
                trSecondaryClassRow.Attributes.Add("style", "display:none")
                ddlSecondaryClassType.SelectedIndex = -1
                trSecondaryClassTypeRow.Attributes.Add("style", "display:none")
                txtClassCode.Text = ""
                lblClassCode.Text = ""
                ' Hide the UMPD rows
                trUMPDRow.Attributes.Add("style", "display:none")
                trUMPDLimitRow.Attributes.Add("style", "display:none")

                ' Update header
                UpdateAccordHeader()

                ' *** GENERAL INFO FIELDS
                txtVehicleYear.Text = MyVehicle.Year
                txtVehicleMake.Text = MyVehicle.Make
                txtVehicleModel.Text = MyVehicle.Model
                txtCostNew.Text = MyVehicle.CostNew
                txtClassCode.Text = MyVehicle.ClassCode
                txtVIN.Text = MyVehicle.Vin 'Added 07/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
                'Added 07/26/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
                hdnValidVin.Value = GetValidVinValue(VehicleIndex)
                If hdnValidVin.Value.ToUpper() = "TRUE" Then
                    If (Not IsNullEmptyorWhitespace(MyVehicle.CostNew) AndAlso MyVehicle.CostNew <> "0" AndAlso MyVehicle.CostNew <> "$0.00") Then
                        'Updated 11/30/2021 for bug 66920 MLW - always allow trailer (i.e. class code begins with 6) cost new to be editable
                        If Not IsNullEmptyorWhitespace(txtClassCode.Text) AndAlso txtClassCode.Text.Length > 4 AndAlso Left(txtClassCode.Text, 1) = "6" Then 'do not disable trailer cost new, they can always change it regardless of using the VIN lookup since the VIN lookup returns 0 for trailers.
                            txtCostNew.Attributes.Remove("disabled")
                        Else
                            txtCostNew.Attributes.Add("disabled", "true")
                        End If
                        'txtCostNew.Attributes.Add("disabled", "true")
                    Else
                        txtCostNew.Attributes.Remove("disabled")
                    End If
                    If Not IsNullEmptyorWhitespace(MyVehicle.SizeTypeId) Then
                        ddlSize.Attributes.Add("disabled", "true")
                    End If
                End If

                ' Set defaults
                ddlComprehensiveDeductible.SelectedValue = "8"
                ddlCollisionDeductible.SelectedValue = "8"
                txtNumberOfDays.Text = "30"
                txtDailyReimbursement.Text = "30"

                ' *** CLASS CODE LOOKUP
                If MyVehicle.ClassCode IsNot Nothing AndAlso MyVehicle.ClassCode.Trim <> "" Then
                    Dim vcc As IFM.VR.Common.Helpers.VehicleClassCodeLookup.ClassCodeLookupFields_Structure = Nothing

                    'Updated 02/17/2021 for CAP Endorsements Task 52974 MLW
                    If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                        vcc = New Common.Helpers.VehicleClassCodeLookup.ClassCodeLookupFields_Structure
                        vcc.VehicleRatingTypeId = MyVehicle.VehicleRatingTypeId
                        vcc.UseCodeId = MyVehicle.UseCodeTypeId
                        vcc.OperatorTypeId = MyVehicle.OperatorTypeId
                        vcc.OperatorUseId = MyVehicle.OperatorUseTypeId
                        vcc.SizeId = MyVehicle.SizeTypeId
                        vcc.TrailerTypeId = MyVehicle.TrailerTypeId
                        vcc.RadiusId = MyVehicle.RadiusTypeId
                        vcc.SecondaryClassId = MyVehicle.SecondaryClassTypeId
                        vcc.SecondaryClassTypeId = MyVehicle.SecondaryClassUsageTypeId
                        vcc.DumpingOperations = MyVehicle.UsedInDumping
                        If MyVehicle.FarmUseCodeTypeId = "3" Then
                            vcc.SeasonalFarmUse = True
                        Else
                            vcc.SeasonalFarmUse = False
                        End If
                    Else
                        ' Reverse class code lookup
                        vcc = IFM.VR.Common.Helpers.VehicleClassCodeLookup.ReverseClassCodeLookup(MyVehicle.ClassCode)
                    End If


                    ' If we got something back from the reverse class code lookup, display the lookup fields
                    If Not (vcc.Equals(New IFM.VR.Common.Helpers.VehicleClassCodeLookup.ClassCodeLookupFields_Structure())) Then
                        'Added 02/16/2021 for CAP Endorsements Task 52974 MLW
                        If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                            ddlVehicleRatingType.Items.Add(New ListItem("N/A", "0"))
                            ddlVehicleRatingType.Items.Add(New ListItem("Public Auto", "21"))
                            ddlVehicleRatingType.Items.Add(New ListItem("Mobile Equipment", "22"))
                            ddlVehicleRatingType.Items.Add(New ListItem("Mobile Home", "3"))
                            ddlVehicleRatingType.Items.Add(New ListItem("Snowmobile", "15"))
                            ddlVehicleRatingType.Items.Add(New ListItem("Motorcycle", "16"))
                            ddlVehicleRatingType.Items.Add(New ListItem("Funeral Director", "7"))
                            ddlOperatorType.Items.Add(New ListItem("N/A", "0"))
                            ddlOperatorUse.Items.Add(New ListItem("N/A", "0"))

                            ddlVehicleRatingType.SelectedValue = vcc.VehicleRatingTypeId
                            LoadUseCodeDDL()
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlUseCode, vcc.UseCodeId)
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlOperatorType, vcc.OperatorTypeId)
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlOperatorUse, vcc.OperatorUseId)
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlSize, vcc.SizeId)
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlTrailerType, vcc.TrailerTypeId)
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlRadius, vcc.RadiusId)
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlSecondaryClass, vcc.SecondaryClassId)
                            If vcc.VehicleRatingTypeId = "9" AndAlso vcc.RadiusId Then
                                LoadSecondaryClassTypeDDL()
                            End If
                            checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlSecondaryClassType, vcc.SecondaryClassTypeId)

                            If (vcc.VehicleRatingTypeId = "9" AndAlso vcc.SizeId <> "21") OrElse vcc.VehicleRatingTypeId = "1" Then
                                trUseCodeRow.Attributes.Add("style", "display:''")
                            End If
                            If vcc.VehicleRatingTypeId = "1" AndAlso vcc.UseCodeId = "21" Then
                                trOperatorTypeRow.Attributes.Add("style", "display:''")
                                trOperatorUseRow.Attributes.Add("style", "display:''")
                            End If
                            If vcc.VehicleRatingTypeId = "9" Then
                                trSizeRow.Attributes.Add("style", "display:''")
                            End If
                            If vcc.VehicleRatingTypeId = "9" AndAlso (vcc.SizeId = "0" OrElse vcc.SizeId = "18" OrElse vcc.SizeId = "19" OrElse vcc.SizeId = "20" OrElse vcc.SizeId = "22" OrElse vcc.SizeId = "23") Then
                                trRadiusRow.Attributes.Add("style", "display:''")
                            End If
                            If vcc.VehicleRatingTypeId = "9" AndAlso vcc.SizeId = "30" Then
                                trTrailerTypeRow.Attributes.Add("style", "display:''")
                            End If
                            If vcc.VehicleRatingTypeId = "9" AndAlso vcc.TrailerTypeId Then
                                trRadiusRow.Attributes.Add("style", "display:''")
                            End If
                            If vcc.VehicleRatingTypeId = "9" AndAlso vcc.RadiusId Then
                                trSecondaryClassRow.Attributes.Add("style", "display:''")
                                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")
                            Else
                                trSecondaryClassTypeRow.Attributes.Add("style", "display:none")
                            End If
                        Else
                            ' Rating Type
                            If Not checkValid_Data_and_DDLListItem_andSelect(ddlVehicleRatingType, vcc.VehicleRatingTypeId) Then
                                ddlVehicleRatingType.SelectedIndex = -1
                            End If
                            ' Use Code
                            If vcc.UseCodeId IsNot Nothing AndAlso vcc.UseCodeId <> "" Then
                                LoadUseCodeDDL()
                                If checkValid_Data_and_DDLListItem_andSelect(ddlUseCode, vcc.UseCodeId) Then
                                    'do nothing
                                End If
                            End If

                            ' Operator
                            If checkValid_Data_and_DDLListItem_andSelect(ddlOperatorType, vcc.OperatorTypeId) Then
                                trOperatorTypeRow.Attributes.Add("style", "display:''")
                            End If
                            ' Operator Use
                            If checkValid_Data_and_DDLListItem_andSelect(ddlOperatorUse, vcc.OperatorUseId) Then
                                trOperatorTypeRow.Attributes.Add("style", "display:''")
                            End If
                            ' Size
                            If checkValid_Data_and_DDLListItem_andSelect(ddlSize, vcc.SizeId) Then
                                trSizeRow.Attributes.Add("style", "display:''")
                            End If
                            ' Trailer Type
                            If checkValid_Data_and_DDLListItem_andSelect(ddlTrailerType, vcc.TrailerTypeId) Then
                                trTrailerTypeRow.Attributes.Add("style", "display:''")
                            End If
                            ' Radius
                            If checkValid_Data_and_DDLListItem_andSelect(ddlRadius, vcc.RadiusId) Then
                                trRadiusRow.Attributes.Add("style", "display:''")
                            End If
                            ' Secondary Class
                            If checkValid_Data_and_DDLListItem_andSelect(ddlSecondaryClass, vcc.SecondaryClassId) Then
                                trSecondaryClassRow.Attributes.Add("style", "display:''")
                            End If
                            ' Secondary Class Type
                            If vcc.SecondaryClassTypeId IsNot Nothing AndAlso vcc.SecondaryClassTypeId <> "" Then
                                LoadSecondaryClassTypeDDL()
                                If checkValid_Data_and_DDLListItem_andSelect(ddlSecondaryClassType, vcc.SecondaryClassTypeId) Then
                                    ' do nothing
                                End If
                            End If
                        End If

                        '' Rating Type
                        'If Not checkValid_Data_and_DDLListItem_andSelect(ddlVehicleRatingType, vcc.VehicleRatingTypeId) Then
                        '    ddlVehicleRatingType.SelectedIndex = -1
                        'End If
                        '' Use Code
                        'If vcc.UseCodeId IsNot Nothing AndAlso vcc.UseCodeId <> "" Then
                        '    LoadUseCodeDDL()
                        '    If checkValid_Data_and_DDLListItem_andSelect(ddlUseCode, vcc.UseCodeId) Then
                        '        'do nothing
                        '    End If
                        'End If

                        '' Operator
                        'If checkValid_Data_and_DDLListItem_andSelect(ddlOperatorType, vcc.OperatorTypeId) Then
                        '    trOperatorTypeRow.Attributes.Add("style", "display:''")
                        'End If
                        '' Operator Use
                        'If checkValid_Data_and_DDLListItem_andSelect(ddlOperatorUse, vcc.OperatorUseId) Then
                        '    trOperatorTypeRow.Attributes.Add("style", "display:''")
                        'End If
                        '' Size
                        'If checkValid_Data_and_DDLListItem_andSelect(ddlSize, vcc.SizeId) Then
                        '    trSizeRow.Attributes.Add("style", "display:''")
                        'End If
                        '' Trailer Type
                        'If checkValid_Data_and_DDLListItem_andSelect(ddlTrailerType, vcc.TrailerTypeId) Then
                        '    trTrailerTypeRow.Attributes.Add("style", "display:''")
                        'End If
                        '' Radius
                        'If checkValid_Data_and_DDLListItem_andSelect(ddlRadius, vcc.RadiusId) Then
                        '    trRadiusRow.Attributes.Add("style", "display:''")
                        'End If
                        '' Secondary Class
                        'If checkValid_Data_and_DDLListItem_andSelect(ddlSecondaryClass, vcc.SecondaryClassId) Then
                        '    trSecondaryClassRow.Attributes.Add("style", "display:''")
                        'End If
                        '' Secondary Class Type
                        'If vcc.SecondaryClassTypeId IsNot Nothing AndAlso vcc.SecondaryClassTypeId <> "" Then
                        '    LoadSecondaryClassTypeDDL()
                        '    If checkValid_Data_and_DDLListItem_andSelect(ddlSecondaryClassType, vcc.SecondaryClassTypeId) Then
                        '        ' do nothing
                        '    End If
                        'End If

                        'Added 08/24/2021 for Bug 64448 MLW
                        If ddlSize.SelectedValue = "21" OrElse ddlSize.SelectedValue = "23" OrElse ddlSize.SelectedValue = "30" Then
                            'Don't show use code for size 21 - Extra Heavy Truck, size 23 - Extra Heavy Truck-Tractors and size 30 - Trailer Types
                            trUseCodeRow.Attributes.Add("style", "display:none")
                        End If

                        lblClassCode.Text = MyVehicle.ClassCode
                        txtClassCode.Text = MyVehicle.ClassCode

                        TestForDumpingOperations()
                        TestForSeasonalFarmUse()
                    End If
                Else
                    If hdnValidVin.Value.ToUpper() = "TRUE" AndAlso (MyVehicle.VehicleRatingTypeId = "1" OrElse MyVehicle.VehicleRatingTypeId = "9") Then
                        ddlVehicleRatingType.SelectedValue = MyVehicle.VehicleRatingTypeId
                        trUseCodeRow.Attributes.Add("style", "display:''")
                        If MyVehicle.VehicleRatingTypeId = "9" AndAlso Not IsNullEmptyorWhitespace(MyVehicle.SizeTypeId) Then
                            ddlSize.SelectedValue = MyVehicle.SizeTypeId
                        End If
                    End If
                End If

                If hdnValidVin.Value.ToUpper() = "TRUE" AndAlso (MyVehicle.VehicleRatingTypeId = "1" OrElse MyVehicle.VehicleRatingTypeId = "9") Then
                    For Each li As ListItem In Me.ddlVehicleRatingType.Items
                        '1=Private Passenger Type, 9=Truck, Trailer, Tractor
                        If MyVehicle.VehicleRatingTypeId = "9" AndAlso li.Value = "1" Then
                            'if Vehicle Rating Type is 9=TTT, disable 1=PPT option
                            li.Attributes.Add("disabled", "disabled")
                        ElseIf MyVehicle.VehicleRatingTypeId = "1" AndAlso li.Value = "9" Then
                            'if Vehicle Rating Type is 1=PPT, disable 9=TTT option
                            li.Attributes.Add("disabled", "disabled")
                        Else
                            li.Attributes.Remove("disabled")
                        End If
                    Next
                Else
                    For Each li As ListItem In Me.ddlVehicleRatingType.Items
                        li.Attributes.Remove("disabled")
                    Next
                End If

                If hdnValidVin.Value.ToUpper() = "TRUE" AndAlso Not IsNullEmptyorWhitespace(MyVehicle.SizeTypeId) Then
                    ddlSize.Attributes.Add("disabled", "true")
                Else
                    ddlSize.Attributes.Remove("disabled")
                End If

                ' *** GARAGING ADDRESS
                If MyVehicle.GaragingAddress IsNot Nothing AndAlso GaragingAddressHasValues() Then
                    ' The vehicle has a garaging address - display it
                    If GaragingAddressMatchesLocation0Address() Then
                        chkUseGaragingAddress.Checked = True
                        'chkUseGaragingAddress.Enabled = True
                        DisableGaragingFields()
                    Else
                        chkUseGaragingAddress.Checked = False
                        EnableGaragingFields()
                    End If
                    txtZip.Text = MyVehicle.GaragingAddress.Address.Zip
                    txtCity.Text = MyVehicle.GaragingAddress.Address.City
                    txtState.Text = MyVehicle.GaragingAddress.Address.State
                    txtCounty.Text = MyVehicle.GaragingAddress.Address.County
                Else
                    ' The vehicle does not have a garaging address - use either location 0 address or policyholder address
                    ' NOTE THAT THERE WILL ALWAYS BE A LOCATION NOW MGB 8-3-17
                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                        ' Location found - Use Garaging Address
                        PopulateGaragingFieldsFromLocationZero()
                    Else
                        ' No location found, use policyholder address & disable the use garaging address checkbox
                        PopulateGaragingFieldsFromPolicyHolder()
                    End If
                End If

                ' *** COVERAGES

                'added 8/11/2018
                Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = GetQuoteToUse()

                ' Liability UM/UIM and UMPD
                PopulateVehicleCoverages(quoteToUse)

                PopulateCompColl(quoteToUse)

                If CAPCustomWrapOrPaintJobHelper.IsCustomWrapOrPaintJobAvailable(Quote) Then
                    Dim originalCostNew As Decimal
                    Dim costNewIsDecimal = MyVehicle.CostNew IsNot Nothing AndAlso QuickQuoteHelperClass.TryParseStringToDecimal(MyVehicle.CostNew, originalCostNew)

                    If MyVehicle.HasCollision Then
                        If VehicleHasCustomWrapOrPaintJob(VehicleIndex) Then
                            chkCustomPaintJobOrWrapCollision.Checked = True
                            If costNewIsDecimal Then
                                hdnOriginalCostNew.Value = If(originalCostNew >= 5000, (originalCostNew - 5000).ToString(), originalCostNew.ToString())
                            End If
                        Else
                            If costNewIsDecimal Then
                                hdnOriginalCostNew.Value = originalCostNew.ToString()
                            End If
                        End If
                        trCustomPaintJobOrWrapCollision.Attributes.Add("style", "display:''")
                        trCustomPaintJobOrWrapComprehensive.Attributes.Add("style", "display:none")
                    ElseIf MyVehicle.HasComprehensive Then
                        If VehicleHasCustomWrapOrPaintJob(VehicleIndex) Then
                            chkCustomPaintJobOrWrapComprehensive.Checked = True
                            If costNewIsDecimal Then
                                hdnOriginalCostNew.Value = If(originalCostNew >= 5000, (originalCostNew - 5000).ToString(), originalCostNew.ToString())
                            End If
                        Else
                            If costNewIsDecimal Then
                                hdnOriginalCostNew.Value = originalCostNew.ToString()
                            End If
                        End If
                        trCustomPaintJobOrWrapComprehensive.Attributes.Add("style", "display:''")
                        trCustomPaintJobOrWrapCollision.Attributes.Add("style", "display:none")
                    Else
                        If costNewIsDecimal Then
                            hdnOriginalCostNew.Value = originalCostNew.ToString()
                        End If
                        trCustomPaintJobOrWrapComprehensive.Attributes.Add("style", "display:none")
                        trCustomPaintJobOrWrapCollision.Attributes.Add("style", "display:none")
                    End If
                End If

                'If Quote.HasBusinessMasterEnhancement Then
                'updated 8/11/2018
                If quoteToUse IsNot Nothing AndAlso quoteToUse.HasBusinessMasterEnhancement Then
                    ' Quote has the BusinessMaster enhancement - disable Rental Reimbursement & Towing & Labor
                    chkRentalReimbursement.Checked = False
                    chkRentalReimbursement.Enabled = False
                    trRentalReimbursementDataRow.Attributes.Add("style", "display:none")
                    chkTowingAndLabor.Enabled = False
                    chkTowingAndLabor.Enabled = False
                Else
                    ' Quote does not have the BusinessMaster enhancement - Rental Reimbursement & Towing & Labor are available
                    chkRentalReimbursement.Enabled = True
                    chkTowingAndLabor.Enabled = True

                    If MyVehicle.HasRentalReimbursement Then
                        chkRentalReimbursement.Checked = True
                        txtNumberOfDays.Text = MyVehicle.RentalReimbursementNumberOfDays
                        txtDailyReimbursement.Text = MyVehicle.RentalReimbursementDailyReimbursement
                        trRentalReimbursementDataRow.Attributes.Add("style", "display:''")
                    Else
                        chkRentalReimbursement.Checked = False
                        txtNumberOfDays.Text = "30"
                        txtDailyReimbursement.Text = "30"
                        trRentalReimbursementDataRow.Attributes.Add("style", "display:none")
                    End If

                    If MyVehicle.HasTowingAndLabor Then
                        chkTowingAndLabor.Checked = True
                    Else
                        chkTowingAndLabor.Checked = False
                    End If
                End If

                'Added 11/25/2020 for CAP Endorsements Task 52981 and 52974 MLW
                ctlVehicle.Visible = False
                If IsQuoteReadOnly() OrElse IsQuoteEndorsement() Then
                    ctlVehicle.VehicleIndex = Me.VehicleIndex
                    ctlVehicle.Visible = True

                    'Added 12/08/2020 for CAP Endorsements Task 52974 MLW
                    If IsQuoteEndorsement() Then
                        lblVIN.Text = "*VIN"
                        lnkCopy.Text = "Copy" 'Updated for Cap Endorsement bug 61606 MLW
                        lnkNew.Text = "Add" 'Updated for Cap Endorsement bug 61606 MLW
                        lnkDelete.Text = "Remove" 'Updated for Cap Endorsement bug 61606 MLW
                        lnkSave.Text = "Save" 'Updated for Cap Endorsement bug 61606 MLW
                        If Not IsNewVehicleOnEndorsement(MyVehicle) Then
                            txtVIN.Enabled = False 'Added 07/07/2021 for CAP Endorsements task 53028 MLW
                            divVinLookup.Visible = False 'Added 07/07/2021 for CAP Endorsements task 53028 MLW
                            btnVinLookup.Visible = False 'Added 07/07/2021 for CAP Endorsements task 53028 MLW
                            trVinLookupMessage.Visible = False 'Added 07/07/2021 for CAP Endorsements task 53028 MLW
                            txtVehicleYear.Enabled = False
                            txtVehicleMake.Enabled = False
                            txtVehicleModel.Enabled = False
                            txtCostNew.Enabled = False
                            txtClassCode.Enabled = False
                            lnkSave_ClassCode.Visible = False
                            ddlVehicleRatingType.Enabled = False
                            ddlUseCode.Enabled = False
                            ddlOperatorType.Enabled = False
                            ddlOperatorUse.Enabled = False
                            ddlSize.Enabled = False
                            ddlTrailerType.Enabled = False
                            ddlRadius.Enabled = False
                            ddlSecondaryClass.Enabled = False
                            chkSeasonalFarmUse.Enabled = False
                            ddlSecondaryClassType.Enabled = False
                            txtClassCode.Enabled = False
                            chkDumpingOperations.Enabled = False
                            lnkSave_Garaging.Visible = False
                            chkUseGaragingAddress.Enabled = False
                            DisableGaragingFields()
                            lnkSave_VehCovs.Visible = False
                            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                                chkLiability.Enabled = False
                                chkUM.Enabled = False
                                chkUIM.Enabled = False
                            Else
                                chkLiabilityUMUIM.Enabled = False
                            End If
                            chkMedicalPayments.Enabled = False
                            chkUMPD.Enabled = False
                            txtUMPDLimit.Enabled = False
                            chkComprehensive.Enabled = False
                            ddlComprehensiveDeductible.Enabled = False
                            chkCollision.Enabled = False
                            ddlCollisionDeductible.Enabled = False
                            chkRentalReimbursement.Enabled = False
                            txtNumberOfDays.Enabled = False
                            txtDailyReimbursement.Enabled = False
                            chkTowingAndLabor.Enabled = False
                        End If
                        If TypeOfEndorsement() = "Add/Delete Vehicle" Then
                            Dim transactionCount As Integer = ddh.GetEndorsementVehicleTransactionCount()
                            If transactionCount >= 3 Then
                                If Not IsNewVehicleOnEndorsement(MyVehicle) Then
                                    lnkDelete.Visible = False
                                End If
                                lnkNew.Visible = False
                                lnkCopy.Visible = False
                                btnAddVehicle.Enabled = False
                            End If
                        ElseIf TypeOfEndorsement() = "Add/Delete Additional Interest" Then
                            lnkDelete.Visible = False
                            lnkNew.Visible = False
                            lnkCopy.Visible = False
                            btnAddVehicle.Enabled = False
                        End If
                    End If
                    'Added 07/07/2021 for CAP Endorsements task 53028 MLW
                    If IsQuoteReadOnly() Then
                        divVinLookup.Visible = False
                        btnVinLookup.Visible = False
                        trVinLookupMessage.Visible = False
                    End If
                End If
                PopulateRACASymbols()
            End If
            Me.PopulateChildControls()
        End If
    End Sub

    Public Sub PopulateRACASymbols()
        If RACASymbolHelper.IsRACASymbolsAvailable(Quote) AndAlso (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote OrElse (IsQuoteEndorsement() AndAlso IsNewVehicleOnEndorsement(MyVehicle))) Then
            If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 1 Then
                PopulateRACASymbolsByType("1", Me.hdnOtherThanCollisionSymbol, Me.hdnOtherThanCollisionOverride)
                PopulateRACASymbolsByType("2", Me.hdnCollisionSymbol, Me.hdnCollisionOverride)
                PopulateRACASymbolsByType("3", Me.hdnLiabilitySymbol, Me.hdnLiabilityOverride)
            End If
        End If
    End Sub

    Public Sub PopulateRACASymbolsByType(typeId As String, hdnSymbol As HiddenField, hdnOverride As HiddenField)
        Dim symbol As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = typeId Select s.SystemGeneratedSymbol).FirstOrDefault()
        Dim symbolOverride As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = typeId Select s.UserOverrideSymbol).FirstOrDefault()
        If symbol IsNot Nothing Then
            hdnSymbol.Value = symbol.Trim()
        End If
        If symbolOverride IsNot Nothing Then
            hdnOverride.Value = symbolOverride.Trim()
        End If
    End Sub

    Public Function GetQuoteToUse() As QuickQuoteObject
        Return SubQuoteForVehicle(MyVehicle) 'should always return something if Me.Quote is something
    End Function

    Public Sub PopulateVehicleCoverages(quoteToUse As QuickQuoteObject)
        PopulateLiabUMUIMMedPay(quoteToUse)
        PopulateUMPD(quoteToUse)
        PopulateCompColl(quoteToUse)
    End Sub

    Private Sub PopulateCompColl(quoteToUse As QuickQuoteObject)
        If MyVehicle.HasComprehensive Then
            chkComprehensive.Checked = True
            'Updated 02/16/2021 for CAP Endorsements Task 52974 MLW
            If IsEndorsementRelated() Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlComprehensiveDeductible, MyVehicle.ComprehensiveDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleId)
            Else
                ddlComprehensiveDeductible.SelectedValue = MyVehicle.ComprehensiveDeductibleId
            End If
            trComprehensiveDeductibleRow.Attributes.Add("style", "display:''")
        Else
            chkComprehensive.Checked = False
            ddlComprehensiveDeductible.SelectedIndex = 0
            For Each li As ListItem In ddlComprehensiveDeductible.Items
                If li.Text = "500" Then
                    ddlComprehensiveDeductible.SelectedIndex = -1
                    li.Selected = True
                    Exit For
                End If
            Next
            trComprehensiveDeductibleRow.Attributes.Add("style", "display:none")
        End If

        If MyVehicle.HasCollision Then
            chkCollision.Checked = True
            'Updated 02/16/2021 for CAP Endorsements Task 52974 MLW
            If IsEndorsementRelated() Then
                IFM.VR.Web.Helpers.WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddlCollisionDeductible, MyVehicle.CollisionDeductibleId, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleId)
            Else
                ddlCollisionDeductible.SelectedValue = MyVehicle.CollisionDeductibleId
            End If
            trCollisionDeductibleRow.Attributes.Add("style", "display:''")
        Else
            chkCollision.Checked = False
            For Each li As ListItem In ddlCollisionDeductible.Items
                If li.Text = "500" Then
                    ddlCollisionDeductible.SelectedIndex = -1
                    li.Selected = True
                    Exit For
                End If
            Next
            trCollisionDeductibleRow.Attributes.Add("style", "display:none")
        End If
    End Sub

    Public Sub PopulateLiabUMUIMMedPay(quoteToUse As QuickQuoteObject)
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            'show separate coverages - Liability, UM, UIM
            trLiabilityUMUIM.Attributes.Add("style", "display:none")
            trLiability.Attributes.Add("style", "display:''")
            trUM.Attributes.Add("style", "display:''")
            trUIM.Attributes.Add("style", "display:''")
        Else
            'show combined coverages - Liability UM/UIM
            trLiabilityUMUIM.Attributes.Add("style", "display:''")
            trLiability.Attributes.Add("style", "display:none")
            trUM.Attributes.Add("style", "display:none")
            trUIM.Attributes.Add("style", "display:none")
        End If

        If quoteToUse IsNot Nothing AndAlso quoteToUse.Liability_UM_UIM_LimitId.Trim <> "" Then
            ' Liability UM/UIM and Medical Payments disabled and checked when Liability UM/UIM was chosen at the policy level
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                chkLiability.Checked = True
                chkLiability.Enabled = False
                If IsEndorsementRelated() AndAlso IsNewVehicleOnEndorsement(MyVehicle) = False Then
                    If QQHelper.IsPositiveIntegerString(MyVehicle.UninsuredMotoristLiabilityLimitId) Then
                        chkUM.Checked = True
                    Else
                        chkUM.Checked = False
                    End If
                    If QQHelper.IsPositiveIntegerString(MyVehicle.UnderinsuredMotoristBodilyInjuryLiabilityLimitId) Then
                        chkUIM.Checked = True
                    Else
                        chkUIM.Checked = False
                    End If
                Else
                    chkUM.Checked = True
                    chkUIM.Checked = True
                End If
                chkUM.Enabled = False
                chkUIM.Enabled = False
            Else
                chkLiabilityUMUIM.Checked = True
                chkLiabilityUMUIM.Enabled = False
            End If
            chkMedicalPayments.Checked = True
            chkMedicalPayments.Enabled = False
        Else
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                chkLiability.Enabled = True
                chkUM.Enabled = True
                chkUIM.Enabled = True
                If MyVehicle.HasLiability_UM_UIM Then
                    chkLiability.Checked = True
                    If IsEndorsementRelated() AndAlso IsNewVehicleOnEndorsement(MyVehicle) = False Then
                        If QQHelper.IsPositiveIntegerString(MyVehicle.UninsuredMotoristLiabilityLimitId) Then
                            chkUM.Checked = True
                        Else
                            chkUM.Checked = False
                        End If
                        If QQHelper.IsPositiveIntegerString(MyVehicle.UnderinsuredMotoristBodilyInjuryLiabilityLimitId) Then
                            chkUIM.Checked = True
                        Else
                            chkUIM.Checked = False
                        End If
                    Else
                        chkUM.Checked = True
                        chkUIM.Checked = True
                    End If
                Else
                    chkLiability.Checked = False
                    chkUM.Checked = False
                    chkUIM.Checked = False
                End If
            Else
                chkLiabilityUMUIM.Enabled = True
                If MyVehicle.HasLiability_UM_UIM Then
                    chkLiabilityUMUIM.Checked = True
                Else
                    chkLiabilityUMUIM.Checked = False
                End If
            End If
            chkMedicalPayments.Enabled = True
            If MyVehicle.HasMedicalPayments Then
                chkMedicalPayments.Checked = True
            Else
                chkMedicalPayments.Checked = False
            End If
        End If
    End Sub

    Public Sub PopulateUMPD(quoteToUse As QuickQuoteObject)
        'UMPD Indiana
        If txtState.Text.ToUpper() = "IN" Then
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                trUMPDRow.Attributes.Add("style", "display:''")
                If SubQuoteFirst IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId) AndAlso MyVehicle.GaragingAddress?.Address?.QuickQuoteState = Quote.QuickQuoteState Then
                    chkUMPD.Checked = True
                Else
                    chkUMPD.Checked = False
                End If
                chkUMPD.Enabled = False
            Else
                trUMPDRow.Attributes.Add("style", "display:none")
            End If
        End If

        ' UMPD - Illinois Only - Show only when garaging address is in Illinois
        ' set the UMPD value to whatever the UM was set to on the policy coverages page
        If txtState.Text.ToUpper = "IL" OrElse txtState.Text.ToUpper = "OH" Then
            'might be able to IN with IL/OH - depends on requirements - need further info - although combined above might not work for IN, not sure how OH is supposed to even work
            trUMPDRow.Attributes.Add("style", "display:''")
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                If MyVehicle.GaragingAddress?.Address?.QuickQuoteState = Quote.QuickQuoteState Then
                    If MyVehicle.HasUninsuredMotoristPropertyDamage Then
                        Select Case MyVehicle.GaragingAddress?.Address?.QuickQuoteState
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                If QQHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId) = False Then
                                    chkUMPD.Checked = False
                                    trUMPDLimitRow.Attributes.Add("style", "display:none")
                                    chkUMPD.Enabled = False
                                Else
                                    chkUMPD.Checked = True
                                    trUMPDLimitRow.Attributes.Add("style", "display:''")
                                    txtUMPDLimit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
                                    chkUMPD.Enabled = True
                                End If
                            Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                                chkUMPD.Checked = True
                                trUMPDLimitRow.Attributes.Add("style", "display:''")
                                txtUMPDLimit.Text = "7,500"
                                chkUMPD.Enabled = True
                        End Select
                    Else
                        chkUMPD.Checked = False
                        Select Case MyVehicle.GaragingAddress?.Address?.QuickQuoteState
                            Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                                If QQHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId) Then
                                    chkUMPD.Enabled = True
                                Else
                                    chkUMPD.Enabled = False
                                End If
                                txtUMPDLimit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
                            Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                                trUMPDLimitRow.Attributes.Add("style", "display:none")
                                txtUMPDLimit.Text = "7,500"
                                chkUMPD.Enabled = True
                        End Select

                    End If
                Else
                    chkUMPD.Checked = False
                    chkUMPD.Enabled = False
                    trUMPDLimitRow.Attributes.Add("style", "display:none")
                End If
            Else
                Dim IllinoisQuote As QuickQuoteObject = ILSubQuote
                If txtState.Text.ToUpper = "OH" Then
                    txtUMPDLimit.Text = "7,500"
                Else
                    If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) AndAlso IllinoisQuote IsNot Nothing Then
                        txtUMPDLimit.Text = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
                    Else
                        txtUMPDLimit.Text = "15,000"
                    End If
                End If
                If MyVehicle.HasUninsuredMotoristPropertyDamage Then
                    If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) AndAlso IllinoisQuote IsNot Nothing Then
                        If QQHelper.IsPositiveIntegerString(IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId) = False AndAlso MyVehicle.GaragingAddress?.Address?.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                            chkUMPD.Checked = False
                            trUMPDLimitRow.Attributes.Add("style", "display:none")
                            chkUMPD.Enabled = False
                        Else
                            chkUMPD.Checked = True
                            trUMPDLimitRow.Attributes.Add("style", "display:''")
                            chkUMPD.Enabled = True
                        End If
                    Else
                        chkUMPD.Checked = True
                        trUMPDLimitRow.Attributes.Add("style", "display:''")
                        chkUMPD.Enabled = True
                    End If
                Else
                    chkUMPD.Checked = False
                    If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) AndAlso IllinoisQuote IsNot Nothing Then
                        If QQHelper.IsPositiveIntegerString(IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId) Then
                            chkUMPD.Enabled = True
                        Else
                            chkUMPD.Enabled = False
                        End If
                    Else
                        chkUMPD.Enabled = True
                    End If
                    trUMPDLimitRow.Attributes.Add("style", "display:none")
                End If
            End If
        End If
    End Sub

    'Added 07/29/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    Private Function GetValidVinValue(vehicleIndex As Integer) As String
        'ValidVIN is used to know whether or not a VIN lookup was performed and if the lookup returned valid VIN results
        'VIN Lookup is required on CAP, using this to know whether or not to disable the rate button and show a "use the VIN lookup" message
        'For new business quote side, the VIN is not required, so we set this value to True when the VIN is blank in order to get to the app side where the lookup will be performed when they do an onchange on the VIN.
        Dim validVin As String
        Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
        validVin = vinDDH.GetValueFromMasterValueDictionaryByKey(vehicleIndex + 1)
        If IsNullEmptyorWhitespace(validVin) Then
            'If not yet in the DevDictionary, assume no lookup was ever done
            validVin = "False"
        End If
        If IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle) Then
            'For endorsements, you cannot change existing vehicle info. No lookup is available, assume vin is valid.
            validVin = "True"
        End If
        Return validVin
    End Function

    Private Sub LoadSecondaryClassTypeDDL()
        Dim li As New ListItem()

        ddlSecondaryClassType.Items.Clear()
        If ddlSecondaryClass.SelectedIndex = -1 Then Exit Sub

        'Added 02/19/2021 for CAP Endorsements Task 52974 MLW
        If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
            li = New ListItem
            li.Text = "N/A"
            li.Value = "0"
            ddlSecondaryClassType.Items.Add(li)
        End If

        Select Case ddlSecondaryClass.SelectedItem.Text
            Case "Food Delivery"
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Canneries"
                li.Value = "10"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Fish and Seafood"
                li.Value = "11"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Frozen Foods"
                li.Value = "12"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Fruit and Vegetables"
                li.Value = "13"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Meat or Poultry"
                li.Value = "14"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "All Others"
                li.Value = "9"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Farmers"
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Individual or Family Owned Corporation (not hauling livestock)"
                li.Value = "23"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Livestock"
                li.Value = "24"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "All Other"
                li.Value = "9"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Dump and Transit Mix"
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Excavating"
                li.Value = "25"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Sand and Gravel (other than quarrying)"
                li.Value = "26"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Mining"
                li.Value = "27"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Quarrying"
                li.Value = "28"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "All Other"
                li.Value = "9"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Contractors"
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Building - Commercial"
                li.Value = "29"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Building - Private Dwelling"
                li.Value = "30"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Repair or Service"
                li.Value = "31"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Excavating"
                li.Value = "25"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Street and Road"
                li.Value = "32"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "All Other"
                li.Value = "9"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Not Otherwise Specified"
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Logging and Lumbering"
                li.Value = "33"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "All Other"
                li.Value = "9"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Truckers" 'Added 02/17/2021 for CAP Endorsements Task 52974 MLW
                li = New ListItem
                li.Text = "Common Carrier"
                li.Value = "1"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Contract Carrier (Not chemicals or Iron and Steel)"
                li.Value = "2"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Contract Carrier Chemicals"
                li.Value = "3"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Contract Iron and Steel"
                li.Value = "4"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Exempt Carrier (other than livestock)"
                li.Value = "5"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Exempt carrier livestock"
                li.Value = "6"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Carrier"
                li.Value = "7"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Tow Truck for Hire"
                li.Value = "8"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Specialized Delivery" 'Added 02/17/2021 for CAP Endorsements Task 52974 MLW
                li = New ListItem
                li.Text = "Armored Cars"
                li.Value = "15"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Film"
                li.Value = "16"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Magazines and Newspapers"
                li.Value = "17"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Mail and Parcel Post"
                li.Value = "18"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "Waste Disposal" 'Added 02/17/2021 for CAP Endorsements Task 52974 MLW
                li = New ListItem
                li.Text = "Automobile Dismantler"
                li.Value = "19"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Building Wrecking Operations"
                li.Value = "20"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Garbage"
                li.Value = "21"
                ddlSecondaryClassType.Items.Add(li)

                li = New ListItem
                li.Text = "Junk Dealer"
                li.Value = "22"
                ddlSecondaryClassType.Items.Add(li)

                trSecondaryClassTypeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case Else
                Exit Sub
        End Select
    End Sub

    Private Sub LoadUseCodeDDL()
        Dim li As New ListItem()

        ddlUseCode.Items.Clear()
        If ddlVehicleRatingType.SelectedIndex = -1 Then Exit Sub

        Select Case ddlVehicleRatingType.SelectedValue
            Case "1"  ' Private passenger
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlUseCode.Items.Add(li)

                'Added 02/16/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                    li = New ListItem
                    li.Text = "N/A"
                    li.Value = "0"
                    ddlUseCode.Items.Add(li)

                    li = New ListItem
                    li.Text = "Driver Training"
                    li.Value = "25"
                    ddlUseCode.Items.Add(li)
                End If

                li = New ListItem
                li.Text = "Business"
                li.Value = "20"
                ddlUseCode.Items.Add(li)

                li = New ListItem
                li.Text = "Personal"
                li.Value = "21"
                ddlUseCode.Items.Add(li)

                'Updated 2/7/2022 for bug 63488 MLW
                If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task63488_CAPRemoveAntiqueAutoUseCode")) = True Then
                    If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                        li = New ListItem
                        li.Text = "Antique Auto"
                        li.Value = "22"
                        ddlUseCode.Items.Add(li)
                    End If
                Else
                    li = New ListItem
                    li.Text = "Antique Auto"
                    li.Value = "22"
                    ddlUseCode.Items.Add(li)
                End If

                li = New ListItem
                li.Text = "Farm"
                li.Value = "23"
                ddlUseCode.Items.Add(li)

                trUseCodeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "9"  ' Trucks
                ' Load Use Codes
                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlUseCode.Items.Add(li)

                'Added 02/16/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                    li = New ListItem
                    li.Text = "N/A"
                    li.Value = "0"
                    ddlUseCode.Items.Add(li)
                End If

                li = New ListItem
                li.Text = "Service"
                li.Value = "28"
                ddlUseCode.Items.Add(li)

                li = New ListItem
                li.Text = "Retail"
                li.Value = "29"
                ddlUseCode.Items.Add(li)

                li = New ListItem
                li.Text = "Commercial"
                li.Value = "30"
                ddlUseCode.Items.Add(li)

                'Added 08/23/2021 for Bug 64448 MLW
                trUseCodeRow.Attributes.Add("style", "display:''")

                ' Load size ddl
                ddlSize.Items.Clear()

                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSize.Items.Add(li)

                'Added 02/19/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                    li = New ListItem
                    li.Text = "N/A"
                    li.Value = "0"
                    ddlSize.Items.Add(li)
                End If

                li = New ListItem
                li.Text = "Light Truck < or equal 10,000 Pounds GVW"
                li.Value = "18"
                ddlSize.Items.Add(li)

                li = New ListItem
                li.Text = "Medium Truck 10,001 to 20,000 Pounds GVW"
                li.Value = "19"
                ddlSize.Items.Add(li)

                li = New ListItem
                li.Text = "Heavy Truck 20,001 to 45,000 Pounds GVW"
                li.Value = "20"
                ddlSize.Items.Add(li)

                li = New ListItem
                li.Text = "Extra Heavy Truck > 45,000 Pounds GVWW"
                li.Value = "21"
                ddlSize.Items.Add(li)

                li = New ListItem
                li.Text = "Heavy Truck-Tractors < or equal 45,000 Pounds GVW"
                li.Value = "22"
                ddlSize.Items.Add(li)

                li = New ListItem
                li.Text = "Extra Heavy Truck-Tractors > 45,000 Pounds GVW"
                li.Value = "23"
                ddlSize.Items.Add(li)

                li = New ListItem
                li.Text = "Trailer Types"
                li.Value = "30"
                ddlSize.Items.Add(li)

                trSizeRow.Attributes.Add("style", "display:''")

                ' Load radius ddl
                ddlRadius.Items.Clear()

                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlRadius.Items.Add(li)

                'Added 02/19/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                    li = New ListItem
                    li.Text = "N/A"
                    li.Value = "0"
                    ddlRadius.Items.Add(li)
                End If

                li = New ListItem
                li.Text = "Local, up to 50 miles"
                li.Value = "1"
                ddlRadius.Items.Add(li)

                li = New ListItem
                li.Text = "Intermediate, 51 to 200 miles"
                li.Value = "2"
                ddlRadius.Items.Add(li)

                trRadiusRow.Attributes.Add("style", "display:''")

                ' Load Secondary Class ddl
                ddlSecondaryClass.Items.Clear()

                li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlSecondaryClass.Items.Add(li)

                'Added 02/19/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteReadOnly() OrElse (IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle)) Then
                    li = New ListItem
                    li.Text = "N/A"
                    li.Value = "0"
                    ddlSecondaryClass.Items.Add(li)

                    li = New ListItem
                    li.Text = "Truckers"
                    li.Value = "1"
                    ddlSecondaryClass.Items.Add(li)

                    li = New ListItem
                    li.Text = "Specialized Delivery"
                    li.Value = "8"
                    ddlSecondaryClass.Items.Add(li)

                    li = New ListItem
                    li.Text = "Waste Disposal"
                    li.Value = "9"
                    ddlSecondaryClass.Items.Add(li)
                End If

                li = New ListItem
                li.Text = "Food Delivery"
                li.Value = "26"
                ddlSecondaryClass.Items.Add(li)

                li = New ListItem
                li.Text = "Farmers"
                li.Value = "27"
                ddlSecondaryClass.Items.Add(li)

                li = New ListItem
                li.Text = "Dump and Transit Mix"
                li.Value = "28"
                ddlSecondaryClass.Items.Add(li)

                li = New ListItem
                li.Text = "Contractors"
                li.Value = "29"
                ddlSecondaryClass.Items.Add(li)

                li = New ListItem
                li.Text = "Not Otherwise Specified"
                li.Value = "30"
                ddlSecondaryClass.Items.Add(li)

                trSecondaryClassRow.Attributes.Add("style", "display:''")

                Exit Select
            Case "7"  ' Funeral Director
                'li = New ListItem
                li.Text = ""
                li.Value = ""
                ddlUseCode.Items.Add(li)

                li = New ListItem
                li.Text = "Limousine"
                li.Value = "2"
                ddlUseCode.Items.Add(li)

                li = New ListItem
                li.Text = "Hearse or Flower Car"
                li.Value = "31"
                ddlUseCode.Items.Add(li)

                trUseCodeRow.Attributes.Add("style", "display:''")

                Exit Select
            Case Else
                Exit Sub
        End Select
    End Sub

    Private Function GaragingAddressHasValues()
        If MyVehicle IsNot Nothing AndAlso MyVehicle.GaragingAddress IsNot Nothing Then
            If MyVehicle.GaragingAddress.Address.Zip.Trim <> "" Then Return True
            If MyVehicle.GaragingAddress.Address.City.Trim <> "" Then Return True
            If MyVehicle.GaragingAddress.Address.County.Trim <> "" Then Return True
        End If

        Return False
    End Function

    Private Function GaragingAddressMatchesLocation0Address() As Boolean
        If MyVehicle IsNot Nothing Then
            If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 Then Return False

            Dim GAR As QuickQuote.CommonObjects.QuickQuoteAddress = MyVehicle.GaragingAddress.Address
            Dim LOC As QuickQuote.CommonObjects.QuickQuoteAddress = Quote.Locations(0).Address

            If GAR.Zip = LOC.Zip AndAlso GAR.City = LOC.City AndAlso GAR.StateId = LOC.StateId AndAlso GAR.County = LOC.County Then
                Return True
            Else
                Return False
            End If
        End If

        Return False
    End Function

    Private Sub DisableGaragingFields()
        txtZip.Enabled = False
        txtCity.Enabled = False
        txtCounty.Enabled = False
        txtState.Enabled = False
    End Sub
    Private Sub EnableGaragingFields()
        txtZip.Enabled = True
        txtCity.Enabled = True
        txtCounty.Enabled = True
        txtState.Enabled = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 12/22/2020 for CAP Endorsements Task 52974 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" AndAlso IsNewVehicleOnEndorsement(MyVehicle)) Then
            'If MyVehicle IsNot Nothing Then
            If MyVehicle IsNot Nothing Then
                ' General Info
                MyVehicle.Year = txtVehicleYear.Text
                MyVehicle.Make = txtVehicleMake.Text
                MyVehicle.Model = txtVehicleModel.Text
                MyVehicle.CostNew = txtCostNew.Text
                MyVehicle.ClassCode = txtClassCode.Text
                'Added 07/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
                MyVehicle.Vin = txtVIN.Text

                ' Lookup values
                MyVehicle.VehicleRatingTypeId = ddlVehicleRatingType.SelectedValue
                'Added 08/24/2021 for Bug 64448 MLW
                If ddlSize.SelectedValue = "21" OrElse ddlSize.SelectedValue = "23" OrElse ddlSize.SelectedValue = "30" Then
                    'Use code returns 0 for the ReverseClassCodeLookup, vcc.UseCodeId, for size 21 - Extra Heavy Truck, size 23 - Extra Heavy Truck-Tractors and size 30 - Trailer Types, so we need to set the size "". Otherwise, at rate Diamond gives an error that use code is not required for Extra Heavy Truck/Extra Heavy Truck-Tractor.
                    ddlUseCode.SelectedValue = ""
                End If
                MyVehicle.UseCodeTypeId = ddlUseCode.SelectedValue
                MyVehicle.OperatorTypeId = ddlOperatorType.SelectedValue
                MyVehicle.OperatorUseTypeId = ddlOperatorUse.SelectedValue
                MyVehicle.SizeTypeId = ddlSize.SelectedValue
                MyVehicle.TrailerTypeId = ddlTrailerType.SelectedValue
                MyVehicle.RadiusTypeId = ddlRadius.SelectedValue
                MyVehicle.SecondaryClassTypeId = ddlSecondaryClass.SelectedValue
                MyVehicle.SecondaryClassUsageTypeId = ddlSecondaryClassType.SelectedValue
                'Added 08/13/2021 for CAP Endorsements Task 53030 MLW - for copied vehicles - only force a True if they leave the VIN blank and have a year, make, model so they can get to the app side
                If Not IsQuoteEndorsement() AndAlso hdnValidVin.Value.ToUpper() = "FALSE" Then
                    If IsNullEmptyorWhitespace(txtVIN.Text) AndAlso (Not IsNullEmptyorWhitespace(txtVehicleYear.Text) AndAlso Not IsNullEmptyorWhitespace(txtVehicleMake.Text) AndAlso Not IsNullEmptyorWhitespace(txtVehicleModel.Text)) Then
                        hdnValidVin.Value = "True"
                    End If
                End If
                'Added 07/26/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
                'save ValidVin to DevDictionary - indicates that the VIN lookup was used and that the VIN used in the lookup was a valid VIN. If not a valid VIN the cost new and size fields remain enabled, user will not be able to rate and a message to use the VIN lookup will appear.
                'This is for new business and endorsements, using VR vehicleNum, value not stored in Diamond
                Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
                vinDDH.AddToMasterValueDictionary(VehicleIndex + 1, hdnValidVin.Value)

                ' Garaging Address
                If MyVehicle.GaragingAddress IsNot Nothing Then MyVehicle.GaragingAddress = New QuickQuote.CommonObjects.QuickQuoteGaragingAddress
                MyVehicle.GaragingAddress.Address.Zip = txtZip.Text
                MyVehicle.GaragingAddress.Address.City = txtCity.Text
                MyVehicle.GaragingAddress.Address.State = Me.txtState.Text.Trim().ToUpper()
                MyVehicle.GaragingAddress.Address.StateId = If(IFM.VR.Common.Helpers.States.DoesStateAbbreviationExist(Me.txtState.Text), IFM.VR.Common.Helpers.States.GetStateInfoFromAbbreviation(Me.txtState.Text).StateId.ToString(), String.Empty)
                MyVehicle.GaragingAddress.Address.County = txtCounty.Text

                ' Dumping Operations & Seasonal Farm Use
                If chkDumpingOperations.Checked Then MyVehicle.UsedInDumping = True Else MyVehicle.UsedInDumping = False
                'FarmUseCode 2 is for Farm use, FarmUseCode 3 is for Seasonal Farm Use  -- CAH 8/16/2017
                'If chkSeasonalFarmUse.Checked Then MyVehicle.FarmUseCodeTypeId = "2" Else MyVehicle.FarmUseCodeTypeId = "0"
                If MyVehicle.SecondaryClassTypeId = "27" Then
                    If chkSeasonalFarmUse.Checked Then
                        MyVehicle.FarmUseCodeTypeId = "3"
                    Else
                        MyVehicle.FarmUseCodeTypeId = "2"
                    End If
                Else
                    MyVehicle.FarmUseCodeTypeId = "0"
                End If


                ' Coverages
                If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                    If chkLiability.Checked Then
                        MyVehicle.HasLiability_UM_UIM = True
                        MyVehicle.Liability_UM_UIM_LimitId = SubQuoteFirst.Liability_UM_UIM_LimitId
                    Else
                        MyVehicle.HasLiability_UM_UIM = False
                    End If
                    If chkUM.Checked Then
                        MyVehicle.UninsuredMotoristLiabilityLimitId = SubQuoteFirst.UnderinsuredMotoristBodilyInjuryLiabilityLimitId
                    Else
                        MyVehicle.UninsuredMotoristLiabilityLimitId = ""
                    End If
                    If chkUIM.Checked Then
                        MyVehicle.UnderinsuredMotoristBodilyInjuryLiabilityLimitId = SubQuoteFirst.UnderinsuredMotoristBodilyInjuryLiabilityLimitId
                    Else
                        MyVehicle.UnderinsuredMotoristBodilyInjuryLiabilityLimitId = ""
                    End If
                Else
                    If chkLiabilityUMUIM.Checked Then
                        MyVehicle.HasLiability_UM_UIM = True
                    Else
                        MyVehicle.HasLiability_UM_UIM = False
                    End If
                End If

                If chkMedicalPayments.Checked Then
                    MyVehicle.HasMedicalPayments = True
                Else
                    MyVehicle.HasMedicalPayments = False
                End If

                If chkComprehensive.Checked Then
                    MyVehicle.HasComprehensive = True
                    MyVehicle.ComprehensiveDeductibleId = ddlComprehensiveDeductible.SelectedValue
                Else
                    MyVehicle.HasComprehensive = False
                    MyVehicle.ComprehensiveDeductibleId = ""
                End If

                If chkCollision.Checked Then
                    MyVehicle.HasCollision = True
                    MyVehicle.CollisionDeductibleId = ddlCollisionDeductible.SelectedValue
                Else
                    MyVehicle.HasCollision = False
                    MyVehicle.CollisionDeductibleId = ""
                End If

                'UMPD
                If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                    'Only save UMPD if vehicle garaging state matching gov state
                    Dim garagingState As String = IFM.VR.Common.Helpers.States.GetStateInfoFromAbbreviation(txtState.Text.ToUpper()).StateName
                    Dim quoteState As String = Quote.QuickQuoteState.ToString()
                    If chkUMPD.Checked AndAlso garagingState = quoteState Then
                        MyVehicle.HasUninsuredMotoristPropertyDamage = True
                    Else
                        MyVehicle.HasUninsuredMotoristPropertyDamage = False
                    End If
                Else
                    ' UMPD 9/26/18 MGB Multistate
                    If txtState.Text.ToUpper = "IL" OrElse txtState.Text.ToUpper = "OH" Then
                        'MyVehicle.UninsuredMotoristPropertyDamageLimitId = ""
                        If chkUMPD.Checked Then
                            MyVehicle.HasUninsuredMotoristPropertyDamage = True
                            'MyVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = "4"    ' $250
                            'MyVehicle.UninsuredMotoristPropertyDamageLimitId = 48   ' 48 = 15,000
                        Else
                            MyVehicle.HasUninsuredMotoristPropertyDamage = False
                            'MyVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                            'MyVehicle.UninsuredMotoristPropertyDamageLimitId = ""
                        End If
                    Else
                        MyVehicle.HasUninsuredMotoristPropertyDamage = False
                    End If
                End If

                If chkRentalReimbursement.Checked Then
                    MyVehicle.HasRentalReimbursement = True
                    MyVehicle.RentalReimbursementNumberOfDays = txtNumberOfDays.Text
                    MyVehicle.RentalReimbursementDailyReimbursement = txtDailyReimbursement.Text
                Else
                    MyVehicle.HasRentalReimbursement = False
                    MyVehicle.RentalReimbursementNumberOfDays = ""
                    MyVehicle.RentalReimbursementDailyReimbursement = ""
                End If

                If chkTowingAndLabor.Checked Then
                    MyVehicle.HasTowingAndLabor = True
                Else
                    MyVehicle.HasTowingAndLabor = False
                End If

                'Added 04/02/2021 for CAP Endorsements Task 52974 MLW
                If IsQuoteEndorsement() AndAlso IsNewVehicleOnEndorsement(MyVehicle) Then
                    ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, VehicleIndex, MyVehicle)
                    Quote.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                    Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
                    Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Vehicle)
                    Quote.TransactionRemark = updatedRemarks
                End If

                If RACASymbolHelper.IsRACASymbolsAvailable(Quote) AndAlso (Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote OrElse (IsQuoteEndorsement() AndAlso IsNewVehicleOnEndorsement(MyVehicle))) Then
                    If MyVehicle.VehicleSymbols Is Nothing Then
                        MyVehicle.VehicleSymbols = New List(Of QuickQuoteVehicleSymbol)
                    End If
                    RACASymbolHelper.AddUpdateRACASymbols(Me.MyVehicle, "1", Me.hdnOtherThanCollisionSymbol.Value, Me.hdnOtherThanCollisionOverride.Value)
                    RACASymbolHelper.AddUpdateRACASymbols(Me.MyVehicle, "2", Me.hdnCollisionSymbol.Value, Me.hdnCollisionOverride.Value)
                    RACASymbolHelper.AddUpdateRACASymbols(Me.MyVehicle, "3", Me.hdnLiabilitySymbol.Value, Me.hdnLiabilityOverride.Value)
                End If
            End If

            If CAPCustomWrapOrPaintJobHelper.IsCustomWrapOrPaintJobAvailable(Quote) Then
                Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "CustomWrapOrPaintJob", Quote.LobType)
                vinDDH.AddToMasterValueDictionary(VehicleIndex + 1, (chkCustomPaintJobOrWrapComprehensive.Checked OrElse chkCustomPaintJobOrWrapCollision.Checked).ToString())
            End If

            Me.SaveChildControls()
            'Updated 06/28/2021 for CAP Endorsements Task 52974 MLW
            If Not IsQuoteEndorsement() Then
                Populate()
            End If
            'Populate()
            UpdateAccordHeader()

        ElseIf IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Additional Interest" Then 'added 5/21/2021 to allow ctl_CAP_App_Vehicle to Save
            Me.SaveChildControls()
            'Populate() '06/28/2021 moved to parent control ctl_CAP_VehicleList save() for CAP Endorsements Task 52974 MLW
            UpdateAccordHeader()
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 12/22/2020 for CAP Endorsements Task 52974 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" AndAlso IsNewVehicleOnEndorsement(MyVehicle)) Then
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            ' Don't validate if vehicle is nothing or we haven't gotten to the vehicle page yet
            If MyVehicle Is Nothing OrElse (Session("DoNotValidateVehicles_" & Session.SessionID.ToString) IsNot Nothing AndAlso Session("DoNotValidateVehicles_" & Session.SessionID.ToString) = True) Then Exit Sub

            MyBase.ValidateControl(valArgs)

            'Updated 06/02/2021 for CAP Endorsements Task 52974 MLW
            If IsQuoteEndorsement() Then
                If IsNullEmptyorWhitespace(MyVehicle.Year) AndAlso IsNullEmptyorWhitespace(MyVehicle.Make) AndAlso IsNullEmptyorWhitespace(MyVehicle.Model) AndAlso IsNullEmptyorWhitespace(MyVehicle.Vin) Then
                    Me.ValidationHelper.GroupName = "Vehicle 0"
                Else
                    Me.ValidationHelper.GroupName = String.Format("{0} {1} {2} {3}", MyVehicle.Year, MyVehicle.Make, MyVehicle.Model, MyVehicle.Vin)
                End If
            Else
                Me.ValidationHelper.GroupName = "Vehicle # " & VehicleIndex + 1.ToString
            End If
            'Me.ValidationHelper.GroupName = "Vehicle # " & VehicleIndex + 1.ToString

            ' General Info
            If txtVehicleYear.Text = String.Empty Then
                Me.ValidationHelper.AddError(txtVehicleYear, "Missing Year", accordList)
            Else
                If txtVehicleYear.Text.IsNumeric() = False OrElse txtVehicleYear.Text.TryToGetInt32() < 1900 Then
                    Me.ValidationHelper.AddError(txtVehicleYear, "Invalid Year", accordList)
                Else
                    If txtVehicleYear.Text.TryToGetInt32() > IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year + 1 Then
                        Me.ValidationHelper.AddError(txtVehicleYear, "Invalid Year", accordList)
                    End If
                End If
            End If
            If txtVehicleMake.Text = String.Empty Then
                Me.ValidationHelper.AddError(txtVehicleMake, "Missing Make", accordList)
            End If
            If txtVehicleModel.Text = String.Empty Then
                Me.ValidationHelper.AddError(txtVehicleModel, "Missing Model", accordList)
            End If

            'VIN Validation - Added 07/08/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
            Dim valItems = Validation.ObjectValidation.CommLines.LOB.CAP.VINValidator.VINValidation(Me.VehicleIndex, Me.Quote, valArgs.ValidationType)
            If valItems.Any() Then
                For Each v In valItems
                    Select Case v.FieldId
                        Case Validation.ObjectValidation.CommLines.LOB.CAP.VINValidator.VehicleVIN
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtVIN, v, accordList)
                    End Select
                Next
            End If

            If chkComprehensive.Checked Then
                ' Cost New is only required when Comprehensive is selected
                If txtCostNew.Text = String.Empty Then
                    Me.ValidationHelper.AddError(txtCostNew, "Missing Cost New", accordList)
                Else
                    If Not IsNumeric(txtCostNew.Text) OrElse CDec(txtCostNew.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtCostNew, "Invalid Cost New", accordList)
                    End If
                End If
            End If
            If txtClassCode.Text = String.Empty Then
                Me.ValidationHelper.AddError(txtClassCode, "Missing Class Code", accordList)
            End If

            ' Rating Type is required MGB 7-17-2017
            If ddlVehicleRatingType.SelectedIndex <= 0 Then
                Me.ValidationHelper.AddError(ddlVehicleRatingType, "Missing Rating Type", accordList)
            End If

            ' Garaging Address
            If txtZip.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtZip, "Missing ZIP", accordList)
            End If
            If txtCity.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtCity, "Missing City", accordList)
            End If
            If txtCounty.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtCounty, "Missing County", accordList)
            End If

            ' Garaging State
            'If String.IsNullOrWhiteSpace(txtState.Text) Then
            '    Me.ValidationHelper.AddError(txtState, "Missing State", accordList)
            'End If

            ' 10-16-2018 Matt A
            ' You can't do validations like this. You'd have to change it for every state added and 
            ' this code doesn't work. The effective date is past 1/1/2019, why does that matter? You need to know if you have a subquote with the state of this garaging address available on this quote.
            'If QuoteIsMultistate(Quote) Then
            '    ' Allow IN & IL
            '    If txtState.Text.ToUpper() <> "IN" AndAlso txtState.Text.ToUpper() <> "IL" Then
            '        Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
            '    End If
            'Else
            '    ' Allow IN only
            '    If txtState.Text.ToUpper() <> "IN" Then
            '        Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
            '    End If
            'End If

            ' this validation is fully backward and forward compatible no changes needed ever again you can add 50 states and it won't need to be changed again
            ' Doesn't do what I need MGB 10/22/18
            'If String.IsNullOrWhiteSpace(txtState.Text) Then
            '    Me.ValidationHelper.AddError(txtState, "Missing State", accordList)
            'Else
            '    If Not IFM.VR.Common.Helpers.States.DoesStateAbbreviationExist(Me.MyVehicle?.GaragingAddress?.Address?.State) Then
            '        Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
            '    Else
            '        If Not Me.SubQuotesContainsState(Me.MyVehicle?.GaragingAddress?.Address?.State) Then
            '            Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
            '        End If
            '    End If
            'End If

            ' STATE - new 10/22/18 MGB
            ' Can't be empty
            If txtState.Text = "" Then Me.ValidationHelper.AddError(txtState, "Missing State", accordList)
            ' Must be a valid state abbreviation
            If Not IFM.VR.Common.Helpers.States.DoesStateAbbreviationExist(Me.MyVehicle?.GaragingAddress?.Address?.State) Then
                Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
            End If

            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' Multistate - Must be one of the supported multistate states
                ' TODO: I need a function that will return all valid states to get rid of the hardcoded values here
                Dim AllAvailableStates As New List(Of String)
                AllAvailableStates.Add("IN")
                AllAvailableStates.Add("IL")
                If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                    AllAvailableStates.Add("OH")
                End If
                Dim found As Boolean = False
                For Each st As String In AllAvailableStates
                    If st = txtState.Text.ToUpper() Then
                        found = True
                        Exit For
                    End If
                Next
                If Not found Then
                    Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
                End If
            Else
                ' Single State - Must be the state on the quote
                If Not Me.SubQuotesContainsState(Me.MyVehicle?.GaragingAddress?.Address?.State) Then
                    Me.ValidationHelper.AddError(txtState, "Invalid State", accordList)
                End If
            End If


            ' Coverages
            If chkComprehensive.Checked Then
                If ddlComprehensiveDeductible.SelectedIndex < 0 Then
                    Me.ValidationHelper.AddError(ddlComprehensiveDeductible, "Missing Deductible", accordList)
                End If
            End If
            If chkCollision.Checked Then
                If ddlCollisionDeductible.SelectedIndex < 0 Then
                    Me.ValidationHelper.AddError(ddlCollisionDeductible, "Missing Deductible", accordList)
                End If
            End If
            If chkRentalReimbursement.Checked Then
                If txtNumberOfDays.Text.Trim = String.Empty Then
                    Me.ValidationHelper.AddError(txtNumberOfDays, "Missing Number of Days", accordList)
                Else
                    If Not IsNumeric(txtNumberOfDays.Text) OrElse CInt(txtNumberOfDays.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtNumberOfDays, "Invalid Number of Days", accordList)
                    End If
                End If
                If txtDailyReimbursement.Text.Trim = String.Empty Then
                    Me.ValidationHelper.AddError(txtDailyReimbursement, "Missing Daily Reimbursement", accordList)
                Else
                    If Not IsNumeric(txtDailyReimbursement.Text) OrElse CInt(txtDailyReimbursement.Text) <= 0 Then
                        Me.ValidationHelper.AddError(txtNumberOfDays, "Invalid Daily Reimbursement", accordList)
                    End If
                End If
            End If

            'Check for Unexceptable Class Code
            Dim badCAPCodes As New List(Of String) From {"21", "22", "23", "24", "25", "26", "29", "41", "42", "43", "44", "49", "51", "52", "53", "54", "55"}
            Dim badCAPCodeMsg = _badCAPCodeMsg
            Dim ClassCodeValue = Me.txtClassCode.Text
            If (ClassCodeValue.Length = 5) Then
                If badCAPCodes.Contains(ClassCodeValue.Substring(3)) Then
                    'Class code is one we don't allow
                    'remove checkbox options
                    If divDumping IsNot Nothing Then divDumping.Style.Add("display", "none")
                    Me.chkDumpingOperations.Checked = False
                    If divSeasonalFarmUse IsNot Nothing Then divSeasonalFarmUse.Style.Add("display", "none")
                    Me.chkSeasonalFarmUse.Checked = False
                    'reset classcode options
                    Me.ddlVehicleRatingType.SelectedIndex = -1
                    If trUseCodeRow IsNot Nothing Then trUseCodeRow.Style.Add("display", "none")
                    If ddlUseCode IsNot Nothing Then ddlUseCode.SelectedIndex = "0"
                    If trOperatorTypeRow IsNot Nothing Then trOperatorTypeRow.Style.Add("display", "none")
                    If ddlOperatorType IsNot Nothing Then ddlOperatorType.SelectedIndex = "0"
                    If trOperatorUseRow IsNot Nothing Then trOperatorUseRow.Style.Add("display", "none")
                    If ddlOperatorUse IsNot Nothing Then ddlOperatorUse.SelectedIndex = "0"
                    If trSizeRow IsNot Nothing Then trSizeRow.Style.Add("display", "none")
                    If ddlSize IsNot Nothing Then ddlSize.SelectedIndex = "0"
                    If trTrailerTypeRow IsNot Nothing Then trTrailerTypeRow.Style.Add("display", "none")
                    If ddlTrailerType IsNot Nothing Then ddlTrailerType.SelectedIndex = "0"
                    If trRadiusRow IsNot Nothing Then trRadiusRow.Style.Add("display", "none")
                    If ddlRadius IsNot Nothing Then ddlRadius.SelectedIndex = "0"
                    If trSecondaryClassRow IsNot Nothing Then trSecondaryClassRow.Style.Add("display", "none")
                    If ddlSecondaryClass IsNot Nothing Then ddlSecondaryClass.SelectedIndex = "0"
                    If trSecondaryClassTypeRow IsNot Nothing Then trSecondaryClassTypeRow.Style.Add("display", "none")
                    'If ddlSecondaryClassType IsNot Nothing Then ddlSecondaryClassType.SelectedIndex = "0"
                    'Me.txtClassCode.Text = ""
                    'Me.lblClassCode.Text = ""
                    Me.ValidationHelper.AddError(Me.txtClassCode, badCAPCodeMsg, accordList)
                    Return
                End If
            End If
            If ValidationHelper.HasErrros Then
                Me.lnkCopy.Visible = False
            Else
                'Updated 06/18/2021 for Bug 62744 MLW
                If IsQuoteEndorsement() Then
                    Dim transactionCount As Integer = ddh.GetEndorsementVehicleTransactionCount()
                    If transactionCount >= 3 Then
                        Me.lnkCopy.Visible = False
                    Else
                        Me.lnkCopy.Visible = True
                    End If
                Else
                    Me.lnkCopy.Visible = True
                End If
                'Me.lnkCopy.Visible = True
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Private Sub PopulateGaragingFieldsFromLocationZero()
        ' Try and populate from Location 0
        If Quote IsNot Nothing AndAlso (Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0) Then
            Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(0)
            txtZip.Text = loc.Address.Zip
            txtCity.Text = loc.Address.City
            txtState.Text = loc.Address.State
            txtCounty.Text = loc.Address.County
            DisableGaragingFields()
        Else
            ' Location 0 does not exist, populate from Policyholder
            chkUseGaragingAddress.Checked = False
            'PopulateGaragingFieldsFromPolicyHolder()
            Dim ph As QuickQuote.CommonObjects.QuickQuotePolicyholder = Quote.Policyholder
            txtZip.Text = ph.Address.Zip
            txtCity.Text = ph.Address.City
            txtState.Text = ph.Address.State
            txtCounty.Text = ph.Address.County
        End If
    End Sub

    Private Sub PopulateGaragingFieldsFromPolicyHolder()
        If Quote IsNot Nothing AndAlso Quote.Policyholder IsNot Nothing Then
            Dim ph As QuickQuote.CommonObjects.QuickQuotePolicyholder = Quote.Policyholder
            txtZip.Text = ph.Address.Zip
            txtCity.Text = ph.Address.City
            txtState.Text = ph.Address.State
            txtCounty.Text = ph.Address.County
            EnableGaragingFields()
        Else
            chkUseGaragingAddress.Checked = False
            txtZip.Text = ""
            txtCity.Text = ""
            txtState.Text = ""
            txtCounty.Text = ""
            EnableGaragingFields()
        End If
    End Sub

    Private Sub TestForDumpingOperations()
        If lblClassCode.Text.Trim <> "" AndAlso lblClassCode.Text.Length > 4 Then
            If lblClassCode.Text.Substring(3, 1) = "7" Then
                divDumping.Attributes.Add("style", "display:''")
                If MyVehicle.UsedInDumping Then
                    chkDumpingOperations.Checked = True
                Else
                    chkDumpingOperations.Checked = False
                End If
            Else
                divDumping.Attributes.Add("style", "display:none")
                chkDumpingOperations.Checked = False
            End If
        End If
    End Sub

    Private Sub TestForSeasonalFarmUse()
        divSeasonalFarmUse.Attributes.Add("style", "display:none")
        chkSeasonalFarmUse.Checked = False

        If ddlSecondaryClass.SelectedValue = "27" Then  ' Secondary Class 27 - Farmers
            If ddlSize.SelectedValue = "30" Then  ' Medium truck
                ' Seasonal Farm Use should be available when SemiTrailer Or Trailer Is selected
                ' SemiTrailer = 2
                ' Trailer = 3
                If ddlTrailerType.SelectedValue = "2" OrElse ddlTrailerType.SelectedValue = "3" Then
                    divSeasonalFarmUse.Attributes.Add("style", "display:''")
                    'FarmUseCode 2 is for Farm use, FarmUseCode 3 is for Seasonal Farm Use  -- CAH 8/16/2017
                    If MyVehicle.FarmUseCodeTypeId = "3" Then   ' FarmUseCode 2 = Seasonal Farm Use <-incorrect
                        chkSeasonalFarmUse.Checked = True
                    Else
                        chkSeasonalFarmUse.Checked = False
                    End If
                End If
            Else
                ' SizeId <> 30
                ' Show the seasonal farm use checkbox if size is NOT 18 (light truck)
                If ddlSize.SelectedValue <> "18" Then ' 18 = Light Truck < Or equal 10,000 pounds GVW
                    divSeasonalFarmUse.Attributes.Add("style", "display:''")
                    'FarmUseCode 2 is for Farm use, FarmUseCode 3 is for Seasonal Farm Use  -- CAH 8/16/2017
                    If MyVehicle.FarmUseCodeTypeId = "3" Then   ' FarmUseCode 2 = Seasonal Farm Use <-incorrect
                        chkSeasonalFarmUse.Checked = True
                    Else
                        chkSeasonalFarmUse.Checked = False
                    End If
                End If
            End If
        End If

        Exit Sub
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click, lnkSave_ClassCode.Click, lnkSave_Garaging.Click, lnkSave_VehCovs.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub lnkNew_Click(sender As Object, e As EventArgs) Handles lnkNew.Click
        RaiseEvent NewVehicleRequested()
    End Sub

    Private Sub lnkDelete_Click(sender As Object, e As EventArgs) Handles lnkDelete.Click
        'Added 04/02/2021 for CAP Endorsements Task 52974 MLW
        If IsQuoteEndorsement() AndAlso TypeOfEndorsement() = "Add/Delete Vehicle" Then
            If IsNewVehicleOnEndorsement(MyVehicle) Then
                ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.RemoveAdd, VehicleIndex, MyVehicle)
                'Set the TransactionReasonId back to 10168 if there are no other vehicles added to the endorsement.
                Dim hasAddedVehicle As Boolean = False
                Dim vehicleList = ddh.GetVehicleDictionary
                For Each vehicle In vehicleList
                    If vehicle.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem Then
                        hasAddedVehicle = True
                        Exit For
                    End If
                Next
                If hasAddedVehicle = False Then
                    Quote.TransactionReasonId = 10168 'Endorsement Change Dec Only
                End If
            Else
                ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Delete, VehicleIndex, MyVehicle)

            End If
            Dim endorsementsRemarksHelper = New EndorsementsRemarksHelper(ddh)
            Dim updatedRemarks As String = endorsementsRemarksHelper.UpdateRemarks(EndorsementsRemarksHelper.RemarksType.Vehicle)
            Quote.TransactionRemark = updatedRemarks
        End If

        'Added 07/28/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
        RemoveValidVINFromDevDictionary(VehicleIndex)

        RaiseEvent DeleteVehicleRequested(VehicleIndex)
    End Sub

    'Added 07/29/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    Private Sub RemoveValidVINFromDevDictionary(vehicleIndex As Integer)
        Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
        vinDDH.RemoveFromMasterValueDictionary(vehicleIndex + 1)
        RenumberValidVINDevDictionary(vehicleIndex)
    End Sub

    'Added 07/29/2021 for CAP Endorsements Tasks 53028 and 53030 MLW
    Private Sub RenumberValidVINDevDictionary(vehicleIndex As Integer)
        Dim vinDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "ValidVIN", Quote.LobType)
        Dim validVINList As Dictionary(Of String, String) = vinDDH.GetMasterValueAsDictionary()
        Dim newValidVINList As Dictionary(Of String, String) = New Dictionary(Of String, String)
        If validVINList IsNot Nothing AndAlso validVINList.Count > 0 Then
            For Each vv In validVINList
                Dim currPosition As Integer
                currPosition = QQHelper.IntegerForString(vv.Key)
                If currPosition > 0 Then
                    If currPosition < vehicleIndex + 1 Then
                        newValidVINList.Add(vv.Key, vv.Value)
                    ElseIf currPosition > vehicleIndex + 1 Then
                        newValidVINList.Add((currPosition - 1).ToString(), vv.Value)
                    End If
                End If
            Next
            vinDDH.SetMasterValueFromDictionary(newValidVINList)
        End If
    End Sub

    Private Sub btnAddVehicle_Click(sender As Object, e As EventArgs) Handles btnAddVehicle.Click
        RaiseEvent NewVehicleRequested()
    End Sub

    Private Sub chkUseGaragingAddress_CheckedChanged(sender As Object, e As EventArgs) Handles chkUseGaragingAddress.CheckedChanged
        If chkUseGaragingAddress.Checked Then
            If Quote IsNot Nothing AndAlso (Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0) Then
                ' Location 0 exists, populate from there
                Dim loc As QuickQuote.CommonObjects.QuickQuoteLocation = Quote.Locations(0)
                txtZip.Text = loc.Address.Zip
                txtCity.Text = loc.Address.City
                txtState.Text = loc.Address.State
                txtCounty.Text = loc.Address.County
                DisableGaragingFields()
            Else
                ' Location 0 does not exist, populate from Policyholder
                Dim ph As QuickQuote.CommonObjects.QuickQuotePolicyholder = Quote.Policyholder
                txtZip.Text = ph.Address.Zip
                txtCity.Text = ph.Address.City
                txtState.Text = ph.Address.State
                txtCounty.Text = ph.Address.County
                DisableGaragingFields()
            End If
            'UMPD
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                PopulateUMPD(GetQuoteToUse())
            End If
        Else
            txtZip.Text = ""
            txtCity.Text = ""
            txtState.Text = ""
            txtCounty.Text = ""
            EnableGaragingFields()
        End If
    End Sub

    Private Sub lnkCopy_Click(sender As Object, e As EventArgs) Handles lnkCopy.Click
        RaiseEvent CopyVehicleRequested(Me.VehicleIndex)
    End Sub

    Private Function checkValid_Data_and_DDLListItem_andSelect(ddlItem As DropDownList, selectionData As String) As Boolean
        If IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(selectionData) Then
            If ddlItem.Items IsNot Nothing AndAlso ddlItem.Items.Count > 0 AndAlso ddlItem.Items.FindByValue(selectionData) IsNot Nothing Then
                ddlItem.SelectedValue = selectionData
                Return True
            End If
        End If
        Return False
    End Function

    'Added 02/17/2021 for CAP Endorsements Task 52974 MLW
    Private Function checkValid_Data_and_DDLListItem_andSelect_Endorsements(ddlItem As DropDownList, selectionData As String) As Boolean
        If (IFM.Common.InputValidation.CommonValidations.IsPositiveWholeNumber(selectionData) OrElse selectionData = 0) Then
            If ddlItem.Items IsNot Nothing AndAlso ddlItem.Items.Count > 0 AndAlso ddlItem.Items.FindByValue(selectionData) IsNot Nothing Then
                ddlItem.SelectedValue = selectionData
                Return True
            End If
        End If
        Return False
    End Function

    'added 5/19/2021 for CAP Endorsements Task 52974 MLW
    Public Sub PopulateAppVehicleInfo()
        Me.ctlVehicle.Populate()
    End Sub

    'added 6/14/2021 for CAP Endorsements Task 52974 MLW
    Private Sub ctlVehicle_NeedToRepopulateTopLevelAIs() Handles ctlVehicle.NeedToRepopulateTopLevelAIs
        RaiseEvent NeedToRepopulateTopLevelAIs()
    End Sub
    'added 02/23/2022 to reset CAP vehicle details task 67489 BD
    Private Sub btnVinReset_Click(sender As Object, e As EventArgs) Handles btnVinReset.Click
        ResetVehicleControl()
    End Sub

    Public Sub ResetVehicleControl()

        Save_FireSaveEvent(False)
        Me.txtVIN.Text = ""
        Me.txtCostNew.Text = ""
        Me.txtVehicleYear.Text = ""
        Me.txtVehicleMake.Text = ""
        Me.txtVehicleModel.Text = ""
        Me.txtClassCode.Text = ""

        Me.ddlVehicleRatingType.SelectedIndex = -1

        Me.hdnSize.Value = ""
        Me.hdnValidVin.Value = False
        Me.ddlSize.Attributes.Remove("disabled")
        For Each li As ListItem In Me.ddlVehicleRatingType.Items
            li.Attributes.Remove("disabled")
        Next

        Save_FireSaveEvent(False)

    End Sub

    Private Function VehicleHasCustomWrapOrPaintJob(vehicleIndex As Integer) As Boolean
        Dim customWrapOrPaintJob As String
        Dim customWrapOrPaintJobDDH As DevDictionaryHelper.DevDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, "CustomWrapOrPaintJob", Quote.LobType)

        customWrapOrPaintJob = customWrapOrPaintJobDDH.GetValueFromMasterValueDictionaryByKey(vehicleIndex + 1)

        If IsNullEmptyorWhitespace(customWrapOrPaintJob) Then
            customWrapOrPaintJob = "False"
        End If

        Return customWrapOrPaintJob = "True"
    End Function

End Class