Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Validation.ObjectValidation.AllLines
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports IFM.PrimativeExtensions
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.Helpers
Imports Diamond.Common.Enums.Billing

Public Class ctlProperty_Address
    Inherits VRControlBase

    Public Event PopulateLocationHeader()
    Public Event ProtectionClassLookupNeeded()
    Public Event PropertyAddressChanged()
    Public Event PropertyCleared()
    Public Event HideHeaderButtons()
    Public Event ShowHeaderButtons()

    ''' If this is NOT governing state location 0
    ''' AND the address matches exactly except for the state
    ''' then the 'No Owned Location in this state' checkbox applies
    ''' Added for KY expansion MGB 4/29/19
    Private ReadOnly Property NoOwnedLocationApplies() As Boolean
        Get
            If Quote IsNot Nothing Then
                If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then
                    If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.Locations IsNot Nothing AndAlso GoverningStateQuote.Locations.Count > 0 AndAlso GoverningStateQuote.Locations(0).Address IsNot Nothing Then
                        If MyLocation.Equals(GoverningStateQuote.Locations(0)) Then
                            ' This is governing state quote location 0
                            Return False
                        Else
                            If chkNoOwnedLocation.Checked Then Return True
                            ' Not governing state location 0, check it
                            Dim myAddr As QuickQuote.CommonObjects.QuickQuoteAddress = MyLocation.Address
                            Dim govAddr As QuickQuote.CommonObjects.QuickQuoteAddress = GoverningStateQuote.Locations(0).Address
                            ' Don't check if the governing state address is empty
                            If Not (govAddr.HouseNum.IsNullEmptyorWhitespace _
                                AndAlso govAddr.StreetName.IsNullEmptyorWhitespace _
                                AndAlso govAddr.City.IsNullEmptyorWhitespace _
                                AndAlso govAddr.Zip.IsNullEmptyorWhitespace) Then
                                ' If the address matches governing state location 0 except for state the No Owned Location applies
                                If myAddr.HouseNum = govAddr.HouseNum AndAlso
                                myAddr.StreetName = govAddr.StreetName AndAlso
                                myAddr.City = govAddr.City AndAlso
                                myAddr.Zip = govAddr.Zip AndAlso
                                myAddr.County = govAddr.County AndAlso
                                myAddr.ApartmentNumber = govAddr.ApartmentNumber AndAlso
                                myAddr.State <> govAddr.State Then
                                    Return True
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            Return False
        End Get
    End Property

    Public Property MyLocationIndex As Int32
        Get
            If ViewState("vs_locationIndex") IsNot Nothing Then
                Return CInt(ViewState("vs_locationIndex"))
            End If
            Return 0
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property IsFirstLocationForStatePart As Boolean
        Get
            Dim _isLocationFirstForStatePart = False
            If Me.MyLocation IsNot Nothing Then
                For Each statePart_State In Me.Quote.QuoteStates
                    Dim loc = IFM.VR.Common.Helpers.MultiState.Locations.GetFirstLocationForStateQuote(Me.Quote, statePart_State)
                    If Me.MyLocation.Equals(loc) Then
                        _isLocationFirstForStatePart = True
                    End If
                Next
            End If
            Return _isLocationFirstForStatePart
        End Get
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            Dim getLocationAtIndex = Function() As QuickQuote.CommonObjects.QuickQuoteLocation
                                         Return Me.Quote?.Locations?.GetItemAtIndex(MyLocationIndex)
                                     End Function
            ' some lobs don't actually want a location 0 they really want is the governing state first location
            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    If Me.MyLocationIndex = 0 Then
                        Dim l = IFM.VR.Common.Helpers.MultiState.Locations.LocationsForGoverningState(Me.Quote, Me.GoverningStateQuote)
                        If l IsNot Nothing AndAlso l.Any() Then
                            Return l.First()
                        End If
                        Return Nothing
                    Else
                        Return getLocationAtIndex()
                    End If
                Case Else
                    Return getLocationAtIndex()
            End Select
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property StreetAddressNum() As String
        Get
            Return txtStreetNum.Text
        End Get
    End Property

    Public ReadOnly Property StreetAddressName() As String
        Get
            Return txtStreetName.Text
        End Get
    End Property

    Public ReadOnly Property City() As String
        Get
            Return txtCityName.Text
        End Get
    End Property

    Public ReadOnly Property County() As String
        Get
            Return txtGaragedCounty.Text
        End Get
    End Property

    Public ReadOnly Property StreetNumClientID() As String
        Get
            Return txtStreetNum.ClientID
        End Get
    End Property

    Public ReadOnly Property CountyClientID() As String
        Get
            Return txtGaragedCounty.ClientID
        End Get
    End Property

    Public ReadOnly Property Section() As String
        Get
            Return txtSection.Text
        End Get
    End Property

    Public ReadOnly Property Township() As String
        Get
            Return txtTownship.Text
        End Get
    End Property

    Public ReadOnly Property Range() As String
        Get
            Return txtRange.Text
        End Get
    End Property

    Public ReadOnly Property NumberOfAmusementAreas() As String
        Get
            Return txtNumberOfAmusementAreas.Text
        End Get
    End Property

    Public ReadOnly Property NumberOfPlaygrounds() As String
        Get
            Return txtNumberOfPlaygrounds.Text
        End Get
    End Property
    Public ReadOnly Property NumberOfSwimmingPools() As String
        Get
            Return txtNumberOfSwimmingPools.Text
        End Get
    End Property

    Public Property ResidenceExists() As Boolean
        Get
            Return ViewState.GetBool("vs_ResidenceExists", False, True)
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_ResidenceExists") = value
        End Set
    End Property

    Private Property ProtectionClassesLoaded() As Boolean
        Get
            Return ViewState.GetBool("vs_PCLoaded", False, True)
        End Get
        Set(ByVal value As Boolean)
            ViewState("vs_PCLoaded") = value
        End Set
    End Property

    ''' <summary>
    ''' Used in WCP
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property NumberOfEmployees As String
        Get
            ' Find the first classification with number of employees set and return it.
            ' Number of employees will be the same on all classifications
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                If LOC.Classifications IsNot Nothing AndAlso LOC.Classifications.Count > 0 Then
                    For Each Cl As QuickQuote.CommonObjects.QuickQuoteClassification In LOC.Classifications
                        If Cl.NumberOfEmployees IsNot Nothing AndAlso IsNumeric(Cl.NumberOfEmployees) Then Return Cl.NumberOfEmployees
                    Next
                End If
            Next
            Return "0"  ' if we got here no value was found for number of employees
        End Get
    End Property

    Public ReadOnly Property isVeriskProtectionClassReady() As Boolean
        Get
            If Quote IsNot Nothing Then
                Return NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote)
            End If
            Return False
        End Get
    End Property

    Public Sub showVeriskProtClassIfAvailable()
        If Quote IsNot Nothing AndAlso NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
            Me.divVeriskFeetToHydrant.Visible = True
            Me.divVeriskProtectionClass.Visible = True
            Me.divFeetToHydrant.Visible = False
            Me.divProtectionClass.Visible = False
            Me.divMilesToFireDept.Visible = False

            If MyLocation IsNot Nothing Then
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(MyLocation.FireHydrantDistanceId) AndAlso CInt(MyLocation.FireHydrantDistanceId) > 0 Then
                    ddlVeriskFeetToHydrant.SelectedValue = MyLocation.FireHydrantDistanceId
                Else
                    If MyLocation.FeetToFireHydrant IsNot Nothing AndAlso MyLocation.FeetToFireHydrant <> String.Empty AndAlso IsNumeric(MyLocation.FeetToFireHydrant) AndAlso CInt(MyLocation.FeetToFireHydrant) > 0 Then
                        If CInt(MyLocation.FeetToFireHydrant) < 1000 Then
                            ddlVeriskFeetToHydrant.SelectedValue = 4
                        Else
                            ddlVeriskFeetToHydrant.SelectedValue = 1
                        End If
                    Else
                        ddlVeriskFeetToHydrant.SelectedIndex = -1
                    End If
                End If
                txtVeriskProtectionClass.Text = MyLocation.ProtectionClass
                txtVeriskProtectionClass.Enabled = False
            End If

        Else
            Me.divVeriskFeetToHydrant.Visible = False
            Me.divVeriskProtectionClass.Visible = False
        End If
    End Sub

    Public Sub saveVeriskProtClassIfAvailable()
        If Quote IsNot Nothing AndAlso NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
            If MyLocation IsNot Nothing Then
                'MyLocation.ProtectionClassId = txtVeriskProtectionClass.Text

                Me.MyLocation.FireHydrantDistanceId = ddlVeriskFeetToHydrant.SelectedValue
                If ddlVeriskFeetToHydrant.SelectedIndex = 1 Then
                    Me.MyLocation.FeetToFireHydrant = "1001"
                ElseIf ddlVeriskFeetToHydrant.SelectedIndex = 2 Then
                    Me.MyLocation.FeetToFireHydrant = "999"
                Else
                    Me.MyLocation.FeetToFireHydrant = ""
                End If

            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.AddressDiv.ClientID

        'If Not IsPostBack Then
        If Me.Quote IsNot Nothing AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
            Me.txtZipCode.AutoPostBack = True
            Me.txtCityName.AutoPostBack = True
            Me.ddCityName.AutoPostBack = True
            Me.txtGaragedCounty.AutoPostBack = True
        End If
        'End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Sub CopyAddressFromPolicyHolder()
        btnCopyAddress_Click(Me, New EventArgs())
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        ' ***************************************************************************************************************************
        ' *******   if you don't have a location yet it is because the kill questions didn't create the default location  ***********
        ' ***************************************************************************************************************************
        'If Me.MyLocation.IsNull AndAlso (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability) Then

        If MyLocation.IsNull Then
            ' NO LOCATION!
            ' You will not have a location on the Comm GL & Comm Auto quotes,
            ' but you still need the script for the controls and the accordion
            Me.VRScript.CreateAccordion(MainAccordionDivId, Me.hiddenAddressIsActive, "0")
            'Me.VRScript.CreateConfirmDialog(Me.lnkClearAddress.ClientID, "Clear Address?")
            Me.VRScript.StopEventPropagation(Me.lnkSaveAddress.ClientID)
            Me.VRScript.StopEventPropagation(Me.lnkClearAddress.ClientID)
            Me.VRScript.AddScriptLine("$(""#" + Me.txtCityName.ClientID + """).autocomplete({ source: INCities });")
            Me.VRScript.AddScriptLine("$(""#" + Me.txtGaragedCounty.ClientID + """).autocomplete({ source: indiana_Counties });")
            Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtGaragedCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
            Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();")
            Me.VRScript.CreateJSBinding(Me.ddCityName, ctlPageStartupScript.JsEventType.onchange, "CopyAddressFromPolicyHolder();if($(this).val() == '0'){$(""#" + Me.txtCityName.ClientID + """).show(); } else {$(""#" + Me.txtCityName.ClientID + """).val($(this).val()); $(""#" + Me.txtCityName.ClientID + """).hide();}")
            Exit Sub
        Else
            Me.VRScript.StopEventPropagation(Me.lnkSaveAddress.ClientID)
            Me.VRScript.StopEventPropagation(Me.lnkClearAddress.ClientID)

            ' LOCATION EXISTS
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Or Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Or Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Or String.IsNullOrWhiteSpace(Me.MyLocation.Address.StreetName) Or String.IsNullOrWhiteSpace(Me.MyLocation.Address.HouseNum) Or Me.ddStateAbbrev.SelectedValue <> "16" Then
                'always open on FAR, BOP, CAP, WCP Quotes
                Me.VRScript.CreateAccordion(MainAccordionDivId, Me.hiddenAddressIsActive, "0")
            Else
                ' might be open, might be closed
                Me.VRScript.CreateAccordion(MainAccordionDivId, Me.hiddenAddressIsActive, "false")
            End If

            'Address Scripts
            Dim headerLabelScriptVariable As String = "locationHeader_" + Me.MyLocationIndex.ToString()
            Dim treeAddressUpdateScript As String = "$('#cphMain_ctlTreeView_rptResidences_lblResidenceDescription_0').text($('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtCityName.ClientID + "').val().toUpperCase());"
            Dim headerUpdateScript As String = ""

            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    ' Added code to truncate the header as it's being updated  MGB 3/20/18 Bug 25666
                    headerUpdateScript = "var txt = " & "$('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase();"
                    headerUpdateScript += "if (txt.length > 38){txt = txt.substring(0,38) + '...';}"
                    headerUpdateScript += String.Format("$('#' + " + headerLabelScriptVariable + ").text('LOCATION #{0} - ' + txt);", MyLocationIndex + 1)
                    'headerUpdateScript = String.Format("$('#' + " + headerLabelScriptVariable + ").text('LOCATION #{0} - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + '');", MyLocationIndex + 1)
                    treeAddressUpdateScript = "" ' just until the treeview items is created
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    headerUpdateScript = "$('#' + " + headerLabelScriptVariable + ").text('Property - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtAptNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtCityName.ClientID + "').val().toUpperCase());"
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    'If MyLocation.AcreageOnly Then
                    '    headerUpdateScript = String.Format("$('#' + " + headerLabelScriptVariable + ").text('Acreage Only LOCATION #{0} - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ', ' + $('#" + Me.txtCityName.ClientID + "').val().toUpperCase() + '');", MyLocationIndex + 1)
                    '    treeAddressUpdateScript = "" ' just until the treeview items is created
                    'Else
                    headerUpdateScript = String.Format("$('#' + " + headerLabelScriptVariable + ").text('LOCATION #{0} - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ', ' + $('#" + Me.txtCityName.ClientID + "').val().toUpperCase() + 'S/T/R: ' + $('#" + Me.txtSection.ClientID + "').val().toUpperCase() + '/' + $('#" + Me.txtTownship.ClientID + "').val().toUpperCase() + '/' + $('#" + Me.txtRange.ClientID + "').val().toUpperCase() + '');", MyLocationIndex + 1)
                    treeAddressUpdateScript = "" ' just until the treeview items is created
            'End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    headerUpdateScript = "$('#' + " + headerLabelScriptVariable + ").text('Location - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ' ' + "
                    If Not String.IsNullOrWhiteSpace(Me.txtAptNum.Text) Then
                        headerUpdateScript += "$('#" + Me.txtAptNum.ClientID + "').val().toUpperCase() + ' ' + "
                    End If
                    headerUpdateScript += "$('#" + Me.txtCityName.ClientID + "').val().toUpperCase());"
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    ' Added CPR 1-29-2018 MGB
                    headerUpdateScript = String.Format("$('#' + " + headerLabelScriptVariable + ").text(Cpr.TruncateEllipse('Location #{0} - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtCityName.ClientID + "').val().toUpperCase() + '', 37));", MyLocationIndex + 1)
                    'headerUpdateScript = String.Format("$('#' + " + headerLabelScriptVariable + ").text(Cpr.TruncateEllipse('Location #{0} - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + '', 37));", MyLocationIndex + 1)
                    treeAddressUpdateScript = "" ' just until the treeview items is created
                Case Else
                    headerUpdateScript = "$('#' + " + headerLabelScriptVariable + ").text('Property - ' + $('#" + Me.txtStreetNum.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtStreetName.ClientID + "').val().toUpperCase() + ' ' + $('#" + Me.txtCityName.ClientID + "').val().toUpperCase());"
            End Select

            'Uncomment below code to fix Property addresses appearing in title bar; this was shelved because of testing time concerns
            'Me.VRScript.AddScriptLine(headerUpdateScript)

            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtAptNum, Me.txtCityName}, ctlPageStartupScript.JsEventType.onkeyup, headerUpdateScript + treeAddressUpdateScript)
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtAptNum, Me.txtCityName}, ctlPageStartupScript.JsEventType.onkeyup, headerUpdateScript + treeAddressUpdateScript)
                Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    ' Js to handle 'No owned locations' checkbox clicks  MGB 4-29-19
                    'Me.VRScript.CreateJSBinding(chkNoOwnedLocation.ClientID, ctlPageStartupScript.JsEventType.onclick, "Wcp.NoOwnedLocationsCheckboxChanged('" & chkNoOwnedLocation.ClientID & "','" & txtStreetNum.ClientID & "','" & txtStreetName.ClientID & "','" & txtCityName.ClientID & "','" & ddStateAbbrev.ClientID & "','" & txtZipCode.ClientID & "','" & txtGaragedCounty.ClientID & "','" & txtNumberOfEmployees.ClientID & "');")

                    Dim LocControl As ctl_WCP_Location = Me.Parent
                    Dim SaveBtnId As String = LocControl.SaveButtonId
                    Dim ClearBtnId As String = LocControl.ClearButtonId
                    Dim AddBtnId As String = LocControl.AddButtonId
                    Dim myClearBtnId As String = lnkClearAddress.ClientID
                    Dim mySaveBtnId As String = lnkSaveAddress.ClientID
                    Me.VRScript.CreateJSBinding(chkNoOwnedLocation.ClientID, ctlPageStartupScript.JsEventType.onclick, "Wcp.NoOwnedLocationsCheckboxChanged('" & chkNoOwnedLocation.ClientID & "','" & txtStreetNum.ClientID & "','" & txtStreetName.ClientID & "','" & txtCityName.ClientID & "','" & ddStateAbbrev.ClientID & "','" & txtZipCode.ClientID & "','" & txtGaragedCounty.ClientID & "','" & txtNumberOfEmployees.ClientID & "','" & SaveBtnId & "','" & ClearBtnId & "','" & AddBtnId & "','" & mySaveBtnId & "','" & myClearBtnId & "');")
                Case Else
                    Me.VRScript.CreateJSBinding({Me.txtStreetNum, Me.txtStreetName, Me.txtCityName}, ctlPageStartupScript.JsEventType.onkeyup, headerUpdateScript + treeAddressUpdateScript)
                    Me.VRScript.CreateJSBinding({Me.txtSection, Me.txtTownship, Me.txtRange}, ctlPageStartupScript.JsEventType.onkeyup, headerUpdateScript + treeAddressUpdateScript)
            End Select

            Me.VRScript.AddScriptLine("$(""#" + Me.txtCityName.ClientID + """).autocomplete({ source: INCities });")
            Me.VRScript.AddScriptLine("$(""#" + Me.txtGaragedCounty.ClientID + """).autocomplete({ source: indiana_Counties });")
            Me.VRScript.CreateJSBinding(Me.txtZipCode, ctlPageStartupScript.JsEventType.onkeyup, "DoCityCountyLookup('" + Me.txtZipCode.ClientID + "','" + Me.ddCityName.ClientID + "','" + Me.txtCityName.ClientID + "','" + Me.txtGaragedCounty.ClientID + "','" + Me.ddStateAbbrev.ClientID + "');")
            Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).hide();")
            Me.VRScript.CreateJSBinding(Me.ddCityName, ctlPageStartupScript.JsEventType.onchange, "CopyAddressFromPolicyHolder();if($(this).val() == '0'){$(""#" + Me.txtCityName.ClientID + """).show(); } else {$(""#" + Me.txtCityName.ClientID + """).val($(this).val()); $(""#" + Me.txtCityName.ClientID + """).hide();}")

            'Added 9/20/2022 for bug 76748 MLW
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                Me.VRScript.CreateJSBinding(Me.ddStateAbbrev, ctlPageStartupScript.JsEventType.onchange, "locationStateChanged('" + Me.ddStateAbbrev.ClientID + "');")
            End If

            'Me.VRScript.AddScriptLine("$(""#" + Me.ddCityName.ClientID + """).bind(""change"",function(e){ CopyAddressFromPolicyHolder();if($(this).val() == '0'){$(""#" + Me.txtCityName.ClientID + """).show(); } else {$(""#" + Me.txtCityName.ClientID + """).val($(this).val()); $(""#" + Me.txtCityName.ClientID + """).hide();}});")

            ' might want to change to ifm.vr.ui.StopEventPropagation(e); but haven't tested it
            Dim copyScript As String = "event.stopPropagation(); $('#" + Me.txtAptNum.ClientID + "').val('" + Me.Quote.Policyholder.Address.ApartmentNumber + "');"
            copyScript += "$('#" + Me.txtCityName.ClientID + "').val('" + Me.Quote.Policyholder.Address.City + "');"
            copyScript += "$('#" + Me.txtGaragedCounty.ClientID + "').val('" + Me.Quote.Policyholder.Address.County + "');"
            copyScript += "$('#" + Me.txtStreetName.ClientID + "').val('" + Me.Quote.Policyholder.Address.StreetName + "');"
            copyScript += "$('#" + Me.txtStreetNum.ClientID + "').val('" + Me.Quote.Policyholder.Address.HouseNum + "');"
            copyScript += "$('#" + Me.txtZipCode.ClientID + "').val('" + Me.Quote.Policyholder.Address.Zip + "');"
            copyScript += "$('#" + Me.ddStateAbbrev.ClientID + "').val('" + Me.Quote.Policyholder.Address.StateId + "');"

            ''' Protection Class - HOM & DFR ONLY
            ''' HOM & DFR set the protection class in a different control so this script is needed
            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                '            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal OrElse Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                Dim pclookup As String = String.Format("ProtectionClass.GetProtectionClass($('#{1}').val(),$('#{2}').val(),$('#{3}').val(),pc_ddprotectionId_Loc{0},pc_txtfeetToHydrantId_Loc{0},pc_txtMilesToFDId_Loc{0},pc_trFeetId_Loc{0},pc_trMilesFDId_Loc{0});", Me.MyLocationIndex, Me.txtCityName.ClientID, Me.txtGaragedCounty.ClientID, Me.ddStateAbbrev.ClientID)

                Me.btnCopyAddress.OnClientClick = copyScript + pclookup + "return false;"
                Me.VRScript.CreateJSBinding({Me.txtZipCode, Me.ddCityName, Me.txtCityName, Me.txtGaragedCounty}, ctlPageStartupScript.JsEventType.onblur, pclookup)

                'run protection class lookup logic on every load '8-5-14 Matt A
                Me.VRScript.AddScriptLine(pclookup)
            End If

            ' Number of Amusement Areas, Playgrounds, Swimming Pools - Commercial BOP Only MGB 2.27.17
            If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                Dim scriptRound As String = "Bop.CheckBOPPropertyValues(""" + txtNumberOfAmusementAreas.ClientID + """);"
                txtNumberOfAmusementAreas.Attributes.Add("onblur", scriptRound)
                scriptRound = "Bop.CheckBOPPropertyValues(""" + txtNumberOfPlaygrounds.ClientID + """);"
                txtNumberOfPlaygrounds.Attributes.Add("onblur", scriptRound)
                scriptRound = "Bop.CheckBOPPropertyValues(""" + txtNumberOfSwimmingPools.ClientID + """);"
                txtNumberOfSwimmingPools.Attributes.Add("onblur", scriptRound)
            End If

            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                ' Acreage Rounding
                'Dim scriptRoundAcre As String = "AlwaysRoundToNextNumber (""" + txtAcerage.ClientID + """);"
                'txtAcerage.Attributes.Add("onblur", scriptRoundAcre)
                Dim scriptAcreChange As String = "TxtAcerageChange (""" + txtAcerage.ClientID + """, """ + divBlanketAcreage.ClientID + """,""" + chkBlanketAcreage.ClientID + """, """ + divtxtTotalBlanketAcreage.ClientID + """);"
                Me.VRScript.CreateJSBinding(Me.txtAcerage, ctlPageStartupScript.JsEventType.onblur, scriptAcreChange, True)
                txtAcerage.Attributes.Add("onblur", scriptAcreChange)

                Dim scriptRoundBlanketAcre As String = "AlwaysRoundToNextNumber (""" + txtTotalBlanketAcreage.ClientID + """);"
                txtTotalBlanketAcreage.Attributes.Add("onblur", scriptRoundBlanketAcre)

                'Toggle BlanketAcarageTextbox
                Dim scriptchkBlanketAcreage As String = "ToggleBlanketAcreageTxtbox (""" + chkBlanketAcreage.ClientID + """, """ + divtxtTotalBlanketAcreage.ClientID + """);"
                chkBlanketAcreage.Attributes.Add("onclick", scriptchkBlanketAcreage)


                If IsOnAppPage = False AndAlso Me.MyLocationIndex = 0 AndAlso (IsQuoteReadOnly() = False AndAlso IsQuoteEndorsement() = False) Then
                    Me.VRScript.AddScriptLine(String.Format("var acresLocation_{0} = new Array();", Me.MyLocationIndex, Me.txtAcerage.ClientID))
                    Me.VRScript.AddScriptLine(String.Format("acresLocation_{0}.push(""{1}"");", Me.MyLocationIndex, Me.txtAcerage.ClientID))
                    Dim startingAcreage As Integer = IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.CurrentTotalAcres(Me.MyLocation, False)
                    Me.VRScript.AddScriptLine(String.Format("var acresLocation_{0}_starting = {1};", Me.MyLocationIndex, startingAcreage))
                End If

                ' Need Township lookup logic
                'Todo - Matt - MS - I'd be surprised if this works because the location probably hasn't been moved to the state part yet to get the proper versionId. Probably need to lookup versionId from lob, effective, and whatever the location thinks its current stateid is.
                'Dim versionIdForLocation As String = If(String.IsNullOrWhiteSpace(Me.SubQuoteForLocation(Me.MyLocation)?.VersionId) = False, Me.SubQuoteForLocation(Me.MyLocation).VersionId, Me.Quote.VersionId)
                'Dim townshipLookup As String = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtGaragedCounty.ClientID}').val(),'{versionIdForLocation}','{Me.hdnTownshipName.ClientID}');"

                Dim townshipLookup As String = $"VRData.TownShip.GetTownshipsByCountyNameBindToDropdown('{Me.ddTownshipName.ClientID}',$('#{Me.txtGaragedCounty.ClientID}').val(),null,'{Me.hdnTownshipName.ClientID}',$('#{Me.ddStateAbbrev.ClientID}').val(), master_LobId, master_effectiveDate);"


                Me.VRScript.CreateJSBinding({Me.txtGaragedCounty, Me.txtZipCode}, ctlPageStartupScript.JsEventType.onblur, townshipLookup)
                Me.VRScript.AddScriptLine(townshipLookup) ' do a lookup on startup of page
                'set the hidden field value when ever the dropdown value changes
                Me.VRScript.CreateJSBinding(Me.ddTownshipName, ctlPageStartupScript.JsEventType.onchange, "$('#" + Me.hdnTownshipName.ClientID + "').val($('#" + Me.ddTownshipName.ClientID + "').val());")

                'chkAcreageOnly.ID = "chkAcreageOnly" + Me.MyLocationIndex.ToString()
                'chkAcreageOnly.ClientIDMode = UI.ClientIDMode.Static

                'Dim scriptAcreageOnly As String = "ToggleAcreageOnlyCheck(this, """ + hiddenAcreageOnly.ClientID + """);"
                'VRScript.CreateJSBinding("chkAcreageOnly" + MyLocationIndex.ToString(), ctlPageStartupScript.JsEventType.onclick, scriptAcreageOnly)

            End If
        End If

        ' Always do this stuff
        Me.VRScript.AddVariableLine($"var locationHeader_{Me.MyLocationIndex} = '{Me.lblAccordHeader.ClientID}';")
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_txtcountyId = '{1}';", Me.MyLocationIndex.ToString(), Me.txtGaragedCounty.ClientID)) 'used on ctl_FarmBuilding_Coverages, ctResidenceCoverages
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_txtzipId = '{1}';", Me.MyLocationIndex.ToString(), Me.txtZipCode.ClientID))
        Me.VRScript.AddVariableLine(String.Format("var location_{0}_ddstateId = '{1}';", Me.MyLocationIndex.ToString(), Me.ddStateAbbrev.ClientID)) 'Added 11/2/18 for multi state MLW
        txtAcerage.Attributes.Add("onfocus", "this.select()")
        txtStreetNum.Attributes.Add("onfocus", "this.select()")
        txtStreetName.Attributes.Add("onfocus", "this.select()")
        txtAptNum.Attributes.Add("onfocus", "this.select()")
        txtSection.Attributes.Add("onfocus", "this.select()")
        txtTownship.Attributes.Add("onfocus", "this.select()")
        txtRange.Attributes.Add("onfocus", "this.select()")
        txtZipCode.Attributes.Add("onfocus", "this.select()")
        txtCityName.Attributes.Add("onfocus", "this.select()")
        txtGaragedCounty.Attributes.Add("onfocus", "this.select()")
        txtDescrption.Attributes.Add("onfocus", "this.select()")
    End Sub

    Public Overrides Sub LoadStaticData()
        If Me.ddStateAbbrev.Items.Count = 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddStateAbbrev, QuickQuoteClassName.QuickQuoteAddress, QuickQuotePropertyName.StateId, SortBy.None, Me.Quote.LobType)
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddlVeriskFeetToHydrant, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.FireHydrantDistanceId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        'Added 12/23/2020 for CAP Endorsements Task 52972 MLW
        If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto) Then
            LoadStaticData()

            Me.tdAcerage.Visible = False
            Me.tdSection.Visible = False
            Me.tdRange.Visible = False
            Me.tdDescription.Visible = False
            Me.tdTownship.Visible = False
            Me.tdTownshipName.Visible = False
            Me.divNumAmusementAreas.Visible = False
            Me.divNumberOfPlaygrounds.Visible = False
            Me.divNumberOfSwimmingPools.Visible = False
            Me.divPOBox.Visible = False
            Me.divNumberOfEmployees.Visible = False
            Me.divFeetToHydrant.Visible = False
            Me.divMilesToFireDept.Visible = False
            Me.divProtectionClass.Visible = False
            Me.divAddressInfoText.Visible = False

            Me.divVeriskFeetToHydrant.Visible = False
            Me.divVeriskProtectionClass.Visible = False

            ' Set the Header text
            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    lblAccordHeader.Text = "Primary Garaging Location"
                    Exit Select
                Case Else
                    lblAccordHeader.Text = "Address"
                    Exit Select
            End Select

            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm OrElse
                Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse
                Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage OrElse
                (MyLocation.IsNotNull AndAlso (String.IsNullOrWhiteSpace(Me.MyLocation.Address.HouseNum) OrElse
                String.IsNullOrWhiteSpace(Me.MyLocation.Address.StreetName) OrElse
                Me.MyLocation.Address.StateId <> Me.GoverningStateQuote.StateId)) Then
                Me.hiddenAddressIsActive.Value = "0" ' always opened on FAR, DFR, BOP, CAP, WCP, CGL, CPR, CPP or if there is an address populated
                'Added 8/23/18 for multi state MLW
                'If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso (Me.Quote.ProgramTypeId <> "6" Or MyLocationIndex <> 0) Then
                Dim ptID As String = Me.Quote.ProgramTypeId
                If SubQuoteFirst IsNot Nothing Then
                    ptID = Me.SubQuoteFirst.ProgramTypeId
                Else
                    ptID = Me.Quote.ProgramTypeId
                End If
                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso (ptID <> "6" Or MyLocationIndex <> 0) Then
                    Me.txtAcerage.Attributes.Add("autofocus", "")
                ElseIf Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                    Me.txtStreetNum.Attributes.Add("autofocus", "")
                End If
            Else
                Me.hiddenAddressIsActive.Value = "false"
            End If

            'Updated 9/4/18 for multi state MLW
            'If MyLocation.IsNotNull Then
            If MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then
                ' Some LOB's won't have a location

                ' Show the 'No owned locations" checkbox for WCP
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso (MyLocation.Address.QuickQuoteState <> Me.Quote.QuickQuoteState) Then
                    divNoOwnedLocation.Attributes.Add("style", "display:''")
                    If NoOwnedLocationApplies() Then chkNoOwnedLocation.Checked = True
                Else
                    divNoOwnedLocation.Attributes.Add("style", "display:none")
                    chkNoOwnedLocation.Checked = False
                End If

                'Address 7-15-14
                Me.txtStreetNum.Text = Me.MyLocation.Address.HouseNum
                Me.txtStreetName.Text = Me.MyLocation.Address.StreetName
                Me.txtAptNum.Text = Me.MyLocation.Address.ApartmentNumber

                If Me.MyLocation.Address.Zip <> "00000-0000" And Me.MyLocation.Address.Zip <> "00000" Then
                    Me.txtZipCode.Text = Me.MyLocation.Address.Zip.Replace("-0000", "")
                Else
                    Me.txtZipCode.Text = ""
                End If

                Me.txtPOBox.Text = MyLocation.Address.POBox   ' Added 9/27/17 MGB

                Me.txtCityName.Text = Me.MyLocation.Address.City
                If Me.MyLocation.Address.StateId = "" Or Me.MyLocation.Address.StateId = "0" Then
                    'Dim subQuotesWithNoLocations = IFM.VR.Common.Helpers.MultiState.Locations.SubQuoteStateIdsWithNoLocation(Me.Quote, Me.SubQuotes)
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, Me.GoverningStateQuote.StateId)
                Else
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, Me.MyLocation.Address.StateId)
                End If
                Me.txtGaragedCounty.Text = Me.MyLocation.Address.County

                If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                    Me.ddStateAbbrev.Enabled = Not IsFirstLocationForStatePart
                    Dim currentStateIdSelection = Me.MyLocation?.Address?.StateId
                    If currentStateIdSelection IsNot Nothing Then
                        If IsFirstLocationForStatePart AndAlso currentStateIdSelection <> "" AndAlso currentStateIdSelection <> Me.Quote.Policyholder.Address.StateId Then
                            btnCopyAddress.Visible = False
                        End If
                    End If
                End If

            End If

            Select Case Me.Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    Me.txtDescrption.Text = Me.MyLocation.Description
                    Me.tdDescription.Visible = False
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    ' do these in addition to the ones above
                    Me.tdAcerage.Visible = True
                    Me.tdSection.Visible = True
                    Me.tdRange.Visible = True
                    Me.tdDescription.Visible = True
                    Me.tdTownship.Visible = True
                    Me.tdTownshipName.Visible = True
                    Me.ctl_Farm_Location_Description_List.Visible = True
                    Me.ctl_Farm_Location_Description_List.MyLocationIndex = Me.MyLocationIndex

                    If Not IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.IsFarmBlanketAcreageAvailable(Quote) Then
                        Me.divBlanketAcreage.Visible = False
                    Else
                        Me.divBlanketAcreage.Visible = True
                    End If

                    'Disable BlanketAcreage for Non PrimaryLocations
                    If Me.MyLocationIndex > 0 Then
                        Me.divBlanketAcreage.Visible = False
                    End If
                    If MyLocation.Acreages Is Nothing Then
                        MyLocation.Acreages = New List(Of QuickQuoteAcreage)()
                    End If
                    If MyLocation.Acreages.Any() = False Then
                        MyLocation.Acreages.Add(New QuickQuoteAcreage())
                    End If
                    Dim acreage As QuickQuoteAcreage = MyLocation.Acreages(0)
                    Me.txtAcerage.Text = acreage.Acreage
                    If MyLocation.Acreages.Count > 1 Then
                        Me.lblAdditionalAcres.Text = String.Format("Total including Acreage Only: {0}", IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.CurrentTotalAcres(Me.MyLocation, False))
                        Me.txtAcerage.ToolTip = String.Format("Total including Acreage Only: {0}", IFM.VR.Common.Helpers.FARM.AcresOnlyHelper.CurrentTotalAcres(Me.MyLocation, False))
                    Else
                        Me.lblAdditionalAcres.Text = ""
                        Me.txtAcerage.ToolTip = ""
                    End If

                    If acreage.Section = "" Then
                        acreage.Section = "0"
                    End If
                    If acreage.Twp = "" Then
                        acreage.Twp = "0"
                    End If
                    If acreage.Range = "" Then
                        acreage.Range = "0"
                    End If

                    If MyLocation.Acreages.Any(Function(a) a.LocationAcreageTypeId = "4") Then
                        Dim blanketAcreage As QuickQuoteAcreage = MyLocation.Acreages.FirstOrDefault(Function(a) a.LocationAcreageTypeId = "4")
                        chkBlanketAcreage.Checked = True
                        txtTotalBlanketAcreage.Text = blanketAcreage.Acreage
                    End If
                    If Me.Quote.Locations(0).ProgramTypeId = "8" Then ' House # and House Name are optional on Farm ProgramType FL=8
                        Me.lblStreetName.Text = Me.lblStreetName.Text.Replace("*", "")
                        Me.lblStreetNum.Text = Me.lblStreetNum.Text.Replace("*", "")
                    End If

                    Me.txtSection.Text = acreage.Section
                    Me.txtTownship.Text = acreage.Twp
                    Me.txtRange.Text = acreage.Range
                    'Helpers.WebHelper_Personal.SetdropDownFromValue(ddTownshipName, acreage.TownshipCodeTypeId) ' set via javascript
                    hdnTownshipName.Value = acreage.TownshipCodeTypeId
                    Me.txtDescrption.Text = acreage.Description
                    Me.divAddressInfoText.Visible = True
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP

                    divAptNum.Visible = False
                    divNumAmusementAreas.Visible = True
                    If MyLocation.NumberOfAmusementAreas <> String.Empty Then
                        txtNumberOfAmusementAreas.Text = MyLocation.NumberOfAmusementAreas
                    Else
                        txtNumberOfAmusementAreas.Text = String.Empty
                    End If
                    divNumberOfPlaygrounds.Visible = True
                    If MyLocation.NumberOfPlaygrounds <> String.Empty Then
                        txtNumberOfPlaygrounds.Text = MyLocation.NumberOfPlaygrounds
                    Else
                        txtNumberOfPlaygrounds.Text = String.Empty
                    End If
                    divNumberOfSwimmingPools.Visible = True
                    If MyLocation.NumberOfPools <> String.Empty Then
                        txtNumberOfSwimmingPools.Text = MyLocation.NumberOfPools
                    Else
                        txtNumberOfSwimmingPools.Text = String.Empty
                    End If
                    ' Disabled description per BOP group instructions 6/16/17 MGB
                    'tdDescription.Visible = True
                    'txtDescrption.Text = MyLocation.Description
                    'Added 09/09/2021 for BOP Endorsements Task 63912 MLW
                    If IsQuoteReadOnly() Then
                        btnCopyAddress.Visible = False
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    divPOBox.Visible = True
                    btnCopyAddress.Visible = False
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    If NoOwnedLocationApplies() Then
                        ' NO OWNED LOCATION APPLIES - Do not display any address info & Disable input
                        ' Clear address info (except state)
                        txtStreetNum.Text = ""
                        txtStreetName.Text = ""
                        txtCityName.Text = ""
                        txtZipCode.Text = ""
                        txtGaragedCounty.Text = ""
                        txtAptNum.Text = ""
                        txtNumberOfEmployees.Text = ""
                        ' Disable fields
                        txtStreetNum.Attributes.Add("disabled", "true")
                        txtStreetName.Attributes.Add("disabled", "true")
                        txtCityName.Attributes.Add("disabled", "true")
                        ddStateAbbrev.Attributes.Add("disabled", "true")
                        txtZipCode.Attributes.Add("disabled", "true")
                        txtGaragedCounty.Attributes.Add("disabled", "true")
                        txtAptNum.Attributes.Add("disabled", "true")
                        txtNumberOfEmployees.Attributes.Add("disabled", "true")
                        lnkClearAddress.Attributes.Add("style", "visibility:hidden")
                        lnkSaveAddress.Attributes.Add("style", "visibility:hidden")
                        RaiseEvent HideHeaderButtons()
                    Else
                        ' NO OWNED LOCATION DOES NOT APPLY
                        ' Enable fields
                        txtStreetNum.Attributes.Remove("disabled")
                        txtStreetName.Attributes.Remove("disabled")
                        txtCityName.Attributes.Remove("disabled")
                        ddStateAbbrev.Attributes.Remove("disabled")
                        txtZipCode.Attributes.Remove("disabled")
                        txtGaragedCounty.Attributes.Remove("disabled")
                        txtAptNum.Attributes.Remove("disabled")
                        txtNumberOfEmployees.Attributes.Remove("disabled")
                        lnkClearAddress.Attributes.Add("style", "visibility:visible")
                        lnkSaveAddress.Attributes.Add("style", "visibility:visible")
                        RaiseEvent ShowHeaderButtons()
                    End If

                    ' Field level stuff
                    divAptNum.Visible = False
                    divNumberOfEmployees.Visible = IsFirstLocationForStatePart
                    If IsFirstLocationForStatePart Then
                        txtNumberOfEmployees.Text = Coalesce(Me.MyLocation?.Classifications.GetItemAtIndex(0)?.NumberOfEmployees, String.Empty)
                    End If

                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    If Not ProtectionClassesLoaded Then LoadProtectionClasses()  ' We don't want to keep loading the protection classes every time populate is called
                    Me.divFeetToHydrant.Visible = True
                    If MyLocation.FeetToFireHydrant <> "" Then txtFeetToHydrant.Text = MyLocation.FeetToFireHydrant
                    Me.divMilesToFireDept.Visible = True
                    If MyLocation.MilesToFireDepartment <> "0.00" Then txtMilesToFireDept.Text = MyLocation.MilesToFireDepartment
                    Me.divProtectionClass.Visible = True
                    SetFromValue(ddProtectionClass, MyLocation.ProtectionClassId)
                    Exit Select
                Case Else
                    Exit Select
            End Select

            showVeriskProtClassIfAvailable()

            ' Store the address info to hidden fields for comparison later MGB 10/24/16
            hdnStreetNum.Value = txtStreetNum.Text
            hdnStreetName.Value = txtStreetName.Text
            hdnAptNum.Value = txtAptNum.Text
            hdnCityName.Value = txtCityName.Text
            hdnStateAbbrev.Value = ddStateAbbrev.SelectedItem.Text
            hdnZipCode.Value = txtZipCode.Text

            ' Set the hdnPCCOrdered hidden field value so the page knows if a report has been ordered
            If MyLocation.IsNotNull Then
                If QQHelper.IsPositiveIntegerString(MyLocation.ProtectionClassSystemGeneratedId) Then
                    hdnPCCOrdered.Value = "Y"
                Else
                    hdnPCCOrdered.Value = "N"
                End If
            Else
                hdnPCCOrdered.Value = "N"
            End If
            Me.PopulateChildControls()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 10/18/2021 for BOP Endorsements Task 61660 MLW
        'If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto) Then 'Added 12/23/2020 for CAP Endorsements Task 52972 MLW
        If AllowValidateAndSave() Then
            Dim OriginalCity As String = Nothing
            Dim OriginalCounty As String = Nothing
            Dim AddrChanged As Boolean = AddressChanged()

            If MyLocation IsNot Nothing Then
                If MyLocation.Address Is Nothing Then MyLocation.Address = New QuickQuoteAddress

                'Address
                Me.MyLocation.Address.POBox = "" '9-26-2016 Matt A - just in case pobox is something it should never be
                Me.MyLocation.Address.HouseNum = Me.txtStreetNum.Text.Trim()
                Me.MyLocation.Address.StreetName = Me.txtStreetName.Text.ToUpper().Trim()
                Me.MyLocation.Address.ApartmentNumber = Me.txtAptNum.Text.ToUpper().Trim()

                Me.MyLocation.Address.Zip = Me.txtZipCode.Text.Trim()
                Me.MyLocation.Address.City = Me.txtCityName.Text.ToUpper().Trim()
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                    If ddStateAbbrev.SelectedItem IsNot Nothing AndAlso ddStateAbbrev.SelectedItem.Text <> "" Then
                        ' WCP - Don't set the state to blank if it's already been set on the address
                        MyLocation.Address.StateId = Me.ddStateAbbrev.SelectedValue
                    End If
                Else
                    If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        Me.MyLocation.Address.StateId = Me.ddStateAbbrev.SelectedValue
                    End If
                End If

                Me.MyLocation.Address.County = Me.txtGaragedCounty.Text.ToUpper().Trim()

                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        If Not NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                            MyLocation.FeetToFireHydrant = txtFeetToHydrant.Text
                            MyLocation.MilesToFireDepartment = txtMilesToFireDept.Text
                            MyLocation.ProtectionClassId = ddProtectionClass.SelectedValue
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        Me.MyLocation.Description = Me.txtDescrption.Text.Trim()
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        'Me.MyLocation.Description = Me.txtDescrption.Text.Trim()
                        If isVeriskProtectionClassReady Then
                            MyLocation.FeetToFireHydrant = ddlVeriskFeetToHydrant.SelectedValue
                        End If
                        If divNumAmusementAreas.Visible Then
                            If txtNumberOfAmusementAreas.Text <> String.Empty AndAlso IsNumeric(txtNumberOfAmusementAreas.Text) Then
                                MyLocation.NumberOfAmusementAreas = txtNumberOfAmusementAreas.Text
                            Else
                                MyLocation.NumberOfAmusementAreas = String.Empty
                            End If
                        End If
                        If divNumberOfPlaygrounds.Visible Then
                            If txtNumberOfPlaygrounds.Text <> String.Empty AndAlso IsNumeric(txtNumberOfPlaygrounds.Text) Then
                                MyLocation.NumberOfPlaygrounds = txtNumberOfPlaygrounds.Text
                            Else
                                MyLocation.NumberOfPlaygrounds = String.Empty
                            End If
                        End If
                        If divNumberOfSwimmingPools.Visible Then
                            If txtNumberOfSwimmingPools.Text <> String.Empty AndAlso IsNumeric(txtNumberOfSwimmingPools.Text) Then
                                MyLocation.NumberOfPools = txtNumberOfSwimmingPools.Text
                            Else
                                MyLocation.NumberOfPools = String.Empty
                            End If
                        End If
                    Case QuickQuoteObject.QuickQuoteLobType.Farm
                        ' do these in addition to the ones above
                        If MyLocation.Acreages Is Nothing Then
                            MyLocation.Acreages = New List(Of QuickQuoteAcreage)()
                        End If
                        If MyLocation.Acreages.Any() = False Then
                            MyLocation.Acreages.Add(New QuickQuoteAcreage())
                        End If
                        Dim acreage As QuickQuoteAcreage = MyLocation.Acreages(0)
                        acreage.Acreage = Me.txtAcerage.Text.Trim()

                        'If hiddenAcreageOnly.Value = "true" Then
                        '    acreage.LocationAcreageTypeId = "3"
                        '    MyLocation.AcreageOnly = True
                        'Else
                        If Me.MyLocationIndex = 0 Then
                            acreage.LocationAcreageTypeId = "1"
                        Else
                            acreage.LocationAcreageTypeId = "2"
                        End If



                        'MyLocation.AcreageOnly = False
                        'End If

                        acreage.Section = Me.txtSection.Text.Trim()
                        acreage.Twp = Me.txtTownship.Text.Trim()
                        acreage.Range = Me.txtRange.Text.Trim()
                        'acreage.TownshipCodeTypeId = ddTownshipName.SelectedValue ' uses  hidden field to avoid viewstate errors due to javascript modifications of the dom
                        acreage.TownshipCodeTypeId = hdnTownshipName.Value

                        acreage.County = Me.txtGaragedCounty.Text
                        acreage.StateId = Me.ddStateAbbrev.SelectedValue
                        If Me.txtDescrption.Text.Trim().Length > 255 Then
                            Me.txtDescrption.Text = Me.txtDescrption.Text.Trim().Substring(0, 255)
                        End If
                        acreage.Description = Me.txtDescrption.Text.Trim()

                        If MyLocation IsNot Nothing Then
                            If MyLocationIndex > 0 Then
                                MyLocation.PrimaryResidence = False
                            Else
                                MyLocation.PrimaryResidence = True
                            End If

                            Dim blanketAcreage As QuickQuoteAcreage = MyLocation.Acreages.FirstOrDefault(Function(a) a.LocationAcreageTypeId = "4")
                            If IFM.VR.Common.Helpers.FARM.FarmBlanketAcreageHelper.IsFarmBlanketAcreageAvailable(Quote) AndAlso chkBlanketAcreage.Checked AndAlso MyLocationIndex = 0 Then
                                If blanketAcreage Is Nothing Then
                                    blanketAcreage = New QuickQuoteAcreage()
                                    blanketAcreage.Acreage = Me.txtTotalBlanketAcreage.Text.Trim()
                                    blanketAcreage.LocationAcreageTypeId = "4"
                                    MyLocation.Acreages.Add(blanketAcreage)
                                Else

                                    blanketAcreage.Acreage = Me.txtTotalBlanketAcreage.Text.Trim()
                                    blanketAcreage.LocationAcreageTypeId = "4"

                                End If
                            Else
                                If blanketAcreage IsNot Nothing Then
                                    MyLocation.Acreages.Remove(blanketAcreage)
                                End If

                            End If
                        End If


                        ''Matt A - 7-22-2015
                        'If MyLocation.Acreages.Count > 1 Then
                        '    If Me.DefaultValidationType = Validation.ObjectValidation.ValidationItem.ValidationType.quote Or Me.DefaultValidationType = Validation.ObjectValidation.ValidationItem.ValidationType.quoteRate Then
                        '        ' you need to remove any incomplete acreage records because it won't rate and you can't fix it on the quote side
                        '        Dim inCompleteList As New List(Of QuickQuoteAcreage)
                        '        For Each a In MyLocation.Acreages.GetRange(1, MyLocation.Acreages.Count - 1) ''do not check the first for incomplete because you can change that on the quote side when needed
                        '            ' is it incomplete???
                        '            If String.IsNullOrWhiteSpace(a.Acreage) Or String.IsNullOrWhiteSpace(a.County) Or String.IsNullOrWhiteSpace(a.Description) Or String.IsNullOrWhiteSpace(a.Range) Or String.IsNullOrWhiteSpace(a.Section) Or String.IsNullOrWhiteSpace(a.TownshipCodeTypeId) Or String.IsNullOrWhiteSpace(a.Twp) Or String.IsNullOrWhiteSpace(a.LocationAcreageTypeId) Then
                        '                inCompleteList.Add(a)
                        '            End If
                        '        Next
                        '        If inCompleteList.Any() Then
                        '            Me.ValidationHelper.AddWarning("One or more incomplete Acreage Only records from the application were removed.")
                        '            For Each a In inCompleteList
                        '                MyLocation.Acreages.Remove(a)
                        '            Next
                        '        End If
                        '    End If
                        'End If

                        'If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                        RaiseEvent PopulateLocationHeader()
                    'End If
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        If Me.MyLocation.Address.StateId <> Me.GoverningStateQuote.StateId Then
                            Me.MyLocation.Address.StateId = Me.GoverningStateQuote.StateId ' validation looks at the dropdown value so it will take care of the message
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuoteLocation)
                        If Quote.Locations.Count = 0 Then
                            Dim newloc As New QuickQuote.CommonObjects.QuickQuoteLocation()
                            Quote.Locations.Add(newloc)
                        End If
                        If Quote.Locations(MyLocationIndex).Address Is Nothing Then Quote.Locations(MyLocationIndex).Address = New QuickQuoteAddress

                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation AndAlso NoOwnedLocationApplies() Then
                            ' NO OWNED LOCATION APPLIES
                            ' MGB 4/29/19
                            ' When 'No Owned Location' applies we need to save the location with all of the same address
                            ' info as Governing State Location 0, except for the state, which will stay the same
                            Dim GovAddr As QuickQuote.CommonObjects.QuickQuoteAddress = New QuickQuoteAddress()
                            If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.Locations.HasItemAtIndex(0) AndAlso GoverningStateQuote.Locations(0).Address IsNot Nothing Then
                                GovAddr = GoverningStateQuote.Locations(0).Address
                            End If

                            'Address
                            MyLocation.Address.POBox = "" '9-26-2016 Matt A - just in case pobox is something it should never be
                            MyLocation.Address.HouseNum = GovAddr.HouseNum
                            MyLocation.Address.StreetName = GovAddr.StreetName
                            MyLocation.Address.ApartmentNumber = GovAddr.ApartmentNumber

                            MyLocation.Address.Zip = GovAddr.Zip
                            MyLocation.Address.City = GovAddr.City
                            MyLocation.Address.StateId = ddStateAbbrev.SelectedValue
                            MyLocation.Address.County = GovAddr.County
                        Else
                            ' NO OWNED LOCATION DOES NOT APPLY
                            ' Save the location from the page controls
                            Quote.Locations(MyLocationIndex).Address.HouseNum = txtStreetNum.Text
                            Quote.Locations(MyLocationIndex).Address.StreetName = txtStreetName.Text
                            Quote.Locations(MyLocationIndex).Address.ApartmentNumber = txtAptNum.Text
                            Quote.Locations(MyLocationIndex).Address.Zip = txtZipCode.Text
                            Quote.Locations(MyLocationIndex).Address.City = txtCityName.Text
                            If ddStateAbbrev.SelectedItem IsNot Nothing AndAlso ddStateAbbrev.SelectedItem.Text <> "" Then  ' We don't want to flip the state if it's already been set on the quote
                                Quote.Locations(MyLocationIndex).Address.StateId = ddStateAbbrev.SelectedValue
                            End If
                            Quote.Locations(MyLocationIndex).Address.County = txtGaragedCounty.Text

                            ' Updated this classification code for multistate
                            If Me.IsFirstLocationForStatePart Then
                                If Me.MyLocation.Classifications Is Nothing Then
                                    Me.MyLocation.Classifications.AddNew()
                                End If
                                ' Bug 60840 - Number of Employees should only be saved to the first class code.  
                                '             The rest of the class codes' number of employees should be set to empty string. MGB 5 / 18 / 21.
                                Dim ndx As Integer = 0
                                For Each classification In Me.MyLocation.Classifications
                                    ndx += 1
                                    If ndx = 1 Then
                                        classification.NumberOfEmployees = If(txtNumberOfEmployees.Text <> "" AndAlso IsNumeric(txtNumberOfEmployees.Text), Me.txtNumberOfEmployees.Text, String.Empty)
                                    Else
                                        classification.NumberOfEmployees = String.Empty
                                    End If
                                Next
                            End If

                            ' Set the Location Name and Tax ID to the PolicyHolder values  MGB 11/7/17 Bug 23127
                            If Quote.Locations(MyLocationIndex).Name Is Nothing Then Quote.Locations(0).Name = New QuickQuoteName
                            Quote.Locations(MyLocationIndex).Name.CommercialName1 = Quote.Policyholder.Name.CommercialName1
                            Quote.Locations(MyLocationIndex).Name.DoingBusinessAsName = Quote.Policyholder.Name.DoingBusinessAsName
                            Quote.Locations(MyLocationIndex).Name.TaxTypeId = Quote.Policyholder.Name.TaxTypeId
                            Quote.Locations(MyLocationIndex).Name.TaxNumber = Quote.Policyholder.Name.TaxNumber
                        End If
                        Exit Select
                    Case Else
                        Exit Select
                End Select

                saveVeriskProtClassIfAvailable()
            Else
                ' LOCATION DOES NOT EXIST - CGL, CAP, WCP
                Select Case Me.Quote.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        'Me.MyLocation.Description = Me.txtDescrption.Text.Trim()
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        If Me.Quote.Locations Is Nothing Then Me.Quote.Locations = New List(Of QuickQuoteLocation)()
                        Dim newloc As New QuickQuote.CommonObjects.QuickQuoteLocation()
                        newloc.Address = New QuickQuoteAddress
                        newloc.Address.HouseNum = txtStreetNum.Text
                        newloc.Address.StreetName = txtStreetName.Text
                        newloc.Address.ApartmentNumber = txtAptNum.Text
                        newloc.Address.POBox = txtPOBox.Text
                        newloc.Address.Zip = txtZipCode.Text
                        newloc.Address.City = txtCityName.Text
                        newloc.Address.StateId = ddStateAbbrev.SelectedValue
                        newloc.Address.County = txtGaragedCounty.Text
                        Me.Quote.Locations.Add(newloc)
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        If Quote.Locations Is Nothing Then Quote.Locations = New List(Of QuickQuoteLocation)
                        If Quote.Locations.Count = 0 Then
                            Dim newloc As New QuickQuote.CommonObjects.QuickQuoteLocation()
                            Quote.Locations.Add(newloc)
                        End If
                        Quote.Locations(MyLocationIndex).Address = New QuickQuoteAddress
                        Quote.Locations(MyLocationIndex).Address.HouseNum = txtStreetNum.Text
                        Quote.Locations(MyLocationIndex).Address.StreetName = txtStreetName.Text
                        Quote.Locations(MyLocationIndex).Address.ApartmentNumber = txtAptNum.Text
                        Quote.Locations(MyLocationIndex).Address.Zip = txtZipCode.Text
                        Quote.Locations(MyLocationIndex).Address.City = txtCityName.Text
                        Quote.Locations(MyLocationIndex).Address.StateId = ddStateAbbrev.SelectedValue
                        Quote.Locations(MyLocationIndex).Address.County = txtGaragedCounty.Text
                        ' Which one?
                        Quote.Locations(MyLocationIndex).NumberOfEmployees = txtNumberOfEmployees.Text
                        'Quote.NumberOfEmployees = txtNumberOfEmployees.Text
                        Exit Select
                    Case Else
                        Exit Select
                End Select

                ' We need to trigger an event if the City or County changed so that we can update the Protection Class codes for the new City/County
                If OriginalCity IsNot Nothing AndAlso OriginalCounty IsNot Nothing Then
                    If OriginalCity.ToUpper <> MyLocation.Address.City.ToUpper OrElse OriginalCounty.ToUpper <> MyLocation.Address.County Then RaiseEvent ProtectionClassLookupNeeded()
                End If

            End If

            If AddrChanged Then
                RaiseEvent PropertyAddressChanged()
            End If
            Me.SaveChildControls()
        End If
        Return True
    End Function

    ''' <summary>
    ''' Compares the address on the object with the one on the page and if they're different returns true, otherwise returns false 
    ''' </summary>
    ''' <returns></returns>
    Private Function AddressChanged() As Boolean
        If Quote IsNot Nothing AndAlso MyLocation IsNot Nothing AndAlso MyLocation.Address IsNot Nothing Then
            Dim zip5 As String = MyLocation.Address.Zip
            If txtZipCode.Text.Length <= 5 Then
                If MyLocation.Address.Zip.Trim.Length > 5 Then zip5 = MyLocation.Address.Zip.Substring(0, 5)
            End If

            ' NEED TO FIX THIS FUNCTION SO IT GETS CALLED BEFORE SAVE

            If MyLocation.Address.HouseNum <> txtStreetNum.Text _
                OrElse MyLocation.Address.StreetName.ToLower <> txtStreetName.Text.ToLower _
                OrElse MyLocation.Address.City.ToLower <> txtCityName.Text.ToLower _
                OrElse MyLocation.Address.StateId <> ddStateAbbrev.SelectedValue _
                OrElse zip5 <> txtZipCode.Text Then
                Return True
            End If

        End If
        Return False
    End Function

    ''' <summary>
    ''' Checks to make sure the state entered is on the quote
    ''' Returns TRUE if so and FALSE if not
    ''' </summary>
    ''' <returns></returns>
    Private Function MultistateStateIsValid() As Boolean
        Dim StatesOnQuote As New List(Of String)
        Dim GovState As String = ""
        Dim found As Boolean = False

        ' Create the list of all states on the quote
        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            found = False
            For Each s As String In StatesOnQuote
                If s = sq.State Then
                    found = True
                    Exit For
                End If
            Next
            If Not found Then StatesOnQuote.Add(sq.State)
        Next

        ' Now check that the entered state is in the list of states on the quote
        For Each st As String In StatesOnQuote
            If ddStateAbbrev.SelectedItem.Text.ToUpper = st.ToUpper Then Return True
        Next

        Return False
    End Function


    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 10/18/2021 for BOP Endorsements Task 61660 MLW
        'If Not IsQuoteEndorsement() OrElse (IsQuoteEndorsement() AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialAuto) Then 'Added 12/23/2020 for CAP Endorsements Task 52972 MLW
        If AllowValidateAndSave() Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Property Address"
            If Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                Me.ValidationHelper.GroupName = String.Format("Location #{0} - Property Address", MyLocationIndex + 1)
            End If
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

            Dim invalidStateMsg = $"You have entered a location for {If(Me.MyLocation?.Address?.State IsNot Nothing, Me.MyLocation.Address.State, "an invalid state.")}. Please remove the location(s) or add additional non-governing state(s) on the policyholder page."

            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    If txtStreetNum.Text.Trim <> String.Empty And txtStreetName.Text.Trim <> String.Empty And txtPOBox.Text.Trim <> String.Empty Then
                        Me.ValidationHelper.AddError("You may enter a Street Address (Street # and Street Name) or P.O. Box, but not both.", txtStreetNum.ClientID)
                        Me.ValidationHelper.AddError("You may enter a Street Address (Street # and Street Name) or P.O. Box, but not both.", txtStreetName.ClientID)
                        Me.ValidationHelper.AddError("You may enter a Street Address (Street # and Street Name) or P.O. Box, but not both.", txtPOBox.ClientID)
                    Else
                        ' Street # & Name OR P.O. Box are required on commercial auto (this is the primary garaging location)
                        If txtStreetNum.Text.Trim = String.Empty And txtStreetName.Text.Trim = String.Empty And txtPOBox.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError("A Street Address (Street # and Street Name) or P.O. Box is required.", Nothing)
                        Else
                            If txtPOBox.Text.Trim = String.Empty Then
                                ' No PO Box so make sure we have a street number and name
                                If txtStreetNum.Text.Trim = String.Empty Then Me.ValidationHelper.AddError("Missing Street Number", txtStreetNum.ClientID)
                                If txtStreetName.Text.Trim = String.Empty Then Me.ValidationHelper.AddError("Missing Street Name", txtStreetName.ClientID)
                            Else
                                ' PO Box has a value so street number and/or name cannot
                                If txtStreetNum.Text.Trim <> String.Empty OrElse txtStreetName.Text.Trim <> String.Empty Then
                                    Me.ValidationHelper.AddError("You may enter a Street Address (Street # and Street Name) or P.O. Box, but not both.", txtStreetNum.ClientID)
                                    Me.ValidationHelper.AddError("You may enter a Street Address (Street # and Street Name) or P.O. Box, but not both.", txtStreetName.ClientID)
                                    Me.ValidationHelper.AddError("You may enter a Street Address (Street # and Street Name) or P.O. Box, but not both.", txtPOBox.ClientID)
                                End If
                            End If
                        End If
                    End If

                    'If txtStreetNum.Text.Trim = String.Empty Then
                    '    Me.ValidationHelper.AddError("Missing Street Number", txtStreetNum.ClientID)
                    'End If
                    'If txtStreetName.Text.Trim = String.Empty Then
                    '    Me.ValidationHelper.AddError("Missing Street Name", txtStreetName.ClientID)
                    'End If
                    If txtCityName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing City", txtCityName.ClientID)
                    End If
                    If txtZipCode.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Zipcode", txtZipCode.ClientID)
                    End If
                    If Me.ddStateAbbrev.SelectedValue <> Me.GoverningStateQuote.StateId Then
                        Me.ValidationHelper.AddError("State must be in governing state", Me.ddStateAbbrev.ClientID)
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    ' On Multistate we need to make sure the state entered is actually on the quote
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                        If Not MultistateStateIsValid() Then
                            Me.ValidationHelper.AddError("Invalid State", ddStateAbbrev.ClientID)  ' Per bug 30057 this is the correct message MGB 11/29/18
                            'Me.ValidationHelper.AddError(invalidStateMsg, ddStateAbbrev.ClientID)
                        End If
                    End If
                    If txtStreetNum.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Street Number", txtStreetNum.ClientID)
                    End If
                    If txtStreetName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Street Name", txtStreetName.ClientID)
                    End If
                    If txtCityName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing City", txtCityName.ClientID)
                    End If
                    If txtZipCode.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Zipcode", txtZipCode.ClientID)
                    End If
                    If txtNumberOfAmusementAreas.Text <> String.Empty Then
                        If Not IsNumeric(txtNumberOfAmusementAreas.Text) OrElse CInt(txtNumberOfAmusementAreas.Text) <= 0 Then
                            Me.ValidationHelper.AddError("Must be blank or a non-zero number", txtNumberOfAmusementAreas.ClientID)
                        End If
                    End If
                    If txtNumberOfPlaygrounds.Text <> String.Empty Then
                        If Not IsNumeric(txtNumberOfPlaygrounds.Text) OrElse CInt(txtNumberOfPlaygrounds.Text) <= 0 Then
                            Me.ValidationHelper.AddError("Must be blank or a non-zero number", txtNumberOfPlaygrounds.ClientID)
                        End If
                    End If
                    If txtNumberOfSwimmingPools.Text <> String.Empty Then
                        If Not IsNumeric(txtNumberOfSwimmingPools.Text) OrElse CInt(txtNumberOfSwimmingPools.Text) <= 0 Then
                            Me.ValidationHelper.AddError("Must be blank or a non-zero number", txtNumberOfSwimmingPools.ClientID)
                        End If
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    ' DO NOT VALIDATE IF 'NO OWNED LOCATION' APPLIES
                    If Not NoOwnedLocationApplies() Then
                        ' On Multistate we need to make sure the state entered is actually on the quote
                        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                            If Not MultistateStateIsValid() Then
                                Me.ValidationHelper.AddError("Invalid State", ddStateAbbrev.ClientID)  ' Per bug 30057 this is the correct message MGB 11/29/18
                                'Me.ValidationHelper.AddError(invalidStateMsg, ddStateAbbrev.ClientID)
                            End If
                        End If
                        If txtStreetNum.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError("Missing Street Number", txtStreetNum.ClientID)
                        End If
                        If txtStreetName.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError("Missing Street Name", txtStreetName.ClientID)
                        End If
                        If txtCityName.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError("Missing City", txtCityName.ClientID)
                        End If
                        If txtZipCode.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError("Missing Zipcode", txtZipCode.ClientID)
                        End If
                        If txtGaragedCounty.Text.Trim = String.Empty Then
                            Me.ValidationHelper.AddError("Missing County", txtGaragedCounty.ClientID)
                        End If
                        If IsFirstLocationForStatePart Then
                            If txtNumberOfEmployees.Text.Trim = String.Empty Then
                                Me.ValidationHelper.AddError("Missing Number of Employees", txtNumberOfEmployees.ClientID)
                            Else
                                If Not IsNumeric(txtNumberOfEmployees.Text) OrElse CInt(txtNumberOfEmployees.Text) <= 0 Then
                                    Me.ValidationHelper.AddError("Number of Employees is invalid", txtNumberOfEmployees.ClientID)
                                End If
                            End If
                        End If
                    End If

                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    ' On Multistate we need to make sure the state entered is actually on the quote
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                        If Not MultistateStateIsValid() Then
                            Me.ValidationHelper.AddError("Invalid State", ddStateAbbrev.ClientID)  ' Per bug 30057 this is the correct message MGB 11/29/18
                            'Me.ValidationHelper.AddError(invalidStateMsg, ddStateAbbrev.ClientID)
                        End If
                    End If
                    If txtStreetNum.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Street Number", txtStreetNum.ClientID)
                    End If
                    If txtStreetName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Street Name", txtStreetName.ClientID)
                    End If
                    If txtCityName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing City", txtCityName.ClientID)
                    End If
                    If txtZipCode.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Zipcode", txtZipCode.ClientID)
                    End If
                    If txtGaragedCounty.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing County", txtGaragedCounty.ClientID)
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty, QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    ' On Multistate we need to make sure the state entered is actually on the quote
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                        If Not MultistateStateIsValid() Then
                            Me.ValidationHelper.AddError("Invalid State", ddStateAbbrev.ClientID)  ' Per bug 30057 this is the correct message MGB 11/29/18
                            'Me.ValidationHelper.AddError(invalidStateMsg, ddStateAbbrev.ClientID)
                        End If
                    End If
                    If txtStreetNum.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Street Number", txtStreetNum.ClientID)
                    End If
                    If txtStreetName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Street Name", txtStreetName.ClientID)
                    End If
                    If txtCityName.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing City", txtCityName.ClientID)
                    End If
                    If txtZipCode.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing Zipcode", txtZipCode.ClientID)
                    End If
                    If txtGaragedCounty.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError("Missing County", txtGaragedCounty.ClientID)
                    End If
                    'Updated 8/10/2022 for bug 76302 MLW
                    If AllowValidation() AndAlso Not NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                        If ddProtectionClass.Items IsNot Nothing AndAlso ddProtectionClass.Items.Count > 0 Then
                            If ddProtectionClass.SelectedIndex <= 0 Then
                                Me.ValidationHelper.AddError("Missing Protection Class", ddProtectionClass.ClientID)
                            End If
                        End If
                    End If

                    If AllowValidation() AndAlso NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                        If ddlVeriskFeetToHydrant.Items IsNot Nothing AndAlso ddlVeriskFeetToHydrant.Items.Count > 0 Then
                            If ddlVeriskFeetToHydrant.SelectedIndex <= 0 Then
                                Me.ValidationHelper.AddError("Missing Feet To Hydrant", ddlVeriskFeetToHydrant.ClientID)
                            End If
                        End If
                    End If

                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Dim valList = LocationAddressValidator.ValidateHOMLocationAddress(Me.Quote, Me.MyLocationIndex, ResidenceExists, valArgs.ValidationType)  ' AddressValidator.AddressValidation(MyLocation.Address, valArgs.ValidationType, True)
                    If valList.Any() Then
                        For Each v In valList
                            Select Case v.FieldId
                                Case LocationAddressValidator.AddressStreetNumber
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                                Case LocationAddressValidator.AddressStreetName
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetName, v, accordList)
                                Case LocationAddressValidator.AddressPoBox
                        'will not happen
                        'Me.ValidationHelper.Val_BindValidationItemToControl(Me.txt, v, divAddressClientId, "0")
                                Case LocationAddressValidator.AddressZipCode
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtZipCode, v, accordList)
                                Case LocationAddressValidator.AddressCity
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtCityName, v, accordList)
                                Case LocationAddressValidator.AddressState
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                                Case LocationAddressValidator.AddressSatetNotIndiana
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddStateAbbrev, v, accordList)
                                Case LocationAddressValidator.AddressStreetAndPoBoxEmpty
                                    v.Message = "Missing Street Number and Name"
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                                Case LocationAddressValidator.AddressStreetAndPoBoxAreSet
                                    ' will not happen
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)
                                Case LocationAddressValidator.PolicyHolderCountyID
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtGaragedCounty, v, accordList)
                                Case LocationAddressValidator.AddressIsEmpty
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtStreetNum, v, accordList)

                    'add Farm item validation catchers

                                Case LocationAddressValidator.Acreage_AcreageAmount
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAcerage, v, accordList)
                    'Case LocationAddressValidator.AcreageType
                    '    Me.ValidationHelper.Val_BindValidationItemToControl(Me.chkAcreageOnly, v, divAddressClientId, "0")
                                Case LocationAddressValidator.AcreageSection
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtSection, v, accordList)
                                Case LocationAddressValidator.AcreageTwp
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtTownship, v, accordList)
                                Case LocationAddressValidator.AcreageRange
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtRange, v, accordList)
                                Case LocationAddressValidator.AcreageTownshipName
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddTownshipName, v, accordList)
                                Case LocationAddressValidator.AcreageDescription
                                    Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtDescrption, v, accordList)

                            End Select
                        Next
                    End If

                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    Dim txtAcerageInput As Integer
                    Dim txtBlanketAcerageInput As Integer
                    Dim hasTxtAcerage As Boolean = Integer.TryParse(txtAcerage.Text, txtAcerageInput)

                    If txtAcerageInput > 0 AndAlso chkBlanketAcreage.Checked Then
                        If Not Integer.TryParse(txtTotalBlanketAcreage.Text, txtBlanketAcerageInput) Then
                            Me.ValidationHelper.AddError("Missing Total Blanket Acreage.", txtTotalBlanketAcreage.ClientID)
                        ElseIf txtBlanketAcerageInput <= 1 Then
                            Me.ValidationHelper.AddError("Total Blanket Acreage must be greater than 1.", txtTotalBlanketAcreage.ClientID)
                        End If
                    End If
                    Exit Select
                Case Else
            End Select

            CheckVeriskProtClass()

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    'Added 8/10/2022 for task 76302 MLW
    Private Function AllowValidation() As Boolean
        Dim allowIt As Boolean = True
        If IsQuoteEndorsement() Then
            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage, QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    If IsPreexistingLocationOnEndorsement(MyLocationIndex) Then
                        allowIt = False
                    End If
            End Select
        End If
        Return allowIt
    End Function

    Private Function CheckVeriskProtClass()
        If AllowValidation() AndAlso NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
            If ddlVeriskFeetToHydrant.Items IsNot Nothing AndAlso ddlVeriskFeetToHydrant.Items.Count > 0 Then
                If ddlVeriskFeetToHydrant.SelectedIndex <= 0 Then
                    Me.ValidationHelper.AddError("Missing Feet To Hydrant", ddlVeriskFeetToHydrant.ClientID)
                End If
            End If
        End If
    End Function

    Public Sub UpdateProtectionClasses()
        LoadProtectionClasses()
    End Sub

    ''' <summary>
    ''' Called whenever there's a change to City, County, Zip, Miles to FD or Feet to Hydrant
    ''' </summary>
    Private Sub LoadProtectionClasses()
        Dim county As String = Nothing
        Dim city As String = Nothing
        Dim ds As DataSet = Nothing
        Dim FTH As String = Nothing
        Dim MTFD As String = Nothing
        Dim SelectedPCId As String = Nothing

        Try
            If ddProtectionClass.Items IsNot Nothing AndAlso ddProtectionClass.Items.Count > 0 And ddProtectionClass.SelectedIndex > 0 AndAlso ddProtectionClass.SelectedValue <> "" Then
                SelectedPCId = ddProtectionClass.SelectedValue
            End If

            'county = Quote.Locations(MyLocationIndex).Address.County
            'city = Quote.Locations(MyLocationIndex).Address.City
            county = txtGaragedCounty.Text
            city = txtCityName.Text
            FTH = txtFeetToHydrant.Text
            MTFD = txtMilesToFireDept.Text

            ' 1-30-2013 MGB Pull protection class by the entered city. 
            ' If no results by city, pull protection classes by county
            ' 10/18/2023 CAH - Do not pull from the DB if we don't have a parameter
            If Not String.IsNullOrWhiteSpace(city) Then
                ds = GetProtectionClasses("CITY", city, FTH, MTFD)
            End If

            If (ds Is Nothing OrElse ds.Tables.Count <= 0 OrElse ds.Tables(0).Rows.Count <= 0) AndAlso Not String.IsNullOrWhiteSpace(county) Then
                ds = GetProtectionClasses("COUNTY", county, FTH, MTFD)
            End If

            ' If the protection class list is still empty, just exit (use the default protection classes only)
            If ds Is Nothing OrElse ds.Tables.Count <= 0 OrElse ds.Tables(0).Rows.Count <= 0 Then
                ddProtectionClass.Items.Clear()
                Me.ddProtectionClass.Items.Add(New ListItem("", ""))
                LoadDefaultProtectionClassNumbers()
                Exit Sub
            End If

            ' Load the protection class list
            ddProtectionClass.Items.Clear()
            Me.ddProtectionClass.Items.Add(New ListItem("", ""))
            For Each dr As DataRow In ds.Tables(0).Rows
                Dim item As New ListItem()
                If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
                    item.Text = dr.Item("community").ToString() & " " & dr.Item("county").ToString()
                Else
                    item.Text = dr.Item("county").ToString()
                End If
                'item.Text = dr.Item("county").ToString()
                'item.Text = dr.Item("community").ToString()
                item.Value = dr.Item("protectionclass").ToString()
                item.Attributes.Add("title", dr.Item("footnote").ToString())
                Me.ddProtectionClass.Items.Add(item)
            Next
            ' Load the defaults at the END of the list BUG 964 MGB 5/1/2013
            LoadDefaultProtectionClassNumbers()

            If ddProtectionClass.Items.Count = 1 Then
                ' no county selected or something went wrong
                LoadDefaultProtectionClassNumbers()
            End If

            ' If there was a protection class selected and it's still in the reloaded list, select it
            'If SelectedPCId IsNot Nothing Then
            '    If ddProtectionClass.Items IsNot Nothing AndAlso ddProtectionClass.Items.Count > 0 Then
            '        For Each li As ListItem In ddProtectionClass.Items
            '            If li.Value = SelectedPCId Then
            '                li.Selected = True
            '                Exit For
            '            End If
            '        Next
            '    End If
            'End If

            ProtectionClassesLoaded = True

            Exit Sub
        Catch ex As Exception
            LoadDefaultProtectionClassNumbers()
        End Try
    End Sub

    Private Function GetProtectionClasses(ByVal CityOrCounty As String, ByVal CityOrCountyName As String, Optional ByVal FeetToHydrant As String = Nothing, Optional ByVal MilesToFireDept As String = Nothing) As DataSet
        Dim td As New DataTable()
        td.Columns.Add(New DataColumn("community"))
        td.Columns.Add(New DataColumn("county"))
        td.Columns.Add(New DataColumn("protectionclass"))
        td.Columns.Add(New DataColumn("footnote"))

        Try
            ' Set defaults MGB 8/21/17
            If MilesToFireDept Is Nothing OrElse MilesToFireDept.Trim = "" Then MilesToFireDept = "5"
            If FeetToHydrant Is Nothing OrElse FeetToHydrant.Trim = "" Then FeetToHydrant = "1000"

            Using conn As New SqlConnection(AppSettings("ConnDiamond"))
                Using cmd As New SqlCommand()
                    conn.Open()
                    cmd.Connection = conn
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandText = "assp_ProtectionClass_Search"
                    Select Case CityOrCounty.ToUpper()
                        Case "CITY"
                            cmd.Parameters.AddWithValue("@SearchType", 1)     ' 0 = County, 1 = Community
                            Exit Select
                        Case "COUNTY"
                            cmd.Parameters.AddWithValue("@SearchType", 0)     ' 0 = County, 1 = Community
                            Exit Select
                        Case Else
                            Return Nothing
                    End Select
                    cmd.Parameters.AddWithValue("@SearchText", CityOrCountyName)
                    cmd.Parameters.AddWithValue("@stateID", Me.ddStateAbbrev.SelectedValue.TryToGetInt32())
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            While reader.Read()
                                Dim township As String = reader.GetString(1)
                                Dim county As String = reader.GetString(2)
                                Dim returnedProtectionClass As String = reader.GetString(3)
                                Dim footnote As String = reader.GetString(5)

                                If returnedProtectionClass.Contains("*") Then
                                    Dim newRow As DataRow = td.NewRow()
                                    'newRow("community") = township + "(10)"
                                    newRow("community") = township
                                    newRow("county") = county + "(10)"
                                    newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber("10")
                                    newRow("footnote") = footnote
                                    td.Rows.Add(newRow)
                                Else
                                    If returnedProtectionClass.Contains("/") Then
                                        ' is split protection class
                                        Dim LeftVal As String = returnedProtectionClass.Split(CChar("/"))(0)
                                        Dim RightVal As String = returnedProtectionClass.Split(CChar("/"))(1)
                                        Dim newRow As DataRow = td.NewRow()
                                        'see if miles is < 5 but > 0 and feet is < 1000 but > 0
                                        If MilesToFireDept <= 5 And MilesToFireDept > 0 And FeetToHydrant <= 1000 And FeetToHydrant > 0 Then
                                            ' use lower val
                                            'newRow("community") = String.Format("{0} ({1})", township, LeftVal.PadLeft(2, CChar("0")))
                                            newRow("community") = String.Format("{0}", township)
                                            newRow("county") = String.Format("{0} ({1})", county, LeftVal.PadLeft(2, CChar("0")))
                                            newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber(LeftVal.PadLeft(2, CChar("0")))
                                        Else
                                            ' maybe miles < 5 but > 0  and Hydrant > 1000 or it doesn't have Hydrant at all
                                            If MilesToFireDept <= 5 And MilesToFireDept > 0 Then
                                                'use higher
                                                'newRow("community") = String.Format("{0} ({1})", township, RightVal.PadLeft(2, CChar("0")))
                                                newRow("community") = String.Format("{0}", township)
                                                newRow("county") = String.Format("{0} ({1})", county, RightVal.PadLeft(2, CChar("0")))
                                                newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber(RightVal.PadLeft(2, CChar("0")))
                                            Else
                                                ' miles is > 5 so it must return a 10 regardless of hydrant
                                                'newRow("community") = String.Format("{0} ({1})", township, "10")
                                                newRow("community") = String.Format("{0}", township)
                                                newRow("county") = String.Format("{0} ({1})", county, "10")
                                                newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber("10")
                                            End If

                                        End If
                                        newRow("footnote") = footnote
                                        td.Rows.Add(newRow)
                                    Else
                                        ' single selection hydrant and miles make no difference
                                        Dim newRow As DataRow = td.NewRow()
                                        'newRow("community") = String.Format("{0} ({1})", township, returnedProtectionClass.PadLeft(2, CChar("0")))
                                        newRow("community") = String.Format("{0}", township)
                                        newRow("county") = String.Format("{0} ({1})", county, returnedProtectionClass.PadLeft(2, CChar("0")))
                                        newRow("protectionclass") = GetProtectionClassIdFromProtectionClassNumber(returnedProtectionClass.PadLeft(2, CChar("0")))
                                        newRow("footnote") = footnote
                                        td.Rows.Add(newRow)
                                    End If
                                End If
                            End While
                            Dim ds As New DataSet("ds1")
                            ds.Tables.Add(td)
                            Return ds
                        End If
                    End Using
                End Using
            End Using
            Return Nothing
        Catch ex As Exception
#If DEBUG Then
            Debugger.Break()
#End If
            Return Nothing
        End Try
    End Function

    Private Function GetProtectionClassIdFromProtectionClassNumber(ByVal num As String) As String

        Select Case num.ToLower()
            Case "01"
                Return "12"
            Case "02"
                Return "13"
            Case "03"
                Return "14"
            Case "04"
                Return "15"
            Case "05"
                Return "16"
            Case "06"
                Return "17"
            Case "07"
                Return "18"
            Case "08"
                Return "19"
            Case "8b"
                Return "20"
            Case "09"
                Return "21"
            Case "10"
                Return "22"
            Case "1X"
                Return "23"
            Case "2X"
                Return "24"
            Case "3X"
                Return "25"
            Case "4X"
                Return "26"
            Case "5X"
                Return "27"
            Case "6X"
                Return "28"
            Case "7X"
                Return "29"
            Case "8X"
                Return "30"
            Case "1Y"
                Return "31"
            Case "2Y"
                Return "32"
            Case "3Y"
                Return "33"
            Case "4Y"
                Return "34"
            Case "5Y"
                Return "35"
            Case "6Y"
                Return "36"
            Case "7Y"
                Return "37"
            Case "8Y"
                Return "38"
            Case "10W"
                Return "39"
            Case Else
                Return ""
        End Select



    End Function

    Public Sub LoadDefaultProtectionClassNumbers()

        If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
            Me.ddProtectionClass.Items.Add(New ListItem("01", "12"))
            Me.ddProtectionClass.Items.Add(New ListItem("02", "13"))
            Me.ddProtectionClass.Items.Add(New ListItem("03", "14"))
            Me.ddProtectionClass.Items.Add(New ListItem("04", "15"))
            Me.ddProtectionClass.Items.Add(New ListItem("05", "16"))
            Me.ddProtectionClass.Items.Add(New ListItem("06", "17"))
            Me.ddProtectionClass.Items.Add(New ListItem("07", "18"))
            Me.ddProtectionClass.Items.Add(New ListItem("08", "19"))
            Me.ddProtectionClass.Items.Add(New ListItem("8B", "20"))
            Me.ddProtectionClass.Items.Add(New ListItem("09", "21"))
            Me.ddProtectionClass.Items.Add(New ListItem("10", "22"))

            Me.ddProtectionClass.Items.Add(New ListItem("1X", "23"))
            Me.ddProtectionClass.Items.Add(New ListItem("2X", "24"))
            Me.ddProtectionClass.Items.Add(New ListItem("3X", "25"))
            Me.ddProtectionClass.Items.Add(New ListItem("4X", "26"))
            Me.ddProtectionClass.Items.Add(New ListItem("5X", "27"))
            Me.ddProtectionClass.Items.Add(New ListItem("6X", "28"))
            Me.ddProtectionClass.Items.Add(New ListItem("7X", "29"))
            Me.ddProtectionClass.Items.Add(New ListItem("8X", "30"))
            Me.ddProtectionClass.Items.Add(New ListItem("1Y", "31"))
            Me.ddProtectionClass.Items.Add(New ListItem("2Y", "32"))
            Me.ddProtectionClass.Items.Add(New ListItem("3Y", "33"))
            Me.ddProtectionClass.Items.Add(New ListItem("4Y", "34"))
            Me.ddProtectionClass.Items.Add(New ListItem("5Y", "35"))
            Me.ddProtectionClass.Items.Add(New ListItem("6Y", "36"))
            Me.ddProtectionClass.Items.Add(New ListItem("7Y", "37"))
            Me.ddProtectionClass.Items.Add(New ListItem("8Y", "38"))
            Me.ddProtectionClass.Items.Add(New ListItem("10W", "39"))
        Else
            Me.ddProtectionClass.Items.Add(New ListItem("01", "12"))
            Me.ddProtectionClass.Items.Add(New ListItem("02", "13"))
            Me.ddProtectionClass.Items.Add(New ListItem("03", "14"))
            Me.ddProtectionClass.Items.Add(New ListItem("04", "15"))
            Me.ddProtectionClass.Items.Add(New ListItem("05", "16"))
            Me.ddProtectionClass.Items.Add(New ListItem("06", "17"))
            Me.ddProtectionClass.Items.Add(New ListItem("07", "18"))
            Me.ddProtectionClass.Items.Add(New ListItem("08", "19"))
            Me.ddProtectionClass.Items.Add(New ListItem("8B", "20"))
            Me.ddProtectionClass.Items.Add(New ListItem("09", "21"))
            Me.ddProtectionClass.Items.Add(New ListItem("10", "22"))
        End If

        Me.ddProtectionClass.ToolTip = "All Protection Classes"
        ProtectionClassesLoaded = True
    End Sub
    Protected Sub lnkSaveAddress_Click(sender As Object, e As EventArgs) Handles lnkSaveAddress.Click
        Session("valuationValue") = "False"
        Me.Save_FireSaveEvent(True)
    End Sub

    Protected Sub lnkClearAddress_Click(sender As Object, e As EventArgs) Handles lnkClearAddress.Click
        Session("valuationValue") = "False"
        ClearControl()
        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
        RaiseEvent PropertyCleared()
    End Sub

    Public Overrides Sub ClearControl()
        MyBase.ClearControl()
        'Address
        Me.txtStreetNum.Text = ""
        Me.txtStreetName.Text = ""
        Me.txtAptNum.Text = ""
        If Me.ddStateAbbrev.Enabled Then
            Me.ddStateAbbrev.SelectedIndex = -1
        End If

        Me.txtZipCode.Text = ""
        Me.txtCityName.Text = ""
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddStateAbbrev, "16")
        Me.txtGaragedCounty.Text = ""

        Me.txtAcerage.Text = ""
        'Me.chkAcreageOnly.Checked = False
        Me.txtSection.Text = "0"
        Me.txtRange.Text = "0"
        Me.txtTownship.Text = "0"
        hdnTownshipName.Value = ""
        Me.txtDescrption.Text = ""

        If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(Quote) Then
            Me.txtMilesToFireDept.Text = "5"
            Me.txtFeetToHydrant.Text = "1000"
        Else
            Me.txtMilesToFireDept.Text = ""
            Me.txtFeetToHydrant.Text = ""
        End If

        Me.ddProtectionClass.SelectedIndex = -1

        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
            RaiseEvent PopulateLocationHeader()
        End If
    End Sub

    Protected Sub btnCopyAddress_Click(sender As Object, e As EventArgs) Handles btnCopyAddress.Click
        Me.txtStreetNum.Text = Me.Quote.Policyholder.Address.HouseNum
        Me.txtStreetName.Text = Me.Quote.Policyholder.Address.StreetName
        Me.txtCityName.Text = Me.Quote.Policyholder.Address.City
        Me.txtAptNum.Text = Me.Quote.Policyholder.Address.ApartmentNumber
        Me.ddStateAbbrev.SelectedValue = Me.Quote.Policyholder.Address.StateId
        Me.txtZipCode.Text = Me.Quote.Policyholder.Address.Zip
        Me.txtGaragedCounty.Text = Me.Quote.Policyholder.Address.County

        Select Case Me.Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                RaiseEvent PopulateLocationHeader()
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP,
                QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                'add to keep Address from being recognized as pre-existing.
                If IsQuoteEndorsement() Then
                    Me.txtStreetNum.Text = String.Empty
                End If
        End Select

        ' Store the address info to hidden fields for comparison later MGB 10/24/16
        hdnStreetNum.Value = txtStreetNum.Text
        hdnStreetName.Value = txtStreetName.Text
        hdnAptNum.Value = txtAptNum.Text
        hdnCityName.Value = txtCityName.Text
        hdnStateAbbrev.Value = ddStateAbbrev.SelectedItem.Text
        hdnZipCode.Value = txtZipCode.Text

        ' Set the hdnPCCOrdered hidden field value so the page knows if a report has been ordered
        If MyLocation.IsNotNull Then
            If QQHelper.IsPositiveIntegerString(MyLocation.ProtectionClassSystemGeneratedId) Then
                hdnPCCOrdered.Value = "Y"
            Else
                hdnPCCOrdered.Value = "N"
            End If
        Else
            hdnPCCOrdered.Value = "N"
        End If

        If Quote IsNot Nothing AndAlso (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) Then
            LoadProtectionClasses()
        End If

        Me.Save_FireSaveEvent(False)
    End Sub

    Private Sub txtFeetToHydrant_TextChanged(sender As Object, e As EventArgs) Handles txtFeetToHydrant.TextChanged, txtMilesToFireDept.TextChanged
        ' We need to reload the protection classes whenevr feet to hydrant or miles to fire dept change
        LoadProtectionClasses()
    End Sub

    Private Sub ProtectionClassFieldChanged(sender As Object, e As EventArgs) Handles txtCityName.TextChanged, txtGaragedCounty.TextChanged, txtZipCode.TextChanged
        '' HOM/DFR have script to handle PC code change (see addscriptwhenrendered) because those LOB's set the protection class fields in another control, so don't execute if that's the LOB
        If Quote IsNot Nothing AndAlso (Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse Quote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal) _
            Then Exit Sub

        LoadProtectionClasses()
    End Sub

    'Added 10/18/2021 for BOP Endorsements Task 61660 MLW
    Private Function AllowValidateAndSave() As Boolean
        If IsQuoteEndorsement() Then
            Select Case Quote.LobType
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    Return False
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    If TypeOfEndorsement() = EndorsementStructures.EndorsementTypeString.BOP_AddDeleteLocationLienholder Then
                        Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
                        If endorsementsPreexistHelper.IsPreexistingLocation(MyLocationIndex) Then
                            Return False
                        Else
                            Return True
                        End If
                    Else
                        Return False
                    End If
                Case Else
                    Return True
            End Select
        Else
            Return True
        End If
        'If IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then 'Added 12/23/2020 for CAP Endorsements Task 52972 MLW
        '    Return False
        'ElseIf IsQuoteEndorsement() AndAlso Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP AndAlso TypeOfEndorsement() = EndorsementStructures.EndorsementTypeString.BOP_AddDeleteLocationLienholder Then
        '    Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(Quote)
        '    If endorsementsPreexistHelper.IsPreexistingLocation(MyLocationIndex) Then
        '        Return False
        '    Else
        '        Return True
        '    End If
        'Else
        '    Return True
        'End If
    End Function

End Class