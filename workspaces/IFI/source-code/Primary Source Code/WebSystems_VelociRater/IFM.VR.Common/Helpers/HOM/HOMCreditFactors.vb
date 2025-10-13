Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonMethods

Namespace IFM.VR.Common.Helpers.HOM

    Public Class HOMCreditFactors

        Public Shared Function GetFirstWrittenCreditPercent(quote As QuickQuote.CommonObjects.QuickQuoteObject) As Integer
            Return CInt((1.0 - quote.ExperienceModificationFactor) * 100)
        End Function

        Public Shared Function GetFactors(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of String)
            Dim list As New List(Of String)
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass 'Added 7/2/18 for HOM2011 Upgrade post go-live changes MLW
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim calcText = ""
                Dim findItems As New List(Of String)
                Select Case topQuote.LobType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        If topQuote.Vehicles.Any() AndAlso topQuote.Vehicles(0).Coverages.Any() Then
                            Dim cov = (From c In topQuote.Vehicles.First().Coverages Select c Order By IFM.Common.InputValidation.InputHelpers.TryToGetDouble(c.FullTermPremium) Descending).FirstOrDefault()
                            If cov IsNot Nothing Then
                                calcText = cov.Calc
                            End If
                            findItems.Add("tier factor")
                            findItems.Add("good student")
                            findItems.Add("defensive driver")
                            findItems.Add("mature driver discount")
                            findItems.Add("anti-theft discount")
                            findItems.Add("multi policy discount")
                            findItems.Add("select market credit")
                            findItems.Add("employee discount credit")

                            findItems.Add("accident surcharge")
                            findItems.Add("violation surcharge")
                            findItems.Add("sdip accident surcharge")
                            findItems.Add("sdip violation surcharge")
                            findItems.Add("out of state surcharge")
                            findItems.Add("inexperience operator surcharge")
                            'Added 6/18/2019 for Bug 31002 MLW
                            findItems.Add("pay plan option")


                            If String.IsNullOrWhiteSpace(calcText) = False AndAlso calcText.Contains("*") Then
                                list = calcText.Split("*").ToList()
                                list = (From l In list Where String.IsNullOrWhiteSpace(l) = False AndAlso l.Contains(" ") AndAlso findItems.Contains(l.Trim().ToLower().Substring(0, l.Trim().ToLower().LastIndexOf(" "))) Select l.ToLower().Trim()).ToList()
                            End If

                        End If


                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal

                        If topQuote.Locations IsNot Nothing AndAlso topQuote.Locations.Any() Then
                            If topQuote.Locations(0) IsNot Nothing Then

                                Select Case topQuote.Locations(0).FormTypeId
                                    Case "5", "7", "4", "25", "26" 'updated 11/16/17 with new form types for HOM Upgrade MLW
                                        calcText = (From c In topQuote.Locations(0).LocationCoverages Where c.CoverageCodeId = "70023" Select c.Calc).FirstOrDefault()
                                    Case Else
                                        calcText = (From c In topQuote.Locations(0).LocationCoverages Where c.CoverageCodeId = "70021" Select c.Calc).FirstOrDefault()
                                End Select

                                findItems.Add("tier factor")
                                If IFM.VR.Common.Helpers.HOM.BuildingFactorHelper.IsBuildingFactorAvailable(topQuote) AndAlso topQuote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then 
                                    findItems.Add("building age")
                                Else
                                    'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                                    If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                        findItems.Add("new home discount factor")
                                    Else
                                        findItems.Add("new home credit")
                                    End If
                                End If
                                findItems.Add("deductible factor")
                                findItems.Add("acv roof")
                                'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                                If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    findItems.Add("auto/home credit factor")
                                Else
                                    findItems.Add("auto/home")
                                End If
                                findItems.Add("experience rating factor")
                                'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                                If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    findItems.Add("mature homeowner discount factor")
                                Else
                                    findItems.Add("mature homeowner credit")
                                End If
                                'findItems.Add("alarm credit")
                                'findItems.Add("protective device factor") ' ML Only -  Alarms
                                'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                                If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    findItems.Add("mobile home tie down factor")
                                Else
                                    findItems.Add("tie down factor") 'ML Only   ' Can look like this sometimes 'Tie Down Factor 0.9 - Credit for Other Liability Insurance 0'
                                End If
                                'Updated 7/2/18 for HOM2011 Upgrade post go-live changes MLW
                                If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    findItems.Add("select market credit factor")
                                Else
                                    findItems.Add("select market credit")
                                End If

                                'farm specific - I don't think we need to break this logic apart by LOB yet because either the discount is there or it isn't
                                findItems.Add("hobby farm") ' Matt A 6-16-2015

                                'T40491
                                findItems.Add("mortgage free discount")


                                If String.IsNullOrWhiteSpace(calcText) = False AndAlso calcText.Contains("*") Then
                                    list = calcText.Split("*").ToList()

                                    If topQuote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                        Dim tieDownindex As Int32 = 0
                                        Dim foundTieDown As Boolean = False
                                        For Each i In list
                                            i = i.Trim()
                                            If i.ToLower().StartsWith("tie down factor") Then
                                                If i.Contains("-") Then
                                                    foundTieDown = True
                                                    Exit For
                                                End If
                                            End If
                                            tieDownindex += 1
                                        Next

                                        If foundTieDown Then
                                            list(tieDownindex) = list(tieDownindex).Substring(0, list(tieDownindex).IndexOf("-")).Trim()
                                        End If

                                        'Added 7/11/18 for HOM2011 Upgrade post implementation changes MLW
                                        'This corrects the select market credit factor missing a space between the text and factor value
                                        'Once InsureSoft adds the space, this section is no longer needed
                                        If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                                Dim smcIndex As Int32 = 0
                                                For Each i In list
                                                    i = i.Trim()
                                                    Dim smcLength As Int32 = i.Length
                                                    If i.ToLower().StartsWith("select market credit factor") Then
                                                        Dim smcString As String = i
                                                        If smcLength > 27 AndAlso (smcString.Substring(0, 28) <> "Select Market Credit Factor ") Then
                                                            Dim left As String = smcString.Substring(0, 27)
                                                            Dim right As String = smcString.Substring(27, smcLength - 27)
                                                            smcString = left & " " & right
                                                            list.Add(String.Format(smcString))
                                                            Exit For
                                                        End If
                                                    End If
                                                    smcIndex += 1
                                                Next
                                            End If
                                        End If

                                        list = (From l In list Where String.IsNullOrWhiteSpace(l) = False AndAlso l.Contains(" ") AndAlso findItems.Contains(l.Trim().ToLower().Substring(0, l.Trim().ToLower().LastIndexOf(" "))) Select l.ToLower().Trim()).ToList()

                                    Dim alarmsCreditsPercent As Double = 0.0

                                    Dim credits = From c In topQuote.Locations(0).Modifiers Where c.ModifierGroupId = 1 Select c
                                    For Each m In credits
                                        ' You don't want to add alarm(s) credits directly - you will roll them up and add them later if any apply
                                        If m.ModifierGroupId = 1 AndAlso m.CheckboxSelected = True AndAlso (m.ParentModifierTypeId = "26" OrElse m.ParentModifierTypeId = "27" OrElse m.ParentModifierTypeId = "28") Then
                                            alarmsCreditsPercent += CDbl(m.ModifierAmount / 100.0)
                                        End If
                                    Next

                                    ' Show rolled up alarm(s) value
                                    If alarmsCreditsPercent > 0 Then
                                        'alarms maxes out at 10%
                                        list.Add(String.Format("alarm credit {0}", If(alarmsCreditsPercent <= 0.1, 1 - alarmsCreditsPercent, 1 - 0.1)))
                                    End If

                                End If


                            End If
                        End If

                End Select


            End If

            Return list
        End Function

        Public Shared Function GetPolicyDiscountsAsListOfPercents(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, ForPrintSummary As Boolean) As List(Of KeyValuePair(Of String, String))
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If

            Dim factors = GetFactors(topQuote)

            Dim factorCredits As New List(Of KeyValuePair(Of String, String))
            For Each f In factors
                f = f.Trim()
                Dim text As String = If(f.Contains(" "), f.Substring(0, f.LastIndexOf(" ")).Trim(), "")

                Select Case text
                    Case "tier factor"
                        If ForPrintSummary Then
                            text = "Credit Tier"
                        Else
                            text = "Credit Tier"
                        End If
                    Case "new home credit", "new home discount factor", "building age" 'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW - added new home discount factor
                        If ForPrintSummary Then
                            If BuildingFactorHelper.IsBuildingFactorAvailable(topQuote) AndAlso topQuote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                'Show as Building Factor for Home only
                                text = "Building Factor"
                            Else
                                'This will show as New Home for all lines of business but Home. Note: when removing the IsBuildingFactorAvailable flag after it expires and we do code cleanup, keep the LobType check.
                                text = "New Home"
                            End If
                        Else
                            If BuildingFactorHelper.IsBuildingFactorAvailable(topQuote) AndAlso topQuote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                'Show as Building Factor for Home only
                                text = "Building Factor"
                            Else
                                'This will show as New Home for all lines of business but Home. Note: when removing the IsBuildingFactorAvailable flag after it expires and we do code cleanup, keep the LobType check.
                                text = "New Home"
                            End If
                        End If
                    Case "deductible factor"
                        If ForPrintSummary Then
                            text = "Deductible"
                        Else
                            text = "Deductible"
                        End If
                    Case "acv roof"
                        If ForPrintSummary Then
                            text = "ACV Roof"
                        Else
                            text = "ACV Roof"
                        End If
                    Case "auto/home discount", "auto/home credit factor" 'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                        If ForPrintSummary Then
                            text = "Auto/Home"
                        Else
                            text = "Auto/Home"
                        End If
                    Case "experience rating factor"
                        If ForPrintSummary Then
                            text = "Longevity Discount"
                        Else
                            text = "Longevity Discount"
                        End If
                    Case "mature homeowner credit", "mature homeowner discount factor" 'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                        If ForPrintSummary Then
                            text = "Mature Homeowner"
                        Else
                            text = "Mature Homeowner"
                        End If
                    Case "alarm credit"
                        If ForPrintSummary Then
                            text = "Alarms Credits"
                        Else
                            text = "Alarms Credits"
                        End If
                    Case "protective device factor"
                        If ForPrintSummary Then
                            text = "Alarms Credits"
                        Else
                            text = "Alarms Credits"
                        End If
                    Case "tie down factor", "mobile home tie down factor" 'Updated 7/3/18 for HOM2011 Upgrade post go-live changes MLW
                        If ForPrintSummary Then
                            text = "Tie Down"
                        Else
                            text = "Tie Down"
                        End If
                    Case "select market credit", "select market credit factor" 'Updated 7/2/18 for HOM2011 Upgrade post go-live changes MLW - added select market credit factor
                        If ForPrintSummary Then
                            text = "Select Market Credit"
                        Else
                            text = "Select Market Credit"
                        End If
                    Case "hobby farm"
                        If ForPrintSummary Then
                            text = "Hobby Farm"
                        Else
                            text = "Hobby Farm"
                        End If
                    Case "pay plan option" 'Added 6/18/2019 for Bug 31002 MLW
                        text = "Pay Plan Discount"
                    Case "mortgage free discount"
                        If ForPrintSummary Then
                            text = "Mortgage-Free Discount"
                        Else
                            text = "Mortgage-Free Discount"
                        End If
                End Select

                Dim value As Double = 0.0
                Double.TryParse(f.Substring(f.LastIndexOf(" "), f.Length - f.LastIndexOf(" ")), value)
                If value <> 0 Then

                    If text = "Mortgage-Free Discount" AndAlso value = 1 Then
                        Continue For
                    End If
                    'Updated 6/18/2019 for Bug 31002 MLW
                    If text <> "Pay Plan Discount" Then
                        value = Math.Round((1 - value) * 100, 0, MidpointRounding.AwayFromZero)
                    End If

                    Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass 'Added 7/2/18 for HOM2011 Upgrade post go-live changes MLW
                    If value > 0.0 Then ' if less than 0 it is a surcharge not a discount
                        'Updated 7/2/18 for HOM2011 Upgrade post go-live changes MLW
                        If qqh.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                            If text = "Alarms Credits" Then
                                factorCredits.Add(New KeyValuePair(Of String, String)(text, value.ToString() + "%"))
                            Else
                                factorCredits.Add(New KeyValuePair(Of String, String)(text, ""))
                            End If
                        Else
                            'Updated 6/18/2019 for Bug 31002 MLW
                            If topQuote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                                factorCredits.Add(New KeyValuePair(Of String, String)(text, value.ToString()))
                            Else
                                factorCredits.Add(New KeyValuePair(Of String, String)(text, value.ToString() + "%"))
                            End If
                        End If
                        'factorCredits.Add(New KeyValuePair(Of String, String)(text, value.ToString() + "%"))
                    End If
                End If
            Next
            Return factorCredits
        End Function

    End Class

End Namespace