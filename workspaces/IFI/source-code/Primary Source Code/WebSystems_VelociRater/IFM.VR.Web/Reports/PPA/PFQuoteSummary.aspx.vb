Imports System.Data
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports System.Xml
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.PPA
Imports IFM.VR.Web.Helpers
Imports PublicQuotingLib.Models
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Public Class PFQuoteSummary
    Inherits System.Web.UI.Page

    'This control is only used for PPA, so no multi state changes are needed 9/17/18 MLW

    Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim summaryHelper As New SummaryHelperClass
    Dim fileName As String = ""
    Dim quoteIds As List(Of String)
    Dim noSurchargesExist As Boolean = True

    'Added 7/26/2019 for Auto Endorsements Task 32783 MLW
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

    'added 2/21/2019
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
            'Me.lblQuoteId.Text = Request.QueryString("QuoteId").ToString
            'updated 5/10/2019
            Me.lblQuoteId.Text = If(Request.QueryString("QuoteId") IsNot Nothing, Request.QueryString("QuoteId").ToString, "")
            'Dim summaryType As String = ""
            'If (RatedQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse RatedQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) Then
            'Else
            '    summaryType = Request.QueryString("summarytype").ToString
            'End If

            Dim summaryType As String = Request.QueryString("summarytype").ToString
            Dim reportHeader As StringBuilder = New StringBuilder
            reportHeader.Append("Indiana Farmers Mutual Insurance Group ")

            'If summaryType = "App" Then
            '    reportHeader.Append("Application Summary")
            'Else
            '    reportHeader.Append("Quote Summary")
            'End If
            'updated 5/10/2019
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                reportHeader.Append("Image Summary")
                If String.IsNullOrWhiteSpace(Me.lblSubHeader.Text) = False AndAlso Len(Me.lblSubHeader.Text) > 6 AndAlso Right(Me.lblSubHeader.Text, 6) = " QUOTE" Then
                    Me.lblSubHeader.Text = Left(Me.lblSubHeader.Text, Len(Me.lblSubHeader.Text) - 6)
                End If
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                reportHeader.Append("Change Summary")
            Else
                reportHeader.Append(If(String.IsNullOrWhiteSpace(summaryType) = False AndAlso UCase(summaryType) = "APP", "Application", "Quote") & " Summary")
            End If

            lblHeader.Text = reportHeader.ToString
            GetQuoteFromDb(summaryType)
            LoadSummaryObjects()
            'If VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.quickQuote) Then
            'updated 2/21/2019
            If quickQuote IsNot Nothing AndAlso VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.quickQuote) Then
                Me.lblMultiPercent.Text = "applied"
                Me.lblCreditPercent.Text = "applied"
                Me.lblGoodPercent.Text = "applied"
                Me.lblMultiVehPercent.Text = "applied"

                Me.lblOOSPercent.Text = "applied"
                Me.lblInexpDrvrPercent.Text = "applied"

                Me.lblMultPolicy.Text = "Auto/Home"
                Me.pnlOOS.Visible = False
                Me.pnlInexpDrvr.Visible = False
            End If
        End If
    End Sub

    Private Sub LoadSummaryObjects()
        If quickQuote IsNot Nothing Then 'added IF 2/21/2019
            'B45083 Change from Client to PolicyHolder "Prepared For:"
            'Me.VR3Proposal_ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(quickQuote.Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), quickQuote.Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
            'Me.VR3Proposal_ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(Me.VR3Proposal_ClientAndAgencyInfo.ClientInfo, quickQuote.Client.PrimaryPhone, "<br />")

            Dim Name As String = quickQuote.Policyholder.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />")
            If quickQuote?.Policyholder2?.Name?.DisplayNameForWeb IsNot Nothing AndAlso quickQuote.Policyholder2.Name.DisplayNameForWeb.IsNullEmptyorWhitespace() = False Then
                Name &= "<br />" & quickQuote.Policyholder2.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />")
            End If
            Me.VR3Proposal_ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(Name, quickQuote.Policyholder.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
            Me.VR3Proposal_ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(Me.VR3Proposal_ClientAndAgencyInfo.ClientInfo, quickQuote.Policyholder.PrimaryPhone, "<br />")

            Me.VR3Proposal_ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(summaryHelper.GetAgencyName(quickQuote.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), quickQuote.Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
            'Dim trimPrimaryPhone As String = quickQuote.Agency.PrimaryPhone.Trim.Remove(quickQuote.Agency.PrimaryPhone.Length - 2)
            'Me.VR3Proposal_ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.VR3Proposal_ClientAndAgencyInfo.AgencyInfo, trimPrimaryPhone, "<br />")
            'updated 9/19/2017
            Me.VR3Proposal_ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.VR3Proposal_ClientAndAgencyInfo.AgencyInfo, quickQuote.Agency.PrimaryPhone, "<br />")
            Me.VR3Proposal_ClientAndAgencyInfo.ProducerCode = quickQuote.AgencyProducerCode
        End If
    End Sub

    Public Sub GetQuoteFromDb(summaryType As String)
        Dim autoFinalize As Boolean = False
        Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
        Dim val_item_counter As Integer = 0 'added 4/11/2013
        Dim noPolicyDiscounts As Boolean = True

        'If IsNumeric(Me.lblQuoteId.Text) = True Then 'removed IF 2/21/2019
        'Dim errorMsg As String = ""
        'Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
        'QQxml.GetRatedQuote(Me.lblQuoteId.Text, quickQuote, rateType, errorMsg)
        'updated 2/21/2019; note: rateType should be set by method, though none of these have the param as ByRef
        quickQuote = RatedQuote
        If quickQuote IsNot Nothing Then
            With quickQuote
                'Policy Information
                'Me.lblQuoteNumber.Text = .QuoteNumber
                'Me.lblDate.Text = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().ToShortDateString()
                'Me.lblEffectiveDate.Text = .EffectiveDate
                'Me.lblPremium.Text = .TotalQuotedPremium
                'updated 5/10/2019
                Me.lblDate.Text = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().ToShortDateString()
                Me.lblPremium.Text = .TotalQuotedPremium
                Me.lblPaymentPlanType.Text = IFM.VR.Common.Helpers.PPA.PPA_Payplans.GetCurrentPayPlanName(quickQuote)
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
                    Me.OldHeaderSection.Visible = False
                    Me.OldQuoteNumberSection.Visible = False
                    Me.OldClientAndAgencySection.Visible = False
                    Me.OldPremSection.Visible = False
                    Me.OldEffDateSection.Visible = False
                    Me.OldPayPlanTypeSection.Visible = False
                    Me.ctlEndorsementOrChangeHeader.Visible = True
                    Me.ctlEndorsementOrChangeHeader.Quote = quickQuote
                End If

                'Updated 7/26/2019 for Auto Endorsements Task 32783 MLW
                If IsBillingUpdate() Then
                    quoteSummaryDetailsContent.Visible = False
                Else

                    'Policy Coverages
                    Me.lblAutoEnhancePrem.Text = .BusinessMasterEnhancementQuotedPremium

                    If QuickQuoteHelperClass.IsValidEffectiveDateForAutoPlusEnhancement(quickQuote.EffectiveDate) Then
                        Me.trAutoPlusEnhance.Visible = True
                        Me.lblAutoPlusEnhancePrem.Text = .AutoPlusEnhancement_QuotedPremium
                    Else
                        Me.trAutoPlusEnhance.Visible = False
                    End If

                    'Policy Discounts
                    If .AutoHome Then
                        Me.pnlMuliPolicy.Visible = True
                        noPolicyDiscounts = False
                    Else
                        Me.pnlMuliPolicy.Visible = False
                    End If

                    pnlMultiLine.Visible = .MultiLineDiscount.TryToGetInt32() > 4 'Parachute 4 = 0, 5 = 1, 6 = 2,7 = 3+

                    If .SelectMarketCredit Then
                        Me.pnlMarketCredit.Visible = True
                        noPolicyDiscounts = False
                    Else
                        Me.pnlMarketCredit.Visible = False
                    End If

                    ' Added 'Advanced Quote Discount' per bug 30754  MGB 4/1/2019
                    If .HasAdvancedQuoteDiscount Then
                        Me.pnlAdvancedQuoteDiscount.Visible = True
                        noPolicyDiscounts = False
                    Else
                        Me.pnlAdvancedQuoteDiscount.Visible = False
                    End If

                    'Added 6/18/2019 Pay Plan Discount per bug 31002 MLW
                    Dim creditList = VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(quickQuote, False)
                    If creditList.Count <= 0 Then
                        Me.pnlPayPlanDiscount.Visible = False
                    Else
                        For inx As Integer = 0 To creditList.Count - 1
                            If creditList(inx).Key = "Pay Plan Discount" AndAlso creditList(inx).Value < 1 Then
                                Me.pnlPayPlanDiscount.Visible = True
                                noPolicyDiscounts = False
                                Exit For
                            Else
                                Me.pnlPayPlanDiscount.Visible = False
                            End If
                        Next
                    End If




                    If .Drivers IsNot Nothing Then
                        'Checks each driver to see if any have "Good Student" or "Defensive Driver" selected
                        For Each item As QuickQuote.CommonObjects.QuickQuoteDriver In .Drivers
                            If item.GoodStudent Then
                                pnlStudent.Visible = True
                                noPolicyDiscounts = False
                            End If
                        Next

                        If .Vehicles IsNot Nothing Then
                            'Checks to see if multiple vehicles are insured, excluding "Other Trailer" & "Rec Trailer"
                            If .Vehicles.Count > 1 Then
                                Dim vehicleCnt As Integer = 0

                                For Each item As QuickQuote.CommonObjects.QuickQuoteVehicle In .Vehicles
                                    If qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, item.BodyTypeId) <> "Rec. Trailer" And
                                       qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, item.BodyTypeId) <> "Other Trailer" Then
                                        vehicleCnt += 1
                                    End If
                                Next

                                'If vehicleCnt > 1 Then
                                pnlMuliVehicle.Visible = True
                                noPolicyDiscounts = False
                                'Else
                                '    pnlMuliVehicle.Visible = False
                                'End If
                            Else
                                pnlMuliVehicle.Visible = False
                            End If

                            If noPolicyDiscounts Then
                                pnlNoDiscounts.Visible = True
                            End If

                            'Get Drivers
                            Dim dtDrivers As New DataTable
                            dtDrivers.Columns.Add("DriverNum", System.Type.GetType("System.String"))
                            dtDrivers.Columns.Add("DriverName", System.Type.GetType("System.String"))
                            dtDrivers.Columns.Add("DOB", System.Type.GetType("System.String"))
                            dtDrivers.Columns.Add("VehicleNum", System.Type.GetType("System.String"))
                            dtDrivers.Columns.Add("Discounts", System.Type.GetType("System.String"))

                            'Get Accidents
                            Dim dtAccidents As New DataTable
                            dtAccidents.Columns.Add("TotalSurcharge", System.Type.GetType("System.String"))
                            dtAccidents.Columns.Add("DriverNum", System.Type.GetType("System.String"))
                            dtAccidents.Columns.Add("LossDate", System.Type.GetType("System.String"))

                            'Get Violations
                            Dim dtViolations As New DataTable
                            dtViolations.Columns.Add("TotalSurcharge", System.Type.GetType("System.String"))
                            dtViolations.Columns.Add("DriverNum", System.Type.GetType("System.String"))
                            dtViolations.Columns.Add("ViolationDate", System.Type.GetType("System.String"))

                            Dim driverCnt As Integer = 0
                            For Each driver As QuickQuoteDriver In .Drivers
                                driverCnt += 1
                                Dim newRow As DataRow = dtDrivers.NewRow
                                newRow.Item("DriverNum") = driverCnt.ToString

                                If driver.Name IsNot Nothing Then
                                    newRow.Item("DriverName") = driver.Name.DisplayName.ToString

                                    If qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, driver.DriverExcludeTypeId) <> "Excluded" Then
                                        newRow.Item("DOB") = driver.Name.BirthDate.ToString
                                    Else
                                        newRow.Item("DOB") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, driver.DriverExcludeTypeId)
                                    End If
                                End If

                                'Lists each vehicle number that the driver is assigned to
                                Dim vehicleNum As StringBuilder = New StringBuilder
                                Dim prtPrimaryFlag As Boolean = True
                                Dim prtOptFlag As Boolean = True
                                Dim assignedVehicle As Integer = 0
                                For Each vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle In .Vehicles
                                    Try
                                        If driverCnt = vehicle.PrincipalDriverNum Then
                                            If prtPrimaryFlag Then
                                                vehicleNum.Append("P:")
                                                prtPrimaryFlag = False
                                            End If
                                            DetermineAssignedVehicleNumber(.Vehicles, vehicle.VehicleNum, assignedVehicle)
                                            vehicleNum.Append(assignedVehicle)
                                            vehicleNum.Append(",")
                                        End If
                                    Catch
                                    End Try

                                    Try
                                        If driverCnt = vehicle.OccasionalDriver1Num Then
                                            If prtOptFlag Then
                                                vehicleNum.Append("O:")
                                                prtOptFlag = False
                                            End If
                                            DetermineAssignedVehicleNumber(.Vehicles, vehicle.VehicleNum, assignedVehicle)
                                            vehicleNum.Append(assignedVehicle)
                                            vehicleNum.Append(",")
                                        End If
                                    Catch
                                    End Try

                                    Try
                                        If driverCnt = vehicle.OccasionalDriver2Num Then
                                            If prtOptFlag Then
                                                vehicleNum.Append("O:")
                                                prtOptFlag = False
                                            End If
                                            DetermineAssignedVehicleNumber(.Vehicles, vehicle.VehicleNum, assignedVehicle)
                                            vehicleNum.Append(assignedVehicle)
                                            vehicleNum.Append(",")
                                        End If
                                    Catch
                                    End Try

                                    Try
                                        If driverCnt = vehicle.OccasionalDriver3Num Then
                                            If prtOptFlag Then
                                                vehicleNum.Append("O:")
                                                prtOptFlag = False
                                            End If
                                            DetermineAssignedVehicleNumber(.Vehicles, vehicle.VehicleNum, assignedVehicle)
                                            vehicleNum.Append(assignedVehicle)
                                            vehicleNum.Append(",")
                                        End If
                                    Catch
                                    End Try
                                Next

                                Dim vehNum As String = vehicleNum.ToString.Trim.TrimEnd(",")
                                newRow.Item("VehicleNum") = vehNum

                                CheckPolicySurcharges(driver, driverCnt, dtAccidents, dtViolations, summaryType, quickQuote.EffectiveDate, quickQuote.Vehicles)
                                newRow.Item("Discounts") = CheckDriverDiscounts(driver)
                                dtDrivers.Rows.Add(newRow)
                            Next

                            dgDrivers.DataSource = dtDrivers
                            dgDrivers.DataBind()
                        End If
                    End If

                    'Payment Plan
                    Dim totalPrem As String = .TotalQuotedPremium

                    If totalPrem <> "" AndAlso IsNumeric(totalPrem) = True Then
                        Me.VR3Proposal_PaymentOptions.DirectMonthlyInstallFee = "3"
                        If Me.quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.quickQuote) Then
                            ' for DirectBill only -  12 = annual 2, 13 = semi-annual 2, 14 = quarterly 2, 15 = monthly 2,18 = renewal credit card monthly 2, 19 = renewal eft monthly 2, 
                            Dim options = IFM.VR.Common.Helpers.PPA.PPA_Payplans.GetPaymentOptions(Me.quickQuote) '7-25-18

                            Me.VR3Proposal_PaymentOptions.DirectAnnualDown = (From o In options Where o.PayPlanId = 12 Select o).First().DownPayment
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectAnnualDown)
                            Me.VR3Proposal_PaymentOptions.DirectAnnualBasicInstall = "N/A"
                            'Added 6/14/2019 for Bug 31360 MLW
                            Me.VR3Proposal_PaymentOptions.DirectAnnualPremiumTotal = (From o In options Where o.PayPlanId = 12 Select o).First().TotalPayments
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectAnnualPremiumTotal)

                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualDown = (From o In options Where o.PayPlanId = 13 Select o).First().DownPayment
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectSemiAnnualDown)
                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualBasicInstall = Me.VR3Proposal_PaymentOptions.DirectSemiAnnualDown
                            'Added 6/14/2019 for Bug 31360 MLW
                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualPremiumTotal = (From o In options Where o.PayPlanId = 13 Select o).First().TotalPayments
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectSemiAnnualPremiumTotal)

                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyDown = (From o In options Where o.PayPlanId = 14 Select o).First().DownPayment
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectQuarterlyDown)
                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyBasicInstall = Me.VR3Proposal_PaymentOptions.DirectQuarterlyDown
                            'Added 6/14/2019 for Bug 31360 MLW
                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyPremiumTotal = (From o In options Where o.PayPlanId = 14 Select o).First().TotalPayments
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectQuarterlyPremiumTotal)

                            Me.VR3Proposal_PaymentOptions.DirectMonthlyDown = qqHelper.getSum((From o In options Where o.PayPlanId = 15 Select o).First().DownPayment, Me.VR3Proposal_PaymentOptions.DirectMonthlyInstallFee)
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectMonthlyDown)
                            Me.VR3Proposal_PaymentOptions.DirectMonthlyBasicInstall = Me.VR3Proposal_PaymentOptions.DirectMonthlyDown
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectMonthlyBasicInstall)
                            'Added 6/14/2019 for Bug 31360 MLW
                            Me.VR3Proposal_PaymentOptions.DirectMonthlyPremiumTotal = (From o In options Where o.PayPlanId = 15 Select o).First().TotalPayments
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectMonthlyPremiumTotal)

                            Me.VR3Proposal_PaymentOptions.RenewalCreditDown = (From o In options Where o.PayPlanId = 18 Select o).First().DownPayment
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.RenewalCreditDown)
                            Me.VR3Proposal_PaymentOptions.RenewalCreditRemainInstall = Me.VR3Proposal_PaymentOptions.DirectMonthlyRemainInstall
                            Me.VR3Proposal_PaymentOptions.RenewalCreditBasicInstall = Me.VR3Proposal_PaymentOptions.RenewalCreditDown
                            'Added 6/14/2019 for Bug 31360 MLW
                            Me.VR3Proposal_PaymentOptions.RenewalCreditPremiumTotal = (From o In options Where o.PayPlanId = 18 Select o).First().TotalPayments
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.RenewalCreditPremiumTotal)

                            Me.VR3Proposal_PaymentOptions.RenewalEftDown = (From o In options Where o.PayPlanId = 19 Select o).First().DownPayment
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.RenewalEftDown)
                            Me.VR3Proposal_PaymentOptions.RenewalEftRemainInstall = Me.VR3Proposal_PaymentOptions.DirectMonthlyRemainInstall
                            Me.VR3Proposal_PaymentOptions.RenewalEftBasicInstall = Me.VR3Proposal_PaymentOptions.RenewalEftDown
                            'Added 6/14/2019 for Bug 31360 MLW
                            Me.VR3Proposal_PaymentOptions.RenewalEftPremiumTotal = (From o In options Where o.PayPlanId = 19 Select o).First().TotalPayments
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.RenewalEftPremiumTotal)

                            Me.VR3Proposal_PaymentOptions.DirectAnnualRemainInstall = "N/A"
                            Me.VR3Proposal_PaymentOptions.DirectMonthlyRemainInstall = "11"
                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyRemainInstall = "3"
                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualRemainInstall = "1"
                            Me.VR3Proposal_PaymentOptions.RenewalCreditRemainInstall = "11"
                            Me.VR3Proposal_PaymentOptions.RenewalEftRemainInstall = "11"

                            If StatePoliceFundHelper.IsStatePoliceFundLabelAvailable(quickQuote) Then
                                lblAnnualPremiumWithFees.Style.Add("display", "")
                                lblAnnualPremiumWithFeesText.Style.Add("display", "")
                                Dim totalFees = 0
                                For Each vehicle As QuickQuoteVehicle In .Vehicles
                                    Dim policeTrainingFeeCoverage As QuickQuoteCoverage = vehicle.Coverages.Where(Function(coverage) coverage.CoverageCodeId = 100014).FirstOrDefault()
                                    If policeTrainingFeeCoverage IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(policeTrainingFeeCoverage.FullTermPremium) Then
                                        totalFees += Convert.ToDouble(policeTrainingFeeCoverage.FullTermPremium)
                                    End If
                                Next
                                lblAnnualPremiumWithFees.Text = options.Where(Function(opt) opt.PayPlanId = 12).FirstOrDefault().DownPayment + totalFees
                                qqHelper.ConvertToQuotedPremiumFormat(lblAnnualPremiumWithFees.Text)
                            Else
                                lblAnnualPremiumWithFees.Style.Add("display", "none")
                                lblAnnualPremiumWithFeesText.Style.Add("display", "none")
                            End If
                        Else
                            Me.VR3Proposal_PaymentOptions.DirectAnnualDown = totalPrem
                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualDown = qqHelper.getDivisionQuotient(totalPrem, "2")
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectSemiAnnualDown)
                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyDown = qqHelper.getDivisionQuotient(totalPrem, "4")
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectQuarterlyDown)
                            Me.VR3Proposal_PaymentOptions.DirectMonthlyDown = qqHelper.getSum(qqHelper.getDivisionQuotient(totalPrem, "12"), Me.VR3Proposal_PaymentOptions.DirectMonthlyInstallFee) 'may also need to include installment fee
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectMonthlyDown)
                            Me.VR3Proposal_PaymentOptions.RenewalCreditDown = Me.VR3Proposal_PaymentOptions.DirectMonthlyDown
                            Me.VR3Proposal_PaymentOptions.RenewalEftDown = Me.VR3Proposal_PaymentOptions.DirectMonthlyDown

                            Me.VR3Proposal_PaymentOptions.RenewalCreditRemainInstall = Me.VR3Proposal_PaymentOptions.DirectMonthlyRemainInstall
                            Me.VR3Proposal_PaymentOptions.RenewalEftRemainInstall = Me.VR3Proposal_PaymentOptions.DirectMonthlyRemainInstall

                            Me.VR3Proposal_PaymentOptions.DirectAnnualBasicInstall = "N/A"
                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualBasicInstall = Me.VR3Proposal_PaymentOptions.DirectSemiAnnualDown
                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyBasicInstall = Me.VR3Proposal_PaymentOptions.DirectQuarterlyDown

                            Me.VR3Proposal_PaymentOptions.DirectMonthlyBasicInstall = Me.VR3Proposal_PaymentOptions.DirectMonthlyDown 'may also need to include installment fee
                            qqHelper.ConvertToQuotedPremiumFormat(Me.VR3Proposal_PaymentOptions.DirectMonthlyBasicInstall)
                            Me.VR3Proposal_PaymentOptions.RenewalCreditBasicInstall = Me.VR3Proposal_PaymentOptions.RenewalCreditDown
                            Me.VR3Proposal_PaymentOptions.RenewalEftBasicInstall = Me.VR3Proposal_PaymentOptions.RenewalEftDown

                            Me.VR3Proposal_PaymentOptions.DirectAnnualRemainInstall = "N/A"
                            Me.VR3Proposal_PaymentOptions.DirectMonthlyRemainInstall = "11"
                            Me.VR3Proposal_PaymentOptions.DirectQuarterlyRemainInstall = "3"
                            Me.VR3Proposal_PaymentOptions.DirectSemiAnnualRemainInstall = "1"
                            Me.VR3Proposal_PaymentOptions.RenewalCreditRemainInstall = "11"
                            Me.VR3Proposal_PaymentOptions.RenewalEftRemainInstall = "11"
                        End If

                    End If
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

    'Since Diamond hides vehicles instead of deleting them, this will determine what the vehicle number actually is. Without doing this,
    'it is possible to have 2 vehicles when Diamond vehicle numbers of 5 & 6 instead of 1 & 2
    Private Sub DetermineAssignedVehicleNumber(vehicles As List(Of QuickQuote.CommonObjects.QuickQuoteVehicle), vehicleNum As String, ByRef assignedVehicle As Integer)
        Dim vehicleCnt As Integer = 1
        For Each item As QuickQuote.CommonObjects.QuickQuoteVehicle In vehicles
            If item.VehicleNum = vehicleNum Then
                Exit For
            End If
            vehicleCnt += 1
        Next

        assignedVehicle = vehicleCnt
    End Sub

    Private Function CheckDriverDiscounts(driverObject As QuickQuote.CommonObjects.QuickQuoteDriver) As String
        Dim driverDiscounts As StringBuilder = New StringBuilder
        Dim insertComma As Boolean = False

        If driverObject IsNot Nothing Then
            With driverObject
                If .DistantStudent Then
                    driverDiscounts.Append("Distant Student")
                    insertComma = True
                Else
                    insertComma = False
                End If

                If .MotorcycleTrainingDisc Then
                    If insertComma Then
                        driverDiscounts.Append(", ")
                    Else
                        insertComma = True
                    End If
                    driverDiscounts.Append("Motorcycle Training")
                End If

                If .GoodStudent Then
                    If insertComma Then
                        driverDiscounts.Append(", ")
                    Else
                        insertComma = True
                    End If
                    driverDiscounts.Append("Good Student")
                End If

                If .Name IsNot Nothing Then
                    ' Check Age of Policyholder 1
                    If .Name.BirthDate <> "" And .Name.BirthDate IsNot Nothing Then
                        Try
                            Dim birthDate As Date = .Name.BirthDate
                            Dim ddDate As Date = .DefDriverDate
                            Dim todayDate As Date = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate()

                            Dim span As TimeSpan = todayDate.Subtract(birthDate)
                            Dim ddSpan As TimeSpan = todayDate.Subtract(ddDate)

                            If ((span.Duration().Days / 365) >= 55) And ((ddSpan.Duration().Days / 365) >= 55 And ddDate <> CDate("1/1/1800")) Then
                                If insertComma Then
                                    driverDiscounts.Append(", ")
                                Else
                                    insertComma = True
                                End If
                                driverDiscounts.Append("Mature Driver")
                            End If
                        Catch ex As Exception

                        End Try

                        'If .DefDriverDate > "1/1/1800" Then
                        If .DefDriverDate > CDate("1/1/1900") Then
                            If insertComma Then
                                driverDiscounts.Append(", ")
                            Else
                                insertComma = True
                            End If
                            driverDiscounts.Append("Defensive Driver")
                        End If
                    End If
                End If
            End With
        End If

        Return driverDiscounts.ToString
    End Function

    Private Sub CheckPolicySurcharges(ByRef driver As QuickQuote.CommonObjects.QuickQuoteDriver, driverCnt As Integer, dtAccidents As DataTable, dtViolations As DataTable, summaryType As String, policyEffectDate As String, vehicle As List(Of QuickQuote.CommonObjects.QuickQuoteVehicle))
        'Dim noSurchargesExist As Boolean = True

        'Check to see if vehicle is kept out of state
        Dim oosFound As Boolean = False
        If Not VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.quickQuote) Then
            For Each item As QuickQuote.CommonObjects.QuickQuoteVehicle In vehicle
                If item.DriverOutOfStateSurcharge Then
                    pnlOOS.Visible = True
                    noSurchargesExist = False
                End If
            Next
        End If


        With driver
            'Check for Inexperienced Driver
            If .Name.BirthDate = "" Or .Name.DriversLicenseDate = "" Then
                Return
            End If

            Try
                If Not VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.quickQuote) Then
                    Dim birthDate As Date = .Name.BirthDate
                    Dim dlDate As Date = .Name.DriversLicenseDate
                    Dim policyDate As Date = policyEffectDate

                    Dim span As TimeSpan = policyDate.Subtract(birthDate)
                    Dim tempDate As String = span.Duration().Days / 365
                    Dim dlSpan As TimeSpan = policyDate.Subtract(dlDate)

                    If (span.Duration().Days / 365) >= 25 And (dlSpan.Duration().Days / 365) < 3 And dlDate <> "01/01/1800" Then
                        pnlInexpDrvr.Visible = True
                        noSurchargesExist = False
                    End If
                End If
            Catch ex As Exception

            End Try

            '
            'Check for Violations & Loss History
            '
            Dim lossNumber As Integer = -1
            Dim lossRecords As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord) = New List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)
            Dim violationRecords As List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation) = New List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation)
            Dim minorViolationRecords As List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation) = New List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation)
            Dim minorViolationRecordsPre As List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation) = New List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation)
            Dim childRestraintRecords As List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation) = New List(Of QuickQuote.CommonObjects.QuickQuoteAccidentViolation)
            Dim todaysDate As Date = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate()
            Dim rateBookDate As Date = Date.Parse("03/24/2014")
            Dim policyEffDate As Date = Date.Parse(policyEffectDate)
            Dim violationSurchargeCnt As Integer = 0
            Dim accidentSurchargeCnt As Integer = 0
            Dim childRestraintCnt As Integer = 0

            '
            'Check loss history
            '
            For Each loss As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In .LossHistoryRecords
                If loss.LossHistorySurchargeId <> 0 Then
                    Dim span = policyEffDate.Subtract(loss.LossDate)

                    'Checks to see if loss is older than 3 years
                    If (span.Duration().Days / 365) < 3 And loss.LossHistorySurchargeId <> 2 Then
                        lossRecords.Add(loss)
                        noSurchargesExist = False
                        accidentSurchargeCnt += 2
                    End If
                End If
            Next

            '
            'Check each violation returned from Diamond
            '
            Dim minorViolationHold As StringBuilder = New StringBuilder
            Dim childRestraintHold As StringBuilder = New StringBuilder
            Dim AccViol_ChildRestraint_TypeId As String = "47" 'Failure to use Child Restraint System
            Dim AccViol_OtherMoving_TypeId As String = "21" 'All other Moving violations
            Dim AccViol_Financial_TypeId As String = "35" 'Financial Responsibility (Unacceptable before 3/24/14 -- No Surcharge after)
            Dim AccViol_NoSurchargeList As New List(Of Integer)
            AccViol_NoSurchargeList.Add(5) 'All other Non-moving violations
            AccViol_NoSurchargeList.Add(6) 'License Restriction
            AccViol_NoSurchargeList.Add(42) 'Not at Fault Accident
            AccViol_NoSurchargeList.Add(35) 'Financial Responsibility (Unacceptable); may not include in this list since it would already be lumped in w/ Majors
            AccViol_NoSurchargeList.Add(36) 'MVR Record Clear
            AccViol_NoSurchargeList.Add(37) 'MVR Record Not Found
            AccViol_NoSurchargeList.Add(38) 'Unassigned MVR Code
            AccViol_NoSurchargeList.Add(44) 'No valid license
            AccViol_NoSurchargeList.Add(45) 'License violation
            AccViol_NoSurchargeList.Add(46) 'Violation on a probationary license
            AccViol_NoSurchargeList.Add(48) 'Texting while Driving
            AccViol_NoSurchargeList.Add(49) 'Use of Telecomm Device while operating
            AccViol_NoSurchargeList.Add(50) 'Carrying unsecured passengers in open area of vehicle

            For Each violation As QuickQuote.CommonObjects.QuickQuoteAccidentViolation In .AccidentViolations
                Dim incidentDate As Date = violation.AvDate
                Dim span = policyEffDate.Subtract(incidentDate)
                Dim postDate As Boolean = False

                'Checks to see if the Incident Date is after the Rate Book date
                If Date.Compare(incidentDate.ToShortDateString, rateBookDate.ToShortDateString) >= 0 Then
                    postDate = True
                End If

                Dim violationType As String = violation.AccidentsViolationsCategory

                If postDate And (AccViol_OtherMoving_TypeId = violation.AccidentsViolationsTypeId Or AccViol_Financial_TypeId = violation.AccidentsViolationsTypeId) Then
                    violationType = "Minor Violation"
                End If

                'Checks to see if violation is older than 3 years
                If (span.Duration().Days / 365) < 3 Then
                    Select Case violationType
                        Case "Major Violation"
                            violationRecords.Add(violation)
                            violationSurchargeCnt += 2
                        Case "Unacceptable"
                            violationRecords.Add(violation)

                            If postDate Then
                                violationSurchargeCnt += 10
                            Else
                                violationSurchargeCnt += 2
                            End If

                        Case "Minor Violation"
                            If Not AccViol_NoSurchargeList.Contains(CInt(violation.AccidentsViolationsTypeId)) Then
                                If postDate Then
                                    violationRecords.Add(violation)

                                    If AccViol_ChildRestraint_TypeId = violation.AccidentsViolationsTypeId Then
                                        childRestraintRecords.Add(violation)

                                        If childRestraintRecords.Count Mod 2 = 0 Then
                                            violationSurchargeCnt += 2
                                            pnlChildRestraintSurchargeMsg.Visible = True
                                        End If
                                        childRestraintHold.Append(violation.AvDate)
                                        childRestraintHold.Append(",")
                                    Else
                                        violationSurchargeCnt += 1
                                    End If
                                Else
                                    If AccViol_ChildRestraint_TypeId <> violation.AccidentsViolationsTypeId Then
                                        minorViolationRecordsPre.Add(violation)
                                        minorViolationRecords.Add(violation)
                                        violationRecords.Add(violation)

                                        'Counts 2 minor violations as a major
                                        If minorViolationRecordsPre.Count Mod 2 = 0 Then
                                            violationSurchargeCnt += 2
                                            pnlSurchargeMsg.Visible = True
                                        End If

                                        minorViolationHold.Append(violation.AvDate)
                                        minorViolationHold.Append(",")
                                    End If
                                End If
                            End If
                    End Select
                End If
            Next

            'Place minor dates into a hold area
            Dim violationHoldStr As String = ""
            If minorViolationHold.Length > 0 Then
                violationHoldStr = minorViolationHold.ToString.Trim.Remove(minorViolationHold.Length - 1)
            End If

            'Place child restraint dates into a hold area
            Dim childRestraintHoldStr As String = ""
            If childRestraintHold.Length > 0 Then
                childRestraintHoldStr = childRestraintHold.ToString.Trim.Remove(childRestraintHold.Length - 1)
            End If

            'Check Violations
            If violationRecords.Count > 0 Then
                If violationSurchargeCnt <> 0 Then
                    'List Drivers
                    Dim newViolation As DataRow = dtViolations.NewRow
                    'Updated 8/15/2019 for Bug 28480 MLW
                    'newViolation.Item("TotalSurcharge") = (violationSurchargeCnt * 0.1).ToString("P0")
                    newViolation.Item("TotalSurcharge") = "applied"
                    newViolation.Item("DriverNum") = driverCnt.ToString

                    'List Violations
                    Dim violationDate As StringBuilder = New StringBuilder

                    For Each violation As QuickQuote.CommonObjects.QuickQuoteAccidentViolation In violationRecords
                        violationDate.Append(violation.AvDate)
                        violationDate.Append(",")
                        noSurchargesExist = False
                    Next

                    Dim violationDateStr As String = ""
                    If violationDate.Length > 0 Then
                        violationDateStr = violationDate.ToString.Trim.Remove(violationDate.Length - 1)
                    End If

                    Dim allViolationDatesSorted As Array = violationDateStr.Split(",")

                    'Sort as actual date fields to ensure chronological sorting
                    violationDateStr = ""
                    Dim arrDates As New List(Of Date)

                    For minorDate As Integer = 0 To allViolationDatesSorted.Length - 1
                        arrDates.Add(allViolationDatesSorted(minorDate))
                    Next

                    arrDates.Sort()

                    For Each minorDate As Date In arrDates
                        violationDateStr = violationDateStr & minorDate & ","
                    Next

                    If violationDateStr.Length > 0 Then
                        violationDateStr = violationDateStr.ToString.Trim.Remove(violationDateStr.Length - 1)
                    End If

                    Dim allViolationsSorted() As String = violationDateStr.Split(",")
                    Dim preMinorViolations() As String = violationHoldStr.Split(",")
                    Dim childRestraintViolations() As String = childRestraintHoldStr.Split(",")
                    Dim truncateViolationDate As StringBuilder = New StringBuilder
                    Dim deleteMinorDate As String = ""
                    Dim deleteChildDate As String = ""
                    Dim removeIndex As Integer = 0

                    '
                    'Remove odd earliest odd minor date occuring before 3/24/2014
                    '
                    If preMinorViolations.Count Mod 2 = 1 Then
                        preMinorViolations.Reverse()
                        For Each removeDate As String In preMinorViolations
                            If removeIndex >= (preMinorViolations.Count - 1) Then
                                deleteMinorDate = removeDate
                            End If

                            removeIndex += 1
                        Next
                    End If

                    Array.Sort(preMinorViolations)

                    Dim dateIndex As Integer = 0
                    Dim minorIndex As Integer = 0
                    For Each minorDate As String In allViolationsSorted
                        Try
                            If preMinorViolations.ToArray.Contains(minorDate) Then
                                For searchIndex As Integer = 0 To preMinorViolations.Length - 1
                                    If StrComp(minorDate, preMinorViolations(searchIndex)) = 0 Then
                                        minorIndex = searchIndex
                                        preMinorViolations(searchIndex) = minorDate.Remove(0)
                                        Exit For
                                    End If
                                Next
                                If StrComp(deleteMinorDate, allViolationsSorted(dateIndex)) = 0 Then
                                    allViolationsSorted(dateIndex) = minorDate.Remove(0)
                                Else
                                    allViolationsSorted(dateIndex) = minorDate.Insert(0, "*")
                                End If

                                minorIndex += 1
                            End If

                            dateIndex += 1
                        Catch
                            Exit For
                        End Try
                    Next

                    '
                    'Remove odd earliest odd child restraint date occuring after 3/24/2014
                    '
                    removeIndex = 0
                    If childRestraintViolations.Count Mod 2 = 1 Then
                        childRestraintViolations.Reverse()
                        For Each removeDate As String In childRestraintViolations
                            If removeIndex >= (childRestraintViolations.Count - 1) Then
                                deleteChildDate = removeDate
                            End If

                            removeIndex += 1
                        Next
                    End If

                    Array.Sort(childRestraintViolations)

                    dateIndex = 0
                    minorIndex = 0
                    For Each childDate As String In allViolationsSorted
                        Try
                            If childRestraintViolations.ToArray.Contains(childDate) Then
                                For searchIndex As Integer = 0 To childRestraintViolations.Length - 1
                                    If StrComp(childDate, childRestraintViolations(searchIndex)) = 0 Then
                                        minorIndex = searchIndex
                                        childRestraintViolations(searchIndex) = childDate.Remove(0)
                                        Exit For
                                    End If
                                Next
                                If StrComp(deleteMinorDate, allViolationsSorted(dateIndex)) = 0 Then
                                    allViolationsSorted(dateIndex) = childDate.Remove(0)
                                Else
                                    allViolationsSorted(dateIndex) = childDate.Insert(0, "**")
                                End If

                                minorIndex += 1
                            End If

                            dateIndex += 1
                        Catch
                            Exit For
                        End Try
                    Next

                    violationDateStr = String.Join(", ", allViolationsSorted.Where(Function(s) Not String.IsNullOrEmpty(s)))

                    newViolation.Item("ViolationDate") = violationDateStr
                    dtViolations.Rows.Add(newViolation)
                End If
            End If

            'Check Losses
            If lossRecords.Count > 0 Then
                If accidentSurchargeCnt <> 0 Then
                    'List Drivers
                    Dim newLoss As DataRow = dtAccidents.NewRow
                    'Updated 8/15/2019 for Bug 28480 MLW
                    'newLoss.Item("TotalSurcharge") = (accidentSurchargeCnt * 0.1).ToString("P0")
                    newLoss.Item("TotalSurcharge") = "applied"
                    newLoss.Item("DriverNum") = driverCnt.ToString

                    'List Accidents
                    Dim lossDate As StringBuilder = New StringBuilder
                    pnlAV.Visible = True

                    For Each loss As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In lossRecords
                        lossDate.Append(loss.LossDate)
                        lossDate.Append(",")
                        noSurchargesExist = False
                    Next

                    Dim lossDateStr As String = ""
                    If lossDate.Length > 0 Then
                        lossDateStr = lossDate.ToString.Trim.Remove(lossDate.Length - 1)
                    End If

                    Dim sortedAccidents() As String
                    sortedAccidents = lossDateStr.Split(",")
                    Array.Sort(sortedAccidents)
                    lossDateStr = String.Join(", ", sortedAccidents)

                    newLoss.Item("LossDate") = lossDateStr
                    dtAccidents.Rows.Add(newLoss)
                End If
            End If
        End With

        If dtAccidents.Rows.Count > 0 Then
            pnlAccidents.Visible = True
        End If
        dgAccSurcharge.DataSource = dtAccidents
        dgAccSurcharge.DataBind()

        If dtViolations.Rows.Count > 0 Then
            pnlDriverViolations.Visible = True
        End If
        dgViolations.DataSource = dtViolations
        dgViolations.DataBind()

        If dtViolations.Rows.Count > 0 And dtAccidents.Rows.Count > 0 Then
            hrLine.Visible = True
        End If

        pnlNoSurchargeExist.Visible = noSurchargesExist
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                If htmlBytes IsNot Nothing Then
                    'fileName = String.Format("SUMMARY{0}.pdf", Me.quickQuote.QuoteNumber)
                    'updated 5/10/2019; removed .pdf 5/24/2019
                    fileName = String.Format("SUMMARY{0}", If(String.IsNullOrWhiteSpace(Me.lblQuoteNumber.Text) = False, Me.lblQuoteNumber.Text, Me.quickQuote.QuoteNumber))
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
                                        'Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("SUMMARY{0}.pdf", Me.quickQuote.QuoteNumber))
                                        'updated 5/24/2019
                                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("SUMMARY{0}.pdf", If(String.IsNullOrWhiteSpace(Me.lblQuoteNumber.Text) = False, Me.lblQuoteNumber.Text, Me.quickQuote.QuoteNumber)))
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

    Private Sub dgDrivers_ItemCreated(sender As Object, e As DataGridItemEventArgs) Handles dgDrivers.ItemCreated
        If VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(Me.quickQuote) Then
            e.Item.Cells(3).Visible = False
        End If
    End Sub


End Class