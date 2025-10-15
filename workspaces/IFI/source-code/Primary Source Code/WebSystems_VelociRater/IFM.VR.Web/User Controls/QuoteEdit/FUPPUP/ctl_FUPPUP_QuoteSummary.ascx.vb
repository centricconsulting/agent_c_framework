Imports System.Globalization
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM
Imports IFM.VR.Web.ENUMHelper
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonObjects.Umbrella

Public Class ctl_FUPPUP_QuoteSummary
    Inherits VRControlBase

    Private _parsedCalc As Helpers.UmbrellaCalcFunctions
    Public Property ParsedCalc() As Helpers.UmbrellaCalcFunctions
        Get
            Return _parsedCalc
        End Get
        Set(ByVal value As Helpers.UmbrellaCalcFunctions)
            _parsedCalc = value
        End Set
    End Property

    Protected Const _MISCELLANEOUS_LIABILITY_TYPE_ID_SWIMMING_POOL = "1"
    Protected Const _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP = "3"
    Protected Const _MISCELLANEOUS_LIABILITY_TYPE_ID_TRAMPOLINE = "2"

    Dim indent As String = "&nbsp;&nbsp;"
    Dim dblindent As String = "&nbsp;&nbsp;&nbsp;&nbsp;"
    'Added 12/17/18 for multi state bug 30351 MLW
    Private NumFormatWithCents As String = "$###,###,###.00"
    Private NumFormat As String = "###,###,###"

    Private HomeArray As ArrayList = New ArrayList()
    Private RVArray As ArrayList = New ArrayList()
    Private WatercraftArray As ArrayList = New ArrayList()
    Private MiscLibArray As ArrayList = New ArrayList()
    Private InvestmentPropArray As ArrayList = New ArrayList()
    Private ProfessionalLibArray As ArrayList = New ArrayList()
    Private AutoArray As ArrayList = New ArrayList()
    Private FarmArray As ArrayList = New ArrayList()
    Private CustomFarmingArray As ArrayList = New ArrayList()
    Private StopGapArray As ArrayList = New ArrayList()
    Private DFRArray As ArrayList = New ArrayList()
    Private CommercialAutoArray As ArrayList = New ArrayList()
    Private WorkersCompArray As ArrayList = New ArrayList()

    Private PpaDriver16To20 As Int16 = 0
    Private PpaDriver21To24 As Int16 = 0
    Private CapDriver16To20 As Int16 = 0
    Private CapDriver21To24 As Int16 = 0


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divUmbrellaQuoteSummary", HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

        If Me.Quote IsNot Nothing Then
            If IsOnAppPage Then
                Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                Me.lblUnderwriterReminder.Visible = True
            Else
                Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
            End If
            'updated 5/10/2019; logic taken from updates for PPA
            Select Case Me.Quote.QuoteTransactionType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                    Me.lblMainAccord.Text = If(Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId) & " Premium"
                    Me.ImageDateAndPremChangeLine.Visible = True
                    If QQHelper.IsDateString(Me.Quote.TransactionEffectiveDate) = True Then
                        Me.lblTranEffDate.Text = "Effective Date: " & Me.Quote.TransactionEffectiveDate 'note: should already be in shortdate format
                    Else
                        Me.lblTranEffDate.Text = ""
                    End If
                    If QQHelper.IsNumericString(Me.Quote.ChangeInFullTermPremium) = True Then 'note: was originally looking for positive decimal, but the change in prem could be zero or negative
                        Me.lblAnnualPremChg.Text = "Annual Premium Change: " & Me.Quote.ChangeInFullTermPremium 'note: should already be in money format
                    Else
                        Me.lblAnnualPremChg.Text = ""
                    End If
                Case Else
                    Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                    Me.ImageDateAndPremChangeLine.Visible = False
            End Select

            Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

            Me.lblPhName.Text = Me.Quote.Policyholder.Name.DisplayName
            If String.IsNullOrEmpty(Me.Quote.Policyholder.Name.DoingBusinessAsName) = False Then
                lblPhName.Text = lblPhName.Text + " DBA " + Me.Quote.Policyholder.Name.DoingBusinessAsName
            End If

            Dim AddressOtherField As AddressOtherField = New AddressOtherField(Me.Quote.Policyholder.Address.Other)
            If AddressOtherField.PrefixType = Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Other Then
                Me.lblCareOf.Text = ""
                Me.trCareOf.Visible = False
            Else
                Me.lblCareOf.Text = AddressOtherField.NameWithPrefix
                Me.trCareOf.Visible = True
            End If

            Me.lblQuoteNum.Text = Me.Quote.QuoteNumber

            Dim zip As String = Me.Quote.Policyholder.Address.Zip
            If zip.Length > 5 Then
                zip = zip.Substring(0, 5)
            End If

            'house num, street, apt, pobox, city, state, zip
            Me.lblPhAddress.Text = String.Format("{0} {1} {2} {3} {4} {5} {6}", Me.Quote.Policyholder.Address.HouseNum, Me.Quote.Policyholder.Address.StreetName, If(String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Address.ApartmentNumber) = False, "Apt# " + Me.Quote.Policyholder.Address.ApartmentNumber, ""), Me.Quote.Policyholder.Address.POBox, Me.Quote.Policyholder.Address.City, Me.Quote.Policyholder.Address.State, zip).Replace("  ", " ").Trim()

            Me.lblEffectiveDate.Text = Me.Quote.EffectiveDate
            Me.lblExpirationDate.Text = Me.Quote.ExpirationDate

            Me.lblFullPremium.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

            Me.tdUmbrellaLimit.InnerText = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaCoverageLimitId, SubQuoteFirst.UmbrellaCoverageLimitId)
            Me.tdUMUIMLimit.InnerText = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaUmUimLimitId, SubQuoteFirst.UmbrellaUmUimLimitId)
            Me.tdSelfInsuredRetentionLimit.InnerText = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaSelfInsuredRetentionLimitId, SubQuoteFirst.UmbrellaSelfInsuredRetentionLimitId)

            Me.tdUmbrellaPremium.InnerText = FormatPremium(GoverningStateQuote.UmbrellaLimitPremium)
            Me.tdUMUIMPremium.InnerText = FormatPremium(GoverningStateQuote.UmbrellaUmUimPremium)

            ParsedCalc = New Helpers.UmbrellaCalcFunctions(SubQuoteFirst.UmbrellaCoverageCalculation)

            If GoverningStateQuote() IsNot Nothing AndAlso GoverningStateQuote.UnderlyingPolicies IsNot Nothing AndAlso GoverningStateQuote.UnderlyingPolicies.Count > 0 Then
                ClearSections()
                CreateSectionData()
                PopulateSections()
            End If

            PopulateChildControls()
        End If
    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init

        Me.UseRatedQuoteImage = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Public Sub CreateSectionData()
        For Each Policy As QuickQuoteUnderlyingPolicy In SubQuoteFirst?.UnderlyingPolicies
            For Each PolicyTypeInfo As PolicyInfo In Policy.PolicyInfos
                Select Case PolicyTypeInfo.TypeId.TryToGetInt32
                    Case PolicyTypeId.AutomobileLiability
                        GetAutoRepeaterData(PolicyTypeInfo)
                    Case PolicyTypeId.PersonalLiability
                        GetPersonalLiabilityRepeaterData(PolicyTypeInfo, Policy.LobId)
                    Case PolicyTypeId.InvestmentPropertyLiability
                        GetInvestmentPropertyRepeaterData(PolicyTypeInfo, Policy.LobId)
                    Case PolicyTypeId.MiscellaneousLiability
                        GetMiscellaneousRepeaterData(PolicyTypeInfo, QuickQuote.CommonMethods.QuickQuoteHelperClass.LobTypeForLobId(QQHelper.IntegerForString(Policy.LobId)))
                    Case PolicyTypeId.WatercraftLiability,
                         PolicyTypeId.Watercraft
                        GetWatercraftRepeaterData(PolicyTypeInfo)
                    Case PolicyTypeId.RecreationalVehicleLiability,
                        PolicyTypeId.RecreationalVehicleLiabilityAuto,
                        PolicyTypeId.RecreationalVehicleLiabilityHome
                        GetRVRepeaterData(PolicyTypeInfo)
                    Case PolicyTypeId.ProfessionalLiability
                        GetProfessionalLiabData(PolicyTypeInfo)
                    Case PolicyTypeId.EmployersLiabilityStopGap '70
                        GetStopGapData(PolicyTypeInfo)
                    Case PolicyTypeId.CommercialAutoLiability '77
                        GetCommercialAutoData(PolicyTypeInfo)
                    Case PolicyTypeId.WorkersCompLiability '65
                        GetWorkersCompData(PolicyTypeInfo)
                    Case PolicyTypeId.FarmLiability '50 - This is CustomFarm
                        GetCustomFarmData(PolicyTypeInfo)
                    Case PolicyTypeId.DwellingFireLiability
                        GetDwellingFireRepeaterData(PolicyTypeInfo)
                End Select
            Next
        Next


    End Sub

    Public Sub PopulateSections()

        If SubQuoteFirst.ProgramTypeId = "4" Then
            'personal 
            perPLHomeCoverage.HeaderText = "Personal Liability - Home"
            perPLHomeCoverage.repeaterData = HomeArray

            perRecreationalVehicle.HeaderText = "Recreational Vehicle"
            perRecreationalVehicle.repeaterData = RVArray

            perWatercraft.HeaderText = "Watercraft"
            perWatercraft.repeaterData = WatercraftArray

            perMiscellaneous.HeaderText = "Miscellaneous"
            perMiscellaneous.repeaterData = MiscLibArray

            perInvestmentProperty.HeaderText = "Investment Property"
            perInvestmentProperty.repeaterData = InvestmentPropArray

            perProfessionalLiability.HeaderText = "Professional Liability"
            perProfessionalLiability.repeaterData = ProfessionalLibArray

            perPLDfrCoverage.HeaderText = "Personal Liability - Dwelling Fire"
            perPLDfrCoverage.repeaterData = DFRArray

            perAuto.HeaderText = "Auto"
            perAuto.repeaterData = AutoArray

        Else
            'farm
            ctlPLFarmCoverage.HeaderText = "Personal Liability - Farm"
            ctlPLFarmCoverage.repeaterData = FarmArray

            ctlCustomFarmingCoverage.HeaderText = "Custom Farming"
            ctlCustomFarmingCoverage.repeaterData = CustomFarmingArray

            ctlWatercraft.HeaderText = "Watercraft"
            ctlWatercraft.repeaterData = WatercraftArray

            ctlProfessionalLiability.HeaderText = "Professional Liability"
            ctlProfessionalLiability.repeaterData = ProfessionalLibArray

            ctlStopGapCoverage.HeaderText = "Optional Liability - Stop Gap Employers Liability"
            ctlStopGapCoverage.repeaterData = StopGapArray

            ctlPLHomeCoverage.HeaderText = "Personal Liability - Home"
            ctlPLHomeCoverage.repeaterData = HomeArray

            ctlRecreationalVehicle.HeaderText = "Recreational Vehicle"
            ctlRecreationalVehicle.repeaterData = RVArray

            ctlMiscellaneous.HeaderText = "Miscellaneous"
            ctlMiscellaneous.repeaterData = MiscLibArray

            ctlInvestmentProperty.HeaderText = "Investment Property"
            ctlInvestmentProperty.repeaterData = InvestmentPropArray

            ctlPLDfrCoverage.HeaderText = "Personal Liability - Dwelling Fire"
            ctlPLDfrCoverage.repeaterData = DFRArray

            ctlAuto.HeaderText = "Auto"
            ctlAuto.repeaterData = AutoArray

            ctlCommercialAutoCoverage.HeaderText = "Commercial Auto"
            ctlCommercialAutoCoverage.repeaterData = CommercialAutoArray

            ctlWorkersCompCoverage.HeaderText = "Workers Comp"
            ctlWorkersCompCoverage.repeaterData = WorkersCompArray

        End If

    End Sub


#Region "DataGathering"
    Public Sub GetAutoRepeaterData(pol As PolicyInfo)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0

        For Each vehicle As Vehicle In If(pol?.Vehicles, Enumerable.Empty(Of Vehicle))
            For item As Integer = 1 To vehicle.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty
                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaVehicleTypeId, vehicle.TypeId)

                If String.IsNullOrWhiteSpace(pol.CombinedSingleLimit) OrElse pol.CombinedSingleLimit = "0" Then
                    Dim LimitA = FormatLimit(pol.BodilyInjuryLimitA)
                    Dim LimitB = FormatLimit(pol.BodilyInjuryLimitB)
                    Dim PropDam = FormatLimit(pol.PropertyDamageLimit)

                    limit = LimitA & "/" & LimitB & "/" & PropDam
                Else
                    limit = FormatLimit(pol.CombinedSingleLimit)
                End If


                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If

                AutoArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                SkipYouthFulOperatorInCalc(PremiumList, PremiumListCount)
            Next
        Next
        If pol?.YouthfulOperators IsNot Nothing Then
            For Each op As YouthfulOperator In pol?.YouthfulOperators
                If op.YouthfulOperatorTypeId = "2" Then
                    PpaDriver16To20 += op.YouthfulOperatorCount
                ElseIf op.YouthfulOperatorTypeId = "3" Then
                    PpaDriver21To24 += op.YouthfulOperatorCount
                End If
            Next
        End If
        UpdateYouthfulDrivers(PremiumList, AutoArray, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
    End Sub

    Public Sub SkipYouthFulOperatorInCalc(PremiumList As List(Of Helpers.Calc), ByRef ListCount As Integer)
        ListCount += 1
        While (PremiumList.Count - 1) >= ListCount AndAlso PremiumList(ListCount)?.Name?.StartsWith("Youthful")
            ListCount += 1
        End While
    End Sub

    Public Sub UpdateYouthfulDrivers(PremiumList As List(Of Helpers.Calc), ByRef inputArray As ArrayList, LobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
        Dim coveragetype As String = String.Empty
        Dim limit As String = String.Empty
        Dim premium As String = String.Empty
        Dim Driver16To20 As Int16 = 0
        Dim Driver21To24 As Int16 = 0
        Select Case LobType
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                Driver16To20 = PpaDriver16To20
                Driver21To24 = PpaDriver21To24
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                Driver16To20 = CapDriver16To20
                Driver21To24 = CapDriver21To24
        End Select

        If Driver16To20 > 0 Then
            coveragetype = "Each operator age 16 through 20 (" & Driver16To20 & ")"
            If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 Then
                Dim charge = PremiumList.Find(Function(x As Helpers.Calc) x.Name.ToUpper = "Youthful 16 To 20 Premium".ToUpper)
                If charge IsNot Nothing Then
                    premium = FormatPremium(charge.Premium)
                Else
                    premium = String.Empty
                End If
            End If
            inputArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
        End If

        If Driver21To24 > 0 Then
            coveragetype = "Each operator age 21 through 24 (" & Driver21To24 & ")"
            If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 Then
                Dim charge = PremiumList.Find(Function(x As Helpers.Calc) x.Name.ToUpper = "Youthful 21 To 24 Premium".ToUpper)
                If charge IsNot Nothing Then
                    premium = FormatPremium(charge.Premium)
                Else
                    premium = String.Empty
                End If
            End If
            inputArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
        End If
    End Sub

    Public Sub GetPersonalLiabilityRepeaterData(pol As PolicyInfo, lobid As String)
        Dim ItemArray As ArrayList = New ArrayList()
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0
        Const AcreText = "Acreage over 1000 acres"
        Const AcreText_Patch = "Acreage over 1000 acress"

        For Each home As PersonalLiability In If(pol?.PersonalLiabilities, Enumerable.Empty(Of PersonalLiability))
            Dim coveragetype As String = String.Empty
            Dim address As String = String.Empty
            Dim limit As String = String.Empty
            Dim premium As String = String.Empty



            coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaPersonalLiabilityTypeId, home.TypeId)

            address = SetDisplayAddress(home.Address)

            limit = FormatLimit(pol.PersonalLiabilityLimit)

            If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                premium = FormatPremium(PremiumList(PremiumListCount).Premium)
            End If

            ItemArray.Add(New Summary4ColumnItem(coveragetype, address, limit, premium))

            Dim IncidentalOnPrem
            Dim IncidentalOffPrem
            If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                IncidentalOnPrem = PremiumList.Find(Function(x As Helpers.Calc) x.Name?.ToUpper = "Incidental Farming".ToUpper)
                IncidentalOffPrem = PremiumList.Find(Function(x As Helpers.Calc) x.Name?.ToUpper = "Farmers Personal Liability".ToUpper)
            End If

            If IncidentalOnPrem IsNot Nothing Then
                coveragetype = "Incidental Farming - On Premises"
                address = String.Empty
                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 Then
                    premium = FormatPremium(IncidentalOnPrem.Premium)
                End If
                ItemArray.Add(New Summary4ColumnItem(indent + coveragetype, address, limit, premium))
            End If

            If IncidentalOffPrem IsNot Nothing Then
                coveragetype = "Incidental Farming - Off Premises"
                address = String.Empty
                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 Then
                    premium = FormatPremium(IncidentalOffPrem.Premium)
                End If
                ItemArray.Add(New Summary4ColumnItem(indent + coveragetype, address, limit, premium))
            End If


            If QuickQuote.CommonMethods.QuickQuoteHelperClass.LobTypeForLobId(QQHelper.IntegerForString(lobid)) = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm Then
                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 Then
                    If PremiumList.HasItemAtIndex(PremiumListCount + 1) Then
                        If PremiumList(PremiumListCount + 1).Name.ToUpper = AcreText.ToUpper OrElse PremiumList(PremiumListCount + 1).Name.ToUpper = AcreText_Patch.ToUpper Then
                            PremiumListCount += 1
                            coveragetype = AcreText
                            address = String.Empty
                            premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                            ItemArray.Add(New Summary4ColumnItem(indent + coveragetype, address, limit, premium))
                        End If
                    End If
                End If
            End If

            PremiumListCount += 1
        Next
        Select Case QuickQuote.CommonMethods.QuickQuoteHelperClass.LobTypeForLobId(QQHelper.IntegerForString(lobid))
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                HomeArray.AddRange(ItemArray.Clone())
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                FarmArray.AddRange(ItemArray.Clone())
            Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                DFRArray.AddRange(ItemArray.Clone())
        End Select
        'add PL-Home additional Info
    End Sub

    Public Sub GetDwellingFireRepeaterData(pol As PolicyInfo)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0

        For Each location As Location In If(pol?.InvestmentProperties, Enumerable.Empty(Of Location))
            For item As Integer = 1 To location.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim address As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty
                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaDwellingTypeId, location.TypeId)

                address = SetDisplayAddress(location.Address)

                limit = FormatLimit(pol.PersonalLiabilityLimit)

                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If

                DFRArray.Add(New Summary4ColumnItem(coveragetype, address, limit, premium))

                PremiumListCount += 1
            Next
        Next
    End Sub

    Public Sub GetInvestmentPropertyRepeaterData(pol As PolicyInfo, lobid As String)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0

        For Each location As Location In If(pol?.InvestmentProperties, Enumerable.Empty(Of Location))
            For item As Integer = 1 To location.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim address As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty

                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaDwellingTypeId, location.TypeId)
                address = SetDisplayAddress(location.Address)
                limit = FormatLimit(pol.PersonalLiabilityLimit)

                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If

                If InvestmentPropArray.Count < 1 Then
                    InvestmentPropArray.Add(New Summary3ColumnItem("Incidental Business Exposures", "", ""))
                End If
                InvestmentPropArray.Add(New Summary3ColumnItem(indent + coveragetype, limit, premium) With {.extra2 = address})
                PremiumListCount += 1
            Next
        Next

    End Sub

    Public ReadOnly Property SwimmingPoolAndTrampolineUnitsAreAvailable As Boolean
        Get
            Dim effDate As Date = CDate(Me.Quote.EffectiveDate)
            Return (SwimmingPoolUnitsHelper.SwimmingPoolSettings.EnabledFlag AndAlso
                    effDate >= SwimmingPoolUnitsHelper.SwimmingPoolSettings.StartDate) AndAlso
                   (TrampolineUnitsHelper.TrampolineSettings.EnabledFlag AndAlso
                    effDate >= TrampolineUnitsHelper.TrampolineSettings.StartDate)
        End Get
    End Property
    Public Sub GetMiscellaneousRepeaterData(pol As PolicyInfo, lobType As QuickQuoteObject.QuickQuoteLobType)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0

        For Each miscLib As MiscellaneousLiability In If(pol?.MiscellaneousLiabilities, Enumerable.Empty(Of MiscellaneousLiability))
            'For item As Integer = 1 To miscLib.NumberOfItems.TryToGetInt32
            Dim coveragetype As String = String.Empty
            Dim address As String = String.Empty
            Dim limit As String = String.Empty
            Dim premium As String = String.Empty

            coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaMiscellaneousLiabilityTypeId, miscLib.TypeId)

            limit = FormatLimit(pol.PersonalLiabilityLimit)

            If PremiumList?.Any() AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                premium = FormatPremium(PremiumList(PremiumListCount).Premium)
            End If

            ''this could definitely be cleaned up, but may require more refactoring
            If miscLib.TypeId <> _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP AndAlso
               lobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso
               SwimmingPoolAndTrampolineUnitsAreAvailable AndAlso
               IsNumeric(miscLib.NumberOfItems) Then
                coveragetype += $" ({miscLib.NumberOfItems})"
                'premium = $"${CDec(premium.ToAlphaNumeric(allowPeriods:=True)) * CDec(miscLib.NumberOfItems)}"
                premium = $"${QQHelper.DecimalForString(premium.ToAlphaNumeric(allowPeriods:=True)) * QQHelper.DecimalForString(miscLib.NumberOfItems)}"
                MiscLibArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                PremiumListCount += CInt(miscLib.NumberOfItems)
            Else
                For item As Integer = 1 To miscLib.NumberOfItems.TryToGetInt32
                    MiscLibArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                    PremiumListCount += 1
                Next
            End If
            ' Next
        Next
    End Sub

    Public Sub GetWatercraftRepeaterData(pol As PolicyInfo)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0
        For Each vehicle As Watercraft In If(pol?.Watercrafts, Enumerable.Empty(Of Watercraft))
            For item As Integer = 1 To vehicle.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim address As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty
                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaWaterCraftTypeId, vehicle.TypeId)

                limit = FormatLimit(pol.PersonalLiabilityLimit)

                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If
                WatercraftArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                PremiumListCount += 1
            Next
        Next
    End Sub

    Public Sub GetRVRepeaterData(pol As PolicyInfo)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0
        For Each vehicle As RecreationalVehicle In If(pol?.RecreationalVehicles, Enumerable.Empty(Of RecreationalVehicle))
            For item As Integer = 1 To vehicle.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim address As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty

                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaRecreationalBodyTypeId, vehicle.TypeId)

                limit = FormatLimit(pol.PersonalLiabilityLimit)

                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If
                RVArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                PremiumListCount += 1
            Next
        Next
    End Sub

    Public Sub GetProfessionalLiabData(pol As PolicyInfo)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0
        For Each job As ProfessionalLiability In If(pol?.ProfessionalLiabilities, Enumerable.Empty(Of ProfessionalLiability))
            For item As Integer = 1 To job.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim address As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty
                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaProfessionalLiabilityId, job.TypeId)

                limit = FormatLimit(pol.PersonalLiabilityLimit)

                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If

                ProfessionalLibArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                PremiumListCount += 1
            Next
        Next
    End Sub

    Public Sub GetStopGapData(pol As PolicyInfo)
        Dim coveragetype As String = String.Empty
        Dim address As String = String.Empty
        Dim limit As String = String.Empty
        Dim premium As String = String.Empty

        coveragetype = "Each Accident"
        limit = FormatLimit(pol.EachAccident)
        premium = "Included"
        StopGapArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))

        coveragetype = "Disease Policy Limit"
        limit = FormatLimit(pol.DiseasePolicyLimit)
        premium = String.Empty
        StopGapArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))

        coveragetype = "Disease Each Employee"
        limit = FormatLimit(pol.DiseaseEachEmployee)
        premium = String.Empty
        StopGapArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
    End Sub

    Public Sub GetCommercialAutoData(pol As PolicyInfo)
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0
        For Each vehicle As Vehicle In If(pol?.Vehicles, Enumerable.Empty(Of Vehicle))
            For item As Integer = 1 To vehicle.NumberOfItems.TryToGetInt32
                Dim coveragetype As String = String.Empty
                Dim address As String = String.Empty
                Dim limit As String = String.Empty
                Dim premium As String = String.Empty
                coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.UmbrellaVehicleTypeId, vehicle.TypeId)

                limit = pol.BodilyInjuryLimitA & "/" & pol.PropertyDamageLimit

                If String.IsNullOrWhiteSpace(pol.CombinedSingleLimit) OrElse pol.CombinedSingleLimit = "0" Then
                    Dim LimitA = FormatLimit(pol.BodilyInjuryLimitA)
                    Dim PropDam = FormatLimit(pol.PropertyDamageLimit)

                    limit = LimitA & "/" & PropDam
                Else
                    limit = FormatLimit(pol.CombinedSingleLimit)
                End If

                If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                    premium = FormatPremium(PremiumList(PremiumListCount).Premium)
                End If
                CommercialAutoArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))
                SkipYouthFulOperatorInCalc(PremiumList, PremiumListCount)
            Next
        Next
        If pol?.YouthfulOperators IsNot Nothing Then
            For Each op As YouthfulOperator In pol?.YouthfulOperators
                If op.YouthfulOperatorTypeId = "2" Then
                    CapDriver16To20 += op.YouthfulOperatorCount
                ElseIf op.YouthfulOperatorTypeId = "3" Then
                    CapDriver21To24 += op.YouthfulOperatorCount
                End If
            Next
        End If
        UpdateYouthfulDrivers(PremiumList, CommercialAutoArray, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
    End Sub

    Public Sub GetWorkersCompData(pol As PolicyInfo)
        Dim coveragetype As String = String.Empty
        Dim address As String = String.Empty
        Dim limit As String = String.Empty
        Dim premium As String = String.Empty

        coveragetype = "Each Accident"
        limit = FormatLimit(pol.EachAccident)
        premium = "Included"
        WorkersCompArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))

        coveragetype = "Disease Policy Limit"
        limit = FormatLimit(pol.DiseasePolicyLimit)
        premium = String.Empty
        WorkersCompArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))

        coveragetype = "Disease Each Employee"
        limit = FormatLimit(pol.DiseaseEachEmployee)
        premium = String.Empty
        WorkersCompArray.Add(New Summary3ColumnItem(coveragetype, limit, premium))


    End Sub

    Public Sub GetCustomFarmData(pol As PolicyInfo)
        Dim ItemArray As ArrayList = New ArrayList()
        Dim PremiumList = ParsedCalc.GetStubGroup(pol.LinkNumber)
        Dim PremiumListCount = 0

        If CustomFarmingArray.Count < 1 Then
            ItemArray.Add(New Summary3ColumnItem("Annual Receipts", "", ""))
        End If

        For Each farm As FarmLiability In If(pol?.FarmLiabilities, Enumerable.Empty(Of FarmLiability))
            Dim coveragetype As String = String.Empty
            Dim address As String = String.Empty
            Dim limit As String = String.Empty
            Dim premium As String = String.Empty

            coveragetype = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteUmbrella, QuickQuotePropertyName.AnnualReceiptsTypeId, farm.TypeId)

            limit = FormatLimit(pol.PersonalLiabilityLimit)

            If PremiumList IsNot Nothing AndAlso PremiumList.Count > 0 AndAlso PremiumList.HasItemAtIndex(PremiumListCount) Then
                premium = FormatPremium(PremiumList(PremiumListCount).Premium)
            End If

            ItemArray.Add(New Summary3ColumnItem(indent + coveragetype, limit, premium))
            PremiumListCount += 1
            If farm.HerbicidesPesticidesUsed.ToUpper_NullSafe = "TRUE" Then
                coveragetype = "Spraying"
            Else
                coveragetype = "No Spraying"
            End If
            limit = String.Empty
            premium = String.Empty
            ItemArray.Add(New Summary3ColumnItem(dblindent + coveragetype, limit, premium))
        Next


        If ItemArray.Count > 1 Then
            CustomFarmingArray = ItemArray
        End If
    End Sub

#End Region

    Public Function FormatPremium(value As String) As String
        If value.IsNumeric Then
            Dim amount As Double
            If Double.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, amount) Then
                If amount > 0 Then
                    Return Format(amount, NumFormatWithCents)
                End If
                Return String.Empty
            End If
        End If
        Return value
    End Function

    Public Function FormatLimit(value As String) As String
        If value.IsNumeric Then
            Dim amount As Integer
            If Integer.TryParse(value, amount) Then
                Return Format(amount, NumFormat)
            End If
        End If
        Return value
    End Function


    Public Sub ClearSections()
        FarmArray = New ArrayList()
        CustomFarmingArray = New ArrayList()
        DFRArray = New ArrayList()
        HomeArray = New ArrayList()
        RVArray = New ArrayList()
        WatercraftArray = New ArrayList()
        MiscLibArray = New ArrayList()
        InvestmentPropArray = New ArrayList()
        ProfessionalLibArray = New ArrayList()
        AutoArray = New ArrayList()
        CommercialAutoArray = New ArrayList()
        StopGapArray = New ArrayList()
        WorkersCompArray = New ArrayList()
    End Sub

    Private Function SetDisplayAddress(address As Address) As String
        Dim _DisplayAddress As String = String.Empty
        If String.IsNullOrWhiteSpace(address.HouseNumber) = False AndAlso String.IsNullOrWhiteSpace(address.StreetName) = False Then
            _DisplayAddress = address.HouseNumber & " " & address.StreetName
        Else
            Return String.Empty
        End If
        Dim cityStateZip As String = QQHelper.appendText(address.City, StateAbbreviationForDiamondStateId(address.StateId), ", ")
        cityStateZip = QQHelper.appendText(cityStateZip, address.Zip, " ")
        Return QQHelper.appendText(_DisplayAddress, cityStateZip, " ")


    End Function

    Private Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        Try
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.printFriendlySummary, "")
        Catch ex As Exception

        End Try
    End Sub

End Class
