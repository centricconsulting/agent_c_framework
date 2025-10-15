Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Data
Imports System.Xml
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM
Imports Microsoft.CodeAnalysis.CSharp.Syntax
Imports System.Configuration.ConfigurationManager

Public Class PFQuoteSummary_FAR
    Inherits System.Web.UI.Page

    Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim summaryHelper As New SummaryHelperClass
    Dim fileName As String = ""
    Dim quoteIds As List(Of String)
    Dim noSurchargesExist As Boolean = True
    Private NumFormatWithCents As String = "$###,###,###.00"

    Private Property TotalOptionalCoveragesPremium As Decimal

    Public Property RowNumber As Int32
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Public ReadOnly Property CurrentForm As String
        Get
            Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, LocalQuickQuote.Locations(0).FormTypeId)
        End Get
    End Property

    Public Property CoverageCode() As DataTable
        Get
            Return Session("sess_CoverageCode")
        End Get
        Set(ByVal value As DataTable)
            Session("sess_CoverageCode") = value
        End Set
    End Property

    Public Property PersPropList() As DataTable
        Get
            Return ViewState("vs_PersProp")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PersProp") = value
        End Set
    End Property

    Public Property PeakSeasonList() As DataTable
        Get
            Return ViewState("vs_PeakSeason")
        End Get
        Set(ByVal value As DataTable)
            ViewState("vs_PeakSeason") = value
        End Set
    End Property

    Public Property PolicyLineCount() As Integer
        Get
            Return Session("sess_PolicyLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_PolicyLineCnt") = value
        End Set
    End Property

    Public Property LocationLineCount() As Integer
        Get
            Return Session("sess_LocationLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_LocationLineCnt") = value
        End Set
    End Property

    Public Property LiabilityLineCount() As Integer
        Get
            Return Session("sess_LiabilityLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_LiabilityLineCnt") = value
        End Set
    End Property

    Public Property DwellingLineCount() As Integer
        Get
            Return Session("sess_DwellingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_DwellingLineCnt") = value
        End Set
    End Property

    Public Property AddlDwellingLineCount() As Integer
        Get
            Return Session("sess_AddlDwellingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_AddlDwellingLineCnt") = value
        End Set
    End Property

    Public Property BuildingsLineCount() As Integer
        Get
            Return Session("sess_BuildingLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_BuildingLineCnt") = value
        End Set
    End Property

    Public Property PersPropLineCount() As Integer
        Get
            Return Session("sess_PersPropLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_PersPropLineCnt") = value
        End Set
    End Property

    Public Property IMLineCount() As Integer
        Get
            Return Session("sess_IMLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_IMLineCnt") = value
        End Set
    End Property

    Public Property RVWaterLineCount() As Integer
        Get
            Return Session("sess_RVWLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_RVWLineCnt") = value
        End Set
    End Property

    Public Property AddlCovLineCount() As Integer
        Get
            Return Session("sess_AddlCoverageLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_AddlCoverageLineCnt") = value
        End Set
    End Property

    Public Property SurLineCount() As Integer
        Get
            Return Session("sess_SurChargeLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_SurChargeLineCnt") = value
        End Set
    End Property

    Public Property CreditLineCount() As Integer
        Get
            Return Session("sess_CreditLineCnt")
        End Get
        Set(ByVal value As Integer)
            Session("sess_CreditLineCnt") = value
        End Set
    End Property

    'Public Property LocationList() As List(Of QuickQuoteLocation)
    '    Get
    '        Return Session("sess_LocationList")
    '    End Get
    '    Set(ByVal value As List(Of QuickQuoteLocation))
    '        Session("sess_LocationList") = value
    '    End Set
    'End Property

    Public Property ShowIMRV() As Boolean
        Get
            Return Session("sess_IMRV")
        End Get
        Set(ByVal value As Boolean)
            Session("sess_IMRV") = value
        End Set
    End Property

    Public Property PersPropQA() As String
        Get
            Return Session("sess_PersPropQA")
        End Get
        Set(ByVal value As String)
            Session("sess_PersPropQA") = value
        End Set
    End Property

    Public Property LocalQuickQuote() As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return Session("sess_LocalQuickQuote")
        End Get
        Set(ByVal value As QuickQuote.CommonObjects.QuickQuoteObject)
            Session("sess_LocalQuickQuote") = value
        End Set
    End Property

    'Added 9/11/18 for multi state MLW - from VrControlBaseEssentials
    Protected ReadOnly Property SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject)
        Get
            If Me.LocalQuickQuote IsNot Nothing Then
                If HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects") Is Nothing Then
                    Dim parts = Me.qqHelper.MultiStateQuickQuoteObjects(Me.LocalQuickQuote)
                    If parts IsNot Nothing Then
                        HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects") = parts
                    End If
                End If
            End If
            Return DirectCast(HttpContext.Current.Items("vrControlBase_MultiStateQuickQuoteObjects"), List(Of QuickQuote.CommonObjects.QuickQuoteObject))
        End Get
    End Property

    'Added 9/11/18 for multi state MLW - from VrControlBaseEssentials
    Protected ReadOnly Property SubQuoteFirst As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Return SubQuotes.GetItemAtIndex(0)
        End Get
    End Property

    'Added 9/11/18 for multi state MLW - from VrControlBaseEssentials
    Protected Function SubQuoteForState(stateType As QuickQuoteHelperClass.QuickQuoteState) As QuickQuote.CommonObjects.QuickQuoteObject
        If Me.SubQuotes IsNot Nothing Then
            Return (From sp In Me.SubQuotes Where sp.QuickQuoteState = stateType Select sp).FirstOrDefault()
        End If
        Return Nothing
    End Function

    'Added 9/11/18 for multi state MLW - from VrControlBaseEssentials
    Protected Function GoverningStateQuote() As QuickQuoteObject 'added 8/15/2018
        If Me.LocalQuickQuote IsNot Nothing Then
            If HttpContext.Current.Items("vrControlBase_GoverningState") Is Nothing Then
                If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), Me.LocalQuickQuote.QuickQuoteState) = True AndAlso Me.LocalQuickQuote.QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.None Then
                    HttpContext.Current.Items("vrControlBase_GoverningState") = SubQuoteForState(Me.LocalQuickQuote.QuickQuoteState)
                Else
                    HttpContext.Current.Items("vrControlBase_GoverningState") = SubQuoteFirst
                End If
            End If
            Return DirectCast(HttpContext.Current.Items("vrControlBase_GoverningState"), QuickQuoteObject)
        End If
        Return Nothing
    End Function

    'added 2/19/2019
    Public ReadOnly Property QuoteId As String
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property
    Private _EndorsementPolicyId As Integer = 0
    Public ReadOnly Property EndorsementPolicyId As Integer
        Get
            If _EndorsementPolicyId <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyId
        End Get
    End Property
    Private _EndorsementPolicyImageNum As Integer = 0
    Public ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            If _EndorsementPolicyImageNum <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Dim strEndorsementPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Request.QueryString("EndorsementPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString
            End If
            Return strEndorsementPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadEndorsementPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _EndorsementPolicyId = 0
            _EndorsementPolicyImageNum = 0
        End If
        Dim strEndorsementPolicyIdAndImageNum As String = EndorsementPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strEndorsementPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strEndorsementPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _EndorsementPolicyId = intList(0)
                If intList.Count > 1 Then
                    _EndorsementPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Private _ReadOnlyPolicyId As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            If _ReadOnlyPolicyId <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyId
        End Get
    End Property
    Private _ReadOnlyPolicyImageNum As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            If _ReadOnlyPolicyImageNum <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Dim strReadOnlyPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString
            End If
            Return strReadOnlyPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadReadOnlyPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _ReadOnlyPolicyId = 0
            _ReadOnlyPolicyImageNum = 0
        End If
        Dim strReadOnlyPolicyIdAndImageNum As String = ReadOnlyPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strReadOnlyPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strReadOnlyPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _ReadOnlyPolicyId = intList(0)
                If intList.Count > 1 Then
                    _ReadOnlyPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Dim _RatedQuote As QuickQuoteObject = Nothing
    Protected ReadOnly Property RatedQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If _RatedQuote Is Nothing Then
                Dim errCreateQSO As String = ""
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, Nothing)
                End If
            End If
            Return _RatedQuote
        End Get
    End Property

    Private ReadOnly Property SuffocationAndCustomFeedingCutoffDate As DateTime
        Get
            Return CDate("7/1/2020")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'lblQuoteId.Text = Request.QueryString("QuoteId").ToString
            'updated 5/10/2019
            Me.lblPrimDwelling.Text = String.Format("{0} {1}", "Primary Dwelling Property Coverage - loc", RowNumber + 1) 'Updated per WS-611

            Me.lblQuoteId.Text = If(Request.QueryString("QuoteId") IsNot Nothing, Request.QueryString("QuoteId").ToString, "")
            Dim summaryType As String = Request.QueryString("summarytype").ToString
            Dim reportHeader As StringBuilder = New StringBuilder
            'reportHeader.Append("Indiana Farmers Mutual Insurance Company ")

            If summaryType = "App" Then
                reportHeader.Append("APPLICATION")
                PersPropQA = "App"
            Else
                reportHeader.Append("QUOTE")
                PersPropQA = "Quote"
            End If

            PolicyLineCount = 0
            LocationLineCount = 0
            DwellingLineCount = 0
            AddlDwellingLineCount = 0
            BuildingsLineCount = 0
            PersPropLineCount = 0
            IMLineCount = 0
            RVWaterLineCount = 0
            AddlCovLineCount = 0
            SurLineCount = 0
            CreditLineCount = 0

            lblHeader.Text = reportHeader.ToString
            ctlClientAndAgencyInfo.ToggleProducer(False)
            GetQuoteFromDb(summaryType)
            LoadSummaryObjects()

            'Updated 9/11/18 for multi state MLW
            If LocalQuickQuote IsNot Nothing Then
                ' Bug# 6253 - Add Policy Type to PDF Summaries
                'Updated 9/11/18 for multi state MLW
                'With LocalQuickQuote '9/11/18 moved to next block below for multi state MLW
                Dim programType As String = ""
                If SubQuoteFirst IsNot Nothing Then
                    'Dim programType As String = "" 'moved 9/11/18 out of this block for multi state MLW
                    If qqHelper.IsNumericString(SubQuoteFirst.ProgramTypeId) = True Then
                        programType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, SubQuoteFirst.ProgramTypeId)
                    ElseIf LocalQuickQuote.Locations IsNot Nothing AndAlso LocalQuickQuote.Locations.Count > 0 AndAlso qqHelper.IsNumericString(LocalQuickQuote.Locations(0).ProgramTypeId) = True Then
                        programType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, LocalQuickQuote.Locations(0).ProgramTypeId)
                    End If
                End If

                With LocalQuickQuote
                    If String.IsNullOrEmpty(programType) = False Then
                        Dim phNameType As String = ""
                        If .Policyholder IsNot Nothing AndAlso .Policyholder.Name IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(.Policyholder.Name.TypeId) = True Then
                            phNameType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.TypeId, .Policyholder.Name.TypeId)
                            If UCase(phNameType).Contains("PERSONAL") = True Then
                                phNameType = "Personal"
                            ElseIf UCase(phNameType).Contains("COMMERCIAL") = True Then
                                phNameType = "Commercial"
                            Else
                                phNameType = ""
                            End If
                        End If

                        ' Bug# 6147 - Append Hobby Farm after description
                        Dim hobbyFarm As String = ""
                        If .Locations(0).HobbyFarmCredit Then
                            hobbyFarm = "- Hobby Farm"
                        Else
                            hobbyFarm = ""
                        End If

                        lblPolicyType.Text = String.Format("{0} {1} {2} {3}", phNameType, programType, " ", hobbyFarm) 'qqHelper.appendText(phNameType, programType, " ", hobbyFarm)
                    End If
                End With
            End If
            If (RatedQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) Then
                EndorsementPrintSection.Visible = True
                QuoteAppPrintSection.Visible = False
                Me.ctlEndorsementOrChangeHeader.Visible = True
                Me.ctlEndorsementOrChangeHeader.Quote = LocalQuickQuote
            Else
                EndorsementPrintSection.Visible = False
                QuoteAppPrintSection.Visible = True
                Me.ctlEndorsementOrChangeHeader.Visible = False
            End If

        End If
    End Sub

    Private Sub LoadSummaryObjects()
        'Updated 9/11/18 for multi state MLW - LocalQuickQuote to GoverningStateQuote
        If Me.GoverningStateQuote IsNot Nothing Then
            If Me.GoverningStateQuote.Applicants IsNot Nothing Then
                Dim applicantStr As StringBuilder = New StringBuilder()

                If LocalQuickQuote.Policyholder.Name.TypeId <> "2" Then
                    ' Personal
                    Dim Name As String = String.Format("{0} {1} {2} {3}", LocalQuickQuote.Policyholder.Name.FirstName, LocalQuickQuote.Policyholder.Name.MiddleName, LocalQuickQuote.Policyholder.Name.LastName, LocalQuickQuote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                    applicantStr.Append(Name + "<br />")
                    ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(applicantStr.ToString().Substring(0, applicantStr.Length - 6), LocalQuickQuote.Policyholder.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                Else
                    ' Commercial
                    ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(LocalQuickQuote.Policyholder.Name.CommercialDBAname, LocalQuickQuote.Policyholder.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                End If

                ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.ClientInfo, LocalQuickQuote.Policyholder.PrimaryPhone, "<br />")
            End If
        End If

        Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(summaryHelper.GetAgencyName(LocalQuickQuote.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), LocalQuickQuote.Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")

        'Dim trimPrimaryPhone As String = ""
        'Try
        '    trimPrimaryPhone = LocalQuickQuote.Agency.PrimaryPhone.Trim.Remove(LocalQuickQuote.Agency.PrimaryPhone.Length - 2)
        'Catch ex As Exception
        '    trimPrimaryPhone = ""
        'End Try

        'ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.AgencyInfo, trimPrimaryPhone, "<br />")
        'updated 9/19/2017
        ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.AgencyInfo, LocalQuickQuote.Agency.PrimaryPhone, "<br />")
        ctlClientAndAgencyInfo.ProducerCode = LocalQuickQuote.AgencyProducerCode

        'CoverageCode = FarmSummaryHelper.GetCoverageCodeCaption()
    End Sub

    Public Sub GetQuoteFromDb(summaryType As String)
        Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
        Dim val_item_counter As Integer = 0 'added 4/11/2013
        Dim noPolicyDiscounts As Boolean = True

        'If IsNumeric(Me.lblQuoteId.Text) = True Then 'removed IF 2/19/2019
        'Dim errorMsg As String = ""
        'Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
        'QQxml.GetRatedQuote(lblQuoteId.Text, LocalQuickQuote, rateType, errorMsg)
        'updated 2/19/2019; note: rateType should be set by method, though none of these have the param as ByRef
        LocalQuickQuote = RatedQuote

        ' Prevents DB call every time info is needed
        LocalQuickQuote = LocalQuickQuote

        If LocalQuickQuote IsNot Nothing Then 'added IF 2/19/2019
            'LocationList = New List(Of QuickQuoteLocation)
            'For Each location As QuickQuoteLocation In LocalQuickQuote.Locations
            '    LocationList.Add(location)
            'Next

            '
            ' Policy Information
            '

            lblQuoteNumber.Text = LocalQuickQuote.QuoteNumber
            lblDate.Text = DateTime.Now.ToShortDateString()
            lblEffectiveDate.Text = LocalQuickQuote.EffectiveDate
            lblPremium.Text = LocalQuickQuote.TotalQuotedPremium
            PolicyLineCount = 19

            '
            ' Get Applicant Information
            '

            ' Applicants Table
            Dim dtApplicants As New DataTable
            dtApplicants.Columns.Add("ApplicantNum", System.Type.GetType("System.String"))
            dtApplicants.Columns.Add("ApplicantName", System.Type.GetType("System.String"))

            Dim applicantCnt As Integer = 0
            'Updated 9/11/18 for multi state MLW - LocalQuickQuote to GoverningStateQuote
            If Me.GoverningStateQuote IsNot Nothing Then
                For Each applicant As QuickQuoteApplicant In Me.GoverningStateQuote.Applicants
                    applicantCnt += 1
                    Dim newRow As DataRow = dtApplicants.NewRow
                    newRow.Item("ApplicantNum") = applicantCnt.ToString()
                    newRow.Item("ApplicantName") = applicant.Name.DisplayName
                    dtApplicants.Rows.Add(newRow)
                Next

                dgApplicants.DataSource = dtApplicants
                dgApplicants.DataBind()
                PolicyLineCount += dtApplicants.Rows.Count
            End If
            '
            ' Total Acreage
            '
            Dim totalAcre As Integer = 0
            'For Each location In LocalQuickQuote.Locations
            For Each location In LocalQuickQuote.Locations
                For Each acre In location.Acreages
                    totalAcre += Integer.Parse(acre.Acreage)
                Next
            Next

            lblTotalAcreageData.Text = totalAcre.ToString()
            LocationLineCount += 5

            If LocalQuickQuote.Locations IsNot Nothing Then
                ' Applicants Table
                Dim dtLocationSummary As New DataTable
                dtLocationSummary.Columns.Add("LocationNum", System.Type.GetType("System.String"))
                dtLocationSummary.Columns.Add("LocationAddress", System.Type.GetType("System.String"))
                dtLocationSummary.Columns.Add("LocationAcreage", System.Type.GetType("System.String"))
                dtLocationSummary.Columns.Add("LocationDesc", System.Type.GetType("System.String"))
                dtLocationSummary.Columns.Add("AcreageOnly", System.Type.GetType("System.String"))

                ' Loop through each location
                Dim locationCnt As Integer = 0
                'For Each location As QuickQuoteLocation In LocalQuickQuote.Locations
                For Each location As QuickQuoteLocation In LocalQuickQuote.Locations
                    locationCnt += 1
                    For Each locationAcre In location.Acreages
                        Dim newRow As DataRow = dtLocationSummary.NewRow
                        If locationAcre.LocationAcreageTypeId <> "4" Then
                            newRow.Item("LocationNum") = locationCnt.ToString()
                            newRow.Item("LocationAddress") = String.Format("{0} {1} {2} {3}", location.Address.HouseNum, location.Address.StreetName, location.Address.State, location.Address.Zip)
                        End If

                        newRow.Item("LocationAcreage") = locationAcre.Acreage.ToString()

                        newRow.Item("LocationDesc") = locationAcre.Description

                        Select Case locationAcre.LocationAcreageTypeId
                            Case "1"
                                newRow.Item("AcreageOnly") = "Primary Location"
                            Case "2"
                                newRow.Item("AcreageOnly") = "Additional Location"
                            Case "3"
                                newRow.Item("AcreageOnly") = "Acreage Only"
                            Case "4"
                                newRow.Item("AcreageOnly") = "Blanket Acreage"
                        End Select
                        'newRow.Item("AcreageOnly") = locationAcre.LocationAcreageTypeId
                        dtLocationSummary.Rows.Add(newRow)
                        LocationLineCount += 1
                    Next
                Next

                dgLocationSummary.DataSource = dtLocationSummary
                dgLocationSummary.DataBind()

                PopulateLiabilityCoverage()

                If LocalQuickQuote.Locations(0).ProgramTypeId <> "8" Then    ' Farm Liability
                    'If LocalQuickQuote.Locations(0).ProgramTypeId <> "6" Then
                    If LocalQuickQuote.Locations(0).ProgramTypeId <> "6" Then
                        dvDwellings.Attributes.Add("style", "display:none;")
                        DwellingLineCount = 0
                    End If

                    'If LocalQuickQuote.Locations(0).ProgramTypeId <> "6" And LocalQuickQuote.Locations(0).ProgramTypeId <> "7" Then
                    If LocalQuickQuote.Locations(0).ProgramTypeId <> "6" And LocalQuickQuote.Locations(0).ProgramTypeId <> "7" Then
                        dvBuilding.Attributes.Add("style", "display:none;")
                        BuildingsLineCount = 0
                    End If

                    ' INLAND MARINE, RV/WATERCRAFT
                    ' Updated IM/RV/WC logic to show IM/RV/WC on commercial FO or SOM policies
                    Dim totalIMPremium As String = "0"
                    ShowIMRV = False

                    ' ProgramTypeId 6 = FO, ProgramTypeId 7 = SOM
                    If LocalQuickQuote.Locations(0).ProgramTypeId = "6" OrElse LocalQuickQuote.Locations(0).ProgramTypeId = "7" Then
                        ShowIMRV = True
                    End If

                    If ShowIMRV Then
                        Dim dtInlandMarine As New DataTable
                        dtInlandMarine.Columns.Add("Coverage", System.Type.GetType("System.String"))
                        dtInlandMarine.Columns.Add("Description", System.Type.GetType("System.String"))
                        dtInlandMarine.Columns.Add("Deductible", System.Type.GetType("System.String"))
                        dtInlandMarine.Columns.Add("Limits", System.Type.GetType("System.String"))
                        dtInlandMarine.Columns.Add("Premium", System.Type.GetType("System.String"))

                        ' IM Jewelry
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Jewelry in Vault
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Antiques - with breakage coverage
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithBreakage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Antiques - without breakage coverage
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.AntiquesWithoutBreakage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Collector Items Hobby - with breakage coverage
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Collector Items Hobby - without breakage coverage
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Cameras
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Computers
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Fine Arts - with breakage coverage
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Fine Arts - without breakage coverage
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Furs
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Guns
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Hearing Aids
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Irrigation Equipment - Named Perils
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentNamedPerils), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Irrigation Equipment - Special Form
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.IrrigationEquipmentSpecialCoverage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Radios - CB
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_CB), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Radios - FM
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Radios_FM), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Reproductive Materials - Named Perils
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsNamedPerils), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Reproductive Materials - Special Form
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.ReproductiveMaterialsSpecialCoverage), dtInlandMarine, totalIMPremium, IMLineCount)

                        ' Telephone - Car or Mobile
                        InlandMarineData(LocalQuickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile), dtInlandMarine, totalIMPremium, IMLineCount)

                        'If IMLineCount > 4 Then
                        If LocalQuickQuote.Locations(0).InlandMarines.Count > 0 Then
                            lblNoIMExist.Visible = False
                            pnlInlandMarine.Visible = True
                            IMLineCount += 7
                        Else
                            IMLineCount += 4
                        End If

                        lblTotalIMPremData.Text = "$" + Double.Parse(totalIMPremium).ToString("N2")

                        '
                        ' RV/Watercraft
                        '
                        Dim totalRVWPremium As String = "0"
                        Dim dtRVWatercraft As New DataTable
                        dtRVWatercraft.Columns.Add("Coverage", System.Type.GetType("System.String"))
                        dtRVWatercraft.Columns.Add("CoverageOpt", System.Type.GetType("System.String"))
                        dtRVWatercraft.Columns.Add("Deductible", System.Type.GetType("System.String"))
                        dtRVWatercraft.Columns.Add("Limits", System.Type.GetType("System.String"))
                        dtRVWatercraft.Columns.Add("Premium", System.Type.GetType("System.String"))

                        'Create a list of watercraft on all locations
                        Dim AllLocationsWatercraftList = New List(Of QuickQuoteRvWatercraft)
                        If LocalQuickQuote?.Locations IsNot Nothing AndAlso LocalQuickQuote.Locations.Count > 0 Then
                            For Each Loc As QuickQuote.CommonObjects.QuickQuoteLocation In LocalQuickQuote.Locations
                                If Loc IsNot Nothing Then
                                    For Each craft In Loc.RvWatercrafts
                                        AllLocationsWatercraftList.Add(craft)
                                    Next
                                End If
                            Next
                        End If

                        'RVWatercraftData(LocalQuickQuote.Locations(0).RvWatercrafts, dtRVWatercraft, totalRVWPremium, RVWaterLineCount)
                        RVWatercraftData(AllLocationsWatercraftList, dtRVWatercraft, totalRVWPremium, RVWaterLineCount)
                        lblTotalRVWatercraftPremData.Text = "$" + Double.Parse(totalRVWPremium).ToString("N2")

                        'If RVWaterLineCount > 8 Then
                        If LocalQuickQuote.Locations(0).RvWatercrafts.Count > 0 Then
                            lblNoRVWaterExist.Visible = False
                            pnlRVWatercraft.Visible = True
                            RVWaterLineCount += 7
                        Else
                            RVWaterLineCount += 4
                        End If

                        ' Determine Additional Amount needed to meet $25.00 minimum premium
                        If (Double.Parse(totalIMPremium) + Double.Parse(totalRVWPremium)) <> 0 Then
                            If (Double.Parse(totalIMPremium) + Double.Parse(totalRVWPremium)) < 25.0 Then
                                lblNoIMExist.Visible = False
                                pnlInlandMarine.Visible = True
                                pnlAdditionalPrem.Visible = True
                                lblMeetMinPrem.Text = "$" + (25.0 - (Double.Parse(totalIMPremium) + Double.Parse(totalRVWPremium))).ToString("N2")
                                If Double.Parse(totalIMPremium) <> 0.0 Then
                                    lblTotalIMPremData.Text = (Double.Parse(lblMeetMinPrem.Text.Replace("$", "")) + Double.Parse(totalIMPremium)).ToString("N2")
                                Else
                                    dgInlandMarine.Visible = False
                                    lblTotalIMPremData.Text = lblMeetMinPrem.Text
                                End If

                                IMLineCount += 1
                            End If
                        End If
                    Else
                        dvInlandMarine.Attributes.Add("style", "display:none;")
                        dvRVWater.Attributes.Add("style", "display:none;")
                    End If
                Else
                    dvDwellings.Attributes.Add("style", "display:none;")
                    DwellingLineCount = 0
                    dvBuilding.Attributes.Add("style", "display:none;")
                    BuildingsLineCount = 0
                    dvPersProp.Attributes.Add("style", "display:none;")
                    PersPropLineCount = 0
                    dvInlandMarine.Attributes.Add("style", "display:none;")
                    IMLineCount = 0
                    dvRVWater.Attributes.Add("style", "display:none;")
                    RVWaterLineCount = 0
                End If

                PopulateOptionalCoverages()

                '
                ' Determine IRPM Credit/Surcharge
                '
                Dim irpmPercentage As Integer = IRPMPercent()

                '
                ' Applied Credits
                '
                Dim dtCreditList As New DataTable
                dtCreditList.Columns.Add("Credit", System.Type.GetType("System.String"))
                dtCreditList.Columns.Add("CreditPercent", System.Type.GetType("System.String"))
                CreditLineCount = 4

                Dim creditList = IFM.VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(LocalQuickQuote, True)

                If creditList.Count <= 0 Then
                    lblNoCredits.Visible = True
                    CreditLineCount += 1
                Else
                    CreditLineCount += creditList.Count
                End If

                For inx As Integer = 0 To creditList.Count - 1
                    Dim newCreditRow As DataRow = dtCreditList.NewRow

                    newCreditRow.Item("Credit") = creditList(inx).Key
                    newCreditRow.Item("CreditPercent") = creditList(inx).Value
                    dtCreditList.Rows.Add(newCreditRow)
                Next

                If LocalQuickQuote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    If irpmPercentage < 0 Then
                        lblNoCredits.Visible = False
                        Dim newCreditRow As DataRow = dtCreditList.NewRow

                        newCreditRow.Item("Credit") = "IRPM Applied"
                        newCreditRow.Item("CreditPercent") = "" ' (irpmPercentage * -1).ToString() + "%"
                        dtCreditList.Rows.Add(newCreditRow)
                        CreditLineCount += 1
                    End If
                End If

                dgCreditList.DataSource = dtCreditList
                dgCreditList.DataBind()

                '
                ' Applied Surcharges
                '
                Dim dtSurchargeList As New DataTable
                dtSurchargeList.Columns.Add("Surcharge", System.Type.GetType("System.String"))
                dtSurchargeList.Columns.Add("SurchargePercent", System.Type.GetType("System.String"))
                SurLineCount = 4

                Dim surchargeList = IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMSurcharges(LocalQuickQuote)

                If surchargeList.Count <= 0 Then
                    lblNoSurchargeExist.Visible = True
                    SurLineCount += 1
                Else
                    SurLineCount += surchargeList.Count
                End If

                For inx As Integer = 0 To surchargeList.Count - 1
                    Dim newSurchargeRow As DataRow = dtSurchargeList.NewRow

                    newSurchargeRow.Item("Surcharge") = surchargeList(inx).Key
                    newSurchargeRow.Item("SurchargePercent") = surchargeList(inx).Value
                    dtSurchargeList.Rows.Add(newSurchargeRow)
                Next

                If LocalQuickQuote.QuoteTransactionType <> QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    If irpmPercentage > 0 Then
                        lblNoSurchargeExist.Visible = False
                        Dim newSurchargeRow As DataRow = dtSurchargeList.NewRow

                        newSurchargeRow.Item("Surcharge") = "IRPM Applied"
                        newSurchargeRow.Item("SurchargePercent") = "" ' irpmPercentage.ToString() + "%"
                        dtSurchargeList.Rows.Add(newSurchargeRow)
                        SurLineCount += 1
                    End If
                End If

                dgSurchargeList.DataSource = dtSurchargeList
                dgSurchargeList.DataBind()

                '
                ' Payment Plan
                '
                Dim totalPrem As String = LocalQuickQuote.TotalQuotedPremium

                If totalPrem <> "" AndAlso IsNumeric(totalPrem) = True Then
                    Me.ctlPaymentOptions.DirectAnnualDown = totalPrem
                    Me.ctlPaymentOptions.DirectMonthlyInstallFee = "3"
                    Me.ctlPaymentOptions.DirectSemiAnnualDown = qqHelper.getDivisionQuotient(totalPrem, "2")
                    qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectSemiAnnualDown)
                    Me.ctlPaymentOptions.DirectQuarterlyDown = qqHelper.getDivisionQuotient(totalPrem, "4")
                    qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectQuarterlyDown)
                    Me.ctlPaymentOptions.DirectMonthlyDown = qqHelper.getSum(qqHelper.getDivisionQuotient(totalPrem, "12"), Me.ctlPaymentOptions.DirectMonthlyInstallFee) 'may also need to include installment fee
                    qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectMonthlyDown)
                    Me.ctlPaymentOptions.EFTMonthlyDown = qqHelper.getDivisionQuotient(totalPrem, "12")
                    qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.EFTMonthlyDown)
                    Me.ctlPaymentOptions.CreditDown = Me.ctlPaymentOptions.EFTMonthlyDown
                    Me.ctlPaymentOptions.RenewalCreditDown = Me.ctlPaymentOptions.EFTMonthlyDown
                    Me.ctlPaymentOptions.RenewalEftDown = Me.ctlPaymentOptions.EFTMonthlyDown
                    Me.ctlPaymentOptions.AnnualMtgDown = Me.ctlPaymentOptions.AnnualMtgDown

                    Me.ctlPaymentOptions.DirectAnnualRemainInstall = "N/A"
                    Me.ctlPaymentOptions.DirectSemiAnnualRemainInstall = "1"
                    Me.ctlPaymentOptions.DirectQuarterlyRemainInstall = "3"
                    Me.ctlPaymentOptions.DirectMonthlyRemainInstall = "11"
                    Me.ctlPaymentOptions.EFTMonthlyRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                    Me.ctlPaymentOptions.CreditRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                    Me.ctlPaymentOptions.RenewalCreditRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                    Me.ctlPaymentOptions.RenewalEftRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                    Me.ctlPaymentOptions.AnnualMtgRemainInstall = Me.ctlPaymentOptions.AnnualMtgRemainInstall

                    Me.ctlPaymentOptions.DirectAnnualBasicInstall = "N/A"
                    Me.ctlPaymentOptions.DirectSemiAnnualBasicInstall = Me.ctlPaymentOptions.DirectSemiAnnualDown
                    Me.ctlPaymentOptions.DirectQuarterlyBasicInstall = Me.ctlPaymentOptions.DirectQuarterlyDown

                    Me.ctlPaymentOptions.DirectMonthlyBasicInstall = Me.ctlPaymentOptions.DirectMonthlyDown 'may also need to include installment fee
                    qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectMonthlyBasicInstall)
                    Me.ctlPaymentOptions.EFTMonthlyBasicInstall = Me.ctlPaymentOptions.EFTMonthlyDown
                    Me.ctlPaymentOptions.CreditBasicInstall = Me.ctlPaymentOptions.CreditDown
                    Me.ctlPaymentOptions.RenewalCreditBasicInstall = Me.ctlPaymentOptions.RenewalCreditDown
                    Me.ctlPaymentOptions.RenewalEftBasicInstall = Me.ctlPaymentOptions.RenewalEftDown
                    Me.ctlPaymentOptions.AnnualMtgInstallAmt = Me.ctlPaymentOptions.AnnualMtgInstallAmt
                End If
            End If
        End If

        'End If

        'If LocalQuickQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
        '    lblCreditSurcharges.Visible = False 
        '    pnlCreditSurcharges.Visible = False
        'Else
        '    lblCreditSurcharges.Visible = True 
        '    pnlCreditSurcharges.Visible = True
        'End If

        'added 4/11/2013
        If valItemsMsg <> "" Then
            valItemsMsg = "Validation Item" & If(val_item_counter = 1, "", "s") & ":  " & valItemsMsg
            If ViewState.Item("valItemsMsg") Is Nothing Then
                ViewState.Add("valItemsMsg", valItemsMsg)
            Else
                ViewState.Item("valItemsMsg") = valItemsMsg
            End If
        End If
    End Sub

    Private Function IRPMPercent() As Integer
        'Updated 9/11/18 for multi state MLW
        Dim IRPMprcnt As Integer = 0
        If SubQuoteFirst IsNot Nothing Then
            IRPMprcnt = Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(11).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(0).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(2).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(3).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(6).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(7).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(8).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(4).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(10).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(5).RiskFactor)) +
        Integer.Parse(ctlIRPM.DiamondToVRConversion(SubQuoteFirst.ScheduledRatings(12).RiskFactor))
        End If
        'Return Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(11).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(0).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(2).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(3).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(6).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(7).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(8).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(4).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(10).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(5).RiskFactor)) +
        'Integer.Parse(ctlIRPM.DiamondToVRConversion(LocalQuickQuote.ScheduledRatings(12).RiskFactor))
        Return IRPMprcnt
    End Function

    Private Sub Pagebreak(state As Boolean, control As String) Handles ctlPageBreak.BreakAddlDwelling, ctlPageBreak.BreakBuildings, ctlPageBreak.BreakPersProp, ctlPageBreak.BreakInlandMarine,
        ctlPageBreak.BreakRVWater, ctlPageBreak.BreakAddlCoverage, ctlPageBreak.BreakAdjustments, ctlPageBreak.BreakSuper, ctlPageBreak.BreakPaymentOpt
        Select Case control
            Case "AddlDwelling"
                AddlDwellingBreak.Visible = state
            Case "Building"
                BuildingsBreak.Visible = state
            Case "PersProp"
                PersonalPropBreak.Visible = state
            Case "IM"
                IMBreak.Visible = state
            Case "RVW"
                RVWBreak.Visible = state
            Case "Additional"
                AddlCoverageBreak.Visible = state
            Case "Adj"
                AdjBreak.Visible = state
            Case "Super"
                SuperBreak.Visible = state
            Case "Payment"
                PaymentOptBreak.Visible = state
        End Select
    End Sub

    Private Sub RVWatercraftData(rvwDataSource As List(Of QuickQuoteRvWatercraft), ByRef dtRVWatercraft As DataTable, ByRef totalRVWPrem As String, ByRef lineCnt As Integer)
        For Each rvwItem As QuickQuoteRvWatercraft In rvwDataSource
            Dim newRVWRow As DataRow = dtRVWatercraft.NewRow
            Dim rvwCoverageOpt As New StringBuilder

            If rvwItem.HasLiability And Not rvwItem.HasLiabilityOnly Then
                rvwCoverageOpt.Append("Physical Damage,Liability")
            End If
            If rvwItem.HasLiability And rvwItem.HasLiabilityOnly Then
                rvwCoverageOpt.Append("Liability")
            End If
            If Not rvwItem.HasLiability And Not rvwItem.HasLiabilityOnly Then
                rvwCoverageOpt.Append("Physical Damage")
            End If

            If rvwItem.UninsuredMotoristBodilyInjuryLimitId <> "" And rvwItem.UninsuredMotoristBodilyInjuryLimitId <> "0" Then
                rvwCoverageOpt.Append(",UWBI")
            End If

            newRVWRow.Item("Coverage") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, rvwItem.RvWatercraftTypeId)
            newRVWRow.Item("CoverageOpt") = rvwCoverageOpt.ToString()
            newRVWRow.Item("Deductible") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, rvwItem.PropertyDeductibleLimitId)
            If rvwItem.RvWatercraftTypeId <> "3" Then
                newRVWRow.Item("Limits") = rvwItem.CostNew.Substring(0, rvwItem.CostNew.Length - 3).Replace("$", "")
            Else
                newRVWRow.Item("Limits") = rvwItem.RvWatercraftMotors.FirstOrDefault.CostNew.Substring(0, rvwItem.RvWatercraftMotors.FirstOrDefault.CostNew.Length - 3).Replace("$", "")
            End If
            newRVWRow.Item("Premium") = rvwItem.CoveragesPremium
            If LocalQuickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso rvwItem.HasLiability AndAlso rvwItem.HasLiabilityOnly AndAlso rvwItem.RvWatercraftTypeId = "6" Then
                newRVWRow.Item("Limits") = "Included"
                newRVWRow.Item("Deductible") = "N/A"
            End If

            dtRVWatercraft.Rows.Add(newRVWRow)
            pnlRVWatercraft.Visible = True

            totalRVWPrem = (Decimal.Parse(totalRVWPrem.Replace(",", "")) + Decimal.Parse(rvwItem.CoveragesPremium.ToString().Replace("$", "").Replace(",", ""))).ToString()
            lineCnt += 1
        Next

        dgRVWatercraft.DataSource = dtRVWatercraft
        dgRVWatercraft.DataBind()
    End Sub

    Private Sub InlandMarineData(imDataSource As List(Of QuickQuoteInlandMarine), ByRef dtInlandMarine As DataTable, ByRef totalIMPrem As String, ByRef lineCnt As Integer)
        For Each imItem As QuickQuoteInlandMarine In imDataSource
            Dim newIMRow As DataRow = dtInlandMarine.NewRow

            Dim inlandType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, imItem.CoverageCodeId)
            inlandType = inlandType.Replace("_", " ").Replace("Inland Marine", "")

            If inlandType = " Miscellaneous Class I" Then
                inlandType = "Silverware"
            End If

            newIMRow.Item("Coverage") = inlandType
            newIMRow.Item("Description") = imItem.Description
            newIMRow.Item("Deductible") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, imItem.DeductibleLimitId)
            newIMRow.Item("Limits") = imItem.IncreasedLimit
            newIMRow.Item("Premium") = imItem.CoveragePremium
            dtInlandMarine.Rows.Add(newIMRow)
            pnlInlandMarine.Visible = True

            totalIMPrem = (Decimal.Parse(totalIMPrem.Replace(",", "")) + Decimal.Parse(imItem.CoveragePremium.ToString().Replace("$", "").Replace(",", ""))).ToString()
            If imItem.Description.Length > 26 Then
                lineCnt += 2
            Else
                lineCnt += 1
            End If
        Next

        dgInlandMarine.DataSource = dtInlandMarine
        dgInlandMarine.DataBind()
    End Sub

    Private Function CreatePersPropDataTable() As DataTable
        Dim dtPersProp As New DataTable
        dtPersProp.Columns.Add("CoverageName", GetType(String))
        dtPersProp.Columns.Add("Description", GetType(String))
        dtPersProp.Columns.Add("Earthquake", GetType(String))
        dtPersProp.Columns.Add("Limit", GetType(String))
        dtPersProp.Columns.Add("Premium", GetType(String))
        dtPersProp.Columns.Add("PersPropRowNum", GetType(String))

        Return dtPersProp
    End Function

    Private Function CreatePeakSeasonCoverageDataTable() As DataTable
        Dim dtPeakSeason As New DataTable
        dtPeakSeason.Columns.Add("Description", GetType(String))
        dtPeakSeason.Columns.Add("StartDate", GetType(String))
        dtPeakSeason.Columns.Add("EndDate", GetType(String))
        dtPeakSeason.Columns.Add("Limit", GetType(String))
        dtPeakSeason.Columns.Add("Premium", GetType(String))
        dtPeakSeason.Columns.Add("PersPropRowNum", GetType(String))
        dtPeakSeason.Columns.Add("PeakRowNum", GetType(String))

        Return dtPeakSeason
    End Function

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                If htmlBytes IsNot Nothing Then
                    fileName = String.Format("SUMMARY{0}.pdf", Me.LocalQuickQuote.QuoteNumber)
                    Dim filePath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & fileName & ".htm"
                    Dim fs As New FileStream(filePath, FileMode.Create)
                    fs.Write(htmlBytes, 0, htmlBytes.Length)
                    fs.Close()

                    If File.Exists(filePath) = True Then 'enclosed block in IF statement to make sure file exists
                        Dim status As String = ""
                        Dim pdfPath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & fileName & ".pdf"

                        Try
                            RunExecutable(Server.MapPath(Request.ApplicationPath) & "\Reports\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ """ & pdfPath & """", status)

                            System.IO.File.Delete(filePath)
                            If File.Exists(pdfPath) = True Then

                                Dim IncludedPDF As String = AppSettings("Form_ML1002Terrorism").ToString '--use AppSettings for any URL(Web) PDF
                                Dim PDFHelper = New PDFTools(pdfPath, IncludedPDF) '--This combines two PDFs in order they are added to arguments
                                Dim fs_pdf = PDFHelper.GetPdfFileStream()

                                Dim pdfBytes As Byte() = New Byte(fs_pdf.Length - 1) {}
                                fs_pdf.Read(pdfBytes, 0, System.Convert.ToInt32(fs_pdf.Length))
                                fs_pdf.Close()
                                If pdfBytes IsNot Nothing Then

                                    Dim proposalId As String = ""
                                    Dim errorMsg As String = ""
                                    Dim successfulInsert As Boolean = False

                                    If errorMsg = "" Then
                                        Response.Clear()
                                        Response.ContentType = "application/pdf"
                                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("SUMMARY{0}.pdf", Me.LocalQuickQuote.QuoteNumber))
                                        Response.BinaryWrite(pdfBytes)
                                        System.IO.File.Delete(pdfPath)
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End Using
        End Using
    End Sub
    Public Sub RunExecutable(ByVal executable As String, ByVal arguments As String, ByRef status As String)
        'using same code as originally used in C:\Users\domin\Documents\Visual Studio 2005\WebSites\TestExecutableCall
        status = ""
        '*************** Feature Flags ***************
        Dim MaxWaitForExitMilliseconds As Integer = 1000
        Dim MaxTrials As Integer = 30
        Dim Trials As Integer = 0
        Dim chc = New CommonHelperClass
        If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") Then
            Dim WkArgs As String = "-q --javascript-delay 200" 'Required Defaults
            If ConfigurationManager.AppSettings("Wkhtmltopdf_RequiredArguments") IsNot Nothing Then
                WkArgs = chc.ConfigurationAppSettingValueAsString("Wkhtmltopdf_RequiredArguments")
            End If

            arguments = WkArgs & " " & arguments

            If ConfigurationManager.AppSettings("Wkhtmltopdf_WaitForExitMilliseconds_Per_Trial") IsNot Nothing Then
                MaxWaitForExitMilliseconds = chc.ConfigurationAppSettingValueAsInteger("Wkhtmltopdf_WaitForExitMilliseconds_Per_Trial")
            End If

            If ConfigurationManager.AppSettings("Wkhtmltopdf_MaxTrials") IsNot Nothing Then
                MaxTrials = chc.ConfigurationAppSettingValueAsInteger("Wkhtmltopdf_MaxTrials")
            End If

        End If

        Dim starter As ProcessStartInfo = New ProcessStartInfo(executable, arguments)
        starter.CreateNoWindow = True
        starter.RedirectStandardOutput = True
        starter.RedirectStandardError = True
        starter.UseShellExecute = False

        Dim process As Process = New Process()
        process.StartInfo = starter

        Dim compareTime As DateTime = DateAdd(DateInterval.Second, -5, Date.Now)

        process.Start()
        'updated to use variable
        If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") = False Then
            ' -----   Old Code  -----
            'process.WaitForExit(4000)
            'updated to use variable
            Dim waitForExitMilliseconds As Integer = 20000
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString <> "" AndAlso IsNumeric(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString) = True Then
                waitForExitMilliseconds = CInt(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString)
            End If
            process.WaitForExit(waitForExitMilliseconds)
            'process.WaitForExit()'never finished
            'process.WaitForExit(30000) 'updated to see if program can finish converting in that amount of time; wasn't creating file
            'process.WaitForExit(60000) 'trying double

            While process.HasExited = False
                If process.StartTime > compareTime Then
                    process.CloseMainWindow()
                    Try
                        process.Kill()
                    Catch ex As Exception
                        status = "<b>Kill failed-</b>"
                    End Try
                End If
            End While
        Else
            ' -----   New Code  -----
            While process.WaitForExit(MaxWaitForExitMilliseconds) = False
                If Trials = MaxTrials Then
                    process.CloseMainWindow()
                    Try
                        process.Kill()
                    Catch ex As Exception
                        status = "<b>Kill failed-</b>"
                    End Try
                End If
                Trials += 1
            End While
        End If

        Dim strOutput As String = process.StandardOutput.ReadToEnd
        Dim strError As String = process.StandardError.ReadToEnd

        If (process.ExitCode <> 0) Then
            status &= "Error<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError

            'added 5/28/2013 for debugging on ifmwebtest (since it always works locally); seems to work okay after changing WaitForExit from 4000 to 10000 (4 seconds to 10 seconds)
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString) = "YES" Then
                Dim eMsg As String = ""
                eMsg &= "<b>executable:</b>  " & executable
                eMsg &= "<br /><br />"
                eMsg &= "<b>arguments:</b>  " & arguments
                eMsg &= "<br /><br />"
                eMsg &= "<b>status:</b>  " & status
                qqHelper.SendEmail("ProposalPdfConverter@indianafarmers.com", "tbirkey@indianafarmers.com", "Error Converting Summary to PDF", eMsg)
            End If
        Else
            'ShowError("Success")
            status &= "Success<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError
        End If

        process.Close()
        process.Dispose()
        process = Nothing
        starter = Nothing
    End Sub

    Private Sub ShowCoverageL(ByRef totalPrem As Integer)
        Dim PremiumInt As Integer = 0
        'Updated 9/11/18 for multi state MLW - LocalQuickQuote to SubQuoteFirst
        If SubQuoteFirst IsNot Nothing Then
            lblCovLLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.OccurrenceLiabilityLimitId, SubQuoteFirst.OccurrenceLiabilityLimitId)

            'Updated 9/11/18 for multi state - total premiums
            'PremiumInt = SubQuoteFirst.Locations_Farm_L_Liability_QuotedPremium
            If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.Locations_Farm_L_Liability_QuotedPremium)) Then
                PremiumInt = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.Locations_Farm_L_Liability_QuotedPremium)
            End If
            totalPrem += PremiumInt
            If PremiumInt <> 0 Then
                'lblCovLPrem.Text = SubQuoteFirst.Locations_Farm_L_Liability_QuotedPremium
                lblCovLPrem.Text = Format(CDec(PremiumInt), NumFormatWithCents)
            Else
                lblCovLPrem.Text = "Included"
            End If

            LiabilityLineCount += 1
        End If
    End Sub

    Private Sub ShowStopGap(ByRef totalPrem As Integer)
        ' Stop Gap
        Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()

        If gsQuote IsNot Nothing AndAlso FieldHasNumericValue(gsQuote.StopGapLimitId, "PFQuoteSummary_FAR", New Label(), False) Then
            Dim prem As String = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.StopGapQuotedPremium)
            If IsNumeric(prem) Then
                lblStopGapPremium.Text = Format(CDec(prem), NumFormatWithCents)
                totalPrem += CInt(prem)
            Else
                lblStopGapPremium.Text = ""
            End If
        Else
            trStopGapRow.Attributes.Add("style", "display:none")
        End If
        Exit Sub
    End Sub

    Private Sub ShowCoverageM(ByRef totalPrem As Integer)
        Dim PremiumInt As Integer = 0
        'Updated 9/11/18 for multi state MLW - LocalQuickQuote to SubQuoteFirst
        If SubQuoteFirst IsNot Nothing Then
            lblCovMLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, SubQuoteFirst.MedicalPaymentsLimitid)

            'Updated 9/11/18 for multi state - total premiums
            'PremiumInt = LocalQuickQuote.Locations_Farm_M_Medical_Payments_QuotedPremium
            If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.Locations_Farm_M_Medical_Payments_QuotedPremium)) Then
                PremiumInt = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.Locations_Farm_M_Medical_Payments_QuotedPremium)
            End If
            totalPrem += PremiumInt
            If PremiumInt <> 0 Then
                'lblCovMPrem.Text = SubQuoteFirst.Locations_Farm_M_Medical_Payments_QuotedPremium
                lblCovMPrem.Text = Format(CDec(PremiumInt), NumFormatWithCents)
            Else
                lblCovMPrem.Text = "Included"
            End If

            LiabilityLineCount += 1
        End If
    End Sub

    Private Sub ToggleAdditionalDwelling(state As Boolean) Handles ctlDwellingList.ToggleAddlDwelling
        If state Then
            dvAddlDwelling.Attributes.Add("style", "display:block;")
        Else
            dvAddlDwelling.Attributes.Add("style", "display:none;")
        End If
    End Sub

    Private Sub ToggleBuildings(state As Boolean) Handles ctlBarnsBuildingsList.ToggleBuilding
        If state Then
            lblNoBuildingsExist.Visible = False
            ctlBarnsBuildingsList.Visible = True
        Else
            lblNoBuildingsExist.Visible = True
            ctlBarnsBuildingsList.Visible = False
        End If
    End Sub

    Private Sub PopulateLiabilityCoverage()
        Dim totalLiability As Integer = 0
        ShowCoverageL(totalLiability)
        ShowCoverageM(totalLiability)
        ShowStopGap(totalLiability)

        lblTotalLiab_Prem.Text = "$" + totalLiability.ToString("N2")
    End Sub

    Private Sub PopulateOptionalCoverages()
        TotalOptionalCoveragesPremium = 0
        'Dim totalOptCoverage As Decimal = 0.0

        If LocalQuickQuote IsNot Nothing Then
            lblNoAddlCovExist.Attributes.Add("style", "display:none;")
            dvAddlCoverage.Attributes.Add("style", "display:block;")

            ' Governing State-based coverages  MGB 2-1-19
            ' Extra Expense needs to read from the Governing State part instead of the State Parts
            Dim hasExtraExpense As Boolean = False
            Dim extraExpenseLimit As String = ""
            Dim extraExpensePremium As String = ""
            Dim GovStQt As QuickQuote.CommonObjects.QuickQuoteObject = IFM.VR.Common.Helpers.MultiState.General.GoverningStateQuote(LocalQuickQuote)
            If GovStQt IsNot Nothing Then
                Dim extraExpense As QuickQuoteOptionalCoverage = GovStQt.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                If extraExpense IsNot Nothing Then
                    hasExtraExpense = True
                    If FarmExtenderHelper.IsFarmExtenderAvailable(LocalQuickQuote) Then
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

                If hasExtraExpense = True Then
                    lblExtraExpense_Limit.Text = extraExpenseLimit
                    lblExtraExpense_Prem.Text = If(Decimal.Parse(extraExpensePremium.Replace("$", "").Replace(",", "")) = 0.0, "Included", extraExpensePremium)
                    trExtraExpenseRow.Attributes.Add("style", "display:'';")
                    'tblExtraExpense.Attributes.Add("style", "display:block;")
                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(extraExpensePremium.Replace("$", "").Replace(",", ""))
                    AddlCovLineCount += 1
                Else
                    'tblExtraExpense.Attributes.Add("style", "display:none;")
                    trExtraExpenseRow.Attributes.Add("style", "display:none;")
                End If
            End If

            ' State-based coverages
            'Updated 9/11/18 for multi state MLW - LocalQuickQuote to SubQuoteFirst
            If SubQuoteFirst IsNot Nothing Then
                With LocalQuickQuote
                    ' Employer's Liability
                    If SubQuoteFirst.HasFarmEmployersLiability Then
                        trEmployeeLiabilityRow.Attributes.Add("style", "display:'';")
                        'dvEmpLiab.Attributes.Add("style", "display:block;")
                        'Updated 9/11/18 for multi state MLW - premium totals
                        'lblEmpLiabPrem.Text = .FarmEmployersLiabilityQuotedPremium
                        If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEmployersLiabilityQuotedPremium)) Then
                            lblEmpLiabPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEmployersLiabilityQuotedPremium)
                        End If

                        ' Full Time Employee exists
                        If SubQuoteFirst.EmployeesFullTime <> "" And SubQuoteFirst.EmployeesFullTime <> "0" Then
                            trFTEmp.Attributes.Add("style", "display:block;")
                            lblFTEmpData.Text = SubQuoteFirst.EmployeesFullTime
                            AddlCovLineCount += 1
                        End If

                        If SubQuoteFirst.EmployeesPartTime41To179Days <> "" And SubQuoteFirst.EmployeesPartTime41To179Days <> "0" Then
                            trPTEmp179.Attributes.Add("style", "display:block;")
                            lblPTEmp179Data.Text = SubQuoteFirst.EmployeesPartTime41To179Days
                            AddlCovLineCount += 1
                        End If

                        If SubQuoteFirst.EmployeesPartTime1To40Days <> "" And SubQuoteFirst.EmployeesPartTime1To40Days <> "0" Then
                            trPTEmp41.Attributes.Add("style", "display:block;")
                            lblPTEmp41Data.Text = SubQuoteFirst.EmployeesPartTime1To40Days
                            AddlCovLineCount += 1
                        End If

                        'Updated 9/11/18 for multi state MLW - premium totals
                        'totalOptCoverage = totalOptCoverage + Decimal.Parse(.FarmEmployersLiabilityQuotedPremium.Replace("$", "").Replace(",", ""))
                        If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEmployersLiabilityQuotedPremium)) Then
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEmployersLiabilityQuotedPremium).Replace("$", "").Replace(",", ""))
                        End If
                    End If

                    ' CONTRACT GROWERS / CUSTOM FEEDING
                    If CDate(SubQuoteFirst.EffectiveDate) < SuffocationAndCustomFeedingCutoffDate Then
                        ' Contract Growers is used if the effective date is before the cutoff
                        If Not String.IsNullOrWhiteSpace(SubQuoteFirst.FarmContractGrowersCareCustodyControlLimitId) Then
                            'tblContractGrow.Attributes.Add("style", "display:block;")
                            trContractGrowersRow.Attributes.Add("style", "display:'';")
                            Select Case SubQuoteFirst.FarmContractGrowersCareCustodyControlLimitId
                                Case "55"
                                    lblContractGrow_Limit.Text = "250,000"
                                Case "34"
                                    lblContractGrow_Limit.Text = "500,000"
                                Case "56"
                                    lblContractGrow_Limit.Text = "1,000,000"
                            End Select

                            'Updated 9/11/18 for multi state MLW - premium totals
                            'lblContractGrow_Prem.Text = .FarmContractGrowersCareCustodyControlQuotedPremium
                            'totalOptCoverage = totalOptCoverage + Decimal.Parse(.FarmContractGrowersCareCustodyControlQuotedPremium.Replace("$", "").Replace(",", ""))
                            If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmContractGrowersCareCustodyControlQuotedPremium)) Then
                                lblContractGrow_Prem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmContractGrowersCareCustodyControlQuotedPremium)
                                TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmContractGrowersCareCustodyControlQuotedPremium).Replace("$", "").Replace(",", ""))
                            End If
                            AddlCovLineCount += 1
                        End If
                    Else
                        ' Custom Feeding is used if the effective date is on or after the cutoff
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingSwineLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingSwineLimitId <> "0" Then
                            trCustomFeedingRow_swine.Attributes.Add("style", "display:'';")
                            lblCFSwineLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingSwineLimitId, SubQuoteFirst.FarmCustomFeedingSwineLimitId)
                            lblCFSwinePremium.Text = SubQuoteFirst.FarmCustomFeedingSwineQuotedPremium
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() SubQuoteFirst.FarmCustomFeedingSwineQuotedPremium).Replace("$", "").Replace(",", "")) 'Bug 47544 -VR FAR Quote Summary Print Shows Incorrect Total Premium for Additional Coverages BB
                            AddlCovLineCount += 1
                        End If
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingPoultryLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingPoultryLimitId <> "0" Then
                            trCustomFeedingRow_poultry.Attributes.Add("style", "display:'';")
                            lblCFPoultryLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingSwineLimitId, SubQuoteFirst.FarmCustomFeedingPoultryLimitId)
                            lblCFPoultryPremium.Text = SubQuoteFirst.FarmCustomFeedingPoultryQuotedPremium
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() SubQuoteFirst.FarmCustomFeedingPoultryQuotedPremium).Replace("$", "").Replace(",", "")) 'Bug 47544 -VR FAR Quote Summary Print Shows Incorrect Total Premium for Additional Coverages BB
                            AddlCovLineCount += 1
                        End If
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingCattleLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingCattleLimitId <> "0" Then
                            trCustomFeedingRow_cattle.Attributes.Add("style", "display:'';")
                            lblCFCattleLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingSwineLimitId, SubQuoteFirst.FarmCustomFeedingCattleLimitId)
                            lblCFCattlePremium.Text = SubQuoteFirst.FarmCustomFeedingCattleQuotedPremium
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() SubQuoteFirst.FarmCustomFeedingCattleQuotedPremium).Replace("$", "").Replace(",", "")) 'Bug 47544 -VR FAR Quote Summary Print Shows Incorrect Total Premium for Additional Coverages BB
                            AddlCovLineCount += 1
                        End If
                        If IFM.Common.InputValidation.InputHelpers.StringHasNumericValue(SubQuoteFirst.FarmCustomFeedingEquineLimitId) AndAlso SubQuoteFirst.FarmCustomFeedingEquineLimitId <> "0" Then
                            trCustomFeedingRow_equine.Attributes.Add("style", "display:'';")
                            lblCFEquineLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingSwineLimitId, SubQuoteFirst.FarmCustomFeedingEquineLimitId)
                            lblCFEquinePremium.Text = SubQuoteFirst.FarmCustomFeedingEquineQuotedPremium
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() SubQuoteFirst.FarmCustomFeedingEquineQuotedPremium).Replace("$", "").Replace(",", "")) 'Bug 47544 -VR FAR Quote Summary Print Shows Incorrect Total Premium for Additional Coverages BB
                            AddlCovLineCount += 1
                        End If
                    End If

                    If .Locations(0).SectionIICoverages IsNot Nothing Then
                        For Each sc As QuickQuoteSectionIICoverage In .Locations(0).SectionIICoverages
                            Select Case sc.CoverageType
                            ' 70135 - Incidental Business Pursuits
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Incidental_Business_Exposures
                                    'dvIncidental.Attributes.Add("style", "display:block;")
                                    trIncidentalBusinessPursuitsRow.Attributes.Add("style", "display:'';")
                                    lblPursuit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.BusinessPursuitTypeId, sc.BusinessPursuitTypeId)
                                    lblPursuitReceipts.Text = sc.EstimatedReceipts
                                    lblIncidentalPrem.Text = sc.Premium
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 2

                                ' 70201 - Family Medical Payments
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments
                                    'tblFamMedPay.Attributes.Add("style", "display:block;")
                                    trFamilyMedPayRow.Attributes.Add("style", "display:'';")
                                    lblNumPersonData.Text = sc.NumberOfPersonsReceivingCare
                                    lblFamMedPayPrem.Text = If(Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", "")) = 0.0, "Included", sc.Premium)
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1

                                ' 70129 - Custom Farming (without spray)
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_No_Spraying
                                    'tblCustomFarm.Attributes.Add("style", "display:block;")
                                    trCustomFarmingRow.Attributes.Add("style", "display:'';")
                                    lblSpray.Text = "without spray"
                                    lblSprayAnnual.Text = sc.EstimatedReceipts
                                    lblCustomFarmPrem.Text = sc.Premium
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1

                                ' 80115 - Custom Farming (with spray)
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Custom_Farming_With_Spraying
                                    'tblCustomFarm.Attributes.Add("style", "display:block;")
                                    trCustomFarmingRow.Attributes.Add("style", "display:'';")
                                    lblSpray.Text = "with spray"
                                    lblSprayAnnual.Text = sc.EstimatedReceipts
                                    lblCustomFarmPrem.Text = sc.Premium
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1

                                ' 40054 - Limited Farm Pollution Liability (Increased Limits) - Personal ONLY
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability
                                    'tblFarmPollution.Attributes.Add("style", "display:block;")
                                    trLimitedFarmPollutionRow.Attributes.Add("style", "display:'';")
                                    lblFarmPollutionLimit.Text = If(Decimal.Parse(sc.TotalLimit.Replace("$", "").Replace(",", "")) = 0.0, "Included", sc.TotalLimit)
                                    lblFarmPollutionPrem.Text = If(Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", "")) = 0.0, "Included", sc.Premium)
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1

                                ' 80094 - Liability Enhancement Endorsement - Commercial ONLY
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement
                                    'tblFarmPollution.Attributes.Add("style", "display:block;")
                                    trLimitedFarmPollutionRow.Attributes.Add("style", "display:'';")
                                    'Updated 8/8/2022 for task 76031 MLW
                                    If LiabilityEnhancement1MHelper.IsLiabilityEnhancement1MAvailable(LocalQuickQuote) Then
                                        lblFarmPollutionCovName.Text = "Liability Enhancement Endorsement"
                                        lblFarmPollutionLimit.Text = If(Decimal.Parse(sc.TotalLimit.Replace("$", "").Replace(",", "")) = 0.0, "Included", sc.TotalLimit)
                                        lblFarmPollutionPrem.Text = If(Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", "")) = 0.0, "Included", sc.Premium)
                                    Else
                                        lblFarmPollutionPrem.Text = sc.Premium
                                    End If
                                    'lblFarmPollutionPrem.Text = sc.Premium
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1

                                    ' 70139 - Personal Liability Coverage (GL-9)
                                    'Case QuickQuoteSectionIICoverage.SectionIICoverageType.Location_Farm_Personal_Liability_GL9
                                    '    'tblGL9.Attributes.Add("style", "display:block;")
                                    '    trPersonalLiabilityRow.Attributes.Add("style", "display:'';")
                                    '    lblGL9_Limit.Text = "N/A"
                                    '    lblGL9_Prem.Text = sc.Premium
                                    '    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    '    AddlCovLineCount += 1
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.Motorized_Vehicles_Ohio
                                    trMotorizedVehiclesOHRow.Attributes.Add("style", "display:''")
                                    If sc.Premium IsNot Nothing AndAlso IsNumeric(sc.Premium) AndAlso CDec(sc.Premium) > 0 Then
                                        lblMotoVehPremium.Text = sc.Premium
                                    Else
                                        lblMotoVehPremium.Text = "Included"
                                    End If
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1
                                    Exit Select
                                Case QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion
                                    trCanineExclusionRow.Attributes.Add("style", "display:''")
                                    lblCanineExclusionDesc.Text = String.Empty
                                    lblCanineExclusionLimit.Text = "N/A"
                                    lblCanineExclusionPrem.Text = "N/A"
                                    AddlCovLineCount += 1
                                    Exit Select
                            End Select
                        Next
                    End If

                    ' Section II GL-9 Farm Personal Liability  MGB 10/24/19 Bug 20407
                    ' Get all of the GL-9 section II coverages from all locations
                    Dim GL9s As List(Of QuickQuoteSectionIICoverage) = IFM.VR.Common.Helpers.FARM.FarmPersonalLiabilityGL9Helper.GetAllQuoteGL9s(LocalQuickQuote)
                    If GL9s.Count > 0 Then
                        trPersonalLiabilityRow.Attributes.Add("style", "display:'';")
                        rptGL9.DataSource = GL9s
                    Else
                        trPersonalLiabilityRow.Attributes.Add("style", "display:none;")
                        rptGL9.DataSource = Nothing
                    End If
                    rptGL9.DataBind()

                    If .Locations(0).AdditionalInterests IsNot Nothing Then
                        If .Locations(0).AdditionalInterests.Count > 0 Then
                            trAddlInsuredRow.Attributes.Add("style", "display:'';")
                            'tblAddlInsured.Attributes.Add("style", "display:block;")
                            Dim dtAddlInterestList As DataTable = New DataTable
                            dtAddlInterestList.Columns.Add("RowHdr", GetType(String))
                            dtAddlInterestList.Columns.Add("DisplayName", GetType(String))
                            AddlCovLineCount += 1

                            For Each ai As QuickQuoteAdditionalInterest In .Locations(0).AdditionalInterests
                                Dim newRow As DataRow = dtAddlInterestList.NewRow
                                newRow.Item("RowHdr") = "Additional Interest - "
                                newRow.Item("DisplayName") = ai.Name.DisplayName
                                dtAddlInterestList.Rows.Add(newRow)
                                AddlCovLineCount += 1
                            Next

                            dgAddlInsured.DataSource = dtAddlInterestList
                            dgAddlInsured.DataBind()
                        End If
                    End If

                    If .Locations(0).SectionICoverages IsNot Nothing Then
                        For Each sc As QuickQuoteSectionICoverage In .Locations(0).SectionICoverages
                            Select Case sc.CoverageType
                            ' 70213 - Identity Fraud Expense
                                Case QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpense
                                    'tblFraud.Attributes.Add("style", "display:block;")
                                    trIdentityFraudExpenseRow.Attributes.Add("style", "display:'';")
                                    lblFraudPrem.Text = sc.Premium
                                    TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(sc.Premium.Replace("$", "").Replace(",", ""))
                                    AddlCovLineCount += 1
                            End Select
                        Next
                    End If

                    ' 80125 - Farm All Star
                    If FarmAllStarHelper.IsFarmAllStarAvailable(LocalQuickQuote) Then
                        If SubQuoteFirst.HasFarmAllStar Then
                            trSewerRow.Attributes.Add("style", "display:'';")
                            lblSewerIncrease.Text = ""
                            lblSewerLimit.Text = "N/A"
                            If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmAllStarQuotedPremium)) Then
                                lblSewerPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmAllStarQuotedPremium)
                                TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmAllStarQuotedPremium).Replace("$", "").Replace(",", ""))
                            End If
                            AddlCovLineCount += 1

                            'Water Backup
                            trWaterBackupRow.Attributes.Add("style", "display:'';")
                            lblWaterBackupLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarLimitId, SubQuoteFirst.FarmAllStarWaterBackupLimitId)
                            lblWaterBackupPrem.Text = "Included"
                            'Water Damage
                            trWaterDamageRow.Attributes.Add("style", "display:'';")
                            lblWaterDamageLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarWaterDamageLimitId, SubQuoteFirst.FarmAllStarWaterDamageLimitId)
                            lblWaterDamagePrem.Text = "Included"
                        Else
                            trSewerRow.Attributes.Add("style", "display:none;")
                            trWaterBackupRow.Attributes.Add("style", "display:none;")
                            trWaterDamageRow.Attributes.Add("style", "display:none;")
                        End If
                    Else
                        trWaterBackupRow.Attributes.Add("style", "display:none;")
                        trWaterDamageRow.Attributes.Add("style", "display:none;")
                        If SubQuoteFirst.FarmAllStarLimitId <> "" And SubQuoteFirst.FarmAllStarLimitId <> "0" Then
                            'tblSewer.Attributes.Add("style", "display:block;")
                            trSewerRow.Attributes.Add("style", "display:'';")
                            lblSewerLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.FarmAllStarLimitId, SubQuoteFirst.FarmAllStarLimitId)
                            'Updated 9/11/18 for multi state MLW - premium totals
                            'lblSewerPrem.Text = .FarmAllStarQuotedPremium
                            'totalOptCoverage = totalOptCoverage + Decimal.Parse(.FarmAllStarQuotedPremium.Replace("$", "").Replace(",", ""))
                            If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmAllStarQuotedPremium)) Then
                                lblSewerPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmAllStarQuotedPremium)
                                TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmAllStarQuotedPremium).Replace("$", "").Replace(",", ""))
                            End If
                            AddlCovLineCount += 1
                        End If
                    End If

                    ' 80140 Extra Expense
                    'For Each sq As QuickQuoteObject In SubQuotes
                    '    If sq.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
                    '        Dim extraExpense As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                    '        If extraExpense IsNot Nothing Then
                    '            hasExtraExpense = True
                    '            extraExpenseLimit = extraExpense.IncreasedLimit
                    '            extraExpensePremium = qqHelper.getSumAndOptionallyMaintainFormatting(extraExpensePremium, extraExpense.Premium, maintainFormattingOrDefaultValue:=True)
                    '        End If
                    '    End If
                    'Next
                    'If hasExtraExpense = True Then
                    '    Type = "Extra Expense"
                    '    lim = extraExpenseLimit
                    '    desc = ""
                    '    prem = If(Decimal.Parse(extraExpensePremium.Replace("$", "").Replace(",", "")) = 0.0, "Included", extraExpensePremium)
                    '    ACTotal += CDec(extraExpensePremium)
                    '    ACTable.Add(New With {.Type = Type, .Desc = desc, .limits = lim, .Premium = prem})
                    'End If

                    ' Noe that extra expense was moved outside the statequotes loop at the top of this function MGB 2-1-19

                    'Updated 9/25/18 for multi state MLW
                    'Dim hasExtraExpense As Boolean = False
                    'Dim extraExpenseLimit As String = ""
                    'Dim extraExpensePremium As String = ""
                    'For Each sq As QuickQuoteObject In SubQuotes
                    '    If sq.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
                    '        Dim extraExpense As QuickQuoteOptionalCoverage = sq.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)
                    '        If extraExpense IsNot Nothing Then
                    '            hasExtraExpense = True
                    '            extraExpenseLimit = extraExpense.IncreasedLimit
                    '            extraExpensePremium = qqHelper.getSumAndOptionallyMaintainFormatting(extraExpensePremium, extraExpense.Premium, maintainFormattingOrDefaultValue:=True)
                    '        End If
                    '    End If
                    'Next
                    'If hasExtraExpense = True Then
                    '    lblExtraExpense_Limit.Text = extraExpenseLimit
                    '    lblExtraExpense_Prem.Text = If(Decimal.Parse(extraExpensePremium.Replace("$", "").Replace(",", "")) = 0.0, "Included", extraExpensePremium)
                    '    tblExtraExpense.Attributes.Add("style", "display:block;")
                    '    totalOptCoverage = totalOptCoverage + Decimal.Parse(extraExpensePremium.Replace("$", "").Replace(",", ""))
                    '    AddlCovLineCount += 1
                    'Else
                    '    tblExtraExpense.Attributes.Add("style", "display:none;")
                    'End If
                    'If SubQuoteFirst.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense).Count > 0 Then
                    '    Dim extraExpense As QuickQuoteOptionalCoverage = SubQuoteFirst.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_F_G_Extra_Expense)

                    '    'TODO: Mary - Update this like ctlQuoteSummary_FAR
                    '    If extraExpense IsNot Nothing Then
                    '        lblExtraExpense_Limit.Text = extraExpense.IncreasedLimit
                    '        lblExtraExpense_Prem.Text = If(Decimal.Parse(extraExpense.Premium.Replace("$", "").Replace(",", "")) = 0.0, "Included", extraExpense.Premium)
                    '        tblExtraExpense.Attributes.Add("style", "display:block;")
                    '        totalOptCoverage = totalOptCoverage + Decimal.Parse(extraExpense.Premium.Replace("$", "").Replace(",", ""))
                    '        AddlCovLineCount += 1
                    '    Else
                    '        tblExtraExpense.Attributes.Add("style", "display:none;")
                    '    End If
                    'End If


                    ' Farm Extender - Boolean
                    If SubQuoteFirst.HasFarmExtender Then
                        'tblFarmExtend.Attributes.Add("style", "display:block;")
                        trFarmExtenderRow.Attributes.Add("style", "display:'';")
                        'Updated 9/11/18 for multi state MLW - premium totals
                        'lblFarmExtend_Prem.Text = If(Decimal.Parse(.FarmExtenderQuotedPremium.Replace("$", "").Replace(",", "")) = 0.0, "Included", .FarmExtenderQuotedPremium)
                        'totalOptCoverage = totalOptCoverage + Decimal.Parse(.FarmExtenderQuotedPremium.Replace("$", "").Replace(",", ""))
                        If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmExtenderQuotedPremium)) Then
                            lblFarmExtend_Prem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmExtenderQuotedPremium)
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmExtenderQuotedPremium).Replace("$", "").Replace(",", ""))
                        End If
                        AddlCovLineCount += 1
                    Else
                        trFarmExtenderRow.Attributes.Add("style", "display:none;")
                        'tblFarmExtend.Attributes.Add("style", "display:none;")
                    End If

                    ' Equipment Breakdown - Boolean
                    If SubQuoteFirst.HasFarmEquipmentBreakdown Then
                        'tblBreakdown.Attributes.Add("style", "display:block;")
                        trEquipmentBreakdownRow.Attributes.Add("style", "display:'';")
                        'Updated 9/11/18 for multi state MLW - premium totals
                        'lblBreakdownPrem.Text = .FarmEquipmentBreakdownQuotedPremium
                        'totalOptCoverage = totalOptCoverage + Decimal.Parse(.FarmEquipmentBreakdownQuotedPremium.Replace("$", "").Replace(",", ""))
                        If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEquipmentBreakdownQuotedPremium)) Then
                            lblBreakdownPrem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEquipmentBreakdownQuotedPremium)
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.FarmEquipmentBreakdownQuotedPremium).Replace("$", "").Replace(",", ""))
                        End If
                        AddlCovLineCount += 1
                    End If

                    ' 70125 - Pollutant Clean Up and Removal - Increased Limits
                    'Updated 9/25/18 for multi state MLW
                    Dim hasFarmIncidentalLimits As Boolean = False
                    Dim farmIncidentalLimit As String = ""
                    Dim farmIncidentalIncreasedLimit As String = ""
                    Dim farmIncidentalPremium As String = ""
                    For Each sq As QuickQuoteObject In SubQuotes
                        If sq.FarmIncidentalLimits IsNot Nothing Then
                            Dim incidentalLimits = sq.FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal)
                            'If .FarmIncidentalLimits.FindAll(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal).Count > 0 Then
                            If incidentalLimits IsNot Nothing Then
                                hasFarmIncidentalLimits = True
                                farmIncidentalPremium = qqHelper.getSumAndOptionallyMaintainFormatting(farmIncidentalPremium, incidentalLimits.Premium, maintainFormattingOrDefaultValue:=True)
                                farmIncidentalIncreasedLimit = incidentalLimits.IncreasedLimitId
                            End If
                        End If
                    Next
                    If hasFarmIncidentalLimits = True Then
                        If farmIncidentalIncreasedLimit <> "" AndAlso farmIncidentalIncreasedLimit <> "0" Then
                            'tblPollution.Attributes.Add("style", "display:block;")
                            trPollutionCleanupRow.Attributes.Add("style", "display:'';")
                            lblPollutionCleanLimit.Text = "25,000"
                            lblPollutionCleanPrem.Text = farmIncidentalPremium
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(farmIncidentalPremium.Replace("$", "").Replace(",", ""))
                            AddlCovLineCount += 1
                        End If
                    End If
                    'If SubQuoteFirst.FarmIncidentalLimits IsNot Nothing Then
                    '    Dim incidentalLimits = .FarmIncidentalLimits.Find(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal)
                    '    'If .FarmIncidentalLimits.FindAll(Function(p) p.CoverageType = QuickQuoteFarmIncidentalLimit.QuickQuoteFarmIncidentalLimitType.Farm_Pollutant_Clean_Up_and_Removal).Count > 0 Then
                    '    If incidentalLimits IsNot Nothing Then
                    '        If incidentalLimits.IncreasedLimitId <> "" AndAlso incidentalLimits.IncreasedLimitId <> "0" Then
                    '            tblPollution.Attributes.Add("style", "display:block;")
                    '            lblPollutionCleanLimit.Text = "25,000"
                    '            lblPollutionCleanPrem.Text = incidentalLimits.Premium
                    '            totalOptCoverage = totalOptCoverage + Decimal.Parse(incidentalLimits.Premium.Replace("$", "").Replace(",", ""))
                    '            AddlCovLineCount += 1
                    '        End If
                    '    End If
                    'End If

                    ' Bug 43384 - HasEPLI must be true AND EPLI COverage Type must be epli UW in order to display.  MGB 11/16/20
                    If SubQuoteFirst.HasEPLI AndAlso SubQuoteFirst.EPLICoverageTypeID = "22" Then
                        'tblEPLI.Attributes.Add("style", "display:block;")
                        trEPLIRow.Attributes.Add("style", "display:'';")
                        lblEPLI.Text = SubQuoteFirst.EPLICoverageType
                        lblEPLIDesc.Text = "Deductible: " + SubQuoteFirst.EPLIDeductible.ToString()
                        lblEPLI_Limit.Text = "Policy/Aggregate: " + SubQuoteFirst.EPLICoverageLimit.ToString()
                        'Updated 9/11/18 for multi state MLW - premium totals
                        'lblEPLI_Prem.Text = .EPLIPremium
                        'totalOptCoverage = totalOptCoverage + Decimal.Parse(.EPLIPremium.Replace("$", "").Replace(",", ""))
                        If IsNumeric(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.EPLIPremium)) Then
                            lblEPLI_Prem.Text = qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.EPLIPremium)
                            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(qqHelper.GetSumForPropertyValues(SubQuotes, Function() LocalQuickQuote.EPLIPremium).Replace("$", "").Replace(",", ""))
                        End If
                        AddlCovLineCount += 1
                    End If

                    ' Loss of Income
                    ' Build a table of all loss of income items, bind it to the repeater
                    Dim tbl As New DataTable()
                    tbl.Columns.Add("type")
                    tbl.Columns.Add("limit")
                    tbl.Columns.Add("desc")
                    tbl.Columns.Add("prem")
                    For Each LOC As QuickQuoteLocation In .Locations
                        If LOC.IncomeLosses IsNot Nothing Then
                            trLossOfIncomeRow.Attributes.Add("style", "display:'';")
                            For Each IncomeLossItem As QuickQuoteIncomeLoss In LOC.IncomeLosses
                                Dim desc As String = ""
                                Dim prem As String = ""
                                ' Format Description - location & Building
                                Dim temp As String() = IncomeLossItem.Description.Split("BLD")
                                If temp IsNot Nothing AndAlso temp.Length = 2 Then
                                    Dim locNum As String = temp(0).Remove(0, 3)
                                    Dim bldnum As String = temp(1).Remove(0, 2)
                                    desc = "Location " & locNum & ", Building " & bldnum
                                End If
                                ' Make sure premium is formatted correctly
                                If IsNumeric(IncomeLossItem.QuotedPremium) Then
                                    prem = Format(CDec(IncomeLossItem.QuotedPremium), "$###,###,##0.00")
                                Else
                                    prem = ""
                                End If
                                Dim newrow As DataRow = tbl.NewRow()
                                newrow("type") = "Loss Of Income"
                                newrow("limit") = IncomeLossItem.Limit
                                newrow("desc") = desc
                                newrow("prem") = prem
                                tbl.Rows.Add(newrow)
                            Next
                        End If
                    Next
                    rptLossOfIncome.DataSource = tbl
                    rptLossOfIncome.DataBind()

                    ' Additional Residence Rented To Others (GL-73)
                    Dim addlResidenceRentedToOthersCovs As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage) = IFM.VR.Common.Helpers.FARM.AddlResidenceRentedToOthersHelper.GetAddlResidenceRentedToOthersCoverages(LocalQuickQuote)
                    If addlResidenceRentedToOthersCovs IsNot Nothing AndAlso addlResidenceRentedToOthersCovs.Count > 0 Then
                        'divAddlResidenceRentedToOthers.Attributes.Add("style", "display:''")
                        trAddlResidenceRentedToOthersRow.Attributes.Add("style", "display:''")
                        rptAddlResidenceRentedToOthers.DataSource = addlResidenceRentedToOthersCovs
                        rptAddlResidenceRentedToOthers.DataBind()
                        AddlCovLineCount += addlResidenceRentedToOthersCovs.Count
                    End If

                    ''' Property in Transit - Leaving in case BAs change mind - delete after 4/29/2020
                    ''If GoverningStateQuote.OptionalCoverages.FindAll(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit).Count > 0 Then
                    ''    Dim propTransit As QuickQuoteOptionalCoverage = GoverningStateQuote.OptionalCoverages.Find(Function(p) p.CoverageType = QuickQuoteOptionalCoverage.QuickQuoteOptionalCoverageType.Farm_Property_in_Transit)
                    ''    If propTransit IsNot Nothing Then
                    ''        trPropertyInTransit.Attributes.Add("style", "display:''")
                    ''        lblPropertyInTransitLimit.Text = propTransit.IncludedLimit
                    ''        lblPropertyInTransitPrem.Text = propTransit.Premium
                    ''        If propTransit.Coverage?.WrittenPremium?.IsNumeric Then
                    ''            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + CDec(propTransit.Coverage.WrittenPremium)
                    ''        End If
                    ''        AddlCovLineCount += 1
                    ''    End If
                    ''End If

                    ' Ref Food Spoilage
                    If Common.Helpers.FARM.RefFoodSpoilageHelper.IsRefFoodSpoilageAvailable(LocalQuickQuote) Then
                        If LocalQuickQuote.Locations(0).IncidentalDwellingCoverages IsNot Nothing Then
                            Dim RefFoodSpoilage As QuickQuoteCoverage
                            RefFoodSpoilage = LocalQuickQuote.Locations(0).IncidentalDwellingCoverages.Find(Function(p) p.CoverageCodeId = "70148")
                            If RefFoodSpoilage Is Nothing Then
                                RefFoodSpoilage = New QuickQuoteCoverage
                                RefFoodSpoilage.ManualLimitAmount = 500
                                RefFoodSpoilage.ManualLimitIncreased = 0
                            End If
                            trRefFoodSpoilageRow.Attributes.Add("style", "display:''")
                            lblRefFoodSpoilage_Limit.Text = Format(CDec(RefFoodSpoilage.ManualLimitAmount), "###,###,###")

                            If qqHelper.IsPositiveIntegerString(RefFoodSpoilage.ManualLimitIncreased) Then
                                'lblRefFoodSpoilage_Limit.Text = Format(CDec(RefFoodSpoilage.ManualLimitAmount), "###,###,###")
                                lblRefFoodSpoilage_Prem.Text = Format(CDec(RefFoodSpoilage.FullTermPremium), "$###,###,##0.00")
                                TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(RefFoodSpoilage.FullTermPremium.Replace("$", "").Replace(",", ""))
                            Else
                                'lblRefFoodSpoilage_Limit.Text = String.Empty
                                lblRefFoodSpoilage_Prem.Text = "Included"
                            End If

                            AddlCovLineCount += 1
                        End If
                    End If
                End With
            End If

            If AddlCovLineCount = 0 Then
                AddlCovLineCount = 1
                dvAddlCoverage.Attributes.Add("style", "display:none;")
                lblNoAddlCovExist.Attributes.Add("style", "display:block;")
            End If

            lblTotalAddlPremData.Text = IFM.Common.InputValidation.InputHelpers.TryToFormatAsCurrency(TotalOptionalCoveragesPremium.ToString(), True)
        End If
    End Sub

    'Private Function GetAddlResidenceRentedToOthersCoverages() As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
    '    Dim covs As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)

    '    If LocalQuickQuote Is Nothing Then Return covs
    '    If LocalQuickQuote.Locations Is Nothing OrElse LocalQuickQuote.Locations.Count <= 0 Then Return covs
    '    'If LocalQuickQuote.Locations(0).SectionIICoverages Is Nothing OrElse LocalQuickQuote.Locations(0).SectionIICoverages.Count <= 0 Then Return covs

    '    'For Each c As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In LocalQuickQuote.Locations(0).SectionIICoverages
    '    '    If c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then
    '    '        covs.Add(c)
    '    '    End If
    '    'Next

    '    For Each l In LocalQuickQuote.Locations
    '        If l.SectionIICoverages IsNot Nothing AndAlso l.SectionIICoverages.Count > 0 Then
    '            For Each c As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage In l.SectionIICoverages
    '                If c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers Then
    '                    covs.Add(c)
    '                End If
    '            Next
    '        End If
    '    Next

    '    Return covs
    'End Function

    Private Sub ToggleFarmPersonalProperty(state As Boolean) Handles ctlPersonalPropertyList.FarmPropertyExist
        If state Then
            lblNoPersPropExist.Visible = False
            'tblPersonalProperty.Visible = True
            ctlPersonalPropertyList.Visible = True
        Else
            lblNoPersPropExist.Visible = True
            'tblPersonalProperty.Visible = False
            ctlPersonalPropertyList.Visible = False
        End If
    End Sub

    Private Sub ToggleFarmIncidental(state As Boolean) Handles ctlFarmIncidentalLimits.FarmIncidentalExist
        If state Then
            lblNoFarmIncidentalLimitsExist.Visible = False
            'tblPersonalProperty.Visible = True
            ctlFarmIncidentalLimits.Visible = True
        Else
            lblNoFarmIncidentalLimitsExist.Visible = True
            'tblPersonalProperty.Visible = False
            ctlFarmIncidentalLimits.Visible = False
        End If
    End Sub

    Private Sub rptAddlResidenceRentedToOthers_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptAddlResidenceRentedToOthers.ItemDataBound
        Dim tblAddlResidenceRentedToOthers As HtmlTable = e.Item.FindControl("tblAddlResidenceRentedToOthers")
        Dim lblAddlResDesc As Label = e.Item.FindControl("lblAddlResDesc")
        Dim lblAddlResPremium As Label = e.Item.FindControl("lblAddlResPremium")

        Dim S2Cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = e.Item.DataItem

        Dim addr As String = ""
        If S2Cov.Address IsNot Nothing Then
            addr = S2Cov.Address.DisplayAddress  ' They decided they want to display the entire address
            'addr += S2Cov.Address.HouseNum
            'If addr.Trim <> "" Then addr += " "
            'addr += S2Cov.Address.StreetName
            'If addr.Trim <> "" Then addr += " "
            'Dim zip5 As String = S2Cov.Address.Zip
            'If S2Cov.Address.Zip.Length > 5 Then zip5 = S2Cov.Address.Zip.Substring(0, 5)
            'addr += zip5
            'If addr.Length > 43 Then addr = addr.Substring(0, 40) & "..."
        End If
        lblAddlResDesc.Text = "Number of Families: " & S2Cov.NumberOfFamilies & "; " & addr

        lblAddlResPremium.Text = S2Cov.Premium

        If IsNumeric(S2Cov.Premium) Then TotalOptionalCoveragesPremium += CDec(S2Cov.Premium)

        Exit Sub
    End Sub

    Private Sub rptGL9_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptGL9.ItemDataBound
        Dim tblGL9 As HtmlTable = e.Item.FindControl("tblGL9")
        Dim lblDesc As Label = e.Item.FindControl("lblGL9_Desc")
        Dim lblLimit As Label = e.Item.FindControl("lblGL9_Limit")
        Dim lblPrem As Label = e.Item.FindControl("lblGL9_Prem")

        Dim MyGL9 As QuickQuoteSectionIICoverage = e.Item.DataItem

        If MyGL9 IsNot Nothing Then
            If MyGL9.Name IsNot Nothing Then
                lblDesc.Text = MyGL9.Name.FirstName & " " & MyGL9.Name.LastName
            End If
            lblLimit.Text = "N/A"
            lblPrem.Text = MyGL9.Premium

            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(MyGL9.Premium.Replace("$", "").Replace(",", ""))
            AddlCovLineCount += 1
        End If

        Exit Sub
    End Sub

    Private Sub rptLossOfIncome_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptLossOfIncome.ItemDataBound
        Dim tblLI As HtmlTable = e.Item.FindControl("tblLossOfIncome")
        Dim lblDesc As Label = e.Item.FindControl("lblLossOfIncomeDesc")
        Dim lblLimit As Label = e.Item.FindControl("lblLossOfIncomeLimit")
        Dim lblPrem As Label = e.Item.FindControl("lblLossOfIncomePremium")

        Dim dr As DataRowView = e.Item.DataItem

        If dr IsNot Nothing Then
            lblDesc.Text = dr("desc").ToString
            lblLimit.Text = dr("limit").ToString
            lblPrem.Text = dr("prem").ToString

            TotalOptionalCoveragesPremium = TotalOptionalCoveragesPremium + Decimal.Parse(dr("prem").ToString.Replace("$", "").Replace(",", ""))
            AddlCovLineCount += 1
        End If

        Exit Sub
    End Sub
End Class