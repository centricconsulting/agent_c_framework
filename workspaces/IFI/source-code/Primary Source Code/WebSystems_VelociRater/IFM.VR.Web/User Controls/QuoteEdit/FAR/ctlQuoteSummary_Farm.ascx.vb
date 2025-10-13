Imports System.Globalization
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM
Imports MigraDoc.DocumentObjectModel.Tables

Public Class ctlQuoteSummary_Farm
    Inherits VRControlBase

#Region "Declarations"
    Public Event RequestNavToIRPM()

    Private Const ClassName As String = "ctlQuoteSummary_Farm"
    Private HasIncedentalFarmOrFarmOwnder = False
    Private indentStyle As String = "padding-left:20px;"
    Private NumFormatNoCents As String = "###,###,##0"  ' No dollar sign 9/16/15 MGB
    Private NumFormatNoCentsWithDollarSign = "$###,###,##0"
    'Private NumFormatNoCents As String = "$###,###,##0"
    Private NumFormatWithCents As String = "$###,###,###.00"
    Private HasPrimaryDwelling = False
    Private CommercialPolicy As Boolean = False

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
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3FarmApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    Private ReadOnly Property SuffocationAndCustomFeedingCutoffDate As DateTime
        Get
            Return CDate("7/1/2020")
        End Get
    End Property

#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Try
            Me.VRScript.CreateAccordion("divFarmQuoteSummary", HiddenField1, "0")
            Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
            Me.Page.MaintainScrollPositionOnPostBack = False
            Me.UnlockTree()
            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "AddScriptWhenRendered", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    ''' <summary>
    ''' The main Populate function - populates the entire form
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Populate()
        Dim FO As String = "6"
        Dim SOM As String = "7"
        Dim FL As String = "8"
        Dim pgm As String = ""

        Try
            lblMsg.Text = "&nbsp;"
            If Me.Quote IsNot Nothing Then
                If Me.MyLocation IsNot Nothing Then

                    ' Determine if this is a commercial policy
                    If (Quote.Policyholder.Name.CommercialName1 IsNot Nothing AndAlso Quote.Policyholder.Name.CommercialName1 <> String.Empty) _
                        OrElse (Quote.Policyholder.Name.CommercialName1 IsNot Nothing AndAlso Quote.Policyholder.Name.CommercialName1 <> String.Empty) Then
                        CommercialPolicy = True
                    Else
                        CommercialPolicy = False
                    End If

                    ' Header Info
                    'If IsOnAppPage Then
                    '    Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                    'Else
                    '    Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                    'End If
                    'updated 5/9/2019; logic taken from updates for PPA
                    Select Case Me.Quote.QuoteTransactionType
                        Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                            Me.lblMainAccord.Text = If(Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId) & " Premium"
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
                            Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                            Me.ImageDateAndPremChangeLine.Visible = False
                    End Select

                    Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                    ' General Info Section
                    If Me.Quote.Policyholder.Name.TypeId = "1" Then
                        Me.lblPhName.Text = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.MiddleName, Me.Quote.Policyholder.Name.LastName, Me.Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                    Else
                        Me.lblPhName.Text = Me.Quote.Policyholder.Name.CommercialDBAname
                    End If

                    Dim AddressOtherField As AddressOtherField = New AddressOtherField(Me.Quote.Policyholder.Address.Other)
                    If AddressOtherField.PrefixType = Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Other Then
                        Me.lblCareOf.Text = ""
                        Me.trCareOf.Visible = False
                    Else
                        Me.lblCareOf.Text = AddressOtherField.NameWithPrefix
                        Me.trCareOf.Visible = True
                    End If

                    'Me.lblQuoteNum.Text = Me.Quote.QuoteNumber
                    ' CAH - Update for Endorsements Quote/Policy Number
                    Select Case Quote.QuoteTransactionType
                        Case QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                            If String.IsNullOrWhiteSpace(Quote.PolicyNumber) = False AndAlso Left(UCase(Quote.PolicyNumber), 1) = "Q" Then
                                Me.lblQuoteNum.Text = Quote.QuoteNumber
                            Else
                                Me.lblQuoteNum.Text = Quote.PolicyNumber
                                If String.IsNullOrWhiteSpace(Me.lblQuoteNo.Text) = False AndAlso Len(Me.lblQuoteNo.Text) >= 5 AndAlso Left(Me.lblQuoteNo.Text, 5) = "Quote" Then
                                    Me.lblQuoteNo.Text = Me.lblQuoteNo.Text.Replace("Quote", "Policy")
                                End If
                            End If
                        Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                            Me.lblQuoteNum.Text = Quote.PolicyNumber
                            If String.IsNullOrWhiteSpace(Me.lblQuoteNo.Text) = False AndAlso Len(Me.lblQuoteNo.Text) >= 5 AndAlso Left(Me.lblQuoteNo.Text, 5) = "Quote" Then
                                Me.lblQuoteNo.Text = Me.lblQuoteNo.Text.Replace("Quote", "Policy")
                            End If
                        Case Else
                            Me.lblQuoteNum.Text = Me.Quote.QuoteNumber
                    End Select

                    Dim zip As String = Me.Quote.Policyholder.Address.Zip
                    If zip.Length > 5 Then
                        zip = zip.Substring(0, 5)
                    End If

                    'house num, street, apt, pobox, city, state, zip
                    Me.lblPhAddress.Text = String.Format("{0} {1} {2} {3} {4} {5} {6}", Me.Quote.Policyholder.Address.HouseNum, Me.Quote.Policyholder.Address.StreetName, If(String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Address.ApartmentNumber) = False, "Apt# " + Me.Quote.Policyholder.Address.ApartmentNumber, ""), Me.Quote.Policyholder.Address.POBox, Me.Quote.Policyholder.Address.City, Me.Quote.Policyholder.Address.State, zip).Replace("  ", " ").Trim()

                    Me.lblEffectiveDate.Text = Me.Quote.EffectiveDate
                    Me.lblExpirationDate.Text = Me.Quote.ExpirationDate

                    Me.lblFullPremium.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                    Me.lblRatedTier.Text = If(Me.Quote.TieringInformation IsNot Nothing, Me.Quote.TieringInformation.RatedTier, "")

                    ' Applicants
                    PopulateApplicants()

                    ' L & M
                    PopulateLandM()

                    ' Location Summary (APPLICATION SUMMARY ONLY) - ALL Program Types
                    If Me.IsOnAppPage Then PopulateLocationSummary()

                    'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
                    If SubQuoteFirst IsNot Nothing Then
                        pgm = SubQuoteFirst.ProgramTypeId

                        ' Primary and Additonal Dwellings - Farmowners only
                        ' REMOVED FL PER BUG 5604 MGB 9/21/15
                        If SubQuoteFirst.ProgramTypeId = FO Then
                            'If Quote.ProgramTypeId = FO OrElse Quote.ProgramTypeId = FL Then
                            ' Primary Dwelling
                            PopulatePrimaryDwelling()
                            If HasPrimaryDwelling Then
                                divPrimaryDwellingCoverages.Visible = True
                                Me.tblCoverageSummary.Text = PopulateDwellingCoverageSummary(0)
                            Else
                                divPrimaryDwellingCoverages.Visible = False
                            End If

                            ' Additional Dwellings
                            PopulateAdditionalDwellings()

                            divPrimaryDwelling.Visible = True
                            ' divAdditionalDwellings.Visible = True ' I set this in PopulateAdditionalDwellings
                        Else
                            divPrimaryDwelling.Visible = False
                            divPrimaryDwellingCoverages.Visible = False
                            divAdditionalDwellings.Visible = False
                        End If

                        ' Barns and Buildings - Farmowners and SOM programs
                        If SubQuoteFirst.ProgramTypeId = FO OrElse SubQuoteFirst.ProgramTypeId = SOM Then
                            PopulateBarnsAndBuildings()
                            divBarnsAndBuildings.Visible = True
                        Else
                            divBarnsAndBuildings.Visible = False
                        End If

                        ' Farm Personal Property - Farmowners and SOM programs
                        If SubQuoteFirst.ProgramTypeId = FO OrElse SubQuoteFirst.ProgramTypeId = SOM Then
                            PopulateFarmPersonalProperty()
                            divFarmPersonalProperty.Visible = True
                        Else
                            divFarmPersonalProperty.Visible = False
                        End If

                        ' Farm Incidental Limits - Farmowners and SOM programs
                        divFarmIncidentalLimits.Visible = False
                        If Common.Helpers.FARM.GlassBreakageForCabs.IsGlassBreakageForCabsAvailable(Quote) Then
                            If SubQuoteFirst.ProgramTypeId = FO OrElse SubQuoteFirst.ProgramTypeId = SOM Then
                                PopulateFarmIncidentalLimits()
                                divFarmIncidentalLimits.Visible = True
                            Else
                                divFarmIncidentalLimits.Visible = False
                            End If
                        End If

                        ' Inland Marine & RV/Watercraft - updated to allow commercial FO and SOM to show IM & RV/WC
                        ' Bug 51653
                        If SubQuoteFirst.ProgramTypeId = FO OrElse SubQuoteFirst.ProgramTypeId = SOM Then
                            ' Show IM
                            PopulateInlandMarine()
                            divInlandMarine.Visible = True
                            ' Show RV/WC
                            PopulateRvWaterCraft()
                            divRVWatercraft.Visible = True
                        Else
                            ' not eligble - Hide IM & RV/WC
                            divInlandMarine.Visible = False
                            divRVWatercraft.Visible = False
                        End If
                    End If

                    ' Additional Coverages - All programs
                    PopulateAdditionalCoverages()

                    ' Credits and Surcharges - All programs
                    'If IsQuoteEndorsement = false Then
                    PopulateCredits()
                    PopulateSurcharges()
                    '    creditsAndSurchargesSection.Visible = True
                    'Else
                    '    creditsAndSurchargesSection.Visible = False
                    'End If

                    Me.ctlQuoteSummaryActions.Populate()
                Else
                    Me.ctlQuoteSummaryActions.Visible = False
                    Throw New Exception("Location is NOTHING!!")
                End If

            End If

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "Populate", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String)
        Try
            sb.AppendLine("<td>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "WriteCell", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String)
        Try
            sb.AppendLine("<td style=""" + styleText + """>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "WriteCell", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String, cssclass As String)
        Try
            sb.AppendLine("<td style=""" + styleText + """ class=""" + cssclass + """>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "WriteCell", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    'Added 10/25/2022 for bug 76012 MLW
    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String, cssclass As String, hoverText As String)
        Try
            sb.AppendLine("<td style=""" + styleText + """ class=""" + cssclass + """>")
            sb.AppendLine("<span title=""" + hoverText + """>" + cellText + "</span>")
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "WriteCell", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    'Added 10/25/2022 for bug 76012 MLW
    Private Sub WriteCellTwoColumns(sb As StringBuilder, cellText As String)
        Try
            sb.AppendLine("<td colspan='2'>")
            sb.AppendLine(cellText)
            sb.AppendLine("</td>")

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "WriteCell", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate the Applicants section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateApplicants()
        Dim html As New StringBuilder()
        Dim address As String = Nothing
        Dim ndx As Integer = -1

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Name")
            WriteCell(html, "SSN")
            WriteCell(html, "Birth Date")
            WriteCell(html, "Relationship")

            html.AppendLine("</tr>")

            Dim opCovList = {(New With {.Name = "", .SSN = "", .MaritalStatus = "", .BirthDate = "", .Occupation = "", .Employer = "", .Relationship = "", .address = ""})}.Take(0).ToList()

            'Updated 9/6/18 for multi state MLW - Quote to GoverningStateQuote
            'If Me.Quote IsNot Nothing AndAlso Me.Quote.Applicants IsNot Nothing Then
            'For Each applicant In Me.Quote.Applicants
            If Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants IsNot Nothing Then
                For Each applicant In Me.GoverningStateQuote.Applicants
                    ndx += 1
                    Dim zip As String = applicant.Address.Zip
                    If zip.Length > 5 Then
                        zip = zip.Substring(0, 5)
                    End If

                    If ndx > 0 Then
                        ' If the second and subsequent applicants don't have any address info, use the first applicant's address (already stored in 'address' variable)
                        If applicant.Address.HouseNum.Trim <> String.Empty AndAlso applicant.Address.City.Trim <> String.Empty Then
                            address = String.Format("{0} {1} {2} {3} {4} {5} {6}", applicant.Address.HouseNum, applicant.Address.StreetName, If(String.IsNullOrWhiteSpace(applicant.Address.ApartmentNumber) = False, "Apt# " + applicant.Address.ApartmentNumber, ""), applicant.Address.POBox, applicant.Address.City, applicant.Address.State, zip).Replace("  ", " ").Trim()
                        End If
                    Else
                        address = String.Format("{0} {1} {2} {3} {4} {5} {6}", applicant.Address.HouseNum, applicant.Address.StreetName, If(String.IsNullOrWhiteSpace(applicant.Address.ApartmentNumber) = False, "Apt# " + applicant.Address.ApartmentNumber, ""), applicant.Address.POBox, applicant.Address.City, applicant.Address.State, zip).Replace("  ", " ").Trim()
                    End If
                    Dim Name As String = String.Format("{0} {1} {2} {3}", applicant.Name.FirstName, applicant.Name.MiddleName, applicant.Name.LastName, applicant.Name.SuffixName).Replace("  ", " ").Trim()
                    'Dim Name As String = String.Format("{0} {1} {2} {3}", Quote.Policyholder.Name.FirstName, Quote.Policyholder.Name.MiddleName, Quote.Policyholder.Name.LastName, Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                    Dim maritalStatus As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.MaritalStatusId, applicant.Name.MaritalStatusId)
                    Dim occupation As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteApplicant, QuickQuoteHelperClass.QuickQuotePropertyName.OccupationTypeId, applicant.OccupationTypeId)
                    Dim relationship As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteApplicant, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, applicant.RelationshipTypeId)

                    opCovList.Add(New With {.Name = Name, .SSN = applicant.Name.TaxNumber_Hyphens, .MaritalStatus = maritalStatus,
                                            .BirthDate = applicant.Name.BirthDate, .Occupation = occupation, .Employer = applicant.Employer, .Relationship = relationship, .address = address})
                Next

                For Each c In opCovList
                    html.AppendLine("<tr>")

                    ' Line 1
                    WriteCell(html, c.Name)
                    WriteCell(html, c.SSN)
                    WriteCell(html, c.BirthDate)
                    WriteCell(html, c.Relationship)
                    html.AppendLine("</tr>")

                    ' Line 2
                    html.AppendLine("<tr>")
                    html.AppendLine("<td colspan=" & """" & "4" & """" & ">")
                    html.AppendLine("&nbsp;&nbsp;" & c.address)
                    html.AppendLine("</td>")
                    html.AppendLine("</tr>")
                Next

            End If

            html.AppendLine("</table>")

            Me.tblApplicants.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateApplicants", ex, lblMsg)
            Exit Sub
        End Try

    End Sub

    ''' <summary>
    ''' Populate the Liability and Med pay section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateLandM()
        Dim html As New StringBuilder()
        Dim ndx As Integer = -1
        Dim cov As String = ""
        Dim lim As String = ""
        Dim prem As String = ""
        Dim LMTotal As Decimal = 0

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Coverage", "width: 300px;text-align:left;")
            WriteCell(html, "Limit", "width: 100px;text-align:right;")
            WriteCell(html, "Premium", "width: 230px;text-align:right;")
            html.AppendLine("</tr>")

            Dim LMCovList = {(New With {.Coverage = "", .Limit = "", .Premium = ""})}.Take(0).ToList()

            If Me.Quote IsNot Nothing Then
                'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst, totalling premiums
                If SubQuoteFirst IsNot Nothing Then
                    ' Occurrence Liability
                    cov = "L. Personal Liability"
                    If FieldHasNumericValue(SubQuoteFirst.OccurrenceLiabilityLimitId, ClassName, lblMsg, False) Then
                        lim = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.OccurrenceLiabilityLimitId, SubQuoteFirst.OccurrenceLiabilityLimitId)
                        If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then
                            lim = Format(CDec(lim), NumFormatNoCents)
                        Else
                            lim = ""
                        End If
                    Else
                        lim = ""
                    End If
                    If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Locations_Farm_L_Liability_QuotedPremium), ClassName, lblMsg, True) Then
                        prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Locations_Farm_L_Liability_QuotedPremium)), NumFormatWithCents)
                        LMTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Locations_Farm_L_Liability_QuotedPremium))
                    Else
                        prem = "Included"
                    End If
                    LMCovList.Add(New With {.Coverage = cov, .Limit = lim, .Premium = prem})

                    ' Med Pay
                    cov = "M. Medical Payments"
                    If FieldHasNumericValue(SubQuoteFirst.MedicalPaymentsLimitid, ClassName, lblMsg, False) Then
                        lim = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, SubQuoteFirst.MedicalPaymentsLimitid)
                        If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then
                            lim = Format(CDec(lim), NumFormatNoCents)
                        Else
                            lim = ""
                        End If
                    Else
                        lim = ""
                    End If
                    'Updated 9/6/18 for multi state MLW
                    'If FieldHasNumericValue(Quote.Locations_Farm_M_Medical_Payments_QuotedPremium, ClassName, lblMsg, True) Then
                    '    prem = Format(CDec(Quote.Locations_Farm_M_Medical_Payments_QuotedPremium), NumFormatWithCents)
                    '    LMTotal += CDec(Quote.Locations_Farm_M_Medical_Payments_QuotedPremium)
                    'Else
                    '    prem = "Included"
                    'End If
                    If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Locations_Farm_M_Medical_Payments_QuotedPremium), ClassName, lblMsg, True) Then
                        prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Locations_Farm_M_Medical_Payments_QuotedPremium)), NumFormatWithCents)
                        LMTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Locations_Farm_M_Medical_Payments_QuotedPremium))
                    Else
                        prem = "Included"
                    End If
                    LMCovList.Add(New With {.Coverage = cov, .Limit = lim, .Premium = prem})

                    ' Stop Gap
                    Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                    If FieldHasNumericValue(gsQuote.StopGapLimitId, ClassName, lblMsg, False) Then
                        cov = "Stop Gap (OH)"
                        lim = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.StopGapLimitId, gsQuote.StopGapLimitId)
                        If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.StopGapQuotedPremium), ClassName, lblMsg, True) Then
                            prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.StopGapQuotedPremium)), NumFormatWithCents)
                            LMTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.StopGapQuotedPremium))
                        Else
                            prem = "Included"
                        End If
                        LMCovList.Add(New With {.Coverage = cov, .Limit = lim, .Premium = prem})
                    End If

                    ' Display the data
                    For Each c In LMCovList
                        html.AppendLine("<tr>")
                        WriteCell(html, c.Coverage)
                        WriteCell(html, c.Limit, "text-align:right;")
                        WriteCell(html, c.Premium, "text-align:right;")
                        html.AppendLine("</tr>")
                    Next
                End If

                ' Total Premium Line
                html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                html.AppendLine("<td colspan=""2"" style=""font-weight=bold;"">Total Liability Premium</td>")
                html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", LMTotal) + "</td>")
                html.AppendLine("</tr>")
            End If

            html.AppendLine("</table>")

            Me.tblLandM.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateLandM", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populates the Location Summary section (APPLICATION SUMMARYY ONLY)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateLocationSummary()
        Dim html As New StringBuilder()
        Dim num As String = Nothing
        Dim address As String = Nothing
        Dim acreage As String = Nothing
        Dim LocType As String = Nothing
        Dim tblLocs = {(New With {.num = "", .Address = "", .Acreage = "", .Location = ""})}.Take(0).ToList()

        Try
            If Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 Then Exit Sub

            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "#", "width: 30px;text-align:center;")
            WriteCell(html, "Address", "width: 350px;text-align:left;")
            WriteCell(html, "Acreage", "width: 75px;text-align:center;")
            WriteCell(html, "Location Type", "width: 175px;text-align:left;")
            html.AppendLine("</tr>")

            ' Build the table
            For Each Loc As QuickQuoteLocation In Quote.Locations
                address = FormatLocationAddress(Loc)
                If Loc.Acreages IsNot Nothing AndAlso Loc.Acreages.Count > 0 Then
                    For Each acr As QuickQuoteAcreage In Loc.Acreages
                        acreage = acr.Acreage
                        LocType = GetLocationAcreageTypeDescription(acr.LocationAcreageTypeId)
                        If LocType.ToUpper = "ACREAGE ONLY" Then
                            num = "1"
                        ElseIf LocType.ToUpper = "BLANKET ACREAGE" Then
                            num = ""
                        Else
                            num = acr.AcreageNum
                        End If
                        tblLocs.Add(New With {.num = num, .Address = address, .Acreage = acreage, .Location = LocType})
                    Next
                End If
            Next

            For Each c In tblLocs
                ' data lines
                html.AppendLine("<tr>")

                If c.Location.ToUpper = "BLANKET ACREAGE" Then
                    c.Address = ""
                    c.num = ""
                End If
                WriteCell(html, c.num, "text-align:center;")
                WriteCell(html, c.Address, "text-align:left;")
                WriteCell(html, c.Acreage, "text-align:center;")
                WriteCell(html, c.Location, "text-align:left;")
                html.AppendLine("</tr>")
            Next

            html.AppendLine("</table>")

            Me.tblLocationSummary.Text = html.ToString()
            Me.divLocationSummary.Visible = True

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateLocationSummary", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate Primary Dwelling section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulatePrimaryDwelling()
        Dim html As New StringBuilder()
        Dim terr As String = ""
        Dim txt As String = ""

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header Row 1
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            html.AppendLine("<td colspan=" & """" & "3" & """" & ">")
            html.AppendLine("Address")
            html.AppendLine("</td>")
            html.AppendLine("<td colspan=" & """" & "2" & """" & ">")
            html.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;Deductible")
            html.AppendLine("</td>")
            html.AppendLine("</tr>")

            ' Header Row 2
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;Construction", "width: 120px;text-align:left;")
            WriteCell(html, "Year Built", "width: 80px;text-align:left;")
            WriteCell(html, "SqFt", "width: 80px;text-align:left;")
            WriteCell(html, "Structure", "width: 280px;text-align:left;")
            'WriteCell(html, "Premium", "text-align:right;", "qs_Grid_cell_premium")
            WriteCell(html, "", "text-align:right;", "qs_Grid_cell_premium")
            html.AppendLine("</tr>")

            Dim opCovList = {(New With {.Address = "", .Construction = "", .Year = "", .SqrFeet = "", .Structure = "", .Deductible = "", .Territory = "", .ProtectionClass = "", .Premium = "", .terr = ""})}.Take(0).ToList()

            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing Then
                Dim index As Int32 = 0
                For Each l In Me.Quote.Locations
                    If FieldHasNumericValue(l.DwellingTypeId, ClassName, lblMsg, True) Then
                        HasPrimaryDwelling = True

                        'Dim address As String = String.Format("{0} {1} {2} {3} {4} {5} {6}", l.Address.HouseNum, l.Address.StreetName, If(String.IsNullOrWhiteSpace(l.Address.ApartmentNumber) = False, "Apt# " + l.Address.ApartmentNumber, ""), l.Address.POBox, l.Address.City, l.Address.State, zip).Replace("  ", " ").Trim()
                        Dim address As String = FormatLocationAddress(l)
                        Dim construction As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, l.ConstructionTypeId)
                        Dim structureType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.StructureTypeId, l.StructureTypeId)
                        If l.TerritoryNumber IsNot Nothing AndAlso l.TerritoryNumber <> String.Empty Then terr = l.TerritoryNumber

                        Dim deductibleLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, l.DeductibleLimitId)
                        If IsNumeric(deductibleLimit) Then
                            deductibleLimit = Format(CDec(deductibleLimit), NumFormatNoCents)
                        End If

                        opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
                                        .Structure = structureType, .Deductible = deductibleLimit, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = "", .terr = l.TerritoryNumber})
                        'opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
                        '                .Structure = structureType, .Deductible = deductibleLimit, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = l.PremiumFullterm, .terr = l.TerritoryNumber})
                        index += 1
                    Else
                        ' No primary Dwelling
                        HasPrimaryDwelling = False
                        html.AppendLine("<tr>")
                        html.AppendLine("<td colspan=" & """" & "4" & """" & ">")
                        '                html.AppendLine("<td colspan=" & """" & "5" & """" & " style=" & """" & "border-top:solid 1px;" & """" & ">")
                        html.AppendLine("N/A")
                        html.AppendLine("</td>")
                        html.AppendLine("</tr>")
                    End If
                    Exit For ' only want first location
                Next

                For Each c In opCovList
                    ' data line 1 - Address, Deductible, territory
                    ' Note that territory is embedded in the data line for some reason
                    html.AppendLine("<tr>")
                    html.AppendLine("<td colspan=" & """" & "3" & """" & ">")
                    html.AppendLine(c.Address)
                    html.AppendLine("</td>")
                    html.AppendLine("<td colspan=" & """" & "2" & """" & ">")
                    txt = "&nbsp;&nbsp;&nbsp;&nbsp;" & c.Deductible
                    If c.terr <> String.Empty Then
                        txt += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" & "Territory " & c.terr
                    End If
                    html.AppendLine(txt)
                    html.AppendLine("</td>")
                    html.AppendLine("</tr>")

                    ' data line 2 - Construction, Year Built, SqFt, Structure, Premium
                    html.AppendLine("<tr>")
                    WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;" & c.Construction, "text-align:left;")
                    WriteCell(html, c.Year, "text-align:left;")
                    WriteCell(html, c.SqrFeet, "text-align:left;")
                    WriteCell(html, c.Structure, "text-align:left;")
                    WriteCell(html, c.Premium, "text-align:right;")
                    html.AppendLine("</tr>")
                Next

            End If

            html.AppendLine("</table>")

            Me.tblPrimaryDwelling.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulatePrimaryDwelling", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate the Additional Dwellings section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateAdditionalDwellings()
        Dim html As New StringBuilder()
        Dim terr As String = ""
        Dim txt As String = ""
        Dim count As String = 0

        Try
            Dim opCovList = {(New With {.Address = "", .Construction = "", .Year = "", .SqrFeet = "", .Structure = "", .Deductible = "", .Territory = "", .ProtectionClass = "", .Premium = "", .terr = ""})}.Take(0).ToList()

            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing Then
                Dim index As Int32 = 0
                For Each l In Me.Quote.Locations
                    ' Location 0 is primary dwelling, skip it
                    If index = 0 Then GoTo Skip
                    ' If the Dwelling Type is not > 0 then this is not a dwelling 'or FormType CAH 20201214
                    If l.DwellingTypeId Is Nothing OrElse l.DwellingTypeId = String.Empty OrElse l.DwellingTypeId = "0" OrElse l.FormTypeId = "" Then GoTo Skip
                    'If l.DwellingTypeId Is Nothing OrElse l.DwellingTypeId = String.Empty OrElse l.DwellingTypeId = "0" OrElse l.FormTypeId <> "13" OrElse l.FormTypeId <> "" Then GoTo Skip

                    ' Count the number of additional dwellings
                    divAdditionalDwellings.Visible = True
                    count += 1

                    'html = New StringBuilder()

                    'html.AppendLine("<table class=""hom_qa_table_shades"">")

                    '' Header Row 1
                    'html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
                    'html.AppendLine("<td colspan=" & """" & "3" & """" & ">")
                    'html.AppendLine("Address")
                    'html.AppendLine("</td>")
                    'html.AppendLine("<td colspan=" & """" & "2" & """" & ">")
                    'html.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;Deductible")
                    'html.AppendLine("</td>")
                    'html.AppendLine("</tr>")

                    '' Header Row 2
                    'html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
                    'WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;Construction", "width: 120px;text-align:left;")
                    'WriteCell(html, "Year Built", "width: 80px;text-align:left;")
                    'WriteCell(html, "SqFt", "width: 80px;text-align:left;")
                    'WriteCell(html, "Structure", "width: 280px;text-align:left;")
                    'WriteCell(html, "Premium", "text-align:right;", "qs_Grid_cell_premium")
                    'html.AppendLine("</tr>")

                    Dim address As String = FormatLocationAddress(l)
                    'Dim address As String = String.Format("{0} {1} {2} {3} {4} {5} {6}", l.Address.HouseNum, l.Address.StreetName, If(String.IsNullOrWhiteSpace(l.Address.ApartmentNumber) = False, "Apt# " + l.Address.ApartmentNumber, ""), l.Address.POBox, l.Address.City, l.Address.State, zip).Replace("  ", " ").Trim()
                    Dim construction As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, l.ConstructionTypeId)
                    Dim structureType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.StructureTypeId, l.StructureTypeId)
                    If l.TerritoryNumber IsNot Nothing AndAlso l.TerritoryNumber <> String.Empty Then terr = l.TerritoryNumber

                    Dim deductibleLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, l.DeductibleLimitId)
                    If IsNumeric(deductibleLimit) Then
                        deductibleLimit = Format(CDec(deductibleLimit), NumFormatNoCents)
                    End If

                    'opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
                    '                .Structure = structureType, .Deductible = deductibleLimit, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = l.PremiumFullterm, .terr = l.TerritoryNumber})
                    opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
                                    .Structure = structureType, .Deductible = deductibleLimit, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = "", .terr = l.TerritoryNumber})
Skip:
                    index += 1
                Next

                html = New StringBuilder()
                html.AppendLine("<table class=""hom_qa_table_shades"">")

                index = 0
                For Each c In opCovList
                    index += 1

                    ' Put a space between dwellings
                    'If index <> 1 Then html.AppendLine("<tr><td colspan=""5"">&nbsp;</td></tr>")

                    ' Header Row 1
                    html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
                    html.AppendLine("<td colspan=" & """" & "3" & """" & ">")
                    html.AppendLine("Address")
                    html.AppendLine("</td>")
                    html.AppendLine("<td colspan=" & """" & "2" & """" & ">")
                    html.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;Deductible")
                    html.AppendLine("</td>")
                    html.AppendLine("</tr>")

                    ' Header Row 2
                    html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
                    WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;Construction", "width: 120px;text-align:left;")
                    WriteCell(html, "Year Built", "width: 80px;text-align:left;")
                    WriteCell(html, "SqFt", "width: 80px;text-align:left;")
                    WriteCell(html, "Structure", "width: 280px;text-align:left;")
                    WriteCell(html, "", "text-align:right;", "qs_Grid_cell_premium")
                    html.AppendLine("</tr>")

                    ' data line 1 - Address, Deductible, territory
                    ' Note that territory is embedded in the data line for some reason

                    html.AppendLine("<tr>")
                    html.AppendLine("<td colspan=" & """" & "3" & """" & ">")
                    html.AppendLine(c.Address)
                    html.AppendLine("</td>")
                    html.AppendLine("<td colspan=" & """" & "2" & """" & ">")
                    txt = "&nbsp;&nbsp;&nbsp;&nbsp;" & c.Deductible
                    If c.terr <> String.Empty Then
                        txt += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" & "Territory " & c.terr
                    End If
                    html.AppendLine(txt)
                    html.AppendLine("</td>")
                    html.AppendLine("</tr>")

                    ' data line 2 - Construction, Year Built, SqFt, Structure, Premium
                    html.AppendLine("<tr>")
                    WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;" & c.Construction, "text-align:left;")
                    WriteCell(html, c.Year, "text-align:left;")
                    WriteCell(html, c.SqrFeet, "text-align:left;")
                    WriteCell(html, c.Structure, "text-align:left;")
                    WriteCell(html, c.Premium, "text-align:right;")
                    html.AppendLine("</tr>")

                    ' This row will hold the coverages table
                    html.AppendLine("<tr>")
                    html.AppendLine("<td colspan=" & """" & "5" & """" & ">")
                    html.AppendLine("<div>")
                    html.AppendLine("<span style=" & """" & "font-weight:700;" & """" & ">Coverages</span>")
                    ' Builds a table of coverages for the current dwelling
                    html.AppendLine(PopulateDwellingCoverageSummary(index))
                    html.AppendLine("</div>")
                    html.AppendLine("</td>")
                    html.AppendLine("</tr>")

                    'html.AppendLine("</table>")
                Next
            End If

            html.AppendLine("</table>")

            If count > 0 Then
                Me.tblAdditionalDwellings.Text = html.ToString()
                divAdditionalDwellings.Visible = True
            Else
                divAdditionalDwellings.Visible = False
            End If

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateAdditionalDwellings", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Return the coverage summary section for Dwellings
    ''' </summary>
    ''' <param name="locationIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function PopulateDwellingCoverageSummary(locationIndex As Int32) As String
        Dim html As New StringBuilder()
        Dim opCovList = {(New With {.Name = "", .Limit = "", .Premium = ""})}.Take(0).ToList()

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

            WriteCell(html, "Coverage")
            WriteCell(html, "Limit", "text-align:right;")
            WriteCell(html, "Premium", "text-align:right;")

            html.AppendLine("</tr>")

            ' MGB - GET CORRECT LOCATION FOR PASSED INDEX - DON'T USE MYLOCATION
            Dim filteredLoc As List(Of QuickQuoteLocation) = Quote.Locations.FindAll(Function(p) p.FormTypeId <> "13" And p.FormTypeId <> "")
            Dim loc As QuickQuoteLocation = filteredLoc(locationIndex)
            Dim quoteForLoc As QuickQuoteObject = Me.SubQuoteForLocation(loc) 'Added 9/11/18 for multi state MLW

            ' COVERAGES A-F

            ' A
            ' do not show on HO-4, ML-4 or FO-4
            If loc.FormTypeId <> "4" AndAlso loc.FormTypeId <> "7" AndAlso loc.FormTypeId <> "17" Then
                Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.A_Dwelling_LimitIncluded)
                Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.A_Dwelling_LimitIncreased)
                Dim lim As String = (A_included + A_increased).ToString
                Dim prem As String = ""
                If FieldHasNumericValue(loc.A_Dwelling_QuotedPremium, ClassName, lblMsg, True) Then
                    prem = Format(CDec(loc.A_Dwelling_QuotedPremium), NumFormatWithCents)
                Else
                    prem = "Included"
                End If

                If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                'opCovList.Add(New With {.Name = "A. Dwelling", .Limit = String.Format("{0:N0} <sup>1</sup>", A_included + A_increased), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.A_Dwelling_QuotedPremium) > 0, loc.A_Dwelling_QuotedPremium, "Included")})
                opCovList.Add(New With {.Name = "A. Dwelling", .Limit = lim, .Premium = prem})
            End If

            ' B
            ' do not show on HO-4, ML-4, HO-6 or FO-4
            If loc.FormTypeId <> "4" And loc.FormTypeId <> "7" And loc.FormTypeId <> "5" And loc.FormTypeId <> "17" Then

                Dim lim As String = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.B_OtherStructures_LimitIncluded)
                If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                Dim prem As String = "Included"
                opCovList.Add(New With {.Name = "B. Other Structures", .Limit = lim, .Premium = prem})


                '' First we need to find the coverage B increase FO-48 coverage if there is one
                '' (CoverageCode_id = 554)
                'Dim FO48IncreasedAmount As Decimal = 0
                'Dim FO48Prem As Decimal = 0
                'For Each s2Cov As QuickQuoteSectionCoverage In loc.SectionCoverages
                '    If s2Cov.Coverages(0).CoverageCodeId = "554" Then
                '        If FieldHasNumericValue(s2Cov.Coverages(0).ManualLimitIncreased, ClassName, lblMsg, True) Then
                '            FO48IncreasedAmount = Format(CDec(s2Cov.Coverages(0).ManualLimitIncreased), NumFormatNoCents)
                '        End If
                '        If FieldHasNumericValue(s2Cov.Coverages(0).WrittenPremium, ClassName, lblMsg, True) Then
                '            FO48Prem = Format(CDec(s2Cov.Coverages(0).WrittenPremium), NumFormatWithCents)
                '        End If
                '        Exit For
                '    End If
                'Next

                'Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.B_OtherStructures_LimitIncluded)
                'Dim B_Increased As Double = FO48IncreasedAmount
                ''Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.B_OtherStructures_LimitIncreased)
                'Dim lim As String = (B_included + B_Increased).ToString
                ''Dim lim As String = B_included.ToString
                'If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                ''Dim prem As String = "Included"
                'Dim prem As String = String.Empty
                'If FieldHasNumericValue(loc.B_OtherStructures_QuotedPremium, ClassName, lblMsg, True) Then
                '    prem = Format(CDec(loc.B_OtherStructures_QuotedPremium) + FO48Prem, NumFormatWithCents)
                'Else
                '    If FieldHasNumericValue(FO48Prem, ClassName, lblMsg, True) Then
                '        prem = Format(FO48Prem, NumFormatWithCents)
                '    End If
                'End If
                ''opCovList.Add(New With {.Name = "B. Other Structures", .Limit = lim, .Premium = prem})
                'opCovList.Add(New With {.Name = "B. Other Structures", .Limit = lim, .Premium = If(B_Increased <= 0, "Included", loc.B_OtherStructures_QuotedPremium)})
            End If

            Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.C_PersonalProperty_LimitIncluded)
            Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.C_PersonalProperty_LimitIncreased)
            Dim clim As String = (C_included + C_increased).ToString
            If FieldHasNumericValue(clim, ClassName, lblMsg, True) Then clim = Format(CDec(clim), NumFormatNoCents)

            If loc.FormTypeId <> "4" Then
                opCovList.Add(New With {.Name = "C. Personal Property", .Limit = clim, .Premium = If(C_increased <= 0, "Included", loc.C_PersonalProperty_QuotedPremium)})
            Else
                'opCovList.Add(New With {.Name = "C. Personal Property", .Limit = String.Format("{0:N0} <sup>1</sup>", C_included + C_increased), .Premium = If(C_increased <= 0, "Included", loc.C_PersonalProperty_QuotedPremium)})
                opCovList.Add(New With {.Name = "C. Personal Property", .Limit = clim, .Premium = If(C_increased <= 0, "Included", loc.C_PersonalProperty_QuotedPremium)})
            End If

            Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.D_LossOfUse_LimitIncluded)
            Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(loc.D_LossOfUse_LimitIncreased)
            Dim dlim As String = (D_included + D_increased).ToString
            If FieldHasNumericValue(dlim, ClassName, lblMsg, True) Then dlim = Format(CDec(dlim), NumFormatNoCents)

            Select Case loc.FormTypeId
                Case "6" ' ml-2
                    opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = dlim, .Premium = If(D_increased <= 0, "Included", loc.D_LossOfUse_QuotedPremium)})
                Case "7" 'ml-4
                    opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = dlim, .Premium = If(D_increased <= 0, "Included", loc.D_LossOfUse_QuotedPremium)})
                Case Else
                    'opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = "(see <sup>2</sup>)", .Premium = If(D_increased <= 0, "Included", loc.D_LossOfUse_QuotedPremium)})
                    opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = dlim, .Premium = If(D_increased <= 0, "Included", loc.D_LossOfUse_QuotedPremium)})
            End Select

            ' MOVE PERSONAL LIABILITY (L) AND MEDICAL PAYMENTS (M) TO THE ADDITIONAL COVERAGES SECTION MGB 9/2/15

            '' NOTE THAT PERSONAL LIABILITY IN FARM IS COVERAGE L WHERE ON HOME IT IS COVERAGE E MGB 8/26/15
            'Dim L_Personal As String = Quote.OccurrenceLiabilityLimit
            'If IsNumeric(L_Personal) Then L_Personal = Format(CDec(L_Personal), NumFormatNoCents)
            'opCovList.Add(New With {.Name = "L. Personal Liability", .Limit = L_Personal, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Quote.PersonalLiabilityQuotedPremium) > 0, Me.Quote.PersonalLiabilityQuotedPremium, "Included")})

            '' NOTE THAT MEDICAL PAYMENTS IN FARM IS COVERAGE M WHERE ON HOME IT IS COVERAGE F MGB 8/26/15
            'Dim M_Medical As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, Me.Quote.MedicalPaymentsLimitid)
            'If IsNumeric(M_Medical) Then M_Medical = Format(CDec(M_Medical), NumFormatNoCents)
            'opCovList.Add(New With {.Name = "M. Medical Payment", .Limit = M_Medical, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Quote.MedicalPaymentsQuotedPremium) > 0, Me.Quote.MedicalPaymentsQuotedPremium, "Included")})

            ' OPTIONAL COVERAGES
            ' Build the coverages data table
            ' DO NOT SHOW FO-48 (CoverageCodeId = 554) -- 10/12/2015 - TLB - Removed code that prevents FO-48 from displaying as a line item
            For Each s2Cov As QuickQuoteSectionCoverage In loc.SectionCoverages
                Select Case s2Cov.Coverages(0).CoverageCodeId
                    'Case "554", "40054", "80094", "80115", "70129", "70201", "70135", "70213", "70139"
                    Case "40054", "80094", "80115", "70129", "70201", "70135", "70213", "70139", "40045", "80553", "80308"
                        ' Don't show any of the above section 2 coverages
                        Exit Select
                    Case "80103"  ' Mine sub
                        Dim lim As String = "N/A"
                        If loc.A_Dwelling_Limit IsNot Nothing AndAlso IsNumeric(loc.A_Dwelling_Limit) Then
                            If CDec(loc.A_Dwelling_Limit) < 300000 Then
                                lim = Format(CDec(loc.A_Dwelling_Limit), NumFormatNoCents)
                            Else
                                lim = "300,000"
                            End If
                        End If
                        Dim prem As String = "Included"
                        If FieldHasNumericValue(s2Cov.Coverages(0).FullTermPremium, ClassName, lblMsg, True) Then prem = Format(CDec(s2Cov.Coverages(0).FullTermPremium), NumFormatWithCents)

                        Dim cov As String = GetCoverageCodeCaption(s2Cov.Coverages(0).CoverageCodeId, stateQuote:=quoteForLoc) 'Updated 9/11/18 with new parameter, stateQuote, for multi state MLW
                        opCovList.Add(New With {.Name = cov, .Limit = lim, .Premium = prem})
                        Exit Select
                    Case Else
                        Dim lim As String = "N/A"

                        If FieldHasNumericValue(s2Cov.Coverages(0).ManualLimitAmount, ClassName, lblMsg, True) OrElse FieldHasNumericValue(s2Cov.Coverages(0).ManualLimitIncreased, ClassName, lblMsg, True) Then
                            If FieldHasNumericValue(s2Cov.Coverages(0).ManualLimitAmount, ClassName, lblMsg, True) Then
                                lim = Format(CDec(s2Cov.Coverages(0).ManualLimitAmount), NumFormatNoCents)
                            Else
                                lim = Format(CDec(s2Cov.Coverages(0).ManualLimitIncreased), NumFormatNoCents)
                            End If
                        End If

                        Dim prem As String = "Included"
                        If FieldHasNumericValue(s2Cov.Coverages(0).FullTermPremium, ClassName, lblMsg, True) Then prem = Format(CDec(s2Cov.Coverages(0).FullTermPremium), NumFormatWithCents)

                        Dim cov As String = GetCoverageCodeCaption(s2Cov.Coverages(0).CoverageCodeId, stateQuote:=quoteForLoc) 'Updated 9/11/18 with new parameter, stateQuote, for multi state MLW
                        opCovList.Add(New With {.Name = cov, .Limit = lim, .Premium = prem})
                        Exit Select
                End Select
            Next

            'Private Power/Light Poles
            If Common.Helpers.FARM.PrivatePowerPolesHelper.IsPrivatePowerPolesAvailable(Quote) Then
                Dim lim As String = Nothing
                Dim prem As String = Nothing
                If loc.IncidentalDwellingCoverages.FindAll(Function(p) p.CoverageCodeId = "70145").Count > 0 Then
                    Dim PrivatePowerPoles As QuickQuoteCoverage
                    PrivatePowerPoles = loc.IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70145")
                    lim = Format(CDec(PrivatePowerPoles.ManualLimitAmount), NumFormatNoCents)
                    If QQHelper.IsPositiveIntegerString(PrivatePowerPoles.ManualLimitIncreased) Then
                        'lim = Format(CDec(PrivatePowerPoles.ManualLimitAmount), NumFormatNoCents)
                        prem = Format(CDec(PrivatePowerPoles.FullTermPremium), NumFormatWithCents)
                    Else
                        prem = "Included"
                    End If
                    opCovList.Add(New With {.Name = "Private Power/Light Poles", .limit = lim, .Premium = prem})
                End If
            End If

            ' Build the coverages html
            For Each c In opCovList
                If String.IsNullOrWhiteSpace(c.Name) Then
                    html.AppendLine("<tr style=""height: 20px;"">")
                Else
                    html.AppendLine("<tr>")
                End If

                WriteCell(html, c.Name)
                WriteCell(html, c.Limit, "text-align:right;")
                WriteCell(html, c.Premium, "text-align:right;")

                html.AppendLine("</tr>")
            Next

            html.AppendLine("</table>")

            Return html.ToString()
        Catch ex As Exception
            HandleError(ClassName, "PopulateDwellingCoverageSummary", ex, lblMsg)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Populate the Barns and Buildings section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateBarnsAndBuildings()
        Dim html As New StringBuilder()
        Dim terr As String = ""
        Dim txt As String = ""
        Dim prem As String = ""

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header Row 1
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            html.AppendLine("<td colspan=" & """" & "6" & """" & ">")
            html.AppendLine("Address")
            html.AppendLine("</td>")
            html.AppendLine("</tr>")

            ' Header Row 2
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

            WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;Building", "width: 340px;text-align:left;")
            WriteCell(html, "Type", "width: 60px;text-align:left;")
            WriteCell(html, "Limit", "width: 60px;text-align:right;")
            WriteCell(html, "Deductible", "width: 60px;text-align:right;")
            WriteCell(html, "Premium", "width:60px;text-align:right;")
            html.AppendLine("</tr>")

            Dim BuildingTable = {(New With {.Address = "", .LocationIndex = "", .Building = "", .BuildingType = "", .Construction = "", .Limit = "", .Deductible = "", .Premium = ""})}.Take(0).ToList()
            Dim CoverageTable = {(New With {.LocationIndex = "", .BuildingIndex = "", .CoverageName = "", .Limit = "", .Premium = "", .LossExt = "", .CoIns = ""})}.Take(0).ToList()



            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing Then
                Dim LocationIndex As Int32 = -1
                For Each Loc As QuickQuoteLocation In Me.Quote.Locations
                    LocationIndex += 1
                    'Updated 9/12/18 for multi state MLW
                    Dim quoteForLoc As QuickQuoteObject = Me.SubQuoteForLocation(Loc) 'Added 9/11/18 for multi state MLW 
                    Dim BuildingIndex As Integer = -1
                    For Each bld As QuickQuoteBuilding In Loc.Buildings
                        BuildingIndex += 1
                        ' Building Info
                        Dim address As String = FormatLocationAddress(Loc)
                        Dim construction As String = GetFarmConstructionTypeDescription(bld.ConstructionId)
                        Dim BuildingType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmTypeId, bld.FarmTypeId)
                        'Dim structureType As String = GetFarmStructureTypeDescription(bld.FarmStructureTypeId)
                        Dim buildingname As String = GetFarmStructureTypeDescription(bld.FarmStructureTypeId)
                        'Added 5/18/18 for Bug 20408 MLW - shows building name and description
                        Dim buildingdesc As String = ""
                        buildingdesc = bld.Description
                        Dim buildingNameAndDesc As String = buildingname
                        If buildingdesc <> "" Then
                            buildingdesc = StrConv(buildingdesc, VbStrConv.ProperCase)
                            buildingNameAndDesc = buildingname & " - " & buildingdesc
                        End If
                        ' Changed deductible to use E deductible 7/21/2015
                        If Loc.TerritoryNumber IsNot Nothing AndAlso Loc.TerritoryNumber <> String.Empty Then terr = Loc.TerritoryNumber
                        Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.E_Farm_DeductibleLimitId, bld.E_Farm_DeductibleLimitId)
                        If IsNumeric(deductible) Then
                            deductible = Format(CDec(deductible), NumFormatNoCents)
                        End If

                        BuildingTable.Add(New With {.Address = address, .LocationIndex = LocationIndex.ToString, .Building = buildingNameAndDesc, .BuildingType = BuildingType, .Construction = construction,
                                        .Limit = Format(CDec(bld.E_Farm_Limit), NumFormatNoCents), .Deductible = deductible, .Premium = bld.E_Farm_QuotedPremium})

                        'added 10/26/2020 (Interoperability)
                        If QQHelper.IsPositiveDecimalString(bld.HouseholdContentsQuotedPremium) = True Then
                            Dim dwellingContentsNameAndDesc As String = buildingname & " Contents"

                            If dwellingContentsNameAndDesc.Contains("Farm Dwelling") OrElse dwellingContentsNameAndDesc.Contains("Mobile Home Dwelling") OrElse dwellingContentsNameAndDesc.Contains("Outbuilding") Then
                                dwellingContentsNameAndDesc = "Household Contents"
                                If String.IsNullOrWhiteSpace(buildingdesc) = False Then
                                    dwellingContentsNameAndDesc &= " - " & buildingdesc
                                End If
                            End If
                            'BuildingTable.Add(New With {.Address = address, .LocationIndex = LocationIndex.ToString, .Building = dwellingContentsNameAndDesc, .BuildingType = BuildingType, .Construction = construction,
                            '            .Limit = Format(CDec(bld.HouseholdContentsLimit), NumFormatNoCents), .Deductible = deductible, .Premium = bld.HouseholdContentsQuotedPremium})
                            BuildingTable.Add(New With {.Address = "", .LocationIndex = LocationIndex.ToString, .Building = dwellingContentsNameAndDesc, .BuildingType = BuildingType, .Construction = construction,
                                        .Limit = Format(CDec(bld.HouseholdContentsLimit), NumFormatNoCents), .Deductible = deductible, .Premium = bld.HouseholdContentsQuotedPremium})
                        End If

                        ' COVERAGES

                        ' (no optionalcoverages in this section)

                        ' OptionalCoverageEs
                        For Each Cov As QuickQuoteOptionalCoverageE In bld.OptionalCoverageEs
                            Dim limit As String = "N/A"


                            'If Loc.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio Then
                            '    Select Case Cov.CoverageCodeId
                            '        Case "70026"
                            '            ' Ohio - Use building limit for mine sub limit up to 300k
                            '            If bld.E_Farm_Limit IsNot Nothing AndAlso IsNumeric(bld.E_Farm_Limit) Then
                            '                If CDec(bld.E_Farm_Limit) < 300000 Then
                            '                    limit = Format(CDec(bld.E_Farm_Limit), NumFormatNoCents)
                            '                Else
                            '                    limit = "300,000"
                            '                End If
                            '            End If
                            '            Exit Select
                            '        Case Else
                            '            If bld.E_Farm_Limit IsNot Nothing AndAlso IsNumeric(bld.E_Farm_Limit) Then
                            '                limit = Format(CDec(bld.E_Farm_Limit), NumFormatNoCents)
                            '            End If
                            '            Exit Select
                            '    End Select
                            'Else
                            '    ' All other states than Ohio - use this logic for limit
                            '    If Cov.IncreasedLimit IsNot Nothing AndAlso IsNumeric(Cov.IncreasedLimit) AndAlso CDec(Cov.IncreasedLimit) > 0 Then limit = Cov.IncreasedLimit
                            '    If IsNumeric(limit) Then limit = Format(CDec(limit), NumFormatNoCents)
                            'End If



                            If Loc.Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio AndAlso Cov.CoverageCodeId = "70026" Then
                                ' Ohio - Use building limit for mine sub limit up to 300k
                                If bld.E_Farm_Limit IsNot Nothing AndAlso IsNumeric(bld.E_Farm_Limit) Then
                                    If CDec(bld.E_Farm_Limit) < 300000 Then
                                        limit = Format(CDec(bld.E_Farm_Limit), NumFormatNoCents)
                                    Else
                                        limit = "300,000"
                                    End If
                                End If
                            Else
                                ' All other states than Ohio - use this logic for limit
                                If Cov.IncreasedLimit IsNot Nothing AndAlso IsNumeric(Cov.IncreasedLimit) AndAlso CDec(Cov.IncreasedLimit) > 0 Then limit = Cov.IncreasedLimit
                                If IsNumeric(limit) Then limit = Format(CDec(limit), NumFormatNoCents)
                            End If

                            Dim dscr As String = ""
                            If Cov.Description IsNot Nothing AndAlso Cov.Description.Trim <> String.Empty Then
                                dscr = Cov.Description
                            Else
                                ' Pull the caption from diamond
                                dscr = GetCoverageCodeCaption(Cov.CoverageCodeId, stateQuote:=quoteForLoc) 'Updated 9/12/18 for multi state MLW
                            End If
                            If FieldHasNumericValue(Cov.Premium, ClassName, lblMsg, True) Then
                                prem = Format(CDec(Cov.Premium), NumFormatWithCents)
                            Else
                                prem = "Included"
                            End If
                            CoverageTable.Add(New With {.LocationIndex = LocationIndex.ToString, .BuildingIndex = BuildingIndex.ToString, .CoverageName = dscr, .Limit = limit, .Premium = prem, .lossExt = "", .CoIns = ""})
                        Next

                        ' Loss Income Coverage
                        Dim lossIncome = Loc.IncomeLosses.Find(Function(p) p.Description.Trim() = String.Format("LOC{0}BLD{1}", Loc.LocationNum, LocationIndex))
                        If lossIncome IsNot Nothing Then
                            CoverageTable.Add(New With {.LocationIndex = LocationIndex.ToString, .BuildingIndex = BuildingIndex.ToString, .CoverageName = "Loss Income", .Limit = lossIncome.Limit, .Premium = lossIncome.QuotedPremium, .lossExt = lossIncome.ExtendFarmIncomeOptionId, .coIns = lossIncome.CoinsuranceTypeId})
                        End If

                        ' Heated Building Surcharge
                        If bld.HeatedBuildingSurchargeGasElectric OrElse bld.HeatedBuildingSurchargeOther Then
                            CoverageTable.Add(New With {.LocationIndex = LocationIndex.ToString, .BuildingIndex = BuildingIndex.ToString, .CoverageName = "Heated Building Surcharge Applies", .Limit = "N/A", .Premium = "", .lossExt = "", .CoIns = ""})
                        End If
                    Next
                Next

                ' MGB
                'For Each location In Quote.Locations
                For locIdx As Integer = 0 To Quote.Locations.Count - 1 'This locIndx and idx doesn't work when locations are deleted and new ones added
                    Dim LocNdx As Integer = locIdx
                    Dim BldNdx As Integer = -1
                    For Each c In BuildingTable.FindAll(Function(p) p.LocationIndex = LocNdx)
                        BldNdx += 1
                        ' data line 1 - Address
                        ' Note that territory is embedded in the data line for some reason
                        html.AppendLine("<tr>")
                        html.AppendLine("<td colspan=" & """" & "6" & """" & ">")
                        html.AppendLine(c.Address)
                        html.AppendLine("</td>")
                        html.AppendLine("</tr>")

                        ' data line 2 - Building, Building Type, Construction, Limit, Deductible, Premium
                        html.AppendLine("<tr>")

                        WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;" & c.Building, "text-align:left;")
                        WriteCell(html, c.BuildingType, "text-align:left;")
                        WriteCell(html, c.Limit, "text-align:right;")
                        WriteCell(html, c.Deductible, "text-align:right;")
                        WriteCell(html, c.Premium, "text-align:right;")

                        html.AppendLine("</tr>")

                        ' Coverages
                        Dim cnt = 0
                        For Each Cov In CoverageTable.FindAll(Function(p) p.LocationIndex = LocNdx.ToString)
                            If BldNdx = Cov.BuildingIndex Then
                                html.AppendLine("<tr>")
                                ' Coverages label only goes on the first line
                                If cnt = 0 Then
                                    WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Coverages", "text-align:left;")
                                Else
                                    WriteCell(html, "&nbsp;")
                                End If

                                If Cov.CoverageName = "Loss Income" AndAlso Cov.BuildingIndex = BldNdx Then
                                    'Dim lossIncome = MyLocation.IncomeLosses.Find(Function(p) p.Description.Trim() = index.ToString())
                                    WriteCell(html, Cov.CoverageName)
                                    WriteCell(html, Cov.Limit, "text-align:right;")
                                    WriteCell(html, "", "text-align:right;")
                                    WriteCell(html, Cov.Premium, "text-align:right;")

                                    html.AppendLine("<tr>")
                                    WriteCell(html, "&nbsp;")
                                    html.AppendLine("<td colspan=" & """" & "4" & """" & ">")
                                    html.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Period of Loss Extension - " & QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteIncomeLoss, QuickQuoteHelperClass.QuickQuotePropertyName.ExtendFarmIncomeOptionId, Cov.LossExt))
                                    html.AppendLine("</td>")
                                    html.AppendLine("</tr>")
                                    html.AppendLine("<tr>")
                                    WriteCell(html, "&nbsp;")
                                    html.AppendLine("<td colspan=" & """" & "4" & """" & ">")
                                    html.AppendLine("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Coinsurance - " & QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteIncomeLoss, QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, Cov.CoIns))
                                    html.AppendLine("</td>")
                                    html.AppendLine("</tr>")
                                Else
                                    WriteCell(html, Cov.CoverageName)
                                    WriteCell(html, Cov.Limit, "text-align:right;")
                                    WriteCell(html, "", "text-align:right;")
                                    WriteCell(html, Cov.Premium, "text-align:right;")
                                End If

                                html.AppendLine("</tr>")
                                cnt += 1
                            End If
                        Next
                    Next
                Next
            End If

            html.AppendLine("</table>")

            Me.tblBarnsAndBuildings.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateBarnsAndBuildings", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate the Farm Personal Property section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateFarmPersonalProperty()
        Dim html As New StringBuilder()
        Dim terr As String = ""
        Dim txt As String = ""
        Dim tot As Decimal = 0
        Dim lim As String = ""
        Dim prem As String = ""
        Dim covName As String = ""
        Dim desc As String = ""
        Dim descHover As String = ""
        Dim deduct As String = ""

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header Row
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            'Updated 10/24/2022 for bug 76012 MLW
            WriteCell(html, "Coverage", "width: 150px;text-align:left;")
            'WriteCell(html, "Coverage", "width: 350px;text-align:left;")
            WriteCell(html, "Description", "width: 200px;") 'Added 10/24/2022 for bug 76012 MLW
            WriteCell(html, "Limit", "width: 90px;text-align:right;")
            WriteCell(html, "&nbsp", "width: 30px;")
            WriteCell(html, "Deductible", "width: 70px;text-align:Right;")
            WriteCell(html, "Premium", "width: 90px;text-align:Right;")
            html.AppendLine("</tr>")

            'Updated 10/24/2022 for bug 76012 MLW
            Dim PPTable = {(New With {.Coverage = "", .Description = "", .DescriptionHover = "", .Limit = "", .Premium = "", .EQ = "", .Theft = "", .Deduct = ""})}.Take(0).ToList()
            'Dim PPTable = {(New With {.Coverage = "", .Limit = "", .Premium = "", .EQ = "", .Theft = "", .Deduct = ""})}.Take(0).ToList()
            Dim PeakTable = {(New With {.Description = "", .Limit = "", .Premium = "", .PPIndex = ""})}.Take(0).ToList()
            Dim CoverageTable = {(New With {.BuildingIndex = "", .CoverageName = "", .Limit = ""})}.Take(0).ToList()

            ' Updated code to save Farm Unscheduled Personal Property to the Governing State part instead of State Parts MGB 2-1-2019 Bug 31175
            Dim index As Int32 = -1
            If Me.Quote IsNot Nothing Then
                If GoverningStateQuote() IsNot Nothing Then
                    deduct = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Farm_F_and_G_DeductibleLimitId, GoverningStateQuote.Farm_F_and_G_DeductibleLimitId)
                    If GoverningStateQuote.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso GoverningStateQuote.ScheduledPersonalPropertyCoverages.Count > 0 Then
                        ' Build the Personal Property and Peak tables
                        For Each spp As QuickQuoteScheduledPersonalPropertyCoverage In GoverningStateQuote.ScheduledPersonalPropertyCoverages
                            ' Data items
                            'tot += CDec(spp.TotalPremium)
                            index += 1
                            lim = spp.IncreasedLimit
                            Dim MainPrem As String = Nothing
                            If lim IsNot Nothing AndAlso lim.Trim <> String.Empty Then
                                ' limit is populated, search on name + limit TASK 61164 MGB
                                MainPrem = SumScheduledPersonalPropertyPremium("MAIN", spp.Description.ToUpper, lim)
                            Else
                                ' Limit is not populated, secarch on name only TASK 61164 MGB
                                MainPrem = SumScheduledPersonalPropertyPremium("MAIN", spp.Description.ToUpper)
                            End If
                            Dim EQPrem As String = SumScheduledPersonalPropertyPremium("EQ", spp.Description.ToUpper)
                            prem = (CDec(MainPrem) + CDec(EQPrem)).ToString
                            'If FieldHasNumericValue(spp.MainCoveragePremium, ClassName, lblMsg, True) Then prem = CDec(spp.MainCoveragePremium).ToString Else prem = "0"
                            'If FieldHasNumericValue(spp.EarthquakePremium, ClassName, lblMsg, True) Then prem = (CDec(prem) + CDec(spp.EarthquakePremium)).ToString
                            tot += CDec(prem)
                            prem = Format(CDec(prem), NumFormatWithCents)
                            'deduct = Quote.Farm_F_and_G_DeductibleLimitId
                            If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents)
                            'Added 10/24/2022 for bug 76012 MLW
                            covName = "" 'reset for every loop
                            Select Case spp.CoverageCodeId
                                Case "40029"
                                    'LocationF_DescribedMachinery
                                    covName = "Farm Machinery Described"
                                Case "80121"
                                    'FarmMachineryDescribed_OpenPerils
                                    covName = "Farm Machinery Described - Open Perils"
                                Case "70063"
                                    'Farm_F_Irrigation_Equipment
                                    covName = "Irrigation Equipment"
                                Case "80122"
                                    'Livestock
                                    covName = "Livestock"
                                Case "80118"
                                    'MiscellaneousFarmPersonalProperty
                                    covName = "Miscellaneous Farm Personal Property"
                                Case "70153"
                                    'Farm_Rented_or_borrowed_Equipment
                                    covName = "Rented or Borrowed Equipment"
                                    If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) AndAlso SubQuoteFirst IsNot Nothing AndAlso SubQuoteFirst.HasFarmExtender Then
                                        If FieldHasNumericValue(prem, ClassName, lblMsg, True) = False Then
                                            prem = "Included"
                                        End If
                                    End If
                                Case "40030"
                                    'LocationF_MachineryNotDescribed
                                    covName = "Farm Machinery - Not Described"
                                Case "70056"
                                    'Farm_F_Grain
                                    covName = "Grain in Buildings"
                                Case "80131"
                                    'GrainintheOpen
                                    covName = "Grain in the Open"
                                Case "70057"
                                    'Farm_F_Hay_in_Barn
                                    covName = "Hay in Buildings"
                                Case "70058"
                                    'Farm_F_Hay_in_the_Open
                                    covName = "Hay in the Open"
                                Case "80119"
                                    'ReproductiveMaterials
                                    covName = "Reproductive Equipment"
                                Case Else
                                    covName = ""
                            End Select
                            desc = spp.Description
                            descHover = "" 'reset for every loop
                            If desc.Length > 65 Then
                                descHover = desc
                                desc = Left(desc, 65) & "..."
                            End If
                            'Updated 10/24/2022 for bug 76012 MLW
                            ''PPTable.Add(New With {.Coverage = spp.Description, .Limit = lim, .Premium = spp.TotalPremium, .EQ = spp.HasEarthquakeCoverage.ToString, .Theft = ""})
                            'PPTable.Add(New With {.Coverage = spp.Description, .Limit = lim, .Premium = prem, .EQ = spp.HasEarthquakeCoverage.ToString, .Theft = "", .Deduct = deduct})
                            PPTable.Add(New With {.Coverage = covName, .Description = desc, .DescriptionHover = descHover, .Limit = lim, .Premium = prem, .EQ = spp.HasEarthquakeCoverage.ToString, .Theft = "", .Deduct = deduct})
                            If spp.PeakSeasons IsNot Nothing AndAlso spp.PeakSeasons.Count > 0 Then
                                For Each ps As QuickQuotePeakSeason In spp.PeakSeasons
                                    Dim plim As String = ps.IncreasedLimit
                                    If IsNumeric(plim) Then plim = Format(CDec(plim), NumFormatNoCents)
                                    If FieldHasNumericValue(ps.Premium, ClassName, lblMsg, True) Then tot += CDec(ps.Premium)
                                    PeakTable.Add(New With {.Description = ps.Description & " " & ps.EffectiveDate & " - " & ps.ExpirationDate, .Limit = plim, .Premium = ps.Premium, .PPIndex = index.ToString})
                                Next
                            End If
                        Next

                    End If

                    'Dim index As Int32 = -1
                    'If Me.Quote IsNot Nothing Then
                    '    'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
                    '    If SubQuoteFirst IsNot Nothing Then
                    '        deduct = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Farm_F_and_G_DeductibleLimitId, SubQuoteFirst.Farm_F_and_G_DeductibleLimitId)
                    '        If SubQuoteFirst.ScheduledPersonalPropertyCoverages IsNot Nothing AndAlso SubQuoteFirst.ScheduledPersonalPropertyCoverages.Count > 0 Then
                    '            ' Build the Personal Property and Peak tables
                    '            For Each spp As QuickQuoteScheduledPersonalPropertyCoverage In SubQuoteFirst.ScheduledPersonalPropertyCoverages
                    '                ' Data items
                    '                'tot += CDec(spp.TotalPremium)
                    '                index += 1
                    '                Dim MainPrem As String = SumScheduledPersonalPropertyPremium("MAIN", spp.Description.ToUpper)
                    '                Dim EQPrem As String = SumScheduledPersonalPropertyPremium("EQ", spp.Description.ToUpper)
                    '                prem = (CDec(MainPrem) + CDec(EQPrem)).ToString
                    '                'If FieldHasNumericValue(spp.MainCoveragePremium, ClassName, lblMsg, True) Then prem = CDec(spp.MainCoveragePremium).ToString Else prem = "0"
                    '                'If FieldHasNumericValue(spp.EarthquakePremium, ClassName, lblMsg, True) Then prem = (CDec(prem) + CDec(spp.EarthquakePremium)).ToString
                    '                tot += CDec(prem)
                    '                prem = Format(CDec(prem), NumFormatWithCents)
                    '                lim = spp.IncreasedLimit
                    '                'deduct = Quote.Farm_F_and_G_DeductibleLimitId
                    '                If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents)
                    '                'PPTable.Add(New With {.Coverage = spp.Description, .Limit = lim, .Premium = spp.TotalPremium, .EQ = spp.HasEarthquakeCoverage.ToString, .Theft = ""})
                    '                PPTable.Add(New With {.Coverage = spp.Description, .Limit = lim, .Premium = prem, .EQ = spp.HasEarthquakeCoverage.ToString, .Theft = "", .Deduct = deduct})
                    '                If spp.PeakSeasons IsNot Nothing AndAlso spp.PeakSeasons.Count > 0 Then
                    '                    For Each ps As QuickQuotePeakSeason In spp.PeakSeasons
                    '                        Dim plim As String = ps.IncreasedLimit
                    '                        If IsNumeric(plim) Then plim = Format(CDec(plim), NumFormatNoCents)
                    '                        If FieldHasNumericValue(ps.Premium, ClassName, lblMsg, True) Then tot += CDec(ps.Premium)
                    '                        PeakTable.Add(New With {.Description = ps.Description & " " & ps.EffectiveDate & " - " & ps.ExpirationDate, .Limit = plim, .Premium = ps.Premium, .PPIndex = index.ToString})
                    '                    Next
                    '                End If
                    '            Next

                    '        End If

                    ' MGB 9/2/15
                    ' The following coverages are stored under Quote.OptionalCoverages:
                    ' * 4H and FFA
                    ' * Sheep Additional Perils
                    If SubQuoteFirst.OptionalCoverages IsNot Nothing AndAlso SubQuoteFirst.OptionalCoverages.Count > 0 Then
                        For Each oc As QuickQuoteOptionalCoverage In SubQuoteFirst.OptionalCoverages
                            Select Case oc.CoverageCodeId
                                Case "80129", "80134"  ' 80129 = 4H and FFA; 80134 = Sheep                                    
                                    'Updated 10/24/2022 for bug 76012 MLW
                                    If oc.CoverageCodeId = "80129" Then
                                        covName = "4H and FFA Animals"
                                        desc = oc.Description
                                        descHover = "" 'reset for every loop
                                        If desc.Length > 65 Then
                                            descHover = desc
                                            desc = Left(desc, 65) & "..."
                                        End If
                                    Else : oc.CoverageCodeId = "80134"
                                        If oc.Description Is Nothing OrElse oc.Description = String.Empty Then
                                            covName = "SHEEP - ADDITIONAL PERILS"
                                        Else
                                            covName = oc.Description
                                        End If
                                        desc = ""
                                        descHover = ""
                                    End If
                                    'If oc.Description Is Nothing OrElse oc.Description = String.Empty Then
                                    '    If oc.CoverageCodeId = "80129" Then
                                    '        desc = "4H AND FFA ANIMALS"
                                    '    Else : oc.CoverageCodeId = "80134"
                                    '        desc = "SHEEP - ADDITIONAL PERILS"
                                    '    End If
                                    'Else
                                    '    desc = oc.Description
                                    'End If

                                    prem = SumOptionalCoveragePremium(oc.CoverageCodeId)
                                    'If FieldHasNumericValue(oc.Premium, ClassName, lblMsg, True) Then prem = CDec(oc.Premium).ToString Else prem = "0"
                                    tot += CDec(prem)
                                    prem = Format(CDec(prem), NumFormatWithCents)
                                    lim = oc.IncreasedLimit
                                    If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents)
                                    'Updated 10/25/2022 for bug 76012 MLW
                                    PPTable.Add(New With {.Coverage = covName, .Description = desc, .DescriptionHover = descHover, .Limit = lim, .Premium = prem, .EQ = "False", .Theft = "", .Deduct = ""})
                                    'PPTable.Add(New With {.Coverage = desc, .Limit = lim, .Premium = prem, .EQ = "False", .Theft = "", .Deduct = ""})
                                    index += 1
                                    Exit Select
                                Case "70038" ' Suffocation of Livestock - Moved from Barns & Buildings 9/21/15
                                    ' The coverage description field holds the location and building numbers  
                                    ' Moved from additional coverages 9/22/15
                                    Dim L As QuickQuoteLocation = Quote.Locations(0) 'Added 11/19/18 for multi state MLW
                                    Dim quoteForLoc As QuickQuoteObject = Me.SubQuoteForLocation(L) 'Added 11/19/18 for multi state MLW 
                                    desc = GetCoverageCodeCaption("70038", stateQuote:=quoteForLoc) 'Updated 11/19/18 for multi state MLW
                                    Dim covinfo() As String = oc.Description.Split(".")
                                    prem = SumOptionalCoveragePremium(oc.CoverageCodeId)
                                    'prem = oc.Premium
                                    tot += CDec(prem)
                                    prem = Format(CDec(prem), NumFormatWithCents)
                                    If covinfo IsNot Nothing AndAlso covinfo.Count = 2 Then
                                        desc += " - Loc " & covinfo(0).ToString() & ", Bldg " & covinfo(1).ToString()
                                    End If
                                    desc = desc.ToUpper()
                                    If oc.IncludedLimit IsNot Nothing AndAlso IsNumeric(oc.IncludedLimit) AndAlso CDec(oc.IncludedLimit) > 0 Then lim = oc.IncludedLimit Else lim = 0
                                    If oc.IncreasedLimit IsNot Nothing AndAlso IsNumeric(oc.IncreasedLimit) AndAlso CDec(oc.IncreasedLimit) > 0 Then
                                        If IsNumeric(lim) Then
                                            lim = CDec(lim) + CDec(oc.IncreasedLimit)
                                        Else
                                            lim = oc.IncreasedLimit
                                        End If
                                    End If
                                    If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents)
                                    'Updated 10/25/2022 for bug 76012 MLW
                                    PPTable.Add(New With {.Coverage = desc, .Description = "", .DescriptionHover = "", .Limit = lim, .Premium = prem, .EQ = "False", .Theft = "", .Deduct = ""})
                                    'PPTable.Add(New With {.Coverage = desc, .Limit = lim, .Premium = prem, .EQ = "False", .Theft = "", .Deduct = ""})
                                    index += 1
                                    Exit Select
                                Case "80566", "80567", "80568", "80569"  ' Added 5/5/2020 MGB Task 45465
                                    ' Suffocation of Livestock and Poultry: Swine, Poultry, Cattle, and Equine respectively
                                    ' EACH BUILDING GETS A LINE!!
                                    desc = ""
                                    prem = ""
                                    Dim LocBldList As New List(Of String)
                                    ' Number the coverages by location and building
                                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                                        For L As Integer = 0 To Quote.Locations.Count - 1
                                            Dim LOC As QuickQuoteLocation = Quote.Locations(L)
                                            If LOC.Buildings IsNot Nothing AndAlso LOC.Buildings.Count > 0 Then
                                                For B As Integer = 0 To LOC.Buildings.Count - 1
                                                    Dim BLD As QuickQuoteBuilding = LOC.Buildings(B)
                                                    LocBldList.Add(" LOC " & L + 1.ToString.Trim & ", BLDG " & B + 1.ToString.Trim)
                                                Next
                                            End If
                                        Next
                                    End If
                                    Dim cnt As Integer = -1
                                    For Each LocBld As String In LocBldList
                                        cnt += 1
                                        Select Case oc.CoverageCodeId
                                            Case "80566" ' swine
                                                desc = "SUFFOCATION OF LIVESTOCK OR POULTRY - SWINE " & LocBld
                                                lim = oc.IncreasedLimit
                                                prem = SumOptionalCoveragePremium(oc.CoverageCodeId)
                                                prem = Format(CDec(prem), NumFormatWithCents)
                                                tot += CDec(prem)
                                                Exit Select
                                            Case "80567" ' Poultry
                                                desc = "SUFFOCATION OF LIVESTOCK OR POULTRY - POULTRY " & LocBld
                                                lim = oc.IncreasedLimit
                                                prem = SumOptionalCoveragePremium(oc.CoverageCodeId)
                                                prem = Format(CDec(prem), NumFormatWithCents)
                                                tot += CDec(prem)
                                                Exit Select
                                            Case "80568" ' Cattle
                                                desc = "SUFFOCATION OF LIVESTOCK OR POULTRY - CATTLE " & LocBld
                                                lim = oc.IncreasedLimit
                                                prem = SumOptionalCoveragePremium(oc.CoverageCodeId)
                                                prem = Format(CDec(prem), NumFormatWithCents)
                                                tot += CDec(prem)
                                                Exit Select
                                            Case "80569" ' Equine
                                                desc = "SUFFOCATION OF LIVESTOCK OR POULTRY - EQUINE " & LocBld
                                                lim = oc.IncreasedLimit
                                                prem = SumOptionalCoveragePremium(oc.CoverageCodeId)
                                                prem = Format(CDec(prem), NumFormatWithCents)
                                                tot += CDec(prem)
                                                Exit Select
                                        End Select

                                        ' Only show the premium on the first one, show 'Included' on the rest
                                        If cnt <> 0 And prem > 0 Then prem = "Included"

                                        If desc <> "" Then
                                            If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents)
                                            'Updated 10/25/2022 for bug 76012 MLW
                                            If prem = "Included" OrElse (IsNumeric(prem) AndAlso CDec(prem) > 0) Then PPTable.Add(New With {.Coverage = desc, .Description = "", .DescriptionHover = "", .Limit = lim, .Premium = prem, .EQ = "False", .Theft = "", .Deduct = ""})
                                            'If prem = "Included" OrElse (IsNumeric(prem) AndAlso CDec(prem) > 0) Then PPTable.Add(New With {.Coverage = desc, .Limit = lim, .Premium = prem, .EQ = "False", .Theft = "", .Deduct = ""})
                                            index += 1
                                        End If
                                    Next

                                    Exit Select
                                Case Else
                                    Exit Select
                            End Select
                        Next
                    End If

                    ' UNSCHEDULED PERSONAL PROPERTY COVERAGE
                    If GoverningStateQuote.UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                        index += 1

                        ' Calculate premium
                        'Updated 9/6/2022 for task 74283 MLW
                        'Dim unscPrem = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.TotalPremium
                        Dim unscPrem = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.MainCoveragePremium
                        Dim EqPrem = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.EarthquakePremium
                        'Dim UnscPrem = QQHelper.GetSumForSubPropertyValues(SubQuotes, Function() Quote.UnscheduledPersonalPropertyCoverage, Function() Quote.UnscheduledPersonalPropertyCoverage.TotalPremium, maintainFormattingOrDefaultValue:=True, verifyMainPropertyExists:=True, verifySubPropertyExists:=True)
                        'Dim EqPrem = QQHelper.GetSumForSubPropertyValues(SubQuotes, Function() Quote.UnscheduledPersonalPropertyCoverage, Function() Quote.UnscheduledPersonalPropertyCoverage.EarthquakePremium, maintainFormattingOrDefaultValue:=True, verifyMainPropertyExists:=True, verifySubPropertyExists:=True)
                        If unscPrem.Trim = "" OrElse (Not IsNumeric(unscPrem)) Then unscPrem = "0"
                        If EqPrem.Trim = "" OrElse (Not IsNumeric(EqPrem)) Then EqPrem = "0"

                        If CDec(unscPrem) + CDec(EqPrem) > 0 Then  ' Only show the coverage if there is premium
                            prem = (CDec(unscPrem) + CDec(EqPrem)).ToString
                            tot += CDec(prem)
                            prem = Format(CDec(prem), NumFormatWithCents)

                            ' Limit
                            lim = QQHelper.GetSumForSubPropertyValues(SubQuotes, Function() Quote.UnscheduledPersonalPropertyCoverage, Function() Quote.UnscheduledPersonalPropertyCoverage.IncreasedLimit, maintainFormattingOrDefaultValue:=True, verifyMainPropertyExists:=True, verifySubPropertyExists:=True)
                            If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents) Else lim = ""

                            ' Add the unscheduled personal property section
                            'Updated 10/25/2022 for bug 76012 MLW
                            PPTable.Add(New With {.Coverage = "UNSCHEDULED FARM PERSONAL PROPERTY", .Description = "", .DescriptionHover = "", .Limit = lim, .Premium = prem, .EQ = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage.ToString, .Theft = "", .Deduct = deduct})
                            'PPTable.Add(New With {.Coverage = "UNSCHEDULED FARM PERSONAL PROPERTY", .Limit = lim, .Premium = prem, .EQ = GoverningStateQuote.UnscheduledPersonalPropertyCoverage.HasEarthquakeCoverage.ToString, .Theft = "", .Deduct = deduct})

                            ' Unscheduled Equipment Peak Seasons
                            If GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons IsNot Nothing AndAlso GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons.Count > 0 Then
                                For Each ps As QuickQuotePeakSeason In GoverningStateQuote.UnscheduledPersonalPropertyCoverage.PeakSeasons
                                    Dim plim As String = ps.IncreasedLimit
                                    If IsNumeric(plim) Then
                                        plim = Format(CDec(plim), NumFormatNoCents)
                                        If FieldHasNumericValue(ps.Premium, ClassName, lblMsg, True) Then tot += CDec(ps.Premium)
                                        PeakTable.Add(New With {.Description = ps.Description & " " & ps.EffectiveDate & " - " & ps.ExpirationDate, .Limit = plim, .Premium = ps.Premium, .PPIndex = index.ToString})
                                    End If
                                Next
                            End If
                        End If
                    End If

                    ' Build the data table rows
                    ' Coverage Row
                    index = 0
                    For Each PPRow In PPTable
                        html.AppendLine("<tr>")
                        'Updated 10/25/2022 for bug 76012 MLW - show coverage and description separate, instead of description in the coverage column
                        Select Case PPRow.Coverage.ToUpper
                            Case "4H AND FFA ANIMALS", "FARM MACHINERY DESCRIBED", "FARM MACHINERY DESCRIBED - OPEN PERILS",
"IRRIGATION EQUIPMENT", "LIVESTOCK", "RENTED OR BORROWED EQUIPMENT", "FARM MACHINERY - NOT DESCRIBED",
"GRAIN IN BUILDINGS", "GRAIN IN THE OPEN", "HAY IN BUILDINGS", "HAY IN THE OPEN", "REPRODUCTIVE EQUIPMENT",
"MISCELLANEOUS FARM PERSONAL PROPERTY"
                                ' Coverage Name
                                WriteCell(html, PPRow.Coverage)
                                ' Description
                                If IsNullEmptyorWhitespace(PPRow.DescriptionHover) = False Then
                                    WriteCell(html, PPRow.Description, "", "", PPRow.DescriptionHover)
                                Else
                                    WriteCell(html, PPRow.Description)
                                End If
                            Case Else
                                ' Description
                                WriteCellTwoColumns(html, PPRow.Coverage)
                        End Select
                        '' Description
                        'WriteCell(html, PPRow.Coverage)
                        ' Limit
                        WriteCell(html, PPRow.Limit, "text-align:right;")
                        ' Earthquake
                        If PPRow.EQ IsNot Nothing AndAlso PPRow.EQ <> String.Empty Then
                            Dim bl As Boolean = CBool(PPRow.EQ)
                            If bl Then
                                WriteCell(html, "EQ", "text-align:center;")
                            Else
                                WriteCell(html, "&nbsp;")
                            End If
                        End If
                        ' Theft - Not included in this release
                        'If PPRow.Theft IsNot Nothing AndAlso PPRow.Theft <> String.Empty Then
                        '    Dim bl As Boolean = CBool(PPRow.Theft)
                        '    If bl Then
                        '        WriteCell(html, "Thft", "text-align:center;")
                        '    Else
                        '        WriteCell(html, "&nbsp;")
                        '    End If
                        'End If
                        WriteCell(html, PPRow.Deduct, "text-align:right;")
                        ' Premium
                        WriteCell(html, PPRow.Premium, "text-align:right;")
                        html.AppendLine("</tr>")

                        ' Peak Season Rows
                        For Each PK In PeakTable
                            If PK.PPIndex = index.ToString Then
                                html.AppendLine("<tr>")
                                'Updated 10/25/2022 for bug 76012 MLW
                                WriteCellTwoColumns(html, "&nbsp;&nbsp;&nbsp;&nbsp;" & PK.Description)
                                'WriteCell(html, "&nbsp;&nbsp;&nbsp;&nbsp;" & PK.Description)
                                'WriteCell(html, "&nbsp;") 'Added 10/25/2022 for bug 76012 MLW
                                WriteCell(html, PK.Limit, "text-align:right;")
                                WriteCell(html, "&nbsp;")
                                WriteCell(html, "&nbsp;")
                                WriteCell(html, PK.Premium, "text-align:right;")
                                html.AppendLine("</tr>")
                            End If
                        Next

                        index += 1
                    Next
                End If
            End If

            ' Total Row
            If tot > 0 Then
                html.AppendLine("<tr style=" & """" & "border-top:solid 2px;" & """" & ">")
                'Updated 10/25/2022 for bug 76012 MLW
                html.AppendLine("<td colspan=" & """" & "5" & """" & ">")
                'html.AppendLine("<td colspan=" & """" & "4" & """" & ">")
                html.AppendLine("Total Farm Personal Property Premium")
                html.AppendLine("</td>")
                WriteCell(html, Format(tot, NumFormatWithCents), "text-align:right;")
                html.AppendLine("</tr>")
            Else
                html.AppendLine("<tr>")
                'Updated 10/25/2022 for bug 76012 MLW
                html.AppendLine("<td colspan=" & """" & "6" & """" & ">")
                'html.AppendLine("<td colspan=" & """" & "5" & """" & ">")
                '                html.AppendLine("<td colspan=" & """" & "5" & """" & " style=" & """" & "border-top:solid 1px;" & """" & ">")
                html.AppendLine("N/A")
                html.AppendLine("</td>")
                html.AppendLine("</tr>")
            End If

            html.AppendLine("</table>")

            Me.tblFarmPersonalProperty.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateFarmPersonalProperty", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate the Farm Incidental Limts section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateFarmIncidentalLimits()
        Dim html As New StringBuilder()
        Dim tot As Decimal = 0
        Dim lim As String = ""
        Dim prem As String = ""
        Dim desc As String = ""
        Dim deduct As String = ""

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header Row
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Coverage", "width: 350px;text-align:left;")
            WriteCell(html, "Limit", "width: 90px;text-align:right;")
            WriteCell(html, "&nbsp", "width: 30px;")
            WriteCell(html, "Deductible", "width: 70px;text-align:Right;")
            WriteCell(html, "Premium", "width: 90px;text-align:Right;")
            html.AppendLine("</tr>")

            Dim FilTable = {(New With {.Coverage = "", .Limit = "", .Deduct = "", .Premium = ""})}.Take(0).ToList()

            If Me.Quote IsNot Nothing Then
                If GoverningStateQuote() IsNot Nothing Then
                    If GoverningStateQuote.FarmIncidentalLimits IsNot Nothing AndAlso GoverningStateQuote.FarmIncidentalLimits.Count > 0 Then
                        For Each oc As QuickQuoteFarmIncidentalLimit In GoverningStateQuote.FarmIncidentalLimits
                            Select Case oc.CoverageType
                                Case QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Glass_Breakage_in_Cabs
                                    ' This should be generic enough to handle all of these, but just in case, we can
                                    ' break them out for further processing by CoverageType.
                                    If Common.Helpers.FARM.GlassBreakageForCabs.IsGlassBreakageForCabsAvailable(Quote) Then
                                        desc = GetCoverageCodeCaption(oc.CoverageCodeId, stateQuote:=GoverningStateQuote)
                                        lim = oc.TotalLimit
                                        deduct = "&nbsp;"
                                        If oc.Premium.TryToGetDouble > 0 Then
                                            tot += oc.Premium.TryToGetDouble
                                            prem = Format(oc.Premium.TryToGetDouble, NumFormatWithCents)
                                        Else
                                            prem = "Included"
                                        End If

                                        FilTable.Add(New With {.Coverage = desc, .Limit = lim, .Deduct = deduct, .Premium = prem})
                                    End If
                                    Exit Select
                                Case QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Fire_Department_Service_Charge

                                    Exit Select
                                Case Else
                                    Exit Select
                            End Select
                        Next
                    End If

                    'Build the data table rows
                    ' Coverage Row
                    For Each PPRow In FilTable
                        html.AppendLine("<tr>")
                        ' Description
                        WriteCell(html, PPRow.Coverage, "width: 350px;")
                        ' Limit 
                        WriteCell(html, PPRow.Limit, "width: 90px;text-align:right;")
                        ' Spacer
                        WriteCell(html, "&nbsp", "width: 30px;")
                        ' Deductable
                        WriteCell(html, PPRow.Deduct, "width: 70px;text-align:right;")
                        ' Premium
                        WriteCell(html, PPRow.Premium, "width: 90px;text-align:right;")
                        html.AppendLine("</tr>")
                    Next
                End If
            End If

            ' Total Row - Show if there is a Value to total Premium and not just all included.
            If tot > 0 Then
                html.AppendLine("<tr style=" & """" & "border-top:solid 2px;" & """" & ">")
                html.AppendLine("<td colspan=" & """" & "4" & """" & ">")
                html.AppendLine("Total Farm Incidental Limits")
                html.AppendLine("</td>")
                WriteCell(html, Format(tot, NumFormatWithCents), "text-align:right;")
                html.AppendLine("</tr>")
            End If

            html.AppendLine("</table>")

            Me.tblFarmIncidentalLimits.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            ' Spacer
            WriteCell(html, "&nbsp", "width: 30px;")
            HandleError(ClassName, "PopulateFarmPersonalProperty", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Sums up the premium across states for the passed optional coverage
    ''' </summary>
    ''' <param name="CovCodeId"></param>
    ''' <returns></returns>
    Private Function SumOptionalCoveragePremium(ByVal CovCodeId) As String
        Dim tot As Decimal = 0

        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            If sq.OptionalCoverages IsNot Nothing Then
                For Each oc As QuickQuote.CommonObjects.QuickQuoteOptionalCoverage In sq.OptionalCoverages
                    If oc.CoverageCodeId = CovCodeId Then
                        If IsNumeric(oc.Premium) Then tot += CDec(oc.Premium)
                        Exit For
                    End If
                Next
            End If
        Next

        Return tot.ToString
    End Function

    ''' <summary>
    ''' Sums the premium for the passed Scheduled Personal Property coverage
    ''' Can either be MAIN or EQ
    ''' 
    ''' Added Limit parameter 5/10/21 to fix the issue where when coverage items have the same name they all show the 
    ''' same premium.  Matching on name and limit fixes the issue.  Bug 61164.
    ''' </summary>
    ''' <param name="MainOrEQ"></param>
    ''' <returns></returns>
    Private Function SumScheduledPersonalPropertyPremium(ByVal MainOrEQ As String, ByVal CovDesc As String, Optional ByVal CovLimit As String = Nothing) As String
        Dim tot As Decimal = 0

        For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
            If sq.ScheduledPersonalPropertyCoverages IsNot Nothing Then
                If MainOrEQ.ToUpper = "MAIN" Then
                    For Each spp As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
                        If CovLimit IsNot Nothing AndAlso CovLimit.Trim <> String.Empty Then
                            ' Match on name and limit
                            If spp.Description.ToUpper = CovDesc.ToUpper AndAlso spp.IncreasedLimit = CovLimit Then
                                If IsNumeric(spp.MainCoveragePremium) Then tot += spp.MainCoveragePremium
                            End If
                        Else
                            ' Only match on name
                            If spp.Description.ToUpper = CovDesc.ToUpper Then
                                If IsNumeric(spp.MainCoveragePremium) Then tot += spp.MainCoveragePremium
                            End If
                        End If
                    Next
                ElseIf MainOrEQ.ToUpper = "EQ" Then  ' note that EQ doesn't send increased limit
                    For Each spp As QuickQuote.CommonObjects.QuickQuoteScheduledPersonalPropertyCoverage In sq.ScheduledPersonalPropertyCoverages
                        If spp.Description.ToUpper = CovDesc.ToUpper Then
                            If IsNumeric(spp.EarthquakePremium) Then tot += spp.EarthquakePremium
                        End If
                    Next
                End If
            End If
        Next

        Return tot.ToString
    End Function

    ''' <summary>
    ''' Populate the Credits section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateCredits()
        Dim html As New StringBuilder()

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Credit", "width:236px")
            WriteCell(html, "&nbsp", "width:77px;")
            html.AppendLine("</tr>")

            Dim opCovList = {(New With {.Name = "", .Percent = ""})}.Take(0).ToList()

            For Each c In IFM.VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(Me.Quote, False)
                opCovList.Add(New With {.Name = c.Key, .Percent = c.Value})
            Next

            For Each c In From o In opCovList Order By IFM.Common.InputValidation.InputHelpers.TryToGetDouble(o.Percent) Descending Select o
                html.AppendLine("<tr>")
                WriteCell(html, c.Name)
                WriteCell(html, c.Percent, "text-align:right;")
                html.AppendLine("</tr>")
            Next

            ' IRPM
            If IsQuoteEndorsement() = False Then
                Dim irpm As String = GetIRPMValue()
                If IsNumeric(irpm) AndAlso CDec(irpm) < 0 Then
                    irpm = irpm.Replace("-", "")
                    html.AppendLine("<tr>")
                    WriteCell(html, "IRPM Applied")
                    WriteCell(html, irpm & "%", "text-align:right;")
                    html.AppendLine("</tr>")
                End If
            End If

            html.AppendLine("</table>")

            Me.tblDiscounts.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateCredits", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Private Function DiamondToVRIRPMConversion(diamondValue) As String
        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Integer

        vrValue = diamondToVRDecimal * 100

        Return vrValue.ToString()
    End Function

    Public Shared Function DiamondToVRConversionDecimal(diamondValue As String) As String

        Dim diamondToVRDecimal As Decimal = Decimal.Parse(diamondValue) - 1
        Dim vrValue As Decimal

        vrValue = Math.Round(diamondToVRDecimal * 100, 1)
        If vrValue = 0.0 Then
            vrValue = 0
        End If

        Return vrValue.ToString()
    End Function


    Private Function VRToDiamondIRPMConversion(vrValue As String) As String
        Dim vrValueDecimal As Decimal = Decimal.Parse(vrValue) / 100
        Dim diamondValue As Decimal

        diamondValue = vrValueDecimal + 1

        Return diamondValue.ToString()
    End Function

    Private Function GetIRPMValue() As String
        Dim IRPMFactor As Decimal = 0
        Dim IRPM_Edited As String = Nothing
        Dim ndx As Integer = 0
        Dim SupportingBusiness As String = ""
        Dim CareCondition As String = ""
        Dim Damage As String = ""
        Dim Concentration As String = ""
        Dim Location As String = ""
        Dim Misc As String = ""
        Dim Roof As String = ""
        Dim IRPMStructure As String = ""
        Dim PastLosses As String = ""
        Dim RiceHulls As String = ""
        Dim Poultry As String = ""

        Try
            'Updated 9/6/18 for multi state MLW - Quote to SubQuoteFirst
            If SubQuoteFirst IsNot Nothing Then
                SupportingBusiness = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(11).RiskFactor)
                CareCondition = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(0).RiskFactor)
                Damage = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(2).RiskFactor)
                Concentration = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(3).RiskFactor)
                Location = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(6).RiskFactor)
                Misc = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(7).RiskFactor)
                Roof = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(8).RiskFactor)
                IRPMStructure = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(4).RiskFactor)
                PastLosses = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(10).RiskFactor)
                RiceHulls = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(5).RiskFactor)
                Poultry = DiamondToVRConversionDecimal(SubQuoteFirst.ScheduledRatings(12).RiskFactor)
            End If

            IRPMFactor = CDec(SupportingBusiness) + CDec(CareCondition) + CDec(Damage) +
                CDec(Concentration) + CDec(Location) + CDec(Misc) + CDec(Roof) +
                CDec(IRPMStructure) + CDec(PastLosses) + CDec(RiceHulls) + CDec(Poultry)

            'IRPM_Edited = Format(IRPMFactor, "#0")

            Return IRPMFactor
        Catch ex As Exception
            HandleError(ClassName, "GetIRPMValue", ex, lblMsg)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Populate the Surcharges section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateSurcharges()
        Dim html As New StringBuilder()

        Try
            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Surcharge", "width:236px;")
            WriteCell(html, "&nbsp", "width:77px;")

            html.AppendLine("</tr>")

            Dim opCovList = {(New With {.Name = "", .Premium = ""})}.Take(0).ToList()

            For Each item In IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMSurcharges(Me.Quote)
                opCovList.Add(New With {.Name = item.Key, .Premium = item.Value})
            Next

            For Each c In opCovList
                html.AppendLine("<tr>")
                WriteCell(html, c.Name)
                WriteCell(html, c.Premium, "text-align:right;")
                html.AppendLine("</tr>")
            Next

            ' IRPM
            If IsQuoteEndorsement() = False Then
                Dim irpm As String = GetIRPMValue()
                If IsNumeric(irpm) AndAlso CDec(irpm) > 0 Then
                    html.AppendLine("<tr>")
                    WriteCell(html, "IRPM Applied")
                    WriteCell(html, irpm & "%", "text-align:right;")
                    html.AppendLine("</tr>")
                End If
            End If

            html.AppendLine("</table>")
            Me.tblSurcharges.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateSurcharges", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate the Inland Marine section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateInlandMarine()
        Dim IMTable = {(New With {.Name = "", .Description = "", .Deductible = "", .Limit = "", .Premium = ""})}.Take(0).ToList()
        Dim sortOrder As New Dictionary(Of Int32, Int32)

        Try
            ' You only need this list if you're sorting the IM's
            'sortOrder.Add(0, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
            'sortOrder.Add(1, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)
            'sortOrder.Add(2, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)
            'sortOrder.Add(3, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)
            'sortOrder.Add(4, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)
            'sortOrder.Add(5, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)
            'sortOrder.Add(6, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)
            'sortOrder.Add(7, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
            'sortOrder.Add(8, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)
            'sortOrder.Add(9, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)
            'sortOrder.Add(10, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)
            'sortOrder.Add(11, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)

            'sortOrder.Add(12, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)
            'sortOrder.Add(13, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)
            'sortOrder.Add(14, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)
            'sortOrder.Add(15, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)
            'sortOrder.Add(16, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage)

            Dim inlandMarineTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.InlandMarinesTotal_CoveragePremium)
            Dim rvWaterTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.RvWatercraftsTotal_Premium)
            If Me.MyLocation IsNot Nothing Then
                If Me.MyLocation.InlandMarines IsNot Nothing Then
                    '' Build the table
                    'Dim sortedInlandMarineItems = From i In Me.MyLocation.InlandMarines
                    '                              Join type In sortOrder On i.InlandMarineType Equals type.Value
                    '                              Order By type.Key
                    '                              Select i
                    For Each cov As QuickQuoteInlandMarine In Me.MyLocation.InlandMarines
                        Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
                        If IsNumeric(deductible) Then deductible = Format(CDec(deductible), NumFormatNoCents)
                        Dim lim As String = cov.IncreasedLimit
                        If IsNumeric(lim) Then lim = Format(CDec(lim), NumFormatNoCents)
                        Dim inlandType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, cov.CoverageCodeId)
                        If cov.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I Then
                            inlandType = "Silverware"
                        Else
                            inlandType = inlandType.Replace("_", " ").Replace("Inland Marine", "")
                        End If

                        IMTable.Add(New With {.Name = inlandType, .Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cov.Description.ToLower()), .Deductible = deductible, .Limit = lim, .Premium = String.Format("{0:C2}", IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CoveragePremium))})
                    Next
                End If
            End If

            ' Build the html
            Dim html As New StringBuilder()
            html.AppendLine("<table class=""hom_qa_table_shades"">")

            ' Header row
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Description", "width:330px;")
            WriteCell(html, "Limit", "text-align:right;width:100px;")
            WriteCell(html, "Deductible", "text-align:right;width:100px;")
            WriteCell(html, "Premium", "text-align:right;width:100px;")
            html.AppendLine("</tr>")

            ' Data rows
            If IMTable.Any() Then
                For Each c In IMTable
                    html.AppendLine("<tr>")
                    WriteCell(html, c.Description)
                    WriteCell(html, c.Limit, "text-align:right;")
                    WriteCell(html, c.Deductible, "text-align:right;")
                    WriteCell(html, c.Premium, "text-align:right;")
                    html.AppendLine("</tr>")
                Next
            End If

            Dim inlandMarineMin As Double = 25

            If (inlandMarineTotal + rvWaterTotal) <> 0 Then
                ' Amount to meet minimum premium
                If (inlandMarineTotal + rvWaterTotal) < inlandMarineMin Then
                    'min line
                    html.AppendLine("<tr>")
                    html.AppendLine("<td colspan=""3"" style=""font-weight=bold;"">Amount to meet Minimum Premium</td>")
                    html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", inlandMarineMin - (inlandMarineTotal + rvWaterTotal)) + "</td>")
                    html.AppendLine("</tr>")
                    'html.AppendLine("<tr>")
                    'html.AppendLine("<td colspan=""3"" style=""font-weight=bold;"">Additional Amount to meet minimum</td>")
                    'html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", inlandMarineMin - (inlandMarineTotal + rvWaterTotal)) + "</td>")
                    'html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                    html.AppendLine("<td colspan=""3"" style=""font-weight=bold;"">Total Inland Marine Premium</td>")
                    html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", inlandMarineTotal + (inlandMarineMin - (inlandMarineTotal + rvWaterTotal))) + "</td>")
                    html.AppendLine("</tr>")
                Else
                    ' Total Premium Line
                    If inlandMarineTotal = 0.0 Then
                        ' no inland marine items
                        html.AppendLine("<tr>")
                        html.AppendLine("<td colspan=""4"">N/A</td>")
                        html.AppendLine("</tr>")
                    Else
                        html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                        html.AppendLine("<td colspan=""3"" style=""font-weight=bold;"">Total Inland Marine Premium</td>")
                        html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", inlandMarineTotal) + "</td>")
                        html.AppendLine("</tr>")
                    End If
                End If
            Else
                ' No inland marine items
                html.AppendLine("<tr>")
                html.AppendLine("<td colspan=""5"">N/A</td>")
                html.AppendLine("</tr>")
            End If

            html.AppendLine("</table>")

            Me.tblInlandMarine.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateInlandMarine", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Populate the RV and Watercraft section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateRvWaterCraft()
        ' Added for task 79067 and 77972 01/25/22 BD
        'Dim RVWCTableFarmGC = {(New With {.Type = "", .Year = "", .Coverage = "", .Deductible = "", .Limits = "", .Premium = ""})}.Take(0).ToList()
        Dim RVWCTable = {(New With {.Type = "", .Year = "", .Coverage = "", .Deductible = "", .Limits = "", .Premium = ""})}.Take(0).ToList()
        Dim rvWaterCraftTotal As Double = 0
        ' Added for task 79067 and 77972 01/25/22 BD
        Dim hasGolfCart As Boolean = False

        Try
            For Each Loc As QuickQuote.CommonObjects.QuickQuoteLocation In Me.Quote?.Locations
                If Loc IsNot Nothing Then
                    '' Add an RV/Watercraft for testing
                    'Dim rvw As New QuickQuoteRvWatercraft
                    'rvw.Year = "2005"
                    'rvw.RvWatercraftTypeId = "1"
                    'rvw.PropertyDeductibleLimitId = "18"
                    'rvw.CostNew = "10000"
                    ''rvw.Premium = "$79.00"
                    'rvw.CoveragesPremium = "$99.00"
                    'Dim mtr As New QuickQuoteRvWatercraftMotor
                    'mtr.CostNew = "$2500"
                    'mtr.Year = "2006"
                    'mtr.Manufacturer = "SEADOO"
                    'mtr.Model = "SD1000"
                    'mtr.MotorTypeId = "2"
                    'mtr.SerialNumber = "1234567"

                    'rvw.RvWatercraftMotors = New List(Of QuickQuoteRvWatercraftMotor)
                    'rvw.RvWatercraftMotors.Add(mtr)

                    'MyLocation.RvWatercrafts = New List(Of QuickQuoteRvWatercraft)
                    'MyLocation.RvWatercrafts.Add(rvw)

                    'MyLocation.RvWatercraftsTotal_CoveragesPremium = "$99.00"
                    ' End add test data

                    rvWaterCraftTotal += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Loc.RvWatercraftsTotal_CoveragesPremium)

                    If Loc.RvWatercrafts IsNot Nothing Then
                        For Each RVW In Loc.RvWatercrafts
                            ' Don't process RV/Watercraft with a type id of -1 (N/A)
                            If RVW.RvWatercraftTypeId >= 0 Then
                                ' Added for task 77959 and 78013 01/25/22 BD
                                If RVW.RvWatercraftTypeId = "6" Then
                                    hasGolfCart = True
                                End If

                                ' Create the data table
                                Dim Type As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, RVW.RvWatercraftTypeId)

                                Dim dedLobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
                                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then
                                    dedLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                                Else
                                    dedLobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
                                End If

                                Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, RVW.PropertyDeductibleLimitId, dedLobType)
                                If IsNumeric(deductible) Then deductible = Format(CDec(deductible), NumFormatNoCents)
                                Dim limit As String = RVW.CostNew
                                If IsNumeric(limit) Then limit = Format(CDec(limit), NumFormatNoCents)

                                Dim CoveragesText As New List(Of String)

                                If RVW.HasLiability And RVW.HasLiabilityOnly = False Then
                                    CoveragesText.Add("PD,Liab")
                                End If
                                If RVW.HasLiability = False And RVW.HasLiabilityOnly Then
                                    CoveragesText.Add("Liab")
                                End If
                                ' Added for task 77959 and 78013 01/25/22 BD
                                If RVW.HasLiability And RVW.HasLiabilityOnly Then
                                    CoveragesText.Add("Liab")
                                    If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso hasGolfCart Then
                                        limit = "Included"
                                        deductible = "N/A"
                                    End If
                                End If
                                If RVW.HasLiability = False And RVW.HasLiabilityOnly = False Then
                                    CoveragesText.Add("PD")
                                End If

                                If RVW.UninsuredMotoristBodilyInjuryLimitId <> "" And RVW.UninsuredMotoristBodilyInjuryLimitId <> "0" Then
                                    CoveragesText.Add("UWBI")
                                End If

                                Dim PremiumSum As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(RVW.CoveragesPremium) ' 0.0

                                Dim desc As String = RVW.Year
                                'Select Case RVW.RvWatercraftTypeId
                                '    Case "3" '3 = Boat Motor Only
                                '        desc = String.Format("{0}, {1} HP", RVW.Year, RVW.HorsepowerCC)
                                '    Case "5" '5 = Accessories
                                '        desc = If(RVW.Description.Length > 40, RVW.Description.Substring(0, 40), RVW.Description)
                                'End Select
                                '' Added for task 77959 and 78013 01/25/22 BD
                                'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso RVW.RvWatercraftTypeId = 6 Then
                                '    RVWCTableFarmGC.Add(New With {.Type = Type, .Year = desc, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limits = limit, .Premium = String.Format("{0:C2}", PremiumSum)})
                                'Else
                                '    RVWCTable.Add(New With {.Type = Type, .Year = desc, .Deductible = deductible, .limits = limit, .Premium = String.Format("{0:C2}", PremiumSum)})
                                'End If
                                'If RVW.RvWatercraftMotors IsNot Nothing AndAlso RVW.RvWatercraftMotors.Count > 0 Then
                                '    For Each motor As QuickQuoteRvWatercraftMotor In RVW.RvWatercraftMotors
                                '        ' Only show motor items that have values
                                '        If FieldHasNumericValue(motor.CostNew, ClassName, lblMsg, True) OrElse motor.Manufacturer <> String.Empty _
                                '        OrElse motor.Model <> String.Empty OrElse motor.SerialNumber <> String.Empty _
                                '        OrElse (motor.Year <> String.Empty AndAlso motor.Year <> "0") Then
                                '            Type = GetRVWMotorType(motor.MotorTypeId)
                                '            limit = motor.CostNew
                                '            If IsNumeric(limit) Then limit = Format(CDec(limit), NumFormatNoCents)
                                '            deductible = ""
                                '            RVWCTable.Add(New With {.Type = Type, .Year = motor.Year, .Deductible = deductible, .limits = limit, .Premium = "Included"})
                                '        End If
                                '    Next
                                'End If

                                Select Case RVW.RvWatercraftTypeId
                                    Case "3" '3 = Boat Motor Only
                                        Dim foundYear As String = String.Empty
                                        Dim foundLimit As String = String.Empty
                                        getBoatMotorValues(RVW, foundYear, foundLimit)
                                        desc = String.Format("{0}, {1} HP", foundYear, RVW.HorsepowerCC)
                                        RVWCTable.Add(New With {.Type = Type, .Year = desc, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limits = foundLimit, .Premium = String.Format("{0:C2}", PremiumSum)})
                                        Continue For

                                    Case "5" '5 = Accessories
                                        desc = If(RVW.Description.Length > 40, RVW.Description.Substring(0, 40), RVW.Description)
                                End Select

                                '' Added for task 77959 and 78013 01/25/22 BD
                                'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso RVW.RvWatercraftTypeId = 6 Then
                                '    RVWCTableFarmGC.Add(New With {.Type = Type, .Year = desc, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limits = limit, .Premium = String.Format("{0:C2}", PremiumSum)})
                                'Else
                                '    RVWCTable.Add(New With {.Type = Type, .Year = desc, .Deductible = deductible, .limits = limit, .Premium = String.Format("{0:C2}", PremiumSum)})
                                'End If
                                RVWCTable.Add(New With {.Type = Type, .Year = desc, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limits = limit, .Premium = String.Format("{0:C2}", PremiumSum)})

                                If RVW.RvWatercraftMotors IsNot Nothing AndAlso RVW.RvWatercraftMotors.Count > 0 Then
                                    For Each motor As QuickQuoteRvWatercraftMotor In RVW.RvWatercraftMotors
                                        ' Only show motor items that have values
                                        If FieldHasNumericValue(motor.CostNew, ClassName, lblMsg, True) OrElse motor.Manufacturer <> String.Empty _
                                            OrElse motor.Model <> String.Empty OrElse motor.SerialNumber <> String.Empty _
                                            OrElse (motor.Year <> String.Empty AndAlso motor.Year <> "0") Then
                                            Type = GetRVWMotorType(motor.MotorTypeId)
                                            limit = motor.CostNew
                                            If IsNumeric(limit) Then limit = Format(CDec(limit), NumFormatNoCents)
                                            deductible = ""
                                            RVWCTable.Add(New With {.Type = Type, .Year = motor.Year, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limits = limit, .Premium = "Included"})
                                        End If
                                    Next
                                End If

                            End If
                        Next
                    End If
                End If
            Next

            ' Create the html
            Dim html As New StringBuilder()
            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

            ' Header Row
            'Removing separate tables because when Golf Cart is on the quote with other RVWatercraft, only Golf Cart shows and the premium shows for all RVWatercraft. Decision was made to show the Coverages column for all RVWatercraft. WS-1155 6/1/2023 MLW
            '' Added for task 77959 and 78013 01/25/22 BD
            'If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso hasGolfCart Then
            '    WriteCell(html, "Type", "width:302px;")
            '    WriteCell(html, "Year", "width:82px;")
            '    WriteCell(html, "Coverage", "text-align:right;width:82px;")
            '    WriteCell(html, "Deductible", "text-align:right;width:82px;")
            '    WriteCell(html, "Limit", "text-align:right;width:82px;")
            '    WriteCell(html, "Premium", "text-align:right;width:82px;")

            '    html.AppendLine("</tr>")
            '    If RVWCTableFarmGC.Any() Then

            '        For Each c In RVWCTableFarmGC
            '            html.AppendLine("<tr>")
            '            WriteCell(html, c.Type)
            '            WriteCell(html, c.Year)
            '            WriteCell(html, c.Coverage, "text-align:right;")
            '            WriteCell(html, c.Deductible, "text-align:right;")
            '            WriteCell(html, c.Limits, "text-align:right;")
            '            WriteCell(html, c.Premium, "text-align:right;")
            '            html.AppendLine("</tr>")
            '        Next
            '        html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
            '        html.AppendLine("<td colspan=""5"" style=""font-weight=bold;"">Total RV and Watercraft Premium</td>")
            '        html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", rvWaterCraftTotal) + "</td>")
            '        html.AppendLine("</tr>")
            '    Else
            '        'no items
            '        html.AppendLine("<tr>")
            '        html.AppendLine("<td colspan=""5"">N/A</td>")
            '        html.AppendLine("</tr>")
            '    End If
            '    html.AppendLine("</table>")
            '    ' Data rows
            'Else
            ' Header Row - CAH 03/23/2023 - Accidentally removed with Changeset 22372 (T77959)
            WriteCell(html, "Type", "width:190px;")
            WriteCell(html, "Year", "width:120px;")
            WriteCell(html, "Coverage", "text-align:right;width:80px;")
            WriteCell(html, "Deductible", "text-align:right;width:80px;")
            WriteCell(html, "Limit", "text-align:right;width:80px;")
            WriteCell(html, "Premium", "text-align:right;width:80px;")
            html.AppendLine("</tr>")

            If RVWCTable.Any() Then

                For Each c In RVWCTable
                    html.AppendLine("<tr>")
                    WriteCell(html, c.Type)
                    WriteCell(html, c.Year)
                    WriteCell(html, c.Coverage, "text-align:right;")
                    WriteCell(html, c.Deductible, "text-align:right;")
                    WriteCell(html, c.Limits, "text-align:right;")
                    WriteCell(html, c.Premium, "text-align:right;")
                    html.AppendLine("</tr>")
                Next
                html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                html.AppendLine("<td colspan=""5"" style=""font-weight=bold;"">Total RV and Watercraft Premium</td>")
                html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", rvWaterCraftTotal) + "</td>")
                html.AppendLine("</tr>")
            Else
                'no items
                html.AppendLine("<tr>")
                html.AppendLine("<td colspan=""6"">N/A</td>")
                html.AppendLine("</tr>")
            End If
            html.AppendLine("</table>")
            'End If
            Me.tblRVWatercraft.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateRVWatercraft", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Private Sub getBoatMotorValues(ByRef RVWatercraftVehicle As QuickQuoteRvWatercraft, ByRef Year As String, ByRef Limit As String)
        If RVWatercraftVehicle IsNot Nothing Then
            ' Try to get values from Motors first (VR saves the data here)
            If RVWatercraftVehicle.RvWatercraftMotors.IsLoaded Then
                Year = RVWatercraftVehicle.RvWatercraftMotors.FirstOrDefault.Year
                Limit = RVWatercraftVehicle.RvWatercraftMotors.FirstOrDefault.CostNew
            End If
            ' Try to get the values from the craft second if they don't exists (Diamond may have them here)
            If Year.TryToGetInt32 = 0 Then
                Year = RVWatercraftVehicle.Year
            End If
            If Limit.TryToGetInt32 = 0 Then
                Limit = RVWatercraftVehicle.CostNew
            End If
        End If
        ' Format the Limit from money
        If IsNumeric(Limit) Then Limit = Format(CDec(Limit), NumFormatNoCents)
    End Sub

    ''' <summary>
    ''' Populate the Additional Coverages section
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateAdditionalCoverages()
        Dim ACTable = {(New With {.Type = "", .Desc = "", .Limits = "", .Premium = ""})}.Take(0).ToList()
        Dim LMTable = {(New With {.Type = "", .Desc = "", .Limits = "", .Premium = ""})}.Take(0).ToList()
        Dim ACTotal As Double = 0
        Dim desc As String = ""
        Dim lim As String = Nothing
        Dim type As String = Nothing
        Dim prem As String = Nothing
        Dim addit As Boolean = False

        Try
            ACTotal = 0

            ' PERSONAL LIABILITY COVERAGE L
            ' Moved from location coverages 9/2/15 MGB
            ' Removed 9/24/15
            'If FieldHasNumericValue(Quote.OccurrenceLiabilityLimit, ClassName, lblMsg, True) Then
            '    type = "L. Personal Liability"
            '    desc = ""
            '    If FieldHasNumericValue(Quote.OccurrenceLiabilityLimit, ClassName, lblMsg, True) Then lim = Format(CDec(Quote.OccurrenceLiabilityLimit), NumFormatNoCents) Else lim = "$0"
            '    prem = "Included"
            '    'ACTotal += CDec(Quote.OccurrencyLiabilityQuotedPremium)
            '    LMTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            'End If

            ' MEDICAL PAYMENTS COVERAGE M
            ' Moved from location coverages 9/2/15 MGB
            ' Removed 9/24/15
            'If FieldHasNumericValue(Quote.MedicalPaymentsLimitid, ClassName, lblMsg, True) Then
            '    type = "M. Medical Payment"
            '    desc = ""
            '    lim = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, Me.Quote.MedicalPaymentsLimitid)
            '    If FieldHasNumericValue(lim, ClassName, lblMsg, False) Then lim = Format(CDec(lim), NumFormatNoCents)
            '    prem = "Included"
            '    'ACTotal += CDec(Quote.MedicalPaymentsQuotedPremium)
            '    LMTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            'End If

            ' Additional Insureds
            If Quote.Locations(0).AdditionalInterests IsNot Nothing AndAlso Quote.Locations(0).AdditionalInterests.Count > 0 Then
                Dim ndx As Integer = 0
                For Each addlint As QuickQuoteAdditionalInterest In Quote.Locations(0).AdditionalInterests
                    If ndx = 0 Then
                        type = "Additional Insured"
                    Else
                        type = ""
                    End If
                    ' Interest type and name
                    desc = GetAdditionalInsuredTypeDescription(addlint.TypeId)
                    If desc <> String.Empty Then desc += " - "
                    desc += addlint.Name.DisplayName
                    lim = ""
                    'lim = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, Me.Quote.MedicalPaymentsLimitid)
                    prem = "Included"
                    'ACTotal += CDec(Quote.MedicalPaymentsQuotedPremium)
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    ndx += 1
                Next
            End If

            ' Employers Liability
            ' Split employee lines into 3 separate lines MGB 9/2/15
            'Updated 9/6/18 for multi state MLW
            'If FieldHasNumericValue(Quote.FarmEmployersLiabilityQuotedPremium, ClassName, lblMsg, True) Then
            If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmEmployersLiabilityQuotedPremium), ClassName, lblMsg, True) Then
                type = "Employer's Liability - Farm Employees"
                lim = ""
                If FieldHasNumericValue(SubQuoteFirst.EmployeesFullTime, ClassName, lblMsg, True) Then
                    'desc = "Full Time Employees (180-365 days): " & Quote.EmployeesFullTime
                    desc = "Full Time(180-365 days): " & SubQuoteFirst.EmployeesFullTime
                Else
                    'desc = "Full Time Employees (180-365 days): 0"
                    desc = "Full Time(180-365 days): 0"
                End If
                'prem = Format(CDec(Quote.FarmEmployersLiabilityQuotedPremium), NumFormatWithCents)
                prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmEmployersLiabilityQuotedPremium)), NumFormatWithCents)
                prem = Format(CDec(prem), NumFormatWithCents)
                ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmEmployersLiabilityQuotedPremium))
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})

                type = ""
                lim = ""
                prem = ""
                If FieldHasNumericValue(SubQuoteFirst.EmployeesPartTime41To179Days, ClassName, lblMsg, True) Then
                    'desc = "Part Time Employees (41-179 days): " & Quote.EmployeesPartTime41To179Days
                    desc = "Part Time(41-179 days): " & SubQuoteFirst.EmployeesPartTime41To179Days
                Else
                    'desc = "Part Time Employees (41-179 days): " & Quote.EmployeesPartTime41To179Days
                    desc = "Part Time(41-179 days): " & SubQuoteFirst.EmployeesPartTime41To179Days
                End If
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})

                type = ""
                lim = ""
                prem = ""
                If FieldHasNumericValue(SubQuoteFirst.EmployeesPartTime1To40Days, ClassName, lblMsg, True) Then
                    'desc = "Part Time Employees (< 42 days): " & Quote.EmployeesPartTime1To40Days
                    desc = "Part Time(< 42 days): " & SubQuoteFirst.EmployeesPartTime1To40Days
                Else
                    'desc = "Part Time Employees (< 42 days): 0"
                    desc = "Part Time(< 42 days): 0"
                End If
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            End If

            'Farm All Star
            If IFM.VR.Common.Helpers.FARM.FarmAllStarHelper.IsFarmAllStarAvailable(Quote) Then
                If SubQuoteFirst.HasFarmAllStar Then
                    type = "Farm All Star"
                    lim = "N/A"
                    desc = ""
                    prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmAllStarQuotedPremium)), NumFormatWithCents)
                    ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmAllStarQuotedPremium))
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    'Water Backup
                    type = "&nbsp;&nbsp; Water Backup"
                    lim = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarLimitId, SubQuoteFirst.FarmAllStarWaterBackupLimitId)
                    If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                    desc = ""
                    prem = "Included"
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    'Water Damage
                    type = "&nbsp;&nbsp; Water Damage"
                    lim = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarWaterDamageLimitId, SubQuoteFirst.FarmAllStarWaterDamageLimitId)
                    If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                    desc = ""
                    prem = "Included"
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
            Else
                'Updated 9/6/18 for multi state MLW
                'If FieldHasNumericValue(Quote.FarmAllStarQuotedPremium, ClassName, lblMsg, True) Then
                If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmAllStarQuotedPremium), ClassName, lblMsg, True) Then
                    type = "Farm All Star"
                    lim = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarLimitId, SubQuoteFirst.FarmAllStarLimitId)
                    If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                    'QQHelper.LoadStaticDataOptionsDropDown(ddlBSDLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)

                    desc = "Backup of Sewer or Drain"
                    prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmAllStarQuotedPremium)), NumFormatWithCents)
                    ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmAllStarQuotedPremium))
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
            End If

            ' Extra Expense
            Dim hasExtraExpense As Boolean = False
            Dim extraExpenseLimit As String = ""
            Dim extraExpensePremium As String = ""
            ' Use the governing state values instead of subquotes MGB 1/31/19 Bug 31175
            Dim extraExpense As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
            If extraExpense IsNot Nothing Then
                hasExtraExpense = True
                If FarmExtenderHelper.IsFarmExtenderAvailable(Quote) Then
                    If extraExpense.Coverage IsNot Nothing Then
                        extraExpenseLimit = FormatNumber(extraExpense.Coverage.ManualLimitAmount.TryToGetInt32, "0")
                    Else
                        extraExpenseLimit = FormatNumber(extraExpense.IncludedLimit.TryToGetInt32 + extraExpense.IncreasedLimit.TryToGetInt32, "0")
                    End If
                Else
                    extraExpenseLimit = extraExpense.IncreasedLimit
                End If
                extraExpensePremium = extraExpense.Premium
            End If
            'For Each sq As QuickQuoteObject In SubQuotes
            '    If sq.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
            '        Dim extraExpense As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
            '        If extraExpense IsNot Nothing Then
            '            hasExtraExpense = True
            '            extraExpenseLimit = extraExpense.IncreasedLimit
            '            extraExpensePremium = QQHelper.getSumAndOptionallyMaintainFormatting(extraExpensePremium, extraExpense.Premium, maintainFormattingOrDefaultValue:=True)
            '        End If
            '    End If
            'Next
            If hasExtraExpense = True Then
                type = "Extra Expense"
                lim = extraExpenseLimit
                desc = ""
                prem = If(Decimal.Parse(extraExpensePremium.Replace("$", "").Replace(",", "")) = 0.0, "Included", extraExpensePremium)
                If prem <> "Included" Then
                    prem = Format(CDec(prem), NumFormatWithCents)
                End If
                ACTotal += CDec(extraExpensePremium)
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            End If
            'If SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
            '    Dim extraExpense As QuickQuoteOptionalCoverage = SubQuoteFirst.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)

            '    If extraExpense IsNot Nothing Then
            '        type = "Extra Expense"
            '        lim = extraExpense.IncreasedLimit
            '        desc = ""
            '        prem = If(Decimal.Parse(extraExpense.Premium.Replace("$", "").Replace(",", "")) = 0.0, "Included", extraExpense.Premium)
            '        ACTotal += CDec(extraExpense.Premium)
            '        ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            '    End If
            'End If

            ' Farm Extender
            'Updated 9/6/18 for multi state MLW
            'If FieldHasNumericValue(Quote.FarmExtenderQuotedPremium, ClassName, lblMsg, True) Then
            If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmExtenderQuotedPremium), ClassName, lblMsg, True) Then
                type = "Farm Extender"
                lim = ""
                desc = ""
                prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmExtenderQuotedPremium)), NumFormatWithCents)
                ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmExtenderQuotedPremium))
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            End If

            ' Equipment Breakdown
            'If FieldHasNumericValue(Quote.FarmEquipmentBreakdownQuotedPremium, ClassName, lblMsg, True) Then
            If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmEquipmentBreakdownQuotedPremium), ClassName, lblMsg, True) Then
                type = "Equipment Breakdown"
                lim = ""
                desc = "Included"
                prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmEquipmentBreakdownQuotedPremium)), NumFormatWithCents)
                ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmEquipmentBreakdownQuotedPremium))
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            End If

            ' Section II coverages
            Dim L As QuickQuoteLocation = Quote.Locations(0)
            Dim quoteForLoc As QuickQuoteObject = Me.SubQuoteForLocation(L) 'Added 9/11/18 for multi state MLW 
            For Each s2Cov As QuickQuoteSectionCoverage In L.SectionCoverages
                type = GetCoverageCodeCaption(s2Cov.Coverages(0).CoverageCodeId, stateQuote:=quoteForLoc) 'Updated 9/11/18 for multi state MLW
                If FieldHasNumericValue(s2Cov.Coverages(0).ManualLimitAmount, ClassName, lblMsg, True) Then
                    lim = Format(CDec(s2Cov.Coverages(0).ManualLimitAmount), NumFormatNoCents)
                Else
                    lim = ""
                End If
                If lim.Trim = "$" Then lim = ""
                If FieldHasNumericValue(s2Cov.Coverages(0).FullTermPremium, ClassName, lblMsg, True) Then
                    prem = Format(CDec(s2Cov.Coverages(0).FullTermPremium), NumFormatWithCents)
                Else
                    prem = "Included"
                End If
                Select Case s2Cov.Coverages(0).CoverageCodeId
                    Case "40054" ' Limited Farm Pollution Liability (Personal Only)
                        'Updated 6/21/2022 for task 71215 MLW
                        If PollutionLiability1MHelper.IsPollutionLiability1MAvailable(Quote) AndAlso FieldHasNumericValue(s2Cov.Coverages(0).FullTermPremium, ClassName, lblMsg, True) Then
                            desc = "Increased Limit"
                        Else
                            desc = ""
                        End If
                        addit = True
                        Exit Select
                    Case "80094" ' Liability Enhancement Endorsement (Commercial Only)
                        'Updated 8/8/2022 for task 76031 MLW
                        If LiabilityEnhancement1MHelper.IsLiabilityEnhancement1MAvailable(Quote) AndAlso FieldHasNumericValue(s2Cov.Coverages(0).FullTermPremium, ClassName, lblMsg, True) Then
                            desc = "Increased Limit"
                        Else
                            'desc = "Included"
                            desc = ""
                        End If
                        addit = True
                        Exit Select
                    Case "80115" ' Custom Farming With Spray
                        'desc = "With Spraying"
                        desc = "W/Spraying"
                        If FieldHasNumericValue(s2Cov.EstimatedReceipts, ClassName, lblMsg, True) Then
                            desc += ";" & Format(CDec(s2Cov.EstimatedReceipts), NumFormatNoCents) & " annual revenue"
                        End If
                        addit = True
                        Exit Select
                    Case "70129" ' Custom Farming Without Spray
                        'desc = "Without Spraying"
                        desc = "WO/Spraying"
                        If FieldHasNumericValue(s2Cov.EstimatedReceipts, ClassName, lblMsg, True) Then
                            desc += ";" & Format(CDec(s2Cov.EstimatedReceipts), NumFormatNoCents) & " annual revenue"
                        End If
                        addit = True
                        Exit Select
                    Case "70201" ' Family Medical Payments
                        If s2Cov.NumberOfPersonsReceivingCare IsNot Nothing AndAlso s2Cov.NumberOfPersonsReceivingCare <> String.Empty Then
                            desc = "Number of Persons: " & s2Cov.NumberOfPersonsReceivingCare
                        Else
                            desc = ""
                        End If
                        addit = True
                        Exit Select
                    Case "70135" ' Incidental Business Pursuits
                        desc = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.BusinessPursuitTypeId, s2Cov.BusinessPursuitTypeId)
                        'desc = GetBusinessPursuitDescription(s2Cov.BusinessPursuitTypeId)
                        If FieldHasNumericValue(s2Cov.EstimatedReceipts, ClassName, lblMsg, True) Then
                            desc += ";" & Format(CDec(s2Cov.EstimatedReceipts), NumFormatNoCents) & " annual revenue"
                        End If
                        addit = True
                        Exit Select
                    Case "70213" ' Identity Fraud Expense
                        desc = "Included"
                        addit = True
                        Exit Select
                    'Case "40045" ' Add'l residence rented to others
                    '    type = "Add'l Res Rented to Others"
                    '    Dim addr As String = ""
                    '    If s2Cov.Address IsNot Nothing Then
                    '        addr = s2Cov.Address.DisplayAddress  ' They decided they want to display the entire address
                    '        'addr += s2Cov.Address.HouseNum
                    '        'If addr.Trim <> "" Then addr += " "
                    '        'addr += s2Cov.Address.StreetName
                    '        'If addr.Trim <> "" Then addr += " "
                    '        'Dim zip5 As String = s2Cov.Address.Zip
                    '        'If s2Cov.Address.Zip.Length > 5 Then zip5 = s2Cov.Address.Zip.Substring(0, 5)
                    '        'addr += zip5
                    '        'If addr.Length > 21 Then addr = addr.Substring(0, 18) & "..."
                    '    End If
                    '    desc = "# of Fam: " & s2Cov.NumberOfFamilies & "; " & addr
                    '    addit = True
                    '    Exit Select
                    Case "80553" ' Motorized Vehicles
                        'type = "Motorized Vehicles (OH)"
                        type = "Motorized Vehicle - OH"   ' use non-plural according to BRD
                        desc = ""
                        lim = ""
                        addit = True
                        Exit Select
                        'Case "80308" ' Canine Exclusion 'Adds multiple-no point without desc
                        '    type = "Exclusion - Canine"
                        '    desc = ""
                        '    lim = "N/A"
                        '    prem = "N/A"
                        '    addit = True
                        '    Exit Select
                    Case Else
                        ' Don't show any other coverages
                        addit = False
                        Exit Select
                End Select
                If addit Then
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    If FieldHasNumericValue(prem, ClassName, lblMsg, True) Then ACTotal += CDec(prem)
                End If
            Next

            'Additional Residence Rented to Others (GL-73)
            Dim addlResidenceRentedToOthersCovs As List(Of QuickQuoteSectionIICoverage) = IFM.VR.Common.Helpers.FARM.AddlResidenceRentedToOthersHelper.GetAddlResidenceRentedToOthersCoverages(Quote)
            If addlResidenceRentedToOthersCovs IsNot Nothing AndAlso addlResidenceRentedToOthersCovs.Count > 0 Then
                For Each addlResidenceCov In addlResidenceRentedToOthersCovs
                    type = "Add'l Res Rented to Others"
                    Dim addr As String = ""
                    If addlResidenceCov.Address IsNot Nothing Then
                        addr = addlResidenceCov.Address.DisplayAddress
                    End If
                    desc = "# of Fam: " & addlResidenceCov.NumberOfFamilies & "; " & addr
                    lim = ""
                    prem = addlResidenceCov.Premium
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    If FieldHasNumericValue(prem, ClassName, lblMsg, True) Then ACTotal += CDec(prem)
                Next
            End If

            'Canine Exclusion - Add only once
            Dim CanineExclusion As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = Nothing
            CanineExclusion = (From cov In L.SectionIICoverages Where cov.CoverageType = QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion Select cov).FirstOrDefault()
            If CanineExclusion IsNot Nothing AndAlso CanineExclusion.Name.FirstName.IsNullEmptyorWhitespace = False AndAlso CanineExclusion.Description.IsNullEmptyorWhitespace = False Then
                type = "Exclusion - Canine"
                desc = ""
                lim = "N/A"
                prem = "N/A"
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                'no need to add premium to Total since it doesn't have one.
            End If

            ' Section II GL-9 Farm Personal Liability  MGB 10/24/19 Bug 20407
            ' Get all of the GL-9 section II coverages from all locations
            Dim GL9s As List(Of QuickQuoteSectionIICoverage) = IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.GetAllQuoteGL9s(Quote)
            ' Loop through all of the GL-9's we just gathered and add them to the display table
            If GL9s.Count > 0 Then
                For Each GL9 In GL9s
                    type = "Farm Personal Liability GL-9"
                    If GL9.Name IsNot Nothing AndAlso (GL9.Name.FirstName IsNot Nothing AndAlso GL9.Name.LastName IsNot Nothing) Then
                        desc = GL9.Name.FirstName + " " & GL9.Name.LastName
                    Else
                        desc = ""
                    End If
                    lim = ""
                    prem = GL9.Premium
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    If FieldHasNumericValue(prem, ClassName, lblMsg, True) Then ACTotal += CDec(prem)
                Next
            End If

            ' Pollution Cleanup and Removal 70152
            'Updated 9/25/18 for multi state MLW
            Dim hasFarmIncidentalLimits As Boolean = False
            Dim farmIncidentalLimit As String = ""
            Dim farmIncidentalIncreasedLimit As String = ""
            Dim farmIncidentalPremium As String = ""
            For Each sq As QuickQuoteObject In SubQuotes
                If sq.FarmIncidentalLimits IsNot Nothing AndAlso sq.FarmIncidentalLimits.Count > 0 Then
                    For Each inc As QuickQuoteFarmIncidentalLimit In sq.FarmIncidentalLimits
                        Select Case inc.CoverageCodeId
                            Case "70152"
                                type = "Pollutant Cleanup & Removal"
                                desc = ""
                                hasFarmIncidentalLimits = True
                                If FieldHasNumericValue(inc.TotalLimit, ClassName, lblMsg, True) Then
                                    farmIncidentalLimit = inc.TotalLimit
                                End If
                                farmIncidentalIncreasedLimit = inc.IncreasedLimitId
                                farmIncidentalPremium = QQHelper.getSumAndOptionallyMaintainFormatting(farmIncidentalPremium, inc.Premium, maintainFormattingOrDefaultValue:=True)
                                Exit Select
                            Case Else
                                Exit Select
                        End Select
                    Next
                End If
            Next
            If hasFarmIncidentalLimits = True Then
                If FieldHasNumericValue(farmIncidentalLimit, ClassName, lblMsg, True) Then
                    lim = farmIncidentalLimit
                    If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
                    If FieldHasNumericValue(farmIncidentalPremium, ClassName, lblMsg, True) Then
                        prem = Format(CDec(farmIncidentalPremium), NumFormatWithCents)
                        ACTotal += CDec(farmIncidentalPremium)
                    Else
                        prem = "Included"
                    End If
                    ' Only display this coverage if the increased limit id is set
                    ' $10,000 is the included limit for this coverage, we only want to display
                    ' if the limit has been increased
                    If FieldHasNumericValue(farmIncidentalIncreasedLimit, ClassName, lblMsg, True) Then
                        ACTable.Add(New With {.Type = type, .Desc = desc, .limits = "25,000", .Premium = prem})
                        'ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    End If
                End If
            End If
            'If SubQuoteFirst.FarmIncidentalLimits IsNot Nothing AndAlso SubQuoteFirst.FarmIncidentalLimits.Count > 0 Then
            '    For Each inc As QuickQuoteFarmIncidentalLimit In SubQuoteFirst.FarmIncidentalLimits
            '        Select Case inc.CoverageCodeId
            '            Case "70152"
            '                type = "Pollutant Cleanup & Removal"
            '                desc = ""
            '                If FieldHasNumericValue(inc.TotalLimit, ClassName, lblMsg, True) Then
            '                    lim = inc.TotalLimit
            '                    If FieldHasNumericValue(lim, ClassName, lblMsg, True) Then lim = Format(CDec(lim), NumFormatNoCents)
            '                    If FieldHasNumericValue(inc.Premium, ClassName, lblMsg, True) Then
            '                        prem = Format(CDec(inc.Premium), NumFormatWithCents)
            '                        ACTotal += CDec(inc.Premium)
            '                    Else
            '                        prem = "Included"
            '                    End If
            '                    ' Only display this coverage if the increased limit id is set
            '                    ' $10,000 is the included limit for this coverage, we only want to display
            '                    ' if the limit has been increased
            '                    If FieldHasNumericValue(inc.IncreasedLimitId, ClassName, lblMsg, True) Then
            '                        ACTable.Add(New With {.Type = type, .Desc = desc, .limits = "25,000", .Premium = prem})
            '                        'ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            '                    End If
            '                End If
            '                Exit Select
            '            Case Else
            '                Exit Select
            '        End Select
            '    Next
            'End If

            'Farm Machinery - Special Coverage - Coverage G
            If SubQuoteFirst.UnscheduledPersonalPropertyCoverage IsNot Nothing AndAlso SubQuoteFirst.UnscheduledPersonalPropertyCoverage.IsLimitedPerilsExtendedCoverage = True Then
                prem = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmMachinerySpecialCoverageG_QuotedPremium)
                ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmMachinerySpecialCoverageG_QuotedPremium))
                ACTable.Add(New With {.Type = "Farm Machinery - Special Coverage - Coverage G", .Desc = "", .limits = "", .Premium = prem})
            End If

            'Property in Transit
            If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit).Count > 0 Then
                Dim propTransit As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit)
                If propTransit IsNot Nothing Then
                    type = "Property in Transit"
                    lim = propTransit.IncludedLimit
                    desc = ""
                    prem = propTransit.Premium
                    ACTotal += CDec(propTransit.Coverage.FullTermPremium)
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
            End If

            If CDate(Quote.EffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
                ' CONTRACT GROWERS - used when effective date is less than cutoff date
                ' Contract Growers - Bug 5594 MGB 9/18/15
                'Updated 9/6/18 for multi state MLW
                'If FieldHasNumericValue(Quote.FarmContractGrowersCareCustodyControlQuotedPremium, ClassName, lblMsg, True) Then
                If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmContractGrowersCareCustodyControlQuotedPremium), ClassName, lblMsg, True) Then
                    type = "Contract Growers CCC"
                    Select Case SubQuoteFirst.FarmContractGrowersCareCustodyControlLimitId
                        Case "55"
                            lim = Format(CDec(250000), NumFormatNoCents)
                            Exit Select
                        Case "34"
                            lim = Format(CDec(500000), NumFormatNoCents)
                            Exit Select
                        Case "34"
                            lim = Format(CDec(500000), NumFormatNoCents)
                            Exit Select
                        Case "56"
                            lim = Format(CDec(1000000), NumFormatNoCents)
                            Exit Select
                        Case Else
                            lim = ""
                            Exit Select
                    End Select
                    desc = "Included"
                    'prem = Format(CDec(Quote.FarmContractGrowersCareCustodyControlQuotedPremium), NumFormatWithCents)
                    'ACTotal += CDec(Quote.FarmContractGrowersCareCustodyControlQuotedPremium)
                    prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmContractGrowersCareCustodyControlQuotedPremium)), NumFormatWithCents)
                    ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmContractGrowersCareCustodyControlQuotedPremium))
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
            Else
                ' CUSTOM FEEDING  - used when effective date >= cutoff date
                ' Swine
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingSwineLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingSwineLimitId <> "0" Then
                    Dim templim As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingSwineLimitId, SubQuoteFirst.FarmCustomFeedingSwineLimitId)
                    type = "Custom Feeding"
                    lim = templim
                    'lim = Format(CDec(templim), NumFormatNoCents)
                    desc = "Swine - " & SubQuoteFirst.FarmCustomFeedingSwineDescription
                    prem = SubQuoteFirst.FarmCustomFeedingSwineQuotedPremium
                    ACTotal += CDec(prem)
                    'prem = Format(CDec(prem), "C0")
                    prem = Format(CDec(prem), NumFormatWithCents)
                    If prem > 0 Then ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
                ' Poultry
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingPoultryLimitId <> "0" Then
                    Dim templim As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingPoultryLimitId, SubQuoteFirst.FarmCustomFeedingPoultryLimitId)
                    type = "Custom Feeding"
                    lim = templim
                    'lim = Format(CDec(templim), NumFormatNoCents)
                    desc = "Poultry - " & SubQuoteFirst.FarmCustomFeedingPoultryDescription
                    prem = SubQuoteFirst.FarmCustomFeedingPoultryQuotedPremium
                    ACTotal += CDec(prem)
                    'prem = Format(CDec(prem), "C0")
                    prem = Format(CDec(prem), NumFormatWithCents)
                    If prem > 0 Then ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
                ' Cattle
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingCattleLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingCattleLimitId <> "0" Then
                    Dim templim As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingCattleLimitId, SubQuoteFirst.FarmCustomFeedingCattleLimitId)
                    type = "Custom Feeding"
                    'lim = Format(CDec(templim), NumFormatNoCents)
                    lim = templim
                    desc = "Cattle - " & SubQuoteFirst.FarmCustomFeedingCattleDescription
                    prem = SubQuoteFirst.FarmCustomFeedingCattleQuotedPremium
                    ACTotal += CDec(prem)
                    'prem = Format(CDec(prem), "C0")
                    prem = Format(CDec(prem), NumFormatWithCents)
                    If prem > 0 Then ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
                If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingEquineLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingEquineLimitId <> "0" Then
                    Dim templim As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingEquineLimitId, SubQuoteFirst.FarmCustomFeedingEquineLimitId)
                    type = "Custom Feeding"
                    'lim = Format(CDec(templim), NumFormatNoCents)
                    lim = templim
                    desc = "Equine - " & SubQuoteFirst.FarmCustomFeedingEquineDescription
                    prem = SubQuoteFirst.FarmCustomFeedingEquineQuotedPremium
                    ACTotal += CDec(prem)
                    'prem = Format(CDec(prem), "C0")
                    prem = Format(CDec(prem), NumFormatWithCents)
                    If prem > 0 Then ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
            End If

            ' EPLI - Bug 5603 MGB 9/21/15
            ' Bug 43384 - Coverage type must be 22 (epli non-uw) in order to display EPLI.  MGB 11/16/2020
            If SubQuoteFirst.HasEPLI AndAlso SubQuoteFirst.EPLICoverageTypeID = "22" Then
                type = "EPLI (non-underwritten)"
                lim = "Policy/Aggregate: " & SubQuoteFirst.EPLICoverageLimit
                If FieldHasNumericValue(SubQuoteFirst.EPLIDeductible, ClassName, lblMsg, False) Then
                    desc = "Deductible: " & Format(CDec(SubQuoteFirst.EPLIDeductible), NumFormatNoCents)
                Else
                    desc = ""
                End If
                'Updated 9/6/18 for multi state MLW
                'If FieldHasNumericValue(Quote.EPLIPremium, ClassName, lblMsg, True) Then
                '    prem = Format(CDec(Quote.EPLIPremium), NumFormatWithCents)
                '    ACTotal += CDec(Quote.EPLIPremium)
                'Else
                '    prem = "Included"
                'End If
                If FieldHasNumericValue(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EPLIPremium), ClassName, lblMsg, True) Then
                    prem = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EPLIPremium)), NumFormatWithCents)
                    ACTotal += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EPLIPremium))
                Else
                    prem = "Included"
                End If
                ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
            End If

            ' Loss of Income - Bug 50241
            For Each LOC As QuickQuoteLocation In Quote.Locations
                If LOC.IncomeLosses IsNot Nothing Then
                    For Each IncomeLossItem As QuickQuoteIncomeLoss In LOC.IncomeLosses
                        type = "Loss of Income"
                        lim = IncomeLossItem.Limit
                        ' Description - location & Building
                        desc = ""
                        Dim temp As String() = IncomeLossItem.Description.Split("BLD")
                        If temp IsNot Nothing AndAlso temp.Length = 2 Then
                            Dim locNum As String = temp(0).Remove(0, 3)
                            Dim bldnum As String = temp(1).Remove(0, 2)
                            desc = "Location " & locNum & ", Building " & bldnum
                        End If
                        If IsNumeric(IncomeLossItem.QuotedPremium) Then
                            prem = Format(CDec(IncomeLossItem.QuotedPremium), NumFormatNoCentsWithDollarSign)
                            ACTotal += CDec(IncomeLossItem.QuotedPremium)
                        Else
                            prem = ""
                        End If
                        ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                    Next
                End If
            Next

            'Ref Food Spoilage
            If Common.Helpers.FARM.RefFoodSpoilageHelper.IsRefFoodSpoilageAvailable(Quote) Then
                If Quote.Locations(MyLocationIndex).IncidentalDwellingCoverages.FindAll(Function(p) p.CoverageCodeId = "70148").Count > 0 Then
                    Dim RefFoodSpoilage As QuickQuoteCoverage
                    RefFoodSpoilage = Quote.Locations(MyLocationIndex).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70148")
                    type = "Refrigerated Food Spoilage"
                    desc = ""
                    lim = Format(CDec(RefFoodSpoilage.ManualLimitAmount), NumFormatNoCents)
                    If QQHelper.IsPositiveIntegerString(RefFoodSpoilage.ManualLimitIncreased) Then
                        'lim = Format(CDec(RefFoodSpoilage.ManualLimitAmount), NumFormatNoCents)
                        prem = Format(CDec(RefFoodSpoilage.FullTermPremium), NumFormatWithCents)
                        ACTotal += CDec(RefFoodSpoilage.FullTermPremium)
                    Else
                        'lim = String.Empty
                        prem = "Included"
                    End If
                    ACTable.Add(New With {.Type = type, .Desc = desc, .limits = lim, .Premium = prem})
                End If
            End If

            ' Create the html
            Dim html As New StringBuilder()
            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

            ' Header Row
            'WriteCell(html, "Type", "width:175pxpx;")
            WriteCell(html, "Coverage", "width:160px;")
            WriteCell(html, "Description", "width:245pxpx;")
            WriteCell(html, "Limits", "text-align:right;width:85px;")
            WriteCell(html, "Premium", "text-align:right;width:85px;")
            html.AppendLine("</tr>")

            ' L & M Data rows
            If LMTable.Any() Then
                For Each c In LMTable
                    html.AppendLine("<tr>")
                    WriteCell(html, c.Type)
                    WriteCell(html, c.Desc)
                    WriteCell(html, c.Limits, "text-align:right;")
                    WriteCell(html, c.Premium, "text-align:right;")
                    html.AppendLine("</tr>")
                Next
                'Else
                '    'no items
                '    html.AppendLine("<tr>")
                '    html.AppendLine("<td colspan=""4"">N/A</td>")
                '    html.AppendLine("</tr>")
            End If

            ' Additional Coverages Data rows
            If ACTable.Any() Then
                'html.AppendLine("<tr><td colspan=""4"">&nbsp;</td></tr>")
                'html.AppendLine("<tr>")
                'html.AppendLine("<td colspan=""4"" style=""font-weight:bold;"">Additional Coverages</td>")
                'html.AppendLine("</tr>")
                For Each c In ACTable
                    html.AppendLine("<tr>")
                    WriteCell(html, c.Type)
                    WriteCell(html, c.Desc)
                    WriteCell(html, c.Limits, "text-align:right;")
                    WriteCell(html, c.Premium, "text-align:right;")
                    html.AppendLine("</tr>")
                Next

                html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                html.AppendLine("<td colspan=""3"" style=""font-weight=bold;"">Total Additional Coverages Premium</td>")
                html.AppendLine("<td style=""text-align:right;"">" + String.Format("{0:C2}", ACTotal) + "</td>")
                html.AppendLine("</tr>")
                'Else
                '    'no items
                '    html.AppendLine("<tr>")
                '    html.AppendLine("<td colspan=""4"">N/A</td>")
                '    html.AppendLine("</tr>")
            End If

            html.AppendLine("</table>")

            Me.tblAdditionalCoverages.Text = html.ToString()

            Exit Sub
        Catch ex As Exception
            HandleError(ClassName, "PopulateAdditionalCoverages", ex, lblMsg)
            Exit Sub
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        ' no saving needed
        Return True
    End Function

    Private Function GetLocationAcreageTypeDescription(ByVal LocationAcreageType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing

        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM LocationAcreageType WHERE LocationAcreageType_Id = " & QQHelper.IntegerForString(LocationAcreageType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Acreage Type Description value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            HandleError(ClassName, "GetLocationAcreageTypeDescription", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetBusinessPursuitDescription(ByVal BusinessPursuit_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM BusinessPursuitType WHERE businesspursuittype_Id = " & QQHelper.IntegerForString(BusinessPursuit_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Business Pursuit Description value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            HandleError(ClassName, "GetBusinessPursuitDescription", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetCoverageDescription(ByVal CoverageCode_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM CoverageCode WHERE CoverageCode_Id = " & QQHelper.IntegerForString(CoverageCode_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Coverage Description value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            HandleError(ClassName, "GetCoverageDescription", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetAdditionalInsuredTypeDescription(ByVal AdditionalInterestType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM AdditionalInterestType WHERE AdditionalInterestType_Id = " & QQHelper.IntegerForString(AdditionalInterestType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Additional Interest Type Description value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            HandleError(ClassName, "GetAdditionalInsuredTypeDescription", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetFarmStructureTypeDescription(ByVal FarmStructureType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM FarmStructureType WHERE FarmStructureType_Id = " & QQHelper.IntegerForString(FarmStructureType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Farm Structure Type value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            HandleError(ClassName, "GetFarmStructureTypeDescription", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetFarmConstructionTypeDescription(ByVal FarmConstructionType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM FarmConstructionType WHERE FarmConstructionType_Id = " & QQHelper.IntegerForString(FarmConstructionType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn Is Nothing Then Throw New Exception("Returned Farm Construction Type value is nothing!")
            Return rtn.ToString
        Catch ex As Exception
            HandleError(ClassName, "FarmConstructionType_Id", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
        End Try
    End Function

    Private Function GetCoverageCodeCaption(ByVal CoverageCode_Id As String, Optional ByVal RemoveIncreasedLimitsText As Boolean = True, Optional ByVal stateQuote As QuickQuoteObject = Nothing) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()
        Dim desc As String = ""
        Dim qqVersionId As String = ""
        Dim sql As String = ""

        Try
            ' Get the quickquote version id from the quote object
            'Updated 9/12/18 for multi state MLW
            'qqVersionId = Quote.VersionId
            If stateQuote IsNot Nothing Then
                qqVersionId = stateQuote.VersionId
            End If
            If QQHelper.IsPositiveIntegerString(qqVersionId) = False Then
                qqVersionId = Quote.VersionId
            End If

            If qqVersionId Is Nothing OrElse qqVersionId = String.Empty Then Throw New Exception("Error getting version id")

            ' Get the caption
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            sql = "SELECT COALESCE(CCV.caption, CC.dscr) AS CoverageDescription, * FROM CoverageCode AS cc WITH (NOLOCK) "
            sql += "LEFT JOIN CoverageCodeVersion AS ccv WITH (NOLOCK) ON ccv.coveragecode_id = cc.coveragecode_id "
            sql += "AND ccv.version_id = " & QQHelper.IntegerForString(qqVersionId) & " WHERE cc.coveragecode_id = " & QQHelper.IntegerForString(CoverageCode_Id)
            cmd.CommandText = sql
            da.SelectCommand = cmd
            da.Fill(tbl)
            If tbl Is Nothing OrElse tbl.Rows.Count <= 0 Then Return ""

            desc = tbl(tbl.Rows.Count - 1)("caption").ToString()
            If desc.Trim = "" Then
                ' added 6/19/2019 MGB sometimes the caption is not there so use the description if there is one
                If Not IsDBNull(tbl(tbl.Rows.Count - 1)("CoverageDescription")) Then
                    desc = tbl(tbl.Rows.Count - 1)("CoverageDescription").ToString()
                End If
            End If
            If RemoveIncreasedLimitsText Then desc = desc.Replace("Increased Limits", "")

            Return desc
        Catch ex As Exception
            HandleError(ClassName, "GetCoverageCodeCaption", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            tbl.Dispose()
            da.Dispose()
        End Try
    End Function

    Private Function GetRVWMotorType(ByVal RVWaterCraftMotorType_Id As String) As String
        Dim conn As New SqlConnection()
        Dim cmd As New SqlCommand()
        Dim rtn As Object = Nothing
        Dim da As New SqlDataAdapter()
        Dim tbl As New DataTable()

        Try
            conn = New SqlConnection(AppSettings("connDiamond"))
            conn.Open()
            cmd.Connection = conn
            cmd.CommandType = CommandType.Text
            cmd.CommandText = "SELECT dscr FROM MotorType WHERE motortype_id = " & QQHelper.IntegerForString(RVWaterCraftMotorType_Id)
            rtn = cmd.ExecuteScalar()
            If rtn IsNot Nothing Then
                Return rtn.ToString()
            Else
                Return ""
            End If
        Catch ex As Exception
            HandleError(ClassName, "GetRVWMotorType", ex, lblMsg)
            Return ""
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
            conn.Dispose()
            cmd.Dispose()
            tbl.Dispose()
            da.Dispose()
        End Try
    End Function

    Private Function FormatLocationAddress(ByVal Loc As QuickQuoteLocation) As String
        Try
            Dim zip As String = Loc.Address.Zip
            If zip.Length > 5 Then
                zip = zip.Substring(0, 5)
            End If
            Return String.Format("{0} {1} {2} {3} {4} {5} {6}", Loc.Address.HouseNum, Loc.Address.StreetName, If(String.IsNullOrWhiteSpace(Loc.Address.ApartmentNumber) = False, "Apt# " + Loc.Address.ApartmentNumber, ""), Loc.Address.POBox, Loc.Address.City, Loc.Address.State, zip).Replace("  ", " ").Trim()
        Catch ex As Exception
            HandleError(ClassName, "FormatLocationAddress", ex, lblMsg)
            Return ""
        End Try
    End Function

    'added 2/19/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQuoteSummaryActions.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

#End Region

#Region "Events"

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True

        If Not IsPostBack Then
            Me.ValidationHelper.GroupName = String.Format("Quote Summary")
            If IsAppPageMode Then
                Session("SumType") = "App"
            Else
                Session("SumType") = ""
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Protected Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        'Response.Redirect(String.Format("~/Reports/FAR/PFQuoteSummary_FAR.aspx?quoteid={0}&summarytype={1}", Request.QueryString("QuoteId").ToString, Session("SumType")))
        'updated 5/8/2019
        Dim quoteOrPolicyInfo As String = ""
        If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "|" & Me.ReadOnlyPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "|" & Me.EndorsementPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
            quoteOrPolicyInfo = "quoteid=" & Me.QuoteId
        End If
        If String.IsNullOrWhiteSpace(quoteOrPolicyInfo) = False Then
            Dim sumType As String = ""
            If Session("SumType") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Session("SumType").ToString) = False AndAlso UCase(Session("SumType").ToString) = "APP" Then
                sumType = "App"
            End If
            Response.Redirect(String.Format("~/Reports/FAR/PFQuoteSummary_FAR.aspx?{0}&summarytype={1}", quoteOrPolicyInfo, sumType))
        End If
    End Sub

    Private Sub ShowIRPMScreen() Handles ctlQuoteSummaryActions.RequestNavigationToIRPM
        RaiseEvent RequestNavToIRPM()
    End Sub

#End Region

#Region "Old Code"
    'Private Sub PopulateRvWaterCraft()
    '    Dim opCovList = {(New With {.Type = "", .Description = "", .Coverage = "", .Deductible = "", .Limit = "", .Premium = ""})}.Take(0).ToList()
    '    Dim rvWaterCraftTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.RvWatercraftsTotal_CoveragesPremium)

    '    Try
    '        If Me.MyLocation IsNot Nothing Then
    '            If Me.MyLocation.RvWatercrafts IsNot Nothing Then
    '                For Each cov In Me.MyLocation.RvWatercrafts
    '                    Dim Type As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, cov.RvWatercraftTypeId)

    '                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, cov.PropertyDeductibleLimitId, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
    '                    Dim limit As String = String.Format("{0:N0}", IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CostNew))

    '                    Dim CoveragesText As New List(Of String)

    '                    If cov.HasLiability And cov.HasLiabilityOnly = False Then
    '                        CoveragesText.Add("PD,Liab")
    '                    End If
    '                    If cov.HasLiability = False And cov.HasLiabilityOnly Then
    '                        CoveragesText.Add("Liab")
    '                    End If
    '                    If cov.HasLiability = False And cov.HasLiabilityOnly = False Then
    '                        CoveragesText.Add("PD")
    '                    End If

    '                    If cov.UninsuredMotoristBodilyInjuryLimitId <> "" And cov.UninsuredMotoristBodilyInjuryLimitId <> "0" Then
    '                        CoveragesText.Add("UWBI")
    '                    End If

    '                    Dim PremiumSum As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CoveragesPremium) ' 0.0
    '                    'For Each c In cov.Coverages
    '                    '    PremiumSum += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(c.WrittenPremium)
    '                    'Next

    '                    Dim desc As String = cov.Year
    '                    Select Case cov.RvWatercraftTypeId
    '                        Case "3" '3 = Boat Motor Only
    '                            desc = String.Format("{0}, {1} HP", cov.Year, cov.HorsepowerCC)
    '                        Case "5" '5 = Accessories
    '                            desc = If(cov.Description.Length > 40, cov.Description.Substring(0, 40), cov.Description)
    '                    End Select

    '                    opCovList.Add(New With {.Type = Type, .Description = desc, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limit = limit, .Premium = String.Format("{0:C2}", PremiumSum)})
    '                    'rvWaterCraftTotal += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(PremiumSum)
    '                Next
    '            End If
    '        End If

    '        Dim html As New StringBuilder()
    '        html.AppendLine("<div class=""hom_qs_Main_Sections"">")
    '        html.AppendLine("<span class=""qs_section_headers"">RV and Watercraft</span>")
    '        html.AppendLine("<div class=""hom_qs_Sub_Sections"">")
    '        html.AppendLine("<table class=""hom_qa_table_shades"">")
    '        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

    '        WriteCell(html, "Type")
    '        WriteCell(html, "Description")
    '        WriteCell(html, "Coverage", "", "qs_Grid_cell_premium")
    '        WriteCell(html, "Deductible", "", "qs_Grid_cell_premium")
    '        WriteCell(html, "Limit <sup>3</sup>", "", "qs_Grid_cell_premium")
    '        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

    '        html.AppendLine("</tr>")
    '        If opCovList.Any() Then
    '            For Each c In opCovList.OrderBy(Function(x)
    '                                                Return x.Type
    '                                            End Function)
    '                html.AppendLine("<tr>")

    '                WriteCell(html, c.Type)
    '                WriteCell(html, c.Description)
    '                WriteCell(html, c.Coverage)
    '                WriteCell(html, c.Deductible)
    '                WriteCell(html, c.Limit)
    '                WriteCell(html, c.Premium)

    '                html.AppendLine("</tr>")
    '            Next

    '            html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
    '            html.AppendLine("<td colspan=""5"" style=""font-weight=bold;"">Total RV and Watercraft Premium</td>")
    '            html.AppendLine("<td>" + String.Format("{0:C2}", rvWaterCraftTotal) + "</td>")
    '            html.AppendLine("</tr>")
    '        Else
    '            'no items
    '            html.AppendLine("<tr>")
    '            html.AppendLine("<td colspan=""6"">N/A</td>")
    '            html.AppendLine("</tr>")
    '        End If
    '        html.AppendLine("</table>")
    '        html.AppendLine("<br/><sup>1</sup>Limit values are rounded to the next highest $1000.")
    '        html.AppendLine("<br/><sup>2</sup>Actual Loss Sustained within 12 months of a covered loss")
    '        html.AppendLine("<br/><sup>3</sup>Limit values are rounded to the next highest $100.")
    '        html.AppendLine("<br/><sup>4</sup>Coverage E and F Limits are extended to this endorsement.")
    '        html.AppendLine("<br/><sup>5</sup>Coverage E limit is extended to this endorsement.")
    '        html.AppendLine("</div>")
    '        html.AppendLine("</div>")
    '        Me.tblRvWaterCraft.Text = html.ToString()

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError(ClassName, "PopulateRVWatercraft", ex, lblMsg)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Sub PopulateInlandMarine()
    '    Dim opCovList = {(New With {.Name = "", .Description = "", .Deductible = "", .Limit = "", .Premium = ""})}.Take(0).ToList()
    '    Dim sortOrder As New Dictionary(Of Int32, Int32)

    '    Try
    '        sortOrder.Add(0, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
    '        sortOrder.Add(1, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)
    '        sortOrder.Add(2, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)
    '        sortOrder.Add(3, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)
    '        sortOrder.Add(4, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)
    '        sortOrder.Add(5, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)
    '        sortOrder.Add(6, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)
    '        sortOrder.Add(7, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
    '        sortOrder.Add(8, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)
    '        sortOrder.Add(9, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)
    '        sortOrder.Add(10, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)
    '        sortOrder.Add(11, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)

    '        sortOrder.Add(12, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)
    '        sortOrder.Add(13, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)
    '        sortOrder.Add(14, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)
    '        sortOrder.Add(15, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)

    '        Dim inlandMarineTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.InlandMarinesTotal_CoveragePremium)
    '        Dim rvWaterTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.RvWatercraftsTotal_Premium)
    '        If Me.MyLocation IsNot Nothing Then
    '            If Me.MyLocation.InlandMarines IsNot Nothing Then
    '                Dim sortedInlandMarineItems = From i In Me.MyLocation.InlandMarines
    '                                      Join type In sortOrder On i.InlandMarineType Equals type.Value
    '                                      Order By type.Key
    '                                      Select i

    '                For Each cov In sortedInlandMarineItems
    '                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
    '                    Dim inlandType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, cov.CoverageCodeId)
    '                    If cov.InlandMarineType = QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I Then
    '                        inlandType = "Silverware"
    '                    Else
    '                        inlandType = inlandType.Replace("_", " ").Replace("Inland Marine", "")
    '                    End If

    '                    opCovList.Add(New With {.Name = inlandType, .Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cov.Description.ToLower()), .Deductible = deductible, .Limit = cov.IncreasedLimit, .Premium = String.Format("{0:C2}", IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CoveragePremium))})
    '                Next
    '            End If
    '        End If

    '        Dim html As New StringBuilder()
    '        html.AppendLine("<div class=""hom_qs_Main_Sections"">")
    '        html.AppendLine("<span class=""qs_section_headers"">Inland Marine</span>")
    '        html.AppendLine("<div class=""hom_qs_Sub_Sections"">")
    '        html.AppendLine("<table class=""hom_qa_table_shades"">")
    '        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

    '        WriteCell(html, "Coverage")
    '        WriteCell(html, "Description")
    '        WriteCell(html, "Deductible", "", "qs_Grid_cell_premium")
    '        WriteCell(html, "Limit <sup>3</sup>", "", "qs_Grid_cell_premium")
    '        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")
    '        html.AppendLine("</tr>")

    '        If opCovList.Any() Then
    '            For Each c In opCovList
    '                html.AppendLine("<tr>")
    '                WriteCell(html, c.Name)
    '                WriteCell(html, c.Description)
    '                WriteCell(html, c.Deductible)
    '                WriteCell(html, c.Limit)
    '                WriteCell(html, c.Premium)

    '                html.AppendLine("</tr>")
    '            Next
    '        End If

    '        Dim inlandMarineMin As Double = 25

    '        If (inlandMarineTotal + rvWaterTotal) <> 0 Then
    '            If (inlandMarineTotal + rvWaterTotal) < inlandMarineMin Then
    '                'min line
    '                html.AppendLine("<tr>")
    '                html.AppendLine("<td style=""height: 20px;"" colspan=""5""/>")
    '                html.AppendLine("</tr>")
    '                html.AppendLine("<tr>")
    '                html.AppendLine("<td colspan=""2"" style=""font-weight=bold;"">Additional Amount to meet minimum</td>")
    '                html.AppendLine("<td>N/A</td>")
    '                html.AppendLine("<td>N/A</td>")
    '                html.AppendLine("<td>" + String.Format("{0:C2}", inlandMarineMin - (inlandMarineTotal + rvWaterTotal)) + "</td>")
    '                html.AppendLine("</tr>")

    '                html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
    '                html.AppendLine("<td colspan=""4"" style=""font-weight=bold;"">Total Inland Marine Premium</td>")
    '                html.AppendLine("<td>" + String.Format("{0:C2}", inlandMarineTotal + (inlandMarineMin - (inlandMarineTotal + rvWaterTotal))) + "</td>")
    '                html.AppendLine("</tr>")
    '            Else
    '                If inlandMarineTotal = 0.0 Then
    '                    ' no inland marine items
    '                    html.AppendLine("<tr>")
    '                    html.AppendLine("<td colspan=""5"">N/A</td>")
    '                    html.AppendLine("</tr>")
    '                Else
    '                    html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
    '                    html.AppendLine("<td colspan=""4"" style=""font-weight=bold;"">Total Inland Marine Premium</td>")
    '                    html.AppendLine("<td>" + String.Format("{0:C2}", inlandMarineTotal) + "</td>")
    '                    html.AppendLine("</tr>")
    '                End If
    '            End If
    '        Else
    '            ' no inland marine items
    '            html.AppendLine("<tr>")
    '            html.AppendLine("<td colspan=""5"">N/A</td>")
    '            html.AppendLine("</tr>")
    '        End If

    '        html.AppendLine("</table>")
    '        html.AppendLine("</div>")
    '        html.AppendLine("</div>")

    '        Me.tblInlandMarine.Text = html.ToString()

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError(ClassName, "PopulateInlandMarine", ex, lblMsg)
    '        Exit Sub
    '    End Try
    'End Sub

    'Private Function GetHOMLosses() As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)
    '    Try
    '        If Me.Quote IsNot Nothing Then
    '            If Me.Quote.LobId = "2" Then
    '                Dim lossList As New List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)()
    '                If Me.Quote.LossHistoryRecords IsNot Nothing Then
    '                    lossList.AddRange(Me.Quote.LossHistoryRecords)
    '                End If
    '                If Me.Quote.Applicants IsNot Nothing Then
    '                    For Each app In Me.Quote.Applicants
    '                        If app.LossHistoryRecords IsNot Nothing Then
    '                            lossList.AddRange(app.LossHistoryRecords)
    '                        End If
    '                    Next
    '                End If

    '                Return If(lossList.Any(), lossList, Nothing)
    '            End If
    '        End If
    '        Return Nothing
    '    Catch ex As Exception
    '        HandleError(ClassName, "GetHOMLosses", ex, lblMsg)
    '        Return Nothing
    '    End Try
    'End Function

    '    Private Sub PopulateAdditionalDwellings()
    '        Dim html As New StringBuilder()

    '        Dim opCovList = {(New With {.Address = "", .Construction = "", .Year = "", .SqrFeet = "", .Structure = "", .Territory = "", .ProtectionClass = "", .Premium = ""})}.Take(0).ToList()

    '        Try
    '            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing Then
    '                Dim index As Int32 = 0
    '                For Each l In Me.Quote.Locations
    '                    If index = 0 Then GoTo SkipLocation
    '                    Dim zip As String = l.Address.Zip
    '                    If zip.Length > 5 Then
    '                        zip = zip.Substring(0, 5)
    '                    End If
    '                    Dim address As String = String.Format("{0} {1} {2} {3}</br>{4} {5} {6}", l.Address.HouseNum, l.Address.StreetName, If(String.IsNullOrWhiteSpace(l.Address.ApartmentNumber) = False, "Apt# " + l.Address.ApartmentNumber, ""), l.Address.POBox, l.Address.City, l.Address.State, zip).Replace("  ", " ").Trim()
    '                    Dim construction As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, l.ConstructionTypeId)
    '                    Dim structureType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.StructureTypeId, l.StructureTypeId)

    '                    opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
    '                                    .Structure = structureType, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = l.PremiumFullterm})
    'SkipLocation:
    '                    index += 1
    '                Next

    '                ' This needs to start at 1 because that's where the additional locations will begin in the location list
    '                ' (location 0 is primary dwelling)
    '                Dim i As Int32 = 1
    '                For Each c In opCovList
    '                    html.AppendLine("<table class=""hom_qa_table_shades"">")
    '                    html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

    '                    WriteCell(html, "Address", "width: 120px;")
    '                    WriteCell(html, "Const", "width: 60px;")
    '                    WriteCell(html, "Year")
    '                    WriteCell(html, "Sq Ft")
    '                    WriteCell(html, "Structure", "width: 60px;")
    '                    WriteCell(html, "Terr")
    '                    WriteCell(html, "P.C.")
    '                    WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

    '                    html.AppendLine("</tr>")
    '                    html.AppendLine("<tr>")

    '                    WriteCell(html, c.Address)
    '                    WriteCell(html, c.Construction)
    '                    WriteCell(html, c.Year)
    '                    WriteCell(html, c.SqrFeet)
    '                    WriteCell(html, c.Structure)
    '                    WriteCell(html, c.Territory)
    '                    WriteCell(html, c.ProtectionClass)
    '                    WriteCell(html, c.Premium)

    '                    html.AppendLine("</tr>")
    '                    html.Append(PopulateCoverageSummary(i))
    '                    i += 1
    '                Next
    '                html.AppendLine("</table>")
    '            End If

    '            Me.tblAdditionalLocations.Text = html.ToString()

    '            Exit Sub
    '        Catch ex As Exception
    '            HandleError("PopulateAdditionalDwellings", ex)
    '            Exit Sub
    '        End Try
    '    End Sub

    'Private Sub PopulatePrimaryLocations()
    '    Dim html As New StringBuilder()

    '    html.AppendLine("<table class=""hom_qa_table_shades"">")
    '    html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

    '    WriteCell(html, "Address", "width: 120px;")
    '    WriteCell(html, "Const", "width: 60px;")
    '    WriteCell(html, "Year")
    '    WriteCell(html, "Sq Ft")
    '    WriteCell(html, "Structure", "width: 120px;") 'was 60px
    '    WriteCell(html, "Territory")
    '    'WriteCell(html, "P.C.")
    '    WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

    '    html.AppendLine("</tr>")

    '    Dim opCovList = {(New With {.Address = "", .Construction = "", .Year = "", .SqrFeet = "", .Structure = "", .Deductible = "", .Territory = "", .ProtectionClass = "", .Premium = ""})}.Take(0).ToList()

    '    If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing Then
    '        Dim index As Int32 = 0
    '        For Each l In Me.Quote.Locations
    '            Dim zip As String = l.Address.Zip
    '            If zip.Length > 5 Then
    '                zip = zip.Substring(0, 5)
    '            End If
    '            Dim address As String = String.Format("{0} {1} {2} {3}</br>{4} {5} {6}", l.Address.HouseNum, l.Address.StreetName, If(String.IsNullOrWhiteSpace(l.Address.ApartmentNumber) = False, "Apt# " + l.Address.ApartmentNumber, ""), l.Address.POBox, l.Address.City, l.Address.State, zip).Replace("  ", " ").Trim()
    '            Dim construction As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, l.ConstructionTypeId)
    '            Dim structureType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.StructureTypeId, l.StructureTypeId)

    '            Dim deductibleLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, l.DeductibleLimitId)

    '            opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
    '                            .Structure = structureType, .Deductible = deductibleLimit, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = l.PremiumFullterm})
    '            index += 1
    '            Exit For ' only want first location
    '        Next

    '        For Each c In opCovList
    '            html.AppendLine("<tr>")

    '            WriteCell(html, c.Address)
    '            WriteCell(html, c.Construction)
    '            WriteCell(html, c.Year)
    '            WriteCell(html, c.SqrFeet)
    '            WriteCell(html, c.Structure)
    '            WriteCell(html, c.Territory)
    '            'WriteCell(html, c.ProtectionClass)
    '            WriteCell(html, c.Premium)

    '            html.AppendLine("</tr>")
    '        Next

    '    End If

    '    html.AppendLine("</table>")

    '    Me.tblLocations.Text = html.ToString()
    'End Sub

    'Private Sub PopulateIncludedCoverages()
    '    Dim html As New StringBuilder()

    '    html.AppendLine("<table class=""hom_qa_table_shades"">")
    '    html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

    '    WriteCell(html, "Coverage")
    '    WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
    '    WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

    '    html.AppendLine("</tr>")

    '    Dim opCovList = {(New With {.Name = "", .Limit = "", .Premium = "", .Style = ""})}.Take(0).ToList()

    '    'don't show section on ML2 and ML4
    '    If Me.MyLocation.FormTypeId <> "6" And Me.MyLocation.FormTypeId <> "7" Then

    '        opCovList.Add(New With {.Name = "Business Property Increased Limits", .Limit = "2,500", .Premium = "Included", .Style = ""})
    '        opCovList.Add(New With {.Name = "Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage", .Limit = "2,500", .Premium = "Included", .Style = ""})

    '        opCovList.Add(New With {.Name = "Cov. C Increased Special Limits of Liability ", .Limit = "", .Premium = "", .Style = ""})

    '        opCovList.Add(New With {.Name = "Firearms", .Limit = "2,000", .Premium = "Included", .Style = indentStyle})
    '        opCovList.Add(New With {.Name = "Jewelry, Watches, and Furs", .Limit = "1,000", .Premium = "Included", .Style = indentStyle})

    '        opCovList.Add(New With {.Name = "Money", .Limit = "200", .Premium = "Included", .Style = indentStyle})

    '        opCovList.Add(New With {.Name = "Securities", .Limit = "1,000", .Premium = "Included", .Style = indentStyle})
    '        opCovList.Add(New With {.Name = "Silverware, Goldware, Pewterware", .Limit = "2,500", .Premium = "Included", .Style = indentStyle})

    '        opCovList.Add(New With {.Name = "Ordinance or Law", .Limit = "Included", .Premium = "Included", .Style = ""})

    '    Else
    '        'opCovList.Add(New With {.Name = "Business Property Increased Limits", .Limit = "2,500", .Premium = "Included", .Style = ""})
    '        opCovList.Add(New With {.Name = "Credit Card, Fund Transfer Card, Forgery and Counterfeit Money Coverage", .Limit = "1,000", .Premium = "Included", .Style = ""})

    '        opCovList.Add(New With {.Name = "Cov. C Increased Special Limits of Liability ", .Limit = "", .Premium = "", .Style = ""})

    '        Dim style As String = "padding-left:20px;"
    '        opCovList.Add(New With {.Name = "Firearms", .Limit = "500", .Premium = "Included", .Style = style})
    '        opCovList.Add(New With {.Name = "Jewelry, Watches, and Furs", .Limit = "500", .Premium = "Included", .Style = style})

    '        opCovList.Add(New With {.Name = "Money", .Limit = "100", .Premium = "Included", .Style = style})

    '        opCovList.Add(New With {.Name = "Securities", .Limit = "500", .Premium = "Included", .Style = style})
    '        opCovList.Add(New With {.Name = "Silverware, Goldware, Pewterware", .Limit = "1,000", .Premium = "Included", .Style = style})

    '        'opCovList.Add(New With {.Name = "Ordinance or Law", .Limit = "Included", .Premium = "Included", .Style = ""})

    '        opCovList.Add(New With {.Name = "Increased Limits Motorized Vehicles", .Limit = "1,000", .Premium = "Included", .Style = ""})
    '        opCovList.Add(New With {.Name = "Fire Department Service Charge", .Limit = "500", .Premium = "Included", .Style = ""})
    '        opCovList.Add(New With {.Name = "Outdoor Antenna", .Limit = "500", .Premium = "Included", .Style = ""})

    '    End If

    '    For Each c In opCovList
    '        html.AppendLine("<tr>")
    '        WriteCell(html, c.Name, c.Style)
    '        WriteCell(html, c.Limit)
    '        WriteCell(html, c.Premium)

    '        html.AppendLine("</tr>")
    '    Next

    '    html.AppendLine("</table>")

    '    Me.tblIncludedCoverages.Text = html.ToString()
    'End Sub

    'Private Sub PopulateOptionalCoverages()
    '    Dim html As New StringBuilder()

    '    html.AppendLine("<table class=""hom_qa_table_shades"">")
    '    html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

    '    WriteCell(html, "Coverage")
    '    WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
    '    WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

    '    html.AppendLine("</tr>")

    '    Dim opCovList = {(New With {.Name = "", .Limit = "", .Premium = "", .Index = 0, .Style = ""})}.Take(0).ToList()

    '    If Me.MyLocation.SectionICoverages IsNot Nothing Then
    '        For Each cov In Me.MyLocation.SectionICoverages
    '            Dim prem As String = cov.Premium
    '            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
    '                prem = "Included"
    '            End If
    '            Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded)
    '            Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased)
    '            Dim CovALimit As Double = A_included + A_increased
    '            Select Case cov.HOM_CoverageType
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage
    '                    opCovList.Add(New With {.Name = "Equipment Breakdown Coverage", .Limit = "50,000", .Premium = prem, .Index = 0, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
    '                    opCovList.Add(New With {.Name = "Personal Property Replacement Cost", .Limit = "N/A", .Premium = prem, .Index = 1, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement
    '                    opCovList.Add(New With {.Name = "Homeowner Enhancement Endorsement", .Limit = "N/A", .Premium = prem, .Index = 2, .Style = ""})
    '                    If MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains).Count <= 0 Then
    '                        opCovList.Add(New With {.Name = "Backup of Sewer or Drain", .Limit = "5,000", .Premium = "Included", .Index = 3, .Style = ""})
    '                    End If
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains
    '                    opCovList.Add(New With {.Name = "Backup of Sewer or Drain", .Limit = (Decimal.Parse(cov.TotalLimit.Replace(",", "")) + 5000).ToString("N0"), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 3, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty
    '                    opCovList.Add(New With {.Name = "Loss of Refrigerated Contents", .Limit = "50,000", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 4, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval
    '                    opCovList.Add(New With {.Name = "Debris Removal", .Limit = "250", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 5, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage
    '                    opCovList.Add(New With {.Name = "Cov. A - Specified Additional Amount of Insurance", .Limit = cov.TotalLimit, .Premium = prem, .Index = 6, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement
    '                    opCovList.Add(New With {.Name = "Actual Cash Value Loss Settlement", .Limit = "N/A", .Premium = prem, .Index = 7, .Style = ""})

    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment
    '                    opCovList.Add(New With {.Name = "Functional Replacement Cost Loss Settlement", .Limit = "N/A", .Premium = prem, .Index = 8, .Style = ""})

    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake
    '                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
    '                    opCovList.Add(New With {.Name = String.Format("Earthquake - (Deductible {0})", deductible), .Limit = "N/A", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 9, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB
    '                    ' limit - lesser of Cov A or $200,000
    '                    Dim Limit As String = ""
    '                    If CovALimit > 200000 Then
    '                        Limit = "$200,000"
    '                    Else
    '                        Limit = String.Format("{0:C0}", CovALimit)
    '                    End If
    '                    opCovList.Add(New With {.Name = "Mine Subsidence Cov A & B", .Limit = Limit, .Premium = prem, .Index = 10, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA
    '                    ' limit - lesser of Cov A or $200,000
    '                    Dim Limit As String = ""
    '                    If CovALimit > 200000 Then
    '                        Limit = "$200,000"
    '                    Else
    '                        Limit = String.Format("{0:C0}", CovALimit)
    '                    End If
    '                    opCovList.Add(New With {.Name = "Mine Subsidence Cov A", .Limit = Limit, .Premium = prem, .Index = 11, .Style = ""})

    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse
    '                    opCovList.Add(New With {.Name = "Sinkhole Collapse", .Limit = "N/A", .Premium = prem, .Index = 12, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
    '                    opCovList.Add(New With {.Name = "Actual Cash Value Loss Settlement/Windstorm or Hail to Roof Surfacing", .Limit = "N/A", .Premium = "Included", .Index = 13, .Style = ""})

    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations
    '                    opCovList.Add(New With {.Name = "Building Additions and Alterations", .Limit = "N/A", .Premium = prem, .Index = 14, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment
    '                    opCovList.Add(New With {.Name = "Loss Assessment", .Limit = "N/A", .Premium = prem, .Index = 16, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial
    '                    opCovList.Add(New With {.Name = "Theft of Building Materials", .Limit = "N/A", .Premium = prem, .Index = 17, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageA
    '                    opCovList.Add(New With {.Name = "Unit-Owners Coverage A", .Limit = "N/A", .Premium = prem, .Index = 18, .Style = ""})

    '            End Select
    '        Next
    '    End If

    '    If Me.MyLocation.SectionIICoverages IsNot Nothing Then
    '        For Each cov In Me.MyLocation.SectionIICoverages
    '            Dim prem As String = cov.Premium
    '            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
    '                prem = "Included"
    '            End If
    '            Select Case cov.CoverageType
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.PersonalInjury '12-9-14
    '                    opCovList.Add(New With {.Name = "Personal Injury", .Limit = "(see <sup>5</sup>)", .Premium = prem, .Index = 19, .Style = ""})
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmersPersonalLiability
    '                    opCovList.Add(New With {.Name = "Incidental Farming Personal Liability", .Limit = cov.TotalLimit, .Premium = prem, .Index = 20, .Style = ""})
    '                    HasIncedentalFarmOrFarmOwnder = True
    '                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
    '                    opCovList.Add(New With {.Name = "Farm Owned and Operated By Insured: 0-100 Acres", .Limit = "(see <sup>4</sup>)", .Premium = prem, .Index = 21, .Style = ""})
    '                    HasIncedentalFarmOrFarmOwnder = True
    '            End Select
    '        Next
    '    End If

    '    If Me.MyLocation.SectionICoverages IsNot Nothing Then
    '        Dim offPremBuildingCount As Int32 = 0

    '        For Each cov As QuickQuoteSectionICoverage In Me.MyLocation.SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises Or p.CoverageCodeId = "3761")
    '            offPremBuildingCount += 1
    '            Dim prem As String = cov.Premium
    '            If offPremBuildingCount = 1 Then
    '                opCovList.Add(New With {.Name = "Specified Other Structures - Off Premises", .Limit = "(see below)", .Premium = "(see below)", .Index = 22, .Style = ""})
    '            End If

    '            Dim zip As String = cov.Address.Zip
    '            If zip.Length > 5 Then
    '                zip = zip.Substring(0, 5)
    '            End If
    '            Dim address As String = String.Format("{0} {1} {2} {3} {4} {5} {6}", cov.Address.HouseNum, cov.Address.StreetName, If(String.IsNullOrWhiteSpace(cov.Address.ApartmentNumber) = False, "Apt# " + cov.Address.ApartmentNumber, ""), cov.Address.POBox, cov.Address.City, cov.Address.State, zip).Replace("  ", " ").Trim()
    '            opCovList.Add(New With {.Name = address, .Limit = cov.IncreasedLimit, .Premium = prem, .Index = 22 + offPremBuildingCount, .Style = indentStyle})

    '            'HasIncedentalFarmOrFarmOwnder = True
    '        Next
    '    End If

    '    If Me.MyLocation.SectionIAndIICoverages IsNot Nothing Then
    '        For Each cov In Me.MyLocation.SectionIAndIICoverages
    '            Select Case cov.MainCoverageType
    '            End Select
    '        Next
    '    End If

    '    For Each c In (From a In opCovList Order By a.Index Select a)
    '        html.AppendLine("<tr>")

    '        WriteCell(html, c.Name, c.Style)
    '        WriteCell(html, If(c.Limit = "0", "N/A", c.Limit)) ' Convert $0 to N/A
    '        WriteCell(html, c.Premium)

    '        html.AppendLine("</tr>")
    '    Next

    '    html.AppendLine("</table>")

    '    Me.tblOptionalCoverages.Text = html.ToString()
    'End Sub

#End Region

End Class