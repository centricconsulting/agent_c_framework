Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Imports Diamond.Common.Services.Interfaces
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Data
Imports System.Xml
Imports System.Diagnostics
Imports System.IO
Imports System.Threading

Public Class PFQuoteSummary_DFR
    Inherits System.Web.UI.Page

    Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim summaryHelper As New SummaryHelperClass
    Dim fileName As String = ""
    Dim quoteIds As List(Of String)
    Dim noSurchargesExist As Boolean = True

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
            Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, quickQuote.Locations(0).FormTypeId).Substring(0, 4)
        End Get
    End Property

    'Added 9/20/2019 for Home Endorsements Task 40274 MLW
    Public ReadOnly Property IsBillingUpdate() As Boolean
        Get
            Dim result As Boolean = False
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("isBillingUpdate").ToString) = False Then
                result = CBool(Request.QueryString("isBillingUpdate").ToString)
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("isBillingUpdate").ToString) = False Then
                result = CBool(Page.RouteData.Values("isBillingUpdate").ToString)
            Else
                If quickQuote IsNot Nothing AndAlso quickQuote.IsBillingEndorsement = True Then
                    result = True
                End If
            End If
            Return result
        End Get

    End Property

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim summaryType As String = Request.QueryString("summarytype").ToString
            'Updated 9/20/2019 for DFR Endorsements Project Task 40274 MLW
            If (RatedQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) Then
                EndorsementPrintSection.Visible = True
                QuoteAppPrintSection.Visible = False
            Else
                EndorsementPrintSection.Visible = False
                QuoteAppPrintSection.Visible = True

                'Me.lblQuoteId.Text = Request.QueryString("QuoteId").ToString
                'updated 5/10/2019
                Me.lblQuoteId.Text = If(Request.QueryString("QuoteId") IsNot Nothing, Request.QueryString("QuoteId").ToString, "")
                Dim reportHeader As StringBuilder = New StringBuilder
                reportHeader.Append("Indiana Farmers Mutual Insurance Group ")

                If summaryType = "App" Then
                    reportHeader.Append("Application Summary")
                Else
                    reportHeader.Append("Quote Summary")
                End If

                lblHeader.Text = reportHeader.ToString
                ctlClientAndAgencyInfo.ToggleProducer(False)
            End If
        GetQuoteFromDb(summaryType)
            LoadSummaryObjects()
        End If
    End Sub

    Private Sub LoadSummaryObjects()
        If quickQuote IsNot Nothing Then 'added IF 2/19/2019
            '9/10/18 No updates needed for multi-state since this is for DFR only
            If quickQuote.Applicants IsNot Nothing Then
                Dim applicantStr As StringBuilder = New StringBuilder()
                Dim Name As String = String.Format("{0} {1} {2} {3}", quickQuote.Policyholder.Name.FirstName, quickQuote.Policyholder.Name.MiddleName, quickQuote.Policyholder.Name.LastName, quickQuote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                applicantStr.Append(Name + "<br />")
                If quickQuote?.Policyholder2?.Name?.DisplayNameForWeb IsNot Nothing AndAlso quickQuote.Policyholder2.Name.DisplayNameForWeb.IsNullEmptyorWhitespace() = False Then
                    Dim Name2 As String = String.Format("{0}", quickQuote.Policyholder2.Name.DisplayNameForWeb).Replace("  ", " ").Trim()
                    applicantStr.Append(Name2 + "<br />")
                End If
                Me.ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(applicantStr.ToString().Substring(0, applicantStr.Length - 6), quickQuote.Policyholder.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                Me.ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.ClientInfo, quickQuote.Policyholder.PrimaryPhone, "<br />")
            End If

            Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(summaryHelper.GetAgencyName(quickQuote.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), quickQuote.Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")

            'Dim trimPrimaryPhone As String = ""
            'Try
            '    trimPrimaryPhone = quickQuote.Agency.PrimaryPhone.Trim.Remove(quickQuote.Agency.PrimaryPhone.Length - 2)
            'Catch ex As Exception
            '    trimPrimaryPhone = ""
            'End Try

            'Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.AgencyInfo, trimPrimaryPhone, "<br />")
            'updated 9/19/2017
            Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.AgencyInfo, quickQuote.Agency.PrimaryPhone, "<br />")
            Me.ctlClientAndAgencyInfo.ProducerCode = quickQuote.AgencyProducerCode
        End If
    End Sub

    Private Sub ShowCoverageA()
        Dim PremiumInt As Double = 0
        Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncluded)
        Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncreased)


        lblCovALimit.Text = (A_included + A_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).A_Dwelling_QuotedPremium
        If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
            PremiumInt = PremiumInt + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_EC_QuotedPremium) + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_VMM_QuotedPremium)
        End If
        If PremiumInt >= 0 Then
            If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
                lblCovAPrem.Text = PremiumInt.TryToFormatAsCurreny()
            Else
                lblCovAPrem.Text = quickQuote.Locations(0).A_Dwelling_QuotedPremium
            End If
        Else
            lblCovAPrem.Text = "Included"
        End If
    End Sub

    Private Sub HideCoverageA()
        pnlCovADwelling.Visible = False
    End Sub

    Private Sub ShowCovAEC()
        Dim PremiumInt As Integer = 0
        Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncluded)
        Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncreased)
        lblCovAEC_Limit.Text = (A_included + A_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).A_Dwelling_EC_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovAEC_Prem.Text = quickQuote.Locations(0).A_Dwelling_EC_QuotedPremium
        Else
            lblCovAEC_Prem.Text = "Included"
        End If

        If ((A_included + A_increased) > 0) Then
            pnlCovAEC.Visible = True
        Else
            pnlCovAEC.Visible = False
        End If

    End Sub

    Private Sub HideCovAEC()
        pnlCovAEC.Visible = False
    End Sub
    Private Sub ShowCovAVMM()
        Dim PremiumInt As Integer = 0
        Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncluded)
        Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncreased)
        lblCovAVMM_Limit.Text = (A_included + A_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).A_Dwelling_VMM_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovAVMM_Prem.Text = quickQuote.Locations(0).A_Dwelling_VMM_QuotedPremium
        Else
            lblCovAVMM_Prem.Text = "Included"
        End If
        If (quickQuote.Locations(0).FormTypeId <> "9") Then
            If ((A_included + A_increased) > 0) Then
                pnlCovAVMM.Visible = True
            Else
                pnlCovAVMM.Visible = False
            End If

        End If

    End Sub

    Private Sub HideCovAVMM()
        pnlCovAVMM.Visible = False
    End Sub

    Private Sub ShowCoverageB()
        Dim PremiumInt As Double = 0
        Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncluded)
        Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncreased)
        lblCovBLimit.Text = (B_included + B_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).B_OtherStructures_QuotedPremium
        If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
            PremiumInt = PremiumInt + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_EC_QuotedPremium) + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_VMM_QuotedPremium)
        End If
        If PremiumInt >= 0 Then
            If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
                lblCovBPrem.Text = PremiumInt.TryToFormatAsCurreny()
            Else
                lblCovBPrem.Text = quickQuote.Locations(0).B_OtherStructures_QuotedPremium
            End If
        Else
            lblCovBPrem.Text = "Included"
        End If
    End Sub

    Private Sub HideCoverageB()
        pnlCovBStruct.Visible = False
    End Sub

    Private Sub ShowCovBEC()
        Dim PremiumInt As Integer = 0
        Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncluded)
        Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncreased)
        lblCovBEC_Limit.Text = (B_included + B_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).B_OtherStructures_EC_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovBEC_Prem.Text = quickQuote.Locations(0).B_OtherStructures_EC_QuotedPremium
        Else
            lblCovBEC_Prem.Text = "Included"
        End If
        If ((B_included + B_increased) > 0) Then
            pnlCovBEC.Visible = True
        Else
            pnlCovBEC.Visible = False
        End If


    End Sub

    Private Sub HideCovBEC()
        pnlCovBEC.Visible = False
    End Sub

    Private Sub ShowCovBVMM()
        Dim PremiumInt As Integer = 0
        Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncluded)
        Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncreased)
        lblCovBVMM_Limit.Text = (B_included + B_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).B_OtherStructures_VMM_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovBVMM_Prem.Text = quickQuote.Locations(0).B_OtherStructures_VMM_QuotedPremium
        Else
            lblCovBVMM_Prem.Text = "Included"
        End If
        If (quickQuote.Locations(0).FormTypeId <> "9") Then
            If ((B_included + B_increased) > 0) Then
                pnlCovBVMM.Visible = True
            Else
                pnlCovBVMM.Visible = False
            End If

        End If

    End Sub

    Private Sub HideCovBVMM()
        pnlCovBVMM.Visible = False
    End Sub

    Private Sub ShowCovCEC()
        Dim PremiumInt As Integer = 0
        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncreased)
        lblCovCEC_Limit.Text = (C_included + C_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).C_Contents_EC_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovCEC_Prem.Text = quickQuote.Locations(0).C_Contents_EC_QuotedPremium
        Else
            lblCovCEC_Prem.Text = "Included"
        End If
        If ((C_included + C_increased) > 0) Then
            pnlCovCEC.Visible = True
        Else
            pnlCovCEC.Visible = False
        End If

    End Sub
    Private Sub HideCovCEC()
        pnlCovCEC.Visible = False
    End Sub

    Private Sub ShowCovCVMM()
        Dim PremiumInt As Integer = 0
        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncreased)
        lblCovCVMM_Limit.Text = (C_included + C_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).C_Contents_VMM_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovCVMM_Prem.Text = quickQuote.Locations(0).C_Contents_VMM_QuotedPremium
        Else
            lblCovCVMM_Prem.Text = "Included"
        End If
        If (quickQuote.Locations(0).FormTypeId <> "9") Then
            If ((C_included + C_increased) > 0) Then
                pnlCovCVMM.Visible = True
            Else
                pnlCovCVMM.Visible = False
            End If
        End If

    End Sub
    Private Sub HideCovCVMM()
        pnlCovCVMM.Visible = False
    End Sub

    Private Sub ShowCovDEC()
        Dim PremiumInt As Integer = 0
        Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncluded)
        Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncreased)

        lblCovDEC_Limit.Text = (D_included + D_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).D_and_E_EC_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovDEC_Prem.Text = quickQuote.Locations(0).D_and_E_EC_QuotedPremium
        Else
            lblCovDEC_Prem.Text = "Included"
        End If
        If ((D_included + D_increased) > 0) Then
            pnlCovDEC.Visible = True
        Else
            pnlCovDEC.Visible = False
        End If



    End Sub

    Private Sub HideCovDEC()
        pnlCovDEC.Visible = False
    End Sub

    Private Sub ShowCovDVMM()
        Dim PremiumInt As Integer = 0
        Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncluded)
        Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncreased)

        lblCovDVMM_Limit.Text = (D_included + D_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).D_and_E_VMM_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovDVMM_Prem.Text = quickQuote.Locations(0).D_and_E_VMM_QuotedPremium
        Else
            lblCovDVMM_Prem.Text = "Included"
        End If
        If (quickQuote.Locations(0).FormTypeId <> "9") Then
            If ((D_included + D_increased) > 0) Then
                pnlCovDVMM.Visible = True
            Else
                pnlCovDVMM.Visible = False
            End If
        End If

    End Sub

    Private Sub HideCovDVMM()
        pnlCovDVMM.Visible = False
    End Sub

    Private Sub ShowCoverageC()
        Dim PremiumInt As Double = 0
        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncreased)
        lblCovCLimit.Text = (C_included + C_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).C_PersonalProperty_QuotedPremium
        If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
            PremiumInt = PremiumInt + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_Contents_EC_QuotedPremium) + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_Contents_VMM_QuotedPremium)
        End If
        If PremiumInt >= 0 Then
            If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
                lblCovCPrem.Text = PremiumInt.TryToFormatAsCurreny()
            Else
                lblCovCPrem.Text = quickQuote.Locations(0).C_PersonalProperty_QuotedPremium
            End If
        Else
            lblCovCPrem.Text = "Included"
        End If
    End Sub

    Private Sub ShowCoverageD(formNum As String)
        Dim PremiumInt As Double = 0
        Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncluded)
        Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncreased)

        If formNum <> "ML-2" And formNum <> "ML-4" Then
            lblCovDLimit.Text = (D_included + D_increased).ToString("N0")

            PremiumInt = quickQuote.Locations(0).D_LossOfUse_QuotedPremium
            If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
                PremiumInt = PremiumInt + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_and_E_EC_QuotedPremium) + IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_and_E_VMM_QuotedPremium)
            End If
            If PremiumInt >= 0 Then
                If (quickQuote.Locations(0).FormTypeId = "11" Or quickQuote.Locations(0).FormTypeId = "12") Then
                    lblCovDPrem.Text = PremiumInt.TryToFormatAsCurreny()
                Else
                    lblCovDPrem.Text = quickQuote.Locations(0).D_LossOfUse_QuotedPremium
                End If
            Else
                lblCovDPrem.Text = "Included"
            End If
        End If
    End Sub

    Public Sub GetQuoteFromDb(summaryType As String)
        Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
        Dim val_item_counter As Integer = 0 'added 4/11/2013
        Dim noPolicyDiscounts As Boolean = True
        Dim policyCount As Integer = 25
        Dim inclCount As Integer = 2
        Dim propCount As Integer = 2
        Dim liabCount As Integer = 4
        Dim imCount As Integer = 4
        Dim rvwCount As Integer = 5
        Dim surCount As Integer = 5
        Dim creditCount As Integer = 0

        'If IsNumeric(Me.lblQuoteId.Text) = True Then 'removed IF 2/19/2019
        'Dim errorMsg As String = ""
        'Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
        'QQxml.GetRatedQuote(Me.lblQuoteId.Text, quickQuote, rateType, errorMsg)
        'updated 2/19/2019; note: rateType should be set by method, though none of these have the param as ByRef
        quickQuote = RatedQuote
        'If quickQuote.Locations IsNot Nothing Then
        'updated 2/19/2019
        If quickQuote IsNot Nothing AndAlso quickQuote.Locations IsNot Nothing AndAlso quickQuote.Locations.Count > 0 Then
            With quickQuote
                'Policy Information
                Me.lblQuoteNumber.Text = .QuoteNumber
                Me.lblDate.Text = DateTime.Now.ToShortDateString()
                Me.lblEffectiveDate.Text = .EffectiveDate
                Me.lblTier.Text = .TieringInformation.RatedTier
                Me.lblPremium.Text = .TotalQuotedPremium



                'Added 9/20/2019 for Home Endorsements Project Task 40274 MLW
                Dim loadEndorsementReadOnlyHeader As Boolean = False
                Select Case .QuoteTransactionType
                    Case QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        If String.IsNullOrWhiteSpace(.PolicyNumber) = False AndAlso Left(UCase(.PolicyNumber), 1) = "Q" Then
                            Me.lblQuoteNumber.Text = .QuoteNumber
                        Else
                            Me.lblQuoteNumber.Text = .PolicyNumber
                            If String.IsNullOrWhiteSpace(Me.lblQuoteNo.Text) = False AndAlso Len(Me.lblQuoteNo.Text) >= 5 AndAlso Left(Me.lblQuoteNo.Text, 5) = "Quote" Then
                                Me.lblQuoteNo.Text = Me.lblQuoteNo.Text.Replace("Quote", "Policy")
                            End If
                        End If
                        Me.lblEffectiveDate.Text = .TransactionEffectiveDate
                        If String.IsNullOrWhiteSpace(Me.lblEffective.Text) = False AndAlso Len(Me.lblEffective.Text) >= 8 AndAlso Left(Me.lblEffective.Text, 8) = "Proposed" Then
                            Me.lblEffective.Text = Me.lblEffective.Text.Replace("Proposed", "Image")
                        End If
                        loadEndorsementReadOnlyHeader = True
                    Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Me.lblQuoteNumber.Text = .PolicyNumber
                        If String.IsNullOrWhiteSpace(Me.lblQuoteNo.Text) = False AndAlso Len(Me.lblQuoteNo.Text) >= 5 AndAlso Left(Me.lblQuoteNo.Text, 5) = "Quote" Then
                            Me.lblQuoteNo.Text = Me.lblQuoteNo.Text.Replace("Quote", "Policy")
                        End If
                        Me.lblEffectiveDate.Text = .TransactionEffectiveDate
                        If String.IsNullOrWhiteSpace(Me.lblEffective.Text) = False AndAlso Len(Me.lblEffective.Text) >= 8 AndAlso Left(Me.lblEffective.Text, 8) = "Proposed" Then
                            Me.lblEffective.Text = Me.lblEffective.Text.Replace("Proposed", "Change")
                        End If
                        loadEndorsementReadOnlyHeader = True
                    Case Else
                        Me.lblQuoteNumber.Text = .QuoteNumber
                        Me.lblEffectiveDate.Text = .EffectiveDate
                End Select
                If loadEndorsementReadOnlyHeader = True Then
                    Me.ctlEndorsementOrChangeHeader.Visible = True
                    Me.ctlEndorsementOrChangeHeader.Quote = quickQuote
                End If

                'Updated 9/20/2019 for Home Endorsements Task 40286 MLW
                If isBillingUpdate() Then
                    quoteSummaryDetailsContent.Visible = False
                Else






                    If .Locations(0) IsNot Nothing Then
                        'Policy Coverages
                        lblResidenceData.Text = String.Format("{0} {1}", .Locations(0).Address.HouseNum, .Locations(0).Address.StreetName)
                        lblFormData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, .Locations(0).FormTypeId)
                        lblDeductibleData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, .Locations(0).DeductibleLimitId)



                        'Base Coverages
                        Select Case lblFormData.Text
                            Case "DP 00 01 - Fire"
                                ShowCoverageA()
                                ShowCoverageB()
                                ShowCoverageC()
                                ShowCoverageD(CurrentForm)
                                policyCount += 4
                                HideCovAEC()
                                HideCovAVMM()
                                HideCovBEC()
                                HideCovBVMM()
                                HideCovCEC()
                                HideCovCVMM()
                                HideCovDEC()
                                HideCovDVMM()
                                inclCount += 4
                            Case Else
                                ShowCoverageA()
                                ShowCoverageB()
                                ShowCoverageC()
                                ShowCoverageD(CurrentForm)
                                policyCount += 4
                                If (.Locations(0).FormTypeId = "9" Or .Locations(0).FormTypeId = "10") Then
                                    ShowCovAEC()
                                    ShowCovAVMM()
                                    ShowCovBEC()
                                    ShowCovBVMM()
                                    ShowCovCEC()
                                    ShowCovCVMM()
                                    ShowCovDEC()
                                    ShowCovDVMM()
                                    If (.Locations(0).FormTypeId = "9") Then
                                        inclCount += 4
                                    Else
                                        inclCount += 8
                                    End If
                                ElseIf (.Locations(0).FormTypeId = "11" Or .Locations(0).FormTypeId = "12") Then
                                    lblIncludedCoverage.Visible = False
                                End If

                                'cjs if VMM Included if formtype includeds VMM else value is No
                                If lblFormData.Text = "DP 00 01 - Fire, EC and V&MM" Or lblFormData.Text = "DP 00 02 - Broad" Or lblFormData.Text = "DP 00 03 - Special" Then
                                    lblVMMData.Text = "Yes"
                                End If
                        End Select

                        '
                        ' Other Property Coverages
                        '
                        If .Locations(0).SectionICoverages IsNot Nothing Then
                            For Each sc As QuickQuoteSectionICoverage In .Locations(0).SectionICoverages
                                Select Case sc.CoverageType
                                    Case QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                                        If lblFormData.Text.Substring(0, 8) <> "DP 00 01" Then
                                            pnlHO_04_81.Visible = True
                                            lblHO_04_81_Limit.Text = "N/A"
                                            lblHO_04_81_Limit.Visible = False
                                            propCount += 1

                                            Dim PremInt As Integer = sc.Premium
                                            If PremInt <> 0 Then
                                                lblHO_04_81_Prem.Text = sc.Premium
                                            Else
                                                lblHO_04_81_Prem.Text = "Included"
                                            End If
                                        End If

                                    Case QuickQuoteSectionICoverage.SectionICoverageType.Earthquake
                                        pnlHO_315B.Visible = True
                                        lblHO_315B_Limit.Text = "N/A"
                                        lblHO_315B_Limit.Visible = False
                                        propCount += 1

                                        Dim PremInt As Integer = sc.Premium
                                        If PremInt <> 0 Then
                                            lblHO_315B_Prem.Text = sc.Premium
                                        Else
                                            lblHO_315B_Prem.Text = "Included"
                                        End If

                                        lblHO_315B_Ded.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, sc.DeductibleLimitId)

                                    Case QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA
                                        pnl92_074A.Visible = True
                                        lbl92_074A_Limit.Text = "N/A"
                                        lbl92_074A_Limit.Visible = False
                                        propCount += 1

                                        Dim PremInt As Integer = sc.Premium
                                        If PremInt <> 0 Then
                                            lbl92_074A_Prem.Text = sc.Premium
                                        Else
                                            lbl92_074A_Prem.Text = "Included"
                                        End If

                                    Case QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB
                                        pnl92_074AB.Visible = True
                                        lbl92_074AB_Limit.Text = "N/A"
                                        lbl92_074AB_Limit.Visible = False
                                        propCount += 1

                                        Dim PremInt As Integer = sc.Premium
                                        If PremInt <> 0 Then
                                            lbl92_074AB_Prem.Text = sc.Premium
                                        Else
                                            lbl92_074AB_Prem.Text = "Included"
                                        End If

                                    Case QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment
                                        If lblFormData.Text.Substring(0, 8) <> "DP 00 01" Then
                                            pnlHO_05_30.Visible = True
                                            lblHO_05_30_Limit.Text = "N/A"
                                            lblHO_05_30_Limit.Visible = False
                                            propCount += 1

                                            Dim PremInt As Integer = sc.Premium
                                            If PremInt <> 0 Then
                                                lblHO_05_30_Prem.Text = sc.Premium
                                            Else
                                                lblHO_05_30_Prem.Text = "Included"
                                            End If
                                        End If

                                    Case QuickQuoteSectionICoverage.SectionICoverageType.SinkholeCollapse
                                        pnlHO_99.Visible = True
                                        lblHO_99_Limit.Text = "N/A"
                                        lblHO_99_Limit.Visible = False
                                        propCount += 1

                                        Dim PremInt As Integer = sc.Premium
                                        If PremInt <> 0 Then
                                            lblHO_99_Prem.Text = sc.Premium
                                        Else
                                            lblHO_99_Prem.Text = "Included"
                                        End If
                                End Select
                            Next

                            'Get Included Limits
                            Dim dtLimits As New DataTable
                            dtLimits.Columns.Add("StructAddress", System.Type.GetType("System.String"))
                            dtLimits.Columns.Add("IncreasedLimit", System.Type.GetType("System.String"))
                            dtLimits.Columns.Add("SpaceFiller", System.Type.GetType("System.String"))
                            dtLimits.Columns.Add("Premium", System.Type.GetType("System.String"))

                            ' Specified Other Structures - Off Premises
                            Dim structDataSource As List(Of QuickQuoteSectionICoverage) = quickQuote.Locations(0).SectionICoverages.FindAll(Function(p) p.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises Or p.CoverageCodeId = "3761")
                            If structDataSource.Count > 0 Then
                                propCount += 1
                            End If
                            For Each struct As QuickQuoteSectionICoverage In structDataSource
                                Dim newRow As DataRow = dtLimits.NewRow
                                Dim zip As String = struct.Address.Zip
                                If zip.Length > 5 Then
                                    zip = zip.Substring(0, 5)
                                End If
                                Dim address As String = String.Format("{0} {1} {2} {3} {4} {5} {6}", struct.Address.HouseNum, struct.Address.StreetName, If(String.IsNullOrWhiteSpace(struct.Address.ApartmentNumber) = False, "Apt# " + struct.Address.ApartmentNumber, ""), struct.Address.POBox, struct.Address.City, struct.Address.State, zip).Replace("  ", " ").Trim()
                                newRow.Item("StructAddress") = address
                                newRow.Item("IncreasedLimit") = struct.IncreasedLimit.ToString
                                newRow.Item("Premium") = struct.Premium.ToString
                                dtLimits.Rows.Add(newRow)
                                pnl92_127.Visible = True
                                propCount += 1
                            Next

                            dgIncreasedLimits.DataSource = dtLimits
                            dgIncreasedLimits.DataBind()

                        Else
                            'No Optional Property Coverages Selected
                            pnlNoSelectedCoverages.Visible = True
                        End If

                        If .Locations(0).SectionIICoverages IsNot Nothing Then
                            For Each sc In .Locations(0).SectionIICoverages
                                Select Case sc.CoverageType
                                    Case QuickQuoteSectionIICoverage.SectionIICoverageType.Non_OwnerOccupiedDwelling
                                        Me.lblNonOwnerOccupiedDwellingPremium.Text = sc.Premium
                                        pnlNonOwnerOccupiedDwelling.Visible = True
                                End Select

                            Next
                        Else
                            divOtherLiabilityCoverages.Visible = False
                        End If


                        liabCount = 4
                        lblCovELimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, .PersonalLiabilityLimitId, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)

                        Dim covEPrem As Integer = .PersonalLiabilityQuotedPremium
                        If covEPrem <> 0 Then
                            lblCovEPrem.Text = .PersonalLiabilityQuotedPremium
                        Else
                            lblCovEPrem.Text = "Included"
                        End If

                        liabCount += 1

                        lblCovFLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, .MedicalPaymentsLimitid, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)

                        Dim covFPrem As Integer = .MedicalPaymentsQuotedPremium
                        If covFPrem <> 0 Then
                            lblCovFPrem.Text = .MedicalPaymentsQuotedPremium
                        Else
                            lblCovFPrem.Text = "Included"
                        End If

                        liabCount += 1

                        '
                        ' Applied Credits
                        '
                        Dim dtCreditList As New DataTable
                        dtCreditList.Columns.Add("Credit", System.Type.GetType("System.String"))
                        dtCreditList.Columns.Add("CreditPercent", System.Type.GetType("System.String"))
                        creditCount = 4

                        Dim creditList = IFM.VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(quickQuote, True)

                        If creditList.Count <= 0 Then
                            lblNoCredits.Visible = True
                            creditCount += 1
                        Else
                            creditCount += creditList.Count
                        End If

                        For inx As Integer = 0 To creditList.Count - 1
                            Dim newCreditRow As DataRow = dtCreditList.NewRow

                            newCreditRow.Item("Credit") = creditList(inx).Key
                            newCreditRow.Item("CreditPercent") = creditList(inx).Value
                            dtCreditList.Rows.Add(newCreditRow)
                        Next

                        dgCreditList.DataSource = dtCreditList
                        dgCreditList.DataBind()
                    End If
                    '
                    ' Applied Surcharges
                    '
                    'Dim dtSurchargeList As New DataTable
                    'dtSurchargeList.Columns.Add("Surcharge", System.Type.GetType("System.String"))
                    'dtSurchargeList.Columns.Add("SurchargePercent", System.Type.GetType("System.String"))
                    'surCount = 4

                    'Dim surchargeList = IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMSurcharges(quickQuote)

                    'If surchargeList.Count <= 0 Then
                    '    lblNoSurchargeExist.Visible = True
                    '    surCount += 1
                    'Else
                    '    surCount += surchargeList.Count
                    'End If

                    'For inx As Integer = 0 To surchargeList.Count - 1
                    '    Dim newSurchargeRow As DataRow = dtSurchargeList.NewRow

                    '    newSurchargeRow.Item("Surcharge") = surchargeList(inx).Key
                    '    newSurchargeRow.Item("SurchargePercent") = surchargeList(inx).Value
                    '    dtSurchargeList.Rows.Add(newSurchargeRow)
                    'Next

                    'dgSurchargeList.DataSource = dtSurchargeList
                    'dgSurchargeList.DataBind()

                    '
                    ' Payment Plan
                    '
                    Dim totalPrem As String = .TotalQuotedPremium

                    If totalPrem <> "" AndAlso IsNumeric(totalPrem) = True Then
                        Me.ctlPaymentOptions.DirectAnnualDown = totalPrem
                        Me.ctlPaymentOptions.DirectMonthlyInstallFee = "3"
                        Me.ctlPaymentOptions.DirectSemiAnnualDown = qqHelper.getDivisionQuotient(totalPrem, "2")
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectSemiAnnualDown)
                        Me.ctlPaymentOptions.DirectQuarterlyDown = qqHelper.getDivisionQuotient(totalPrem, "4")
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectQuarterlyDown)
                        Me.ctlPaymentOptions.DirectMonthlyDown = qqHelper.getSum(qqHelper.getDivisionQuotient(totalPrem, "12"), Me.ctlPaymentOptions.DirectMonthlyInstallFee) 'may also need to include installment fee
                        'Me.ctlPaymentOptions.DirectMonthlyDown = qqHelper.getDivisionQuotient(totalPrem, "12") 'may also need to include installment fee
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

                    PageBreaks(lblFormData.Text.Substring(0, 4), policyCount, inclCount, propCount, liabCount, imCount, rvwCount, creditCount, surCount)
                End If
            End With
        End If
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

    Private Sub PageBreaks(form As String, policyCoverage As Integer, includedCoverage As Integer, otherPropCoverage As Integer, liabCoverage As Integer, imCoverage As Integer, rvwCoverage As Integer, creditCount As Integer, surCount As Integer)
        Dim linesOnPage As Integer = 54
        Dim formTotal As Integer = policyCoverage + includedCoverage + otherPropCoverage

        If formTotal > linesOnPage Then
            PropBreak.Visible = True
            PropLine.Visible = False
            formTotal = 0
        End If

        If formTotal = linesOnPage Then
            LiabBreak.Visible = True
            formTotal = 0
        End If

        formTotal += liabCoverage
        If formTotal >= linesOnPage Then
            LiabBreak.Visible = True
            formTotal = liabCoverage
        End If

        'If formTotal = linesOnPage Then
        '    IMBreak.Visible = True
        '    formTotal = 0
        'End If

        'formTotal += imCoverage
        'If quickQuote.Locations(0).InlandMarines.Count > 0 Then
        '    If formTotal > linesOnPage Then
        '        IMBreak.Visible = True
        '        imCoverage = formTotal - linesOnPage
        '        formTotal = imCoverage
        '    End If
        'Else
        '    If (formTotal > linesOnPage) Or ((formTotal - linesOnPage) > imCoverage) Then
        '        IMBreak.Visible = True
        '        formTotal = 0
        '    End If
        'End If

        'If formTotal = linesOnPage Then
        '    RVWBreak.Visible = True
        '    formTotal = 0
        'End If

        'formTotal += rvwCoverage
        'If quickQuote.Locations(0).RvWatercrafts.Count > 0 Then
        '    If formTotal > linesOnPage Then
        '        rvwCoverage = formTotal - linesOnPage

        '        If (rvwCoverage - 8) <= 0 And (rvwCoverage - 8) > -2 Then
        '            RVWBreak.Visible = True
        '        End If

        '        formTotal = rvwCoverage
        '    End If

        If formTotal = linesOnPage Then
            AdjBreak.Visible = True
            formTotal = 0
        End If
        'Else
        ''If (formTotal > linesOnPage) Or ((formTotal - linesOnPage) > rvwCoverage) Then
        ''    RVWBreak.Visible = True
        ''End If
        'formTotal = 0
        'End If

        If surCount > creditCount Then
            formTotal += surCount
        Else
            formTotal += creditCount
        End If

        If formTotal > linesOnPage Then
            AdjBreak.Visible = True

            If surCount > creditCount Then
                formTotal = surCount
            Else
                formTotal = creditCount
            End If
        End If

        If formTotal = linesOnPage Then
            AdjBreak.Visible = True
            formTotal = 0
        End If

        formTotal += 3
        If formTotal > linesOnPage Then
            AdjBreak.Visible = True
        End If
    End Sub
    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                If htmlBytes IsNot Nothing Then
                    fileName = String.Format("SUMMARY{0}.pdf", Me.quickQuote.QuoteNumber)
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
                                Dim fs_pdf As New FileStream(pdfPath, FileMode.Open, FileAccess.Read)
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
                                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("SUMMARY{0}.pdf", Me.quickQuote.QuoteNumber))
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
End Class