Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.CGL
Imports IFM.VR.Common.Helpers.CL
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Public Class ctl_CPR_Coverages
    Inherits VRControlBase

    Public Event BlanketDeductibleChanged()
    Public Event AgreedAmountChanged(ByVal newvalue As Boolean)
    Public Event NeedToReloadCIMTransportation()
    Public Event NeedToClearCIMTransportation()

    Private ReadOnly Property QuoteISCPP As Boolean
        Get
            If Quote IsNot Nothing Then
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then Return True Else Return False
            Else
                Return False
            End If
        End Get
    End Property

    'added 7/29/2021
    Private Function OkayToDisplayFoodManufacturers(Optional ByVal newDate As String = "") As Boolean
        Dim isOkay As Boolean = False
        If Quote IsNot Nothing AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            If QQHelper.IsValidDateString(newDate, mustBeGreaterThanDefaultDate:=True) = False Then
                newDate = Quote.EffectiveDate
            End If
            If QQHelper.DateForString(newDate) >= QQHelper.DateForString(QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_EffectiveDate) Then
                If QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_Enabled() = True OrElse (SubQuoteFirst.HasFoodManufacturersEnhancement = True AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_MaintainCoverageWhenDisabled() = True) Then
                    isOkay = True
                End If
            End If
        End If
        Return isOkay
    End Function
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(divGenInfo.ClientID, Me.hdnAccord, "0")

        Me.VRScript.StopEventPropagation(lnkSave.ClientID)

        Me.VRScript.CreateJSBinding(Me.chkBusinessIncomeALS, ctlPageStartupScript.JsEventType.onchange, $"Cpr.BusinessIncomeALSChanged('{chkBusinessIncomeALS.ClientID}','{trBIALSLimitRow.ClientID}','{trBIALSInfoRow.ClientID}','{trBIALSWaitingPeriodRow.ClientID}','{hdnEffDate.ClientID}');")

        Me.VRScript.CreateJSBinding(Me.ddBlanketRating, ctlPageStartupScript.JsEventType.onchange, "Cpr.BlanketRatingChanged('" & ddBlanketRating.ClientID & "','" & trBlanketCauseOfLossRow.ClientID & "','" & trBlanketCoinsuranceRow.ClientID & "','" & trBlanketValuationRow.ClientID & "','" & trAgreedAmountRow.ClientID & "','" & trAgreedAmountInfoRow.ClientID & "','" & trDeductibleRow.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkAgreedAmount, ctlPageStartupScript.JsEventType.onchange, "Cpr.CPRAgreedAmountChanged('" & chkAgreedAmount.ClientID & "','" & hdnAgreedAmountValue.ClientID & "');")

        ' CPP Specific
        'Me.VRScript.CreateJSBinding(Me.ddPackageModificationAssignmentType, ctlPageStartupScript.JsEventType.onchange, "Cpp.PackageModificationChanged('" & ddPackageModificationAssignmentType.ClientID & "','" & trPkgModApartmentTypeInfoRow.ClientID & "','" & trPkgModContractorsTypeInfoRow.ClientID & "','" & trPkgModRestaurantTypeInfoRow.ClientID & "','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "');")
        'Updated 6/29/2022 for task 75037 MLW
        'Me.VRScript.CreateJSBinding(Me.ddPackageModificationAssignmentType, ctlPageStartupScript.JsEventType.onchange, "Cpp.PackageModificationChanged('" & ddPackageModificationAssignmentType.ClientID & "','" & trPkgModApartmentTypeInfoRow.ClientID & "','" & trPkgModContractorsTypeInfoRow.ClientID & "','" & trPkgModRestaurantTypeInfoRow.ClientID & "','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.ddPackageModificationAssignmentType, ctlPageStartupScript.JsEventType.onchange, "Cpp.PackageModificationChanged('" & ddPackageModificationAssignmentType.ClientID & "','" & trPkgModApartmentTypeInfoRow.ClientID & "','" & trPkgModContractorsTypeInfoRow.ClientID & "','" & trPkgModRestaurantTypeInfoRow.ClientID & "','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "','" & chkPlusEnhancement.ClientID & "','" & trPkgModHotelTypeInfoRow.ClientID & "');")

        ' Added Food Manufacturer's 6/28/21 MGB
        'Updated 6/29/2022 for task 75037 MLW
        'Me.VRScript.CreateJSBinding(Me.chkEnhancement, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('PROP','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "');")
        'Me.VRScript.CreateJSBinding(Me.chkContractorsEnhancementPackage, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('CONT','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "');")
        'Me.VRScript.CreateJSBinding(Me.chkManufacturersEnhancementPackage, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('MANUF','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "');")
        'Me.VRScript.CreateJSBinding(Me.chkFoodManufacturersEnhancementPackage, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('FOODMANUF','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkEnhancement, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('PROP','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "','" & chkPlusEnhancement.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkContractorsEnhancementPackage, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('CONT','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "','" & chkPlusEnhancement.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkManufacturersEnhancementPackage, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('MANUF','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "','" & chkPlusEnhancement.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkFoodManufacturersEnhancementPackage, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('FOODMANUF','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "','" & chkPlusEnhancement.ClientID & "');")
        'Added 6/29/2022 for task 75037 MLW
        If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
            Me.VRScript.CreateJSBinding(Me.chkPlusEnhancement, ctlPageStartupScript.JsEventType.onchange, "Cpp.PropertyOrContractorsOrManufacturersEnhancementChanged('PROPPLUS','" & chkEnhancement.ClientID & "','" & chkContractorsEnhancementPackage.ClientID & "','" & trContractorsEnhancementInfoRow.ClientID & "','" & chkManufacturersEnhancementPackage.ClientID & "','" & trManufacturersEnhancementInfoRow.ClientID & "','" & chkFoodManufacturersEnhancementPackage.ClientID & "','" & trFoodManufacturersEnhancementInfoRow.ClientID & "','" & chkPlusEnhancement.ClientID & "');")
        End If

        ' Handles changes to the Policy Type dropdown Preferred Option updated 2/4/21 
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            'Me.ddPolicyType.SelectedValue = "60"   ' we can't set this here or it will override what's been selected. Bug 64233 MGB 9/21/21
            Me.ddPolicyType.CssClass = "CPR_StdDdl"
            Dim preferredPopupMessage As String = "<div><b>To qualify for the preferred program, the risk must have the following criteria-</b></div><div>"
            preferredPopupMessage = preferredPopupMessage & "• 3 year policy loss ratio of 55% or less.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Building age of 25 years or less.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Building age more than 25 years of age that have had major upgrades to roof, hvac, plumbing within the past 10 years and shown to be in better than average condition based on photos and/or loss control.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Business is managed by an experienced insured (minimum of 3 years of experience) in the same business.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Insured location has additional property safeguards than the average risk such as automatic sprinkler systems, central station alarms, fenced or otherwise better protected from losses.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Above average maintenance and housekeeping verified by agent, loss control inspection or other reliable source.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Property located in Protection Class 7 or better.<br />"
            preferredPopupMessage = preferredPopupMessage & "• Exposure with Risk Grade 1 or 2.<br />"
            preferredPopupMessage = preferredPopupMessage & "•Risk has a formal safety program in place as confirmed by agent or loss control.<br />"
            preferredPopupMessage = preferredPopupMessage & "</div>"
            'updated 2/2/21 
            Using popupSpecial As New PopupMessageClass.PopupMessageObject(Me.Page, preferredPopupMessage, "Preferred Rating Program Guidelines")
                With popupSpecial
                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onchange
                    .DropDownValueToBindTo = "61"
                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                    .isModal = False
                    .AddButton("OK", True)
                    .width = 550
                    .height = 500
                    .AddControlToBindTo(ddPolicyType)
                    .divId = "ddPolicyTypePopup"
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End If

        Exit Sub
    End Sub

    ''' <summary>
    ''' Updates the script that shows the data rows for the Business Income ALS coverage.
    ''' When the effective date changes the Waiting Period row may or may not be shown.
    ''' Uses the NewEffDate value when passed, otherwise uses the effective date on the quote.
    ''' </summary>
    ''' <param name="NewEffDate"></param>
    Private Sub UpdateBusinessIncomeALSScript(Optional NewEffDate As String = Nothing)
        Dim effdt As String = Nothing
        If NewEffDate IsNot Nothing AndAlso IsDate(NewEffDate) Then
            effdt = NewEffDate
        Else
            If Quote IsNot Nothing Then effdt = Quote.EffectiveDate
        End If
        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddPolicyType.Items Is Nothing OrElse ddPolicyType.Items.Count <= 0 Then
            ' Program
            QQHelper.LoadStaticDataOptionsDropDown(ddPolicyType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTypeId, , Quote.LobType)
            ' Blanket Rating
            QQHelper.LoadStaticDataOptionsDropDown(ddBlanketRating, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketRatingOptionId, , Quote.LobType)
            ' Blanket Cause of Loss
            QQHelper.LoadStaticDataOptionsDropDown(ddBlanketCauseOfLoss, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketBuildingAndContentsCauseOfLossTypeId, , Quote.LobType)
            ' Blanket Coinsurance
            QQHelper.LoadStaticDataOptionsDropDown(ddBlanketCoinsurance, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketBuildingAndContentsCoinsuranceTypeId, , Quote.LobType)
            ' Blanket Valuation
            QQHelper.LoadStaticDataOptionsDropDown(ddBlanketValuation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BlanketBuildingAndContentsValuationId, , Quote.LobType)
            '' Deductible -  moved to LoadBlanketDeductible()
            'QQHelper.LoadStaticDataOptionsDropDown(ddDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
            '' Waiting Period
            QQHelper.LoadStaticDataOptionsDropDown(ddBusinessIncomeALSWaitingPeriod, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.BusinessIncomeCov_WaitingPeriodTypeId, , Quote.LobType)

            ' CPP Specific
            ' Package Type
            QQHelper.LoadStaticDataOptionsDropDown(ddPackageType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PackageTypeId, , Quote.LobType)
            ' Package Modification
            QQHelper.LoadStaticDataOptionsDropDown(ddPackageModificationAssignmentType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PackageModificationAssignmentTypeId, , Quote.LobType)

            If HotelMotelRemovedRisks.IsHotelMotelRemovedRisksAvailable(Quote) = True AndAlso IsQuoteEndorsement() = False Then
                Dim cprValueToRemove As String = "MOTEL/HOTEL"
                If ddPackageModificationAssignmentType.Items.FindByText(cprValueToRemove) IsNot Nothing Then
                    ddPackageModificationAssignmentType.Items.Remove(ddPackageModificationAssignmentType.Items.FindByText(cprValueToRemove))
                End If
            End If

            ddBlanketValuation.Items.Remove(New ListItem("FUNCTIONAL REPLACEMENT COST", "7"))

            LoadBlanketDeductible()
        End If

        Exit Sub
    End Sub

    Public Sub EffectiveDateChanging(ByVal NewEffDate As String, ByVal OldEffDate As String)
        ' Show or hide the waiting period dropdown based on the new effective date
        ' Waiting period is shown if effective date is 3/1/2020 or greater
        ' MGB 3/2/2020 Task 41357

        hdnEffDate.Value = NewEffDate

        trBIALSInfoRow.Attributes.Add("style", "display:none")
        trBIALSLimitRow.Attributes.Add("style", "display:none")
        trBIALSWaitingPeriodRow.Attributes.Add("style", "display:none")

        If chkBusinessIncomeALS.Checked Then
            trBIALSInfoRow.Attributes.Add("style", "display:''")
            trBIALSLimitRow.Attributes.Add("style", "display:''")
            If NewEffDate IsNot Nothing AndAlso IsDate(NewEffDate) Then
                If CDate(NewEffDate) >= CDate("3/1/2020") Then
                    trBIALSWaitingPeriodRow.Attributes.Add("style", "display:''")
                End If
            End If
        End If

        ' Food Manufacturing
        If Quote IsNot Nothing AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            'If CDate(NewEffDate) < CDate(QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_EffectiveDate) Then
            If OkayToDisplayFoodManufacturers(newDate:=NewEffDate) = False Then
                ' Food Manufacturing not in effect yet - uncheck and hide the checkbox, hide the info row
                chkFoodManufacturersEnhancementPackage.Checked = False
                chkFoodManufacturersEnhancementPackage.Enabled = False
                trFoodManufacturersEnhancementRow.Attributes.Add("style", "display:none")
                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                ' If food manufacturing was originally checked, the other enhancement checkboxes would be disabled,
                ' so we need to re-enable them here
                chkEnhancement.Enabled = True
                chkContractorsEnhancementPackage.Enabled = True
                chkManufacturersEnhancementPackage.Enabled = True

                If GoverningStateQuote.HasFoodManufacturersEnhancement = True Then
                    For Each sq In Me.SubQuotes
                        sq.HasFoodManufacturersEnhancement = False 'this is done since Transportation control will check it and set defaults for FMEE if needed
                    Next

                    RaiseEvent NeedToClearCIMTransportation()
                    RaiseEvent NeedToReloadCIMTransportation()
                End If
            Else
                ' Food Manufacturing is in effect - show the checkbox, populate it, and show the info row if checked
                chkFoodManufacturersEnhancementPackage.Enabled = True
                trFoodManufacturersEnhancementRow.Attributes.Add("style", "display:''")
                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                chkFoodManufacturersEnhancementPackage.Checked = SubQuoteFirst.HasFoodManufacturersEnhancement
                If chkFoodManufacturersEnhancementPackage.Checked Then
                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:''")
                End If
            End If
        End If

        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        Dim BlanketID As String = Nothing
        Dim showFoodManufacturers As Boolean = False

        LoadStaticData()

        If QuoteISCPP Then
            btnSubmit.Text = "Save Property Policy Level Coverages"
        Else
            btnSubmit.Text = "Save Policy Level Coverages"
        End If

        'Updated 12/08/2021 for CPP Endorsements Task 65084 MLW
        If Not IsQuoteReadOnly() Then
            ' SET THE DEFAULTS
            ' Policy Type: Standard
            SetFromValue(ddPolicyType, "0")
            ' Package Modification Assignment Type
            SetFromValue(ddPackageModificationAssignmentType, "0")
            ' Blanket Rating: N/A
            SetFromValue(ddBlanketRating, "0")
            ' Cause of Loss
            SetFromValue(Me.ddBlanketCauseOfLoss, "3")
            ' Coinsurance
            SetFromValue(Me.ddBlanketCoinsurance, "6")
            ' Valuation
            SetFromValue(Me.ddBlanketValuation, "1")
            ' Deductible
            SetFromValue(ddDeductible, "9")  ' 500 changed to 1000 per task 62836

        End If

        ' Set the hidden value effective date
        hdnEffDate.Value = Quote.EffectiveDate

        ' Hide rows
        trBlanketCauseOfLossRow.Attributes.Add("style", "display:none")
        trBlanketCoinsuranceRow.Attributes.Add("style", "display:none")
        trBlanketValuationRow.Attributes.Add("style", "display:none")
        trAgreedAmountRow.Attributes.Add("style", "display:none")
        trAgreedAmountInfoRow.Attributes.Add("style", "display:none")
        trBIALSLimitRow.Attributes.Add("style", "display:none")
        trBIALSInfoRow.Attributes.Add("style", "display:none")
        trBIALSWaitingPeriodRow.Attributes.Add("style", "display:none")
        ' CPP
        trPackageTypeRow.Attributes.Add("style", "display:none")
        trPackageModificationAssignmentTypeRow.Attributes.Add("style", "display:none")
        trPkgModApartmentTypeInfoRow.Attributes.Add("style", "display:none")
        trPkgModContractorsTypeInfoRow.Attributes.Add("style", "display:none")
        trPkgModRestaurantTypeInfoRow.Attributes.Add("style", "display:none")
        trDeductibleRow.Attributes.Add("style", "display:none")
        trContractorsEnhancementRow.Attributes.Add("style", "display:none")
        trManufacturersEnhancementRow.Attributes.Add("style", "display:none")
        trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
        trFoodManufacturersEnhancementRow.Attributes.Add("style", "display:none")
        trPkgModHotelTypeInfoRow.Attributes.Add("style", "display:none")

        'Added 6/28/2022 for task 75037 MLW
        trPlusEnhancementRow.Attributes.Add("style", "display:none")
        If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
            trPlusEnhancementRow.Attributes.Add("style", "display:''")
        End If

        ' POPULATE
        If Me.Quote IsNot Nothing Then
            'Updated 12/14/18 for multi state bug 30354 MLW
            ' CPP specific fields
            ' - Package Type
            ' - Package Modification Assignment Type
            If QuoteISCPP Then
                If Quote.PolicyTypeId <> "" Then SetFromValue(ddPolicyType, Quote.PolicyTypeId)
                SetFromValue(ddPackageType, Quote.PackageTypeId)
                trPackageModificationAssignmentTypeRow.Attributes.Add("style", "display:''")
                SetFromValue(ddPackageModificationAssignmentType, Quote.PackageModificationAssignmentTypeId)
                Select Case ddPackageModificationAssignmentType.SelectedValue
                    Case "1"   ' Apartment
                        trPkgModApartmentTypeInfoRow.Attributes.Add("style", "display:''")
                        Exit Select
                    Case "2"   ' Contractors
                        trPkgModContractorsTypeInfoRow.Attributes.Add("style", "display:''")
                        Exit Select
                    Case "5"   ' Mercantile
                        trPkgModRestaurantTypeInfoRow.Attributes.Add("style", "display:''")
                        Exit Select
                    Case "6" 'Motel/Hotel
                        If HotelMotelRemovedRisks.IsHotelMotelRemovedRisksAvailable(Quote) = True AndAlso IsQuoteEndorsement() = False Then
                            trPkgModHotelTypeInfoRow.Attributes.Add("style", "display:none")
                        Else
                            trPkgModHotelTypeInfoRow.Attributes.Add("style", "display:''")
                        End If
                    Case Else
                        Exit Select
                End Select
            Else
                If Me.SubQuoteFirst IsNot Nothing Then
                    If SubQuoteFirst.PolicyTypeId <> "" Then SetFromValue(ddPolicyType, SubQuoteFirst.PolicyTypeId)
                End If
            End If
            If Me.SubQuoteFirst IsNot Nothing Then
                'If SubQuoteFirst.PolicyTypeId <> "" Then SetFromValue(ddPolicyType, SubQuoteFirst.PolicyTypeId)

                ' CPP specific fields
                ' - Package Type
                ' - Package Modification Assignment Type
                ' - Contractors Enhancement
                ' - Manufacturers Enhancement
                If QuoteISCPP Then
                    trContractorsEnhancementRow.Attributes.Add("style", "display:''")
                    trManufacturersEnhancementRow.Attributes.Add("style", "display:''")
                    'If CDate(Quote.EffectiveDate) >= CDate(QuickQuote.CommonMethods.QuickQuoteHelperClass.FoodManufacturers_EffectiveDate) Then
                    If OkayToDisplayFoodManufacturers() = True Then
                        showFoodManufacturers = True
                        trFoodManufacturersEnhancementRow.Attributes.Add("style", "display:''")
                    End If
                    ' Don't show dropdowns for PackageType (if CPP). Logic is here to support legacy policies and endorsements. B42921
                    If (SubQuoteFirst.PackageTypeId) = "2" Then
                        trPackageTypeRow.Attributes.Add("style", "display:''")
                    End If

                    ' Set all of the enhancements to enabled and unchecked, hide the info rows
                    chkEnhancement.Checked = False
                    chkEnhancement.Enabled = True
                    'Added 6/28/2022 for task 75037 MLW
                    If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                        chkPlusEnhancement.Checked = False
                        chkPlusEnhancement.Enabled = True
                    End If
                    chkContractorsEnhancementPackage.Checked = False
                    chkContractorsEnhancementPackage.Enabled = True
                    trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                    chkManufacturersEnhancementPackage.Checked = False
                    chkManufacturersEnhancementPackage.Enabled = True
                    trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                    chkFoodManufacturersEnhancementPackage.Checked = False
                    chkFoodManufacturersEnhancementPackage.Enabled = True
                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")

                    ' Enhancements
                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                        If SubQuoteFirst.PackageModificationAssignmentTypeId IsNot Nothing AndAlso SubQuoteFirst.PackageModificationAssignmentTypeId <> "" Then
                            If SubQuoteFirst.PackageModificationAssignmentTypeId = "1" Then
                                ' When Pkg Mod Type is Apartment (ID 1), disable the other enhancement checkboxes
                                ' Bug 52991 5/17/21/ MGB
                                chkEnhancement.Enabled = False
                                'Added 6/28/2022 for task 75037 MLW
                                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                    chkPlusEnhancement.Enabled = False
                                End If
                                chkContractorsEnhancementPackage.Enabled = False
                                chkManufacturersEnhancementPackage.Enabled = False
                                chkFoodManufacturersEnhancementPackage.Enabled = False
                            Else
                                ' Pkg Mod Type is not Apartment.  Set the enhancement checkboxes.
                                If SubQuoteFirst.Has_PackageCPR_EnhancementEndorsement Then
                                    chkEnhancement.Checked = True
                                    chkEnhancement.Enabled = True
                                    'Added 6/28/2022 for task 75037 MLW
                                    If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                        chkPlusEnhancement.Checked = False
                                        chkPlusEnhancement.Enabled = False
                                    End If
                                    chkContractorsEnhancementPackage.Checked = False
                                    chkContractorsEnhancementPackage.Enabled = False
                                    trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkManufacturersEnhancementPackage.Checked = False
                                    chkManufacturersEnhancementPackage.Enabled = False
                                    trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkFoodManufacturersEnhancementPackage.Checked = False
                                    chkFoodManufacturersEnhancementPackage.Enabled = False
                                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                ElseIf SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement Then
                                    'Added 6/28/2022 for task 75037 MLW
                                    chkEnhancement.Checked = False
                                    chkEnhancement.Enabled = False
                                    'Added 6/28/2022 for task 75037 MLW
                                    If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                        chkPlusEnhancement.Checked = True
                                        chkPlusEnhancement.Enabled = True
                                    End If
                                    chkContractorsEnhancementPackage.Checked = False
                                    chkContractorsEnhancementPackage.Enabled = False
                                    trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkManufacturersEnhancementPackage.Checked = False
                                    chkManufacturersEnhancementPackage.Enabled = False
                                    trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkFoodManufacturersEnhancementPackage.Checked = False
                                    chkFoodManufacturersEnhancementPackage.Enabled = False
                                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                ElseIf SubQuoteFirst.HasContractorsEnhancement Then
                                    chkEnhancement.Checked = False
                                    chkEnhancement.Enabled = False
                                    'Added 6/28/2022 for task 75037 MLW
                                    If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                        chkPlusEnhancement.Checked = False
                                        chkPlusEnhancement.Enabled = False
                                    End If
                                    chkContractorsEnhancementPackage.Checked = True
                                    chkContractorsEnhancementPackage.Enabled = True
                                    trContractorsEnhancementInfoRow.Attributes.Add("style", "display:''")
                                    chkManufacturersEnhancementPackage.Checked = False
                                    chkManufacturersEnhancementPackage.Enabled = False
                                    trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkFoodManufacturersEnhancementPackage.Checked = False
                                    chkFoodManufacturersEnhancementPackage.Enabled = False
                                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                ElseIf SubQuoteFirst.HasManufacturersEnhancement Then
                                    chkEnhancement.Checked = False
                                    chkEnhancement.Enabled = False
                                    'Added 6/28/2022 for task 75037 MLW
                                    If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                        chkPlusEnhancement.Checked = False
                                        chkPlusEnhancement.Enabled = False
                                    End If
                                    chkManufacturersEnhancementPackage.Checked = True
                                    chkManufacturersEnhancementPackage.Enabled = True
                                    trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:''")
                                    chkContractorsEnhancementPackage.Checked = False
                                    chkContractorsEnhancementPackage.Enabled = False
                                    trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkFoodManufacturersEnhancementPackage.Checked = False
                                    chkFoodManufacturersEnhancementPackage.Enabled = False
                                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    'ElseIf SubQuoteFirst.HasFoodManufacturersEnhancement Then
                                ElseIf showFoodManufacturers = True AndAlso SubQuoteFirst.HasFoodManufacturersEnhancement Then
                                    chkEnhancement.Checked = False
                                    chkEnhancement.Enabled = False
                                    'Added 6/28/2022 for task 75037 MLW
                                    If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                        chkPlusEnhancement.Checked = False
                                        chkPlusEnhancement.Enabled = False
                                    End If
                                    chkManufacturersEnhancementPackage.Checked = False
                                    chkManufacturersEnhancementPackage.Enabled = False
                                    trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkContractorsEnhancementPackage.Checked = False
                                    chkContractorsEnhancementPackage.Enabled = False
                                    trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                    chkFoodManufacturersEnhancementPackage.Checked = True
                                    chkFoodManufacturersEnhancementPackage.Enabled = True
                                    trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:''")
                                End If
                            End If
                        Else
                            ' Pkg Mod Type is nothing or empty?? we should not get here but if we do we want some code here.
                            If SubQuoteFirst.Has_PackageCPR_EnhancementEndorsement Then
                                chkEnhancement.Checked = True
                                chkEnhancement.Enabled = True
                                'Added 6/28/2022 for task 75037 MLW
                                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                    chkPlusEnhancement.Checked = False
                                    chkPlusEnhancement.Enabled = False
                                End If
                                chkContractorsEnhancementPackage.Checked = False
                                chkContractorsEnhancementPackage.Enabled = False
                                trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkManufacturersEnhancementPackage.Checked = False
                                chkManufacturersEnhancementPackage.Enabled = False
                                trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkFoodManufacturersEnhancementPackage.Checked = False
                                chkFoodManufacturersEnhancementPackage.Enabled = False
                                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                            ElseIf SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement Then
                                'Added 6/28/2022 for task 75037 MLW
                                chkEnhancement.Checked = False
                                chkEnhancement.Enabled = False
                                'Added 6/28/2022 for task 75037 MLW
                                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                    chkPlusEnhancement.Checked = True
                                    chkPlusEnhancement.Enabled = True
                                End If
                                chkContractorsEnhancementPackage.Checked = False
                                chkContractorsEnhancementPackage.Enabled = False
                                trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkManufacturersEnhancementPackage.Checked = False
                                chkManufacturersEnhancementPackage.Enabled = False
                                trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkFoodManufacturersEnhancementPackage.Checked = False
                                chkFoodManufacturersEnhancementPackage.Enabled = False
                                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                            ElseIf SubQuoteFirst.HasContractorsEnhancement Then
                                chkEnhancement.Checked = False
                                chkEnhancement.Enabled = False
                                'Added 6/28/2022 for task 75037 MLW
                                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                    chkPlusEnhancement.Checked = False
                                    chkPlusEnhancement.Enabled = False
                                End If
                                chkContractorsEnhancementPackage.Checked = True
                                chkContractorsEnhancementPackage.Enabled = True
                                trContractorsEnhancementInfoRow.Attributes.Add("style", "display:''")
                                chkManufacturersEnhancementPackage.Checked = False
                                chkManufacturersEnhancementPackage.Enabled = False
                                trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkFoodManufacturersEnhancementPackage.Checked = False
                                chkFoodManufacturersEnhancementPackage.Enabled = False
                                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                            ElseIf SubQuoteFirst.HasManufacturersEnhancement Then
                                chkEnhancement.Checked = False
                                chkEnhancement.Enabled = False
                                'Added 6/28/2022 for task 75037 MLW
                                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                    chkPlusEnhancement.Checked = False
                                    chkPlusEnhancement.Enabled = False
                                End If
                                chkManufacturersEnhancementPackage.Checked = True
                                chkManufacturersEnhancementPackage.Enabled = True
                                trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:''")
                                chkContractorsEnhancementPackage.Checked = False
                                chkContractorsEnhancementPackage.Enabled = False
                                trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkFoodManufacturersEnhancementPackage.Checked = False
                                chkFoodManufacturersEnhancementPackage.Enabled = False
                                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                'ElseIf SubQuoteFirst.HasFoodManufacturersEnhancement Then
                            ElseIf showFoodManufacturers = True AndAlso SubQuoteFirst.HasFoodManufacturersEnhancement Then
                                chkEnhancement.Checked = False
                                chkEnhancement.Enabled = False
                                'Added 6/28/2022 for task 75037 MLW
                                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                    chkPlusEnhancement.Checked = False
                                    chkPlusEnhancement.Enabled = False
                                End If
                                chkManufacturersEnhancementPackage.Checked = False
                                chkManufacturersEnhancementPackage.Enabled = False
                                trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkContractorsEnhancementPackage.Checked = False
                                chkContractorsEnhancementPackage.Enabled = False
                                trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                                chkFoodManufacturersEnhancementPackage.Checked = True
                                chkFoodManufacturersEnhancementPackage.Enabled = True
                                trFoodManufacturersEnhancementInfoRow.Attributes.Add("style", "display:''")
                            End If
                        End If
                    Else
                        ' NOT CPP
                        ' If Contractors Enhancement is selected, Manufacturers is disabled and vice-versa
                        If SubQuoteFirst.HasBusinessMasterEnhancement Then
                            chkEnhancement.Checked = True
                            chkEnhancement.Enabled = True
                            'Added 6/28/2022 for task 75037 MLW
                            If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                chkPlusEnhancement.Checked = False
                                chkPlusEnhancement.Enabled = False
                            End If
                            chkContractorsEnhancementPackage.Checked = False
                            chkContractorsEnhancementPackage.Enabled = False
                            trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                            chkManufacturersEnhancementPackage.Checked = False
                            chkManufacturersEnhancementPackage.Enabled = False
                            trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                        ElseIf SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement Then
                            'Added 6/28/2022 for task 75037 MLW
                            chkEnhancement.Checked = False
                            chkEnhancement.Enabled = False
                            If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                chkPlusEnhancement.Checked = True
                                chkPlusEnhancement.Enabled = True
                            End If
                            chkContractorsEnhancementPackage.Checked = False
                            chkContractorsEnhancementPackage.Enabled = False
                            trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                            chkManufacturersEnhancementPackage.Checked = False
                            chkManufacturersEnhancementPackage.Enabled = False
                            trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                        ElseIf SubQuoteFirst.HasContractorsEnhancement Then
                            chkEnhancement.Checked = False
                            chkEnhancement.Enabled = False
                            If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                chkPlusEnhancement.Checked = False
                                chkPlusEnhancement.Enabled = False
                            End If
                            chkContractorsEnhancementPackage.Checked = True
                            chkContractorsEnhancementPackage.Enabled = True
                            trContractorsEnhancementInfoRow.Attributes.Add("style", "display:''")
                            chkManufacturersEnhancementPackage.Checked = False
                            chkManufacturersEnhancementPackage.Enabled = False
                            trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:none")
                        ElseIf SubQuoteFirst.HasManufacturersEnhancement Then
                            chkEnhancement.Checked = False
                            chkEnhancement.Enabled = False
                            If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                                chkPlusEnhancement.Checked = False
                                chkPlusEnhancement.Enabled = False
                            End If
                            chkManufacturersEnhancementPackage.Checked = True
                            chkManufacturersEnhancementPackage.Enabled = True
                            trManufacturersEnhancementInfoRow.Attributes.Add("style", "display:''")
                            chkContractorsEnhancementPackage.Checked = False
                            chkContractorsEnhancementPackage.Enabled = False
                            trContractorsEnhancementInfoRow.Attributes.Add("style", "display:none")
                        End If
                    End If
                End If

                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    chkEnhancement.Checked = SubQuoteFirst.Has_PackageCPR_EnhancementEndorsement
                Else
                    chkEnhancement.Checked = SubQuoteFirst.HasBusinessMasterEnhancement
                End If

                'Added 6/28/2022 for task 75037 MLW
                'Property Plus Enhancement Endorsement
                If CPR.CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                    chkPlusEnhancement.Checked = SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement
                End If

                ' Business Income ALS
                ddBusinessIncomeALSWaitingPeriod.SelectedIndex = 0
                If SubQuoteFirst.HasBusinessIncomeALS Then
                    chkBusinessIncomeALS.Checked = True
                    trBIALSInfoRow.Attributes.Add("style", "display:''")
                    trBIALSLimitRow.Attributes.Add("style", "display:''")

                    ' Only show waiting period dropdown if effective date on or after 3/1/2020
                    ' Note that if the effective date is before 3/1/2020 the waiting period value gets set in the building control
                    If Quote.EffectiveDate >= CDate("3/1/2020") Then
                        trBIALSWaitingPeriodRow.Attributes.Add("style", "display:''")
                        ' Set the waiting period 
                        If Quote.Locations IsNot Nothing Then
                            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                                If L.Buildings IsNot Nothing Then
                                    For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                        If B.BusinessIncomeCov_WaitingPeriodTypeId IsNot Nothing AndAlso B.BusinessIncomeCov_WaitingPeriodTypeId <> String.Empty AndAlso IsNumeric(B.BusinessIncomeCov_WaitingPeriodTypeId) Then
                                            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddBusinessIncomeALSWaitingPeriod, B.BusinessIncomeCov_WaitingPeriodTypeId)
                                            GoTo exitlocloop
                                        End If
                                    Next
                                End If
                            Next
exitlocloop:
                        End If
                    End If

                    txtBusinessIncomeALSLimit.Text = SubQuoteFirst.BusinessIncomeALSLimit
                Else
                    chkBusinessIncomeALS.Checked = False
                End If

                ' BLANKET & AGREED AMOUNT
                If BlanketHelper_CPR_CPP.Has_CPRCPP_Blanket(SubQuoteFirst) Then
                    ' Show the blanket rows
                    trBlanketCauseOfLossRow.Attributes.Add("style", "display:''")
                    trBlanketCoinsuranceRow.Attributes.Add("style", "display:''")
                    trBlanketValuationRow.Attributes.Add("style", "display:''")
                    trAgreedAmountRow.Attributes.Add("style", "display:''")
                    trAgreedAmountInfoRow.Attributes.Add("style", "display:''")
                    trDeductibleRow.Attributes.Add("style", "display:''")

                    ' Set the blanket dropdowns
                    SetFromValue(Me.ddBlanketCauseOfLoss, BlanketHelper_CPR_CPP.Get_SelectedBlanketCauseOfLossProperty(SubQuoteFirst))
                    SetFromValue(Me.ddBlanketCoinsurance, BlanketHelper_CPR_CPP.Get_SelectedBlanketcoInsuranceProperty(SubQuoteFirst))
                    SetFromValue(Me.ddBlanketValuation, BlanketHelper_CPR_CPP.Get_SelectedBlanketValuationProperty(SubQuoteFirst))
                    ' Blanket deductible is always set on location 0 building 0
                    PopulateBlanketDeductible()
                    'If SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketBuildingAndContents Then
                    '    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                    '    ''Updated 12/01/2021 for CPP Endorsements Task 65084 MLW
                    '    'If IsQuoteReadOnly() Then
                    '    '    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                    '    'Else
                    '    '    SetFromValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId)
                    '    'End If
                    '    ''SetFromValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId)
                    'ElseIf SubQuoteFirst.HasBlanketContents Then
                    '    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                    '    ''Updated 12/01/2021 for CPP Endorsements Task 65084 MLW
                    '    'If IsQuoteReadOnly() Then
                    '    '    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                    '    'Else
                    '    '    SetFromValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId)
                    '    'End If
                    '    ''SetFromValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId)
                    'End If

                    ' Agreed Amount
                    If SubQuoteFirst.HasBlanketBuildingAndContents Then
                        SetFromValue(ddBlanketRating, "1")
                        ' Combined
                        If SubQuoteFirst.BlanketBuildingAndContentsIsAgreedValue Then
                            ' Agreed = true
                            chkAgreedAmount.Checked = True
                            hdnAgreedAmountValue.Value = "1"
                            ddBlanketCoinsurance.SelectedValue = "7"
                            ddBlanketCoinsurance.Attributes.Add("disabled", "true")
                        Else
                            ' Agreed = false    
                            chkAgreedAmount.Checked = False
                            hdnAgreedAmountValue.Value = ""
                            ddBlanketCoinsurance.Attributes.Remove("disabled")
                        End If
                    ElseIf SubQuoteFirst.HasBlanketBuilding Then
                        SetFromValue(ddBlanketRating, "2")
                        If SubQuoteFirst.BlanketBuildingIsAgreedValue Then
                            ' Agreed = true
                            chkAgreedAmount.Checked = True
                            hdnAgreedAmountValue.Value = "1"
                            ddBlanketCoinsurance.SelectedValue = "7"
                            ddBlanketCoinsurance.Attributes.Add("disabled", "true")
                        Else
                            ' Agreed = false    
                            chkAgreedAmount.Checked = False
                            hdnAgreedAmountValue.Value = ""
                            ddBlanketCoinsurance.Attributes.Remove("disabled")
                        End If
                    ElseIf SubQuoteFirst.HasBlanketContents Then
                        ' Personal Property Only
                        SetFromValue(ddBlanketRating, "3")
                        If SubQuoteFirst.BlanketContentsIsAgreedValue Then
                            ' Agreed = true
                            chkAgreedAmount.Checked = True
                            hdnAgreedAmountValue.Value = "1"
                            ddBlanketCoinsurance.SelectedValue = "7"
                            ddBlanketCoinsurance.Attributes.Add("disabled", "true")
                        Else
                            ' Agreed = false    
                            chkAgreedAmount.Checked = False
                            hdnAgreedAmountValue.Value = ""
                            ddBlanketCoinsurance.Attributes.Remove("disabled")
                        End If
                    End If
                End If
            End If
            'Added 11/22/2021 for CPP Endorsements task 64909 MLW
            If IsQuoteReadOnly() Then
                btnSubmit.Visible = False

                Dim policyNumber As String = Me.Quote.PolicyNumber
                Dim imageNum As Integer = 0
                Dim policyId As Integer = 0
                Dim toolTip As String = "Make a change to this policy"
                'Dim qqHelper As New QuickQuoteHelperClass
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
                QuickQuoteHelperClass.configAppSettingValueAsString("")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) Then
                    readOnlyViewPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
                End If

                btnMakeAChange.Enabled = IFM.VR.Web.Helpers.Endorsement_ChangeBtnEnable.IsChangeBtnEnabled(policyNumber, imageNum, policyId, toolTip)
                readOnlyViewPageUrl &= policyId.ToString & "|" & imageNum.ToString
                btnMakeAChange.ToolTip = toolTip
                btnMakeAChange.Attributes.Item("href") = readOnlyViewPageUrl
            Else
                btnMakeAChange.Visible = False
            End If
            If Not IsQuoteReadOnly() Then
                PopulateCPREnhancementEndorsementCheckboxes()
            End If
        End If

        Exit Sub
    End Sub
    Public Sub UpdateBlanketDeductibleFromBuildingZero()
        If ddBlanketRating.SelectedValue <> "0" Then
            Select Case ddBlanketRating.SelectedValue
                Case "1", "2"
                    ' Building & Combined
                    If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).DeductibleId <> "" Then
                            'SetFromValue(ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId)
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                        End If
                    Else
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings(0).DeductibleId <> "" Then
                            'SetFromValue(ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId)
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                        End If
                    End If
                    Exit Select
                Case "3"
                    ' Contents only
                    If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).DeductibleId <> "" Then
                            'SetFromValue(ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId)
                            'WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.PersPropCov_DeductibleId)
                        End If
                    Else
                        If Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings.HasItemAtIndex(0) AndAlso Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId <> "" Then
                            'SetFromValue(ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId)
                            'WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.DeductibleId)
                            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId, QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.PersPropCov_DeductibleId)
                        End If
                    End If
                    Exit Select
            End Select
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub

    'Private ReadOnly Property QuoteHasTransportationCoverage() As Boolean
    '    Get
    '        If String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationCatastropheDeductibleId) = False _
    '            AndAlso (String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationAnyOneOwnedVehicleLimit) = False _
    '            OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationAnyOneOwnedVehicleNumberOfVehicles) = False _
    '            OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.TransportationCatastropheDescription) = False) Then
    '            Return True
    '        End If
    '        Return False
    '    End Get
    'End Property

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            ' B42921: Row removed for future. Legacy Quotes may still have POP ("2"), so we allow for that.
            If QuoteISCPP AndAlso ddPackageType.SelectedValue <> "2" Then
                ddPackageType.SelectedValue = "1"
            End If

            'Updated 12/14/18 for multi state bug 30354 MLW
            If QuoteISCPP Then
                Quote.PolicyTypeId = ddPolicyType.SelectedValue
                Quote.PackageTypeId = ddPackageType.SelectedValue
                Quote.PackageModificationAssignmentTypeId = ddPackageModificationAssignmentType.SelectedValue
                Quote.BlanketRatingOptionId = ddBlanketRating.SelectedValue 'Added 12/18/18 for multi state bug 30442 MLW
            End If

            Dim alreadyHasFoodManufacturers As Boolean = GoverningStateQuote.HasFoodManufacturersEnhancement

            For Each sq In Me.SubQuotes
                ' Policy Type
                sq.PolicyTypeId = ddPolicyType.SelectedValue

                ' CPP Specific Fields
                ' - Package Type
                ' - Package Modification Assignment Type
                ' - Contractors Enhancement
                ' - Manufacturers Enhancement
                ' - Food Manufacturers Enhancement
                sq.CPP_TargetMarketID = "0"
                Quote.CPP_TargetMarketID = "0"
                If QuoteISCPP Then
                    sq.PackageTypeId = ddPackageType.SelectedValue
                    sq.PackageModificationAssignmentTypeId = ddPackageModificationAssignmentType.SelectedValue
                    sq.Has_PackageCPR_EnhancementEndorsement = chkEnhancement.Checked
                    sq.HasContractorsEnhancement = chkContractorsEnhancementPackage.Checked
                    sq.HasManufacturersEnhancement = chkManufacturersEnhancementPackage.Checked
                    sq.HasFoodManufacturersEnhancement = chkFoodManufacturersEnhancementPackage.Checked
                    If sq.HasFoodManufacturersEnhancement Then
                        sq.CPP_TargetMarketID = "9"
                        Quote.CPP_TargetMarketID = "9"
                    End If
                End If

                ' Enhancement
                Select Case Quote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        sq.HasBusinessMasterEnhancement = chkEnhancement.Checked
                        Exit Select
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        sq.Has_PackageCPR_EnhancementEndorsement = chkEnhancement.Checked
                        Exit Select
                End Select

                'Added 6/28/2022 for task 75037 MLW
                'Property Plus Enhancement
                If CPR_PropertyPlusEnhancementEndorsement.IsPropPlusEnhancementAvailable(Quote) Then
                    sq.Has_PackageCPR_PlusEnhancementEndorsement = chkPlusEnhancement.Checked
                End If


                ' On a new quote we may not have a location & building yet so create them here
                If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuoteLocation)
                If Quote.Locations.Count = 0 Then
                    Quote.Locations.Add(New QuickQuoteLocation)
                    If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                        Dim MyLocation = Me.Quote.Locations?.LastOrDefault
                        If MyLocation IsNot Nothing Then
                            MyLocation.FeetToFireHydrant = "1000"
                            MyLocation.MilesToFireDepartment = "5"
                        End If
                    End If
                End If
                If Quote.Locations(0).Buildings Is Nothing Then Quote.Locations(0).Buildings = New List(Of QuickQuoteBuilding)
                If Quote.Locations(0).Buildings.Count = 0 Then Quote.Locations(0).Buildings.Add(New QuickQuoteBuilding)

                ' Clear all the blanket & agreed amount values before saving
                'Quote.HasBlanketBuildingAndContents = False
                sq.BlanketBuildingAndContentsCauseOfLossTypeId = ""
                sq.BlanketBuildingAndContentsCoinsuranceTypeId = ""
                sq.BlanketBuildingAndContentsValuationId = ""
                sq.BlanketBuildingAndContentsIsAgreedValue = False
                sq.BlanketBuildingCauseOfLossTypeId = ""
                sq.BlanketBuildingCoinsuranceTypeId = ""
                sq.BlanketBuildingValuationId = ""
                sq.BlanketBuildingIsAgreedValue = False
                sq.BlanketContentsCauseOfLossTypeId = ""
                sq.BlanketContentsCoinsuranceTypeId = ""
                sq.BlanketContentsValuationId = ""
                sq.BlanketContentsIsAgreedValue = False

                sq.BlanketBuildingAndContentsDeductibleID = ""
                sq.BlanketBuildingDeductibleID = ""
                sq.BlanketContentsDeductibleID = ""

                sq.BlanketRatingOptionId = ddBlanketRating.SelectedValue

                ' Agreed Amount - Save to the blanket coverages at the policy level
                ' 1 = Combined
                ' 2 = Building
                ' 3 = Property
                If chkAgreedAmount.Checked Then
                    ' AGREED AMOUNT CHECKED
                    Select Case ddBlanketRating.SelectedValue
                        Case "0"  ' N/A
                            sq.BlanketBuildingAndContentsIsAgreedValue = False
                            sq.BlanketBuildingIsAgreedValue = False
                            sq.BlanketContentsIsAgreedValue = False
                            Exit Select
                        Case "1"  ' Combined
                            If Not sq.BlanketBuildingAndContentsIsAgreedValue Then
                                sq.BlanketBuildingAndContentsIsAgreedValue = True
                                RaiseEvent AgreedAmountChanged(True)
                            End If
                            Exit Select
                        Case "2"   ' Building
                            If Not sq.BlanketBuildingIsAgreedValue Then
                                sq.BlanketBuildingIsAgreedValue = True
                                RaiseEvent AgreedAmountChanged(True)
                            End If
                            Exit Select
                        Case "3"  ' Personal Property Only
                            If Not sq.BlanketContentsIsAgreedValue Then
                                sq.BlanketContentsIsAgreedValue = True
                                RaiseEvent AgreedAmountChanged(True)
                            End If
                            Exit Select
                    End Select
                Else
                    ' AGREED AMOUNT UN-CHECKED
                    Select Case ddBlanketRating.SelectedValue
                        Case "0"  ' N/A
                            sq.BlanketBuildingAndContentsIsAgreedValue = False
                            sq.BlanketBuildingIsAgreedValue = False
                            sq.BlanketContentsIsAgreedValue = False
                            Exit Select
                        Case "1"  '  Combined
                            If sq.BlanketBuildingAndContentsIsAgreedValue Then
                                sq.BlanketBuildingAndContentsIsAgreedValue = False
                                RaiseEvent AgreedAmountChanged(False)
                            End If
                            Exit Select
                        Case "2"   ' Building
                            If sq.BlanketBuildingIsAgreedValue Then
                                sq.BlanketBuildingIsAgreedValue = False
                                RaiseEvent AgreedAmountChanged(False)
                            End If
                            Exit Select
                        Case "3"
                            If sq.BlanketContentsIsAgreedValue Then
                                sq.BlanketContentsIsAgreedValue = False
                                RaiseEvent AgreedAmountChanged(False)
                            End If
                            Exit Select
                    End Select
                End If

                ' Tracking the quote to see if it previously had a blanket for code below - CAH - MGB
                Dim HadBlanket = sq.HasBlanketBuildingAndContents = True OrElse sq.HasBlanketBuilding = True OrElse sq.HasBlanketContents = True
                ' BLANKET
                Select Case ddBlanketRating.SelectedValue
                    Case "0"  ' N/A
                        sq.HasBlanketBuildingAndContents = False
                        sq.HasBlanketBuilding = False
                        sq.HasBlanketContents = False
                        sq.BlanketContentsDeductibleID = ""
                        sq.BlanketBuildingDeductibleID = ""
                        sq.BlanketBuildingAndContentsDeductibleID = ""
                        Exit Select
                    Case "1"  ' Combined
                        sq.HasBlanketBuildingAndContents = True
                        sq.HasBlanketBuilding = False
                        sq.HasBlanketContents = False
                        sq.BlanketBuildingAndContentsCauseOfLossTypeId = ddBlanketCauseOfLoss.SelectedValue
                        If chkAgreedAmount.Checked Then
                            ' If agreed amount is checked we force the coinsurance to 100%
                            sq.BlanketBuildingAndContentsCoinsuranceTypeId = "7"
                        Else
                            ' If agreed amount is not checked, set the coinsurance to the dropdown value
                            sq.BlanketBuildingAndContentsCoinsuranceTypeId = ddBlanketCoinsurance.SelectedValue
                        End If
                        sq.BlanketBuildingAndContentsValuationId = ddBlanketValuation.SelectedValue
                        sq.BlanketBuildingAndContentsDeductibleID = ddDeductible.SelectedValue
                        Exit Select
                    Case "2"  ' Building
                        sq.HasBlanketBuilding = True
                        sq.HasBlanketBuildingAndContents = False
                        sq.HasBlanketContents = False
                        sq.BlanketBuildingCauseOfLossTypeId = ddBlanketCauseOfLoss.SelectedValue
                        If chkAgreedAmount.Checked Then
                            ' If agreed amount is checked we force the coinsurance to 100%
                            sq.BlanketBuildingCoinsuranceTypeId = "7"
                        Else
                            ' If agreed amount is not checked, set the coinsurance to the dropdown value
                            sq.BlanketBuildingCoinsuranceTypeId = ddBlanketCoinsurance.SelectedValue
                        End If
                        sq.BlanketBuildingValuationId = ddBlanketValuation.SelectedValue
                        sq.BlanketBuildingDeductibleID = ddDeductible.SelectedValue
                        Exit Select
                    Case "3"  ' Property
                        sq.HasBlanketContents = True
                        sq.HasBlanketBuilding = False
                        sq.HasBlanketBuildingAndContents = False
                        sq.BlanketContentsCauseOfLossTypeId = ddBlanketCauseOfLoss.SelectedValue
                        If chkAgreedAmount.Checked Then
                            ' If agreed amount is checked we force the coinsurance to 100%
                            sq.BlanketContentsCoinsuranceTypeId = "7"
                        Else
                            ' If agreed amount is not checked, set the coinsurance to the dropdown value
                            sq.BlanketContentsCoinsuranceTypeId = ddBlanketCoinsurance.SelectedValue
                        End If
                        sq.BlanketContentsValuationId = ddBlanketValuation.SelectedValue
                        sq.BlanketContentsDeductibleID = ddDeductible.SelectedValue
                        Exit Select
                End Select

                '' 1=combined 2=bldg 3=contents 
                'If ddBlanketRating.SelectedIndex >= 0 AndAlso ddBlanketRating.SelectedValue <> "0" Then
                '    If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                '        'If ddBlanketRating.SelectedValue = "1" OrElse ddBlanketRating.SelectedValue = "2" Then
                '        '    ' Combined or Building
                '        '    If Quote.Locations(0).DeductibleId <> ddDeductible.SelectedValue Then
                '        '        Quote.Locations(0).DeductibleId = ddDeductible.SelectedValue
                '        '        RaiseEvent BlanketDeductibleChanged()
                '        '    End If
                '        'Else
                '        '    ' Property only
                '        '    If Quote.Locations(0).DeductibleId <> ddDeductible.SelectedValue Then
                '        '        Quote.Locations(0).DeductibleId = ddDeductible.SelectedValue
                '        '        RaiseEvent BlanketDeductibleChanged()
                '        '    End If
                '        'End If
                '        BlanketHelper_CPR_CPP.SetBlanketDeductibleID(Quote, ddDeductible.SelectedValue)
                '    Else
                '        If ddBlanketRating.SelectedValue = "1" OrElse ddBlanketRating.SelectedValue = "2" Then
                '            ' Combined or Building
                '            If Quote.Locations(0).Buildings(0).DeductibleId <> ddDeductible.SelectedValue Then
                '                Quote.Locations(0).Buildings(0).DeductibleId = ddDeductible.SelectedValue
                '                RaiseEvent BlanketDeductibleChanged()
                '            End If
                '        Else
                '            ' Property only
                '            If Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId <> ddDeductible.SelectedValue Then
                '                Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId = ddDeductible.SelectedValue
                '                RaiseEvent BlanketDeductibleChanged()
                '            End If
                '        End If
                '    End If

                'ElseIf ddBlanketRating.SelectedIndex = 0 AndAlso HadBlanket Then
                '    'If Page is set to NO blanket but Quote HAD a Blanket we need to delete the blanket values
                '    'because the user is removing the blanket, else leave them alone.  CAH - MGB 20200308
                '    Quote.Locations(0).Buildings(0).DeductibleId = ""
                '    Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId = ""
                'End If

                '' SAVE THE BLANKET PROPERTIES TO THE LOCATION 0
                'If ddBlanketRating.SelectedIndex <> "0" Then
                '    Quote.Locations(0).DeductibleId = ddDeductible.SelectedValue
                '    Quote.Locations(0).CauseOfLossTypeId = ddBlanketCauseOfLoss.SelectedValue
                '    Quote.Locations(0).CoinsuranceTypeId = ddBlanketCoinsurance.SelectedValue
                '    Quote.Locations(0).ValuationMethodTypeId = ddBlanketValuation.SelectedValue
                'Else
                '    Quote.Locations(0).DeductibleId = ""
                '    Quote.Locations(0).CauseOfLossTypeId = ""
                '    Quote.Locations(0).CoinsuranceTypeId = ""
                '    Quote.Locations(0).ValuationMethodTypeId = ""
                'End If

                '' Apply any blanket changes to all buildings - uses the original method from old look & feel
                'BlanketHelper_CPR_CPP.PropagateBlanketChange(Quote, sq)

                ' Business Income ALS
                sq.HasBusinessIncomeALS = chkBusinessIncomeALS.Checked
                If sq.HasBusinessIncomeALS Then
                    sq.BusinessIncomeALSLimit = txtBusinessIncomeALSLimit.Text
                    ' Add the waiting period to all buildings on this location
                    If sq.Locations IsNot Nothing AndAlso sq.Locations.Count > 0 Then
                        For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In sq.Locations
                            For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                B.BusinessIncomeCov_WaitingPeriodTypeId = ddBusinessIncomeALSWaitingPeriod.SelectedValue
                            Next
                        Next
                    End If
                Else
                    sq.BusinessIncomeALSLimit = ""
                    Quote.Locations(0).Buildings(0).BusinessIncomeCov_WaitingPeriodTypeId = ""
                    ' Clear the waiting period on all buildings on this location
                    If sq.Locations IsNot Nothing AndAlso sq.Locations.Count > 0 Then
                        For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In sq.Locations
                            For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                                B.BusinessIncomeCov_WaitingPeriodTypeId = ""
                            Next
                        Next
                    End If
                End If
                SaveCGAndCPRPackagesEnhancement(Quote, sq)
            Next

            Dim HadSQFBlanket = SubQuoteFirst.HasBlanketBuildingAndContents = True OrElse SubQuoteFirst.HasBlanketBuilding = True OrElse SubQuoteFirst.HasBlanketContents = True
            ' 1=combined 2=bldg 3=contents 
            If ddBlanketRating.SelectedIndex >= 0 AndAlso ddBlanketRating.SelectedValue <> "0" Then
                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) Then
                    'If ddBlanketRating.SelectedValue = "1" OrElse ddBlanketRating.SelectedValue = "2" Then
                    '    ' Combined or Building
                    '    If Quote.Locations(0).DeductibleId <> ddDeductible.SelectedValue Then
                    '        Quote.Locations(0).DeductibleId = ddDeductible.SelectedValue
                    '        RaiseEvent BlanketDeductibleChanged()
                    '    End If
                    'Else
                    '    ' Property only
                    '    If Quote.Locations(0).DeductibleId <> ddDeductible.SelectedValue Then
                    '        Quote.Locations(0).DeductibleId = ddDeductible.SelectedValue
                    '        RaiseEvent BlanketDeductibleChanged()
                    '    End If
                    'End If
                    BlanketHelper_CPR_CPP.SetBlanketDeductibleID(Quote, ddDeductible.SelectedValue)
                Else
                    If ddBlanketRating.SelectedValue = "1" OrElse ddBlanketRating.SelectedValue = "2" Then
                        ' Combined or Building
                        If Quote.Locations(0).Buildings(0).DeductibleId <> ddDeductible.SelectedValue Then
                            Quote.Locations(0).Buildings(0).DeductibleId = ddDeductible.SelectedValue
                            RaiseEvent BlanketDeductibleChanged()
                        End If
                    Else
                        ' Property only
                        If Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId <> ddDeductible.SelectedValue Then
                            Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId = ddDeductible.SelectedValue
                            RaiseEvent BlanketDeductibleChanged()
                        End If
                    End If
                End If

            ElseIf ddBlanketRating.SelectedIndex = 0 AndAlso HadSQFBlanket Then
                'If Page is set to NO blanket but Quote HAD a Blanket we need to delete the blanket values
                'because the user is removing the blanket, else leave them alone.  CAH - MGB 20200308
                Quote.Locations(0).Buildings(0).DeductibleId = ""
                Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId = ""
            End If

            ' SAVE THE BLANKET PROPERTIES TO THE LOCATION 0
            If ddBlanketRating.SelectedIndex <> "0" Then
                'Quote.Locations(0).DeductibleId = ddDeductible.SelectedValue
                Quote.Locations(0).CauseOfLossTypeId = ddBlanketCauseOfLoss.SelectedValue
                Quote.Locations(0).CoinsuranceTypeId = ddBlanketCoinsurance.SelectedValue
                Quote.Locations(0).ValuationMethodTypeId = ddBlanketValuation.SelectedValue
            Else
                'Quote.Locations(0).DeductibleId = ""
                Quote.Locations(0).CauseOfLossTypeId = ""
                Quote.Locations(0).CoinsuranceTypeId = ""
                Quote.Locations(0).ValuationMethodTypeId = ""
            End If

            ' Apply any blanket changes to all buildings - uses the original method from old look & feel
            BlanketHelper_CPR_CPP.PropagateBlanketChange(Quote, SubQuoteFirst)


            'moved here since we don't need it on all state quotes, just govStateQuote
            'doing Transportation work from Transportation control relies on this control's Save to happen before CIM/Transportation Save
            If chkFoodManufacturersEnhancementPackage.Checked Then
                If alreadyHasFoodManufacturers = False Then
                    RaiseEvent NeedToReloadCIMTransportation() 'Transportation control will default the values for FMEE if needed
                End If
            Else
                ' When Food Manufacturer's is removed, remove the transportation coverage as well, regardless of whether it was on the quote before food manufacturers was added.  Per Barb 7/15/21
                If alreadyHasFoodManufacturers = True Then
                    'If removing Then transportation, only Do so If it could've previously been added because of FMEE
                    'may also need to see if we need to remove the CIM packagePart
                    RaiseEvent NeedToClearCIMTransportation() 'Transportation control can take care of clearing out everything specific to it
                    RaiseEvent NeedToReloadCIMTransportation() 'CIM control can take care of removing CIM pp if needed
                End If
            End If
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Me.ValidationHelper.GroupName = "General Information"

        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            If ddPackageModificationAssignmentType.SelectedIndex < 0 OrElse ddPackageModificationAssignmentType.SelectedValue = "-1" Then
                Me.ValidationHelper.AddError(ddPackageModificationAssignmentType, "Missing Package Modification Assignment Type", accordList)
            End If
        End If

        If chkBusinessIncomeALS.Checked Then
            If Not IsNumeric(txtBusinessIncomeALSLimit.Text) Then
                Me.ValidationHelper.AddError(txtBusinessIncomeALSLimit, "Missing Business Income ALS Limit", accordList)
            Else
                If CDec(txtBusinessIncomeALSLimit.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtBusinessIncomeALSLimit, "Invalid Business Income ALS Limit", accordList)
                End If
            End If
        End If

    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click, btnSaveAndGotoLocations.Click
        Me.Save_FireSaveEvent()
        Populate()
        If sender.Equals(btnSaveAndGotoLocations) Then
            If Me.ValidationSummmary.HasErrors = False Then
                If QuoteISCPP Then
                    Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.CPP_CPR_Locations, "")
                Else
                    Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.location, "")
                End If
            End If
        Else
            Populate()
        End If
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub

    'Added 11/22/2021 for CPP Endorsements task 64909 MLW
    Protected Sub btnMakeAChange_Click(sender As Object, e As EventArgs) Handles btnMakeAChange.Click
        Response.Redirect(btnMakeAChange.Attributes.Item("href"))
    End Sub

    Public Sub LoadBlanketDeductible()
        If Quote IsNot Nothing Then
            ' Deductible
            QQHelper.LoadStaticDataOptionsDropDown(ddDeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, , Quote.LobType)
            If CPRRemovePropDedBelow1k.IsPropertyDeductibleBelow1kAvailable(Quote) Then
                Dim Item500 = New ListItem("500", "8")
                ddDeductible.Items.Remove(Item500)
            End If
        End If
    End Sub

    Public Sub PopulateBlanketDeductible()
        If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
            If BlanketHelper_CPR_CPP.Has_CPRCPP_Blanket(SubQuoteFirst) Then
                Dim deductibleIdToUse As String = String.Empty
                Dim qqClassName As String = String.Empty
                Dim qqPropertTyName As String = String.Empty

                If RemoveBuildingLevelDeductibleHelper.IsRemoveBuildingLevelDeductibleAvailable(Quote) AndAlso (SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketBuildingAndContents OrElse SubQuoteFirst.HasBlanketContents) Then
                    deductibleIdToUse = BlanketHelper_CPR_CPP.GetBlanketDeductibleID(SubQuoteFirst)
                    qqClassName = QuickQuoteClassName.QuickQuoteLocation
                    qqPropertTyName = QuickQuotePropertyName.DeductibleId
                Else
                    If SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketBuildingAndContents Then
                        deductibleIdToUse = Quote.Locations(0).Buildings(0).DeductibleId
                        qqClassName = QuickQuoteClassName.QuickQuoteLocation
                        qqPropertTyName = QuickQuotePropertyName.DeductibleId
                    ElseIf SubQuoteFirst.HasBlanketContents Then
                        deductibleIdToUse = Quote.Locations(0).Buildings(0).PersPropCov_DeductibleId
                        qqClassName = QuickQuoteClassName.QuickQuoteBuilding
                        qqPropertTyName = QuickQuotePropertyName.PersPropCov_DeductibleId
                    End If
                End If
                WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ddDeductible, deductibleIdToUse, qqClassName, qqPropertTyName)
            End If
        End If
    End Sub

    Public Sub PopulateCPREnhancementEndorsementCheckboxes()
        If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.MedicalExpensesLimitId = "327" AndAlso (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability) Then
            Me.chkContractorsEnhancementPackage.Checked = False
            Me.chkContractorsEnhancementPackage.Enabled = False
            Me.chkManufacturersEnhancementPackage.Checked = False
            Me.chkManufacturersEnhancementPackage.Enabled = False
            Me.chkEnhancement.Checked = False
            Me.chkEnhancement.Enabled = False
            Me.chkPlusEnhancement.Checked = False
            Me.chkPlusEnhancement.Enabled = False
            Me.chkFoodManufacturersEnhancementPackage.Checked = False
            Me.chkFoodManufacturersEnhancementPackage.Enabled = False
        End If
    End Sub

    Public Sub SaveCGAndCPRPackagesEnhancement(Quote As QuickQuoteObject, sq As QuickQuoteObject)
        If Quote IsNot Nothing Then
            If Not IsQuoteReadOnly() AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.MedicalExpensesLimitId = "327" Then
                sq.Has_PackageGL_PlusEnhancementEndorsement = False
                sq.Has_PackageGL_EnhancementEndorsement = False
                sq.HasContractorsEnhancement = False
                sq.HasFoodManufacturersEnhancement = False
                sq.Has_PackageCPR_EnhancementEndorsement = False
                sq.Has_PackageCPR_PlusEnhancementEndorsement = False
            End If
        End If
    End Sub
End Class