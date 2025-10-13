Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.PPA
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports IFM.Common.InputValidation.InputHelpers
Imports IFM.PrimativeExtensions
Imports Diamond.Common.Objects.Claims.Dashboard

Public Class ctlVehicle_PPA
    Inherits VRControlBase

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Public Event VehicleControlRemoving(index As Int32)
    Public Event NewVehcileRequested()
    Public Event ReplaceVehicleTitleBar(index As Int32)

    Private ReadOnly Property MotorcycleCustomEquipmentTotalDictionaryName As String
        Get
            Return QuoteId & "_" & "MCCustomEquipTotal_" & VehicleIndex
        End Get
    End Property

    Public ReadOnly Property MyVehicle As QuickQuoteVehicle
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Vehicles.GetItemAtIndex(Me.VehicleIndex)
            End If
            Return Nothing
        End Get
    End Property

    Private Property HasReplacedVehicle As Boolean
        Get
            If Not IsNullEmptyorWhitespace(hdnHasReplacedVehicle.Value) Then
                hdnHasReplacedVehicle.Value = False
            End If
            Return CBool(hdnHasReplacedVehicle.Value)
        End Get
        Set(value As Boolean)
            hdnHasReplacedVehicle.Value = value
        End Set
    End Property

    Public Property VehicleIndex As Int32
        Get
            If ViewState("vs_vehicleNum") Is Nothing Then
                ViewState("vs_vehicleNum") = -1
            End If
            Return CInt(ViewState("vs_vehicleNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_vehicleNum") = value

            Dim symbolDD_Vin As String = "if((event.keyCode >= 48 && event.keyCode <= 90) || event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 32)"
            symbolDD_Vin += "{"
            symbolDD_Vin += "if($(""#" + Me.txtSymbol.ClientID + """).val() != '')"
            symbolDD_Vin += "{$(""#" + Me.txtYear.ClientID + """).val('');$(""#" + Me.txtMake.ClientID + """).val('');$(""#" + Me.txtModel.ClientID + """).val('');$(""#" + Me.txtSymbol.ClientID + """).val('');}"
            symbolDD_Vin += "}"

            Dim symbolDD_YMM As String = "if((event.keyCode >= 48 && event.keyCode <= 90) || event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 32)"
            symbolDD_YMM += "{"
            symbolDD_YMM += "if($(""#" + Me.txtSymbol.ClientID + """).val() != '')"
            symbolDD_YMM += "{$(""#" + Me.txtVinNumber.ClientID + """).val('');$(""#" + Me.txtSymbol.ClientID + """).val('');}"
            symbolDD_YMM += "}"

            Dim scriptHeaderUpdate As String = "updateVehicleHeaderText(""" + Me.lblAccordHeader.ClientID + """," + value.ToString() + ",""" + Me.txtMake.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.txtYear.ClientID + """); "
            Me.txtVinNumber.Attributes.Add("onkeyup", scriptHeaderUpdate + symbolDD_Vin)
            Me.txtMake.Attributes.Add("onkeyup", scriptHeaderUpdate + symbolDD_YMM)
            Me.txtModel.Attributes.Add("onkeyup", scriptHeaderUpdate + symbolDD_YMM)
            Me.txtYear.Attributes.Add("onkeyup", scriptHeaderUpdate + symbolDD_YMM)

            If Quote?.LobType = QuickQuoteLobType.AutoPersonal Then
                Me.ctlVehicleAdditionalInterestList.VehicleIndex = Me.VehicleIndex
                If IFM.VR.Common.Helpers.PPA.VehicleLookupPopupHelper.IsVehicleLookupPopupAvailable(Quote) Then
                    Me.ctlVinLookup.VehicleIndex = Me.VehicleIndex
                End If
            End If
        End Set
    End Property

    Public Property NamedNonOwnedAndExtended_FormCache As Dictionary(Of String, String)
        Get
            If ViewState("vs_NamedNonOwnedAndExtended_FormCache") IsNot Nothing Then
                Return DirectCast(ViewState("vs_NamedNonOwnedAndExtended_FormCache"), Dictionary(Of String, String))
            Else
                ViewState("vs_NamedNonOwnedAndExtended_FormCache") = New Dictionary(Of String, String)
                Return DirectCast(ViewState("vs_NamedNonOwnedAndExtended_FormCache"), Dictionary(Of String, String))
            End If
            Return Nothing
        End Get
        Set(value As Dictionary(Of String, String))
            ViewState("vs_NamedNonOwnedAndExtended_FormCache") = value
        End Set
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer ' New for accordion logic Matt A - 7/14/15
        Get
            Return Me.VehicleIndex
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
#If Not DEBUG Then
        Me.txtSymbol.Attributes.Add("style", "display:none;")
#End If
        If Quote?.LobType <> QuickQuoteLobType.AutoPersonal Then
            Me.ctlVehicleAdditionalInterestList.Visible = False
        End If

        If Not IsPostBack Then
            LoadStaticData()
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

        Me.VRScript.StopEventPropagation(Me.lnkBtnReplace.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnAddvehicle.ClientID)
        Me.VRScript.CreateConfirmDialog(Me.lnkRemove.ClientID, "Remove this Vehicle?")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        Me.VRScript.StopEventPropagation(Me.lnkBtnDriverAssSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSpecificSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSaveOtherInfo.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkBtnGaragedSave.ClientID)
        Me.VRScript.CreateAccordion(Me.divDriverAssignment.ClientID, Me.hiddenDriverAssignment, "0")
        Me.VRScript.CreateAccordion(Me.divAutoSpecificdata.ClientID, Me.hiddenAutoSpecificdata, "false")
        Me.VRScript.CreateAccordion(Me.divOtherVehicleinfo.ClientID, Me.hiddenOtherInfo, "0")
        Me.VRScript.CreateAccordion(Me.divGaraged.ClientID, Me.hiddenGaraged, "false")
        Me.VRScript.CreateAccordion(Me.divVinLookup.ClientID, Me.hiddenVinLookup, "0")

        Me.VRScript.AddScriptLine("$(""#" + Me.txtMake.ClientID + """).autocomplete({ source: 'GenHandlers/Vr_Pers/MakeModelLookup.ashx?GetMakes=Yes', minLength: 2, delay: 300 });")

        ' this auto complete is done a bit different as it uses a dynamic term from a differnt sender control
        Me.VRScript.AddScriptLine("SetUpModelAutoComplete('" + Me.txtModel.ClientID + "','" + Me.txtMake.ClientID + "','" + Me.txtYear.ClientID + "');")

        Me.VRScript.AddScriptLine("$(""#" + Me.txtGaragedCounty.ClientID + """).autocomplete({ source: indiana_Counties });")
        Me.VRScript.AddScriptLine("$(""#" + Me.txtGaragedCity.ClientID + """).autocomplete({ source: INCities });")

        ' symbols is always read-only
        'Me.VRScript.AddScriptLine("$(""#" + Me.txtSymbol.ClientID + """).attr('disabled', true);")

        'need to delete symbols everytime any of the following change
        ' symbol dirty detect
        Dim symbolDD_Vin As String = "if((event.keyCode >= 48 && event.keyCode <= 90) || event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 32)"
        symbolDD_Vin += "{"
        symbolDD_Vin += "if($(""#" + Me.txtSymbol.ClientID + """).val() != '')"
        symbolDD_Vin += "{$(""#" + Me.txtYear.ClientID + """).val('');$(""#" + Me.txtMake.ClientID + """).val('');$(""#" + Me.txtModel.ClientID + """).val('');$(""#" + Me.txtSymbol.ClientID + """).val('');}"
        symbolDD_Vin += "}"

        Dim symbolDD_YMM As String = "if((event.keyCode >= 48 && event.keyCode <= 90) || event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 32)"
        symbolDD_YMM += "{"
        symbolDD_YMM += "if($(""#" + Me.txtSymbol.ClientID + """).val() != '')"
        symbolDD_YMM += "{$(""#" + Me.txtVinNumber.ClientID + """).val('');$(""#" + Me.txtSymbol.ClientID + """).val('');}"
        symbolDD_YMM += "}"

        'Dim symbolDD As String = "if((event.keyCode >= 48 && event.keyCode <= 90) || event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 32)"
        'symbolDD += "{$(""#" + Me.txtSymbol.ClientID + """).val('');}"

        Dim scriptHeaderUpdate As String = "updateVehicleHeaderText(""" + Me.lblAccordHeader.ClientID + """," + Me.VehicleIndex.ToString() + ",""" + Me.txtMake.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.txtYear.ClientID + """); "

        Dim showVinLookupButton As String = "ToggleVinLookupButton(""" & Me.btnVinLookup.ClientID & """,""" & Me.txtVinNumber.ClientID & """,""" & Me.txtYear.ClientID & """,""" & Me.txtCostNew.ClientID & """,""" & Me.ddBodyType.ClientID & """);"
        Me.VRScript.AddScriptLine(showVinLookupButton) ' run at startup

        Me.VRScript.CreateJSBinding(Me.txtVinNumber, ctlPageStartupScript.JsEventType.onkeyup, symbolDD_Vin + showVinLookupButton)
        Me.VRScript.CreateJSBinding(Me.txtVinNumber, ctlPageStartupScript.JsEventType.onchange, showVinLookupButton)

        Me.VRScript.CreateJSBinding(Me.txtMake, ctlPageStartupScript.JsEventType.onkeyup, scriptHeaderUpdate + symbolDD_YMM)
        Me.VRScript.CreateJSBinding(Me.txtModel, ctlPageStartupScript.JsEventType.onkeyup, scriptHeaderUpdate + symbolDD_YMM)
        Me.VRScript.CreateJSBinding(Me.txtYear, ctlPageStartupScript.JsEventType.onkeyup, "$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));" + scriptHeaderUpdate + symbolDD_YMM + showVinLookupButton)

        'Added 10/13/2022 for task 75263 MLW
        ' VIN Lookup
        'Dim versionId As String = Quote.VersionId
        'Dim policyId As String = Quote.PolicyId
        'Dim policyImageNumber As String = Quote.PolicyImageNum
        Dim vehicleNum As String = "0"
        If MyVehicle IsNot Nothing Then
            vehicleNum = MyVehicle.VehicleNum
        End If
        Dim isNewBusiness As String = "True"
        If IsQuoteEndorsement() Then
            isNewBusiness = "False"
        End If
        Dim isNewRAPALookupAvailable As String = IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(QQHelper.IntegerForString(Quote.VersionId), QQHelper.DateForString(Quote.EffectiveDate), If(Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, False, True))
        'Updated 10/13/2022 for task 75263 MLW
        'Updated 07/22/2021 to send versionId MLW - versioning added with CAP Endorsements
        ' no Grid Vin Lookup - btnVinLookup.Attributes.Add("onclick", " SetupVinSearch(""" + Me.txtVinNumber.ClientID + """,""" + Me.txtMake.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.txtYear.ClientID + """,""" + Me.btnVinLookup.ClientID + """,""" + Me.ddBodyType.ClientID + """,""" + Me.ddAntiTheft.ClientID + """,""" + Me.ddAirBags.ClientID + """,""" + Me.divVinLookup.ClientID + """,""" + Me.divVinLookupContents.ClientID + """,""" + Me.ddPerformance.ClientID + """,""" + Me.txtSymbol.ClientID + """,""" + Me.txtCostNew.ClientID + """) ;return false;")
        'Me.VRScript.CreateJSBinding(btnVinLookup, ctlPageStartupScript.JsEventType.onclick, "SetupVinSearch(""" + Me.txtVinNumber.ClientID + """,""" + Me.txtMake.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.txtYear.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.ddBodyType.ClientID + """,""" + Me.ddAntiTheft.ClientID + """,""" + Me.ddAirBags.ClientID + """,""" + Me.divVinLookup.ClientID + """,""" + Me.divVinLookupContents.ClientID + """,""" + Me.ddPerformance.ClientID + """,""" + Me.txtSymbol.ClientID + """,""" + Me.txtCostNew.ClientID + """) ; $(""#" + HiddenLookupWasFired.ClientID + """).val('1'); return false;")
        'Me.VRScript.CreateJSBinding(btnVinLookup, ctlPageStartupScript.JsEventType.onclick, "SetupVinSearch(""" + Me.txtVinNumber.ClientID + """,""" + Me.txtMake.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.txtYear.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.ddBodyType.ClientID + """,""" + Me.ddAntiTheft.ClientID + """,""" + Me.ddAirBags.ClientID + """,""" + Me.divVinLookup.ClientID + """,""" + Me.divVinLookupContents.ClientID + """,""" + Me.ddPerformance.ClientID + """,""" + Me.txtSymbol.ClientID + """,""" + Me.txtCostNew.ClientID + """,""" + Quote.VersionId + """) ; $(""#" + HiddenLookupWasFired.ClientID + """).val('1'); return false;")
        If IFM.VR.Common.Helpers.PPA.VehicleLookupPopupHelper.IsVehicleLookupPopupAvailable(Quote) Then
            Dim scriptSetVinLookupFieldsFromParent As String = "SetVinLookupFieldsFromParent(""" & Me.HiddenLookupWasFired.ClientID & """," & VehicleIndex & ");"
            Me.VRScript.CreateJSBinding(Me.btnVinLookup, ctlPageStartupScript.JsEventType.onclick, "$('#vinLookupPopupContent').dialog('open');" + scriptSetVinLookupFieldsFromParent)
        Else
            Me.VRScript.CreateJSBinding(Me.btnVinLookup, ctlPageStartupScript.JsEventType.onclick, "SetupVinSearch(""" + Me.txtVinNumber.ClientID + """,""" + Me.txtMake.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.txtYear.ClientID + """,""" + Me.txtModel.ClientID + """,""" + Me.ddBodyType.ClientID + """,""" + Me.ddAntiTheft.ClientID + """,""" + Me.ddAirBags.ClientID + """,""" + Me.divVinLookup.ClientID + """,""" + Me.divVinLookupContents.ClientID + """,""" + Me.ddPerformance.ClientID + """,""" + Me.txtSymbol.ClientID + """,""" + Me.txtCostNew.ClientID + """,""" + Quote.VersionId + """,""" + Quote.PolicyId + """,""" + Quote.PolicyImageNum + """,""" + vehicleNum + """,""" + isNewBusiness + """,""" + If(Quote.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote, Quote.TransactionEffectiveDate, Quote.EffectiveDate) + """,""" + isNewRAPALookupAvailable + """) ; $(""#" + HiddenLookupWasFired.ClientID + """).val('1'); return false;")
        End If
        
        Dim isCustomEquipmentAvailable As String = IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote)
        ' combine above checks with the 'Other Vehicle Information' toggle
        Me.VRScript.CreateJSBinding(Me.ddBodyType, ctlPageStartupScript.JsEventType.onchange, "ToggleOtherVehicleInformation('" & Me.ddBodyType.ClientID & "','" & Me.divOtherVehicleinfo.ClientID & "','" & Me.ddUse.ClientID & "','" & Me.txtCashValue.ClientID & "','" & Me.txtStatedAmt.ClientID & "','" & Me.txtCostNew.ClientID & "','" & Me.ddMotorCyleType.ClientID & "','" & Me.txtHorsePower.ClientID & "','" & Me.divDriverAssignment.ClientID & "',false,'" & Me.txtSymbol.ClientID & "','" & Me.chkExtenedNonOwned.ClientID & "','" & Me.chkNamedNonOwner.ClientID & "','" & Me.HiddenLookupWasFired.ClientID & "','" & Me.txtVinNumber.ClientID & "','" & Me.txtYear.ClientID & "','" & Me.trCustomEquipment.ClientID & "','" & isNewRAPALookupAvailable & "','" & isCustomEquipmentAvailable & "');") 'trCustomEquipment
        Me.VRScript.AddScriptLine("ToggleOtherVehicleInformation('" & Me.ddBodyType.ClientID & "','" & Me.divOtherVehicleinfo.ClientID & "','" & Me.ddUse.ClientID & "','" & Me.txtCashValue.ClientID & "','" & Me.txtStatedAmt.ClientID & "','" & Me.txtCostNew.ClientID & "','" & Me.ddMotorCyleType.ClientID & "','" & Me.txtHorsePower.ClientID & "','" & Me.divDriverAssignment.ClientID & "',true,'" & Me.txtSymbol.ClientID & "','" & Me.chkExtenedNonOwned.ClientID & "','" & Me.chkNamedNonOwner.ClientID & "','" & Me.HiddenLookupWasFired.ClientID & "','" & Me.txtVinNumber.ClientID & "','" & Me.txtYear.ClientID & "','" & Me.trCustomEquipment.ClientID & "','" & isNewRAPALookupAvailable & "','" & isCustomEquipmentAvailable & "');")

        ' added to toggle chkNamedNonOwner And chkExtenedNonOwned 09/20/2022 BD
        VRScript.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleElementEnable(['" + Me.chkNamedNonOwner.ClientID + "']);});")
        VRScript.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleElementEnable(['" + Me.chkExtenedNonOwned.ClientID + "']);});")
        VRScript.CreateJSBinding(Me.chkNamedNonOwner, ctlPageStartupScript.JsEventType.onchange, "$(document).ready(function () {ifm.vr.ui.SingleElementDisable(['" + Me.chkExtenedNonOwned.ClientID + "']);});")
        VRScript.CreateJSBinding(Me.chkExtenedNonOwned, ctlPageStartupScript.JsEventType.onchange, "$(document).ready(function () {ifm.vr.ui.SingleElementDisable(['" + Me.chkNamedNonOwner.ClientID + "']);});")

        ' check for a combination of body type and 'Use'
        Dim bodyType_Van As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Van", Me.Quote.LobType)
        Dim bodyType_PickupwCamper As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Pickup w/Camper", Me.Quote.LobType)
        Dim bodyType_PickupwoCamper As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Pickup w/o Camper", Me.Quote.LobType)

        Dim use_Business As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.VehicleUseTypeId, "Business", Me.Quote.LobType)
        Dim use_Farm As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.VehicleUseTypeId, "Farm", Me.Quote.LobType)

        Dim UseBodyTYpeCheck As String = "if(($(""#" + Me.ddBodyType.ClientID + """).val() == '" + bodyType_Van + "' | $(""#" + Me.ddBodyType.ClientID + """).val() == '" + bodyType_PickupwCamper + "' | $(""#" + Me.ddBodyType.ClientID + """).val() == '" + bodyType_PickupwoCamper + "') & ($(this).val() == '" + use_Business + "' | $(this).val() =='" + use_Farm + "')){alert('This combination of body type and vehicle use will require Underwriting review prior to issuance.');}"
        Me.VRScript.CreateJSBinding(Me.ddUse, ctlPageStartupScript.JsEventType.onchange, UseBodyTYpeCheck)

        ' START - this crazy stuff is needed because of the custom tab orders required
        Me.VRScript.CreateJSBinding(Me.ddUse, ctlPageStartupScript.JsEventType.onkeydown, "TabLogicDDUse(event,'" + Me.txtCostNew.ClientID + "','" + Me.h3DriverAssignment.ClientID + "');")
        ' END - this crazy stuff is needed because of the custom tab orders required

        '7-21-14 copy garaging address
        Me.VRScript.AddVariableLine("garaged_copy.push(new GaragedCopyObject('" + Me.txtGaragedStreetNum.ClientID + "','" + Me.txtGaragedStreet.ClientID + "','" + Me.txtGaragedApt.ClientID + "','" + Me.txtGaragedCity.ClientID + "','" + Me.ddGaragedState.ClientID + "','" + Me.txtGaragedZip.ClientID + "','" + Me.txtGaragedCounty.ClientID + "'));")

        If IsQuoteEndorsement() Then
            'Me.VRScript.CreateJSBinding(Me.chkVehicleHasALienholderOrLease, ctlPageStartupScript.JsEventType.onchange, "if ($(this).prop('checked')) {$('.EndoAIClickTarget')[0].click(); $(this).prop('checked',true);}")
            'Me.VRScript.CreateJSBinding(Me.chkVehicleHasALienholderOrLease, ctlPageStartupScript.JsEventType.onchange, "if ($(this).prop('checked')) {$(this).closest('.ctlVehicle_PPA_container').find('.EndoAIClickTarget').first().click(); $(this).prop('checked',true);}")

            'Added 4/26/2022 for bug 51567 MLW
            If Not IsNewVehicleOnEndorsement(MyVehicle) Then
                Dim ddBodyTypeScript As String = "DisableBodyTypeOptions('" & Me.ddBodyType.ClientID & "');"
                Me.VRScript.AddScriptLine(ddBodyTypeScript) ' run at startup
            End If
        End If

    End Sub

    Public Sub LoadDriverListDropdowns()

        If Me.Quote IsNot Nothing Then
            '' remember to re-select any existing selections if possible
            'Dim pDriver As String = Me.ddPrincipalDriver.SelectedValue
            Me.ddPrincipalDriver.Items.Clear()

            'Dim oDriver1 As String = Me.ddOccDriver1.SelectedValue
            Me.ddOccDriver1.Items.Clear()

            'Dim oDriver2 As String = Me.ddOccDriver2.SelectedValue
            Me.ddOccDriver2.Items.Clear()

            'Dim oDriver3 As String = Me.ddOccDriver3.SelectedValue
            Me.ddOccDriver3.Items.Clear()

            Dim ratedDriverCount As Int32 = 0
            Me.ddPrincipalDriver.Items.Add(New ListItem("", ""))
            Me.ddOccDriver1.Items.Add(New ListItem("", ""))
            Me.ddOccDriver2.Items.Add(New ListItem("", ""))
            Me.ddOccDriver3.Items.Add(New ListItem("", ""))

            If Me.Quote.Drivers IsNot Nothing Then
                Dim index As Int32 = 1 ' NOT zero based !!!!!!!
                For Each driver As QuickQuote.CommonObjects.QuickQuoteDriver In Me.Quote.Drivers
                    If driver.DriverExcludeTypeId = "1" Then
                        ratedDriverCount += 1
                        Dim driverName As String = driver.Name.FirstName + " " + driver.Name.MiddleName + " " + driver.Name.LastName + " " + driver.Name.SuffixName
                        driverName = driverName.Replace("  ", " ").Trim()
                        Me.ddPrincipalDriver.Items.Add(New ListItem(String.Format("#{0} - {1}", index, driverName), index.ToString))
                        Me.ddOccDriver1.Items.Add(New ListItem(String.Format("#{0} - {1}", index, driverName), index.ToString))
                        Me.ddOccDriver2.Items.Add(New ListItem(String.Format("#{0} - {1}", index, driverName), index.ToString))
                        Me.ddOccDriver3.Items.Add(New ListItem(String.Format("#{0} - {1}", index, driverName), index.ToString))
                    End If
                    index += 1
                Next
            End If

            'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddPrincipalDriver, pDriver)
            'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddOccDriver1, oDriver1)
            'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddOccDriver2, oDriver2)
            'IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddOccDriver3, oDriver3)

            Me.trOdriver1.Visible = False
            Me.trOdriver2.Visible = False
            Me.trOdriver3.Visible = False

            Select Case ratedDriverCount
                Case 0
                    'do nothing - but don't want else to catch on this
                Case 1
                    'do nothing - but don't want else to catch on this
                Case 2
                    Me.trOdriver1.Visible = True
                Case 3
                    Me.trOdriver1.Visible = True
                    Me.trOdriver2.Visible = True
                Case Else
                    Me.trOdriver1.Visible = True
                    Me.trOdriver2.Visible = True
                    Me.trOdriver3.Visible = True
            End Select

        End If

    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddBodyType.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddPerformance, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.PerformanceTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddBodyType, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddUse, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.VehicleUseTypeId, SortBy.None, Me.Quote.LobType)

            QQHelper.LoadStaticDataOptionsDropDown(Me.ddAirBags, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.RestraintTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddAntiTheft, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.AntiTheftTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddMotorCyleType, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.VehicleTypeId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddGaragedState, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddAnnualMileage, QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.AnnualMileage, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()

        If Me.Quote IsNot Nothing Then
            LoadStaticData()

            If IFM.VR.Common.Helpers.PPA.VehicleLookupPopupHelper.IsVehicleLookupPopupAvailable(Quote) Then
                'For popup vin/ymm lookup to pass data between the popup control and this control
                txtVinNumber.AddCssClass("VehicleVinNum_" & VehicleIndex)
                txtYear.AddCssClass("VehicleYear_" & VehicleIndex)
                txtMake.AddCssClass("VehicleMake_" & VehicleIndex)
                txtModel.AddCssClass("VehicleModel_" & VehicleIndex)
                txtSymbol.AddCssClass("VehicleSymbol_" & VehicleIndex)
                ddBodyType.AddCssClass("VehicleBodyType_" & VehicleIndex)
                ddAirBags.AddCssClass("VehicleAirBags_" & VehicleIndex)
                ddAntiTheft.AddCssClass("VehicleAntiTheft_" & VehicleIndex)
                ddPerformance.AddCssClass("VehiclePerformance_" & VehicleIndex)
                txtCostNew.AddCssClass("VehicleCostNew_" & VehicleIndex)
            End If
            
            If IsQuoteEndorsement() Then
                chkVehicleHasALienholderOrLease.AutoPostBack = True
                lnkBtnReplace.Visible = True
            Else
                chkVehicleHasALienholderOrLease.AutoPostBack = False
                lnkBtnReplace.Visible = False
            End If

            If VehicleIndex = 0 Then
                ddBodyType.Items.Remove(ddBodyType.Items.FindByValue(ENUMHelper.VehicleBodyType.bodyType_OtherTrailer.ToString))
                ddBodyType.Items.Remove(ddBodyType.Items.FindByValue(ENUMHelper.VehicleBodyType.bodyType_RecTrailer.ToString))
                'ddBodyType.Items.Remove(ddBodyType.Items.FindByValue(ENUMHelper.VehicleBodyType.bodyType_Motorcycle))
            End If

            LoadDriverListDropdowns()

            If Me.Quote.Vehicles IsNot Nothing Then
                '7-21-14 Garage Address Copy
                Me.trCopyGaragingAddress.Visible = Me.Quote.Vehicles.Count > 1
                Me.btnCopyGaragingAddress.OnClientClick = "GetGaragingDropDown($(this)," + Me.VehicleIndex.ToString() + ");$(this).next().focus(); return false;"
            End If

            If MyVehicle IsNot Nothing Then
                Me.lblAccordHeader.Text = String.Format("Vehicle #{0} - {1} {2} {3}", VehicleIndex + 1, MyVehicle.Year, MyVehicle.Make, MyVehicle.Model).ToUpper()
                If IsQuoteEndorsement() Then
                    Me.lblAccordHeader.Text = EllipsisText(Me.lblAccordHeader.Text, 20)
                Else
                    Me.lblAccordHeader.Text = EllipsisText(Me.lblAccordHeader.Text, 44)
                End If
                ' Added logic for the new Lienholder/Lease checkbox Bug 32112 MGB 4-17-19
                ' Always check it of there are any AI's on the vehicle
                'chkVehicleHasALienholderOrLease.Checked = False
                'If VehicleHasAnyAI() Then chkVehicleHasALienholderOrLease.Checked = True
                ' 
                'Condensing Codefrom above CAH 6/27/2019 Endorsements
                chkVehicleHasALienholderOrLease.Checked = VehicleHasAnyAI()

                Me.chkNamedNonOwner.Checked = MyVehicle.NonOwnedNamed
                Me.chkExtenedNonOwned.Checked = MyVehicle.NonOwned

                If MyVehicle.Year.Trim = "0" Then
                    Me.txtYear.Text = ""
                Else
                    Me.txtYear.Text = MyVehicle.Year
                End If

                Me.txtMake.Text = MyVehicle.Make.ToUpper()
                Me.txtModel.Text = MyVehicle.Model.ToUpper()
                SetdropDownFromValue(Me.ddPerformance, MyVehicle.PerformanceTypeId)
                SetdropDownFromValue(Me.ddBodyType, MyVehicle.BodyTypeId)

                'Added 4/26/2022 for bug 51567 MLW - do not allow endorsements to edit vehicles of body type Rec. Trailer, Other Trailer, Motor Home, Motorcycle, Antique Auto or Classic Auto 
                If IsQuoteEndorsement() AndAlso Not IsNewVehicleOnEndorsement(MyVehicle) Then
                    Dim strBodyType As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, MyVehicle.BodyTypeId)
                    Select Case strBodyType.ToUpper()
                        Case "REC. TRAILER", "OTHER TRAILER", "ANTIQUE AUTO", "CLASSIC AUTO", "MOTOR HOME", "MOTORCYCLE"
                            'Allowing these fields to be editable for existing vehicles on endorsements clears out the vehicle symbols, do not want to clear symbols since Diamond does not update these symbols at rate for existing vehicles only new vehicles
                            txtVinNumber.Enabled = False
                            txtYear.Enabled = False
                            txtMake.Enabled = False
                            txtModel.Enabled = False
                            ddBodyType.Enabled = False
                            txtCostNew.Enabled = False
                            txtStatedAmt.Enabled = False
                        Case Else
                            'Show all, no restrictions
                            txtVinNumber.Enabled = True
                            txtYear.Enabled = True
                            txtMake.Enabled = True
                            txtModel.Enabled = True
                            ddBodyType.Enabled = True
                            txtCostNew.Enabled = True
                            txtStatedAmt.Enabled = True
                    End Select
                End If

                If MyVehicle.Vin.ToLower().Trim().StartsWith("none") Then
                    Me.txtVinNumber.Text = "NONE"
                Else
                    Me.txtVinNumber.Text = MyVehicle.Vin
                End If

                If MyVehicle.CostNew.Trim() = "0" Or MyVehicle.CostNew.Trim() = "$0" Or MyVehicle.CostNew.Trim() = "$0.00" Then
                    Me.txtCostNew.Text = ""
                Else
                    Me.txtCostNew.Text = TryToFormatAsCurrency(MyVehicle.CostNew, False)
                End If

                SetdropDownFromValue(Me.ddPrincipalDriver, MyVehicle.PrincipalDriverNum)
                SetdropDownFromValue(Me.ddOccDriver1, MyVehicle.OccasionalDriver1Num)
                SetdropDownFromValue(Me.ddOccDriver2, MyVehicle.OccasionalDriver2Num)
                SetdropDownFromValue(Me.ddOccDriver3, MyVehicle.OccasionalDriver3Num)

                If MyVehicle.VehicleUseTypeId.Trim() = "" Then
                    SetdropDownFromValue(Me.ddUse, "6")
                Else
                    SetdropDownFromValue(Me.ddUse, MyVehicle.VehicleUseTypeId)
                End If

                SetdropDownFromValue(Me.ddAirBags, MyVehicle.RestraintTypeId)
                SetdropDownFromValue(Me.ddAntiTheft, MyVehicle.AntiTheftTypeId)

                '4-15-2014
                If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 1 Then
                    Dim symbol1 As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "1" Select s.UserOverrideSymbol).FirstOrDefault()
                    Dim symbol2 As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "2" Select s.UserOverrideSymbol).FirstOrDefault()
                    Dim symbol3 As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "3" Select s.UserOverrideSymbol).FirstOrDefault()
                    If symbol3 IsNot Nothing Then
                        'Me.txtSymbol.Text = symbol1.Trim() + "/" + symbol2.Trim() + "/" + symbol3.Trim()
                        Dim strSymbols As String = symbol1.Trim() + "/" + symbol2.Trim() + "/" + symbol3.Trim()
                        If IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(Quote) Then
                            Dim symbol4 As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "8" Select s.UserOverrideSymbol).FirstOrDefault()
                            Dim symbol5 As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "9" Select s.UserOverrideSymbol).FirstOrDefault()
                            Dim symbol6 As String = (From s In MyVehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "14" Select s.UserOverrideSymbol).FirstOrDefault()
                            If symbol4 IsNot Nothing AndAlso symbol5 IsNot Nothing AndAlso symbol6 IsNot Nothing Then
                                strSymbols = strSymbols + "/" + symbol4.Trim() + "/" + symbol5.Trim() + "/" + symbol6.Trim()
                            End If
                        End If
                        Me.txtSymbol.Text = strSymbols
                    Else
                        Me.txtSymbol.Text = symbol1.Trim() + "/" + symbol2.Trim()
                    End If

                Else
                    Me.txtSymbol.Text = ""
                End If

                If MyVehicle.CubicCentimeters.Trim() = "0" Then
                    Me.txtHorsePower.Text = ""
                Else
                    Me.txtHorsePower.Text = MyVehicle.CubicCentimeters
                End If

                SetdropDownFromValue(Me.ddMotorCyleType, MyVehicle.VehicleTypeId)
                'Me.txtHorsePower.Text = vehicle.ho
                If MyVehicle.ActualCashValue.Trim() = "0" Or MyVehicle.ActualCashValue.Trim = "$0" Or MyVehicle.ActualCashValue.Trim = "$0.00" Then
                    Me.txtCashValue.Text = ""
                Else
                    Me.txtCashValue.Text = TryToFormatAsCurrency(MyVehicle.ActualCashValue, False)
                End If

                If MyVehicle.StatedAmount.Trim() = "0" Or MyVehicle.StatedAmount.Trim() = "$0" Or MyVehicle.StatedAmount.Trim = "$0.00" Then
                    Me.txtStatedAmt.Text = ""
                Else
                    Me.txtStatedAmt.Text = TryToFormatAsCurrency(MyVehicle.StatedAmount, False)
                End If

                If IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote) Then
                    If IsNullEmptyorWhitespace(MyVehicle.CustomEquipmentAmount.Trim()) OrElse MyVehicle.CustomEquipmentAmount.Trim() = "0" OrElse MyVehicle.CustomEquipmentAmount.Trim() = "$0" OrElse MyVehicle.CustomEquipmentAmount.Trim = "$0.00" Then
                        Me.txtCustomEquipment.Text = ""
                    Else
                        Me.txtCustomEquipment.Text = TryToFormatAsCurrency(MyVehicle.CustomEquipmentAmount, False)
                    End If
                Else
                    trCustomEquipment.Attributes.Add("style", "display:none;")
                End If

                If Me.ddBodyType.SelectedValue = "20" Then
                    trCustomEquipment.Attributes.Add("style", "display:none;")
                    Me.txtCustomEquipment.Text = ""
                End If

                Me.txtGaragedStreetNum.Text = MyVehicle.GaragingAddress.Address.HouseNum
                Me.txtGaragedStreet.Text = MyVehicle.GaragingAddress.Address.StreetName
                Me.txtGaragedApt.Text = MyVehicle.GaragingAddress.Address.ApartmentNumber
                Me.txtGaragedOtherInfo.Text = MyVehicle.GaragingAddress.Address.Other
                Me.txtGaragedCity.Text = MyVehicle.GaragingAddress.Address.City
                SetdropDownFromValue(Me.ddGaragedState, MyVehicle.GaragingAddress.Address.StateId)

                If MyVehicle.GaragingAddress.Address.Zip <> "00000" And MyVehicle.GaragingAddress.Address.Zip <> "00000-0000" Then
                    Me.txtGaragedZip.Text = MyVehicle.GaragingAddress.Address.Zip
                Else
                    Me.txtGaragedZip.Text = ""
                End If

                Me.txtGaragedCounty.Text = MyVehicle.GaragingAddress.Address.County

                If MyVehicle.NonOwnedNamed Then
                    SetNamedNonOwner(MyVehicle.NonOwnedNamed) '3-24-14
                End If

                If MyVehicle.NonOwned Then
                    SetExtendedNonOwned(MyVehicle.NonOwned) '3-24-14
                End If

                SetdropDownFromValue(Me.ddAnnualMileage, MyVehicle.AnnualMiles)

            End If

            Me.divAnnualMileage.Visible = VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)
            Me.divDriverAssignment.Visible = Not VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote)

            Me.PopulateChildControls()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Vehicle #{0}", Me.VehicleIndex + 1)

        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Dim accordListPlusOtherInfo As New List(Of VRAccordionTogglePair)
        accordListPlusOtherInfo.AddRange(accordList)
        accordListPlusOtherInfo.Add(New VRAccordionTogglePair(Me.divOtherVehicleinfo.ClientID, "0"))

        Dim accordListPlusDriverAssignment As New List(Of VRAccordionTogglePair)
        accordListPlusDriverAssignment.AddRange(accordList)
        accordListPlusDriverAssignment.Add(New VRAccordionTogglePair(Me.divDriverAssignment.ClientID, "0"))

        Dim valItems = VehicleValidator.VehicleValidation(Me.VehicleIndex, Me.Quote, valArgs.ValidationType)
        If valItems.Any() Then

            For Each v In valItems
                Select Case v.FieldId
                    Case VehicleValidator.VehicleCostNew
                        If Not (IsQuoteEndorsement() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote) = False AndAlso Quote.LobType = QuickQuoteLobType.AutoPersonal) Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCostNew, v, accordList)
                        End If
                    Case VehicleValidator.VehicleVIN
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtVinNumber, v, accordList)
                    Case VehicleValidator.VehicleMake
                        If Not (IsQuoteEndorsement() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote) = False AndAlso Quote.LobType = QuickQuoteLobType.AutoPersonal) Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMake, v, accordList)
                            If v.Message = "Invalid Make and/or Model" Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtModel, v, accordList)
                            End If
                        End If
                    Case VehicleValidator.VehicleModel
                        If Not (IsQuoteEndorsement() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote) = False AndAlso Quote.LobType = QuickQuoteLobType.AutoPersonal) Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtModel, v, accordList)
                        End If
                    Case VehicleValidator.VehicleYear
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtYear, v, accordList)
                    Case VehicleValidator.VehicleBodyType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBodyType, v, accordList)
                    Case VehicleValidator.VehicleMotorCycleType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddMotorCyleType, v, accordListPlusOtherInfo)
                    Case VehicleValidator.VehicleMotorCycleHorsePower
                        'Updated 2/8/2022 for bug 64563 MLW
                        If QQHelper.BitToBoolean(ConfigurationManager.AppSettings("Task64563_PPAHorsepowerCCValidation_NBandEndoNewVehicle")) = True Then
                            'If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso IsNewVehicleOnEndorsement(MyVehicle)) Then
                            If Not (IsQuoteEndorsement() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote) = False AndAlso Quote.LobType = QuickQuoteLobType.AutoPersonal) Then
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtHorsePower, v, accordListPlusOtherInfo)
                            End If
                        Else
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtHorsePower, v, accordListPlusOtherInfo)
                        End If
                        'Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtHorsePower, v, accordListPlusOtherInfo)
                    Case VehicleValidator.VehicleStatedAmount
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStatedAmt, v, accordListPlusOtherInfo)
                    Case VehicleValidator.VehiclePrimaryAssignedDriver
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddPrincipalDriver, v, accordListPlusDriverAssignment)
                    Case VehicleValidator.VehicleDriverAssignedMoreThanOnce
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddOccDriver1, v, accordListPlusDriverAssignment)
                    Case VehicleValidator.VehicleBothNonOwnerTypesSelected
                        ValidationHelper.Val_BindValidationItemToControl(Me.chkNamedNonOwner, v, accordList)
                        Me.chkNamedNonOwner.Checked = False
                        Me.chkExtenedNonOwned.Checked = False
                        Me.chkNamedNonOwner.Enabled = True
                        Me.chkExtenedNonOwned.Enabled = True
                        MyVehicle.NonOwned = False
                        MyVehicle.NonOwnedNamed = False
                        SetExtendedNonOwned(Me.chkExtenedNonOwned.Checked, False)
                        SetNamedNonOwner(Me.chkNamedNonOwner.Checked, False)
                        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
                    Case VehicleValidator.VehicleUse
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddUse, v, accordList)
                    Case VehicleValidator.AnnualMileage
                        If Not (IsQuoteEndorsement() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote) = False AndAlso Quote.LobType = QuickQuoteLobType.AutoPersonal) Then
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddAnnualMileage, v, accordList)
                        End If
                    Case VehicleValidator.VehicleSymbols
                        If Not IsQuoteEndorsement() Then
                            Me.ValidationHelper.AddError(v.Message)
                        End If
                End Select
            Next
        End If

        Dim valGaragingItems = VehicleGaragingValidator.ValidateVehicleAddress(Me.VehicleIndex, Me.Quote, valArgs.ValidationType)
        If valGaragingItems.Any() Then
             '.ValidationHelper.GroupName = "Garaging address is required on all vehicles when a PO Box is used for the mailing address."

             For Each v In valGaragingItems
                Select Case v.FieldId
                    'Case VehicleGaragingValidator.MainValidationText
                    '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedStreet, v, accordList)
                    Case VehicleGaragingValidator.HouseNumberMissing
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedStreetNum, v, accordList)
                    Case VehicleGaragingValidator.StreetNameMissing
                         Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedStreet, v, accordList)
                    Case VehicleGaragingValidator.ZipCodeIsMissing
                         Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedZip, v, accordList)
                    Case VehicleGaragingValidator.CityIsMissing
                         Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedCity, v, accordList)
                    Case VehicleGaragingValidator.StateIsMissing
                         Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddGaragedState, v, accordList)
                    Case VehicleGaragingValidator.CountyIsMissing
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedCounty, v, accordList)
                End Select
             Next

        End If

        Me.ValidateChildControls(valArgs)

    End Sub

    Public Overrides Function Save() As Boolean
        If MyVehicle IsNot Nothing Then

            Dim bodyType_MotorCycle As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Motorcycle", Me.Quote.LobType)
            Dim bodyType_MotorHome As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Motor Home", Me.Quote.LobType)
            Dim bodyType_PICKUPWCAMPER As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "PICKUP W/CAMPER", Me.Quote.LobType)
            Dim bodyType_RecTrailer As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Rec. Trailer", Me.Quote.LobType)
            Dim bodyType_ClassicAuto As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Classic Auto", Me.Quote.LobType)
            Dim bodyType_OtherTrailer As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Other Trailer", Me.Quote.LobType)
            Dim bodyType_AntiqueAuto As String = QQHelper.GetStaticDataValueForText(QuickQuoteClassName.QuickQuoteVehicle, QuickQuotePropertyName.BodyTypeId, "Antique Auto", Me.Quote.LobType)

            ' Add a fake AI if the Vehicle has lienholder or lease checkbox is checked
            ' Per bug 32112 MGB 4-17-19
            If Not IsOnAppPage AndAlso Not IsQuoteEndorsement() AndAlso Not IsQuoteReadOnly() Then
                If chkVehicleHasALienholderOrLease.Checked Then
                    AddFakeAIIfNeeded()
                Else
                    ' If unchecked remove any fake ai from the vehicle
                    RemoveFakeAI()
                End If
            End If

            ' New business, set lienholder checkbox if any AI's on vehicle
            If Not IsOnAppPage AndAlso Not IsQuoteEndorsement() AndAlso Not IsQuoteReadOnly() Then
                ' If the checkbox is unchecked but there's any AI on the vehicle then re-check the box
                If Not chkVehicleHasALienholderOrLease.Checked Then
                    If VehicleHasAnyAI() Then chkVehicleHasALienholderOrLease.Checked = True
                End If
            End If

            ' On a endorsements new vehicle, check the lienholder checkbox if there are any
            ' lienholders on the vehicle.  Note that we do NOT set the lienholder checkbox
            ' if this is an existing vehicle on an endorsement.
            If (Not IsOnAppPage) AndAlso IsQuoteEndorsement() AndAlso (Not IsQuoteReadOnly()) AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote) Then
                ' If the checkbox is unchecked but there's any AI on the vehicle then re-check the box
                If Not chkVehicleHasALienholderOrLease.Checked Then
                    If VehicleHasAnyAI() Then chkVehicleHasALienholderOrLease.Checked = True
                End If
            End If

            MyVehicle.NonOwnedNamed = Me.chkNamedNonOwner.Checked
            MyVehicle.NonOwned = Me.chkExtenedNonOwned.Checked

            If (Me.chkExtenedNonOwned.Checked = False And Me.chkNamedNonOwner.Checked = False) And String.IsNullOrWhiteSpace(Me.txtSymbol.Text) And (String.IsNullOrWhiteSpace(Me.txtCostNew.Text) Or IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtCostNew.Text) <= 0.0) Then
                ' try a vin search if no result throw error - will do this again at save
                If String.IsNullOrWhiteSpace(Me.txtSymbol.Text) And String.IsNullOrWhiteSpace(Me.txtVinNumber.Text) = False Then
                    'Updated 10/18/2022 for task 75263 MLW
                    'Dim VinResults = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo(Me.txtVinNumber.Text, "", "", 0, If(IsDate(Me.Quote.EffectiveDate), CDate(Me.Quote.EffectiveDate), DateTime.MinValue), Me.Quote.VersionId.TryToGetInt32())
                    Dim lookupType As Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA
                    If IsNewModelISORAPALookupTypeAvailable(QQHelper.IntegerForString(Quote.VersionId), QQHelper.DateForString(Quote.EffectiveDate), If(Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, False, True)) Then
                        lookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi
                    End If
                    Dim effDate As String = If(Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, Quote.TransactionEffectiveDate, Quote.EffectiveDate)
                    Dim VinResults = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(Me.txtVinNumber.Text, "", "", 0, If(IsDate(effDate), CDate(effDate), DateTime.MinValue), Me.Quote.VersionId.TryToGetInt32(), lookupType, Me.Quote.PolicyId, Me.Quote.PolicyImageNum, "0")
                    If VinResults.Any() And VinResults.Count = 1 Then
                        If IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.Quote) Then
                            If IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(Quote) Then
                                Me.txtSymbol.Text = VinResults(0).CompSymbol + "/" + VinResults(0).CollisionSymbol + "/" + VinResults(0).LiabilitySymbol + "/" + VinResults(0).BodilyInjurySymbol + "/" + VinResults(0).PropertyDamageSymbol + "/" + VinResults(0).MedPaySymbol
                            Else
                                Me.txtSymbol.Text = VinResults(0).CompSymbol + "/" + VinResults(0).CollisionSymbol + "/" + VinResults(0).LiabilitySymbol ' swapped 6-9-14
                            End If
                            'Me.txtSymbol.Text = VinResults(0).CompSymbol + "/" + VinResults(0).CollisionSymbol + "/" + VinResults(0).LiabilitySymbol ' swapped 6-9-14
                        Else
                            Me.txtSymbol.Text = VinResults(0).CompSymbol + "/" + VinResults(0).CollisionSymbol ' swapped 6-9-14
                        End If

                        Me.txtMake.Text = VinResults(0).Make.ToUpper()
                        Me.txtYear.Text = VinResults(0).Year.ToString
                        'Me.txtModel.Text = VinResults(0).Model.ToUpper()
                        Me.txtModel.Text = VinResults(0).Description.ToUpper() 'updated to match functionality everywhere else in VR.
                    End If
                End If
            End If
            MyVehicle.Year = Me.txtYear.Text.Trim()
            MyVehicle.Make = Me.txtMake.Text.ToUpper().Trim()
            MyVehicle.Model = Me.txtModel.Text.ToUpper().Trim()
            MyVehicle.BodyTypeId = Me.ddBodyType.SelectedValue
            If Me.txtVinNumber.Text.ToLower().Trim() = "none" AndAlso Me.chkExtenedNonOwned.Checked = False AndAlso Me.chkNamedNonOwner.Checked = False Then
                MyVehicle.Vin = "none" + Me.VehicleIndex.ToString()
            Else
                MyVehicle.Vin = Me.txtVinNumber.Text.Trim()
            End If

            MyVehicle.CostNew = TryToFormatAsCurrency(Me.txtCostNew.Text.Trim(), False)

            MyVehicle.PerformanceTypeId = Me.ddPerformance.SelectedValue '4-2-14 required to rate


            If MyVehicle.VehicleSymbols Is Nothing Then
                MyVehicle.VehicleSymbols = New List(Of QuickQuoteVehicleSymbol)
            Else
                MyVehicle.VehicleSymbols.Clear()
            End If

            If Me.txtSymbol.Text.Contains("/") Then
                Dim splitSymbolsVals As String() = Me.txtSymbol.Text.Trim().Split("/"c)
                Dim symbolTypeId As Integer = 0
                If splitSymbolsVals.Count > 1 Then
                    For i = 0 To splitSymbolsVals.Length - 1
                        If IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(Quote) Then
                            symbolTypeId = i + 1
                            If splitSymbolsVals.Length >= 4 Then
                                'NOTE: if order of symbols changes, this will need to be updated. It is assumed that the order will be 1=other(comp),2=coll,3=liab,8=bodilyInjury,9=propertyDamage,14=medPay always
                                Select Case i
                                    Case "3"
                                        symbolTypeId = "8"
                                    Case "4"
                                        symbolTypeId = "9"
                                    Case "5"
                                        symbolTypeId = "14"
                                End Select
                            End If
                            IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, symbolTypeId.ToString(), splitSymbolsVals(i))
                        Else
                            IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, (i + 1).ToString(), splitSymbolsVals(i))
                        End If
                    Next
                End If
            Else
                If Me.chkExtenedNonOwned.Checked Or Me.chkNamedNonOwner.Checked Then
                    'IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "0", "1")
                    'IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "0", "2")
                    'IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "0", "3")
                    IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "1", "0")
                    IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "2", "0")
                    IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "3", "0")
                    If IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(Quote) Then
                        IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "8", "0")
                        IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "9", "0")
                        IFM.VR.Common.Helpers.PPA.PrefillHelper.AddUpdateAutoSymbols(Me.Quote, Me.MyVehicle, "14", "0")
                    End If
                Else
                    'already cleared above
                End If
            End If


            If MyVehicle.BodyTypeId = bodyType_RecTrailer.ToString() Or MyVehicle.BodyTypeId = bodyType_OtherTrailer.ToString() Then
                MyVehicle.PrincipalDriverNum = ""
                MyVehicle.OccasionalDriver1Num = ""
                MyVehicle.OccasionalDriver2Num = ""
                MyVehicle.OccasionalDriver3Num = ""
            Else
                MyVehicle.PrincipalDriverNum = Me.ddPrincipalDriver.SelectedValue
                MyVehicle.OccasionalDriver1Num = Me.ddOccDriver1.SelectedValue
                MyVehicle.OccasionalDriver2Num = Me.ddOccDriver2.SelectedValue
                MyVehicle.OccasionalDriver3Num = Me.ddOccDriver3.SelectedValue

                If MyVehicle.PrincipalDriverNum <> "" AndAlso MyVehicle.PrincipalDriverNum IsNot Nothing Then
                    If MyVehicle.NonOwned Then
                        Dim enoDriver As QuickQuoteDriver = Quote.Drivers(Integer.Parse(MyVehicle.PrincipalDriverNum) - 1) '.Find(Function(p) p.DriverNum = MyVehicle.PrincipalDriverNum)

                        If enoDriver IsNot Nothing Then
                            enoDriver.ExtendedNonOwned = True
                        End If
                    Else
                        Dim enoDriver As QuickQuoteDriver = Quote.Drivers(Integer.Parse(MyVehicle.PrincipalDriverNum) - 1)
                        enoDriver.ExtendedNonOwned = False
                    End If
                End If
            End If

            If MyVehicle.BodyTypeId <> bodyType_MotorCycle Then '5-15-2014 if this was in app and they came back and changed to car scheduled items no longer apply
                If MyVehicle.ScheduledItems IsNot Nothing Then
                    MyVehicle.ScheduledItems.Clear()
                End If
            End If

            MyVehicle.VehicleUseTypeId = Me.ddUse.SelectedValue
            MyVehicle.RestraintTypeId = Me.ddAirBags.SelectedValue
            MyVehicle.AntiTheftTypeId = Me.ddAntiTheft.SelectedValue
            ' Brake Type ???? 7-31-14

            'Me.txtOtherThanCollisionSystemGen.Text = vehicle.symobols
            'Me.txtOtherThanCollisionSystemOverride.Text = vehicle.symbols

            'Me.txtCollisionSystemGen.Text = vehicle.symbols
            'Me.txtCollisionOverride.Text = vehicle.symbols

            If Me.ddBodyType.SelectedValue = bodyType_MotorCycle Then
                ' these are only available on motorcycle body type
                MyVehicle.VehicleTypeId = Me.ddMotorCyleType.SelectedValue
                MyVehicle.CubicCentimeters = Me.txtHorsePower.Text.Trim().Replace(",", "")
                MyVehicle.ActualCashValue = "" ' Me.txtCashValue.Text.Trim()
                MyVehicle.StatedAmount = "" 'Me.txtStatedAmt.Text.Trim()
            Else
                'motorcycle only fields must be blank
                MyVehicle.VehicleTypeId = ""
                Me.txtHorsePower.Text = ""
                QQDevDictionary_SetItem(MotorcycleCustomEquipmentTotalDictionaryName, "0")

                ' Switched it to use Stated Amount instead of Actual Cash Value BUG 3681 MGB 9/16/14
                If Me.ddBodyType.SelectedValue = bodyType_ClassicAuto Then
                    'vehicle.StatedAmount = ""
                    'vehicle.ActualCashValue = Helpers.WebHelper_Personal.TryToFormatAsCurrency(Me.txtCashValue.Text, False)
                    MyVehicle.StatedAmount = TryToFormatAsCurrency(Me.txtStatedAmt.Text, False)
                    MyVehicle.ActualCashValue = ""
                End If

                If Me.ddBodyType.SelectedValue = bodyType_OtherTrailer Or Me.ddBodyType.SelectedValue = bodyType_AntiqueAuto Then
                    MyVehicle.ActualCashValue = ""
                    MyVehicle.StatedAmount = TryToFormatAsCurrency(Me.txtStatedAmt.Text, False)
                End If

            End If
            If IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote) Then
                MyVehicle.CustomEquipmentAmount = TryToFormatAsCurrency(Me.txtCustomEquipment.Text, False)
            End If

            If Me.ddBodyType.SelectedValue = bodyType_OtherTrailer Then
                MyVehicle.CustomEquipmentAmount = ""
            End If

            MyVehicle.GaragingAddress.Address.HouseNum = Me.txtGaragedStreetNum.Text.Trim()
            MyVehicle.GaragingAddress.Address.StreetName = Me.txtGaragedStreet.Text.Trim()
            MyVehicle.GaragingAddress.Address.ApartmentNumber = Me.txtGaragedApt.Text.Trim()
            MyVehicle.GaragingAddress.Address.Other = Me.txtGaragedOtherInfo.Text.Trim()
            MyVehicle.GaragingAddress.Address.City = Me.txtGaragedCity.Text.Trim()
            MyVehicle.GaragingAddress.Address.StateId = Me.ddGaragedState.SelectedValue
            MyVehicle.GaragingAddress.Address.Zip = Me.txtGaragedZip.Text.Trim()
            MyVehicle.GaragingAddress.Address.County = Me.txtGaragedCounty.Text.Trim()

            MyVehicle.DriverOutOfStateSurcharge = GoverningStateQuote.StateId.TryToGetInt32 <> MyVehicle.GaragingAddress.Address.StateId.TryToGetInt32 And MyVehicle.GaragingAddress.Address.StateId.TryToGetInt32 <> 0

            MyVehicle.AnnualMiles = Me.ddAnnualMileage.SelectedValue

            ' 20210913 MGB Bug 64824
            ' Only set HasAutoLoanOrLease if:
            '   - This is a NEW BUSINESS quote.
            '   - This is an endorsement -AND- the vehicle is new to the image
            If (Not IsQuoteEndorsement()) OrElse (IsQuoteEndorsement() AndAlso QQHelper.IsQuickQuoteVehicleNewToImage(MyVehicle, Quote)) Then
                Dim todayDate As Long = DateTime.Now.Year
                If DateTime.Now.Month >= 10 Then
                    todayDate += 1
                End If
                Dim vYear As Int32 = 0
                Dim VehicleYear As String = ""
                If IsNumeric(MyVehicle.Year) Then
                    VehicleYear = MyVehicle.Year
                    If (todayDate - Integer.Parse(VehicleYear)) <= 5 AndAlso VehicleHasAnyAI() Then
                        MyVehicle.HasAutoLoanOrLease = chkVehicleHasALienholderOrLease.Checked
                    Else
                        MyVehicle.HasAutoLoanOrLease = False
                    End If
                Else
                    MyVehicle.HasAutoLoanOrLease = False
                End If
            End If
            Me.SaveChildControls()

            ' Set Checkbox after AI's have been updated
            If IsQuoteEndorsement() Then
                chkVehicleHasALienholderOrLease.Checked = VehicleHasAnyAI()
                'MyVehicle.HasAutoLoanOrLease = chkVehicleHasALienholderOrLease.Checked
            End If

            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Checks to see if a fake AI is on this vehicle.
    ''' Returns the AI if so, nothing if not
    ''' </summary>
    ''' <returns></returns>
    Private Function GetFakeAI() As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest
        Dim FakeAIFound As Boolean = False
        Dim FakeAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = Nothing

        If MyVehicle.AdditionalInterests IsNot Nothing Then
            For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In MyVehicle.AdditionalInterests
                If ai.Name IsNot Nothing Then
                    If ai.Name.FirstName.ToUpper = "FAKE" AndAlso ai.Name.LastName.ToUpper = "AI" Then
                        FakeAI = ai
                        Exit For
                    End If
                End If
            Next
        End If

        Return FakeAI
    End Function

    ''' <summary>
    ''' Checks to see if a fake AI exists, if not it creates one and adds it to the vehicle.  If fake ai already exists does nothing.
    ''' </summary>
    Public Sub AddFakeAIIfNeeded()
        ' Note that if there's already an AI on the vehicle do not create a fake AI.
        If MyVehicle.AdditionalInterests Is Nothing OrElse MyVehicle.AdditionalInterests.Count = 0 Then
            MyVehicle.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            ' No AI's - create a fake one
            ' Create a fake AI with type of 'First Lienholder'
            Dim FakeAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInterest()
            FakeAI.TypeId = QuickQuoteAdditionalInterest.AdditionalInterestType.FirstLienholder
            FakeAI.Name = New QuickQuoteName()
            FakeAI.Name.BirthDate = "01/01/2000"
            FakeAI.Name.FirstName = "Fake"
            FakeAI.Name.LastName = "AI"
            FakeAI.Address = New QuickQuoteAddress()
            FakeAI.Address.HouseNum = "999"
            FakeAI.Address.StreetName = "Fake Street"
            FakeAI.Address.City = "FakeTown"
            FakeAI.Address.StateId = "16"
            FakeAI.Address.Zip = "99999"
            ' Add the fake AI to the vehicle
            If MyVehicle.AdditionalInterests Is Nothing Then MyVehicle.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            MyVehicle.AdditionalInterests.Add(FakeAI)
        End If
    End Sub

    ''' <summary>
    ''' Removes the fake ai from the vehicle if there is one
    ''' </summary>
    Public Sub RemoveFakeAI()
        Dim ExistingFakeAI As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest = GetFakeAI()

        ' If a fake AI exists, remove it
        If ExistingFakeAI IsNot Nothing Then
            Dim ndx As Integer = -1
            If MyVehicle.AdditionalInterests IsNot Nothing Then
                For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In MyVehicle.AdditionalInterests
                    ndx += 1
                    If ai.Equals(ExistingFakeAI) Then
                        MyVehicle.AdditionalInterests.RemoveAt(ndx)
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    'Private Sub ctlVehicle_PPA_AddFakeAIIfNeeded() Handles ctlVehicleAdditionalInterestList.AddFakeAIIfNeeded
    '    AddFakeAIIfNeeded()
    'End Sub
    'Private Sub ctlVehicle_PPA_RemoveFakeAI() Handles ctlVehicleAdditionalInterestList.RemoveFakeAI
    '    RemoveFakeAI()
    'End Sub

    ''' <summary>
    ''' Returns true if the vehicle has a real AI (as opposed to a Fake AI)
    ''' </summary>
    ''' <returns></returns>
    Private Function VehicleHasRealAI() As Boolean
        If MyVehicle.AdditionalInterests Is Nothing Then Return False
        If MyVehicle.AdditionalInterests.Count = 0 Then Return False
        For Each ai As QuickQuote.CommonObjects.QuickQuoteAdditionalInterest In MyVehicle.AdditionalInterests
            If ai.Name IsNot Nothing Then
                If ai.Name.FirstName.ToUpper <> "FAKE" AndAlso ai.Name.LastName.ToUpper <> "AI" Then Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' Returns true if the vehicle has a real AI (as opposed to a Fake AI)
    ''' </summary>
    ''' <returns></returns>
    Private Function VehicleHasAnyAI() As Boolean
        If MyVehicle.AdditionalInterests Is Nothing Then Return False
        If MyVehicle.AdditionalInterests.Count = 0 Then Return False Else Return True
    End Function

    Protected Sub lnkRemove_Click(sender As Object, e As EventArgs) Handles lnkRemove.Click
        If Quote IsNot Nothing Then
            If Me.Quote.Vehicles IsNot Nothing Then
                ' need to tell the list control to tell the other driver controls to save now
                ' you need to do this because the indexes will be different after the driver is removed
                Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))

                ' all the other drivers info is now saved so it is safe to remove this one
                Me.Quote.Vehicles.RemoveAt(Me.VehicleIndex)
                ' let the list know which pane is about to be removed for active pane management
                RaiseEvent VehicleControlRemoving(Me.VehicleIndex)

                ' need to tell the list to refresh to get new driver indexes
                Me.ParentVrControl.Populate()

                ' save again after the removed
                Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
            End If

        End If
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click, lnkBtnDriverAssSave.Click, lnkBtnSpecificSave.Click,
                                                                                lnkBtnSaveOtherInfo.Click, lnkBtnGaragedSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Private Sub SetNamedNonOwner(applied As Boolean, Optional setEditmode As Boolean = False)

        If applied Then
            ' it wasn't checked a second ago so hold on to the old values
            If setEditmode Then
                Me.NamedNonOwnedAndExtended_FormCache.Clear()
                If MyVehicle IsNot Nothing Then
                    Me.NamedNonOwnedAndExtended_FormCache.Add("vin", If(MyVehicle.Vin.ToLower() <> "none", MyVehicle.Vin, ""))
                    Me.NamedNonOwnedAndExtended_FormCache.Add("make", If(MyVehicle.Make.ToLower() <> "named", MyVehicle.Make, ""))
                    Me.NamedNonOwnedAndExtended_FormCache.Add("model", If(MyVehicle.Model.ToLower() <> "non-owner", MyVehicle.Model, ""))
                    Me.NamedNonOwnedAndExtended_FormCache.Add("year", MyVehicle.Year)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("bodyTypeID", MyVehicle.BodyTypeId)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("useID", MyVehicle.VehicleUseTypeId)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("airBagsID", MyVehicle.RestraintTypeId)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("antiTheftID", MyVehicle.AntiTheftTypeId)
                    If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 1 Then
                        If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 2 Then
                            Me.NamedNonOwnedAndExtended_FormCache.Add("symbols", MyVehicle.VehicleSymbols(0).UserOverrideSymbol + "/" + MyVehicle.VehicleSymbols(1).UserOverrideSymbol + "/" + MyVehicle.VehicleSymbols(2).UserOverrideSymbol)
                        Else
                            Me.NamedNonOwnedAndExtended_FormCache.Add("symbols", MyVehicle.VehicleSymbols(0).UserOverrideSymbol + "/" + MyVehicle.VehicleSymbols(1).UserOverrideSymbol)
                        End If

                    Else
                        Me.NamedNonOwnedAndExtended_FormCache.Add("symbols", "")
                    End If

                    Me.NamedNonOwnedAndExtended_FormCache.Add("performance", MyVehicle.PerformanceTypeId)
                End If
            End If

            Me.txtVinNumber.Text = "NONE"
            Me.txtMake.Text = "Named"
            Me.txtModel.Text = "Non-Owner"
            If IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Month >= 10 Then
                Me.txtYear.Text = (IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year + 1).ToString()
            Else
                Me.txtYear.Text = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().Year.ToString()
            End If

            Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBodyType, "14")
            Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddUse, "6")

            'Me.ddPrincipalDriver.SelectedIndex = 0
            'Me.ddOccDriver1.SelectedIndex = 0
            'Me.ddOccDriver2.SelectedIndex = 0
            'Me.ddOccDriver3.SelectedIndex = 0

            Me.ddAirBags.SelectedIndex = 0
            Me.ddAntiTheft.SelectedIndex = 0

            Me.txtSymbol.Text = ""
            Me.ddPerformance.SelectedIndex = 0
            Me.txtCostNew.Text = ""

        Else
            If NamedNonOwnedAndExtended_FormCache.Any() Then
                Me.txtVinNumber.Text = Me.NamedNonOwnedAndExtended_FormCache("vin")
                Me.txtMake.Text = Me.NamedNonOwnedAndExtended_FormCache("make")
                Me.txtModel.Text = Me.NamedNonOwnedAndExtended_FormCache("model")
                Me.txtYear.Text = Me.NamedNonOwnedAndExtended_FormCache("year")

                Me.txtSymbol.Text = Me.NamedNonOwnedAndExtended_FormCache("symbols")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBodyType, Me.NamedNonOwnedAndExtended_FormCache("bodyTypeID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddUse, Me.NamedNonOwnedAndExtended_FormCache("useID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAirBags, Me.NamedNonOwnedAndExtended_FormCache("airBagsID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAntiTheft, Me.NamedNonOwnedAndExtended_FormCache("antiTheftID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddPerformance, Me.NamedNonOwnedAndExtended_FormCache("performance"))

            Else
                Me.txtVinNumber.Text = ""
                Me.txtMake.Text = ""
                Me.txtModel.Text = ""
                Me.txtYear.Text = ""

                Me.txtSymbol.Text = ""

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBodyType, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddUse, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAirBags, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAntiTheft, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddPerformance, "0")
            End If
        End If

        Me.txtVinNumber.Enabled = Not applied
        Me.txtMake.Enabled = Not applied
        Me.txtModel.Enabled = Not applied
        Me.txtYear.Enabled = Not applied

        Me.ddBodyType.Enabled = Not applied
        Me.ddUse.Enabled = Not applied

        Me.ddAirBags.Enabled = Not applied
        Me.ddAntiTheft.Enabled = Not applied
        Me.chkExtenedNonOwned.Enabled = Not applied

        If setEditmode Then
            Me.VRScript.AddScriptLine("ifm.vr.ui.LockTree_Freeze(); ")
        End If

    End Sub

    Protected Sub chkNamedNonOwner_CheckedChanged(sender As Object, e As EventArgs) Handles chkNamedNonOwner.CheckedChanged
        'Me.Save()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        SetNamedNonOwner(Me.chkNamedNonOwner.Checked, True)
        'Me.Save()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        Populate()

    End Sub

    Private Sub SetExtendedNonOwned(applied As Boolean, Optional setEditmode As Boolean = False)

        If applied Then
            ' it wasn't checked a second ago so hold on to the old values
            If setEditmode Then
                Me.NamedNonOwnedAndExtended_FormCache.Clear()
                If MyVehicle IsNot Nothing Then
                    Me.NamedNonOwnedAndExtended_FormCache.Add("vin", If(MyVehicle.Vin.ToLower() <> "none", MyVehicle.Vin, ""))
                    Me.NamedNonOwnedAndExtended_FormCache.Add("make", If(MyVehicle.Make.ToLower() <> "extd", MyVehicle.Make, ""))
                    Me.NamedNonOwnedAndExtended_FormCache.Add("model", If(MyVehicle.Model.ToLower() <> "non-owned", MyVehicle.Model, ""))
                    Me.NamedNonOwnedAndExtended_FormCache.Add("year", MyVehicle.Year)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("bodyTypeID", MyVehicle.BodyTypeId)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("useID", MyVehicle.VehicleUseTypeId)

                    Me.NamedNonOwnedAndExtended_FormCache.Add("airBagsID", MyVehicle.RestraintTypeId)
                    Me.NamedNonOwnedAndExtended_FormCache.Add("antiTheftID", MyVehicle.AntiTheftTypeId)

                    If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 1 Then
                        If MyVehicle.VehicleSymbols IsNot Nothing AndAlso MyVehicle.VehicleSymbols.Count > 2 Then
                            Me.NamedNonOwnedAndExtended_FormCache.Add("symbols", MyVehicle.VehicleSymbols(0).UserOverrideSymbol + "/" + MyVehicle.VehicleSymbols(1).UserOverrideSymbol + "/" + MyVehicle.VehicleSymbols(2).UserOverrideSymbol)
                        Else
                            Me.NamedNonOwnedAndExtended_FormCache.Add("symbols", MyVehicle.VehicleSymbols(0).UserOverrideSymbol + "/" + MyVehicle.VehicleSymbols(1).UserOverrideSymbol)
                        End If

                    Else
                        Me.NamedNonOwnedAndExtended_FormCache.Add("symbols", "")
                    End If

                    Me.NamedNonOwnedAndExtended_FormCache.Add("performance", MyVehicle.PerformanceTypeId)
                End If
            End If

            Me.txtVinNumber.Text = "NONE"
            Me.txtMake.Text = "EXTD"
            Me.txtModel.Text = "NON-OWNED"
            Me.txtYear.Text = "1900"

            Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBodyType, "14")
            Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddUse, "6")

            Me.ddAirBags.SelectedIndex = 0
            Me.ddAntiTheft.SelectedIndex = 0

            Me.txtSymbol.Text = ""
            Me.ddPerformance.SelectedIndex = 0
            Me.txtCostNew.Text = ""

        Else
            If NamedNonOwnedAndExtended_FormCache.Any() Then
                Me.txtVinNumber.Text = Me.NamedNonOwnedAndExtended_FormCache("vin")
                Me.txtMake.Text = Me.NamedNonOwnedAndExtended_FormCache("make")
                Me.txtModel.Text = Me.NamedNonOwnedAndExtended_FormCache("model")
                Me.txtYear.Text = Me.NamedNonOwnedAndExtended_FormCache("year")
                Me.txtSymbol.Text = Me.NamedNonOwnedAndExtended_FormCache("symbols")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBodyType, Me.NamedNonOwnedAndExtended_FormCache("bodyTypeID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddUse, Me.NamedNonOwnedAndExtended_FormCache("useID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAirBags, Me.NamedNonOwnedAndExtended_FormCache("airBagsID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAntiTheft, Me.NamedNonOwnedAndExtended_FormCache("antiTheftID"))

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddPerformance, Me.NamedNonOwnedAndExtended_FormCache("performance"))
            Else
                Me.txtVinNumber.Text = ""
                Me.txtMake.Text = ""
                Me.txtModel.Text = ""
                Me.txtYear.Text = ""

                Me.txtSymbol.Text = ""

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddBodyType, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddUse, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAirBags, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAntiTheft, "0")

                Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddPerformance, "0")
            End If
        End If

        Me.txtVinNumber.Enabled = Not applied
        Me.txtMake.Enabled = Not applied
        Me.txtModel.Enabled = Not applied
        Me.txtYear.Enabled = Not applied

        Me.ddBodyType.Enabled = Not applied
        Me.ddUse.Enabled = Not applied

        Me.ddAirBags.Enabled = Not applied
        Me.ddAntiTheft.Enabled = Not applied
        Me.chkNamedNonOwner.Enabled = Not applied

        If setEditmode Then
            Me.VRScript.AddScriptLine("ifm.vr.ui.LockTree_Freeze(); ")
        End If

    End Sub

    Protected Sub chkExtenedNonOwned_CheckedChanged(sender As Object, e As EventArgs) Handles chkExtenedNonOwned.CheckedChanged
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        'Me.Save()
        SetExtendedNonOwned(Me.chkExtenedNonOwned.Checked, True)
        'Save()
        Me.Save_FireSaveEvent(New VrControlBaseSaveEventArgs(Me, False, New IFM.VR.Web.VRValidationArgs(DefaultValidationType)))
        Populate()
    End Sub

    Protected Sub lnkBtnAddvehicle_Click(sender As Object, e As EventArgs) Handles lnkBtnAddvehicle.Click
        RaiseEvent NewVehcileRequested()
    End Sub

    Public Sub chkVehicleHasALienholderOrLease_CheckedChanged(sender As Object, e As EventArgs) Handles chkVehicleHasALienholderOrLease.CheckedChanged
        If IsQuoteEndorsement() Then
            If chkVehicleHasALienholderOrLease.Checked Then
                ctlVehicleAdditionalInterestList.lnkBtnAdd_Click(sender, e)
            End If
        End If

    End Sub

    Public Overrides Sub ClearControl()
        Me.txtCashValue.Text = ""
        Me.txtCostNew.Text = ""
        Me.txtGaragedApt.Text = ""
        Me.txtGaragedCity.Text = ""
        Me.txtGaragedCounty.Text = ""
        Me.txtGaragedOtherInfo.Text = ""
        Me.txtGaragedStreet.Text = ""
        Me.txtGaragedStreetNum.Text = ""
        Me.txtGaragedZip.Text = ""
        Me.txtHorsePower.Text = ""
        Me.txtMake.Text = ""
        Me.txtModel.Text = ""
        Me.txtStatedAmt.Text = ""
        If IFM.VR.Common.Helpers.PPA.CustomEquipmentHelper.IsCustomEquipmentAvailable(Quote) Then
            Me.txtCustomEquipment.Text = ""
        End If
        Me.txtSymbol.Text = ""
        Me.txtVinNumber.Text = ""
        Me.txtYear.Text = ""
        Me.ddAirBags.SelectedIndex = -1
        Me.ddAntiTheft.SelectedIndex = -1
        Me.ddBodyType.SelectedIndex = -1
        Me.ddGaragedState.SelectedIndex = -1
        Me.ddMotorCyleType.SelectedIndex = -1
        Me.ddOccDriver1.SelectedIndex = -1
        Me.ddOccDriver2.SelectedIndex = -1
        Me.ddOccDriver3.SelectedIndex = -1
        Me.ddPerformance.SelectedIndex = -1
        Me.ddPrincipalDriver.SelectedIndex = -1
        Me.ddUse.SelectedIndex = -1
        Me.chkExtenedNonOwned.Checked = False
        Me.chkNamedNonOwner.Checked = False

        MyBase.ClearControl()
    End Sub

    Private Sub lnkBtnReplace_Click(sender As Object, e As EventArgs) Handles lnkBtnReplace.Click
        RaiseEvent ReplaceVehicleTitleBar(VehicleIndex)
    End Sub

End Class