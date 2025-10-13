Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.HOM
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM

Public Class ctlPropertyAdditionalQuestions
    Inherits VRControlBase

    Public Event PolicyholderReloadNeeded()

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
    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() AndAlso Me.Quote.Locations.Count > MyLocationIndex Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    'added 11/22/17 for HOM Upgrade - MLW
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
            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.AdditionalQuestionsDiv.ClientID
        If Not IsPostBack Then
            If Me.Quote.LobType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                'Updated 12/15/17 for HOM Upgrade MLW - Updated 6/7/18 for Bug 26865 (no quote location when coming from PPA bridge before form type selected, yet somehow this page is loaded) MLW
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count > 0 AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                    Dim removePDTheft As ListItem = ddBurglarAlarmType.Items.FindByText("POLICE DEPARTMENT THEFT ALARM")
                    If removePDTheft IsNot Nothing Then
                        Me.ddBurglarAlarmType.Items.Remove(removePDTheft)
                    End If
                Else
                    Me.ddBurglarAlarmType.Items.Remove("Police Department Theft Alarm".ToUpper())
                End If
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        ' Additional Questions
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, HiddenField5, "0")
        Me.VRScript.CreateConfirmDialog(Me.lnkClearAdditionalQuestions.ClientID, "Clear Additional Questions?")
        Me.VRScript.StopEventPropagation(Me.lnkSaveAdditionalQuestions.ClientID)

        'First Written
        Me.VRScript.AddScriptLine("$(""#" + Me.txtFirstWrittenDate.ClientID + """).datepicker({changeMonth: true,changeYear: true,maxDate: ""-1D"",showButtonPanel: true});")
        Me.VRScript.AddScriptLine("if($('#" + chkFirstWrittenDate.ClientID + "').is(':checked')) {$('#divFirstWrittenDate').show();} ")

        Me.VRScript.CreateTextboxWaterMark(Me.txtFirstWrittenDate, "MM/DD/YYYY")
        Me.VRScript.CreateTextBoxFormatter(Me.txtFirstWrittenDate, ctlPageStartupScript.FormatterType.DateFormat, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.AddScriptLine("if($('#" + chkFirstWrittenDate.ClientID + "').prop('checked')){$(this).next('div').show();}else{$(this).next('div').hide();}")

        If Farm_General.hasAdditionalQuestionsForFarmItemNumberOfUnitsUpdate(Quote) Then
            'Trampoline
            If TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote) Then
                Dim scriptTrampExclusion As String = "ToggleNumOfUnits(""" + chkTrampoline.ClientID + """, """ + divNumOfTrampolines.ClientID + """, """ + txtNumOfTrampolines.ClientID + """);"
                chkTrampoline.Attributes.Add("onclick", scriptTrampExclusion)
                Me.VRScript.CreateTextBoxFormatter(Me.txtNumOfTrampolines, ctlPageStartupScript.FormatterType.NumericNoCommas, ctlPageStartupScript.JsEventType.onkeyup)
            End If

            'SwimmingPools
            If SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote) Then
                Dim scriptPoolExclusion As String = "ToggleNumOfUnits(""" + chkSwimmingPool.ClientID + """, """ + divNumOfSwimmingPools.ClientID + """, """ + txtNumOfSwimmingPools.ClientID + """);"
                chkSwimmingPool.Attributes.Add("onclick", scriptPoolExclusion)
                Me.VRScript.CreateTextBoxFormatter(Me.txtNumOfSwimmingPools, ctlPageStartupScript.FormatterType.NumericNoCommas, ctlPageStartupScript.JsEventType.onkeyup)
            End If

            'Added 6/14/2022 for task 72947 MLW
            'Woodburning Stove
            If WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote) Then
                Dim scriptWBSExclusion As String = "ToggleNumOfUnits(""" + chkWoodStove.ClientID + """, """ + divNumOfWoodburningStoves.ClientID + """, """ + txtNumOfWoodburningStoves.ClientID + """);"
                chkWoodStove.Attributes.Add("onclick", scriptWBSExclusion)
            End If
        End If

        'This function is in the HOM js file, do not create the binding if not a HOM quote
        If Quote IsNot Nothing AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
            Me.VRScript.CreateJSBinding(chkWoodStove.ClientID, ctlPageStartupScript.JsEventType.onchange, "HandleWoodburningStoveClicks('" & chkWoodStove.ClientID & "','" & divWoodburningInfoSection.ClientID & "');")
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        divWoodburningInfoSection.Attributes.Add("style", "display:none")

        Select Case Me.Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                'Additional Questions
                Me.chkHasAutoPolicy.Checked = Me.MyLocation.MultiPolicyDiscount ' there is logic to determine this under InsuredList->Save()

                'Updated 12/5/17 for HOM Upgrade MLW
                If Me.MyLocation.FormTypeId = "6" Or Me.MyLocation.FormTypeId = "7" Then ' do not apply to mobile homes
                    Me.chkAnyChildren.Checked = True
                    Me.trQuestion2.Visible = False
                Else
                    'If (Me.MyLocation.FormTypeId = "22" Or Me.MyLocation.FormTypeId = "25") And Me.MyLocation.StructureTypeId = "2" Then
                    '    'Updated 6/29/18 for HOM2011 Upgrade post go-live changes MLW - now available for mobile homes
                    '    'Me.chkAnyChildren.Checked = True
                    '    'Me.trQuestion2.Visible = False
                    '    If Me.MyLocation.SelectMarketCredit = True Then
                    '        Me.chkAnyChildren.Checked = True
                    '    Else
                    '        Me.chkAnyChildren.Checked = False
                    '    End If

                    '    Me.trQuestion2.Visible = True
                    'Else

                    Me.trQuestion2.Visible = True
                    Me.chkAnyChildren.Checked = Not Me.MyLocation.SelectMarketCredit
                    'End If
                End If
                'If Me.MyLocation.FormTypeId <> "6" And Me.MyLocation.FormTypeId <> "7" Then ' do not apply to mobile homes
                '    Me.trQuestion2.Visible = True
                '    Me.chkAnyChildren.Checked = Not Me.MyLocation.SelectMarketCredit
                'Else
                '    Me.chkAnyChildren.Checked = True
                '    Me.trQuestion2.Visible = False
                'End If
                Me.chkSmokeAlarms.Checked = Me.MyLocation.FireSmokeAlarm_SmokeAlarm
                If Me.MyLocation.FireSmokeAlarm_LocalAlarmSystem Then
                    Me.ddFireAlarmType.SetFromValue("2")
                End If
                If Me.MyLocation.FireSmokeAlarm_CentralStationAlarmSystem Then
                    Me.ddFireAlarmType.SetFromValue("3")
                End If

                If Me.MyLocation.BurglarAlarm_LocalAlarmSystem Then
                    Me.ddBurglarAlarmType.SetFromValue("3")
                End If
                If Me.MyLocation.BurglarAlarm_CentralStationAlarmSystem Then
                    Me.ddBurglarAlarmType.SetFromValue("1")
                End If

                'Updated 12/15/17 sprinkler for HOM Upgrade MLW
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                    Me.ddSprinklerType.SetFromValue("")
                    Me.trQuestion6.Visible = False
                    Me.chkSprinkler.Checked = Me.MyLocation.SprinklerSystem
                Else
                    Me.chkSprinkler.Checked = False 'added 12/18/17 for HOM Upgrade MLW
                    Me.trQuestion6b.Visible = False 'added 12/18/17 for HOM Upgrade MLW

                    If Me.MyLocation.SprinklerSystem_AllExcept Then
                        Me.ddSprinklerType.SetFromValue("1")
                    End If
                    If Me.MyLocation.SprinklerSystem_AllIncluding Then
                        Me.ddSprinklerType.SetFromValue("2")
                    End If
                End If

                'Updated 12/5/17 and 12/15/17 trampoline for HOM Upgrade MLW
                If Me.MyLocation.FormTypeId = "6" Or Me.MyLocation.FormTypeId = "7" Then ' do not apply to mobile homes
                    Me.chkTrampoline.Checked = False
                    Me.chkSwimmingPool.Checked = False
                    Me.chkWoodStove.Checked = False
                    Me.trQuestion7.Visible = False
                    Me.trQuestion8.Visible = False
                    Me.trQuestion9.Visible = False
                Else
                    'Updated 12/15/17 trampoline for HOM Upgrade MLW
                    If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "25") AndAlso Me.MyLocation.StructureTypeId = "2" Then
                        Me.chkTrampoline.Checked = Me.MyLocation.TrampolineSurcharge
                        Me.chkSwimmingPool.Checked = False
                        Me.chkWoodStove.Checked = False
                        Me.trQuestion8.Visible = False
                        Me.trQuestion9.Visible = False
                    Else
                        Me.chkTrampoline.Checked = Me.MyLocation.TrampolineSurcharge
                        Me.chkSwimmingPool.Checked = Me.MyLocation.SwimmingPoolHotTubSurcharge

                        ' Wood stove logic 11/30/21
                        Me.chkWoodStove.Checked = Me.MyLocation.WoodOrFuelBurningApplianceSurcharge
                        Dim PHEmail As String = GetPolicyHolderEmailForWoodBurningStove("")
                        ' Fill in the wood burning email regardless of the checkbox state.
                        ' If they check it later we want the email there.
                        Me.txtWoodBurningEmail.Text = PHEmail

                        ' This code will store the values of KeyWasPresent and KeyWasSet to hidden fields,
                        ' so that we can minimize database lookups.  We'll check the hidden fields in
                        ' QuoteHadWoodburningSurchargeOnPreviousImage, and if the values are in the hidden fields, will
                        ' use those values instead of doing another lookup.
                        Dim KeyWasPresent As Boolean = False
                        Dim KeyWasSet As Boolean = False
                        Dim err As String = Nothing
                        Dim WoodBurningIsNew As Boolean = WoodburningIsNewToEndorsement(KeyWasPresent, KeyWasSet)
                        If Me.IsQuoteEndorsement = False OrElse (Me.IsQuoteEndorsement = True AndAlso WoodBurningIsNew) Then
                            ' New business, or wood burning was added on the current endorsement transaction
                            If PHEmail = "" OrElse IFM.Common.InputValidation.CommonValidations.IsValidEmail(PHEmail) = False Then
                                ' If the email is bad allow them to edit it here
                                Me.txtWoodBurningEmail.ReadOnly = False
                                Me.txtWoodBurningEmail.BackColor = Drawing.Color.White
                            Else
                                ' if the email is valid, you have to go to the policyholders
                                ' page to change the email
                                Me.txtWoodBurningEmail.ReadOnly = True
                                Me.txtWoodBurningEmail.BackColor = Drawing.Color.LightGray
                            End If
                        ElseIf Me.IsQuoteEndorsement AndAlso WoodBurningIsNew = False Then
                            ' This is an endorsement but wood burning is not new.
                            ' Disable the woodburning email textbox.
                            ' You have to go to the policyholders page to change the email.
                            Me.txtWoodBurningEmail.ReadOnly = True
                            Me.txtWoodBurningEmail.BackColor = Drawing.Color.LightGray
                        End If

                        If chkWoodStove.Checked Then
                            divWoodburningInfoSection.Attributes.Add("style", "display:''")
                        End If
                    End If
                End If
                'If Me.MyLocation.FormTypeId <> "6" And Me.MyLocation.FormTypeId <> "7" Then ' do not apply to mobile homes
                '    Me.chkTrampoline.Checked = Me.MyLocation.TrampolineSurcharge
                '    Me.chkSwimmingPool.Checked = Me.MyLocation.SwimmingPoolHotTubSurcharge
                '    Me.chkWoodStove.Checked = Me.MyLocation.WoodOrFuelBurningApplianceSurcharge
                'Else
                '    Me.chkTrampoline.Checked = False
                '    Me.chkSwimmingPool.Checked = False
                '    Me.chkWoodStove.Checked = False
                '    Me.trQuestion7.Visible = False
                '    Me.trQuestion8.Visible = False
                '    Me.trQuestion9.Visible = False
                'End If

                Me.txtFirstWrittenDate.Text = Me.Quote.FirstWrittenDate
                Me.chkFirstWrittenDate.Checked = String.IsNullOrWhiteSpace(Me.txtFirstWrittenDate.Text) = False And Me.txtFirstWrittenDate.Text <> Me.Quote.EffectiveDate

                'Updated 03/24/2020 for Home Endorsements task 45262 MLW
                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                    'T40490 CAH
                    If IFM.VR.Common.Helpers.HOM.HOM_General.IsHomMortgageFreeEligable(Me.Quote) Then
                        Me.trQuestion11.Visible = True
                        If Me.Quote?.Locations(0)?.AdditionalInterests?.Count > 0 Then
                            Me.chkMortgageContractSeller.Checked = True
                        Else
                            Me.chkMortgageContractSeller.Checked = False
                        End If
                    End If
                End If

            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                'Updated 8/23/18 for multi state MLW
                If SubQuoteFirst IsNot Nothing Then
                    If Me.SubQuoteFirst.ProgramTypeId = "6" AndAlso Me.MyLocationIndex = 0 Then
                        'Additional Questions
                        Me.chkSmokeAlarms.Attributes.Add("autofocus", "")
                        Me.trQuestion1.Visible = False
                        Me.trQuestion10.Visible = False
                        Me.trQuestion2.Visible = False
                        If SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote) = False Then
                            Me.trQuestion8.Visible = False
                        End If
                        Me.chkSmokeAlarms.Text = Me.chkSmokeAlarms.Text.Replace("smoke alarm", "fire department alarm")
                        Me.chkSmokeAlarms.Checked = Me.MyLocation.FireDepartmentAlarm
                        If Me.MyLocation.FireSmokeAlarm_LocalAlarmSystem Then
                            Me.ddFireAlarmType.SetFromValue("2")
                        End If
                        If Me.MyLocation.FireSmokeAlarm_CentralAlarmSystem Then
                            Me.ddFireAlarmType.SetFromValue("3")
                        End If

                        If Me.MyLocation.BurglarAlarm_LocalAlarmSystem Then
                            Me.ddBurglarAlarmType.SetFromValue("3")
                        End If
                        If Me.MyLocation.BurglarAlarm_CentralAlarmSystem Then
                            Me.ddBurglarAlarmType.SetFromValue("1")
                        End If
                        If Me.MyLocation.PoliceDepartmentTheftAlarm Then
                            Me.ddBurglarAlarmType.SetFromValue("2")
                        End If

                        'Added 12/18/17 for HOM Upgrade MLW
                        Me.chkSprinkler.Checked = False
                        Me.trQuestion6b.Visible = False

                        If Me.MyLocation.SprinklerSystem_AllExcept Then
                            Me.ddSprinklerType.SetFromValue("1")
                        End If
                        If Me.MyLocation.SprinklerSystem_AllIncluding Then
                            Me.ddSprinklerType.SetFromValue("2")
                        End If

                        Me.chkSwimmingPool.Checked = Me.MyLocation.SwimmingPoolHotTubSurcharge
                        divNumOfSwimmingPools.Attributes.Add("style", "display: none")
                        txtNumOfSwimmingPools.Text = 0
                        UpdateNumberOfUnits(SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote), chkSwimmingPool, divNumOfSwimmingPools, txtNumOfSwimmingPools, Me.MyLocation.SwimmingPoolHotTubSurcharge_NumberOfUnits)

                        Me.chkTrampoline.Checked = Me.MyLocation.TrampolineSurcharge
                        divNumOfTrampolines.Attributes.Add("style", "display: none")
                        txtNumOfTrampolines.Text = 0
                        UpdateNumberOfUnits(TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote), chkTrampoline, divNumOfTrampolines, txtNumOfTrampolines, Me.MyLocation.TrampolineSurcharge_NumberOfUnits)

                        Me.chkWoodStove.Checked = Me.MyLocation.WoodOrFuelBurningApplianceSurcharge
                        divNumOfWoodburningStoves.Attributes.Add("style", "display: none")
                        txtNumOfWoodburningStoves.Text = 0
                        UpdateNumberOfUnits(WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote), chkWoodStove, divNumOfWoodburningStoves, txtNumOfWoodburningStoves, Me.MyLocation.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits)

                    ElseIf Me.SubQuoteFirst.ProgramTypeId = "6" AndAlso Me.MyLocationIndex > 0 AndAlso Farm_General.hasAdditionalQuestionsForFarmItemNumberOfUnitsUpdate(Quote) Then
                        Me.trQuestion1.Visible = False
                        Me.trQuestion2.Visible = False
                        Me.trQuestion3.Visible = False
                        Me.trQuestion4.Visible = False
                        Me.trQuestion5.Visible = False
                        Me.trQuestion6.Visible = False
                        Me.trQuestion6b.Visible = False
                        Me.trQuestion10.Visible = False
                        Me.trQuestion11.Visible = False

                        Me.chkSwimmingPool.Checked = Me.MyLocation.SwimmingPoolHotTubSurcharge
                        divNumOfSwimmingPools.Attributes.Add("style", "display: none")
                        txtNumOfSwimmingPools.Text = 0
                        UpdateNumberOfUnits(SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote), chkSwimmingPool, divNumOfSwimmingPools, txtNumOfSwimmingPools, Me.MyLocation.SwimmingPoolHotTubSurcharge_NumberOfUnits)

                        Me.chkTrampoline.Checked = Me.MyLocation.TrampolineSurcharge
                        divNumOfTrampolines.Attributes.Add("style", "display: none")
                        txtNumOfTrampolines.Text = 0
                        UpdateNumberOfUnits(TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote), chkTrampoline, divNumOfTrampolines, txtNumOfTrampolines, Me.MyLocation.TrampolineSurcharge_NumberOfUnits)

                        Me.chkWoodStove.Checked = Me.MyLocation.WoodOrFuelBurningApplianceSurcharge
                        divNumOfWoodburningStoves.Attributes.Add("style", "display: none")
                        txtNumOfWoodburningStoves.Text = 0
                        UpdateNumberOfUnits(WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote), chkWoodStove, divNumOfWoodburningStoves, txtNumOfWoodburningStoves, Me.MyLocation.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits)
                    Else
                        Me.Visible = False
                    End If
                End If
        End Select

    End Sub

    ''' <summary>
    ''' Attempts to get an email address from policyholder1/policyholder2 for the wood burning stove data
    ''' </summary>
    ''' <returns></returns>
    Private Function GetPolicyHolderEmailForWoodBurningStove(ByRef WhichEmail As String) As String
        Dim email As String = Nothing
        WhichEmail = ""

        ' First check policyholder1 email
        If Quote.Policyholder IsNot Nothing Then
            If Quote.Policyholder.HasData Then
                If Not Quote.Policyholder.PrimaryEmail.IsNullEmptyorWhitespace Then
                    email = Quote.Policyholder.PrimaryEmail
                    WhichEmail = "PH1"
                End If
            End If
        End If

        ' Next check policyholder2 email
        If email Is Nothing AndAlso Quote.Policyholder2 IsNot Nothing Then
            If Quote.Policyholder2.HasData Then
                If Not Quote.Policyholder2.PrimaryEmail.IsNullEmptyorWhitespace Then
                    email = Quote.Policyholder2.PrimaryEmail
                    WhichEmail = "PH2"
                End If
            End If
        End If

        If email IsNot Nothing Then Return email Else Return ""
    End Function

    Public Function WoodburningIsNewToEndorsement(Optional ByRef KeyWasPresent As Boolean = False, Optional ByRef KeyWasSet As Boolean = False) As Boolean
        KeyWasPresent = False
        KeyWasSet = False

        Dim HadPreviousWoodburning As Boolean = IFM.VR.Web.Helpers.WebHelper_Personal.QuoteHadWoodburningSurchargeOnPreviousImage(Me.QuoteId, Me.Quote, KeyWasPresent, KeyWasSet)

        Return Not HadPreviousWoodburning
    End Function

    Public Overrides Function Save() As Boolean
        'Additional Questions

        Select Case Me.Quote.LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                ' 06/16/2022 CAH - Do not set the below elements in Home
                ' 06/16/2022 CAH - Farm Only options: txtNumOfTrampolines.Text,txtNumOfSwimmingPools.Text, txtNumOfWoodburningStoves.Text

                Me.MyLocation.MultiPolicyDiscount = Me.chkHasAutoPolicy.Checked

                'Me.MyLocation.SelectMarketCredit = False 'Removed 7/2/18 for HOM2011 Upgrade post go-live changes MLW
                If Me.MyLocation.FormTypeId.NotEqualsAny("6", "7") Then ' do not apply to mobile homes
                    'Updated 12/5/17 for HOM Upgrade MLW - updated 6/29/18 for HOM2011 Upgrade post go-live changes MLW - now available on mobile homes
                    'If ((Me.MyLocation.FormTypeId = "22" Or Me.MyLocation.FormTypeId = "25") And Me.MyLocation.StructureTypeId = "2") Then
                    '    Me.MyLocation.SelectMarketCredit = False
                    'Else
                    Me.MyLocation.SelectMarketCredit = Not Me.chkAnyChildren.Checked
                    'End If
                Else
                    Me.MyLocation.SelectMarketCredit = False
                End If

                Me.MyLocation.FireSmokeAlarm_SmokeAlarm = Me.chkSmokeAlarms.Checked
                Me.MyLocation.FireSmokeAlarm_LocalAlarmSystem = Me.ddFireAlarmType.SelectedValue = "2"
                Me.MyLocation.FireSmokeAlarm_CentralStationAlarmSystem = Me.ddFireAlarmType.SelectedValue = "3"

                Me.MyLocation.BurglarAlarm_LocalAlarmSystem = Me.ddBurglarAlarmType.SelectedValue = "3"
                Me.MyLocation.BurglarAlarm_CentralStationAlarmSystem = Me.ddBurglarAlarmType.SelectedValue = "1"

                'Updated 12/18/17 for HOM Upgrade MLW
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") Then
                    Me.MyLocation.SprinklerSystem = Me.chkSprinkler.Checked
                Else
                    Me.MyLocation.SprinklerSystem_AllExcept = Me.ddSprinklerType.SelectedValue = "1"
                    Me.MyLocation.SprinklerSystem_AllIncluding = Me.ddSprinklerType.SelectedValue = "2"
                End If

                Me.MyLocation.TrampolineSurcharge = Me.chkTrampoline.Checked
                'Me.MyLocation.NumberOfPools = CInt(Me.chkSwimmingPool.Checked).ToString()
                Me.MyLocation.SwimmingPoolHotTubSurcharge = Me.chkSwimmingPool.Checked
                'Me.MyLocation.NumberOfSolidFuelBurningUnits = CInt(Me.chkWoodStove.Checked).ToString()
                Me.MyLocation.WoodOrFuelBurningApplianceSurcharge = Me.chkWoodStove.Checked
                Dim WhichEmail As String = ""
                Dim PHEmail As String = GetPolicyHolderEmailForWoodBurningStove(WhichEmail)
                If PHEmail = "" Then
                    ' Policyholder emails came back empty, if we collected an email for woodburning stove,
                    ' update the policyholder primary email with that email.
                    If Me.txtWoodBurningEmail.Text <> "" Then
                        If Quote.Policyholder.Emails Is Nothing OrElse Quote.Policyholder.Emails.Count <= 0 Then
                            ' No emails object on the policyholder.  Create it and add the new email.
                            Quote.Policyholder.Emails = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)
                            Dim newEmail As New QuickQuote.CommonObjects.QuickQuoteEmail()
                            newEmail.Address = Me.txtWoodBurningEmail.Text
                            Quote.Policyholder.Emails.Add(newEmail)
                        Else
                            ' Policyholder email came back empty but a emails item was found (in policyholder),
                            ' so apparently it's empty.  Populate it with what we collected for the woodburning email.
                            Quote.Policyholder.Emails(0).Address = txtWoodBurningEmail.Text
                        End If
                        RaiseEvent PolicyholderReloadNeeded()
                    End If
                Else
                    ' An email came back from the policyholders, check to see if it's valid.  If valid do nothing.
                    If IFM.Common.InputValidation.CommonValidations.IsValidEmail(PHEmail) = False AndAlso IFM.Common.InputValidation.CommonValidations.IsValidEmail(txtWoodBurningEmail.Text) = True Then
                        ' INVALID PH EMAIL ADDRESS BUT VALID WOOD BURNING EMAIL ADDRESS
                        ' If the email saved on the policyholder is invalid, but the one 
                        ' for woodburning is valid, overwrite the one on the policyholder
                        ' with the valid one.  Note that we want to update the correct
                        ' invalid email, it could have been on PH1 or PH2
                        If WhichEmail = "PH1" Then
                            ' update the policyholder 1 record
                            If Quote.Policyholder.Emails Is Nothing Then Quote.Policyholder.Emails = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)
                            If Quote.Policyholder.Emails.Count <= 0 Then
                                Dim newEmail As New QuickQuote.CommonObjects.QuickQuoteEmail
                                newEmail.Address = txtWoodBurningEmail.Text
                                Quote.Policyholder.Emails.Add(newEmail)
                            Else
                                Quote.Policyholder.Emails(0).Address = txtWoodBurningEmail.Text
                            End If
                        Else
                            ' update the policyholder 2 record
                            ' I'm not sure how an invalid email would get into PH2
                            ' but this code handles for it.
                            If Quote.Policyholder2.Emails Is Nothing Then Quote.Policyholder2.Emails = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)
                            If Quote.Policyholder2.Emails.Count <= 0 Then
                                Dim newEmail As New QuickQuote.CommonObjects.QuickQuoteEmail
                                newEmail.Address = txtWoodBurningEmail.Text
                                Quote.Policyholder2.Emails.Add(newEmail)
                            Else
                                Quote.Policyholder2.Emails(0).Address = txtWoodBurningEmail.Text
                            End If
                        End If
                        RaiseEvent PolicyholderReloadNeeded()
                    End If
                End If

                If Me.chkFirstWrittenDate.Checked Then
                    Me.Quote.FirstWrittenDate = Me.txtFirstWrittenDate.Text
                Else
                    Me.Quote.FirstWrittenDate = Me.Quote.EffectiveDate
                End If

                'Updated 03/24/2020 for Home Endorsements task 45262 MLW
                If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
                    'T40490 CAH
                    If Me.Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24") Then
                        'add fake ai if checked (if not present), remove fake if not.
                        If Me.chkMortgageContractSeller.Checked Then
                            IFM.VR.Common.Helpers.AdditionalInterest.AddFakeAI_HOM(Me.Quote)
                        Else
                            IFM.VR.Common.Helpers.AdditionalInterest.RemoveAllFakeAIs(Me.Quote)
                        End If
                    End If
                End If

            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                'Updated 8/23/18 for multi state MLW
                If SubQuoteFirst IsNot Nothing Then
                    Select Case Me.SubQuoteFirst.ProgramTypeId
                        Case "6"
                            If Me.MyLocationIndex = 0 Then 'Farm Owners Only and Only on first location
                                Me.MyLocation.FireDepartmentAlarm = Me.chkSmokeAlarms.Checked
                                Me.MyLocation.FireSmokeAlarm_LocalAlarmSystem = Me.ddFireAlarmType.SelectedValue = "2"
                                Me.MyLocation.FireSmokeAlarm_CentralAlarmSystem = Me.ddFireAlarmType.SelectedValue = "3"
                                Me.MyLocation.BurglarAlarm_LocalAlarmSystem = Me.ddBurglarAlarmType.SelectedValue = "3"
                                Me.MyLocation.BurglarAlarm_CentralAlarmSystem = Me.ddBurglarAlarmType.SelectedValue = "1"
                                Me.MyLocation.PoliceDepartmentTheftAlarm = Me.ddBurglarAlarmType.SelectedValue = "2"
                                Me.MyLocation.SprinklerSystem_AllExcept = Me.ddSprinklerType.SelectedValue = "1"
                                Me.MyLocation.SprinklerSystem_AllIncluding = Me.ddSprinklerType.SelectedValue = "2"
                            End If

                            Me.MyLocation.TrampolineSurcharge = Me.chkTrampoline.Checked
                            If TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote) AndAlso chkTrampoline.Checked Then
                                Me.MyLocation.TrampolineSurcharge_NumberOfUnits = QQHelper.IntegerForString(txtNumOfTrampolines.Text)
                                If IsQuoteEndorsement() AndAlso QQHelper.IsPositiveIntegerString(txtNumOfTrampolines.Text) = False Then
                                    Me.MyLocation.TrampolineSurcharge_NumberOfUnits = 1
                                End If
                            Else
                                Me.MyLocation.TrampolineSurcharge_NumberOfUnits = 0
                            End If

                            Me.MyLocation.SwimmingPoolHotTubSurcharge = Me.chkSwimmingPool.Checked
                            If SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote) AndAlso chkSwimmingPool.Checked Then
                                Me.MyLocation.SwimmingPoolHotTubSurcharge_NumberOfUnits = QQHelper.IntegerForString(txtNumOfSwimmingPools.Text)
                                If IsQuoteEndorsement() AndAlso QQHelper.IsPositiveIntegerString(txtNumOfSwimmingPools.Text) = False Then
                                    Me.MyLocation.SwimmingPoolHotTubSurcharge_NumberOfUnits = 1
                                End If
                            Else
                                Me.MyLocation.SwimmingPoolHotTubSurcharge_NumberOfUnits = 0
                            End If

                            Me.MyLocation.WoodOrFuelBurningApplianceSurcharge = Me.chkWoodStove.Checked
                            If WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote) AndAlso chkWoodStove.Checked Then
                                Me.MyLocation.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits = QQHelper.IntegerForString(txtNumOfWoodburningStoves.Text)
                                If IsQuoteEndorsement() AndAlso QQHelper.IsPositiveIntegerString(txtNumOfWoodburningStoves.Text) = False Then
                                    Me.MyLocation.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits = 1
                                End If
                            Else
                                Me.MyLocation.WoodOrFuelBurningApplianceSurcharge_NumberOfUnits = 0
                            End If
                        Case Else
                            ' is not visible so don't save anything
                    End Select
                End If
        End Select

        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Property Additional Questions"
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
        Dim valList = PropertyValidator.ValidateHOMLocation(Me.Quote, valArgs.ValidationType)
        If valList.Any() Then
            For Each v In valList
                Select Case v.FieldId
                    Case PropertyValidator.FirstWrittenDate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtFirstWrittenDate, v, accordList)
                End Select
            Next
        End If

        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso (Me.IsQuoteEndorsement = False OrElse (Me.IsQuoteEndorsement = True AndAlso WoodburningIsNewToEndorsement() = True)) Then
            ' New business or endorsement with new wood burning - validate the wood burning email
            ' Wood burning stove requires an email address
            If chkWoodStove.Checked Then
                If txtWoodBurningEmail.Text.Trim = "" Then
                    Me.ValidationHelper.AddError(txtWoodBurningEmail, "Missing contact Email", accordList)
                ElseIf IFM.Common.InputValidation.CommonValidations.IsValidEmail(txtWoodBurningEmail.Text) = False Then
                    Me.ValidationHelper.AddError(txtWoodBurningEmail, "Invalid Email", accordList)
                End If
            End If
            'ElseIf Me.IsQuoteEndorsement = True AndAlso WoodburningIsNewToEndorsement() = False Then
            ' This is an endorsement but wood burning is not new - do not validate the wood burning email
        End If

        'Added 6/14/2022 for task 72947 MLW
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm AndAlso SubQuoteFirst.ProgramTypeId = "6" Then
            FarmNumUnitsCheck(WoodburningStoveHelper.IsWoodburningNumOfUnitsAvailable(Quote), chkWoodStove, txtNumOfWoodburningStoves, accordList)
            FarmNumUnitsCheck(SwimmingPoolUnitsHelper.isSwimmingPoolUnitsAvailable(Quote), chkSwimmingPool, txtNumOfSwimmingPools, accordList)
            FarmNumUnitsCheck(TrampolineUnitsHelper.isTrampolineUnitsAvailable(Quote), chkTrampoline, txtNumOfTrampolines, accordList)
        End If

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Sub FarmNumUnitsCheck(enabled As Boolean, chkBox As CheckBox, NumUnitsTextbox As TextBox, ByRef accordList As List(Of VRAccordionTogglePair))
        If enabled AndAlso chkBox.Checked Then
            If NumUnitsTextbox.Text.Trim = "" Then
                Me.ValidationHelper.AddError(NumUnitsTextbox, "Missing Number of Units", accordList)
            ElseIf QQHelper.IntegerForString(NumUnitsTextbox.Text) <= 0 Then
                Me.ValidationHelper.AddError(NumUnitsTextbox, "Number of Units must be greater than 0", accordList)
            End If
        End If
    End Sub

    Public Overrides Sub ClearControl()
        Me.chkAnyChildren.Checked = False
        Me.chkHasAutoPolicy.Checked = False
        Me.chkSmokeAlarms.Checked = False
        Me.chkSprinkler.Checked = False 'Added 12/18/17 for HOM Upgrade MLW
        Me.ddSprinklerType.SetFromValue("")
        Me.ddBurglarAlarmType.SetFromValue("")
        Me.ddFireAlarmType.SetFromValue("")
        Me.chkSwimmingPool.Checked = False
        Me.chkTrampoline.Checked = False
        Me.chkWoodStove.Checked = False
        Me.chkMortgageContractSeller.Checked = False

        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm AndAlso SubQuoteFirst.ProgramTypeId = "6" Then
            txtNumOfTrampolines.Text = "0"
            txtNumOfSwimmingPools.Text = "0"
            txtNumOfWoodburningStoves.Text = "0"
            divNumOfSwimmingPools.Attributes.Add("style", "display: none")
            divNumOfTrampolines.Attributes.Add("style", "display: none")
            divNumOfWoodburningStoves.Attributes.Add("style", "display: none")
        End If

        MyBase.ClearControl()
    End Sub
    Protected Sub lnkClearAdditionalQuestions_Click(sender As Object, e As EventArgs) Handles lnkClearAdditionalQuestions.Click

        ' you don't want to clear the whlole control here

        Me.chkAnyChildren.Checked = False
        Me.chkHasAutoPolicy.Checked = False
        Me.chkSmokeAlarms.Checked = False
        Me.chkSprinkler.Checked = False 'Added 12/18/17 for HOM Upgrade MLW
        Me.ddSprinklerType.SetFromValue("")
        Me.ddBurglarAlarmType.SetFromValue("")
        Me.ddFireAlarmType.SetFromValue("")
        Me.chkSwimmingPool.Checked = False
        Me.chkTrampoline.Checked = False
        Me.chkWoodStove.Checked = False
        Me.chkMortgageContractSeller.Checked = False

        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm AndAlso SubQuoteFirst.ProgramTypeId = "6" Then
            txtNumOfTrampolines.Text = "0"
            txtNumOfSwimmingPools.Text = "0"
            txtNumOfWoodburningStoves.Text = "0"
            divNumOfSwimmingPools.Attributes.Add("style", "display: none")
            divNumOfTrampolines.Attributes.Add("style", "display: none")
            divNumOfWoodburningStoves.Attributes.Add("style", "display: none")
        End If

        'force edit mode so they have to save at some point before leaving
        Me.LockTree()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles lnkSaveAdditionalQuestions.Click
        'all saves call this event sub so keep it generic
        Me.Save_FireSaveEvent(True)
        Me.Populate()
    End Sub

    Protected Sub UpdateNumberOfUnits(enabled As Boolean, chkBox As CheckBox, NumDiv As HtmlGenericControl, NumText As TextBox, ByRef NumOfUnits As Integer)
        If enabled Then
            If chkBox.Checked Then
                NumDiv.Attributes.Add("style", "display: block")
                If NumOfUnits <= 0 Then NumOfUnits = 1
                NumText.Text = NumOfUnits
            End If
        End If
    End Sub

End Class